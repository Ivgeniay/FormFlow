import axios from "axios";
import { ENV } from "../config/env";

const API_BASE_URL = ENV.API_URL;

export interface ApiTokenDto {
	id: string;
	token: string;
	createdAt: string;
	isActive: boolean;
}

export interface GenerateTokenResponse {
	id: string;
	token: string;
	createdAt: string;
	isActive: boolean;
}

class ApiTokenApi {
	async generateToken(accessToken: string): Promise<GenerateTokenResponse> {
		const response = await axios.post<GenerateTokenResponse>(
			`${API_BASE_URL}/apitoken/generate`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getCurrentToken(accessToken: string): Promise<ApiTokenDto | null> {
		try {
			const response = await axios.get<ApiTokenDto>(
				`${API_BASE_URL}/apitoken/current`,
				{
					headers: { Authorization: `Bearer ${accessToken}` },
				}
			);
			return response.data;
		} catch (error: any) {
			if (error.response?.status === 404) {
				return null;
			}
			throw error;
		}
	}

	async getTokenHistory(accessToken: string): Promise<ApiTokenDto[]> {
		const response = await axios.get<ApiTokenDto[]>(
			`${API_BASE_URL}/apitoken/history`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async revokeToken(tokenId: string, accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/apitoken/${tokenId}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}

	async revokeAllTokens(accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/apitoken/revoke-all`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}
}

export const apiTokenApi = new ApiTokenApi();