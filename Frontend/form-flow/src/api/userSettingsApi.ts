import axios, { AxiosError } from "axios";
import { ENV } from "../config/env";

export interface UserSettingsDto {
	id: string;
	userId: string;
	colorThemeId: string;
	languageId: string;
	createdAt: string;
	updatedAt: string;
	colorTheme: ColorThemeDto;
	language: LanguageDto;
}

export interface ColorThemeDto {
	id: string;
	name: string;
	cssClass: string;
	colorVariables: string;
	isDefault: boolean;
	isActive: boolean;
}

export interface LanguageDto {
	id: string;
	code: string;
	shortCode: string;
	name: string;
	iconURL?: string;
	region: string;
	isDefault: boolean;
	isActive: boolean;
}

export interface DefaultSettingsDto {
	defaultColorTheme: ColorThemeDto;
	defaultLanguage: LanguageDto;
}

export interface ErrorResponse {
	message: string;
}

class UserSettingsApiService {
	private readonly baseUrl = `${ENV.API_URL}/usersettings`;

	private handleError(error: AxiosError): never {
		if (error.response?.data) {
			const errorData = error.response.data as ErrorResponse;
			throw new Error(
				errorData.message || "An error occurred while processing the request"
			);
		}
		throw new Error("Network error. Please check your internet connection");
	}

	async getMySettings(accessToken: string): Promise<UserSettingsDto> {
		try {
			const response = await axios.get<UserSettingsDto>(`${this.baseUrl}`, {
				headers: {
					Authorization: `Bearer ${accessToken}`,
				},
			});
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async getAvailableColorThemes(): Promise<ColorThemeDto[]> {
		try {
			const response = await axios.get<ColorThemeDto[]>(
				`${this.baseUrl}/color-themes`
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async getAvailableLanguages(): Promise<LanguageDto[]> {
		try {
			const response = await axios.get<LanguageDto[]>(`${this.baseUrl}/languages`);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async getDefaults(): Promise<DefaultSettingsDto> {
		try {
			const response = await axios.get<DefaultSettingsDto>(
				`${this.baseUrl}/defaults`
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async setColorTheme(
		colorThemeId: string,
		accessToken: string
	): Promise<UserSettingsDto> {
		try {
			const response = await axios.post<UserSettingsDto>(
				`${this.baseUrl}/color-theme/${colorThemeId}`,
				{},
				{
					headers: {
						Authorization: `Bearer ${accessToken}`,
					},
				}
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async setLanguage(
		languageId: string,
		accessToken: string
	): Promise<UserSettingsDto> {
		try {
			const response = await axios.post<UserSettingsDto>(
				`${this.baseUrl}/language/${languageId}`,
				{},
				{
					headers: {
						Authorization: `Bearer ${accessToken}`,
					},
				}
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async resetToDefaults(accessToken: string): Promise<UserSettingsDto> {
		try {
			const response = await axios.post<UserSettingsDto>(
				`${this.baseUrl}/reset`,
				{},
				{
					headers: {
						Authorization: `Bearer ${accessToken}`,
					},
				}
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async initializeSettings(accessToken: string): Promise<UserSettingsDto> {
		try {
			const response = await axios.post<UserSettingsDto>(
				`${this.baseUrl}/initialize`,
				{},
				{
					headers: {
						Authorization: `Bearer ${accessToken}`,
					},
				}
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}
}

export const userSettingsApi = new UserSettingsApiService();
