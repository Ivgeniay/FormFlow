import axios from "axios";
import { ENV } from "../config/env";
import { PaginatedResponse } from "../shared/api_types";

const API_BASE_URL = ENV.API_URL;

export interface LikeDto {
	id: string;
	templateId: string;
	userId: string;
	userName: string;
	createdAt: string;
}

export interface LikeStatusResponse {
	isLiked: boolean;
	totalLikes: number;
}

class LikesApi {
	async getLikesByTemplate(
		templateId: string,
		page: number = 1,
		pageSize: number = 20
	): Promise<PaginatedResponse<LikeDto>> {
		const response = await axios.get<PaginatedResponse<LikeDto>>(
			`${API_BASE_URL}/like/template/${templateId}`,
			{
				params: { page, pageSize },
			}
		);
		return response.data;
	}

	async getLikeStatus(
		templateId: string,
		accessToken: string
	): Promise<LikeStatusResponse> {
		const response = await axios.get<LikeStatusResponse>(
			`${API_BASE_URL}/like/template/${templateId}/status`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async hasUserLiked(
		templateId: string,
		accessToken: string
	): Promise<boolean> {
		const response = await axios.get<{ isLiked: boolean }>(
			`${API_BASE_URL}/like/template/${templateId}/user-liked`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data.isLiked;
	}

	async getLikesCount(templateId: string): Promise<number> {
		const response = await axios.get<{ count: number }>(
			`${API_BASE_URL}/like/template/${templateId}/count`
		);
		return response.data.count;
	}
}

export const likesApi = new LikesApi();
