import axios from "axios";
import { ENV } from "../config/env";
import { FormDto, TemplateDto, PaginatedResponse } from "../shared/api_types";

const API_BASE_URL = ENV.API_URL;

export interface SubmitFormRequest {
	templateId: string;
	answers: Record<string, any>;
	sendCopyToEmail: boolean;
}

export interface UpdateFormRequest {
	id: string;
	answers: Record<string, any>;
}

export interface FormAccessResponse {
	canFillForm: boolean;
	hasAlreadySubmitted: boolean;
	denialReason?: string;
	template?: TemplateDto;
	existingForm?: FormDto;
}

export interface FormSubmissionStatus {
	hasSubmitted: boolean;
	templateId: string;
}

class FormApi {
	async submitForm(
		request: SubmitFormRequest,
		accessToken: string
	): Promise<FormDto> {
		const response = await axios.post<FormDto>(
			`${API_BASE_URL}/form`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getForm(id: string, accessToken: string): Promise<FormDto> {
		const response = await axios.get<FormDto>(`${API_BASE_URL}/form/${id}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
		return response.data;
	}

	async updateForm(
		request: UpdateFormRequest,
		accessToken: string
	): Promise<FormDto> {
		const response = await axios.put<FormDto>(
			`${API_BASE_URL}/form/${request.id}`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async deleteForm(id: string, accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/form/${id}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}

	async getFormAccess(
		templateId: string,
		accessToken: string
	): Promise<FormAccessResponse> {
		const response = await axios.get<FormAccessResponse>(
			`${API_BASE_URL}/form/template/${templateId}/access`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getMyForms(
		page: number = 1,
		pageSize: number = 20,
		accessToken: string
	): Promise<PaginatedResponse<FormDto>> {
		const response = await axios.get<PaginatedResponse<FormDto>>(
			`${API_BASE_URL}/form/my`,
			{
				params: { page, pageSize },
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getFormsByTemplate(
		templateId: string,
		page: number = 1,
		pageSize: number = 20,
		accessToken: string
	): Promise<PaginatedResponse<FormDto>> {
		const response = await axios.get<PaginatedResponse<FormDto>>(
			`${API_BASE_URL}/form/template/${templateId}`,
			{
				params: { page, pageSize },
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getMyFormForTemplate(
		templateId: string,
		accessToken: string
	): Promise<FormDto> {
		const response = await axios.get<FormDto>(
			`${API_BASE_URL}/form/template/${templateId}/my`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async hasUserSubmittedForm(
		templateId: string,
		accessToken: string
	): Promise<FormSubmissionStatus> {
		const response = await axios.get<FormSubmissionStatus>(
			`${API_BASE_URL}/form/template/${templateId}/submitted`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}
}

export const formApi = new FormApi();
