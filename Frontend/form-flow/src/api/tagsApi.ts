import axios from "axios";
import { ENV } from "../config/env";
import { TagDto, PaginatedResponse, TemplateDto } from "../shared/api_types";

const API_BASE_URL = ENV.API_URL;

export interface CreateTagRequest {
	name: string;
}

export interface TagStatsResponse {
	totalTags: number;
	usedTags: number;
	unusedTags: number;
	topTags: Array<{ name: string; usageCount: number }>;
	averageUsage: number;
}

class TagsApi {
	async getTags(
		page: number = 1,
		pageSize: number = 50
	): Promise<PaginatedResponse<TagDto>> {
		const response = await axios.get<PaginatedResponse<TagDto>>(
			`${API_BASE_URL}/tag`,
			{
				params: { page, pageSize },
			}
		);
		return response.data;
	}

	async searchTags(query: string, limit: number = 10): Promise<TagDto[]> {
		const response = await axios.get<TagDto[]>(`${API_BASE_URL}/tag/search`, {
			params: { q: query, limit },
		});
		return response.data;
	}

	async getTagCloud(maxTags: number = 50): Promise<TagDto[]> {
		const response = await axios.get<TagDto[]>(`${API_BASE_URL}/tag/cloud`, {
			params: { maxTags },
		});
		return response.data;
	}

	async getPopularTags(count: number = 20): Promise<TagDto[]> {
		const response = await axios.get<TagDto[]>(`${API_BASE_URL}/tag/popular`, {
			params: { count },
		});
		return response.data;
	}

	async getTemplatesByTag(
		tagName: string,
		page: number = 1,
		pageSize: number = 20
	): Promise<PaginatedResponse<TemplateDto>> {
		const response = await axios.get<PaginatedResponse<TemplateDto>>(
			`${API_BASE_URL}/tag/${encodeURIComponent(tagName)}/templates`,
			{
				params: { page, pageSize },
			}
		);
		return response.data;
	}

	async getTagById(id: string): Promise<TagDto> {
		const response = await axios.get<TagDto>(`${API_BASE_URL}/tag/${id}`);
		return response.data;
	}

	async getTagByName(tagName: string): Promise<TagDto> {
		const response = await axios.get<TagDto>(
			`${API_BASE_URL}/tag/by-name/${encodeURIComponent(tagName)}`
		);
		return response.data;
	}

	async getTagsForAutocomplete(
		query: string,
		limit: number = 10
	): Promise<string[]> {
		const response = await axios.get<string[]>(
			`${API_BASE_URL}/tag/autocomplete`,
			{
				params: { q: query, limit },
			}
		);
		return response.data;
	}

	async createTag(
		request: CreateTagRequest,
		accessToken: string
	): Promise<TagDto> {
		const response = await axios.post<TagDto>(`${API_BASE_URL}/tag`, request, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
		return response.data;
	}

	async updateTag(
		id: string,
		request: CreateTagRequest,
		accessToken: string
	): Promise<TagDto> {
		const response = await axios.put<TagDto>(
			`${API_BASE_URL}/tag/${id}`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async deleteTag(id: string, accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/tag/${id}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}

	async cleanupUnusedTags(accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/tag/cleanup`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}

	async recalculateTagUsage(id: string, accessToken: string): Promise<TagDto> {
		const response = await axios.post<{ tag: TagDto }>(
			`${API_BASE_URL}/tag/${id}/recalculate-usage`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data.tag;
	}

	async getTagStats(accessToken: string): Promise<TagStatsResponse> {
		const response = await axios.get<TagStatsResponse>(
			`${API_BASE_URL}/tag/stats`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}
}

export const tagsApi = new TagsApi();
