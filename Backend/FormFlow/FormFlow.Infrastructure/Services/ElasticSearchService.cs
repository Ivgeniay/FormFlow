using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.SearchService;
using Elasticsearch.Net;
using Nest;

namespace FormFlow.Infrastructure.Services
{
    public class ElasticSearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;
        private const string IndexName = "templates";

        public ElasticSearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<SearchResult<TemplateSearchDocument>> SearchTemplatesAsync(SearchQuery query)
        {
            var searchRequest = new SearchRequest<TemplateSearchDocument>(IndexName)
            {
                Query = BuildQuery(query),
                Sort = BuildSort(query.SortBy),
                From = (query.Page - 1) * query.PageSize,
                Size = query.PageSize
            };

            var response = await _elasticClient.SearchAsync<TemplateSearchDocument>(searchRequest);

            if (!response.IsValid)
            {
                throw new Exception($"Elasticsearch error: {response.OriginalException?.Message}");
            }

            return new SearchResult<TemplateSearchDocument>
            {
                Results = response.Documents.ToList(),
                TotalCount = (int)response.Total,
                Page = query.Page,
                PageSize = query.PageSize,
                Query = query.Query,
                SearchTime = TimeSpan.FromMilliseconds(response.Took)
            };
        }

        public async Task<List<string>> GetSearchSuggestionsAsync(string query, int limit = 10)
        {
            var searchRequest = new SearchRequest<TemplateSearchDocument>(IndexName)
            {
                Query = new MultiMatchQuery
                {
                    Query = query,
                    Fields = new[] { "title^2", "description", "questionsText", "authorName" },
                    Type = TextQueryType.BoolPrefix
                },
                Size = limit,
                Source = new SourceFilter
                {
                    Includes = new[] { "title" }
                }
            };

            var response = await _elasticClient.SearchAsync<TemplateSearchDocument>(searchRequest);

            if (!response.IsValid)
            {
                return new List<string>();
            }

            return response.Documents.Select(d => d.Title).Distinct().ToList();
        }

        public async Task IndexTemplateAsync(TemplateSearchDocument document)
        {
            var response = await _elasticClient.IndexAsync(document, i => i
                .Index(IndexName)
                .Id(document.Id)
                .Refresh(Refresh.WaitFor));

            if (!response.IsValid)
            {
                throw new Exception($"Failed to index template: {response.OriginalException?.Message}");
            }
        }

        public async Task UpdateTemplateIndexAsync(TemplateSearchDocument document)
        {
            await IndexTemplateAsync(document);
        }

        public async Task RemoveTemplateFromIndexAsync(Guid templateId)
        {
            var response = await _elasticClient.DeleteAsync<TemplateSearchDocument>(templateId, d => d
                .Index(IndexName)
                .Refresh(Refresh.WaitFor));

            if (!response.IsValid && response.Result != Result.NotFound)
            {
                throw new Exception($"Failed to remove template from index: {response.OriginalException?.Message}");
            }
        }

        public async Task ReindexAllTemplatesAsync()
        {
            var deleteResponse = await _elasticClient.Indices.DeleteAsync(IndexName);

            await CreateIndexAsync();
        }

