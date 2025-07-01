import axios from "axios";
import { ENV } from "../config/env";
import { TemplateDto } from "../shared/api_types";

const API_BASE_URL = ENV.API_URL;

export enum SearchSortBy {
	Relevance = 0,
	Date = 1,
	Popularity = 2,
	Title = 3,
}

export interface SearchRequest {
	q?: string;
	tags?: string[];
	author?: string;
	topic?: string;
	sortBy?: SearchSortBy;
	includeArchived?: boolean;
	createdAfter?: string;
	createdBefore?: string;
	page?: number;
	pageSize?: number;
}

export interface SearchPagination {
	currentPage: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
	hasNext: boolean;
	hasPrevious: boolean;
}

export interface SearchInfo {
	query: string;
	tags: string[];
	author?: string;
	topic?: string;
	sortBy: string;
	dateRange?: {
		from?: string;
		to?: string;
	};
	includeArchived: boolean;
	searchTime: number;
}

export interface SearchResponse {
	templates: TemplateDto[];
	pagination: SearchPagination;
	searchInfo: SearchInfo;
}

export interface AdminSearchRequest extends SearchRequest {
	includeDeleted?: boolean;
	includeUnpublished?: boolean;
}

export interface SearchSuggestionsResponse {
	suggestions: string[];
}

class SearchApi {
	async searchTemplates(request: SearchRequest): Promise<SearchResponse> {
		const params: any = {
			q: request.q || "",
			page: request.page || 1,
			pageSize: request.pageSize || 20,
			sortBy: Number(request.sortBy || SearchSortBy.Relevance),
			includeArchived: request.includeArchived || false,
		};

		if (request.tags && request.tags.length > 0) {
			params.tags = request.tags;
		}

		if (request.author && request.author.trim()) {
			params.author = request.author;
		}

		if (request.topic && request.topic.trim()) {
			params.topic = request.topic;
		}

		if (request.createdAfter) {
			params.createdAfter = request.createdAfter;
		}

		if (request.createdBefore) {
			params.createdBefore = request.createdBefore;
		}

		const response = await axios.get<SearchResponse>(
			`${API_BASE_URL}/search/templates`,
			{
				params,
				paramsSerializer: {
					indexes: null,
				},
			}
		);

		return response.data;
	}

	async adminSearch(
		request: AdminSearchRequest,
		accessToken: string
	): Promise<SearchResponse> {
		const params: any = {
			q: request.q || "",
			page: request.page || 1,
			pageSize: request.pageSize || 20,
			sortBy: Number(request.sortBy || SearchSortBy.Relevance),
			includeArchived: request.includeArchived || true,
			includeDeleted: request.includeDeleted || true,
			includeUnpublished: request.includeUnpublished || true,
		};

		if (request.tags && request.tags.length > 0) {
			params.tags = request.tags;
		}

		if (request.author) {
			params.author = request.author;
		}

		if (request.topic) {
			params.topic = request.topic;
		}
		const response = await axios.get<SearchResponse>(
			`${API_BASE_URL}/search/admin`,
			{
				params,
				paramsSerializer: {
					indexes: null,
				},
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getSearchSuggestions(
		query: string,
		limit: number = 10
	): Promise<string[]> {
		const response = await axios.get<SearchSuggestionsResponse>(
			`${API_BASE_URL}/search/suggestions`,
			{
				params: { q: query, limit },
			}
		);
		return response.data.suggestions;
	}

	async quickSearch(query: string, limit: number = 5): Promise<TemplateDto[]> {
		const response = await axios.get<{ templates: TemplateDto[] }>(
			`${API_BASE_URL}/search/quick`,
			{
				params: { q: query, limit },
			}
		);
		return response.data.templates;
	}

	async reindexTemplates(accessToken: string): Promise<{
		message: string;
		note: string;
		startedAt: string;
		startedBy: string;
	}> {
		const response = await axios.post(
			`${API_BASE_URL}/search/reindex`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}
}

export const searchApi = new SearchApi();
