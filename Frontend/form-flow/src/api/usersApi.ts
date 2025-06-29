import axios from "axios";
import { ENV } from "../config/env";
import {
	UserDto,
	UserSearchDto,
	PaginatedResponse,
	ContactDto,
	AuthenticationResult,
} from "../shared/api_types";

const API_BASE_URL = ENV.API_URL;

export interface AddContactRequest {
	type: number;
	value: string;
	isPrimary: boolean;
}
export interface UpdateContactRequest {
	id: string;
	type: number;
	value: string;
	isPrimary: boolean;
}

export interface AuthMethodDto {
	type: string;
	isActive: boolean;
}

export interface EmailAuthMethodDto extends AuthMethodDto {
	email: string;
}

export interface GoogleAuthMethodDto extends AuthMethodDto {
	googleId: string;
	email: string;
}

export interface AuthMethodsResponse {
	authMethods: AuthMethodDto[];
}

export interface UserSettingsDto {
	id: string;
	userId: string;
	languageCode: string;
	themeId: string;
	createdAt: string;
	updatedAt: string;
}

export interface UpdateUserSettingsRequest {
	languageCode?: string;
	themeId?: string;
}

export interface UserStatsResponse {
	totalUsers: number;
	activeUsers: number;
	blockedUsers: number;
	adminUsers: number;
	regularUsers: number;
	newUsersThisWeek: number;
	newUsersThisMonth: number;
	usersByRole: Record<string, number>;
	generatedAt: string;
}

export interface UserBlockRequest {
	userId: string;
	reason?: string;
}

export interface UserRoleChangeRequest {
	userId: string;
	newRole: number;
}

class UsersApi {
	async getUsers(
		page: number = 1,
		pageSize: number = 20,
		accessToken: string
	): Promise<PaginatedResponse<UserDto>> {
		const response = await axios.get<PaginatedResponse<UserDto>>(
			`${API_BASE_URL}/user`,
			{
				params: { page, pageSize },
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async searchUsers(
		query: string,
		limit: number = 10,
		accessToken: string
	): Promise<UserSearchDto[]> {
		const response = await axios.get<UserSearchDto[]>(
			`${API_BASE_URL}/user/search`,
			{
				params: { q: query, limit },
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getUserById(id: string, accessToken: string): Promise<UserDto> {
		const response = await axios.get<UserDto>(`${API_BASE_URL}/user/${id}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
		return response.data;
	}

	async getCurrentUserProfile(accessToken: string): Promise<UserDto> {
		const response = await axios.get<UserDto>(`${API_BASE_URL}/user/me`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
		return response.data;
	}

	async getUserStats(accessToken: string): Promise<UserStatsResponse> {
		const response = await axios.get<UserStatsResponse>(
			`${API_BASE_URL}/user/stats`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getUserContacts(accessToken: string): Promise<ContactDto[]> {
		const response = await axios.get<ContactDto[]>(
			`${API_BASE_URL}/user/me/contacts`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async updateUserContact(
		contactId: string,
		request: UpdateContactRequest,
		accessToken: string
	): Promise<ContactDto> {
		const response = await axios.put<ContactDto>(
			`${API_BASE_URL}/user/contacts/${contactId}`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async deleteUserContact(
		contactId: string,
		accessToken: string
	): Promise<void> {
		await axios.delete(`${API_BASE_URL}/user/contacts/${contactId}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}

	async setUserPrimaryContact(
		contactId: string,
		accessToken: string
	): Promise<void> {
		await axios.post(
			`${API_BASE_URL}/user/contacts/${contactId}/set-primary`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
	}

	async getUserSettings(accessToken: string): Promise<UserSettingsDto> {
		const response = await axios.get<UserSettingsDto>(
			`${API_BASE_URL}/user/me/settings`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async updateUserSettings(
		request: UpdateUserSettingsRequest,
		accessToken: string
	): Promise<UserSettingsDto> {
		const response = await axios.put<UserSettingsDto>(
			`${API_BASE_URL}/user/me/settings`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async addUserContact(
		userId: string,
		request: AddContactRequest,
		accessToken: string
	): Promise<ContactDto> {
		const response = await axios.post<ContactDto>(
			`${API_BASE_URL}/user/${userId}/contacts`,
			request,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async blockUser(userId: string, accessToken: string): Promise<void> {
		await axios.post(
			`${API_BASE_URL}/user/${userId}/block`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
	}

	async unblockUser(userId: string, accessToken: string): Promise<void> {
		await axios.post(
			`${API_BASE_URL}/user/${userId}/unblock`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
	}

	async deleteUser(userId: string, accessToken: string): Promise<void> {
		await axios.delete(`${API_BASE_URL}/user/${userId}`, {
			headers: { Authorization: `Bearer ${accessToken}` },
		});
	}

	async promoteToRole(
		accessToken: string,
		role: number
	): Promise<AuthenticationResult> {
		const response = await axios.post<AuthenticationResult>(
			`${API_BASE_URL}/auth/promote-to-role`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}`, role: `${role}` },
			}
		);

		return response.data;
	}

	async toggleAdminRole(userId: string, accessToken: string): Promise<void> {
		await axios.post(
			`${API_BASE_URL}/user/${userId}/toggle-admin`,
			{},
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
	}
}

export const usersApi = new UsersApi();
