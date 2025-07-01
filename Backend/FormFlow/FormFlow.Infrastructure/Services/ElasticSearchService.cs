using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.SearchService;
using Elasticsearch.Net;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General.QuestionDetailsModels;
using Nest;
using Newtonsoft.Json;
using FormFlow.Domain.Models.General;
using Newtonsoft.Json.Serialization;

namespace FormFlow.Infrastructure.Services
{
    public class ElasticSearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ITemplateRepository _templateRepository;
        private const string IndexName = "templates";

        public ElasticSearchService(IElasticClient elasticClient, ITemplateRepository templateRepository)
        {
            _elasticClient = elasticClient;
            _templateRepository = templateRepository;
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
                Query = new BoolQuery
                {
                    Should = new QueryContainer[]
                    {
                        new MultiMatchQuery
                        {
                            Query = query,
                            Fields = new[] { "title^4", "topic^3", "description^2", "questionsText", "authorName" },
                            Type = TextQueryType.PhrasePrefix
                        },
                        new TermsQuery
                        {
                            Field = "tags",
                            Terms = new[] { query.ToLower() }
                        },
                        new WildcardQuery
                        {
                            Field = "tags",
                            Value = $"*{query.ToLower()}*"
                        },
                        new WildcardQuery
                        {
                            Field = "topic",
                            Value = $"*{query.ToLower()}*"
                        },
                        new WildcardQuery
                        {
                            Field = "title",
                            Value = $"*{query.ToLower()}*"
                        },
                    },
                    MustNot = new QueryContainer[]
                    {
                        new TermQuery
                        {
                            Field = "isDeleted",
                            Value = true
                        }
                    },
                    MinimumShouldMatch = 1
                },

                Size = limit,
                Source = new SourceFilter
                {
                    Includes = new[] { "tags", "title", "topic", "description" }
                }
            };

            var response = await _elasticClient.SearchAsync<TemplateSearchDocument>(searchRequest);

            if (response.ApiCall.HttpStatusCode != 200)
            {
                throw new Exception(response.ApiCall.OriginalException.Message);
            }

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

            int page = 1;
            int elementOnPage = 5;

            //var templates = await _templateRepository.GetPublicTemplatesPagedAsync(page, elementOnPage);
            //while(templates.Pagination.HasNext)
            //{
            //    foreach(var template in templates.Data)
            //    {
            //        var searchDocument = await BuildTemplateSearchDocumentAsync(template);
            //        await IndexTemplateAsync(searchDocument);
            //    }
            //    page++;
            //    templates = await _templateRepository.GetPublicTemplatesPagedAsync(page, elementOnPage);
            //}

            do
            {
                var templates = await _templateRepository.GetPublicTemplatesPagedAsync(page, elementOnPage);

                foreach (var template in templates.Data)
                {
                    var searchDocument = await BuildTemplateSearchDocumentAsync(template);
                    await IndexTemplateAsync(searchDocument);
                }

                if (!templates.Pagination.HasNext)
                    break;

                page++;
            } while (true);
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
                            .Name(n => n.Topic)
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

            if (query.Topic != null)
            {
                queries.Add(new TermQuery
                {
                    Field = "topic",
                    Value = query.Topic
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
                SearchSortBy.Date => new List<ISort> { new FieldSort { Field = "createdAt", Order = SortOrder.Descending } },
                SearchSortBy.Popularity => new List<ISort> { new FieldSort { Field = "formsCount", Order = SortOrder.Descending } },
                SearchSortBy.Title => new List<ISort> { new FieldSort { Field = "title.keyword", Order = SortOrder.Ascending } },
                SearchSortBy.Relevance => new List<ISort> { new FieldSort { Field = "_score", Order = SortOrder.Descending } },
                _ => new List<ISort> { new FieldSort { Field = "_score", Order = SortOrder.Descending } }
            };
        }



        private async Task<TemplateSearchDocument> BuildTemplateSearchDocumentAsync(FormFlow.Domain.Models.General.Template template)
        {
            var questionsText = template.Questions?
                .Where(q => !q.IsDeleted)
                .Select(q =>
                {
                    try
                    {
                        //var questionDetails = JsonConvert.DeserializeObject<QuestionDetails>(q.Data, new JsonSerializerSettings
                        var questionDetails = JsonConvert.DeserializeObject<dynamic>(q.Data, new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                        return $"{questionDetails?.title} {questionDetails?.description}";
                    }
                    catch
                    {
                        return "";
                    }
                })
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Aggregate("", (current, text) => current + " " + text)
                .Trim() ?? "";

            var commentsText = template.Comments?
                .Where(c => !c.IsDeleted)
                .Select(c => c.Content)
                .Where(content => !string.IsNullOrWhiteSpace(content))
                .Aggregate("", (current, content) => current + " " + content)
                .Trim() ?? "";

            var topicNames = await _templateRepository.GetTemplateTopicsAsync(new List<Guid> { template.Id });
            var topicName = topicNames.GetValueOrDefault(template.Id, "Other");

            return new TemplateSearchDocument
            {
                Id = template.Id,
                Title = template.Title,
                Topic = topicName,
                Description = template.Description,
                QuestionsText = questionsText,
                CommentsText = commentsText,
                AuthorName = template.Author?.UserName ?? "",
                Tags = template.Tags == null ? new List<string>() : template.Tags.Select(tt => tt.Tag.Name).ToList(),
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                FormsCount = template.FormsCount,
                LikesCount = template.LikesCount,
                CommentsCount = template.CommentsCount,
                IsArchived = template.IsArchived,
                IsPublished = template.IsPublished,
                IsDeleted = template.IsDeleted
            };
        }
    }
}
