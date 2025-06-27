import axios from "axios";
import { CommentDto, PaginatedResponse } from "../shared/api_types";
import { ENV } from "../config/env";

const API_BASE_URL = ENV.API_URL;

export interface AddCommentRequest {
	templateId: string;
	content: string;
}

class CommentApi {
	async getCommentsByTemplate(
		templateId: string,
		page: number = 1,
		pageSize: number = 20
	): Promise<PaginatedResponse<CommentDto>> {
		const response = await axios.get<PaginatedResponse<CommentDto>>(
			`${API_BASE_URL}/comment/template/${templateId}`,
			{
				params: { page, pageSize },
			}
		);
		return response.data;
	}

	async getRecentComments(
		templateId: string,
		count: number = 10
	): Promise<CommentDto[]> {
		const response = await axios.get<CommentDto[]>(
			`${API_BASE_URL}/comment/template/${templateId}/recent`,
			{
				params: { count },
			}
		);
		return response.data;
	}

	async getComment(commentId: string): Promise<CommentDto> {
		const response = await axios.get<CommentDto>(
			`${API_BASE_URL}/comment/${commentId}`
		);
		return response.data;
	}

	async deleteComment(commentId: string, accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/comment/${commentId}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}
}

export const commentApi = new CommentApi();
