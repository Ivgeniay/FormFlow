import axios from "axios";
import { ENV } from "../config/env";

const API_BASE_URL = ENV.API_URL;

export interface SubscriptionStatusResponse {
	templateId: string;
	subscribed: boolean;
}

export interface SubscriptionToggleResponse {
	message: string;
	templateId: string;
	subscribed: boolean;
	action: "subscribed" | "unsubscribed";
}

export interface TemplateSubscriptionDto {
	id: string;
	title: string;
	topicId: string;
	description: string;
	imageUrl?: string;
	authorName: string;
	createdAt: string;
	tags: string[];
	formsCount: number;
	likesCount: number;
}

export interface UserSearchDto {
	id: string;
	userName: string;
	primaryEmail?: string;
}

export interface SubscribersCheckResponse {
	templateId: string;
	hasSubscribers: boolean;
}

class FormSubscribeApi {
	async getSubscriptionStatus(
		templateId: string,
		accessToken: string
	): Promise<SubscriptionStatusResponse> {
		const response = await axios.get<SubscriptionStatusResponse>(
			`${API_BASE_URL}/FormSubscribe/template/${templateId}/status`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async toggleSubscription(
		templateId: string,
		accessToken: string
	): Promise<SubscriptionToggleResponse> {
		const response = await axios.post<SubscriptionToggleResponse>(
			`${API_BASE_URL}/FormSubscribe/template/${templateId}/toggle`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async subscribeToTemplate(
		templateId: string,
		accessToken: string
	): Promise<SubscriptionToggleResponse> {
		const response = await axios.post<SubscriptionToggleResponse>(
			`${API_BASE_URL}/FormSubscribe/template/${templateId}`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async unsubscribeFromTemplate(
		templateId: string,
		accessToken: string
	): Promise<SubscriptionToggleResponse> {
		const response = await axios.delete<SubscriptionToggleResponse>(
			`${API_BASE_URL}/FormSubscribe/template/${templateId}`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getMySubscriptions(
		accessToken: string
	): Promise<TemplateSubscriptionDto[]> {
		const response = await axios.get<TemplateSubscriptionDto[]>(
			`${API_BASE_URL}/FormSubscribe/my`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getTemplateSubscribers(
		templateId: string,
		accessToken: string
	): Promise<UserSearchDto[]> {
		const response = await axios.get<UserSearchDto[]>(
			`${API_BASE_URL}/FormSubscribe/template/${templateId}/subscribers`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async checkTemplateHasSubscribers(
		templateId: string,
		accessToken: string
	): Promise<SubscribersCheckResponse> {
		const response = await axios.get<SubscribersCheckResponse>(
			`${API_BASE_URL}/FormSubscribe/template/${templateId}/has-subscribers`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}
}

export const formSubscribeApi = new FormSubscribeApi();