        private async Task CreateIndexAsync()
        {
            var createIndexResponse = await _elasticClient.Indices.CreateAsync(IndexName, c => c
                .Map<TemplateSearchDocument>(m => m
                    .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Text(t => t
                            .Name(n => n.Title)
                            .Analyzer("standard")
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword"))
                            )
                        )
                        .Text(t => t
                            .Name(n => n.Description)
                            .Analyzer("standard")
                        )
                        .Text(t => t
                            .Name(n => n.QuestionsText)
                            .Analyzer("standard")
                        )
                        .Text(t => t
                            .Name(n => n.CommentsText)
                            .Analyzer("standard")
                        )
                        .Text(t => t
                            .Name(n => n.AuthorName)
                            .Analyzer("standard")
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword"))
                            )
                        )
                        .Keyword(k => k
                            .Name(n => n.Tags)
                        )
                        .Date(d => d
                            .Name(n => n.CreatedAt)
                        )
                        .Date(d => d
                            .Name(n => n.UpdatedAt)
                        )
                        .Number(n => n
                            .Name(nn => nn.FormsCount)
                            .Type(NumberType.Integer)
                        )
                        .Number(n => n
                            .Name(nn => nn.LikesCount)
                            .Type(NumberType.Integer)
                        )
                        .Number(n => n
                            .Name(nn => nn.CommentsCount)
                            .Type(NumberType.Integer)
                        )
                        .Boolean(b => b
                            .Name(n => n.IsArchived)
                        )
                        .Boolean(b => b
                            .Name(n => n.IsPublished)
                        )
                        .Boolean(b => b
                            .Name(n => n.IsDeleted)
                        )
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Failed to create index: {createIndexResponse.OriginalException?.Message}");
            }
        }

        private static QueryContainer BuildQuery(SearchQuery query)
        {
            var queries = new List<QueryContainer>();

            if (!string.IsNullOrWhiteSpace(query.Query))
            {
                queries.Add(new MultiMatchQuery
                {
                    Query = query.Query,
                    Fields = new[] { "title^3", "description^2", "questionsText", "commentsText", "authorName" },
                    Type = TextQueryType.BestFields,
                    Fuzziness = Fuzziness.Auto
                });
            }

            if (query.Tags != null && query.Tags.Any())
            {
                queries.Add(new TermsQuery
                {
                    Field = "tags",
                    Terms = query.Tags
                });
            }

            if (!string.IsNullOrWhiteSpace(query.AuthorName))
            {
                queries.Add(new MatchQuery
                {
                    Field = "authorName",
                    Query = query.AuthorName
                });
            }

            if (query.CreatedAfter.HasValue)
            {
                queries.Add(new DateRangeQuery
                {
                    Field = "createdAt",
                    GreaterThanOrEqualTo = query.CreatedAfter.Value
                });
            }

            if (query.CreatedBefore.HasValue)
            {
                queries.Add(new DateRangeQuery
                {
                    Field = "createdAt",
                    LessThanOrEqualTo = query.CreatedBefore.Value
                });
            }

            if (!query.IncludeDeleted.GetValueOrDefault(false))
            {
                queries.Add(new TermQuery
                {
                    Field = "isDeleted",
                    Value = false
                });
            }

            if (!query.IncludeArchived.GetValueOrDefault(false))
            {
                queries.Add(new TermQuery
                {
                    Field = "isArchived",
                    Value = false
                });
            }

            if (!query.IncludeUnpublished.GetValueOrDefault(false))
            {
                queries.Add(new TermQuery
                {
                    Field = "isPublished",
                    Value = true
                });
            }

            return queries.Any() ? new BoolQuery { Must = queries } : new MatchAllQuery();
        }

        private static IList<ISort> BuildSort(SearchSortBy sortBy)
        {
            return sortBy switch
            {
                SearchSortBy.CreatedDate => new List<ISort> { new FieldSort { Field = "createdAt", Order = SortOrder.Descending } },
                SearchSortBy.FormsCount => new List<ISort> { new FieldSort { Field = "formsCount", Order = SortOrder.Descending } },
                SearchSortBy.LikesCount => new List<ISort> { new FieldSort { Field = "likesCount", Order = SortOrder.Descending } },
                SearchSortBy.Title => new List<ISort> { new FieldSort { Field = "title.keyword", Order = SortOrder.Ascending } },
                SearchSortBy.Relevance => new List<ISort> { new FieldSort { Field = "_score", Order = SortOrder.Descending } },
                _ => new List<ISort> { new FieldSort { Field = "_score", Order = SortOrder.Descending } }
            };
        }
    }
}
