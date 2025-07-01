import axios from "axios";
import { ENV } from "../config/env";

const API_BASE_URL = ENV.API_URL;

export interface TopicDto {
	id: string;
	name: string;
	isActive: boolean;
}

export interface PaginatedTopicsResponse {
	data: TopicDto[];
	currentPage: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
	hasNext: boolean;
	hasPrevious: boolean;
}

export interface CreateTopicRequest {
	name: string;
}

export interface UpdateTopicRequest {
	name: string;
}

export interface CheckTopicNameRequest {
	name: string;
}

export interface TopicExistsResponse {
	exists: boolean;
}

class TopicsApi {
	async getTopics(
		page: number = 1,
		pageSize: number = 20
	): Promise<PaginatedTopicsResponse> {
		const response = await axios.get<PaginatedTopicsResponse>(
			`${API_BASE_URL}/topic`,
			{
				params: { page, pageSize },
			}
		);
		return response.data;
	}

	async getTopicsList(count: number = 10): Promise<TopicDto[]> {
		const response = await axios.get<TopicDto[]>(`${API_BASE_URL}/topic/list`, {
			params: { count },
		});
		return response.data;
	}

	async getTopicById(id: string): Promise<TopicDto> {
		const response = await axios.get<TopicDto>(`${API_BASE_URL}/topics/${id}`);
		return response.data;
	}

	async getTopicForAutocomplete(
		query: string,
		limit: number = 10
	): Promise<string[]> {
		const response = await axios.get<string[]>(
			`${API_BASE_URL}/topic/autocomplete`,
			{
				params: { q: query, limit },
			}
		);
		return response.data;
	}

	async createTopic(
		request: CreateTopicRequest,
		accessToken: string
	): Promise<TopicDto> {
		const response = await axios.post<TopicDto>(
			`${API_BASE_URL}/topic`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async updateTopic(
		id: string,
		request: UpdateTopicRequest,
		accessToken: string
	): Promise<TopicDto> {
		const response = await axios.put<TopicDto>(
			`${API_BASE_URL}/topic/${id}`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async deleteTopic(id: string, accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/topics/${id}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}

	async topicExists(id: string): Promise<boolean> {
		const response = await axios.get<TopicExistsResponse>(
			`${API_BASE_URL}/topic/${id}/exists`
		);
		return response.data.exists;
	}

	async checkTopicName(request: CheckTopicNameRequest): Promise<boolean> {
		const response = await axios.post<TopicExistsResponse>(
			`${API_BASE_URL}/topic/check-name`,
			request
		);
		return response.data.exists;
	}
}

export const topicsApi = new TopicsApi();
