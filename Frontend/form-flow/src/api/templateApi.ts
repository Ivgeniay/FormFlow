import axios from "axios";
import { ENV } from "../config/env";
import { TemplateDto, PaginatedResponse } from "../shared/api_types";

const API_BASE_URL = ENV.API_URL;

export interface CreateTemplateRequest {
	title: string;
	description: string;
	topicId: string;
	accessType: number;
	tags: string[];
	allowedUserIds: string[];
	questions: QuestionCreateRequest[];
}

export interface UpdateTemplateRequest {
	id: string;
	title: string;
	description: string;
	topicId: string;
	accessType: number;
	tags: string[];
	allowedUserIds: string[];
	questions: QuestionUpdateRequest[];
}

export interface CreateNewVersionRequest {
	baseTemplateId: string;
	title: string;
	description: string;
	topicId: string;
	accessType: number;
	tags: string[];
	allowedUserIds: string[];
	questions: QuestionCreateRequest[];
}

export interface UpdateTemplateTagsRequest {
	tags: string[];
}

export interface UpdateTemplateAllowedUsersRequest {
	allowedUserIds: string[];
}

export interface QuestionCreateRequest {
	order: number;
	showInResults: boolean;
	isRequired: boolean;
	data: string;
}

export interface QuestionUpdateRequest {
	id?: string;
	order: number;
	showInResults: boolean;
	isRequired: boolean;
	data: string;
}

export interface TemplateVersionInfo {
	id: string;
	version: number;
	isCurrentVersion: boolean;
	createdAt: string;
	updatedAt: string;
}

class TemplateApi {
	async createTemplate(
		request: CreateTemplateRequest,
		accessToken: string
	): Promise<TemplateDto> {
		const response = await axios.post<TemplateDto>(
			`${API_BASE_URL}/template`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getTemplate(id: string, accessToken?: string): Promise<TemplateDto> {
		const headers = accessToken
			? { Authorization: `Bearer ${accessToken}` }
			: {};
		const response = await axios.get<TemplateDto>(
			`${API_BASE_URL}/template/${id}`,
			{
				headers,
			}
		);
		return response.data;
	}

	async updateTemplate(
		id: string,
		request: UpdateTemplateRequest,
		accessToken: string
	): Promise<TemplateDto> {
		const response = await axios.put<TemplateDto>(
			`${API_BASE_URL}/template/${id}`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async deleteTemplate(id: string, accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/template/${id}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}

	async publishTemplate(id: string, accessToken: string): Promise<TemplateDto> {
		const response = await axios.post<TemplateDto>(
			`${API_BASE_URL}/template/${id}/publish`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async unpublishTemplate(
		id: string,
		accessToken: string
	): Promise<TemplateDto> {
		const response = await axios.post<TemplateDto>(
			`${API_BASE_URL}/template/${id}/unpublish`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getLatestTemplates(count: number = 10): Promise<TemplateDto[]> {
		const response = await axios.get<TemplateDto[]>(
			`${API_BASE_URL}/template/latest`,
			{
				params: { count },
			}
		);
		return response.data;
	}

	async getTemplatesByTag(
		tagName: string,
		page: number = 1,
		pageSize: number = 20
	): Promise<PaginatedResponse<TemplateDto>> {
		const response = await axios.get<PaginatedResponse<TemplateDto>>(
			`${API_BASE_URL}/template/by-tag/${encodeURIComponent(tagName)}`,
			{
				params: { page, pageSize },
			}
		);
		return response.data;
	}

	async getPopularTemplates(
		count: number = 10,
		page: number = 1
	): Promise<PaginatedResponse<TemplateDto>> {
		const response = await axios.get<PaginatedResponse<TemplateDto>>(
			`${API_BASE_URL}/template/popular`,
			{
				params: { count, page },
			}
		);
		return response.data;
	}

	async getAllTemplatesForAdmin(
		page: number = 1,
		pageSize: number = 20,
		accessToken: string
	): Promise<PaginatedResponse<TemplateDto>> {
		const response = await axios.get<PaginatedResponse<TemplateDto>>(
			`${API_BASE_URL}/template/admin`,
			{
				params: { page, pageSize },
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getTemplateVersions(
		baseTemplateId: string,
		accessToken: string
	): Promise<TemplateDto[]> {
		const response = await axios.get<TemplateDto[]>(
			`${API_BASE_URL}/template/${baseTemplateId}/versions`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getSpecificVersion(
		baseTemplateId: string,
		version: number,
		accessToken?: string
	): Promise<TemplateDto> {
		const headers = accessToken
			? { Authorization: `Bearer ${accessToken}` }
			: {};
		const response = await axios.get<TemplateDto>(
			`${API_BASE_URL}/template/${baseTemplateId}/version/${version}`,
			{
				headers,
			}
		);
		return response.data;
	}

	async createNewVersion(
		id: string,
		request: CreateNewVersionRequest,
		accessToken: string
	): Promise<TemplateDto> {
		const response = await axios.post<TemplateDto>(
			`${API_BASE_URL}/template/${id}/create-version`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getVersionInfo(
		id: string,
		accessToken: string
	): Promise<TemplateVersionInfo> {
		const response = await axios.get<TemplateVersionInfo>(
			`${API_BASE_URL}/template/${id}/version-info`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async updateTemplateTags(
		id: string,
		request: UpdateTemplateTagsRequest,
		accessToken: string
	): Promise<TemplateDto> {
		const response = await axios.put<TemplateDto>(
			`${API_BASE_URL}/template/${id}/tags`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async updateTemplateAllowedUsers(
		id: string,
		request: UpdateTemplateAllowedUsersRequest,
		accessToken: string
	): Promise<TemplateDto> {
		const response = await axios.put<TemplateDto>(
			`${API_BASE_URL}/template/${id}/allowed-users`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}
}

export const templateApi = new TemplateApi();
