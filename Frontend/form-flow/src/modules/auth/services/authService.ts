import axios, { AxiosError } from 'axios';
import { ENV } from '../../../config/env';
import {
	RegisterRequest,
	LoginRequest,
	GoogleLoginRequest,
	RefreshTokenRequest,
	LogoutRequest,
	ValidateTokenRequest,
	CheckEmailRequest,
	CheckUsernameRequest,
	AuthResponse,
	RefreshTokenResponse,
	ValidateTokenResponse,
	ErrorResponse,
	SuccessResponse,
	CheckExistsResponse,
} from '../types/authTypes';

class AuthService {
	private readonly baseUrl = `${ENV.API_URL}/auth`;

	private handleError(error: AxiosError): never {
		if (error.response?.data) {
			const errorData = error.response.data as ErrorResponse;
			throw new Error(
				errorData.message || 'An error occurred while processing the request'
			);
		}
		throw new Error('Network error. Please check your internet connection');
	}

	async register(data: RegisterRequest): Promise<AuthResponse> {
		try {
			const response = await axios.post<AuthResponse>(
				`${this.baseUrl}/register`,
				data
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async login(data: LoginRequest): Promise<AuthResponse> {
		try {
			const response = await axios.post<AuthResponse>(
				`${this.baseUrl}/login`,
				data
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async googleLogin(data: GoogleLoginRequest): Promise<AuthResponse> {
		try {
			const response = await axios.post<AuthResponse>(
				`${this.baseUrl}/google-login`,
				data
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async refreshToken(data: RefreshTokenRequest): Promise<RefreshTokenResponse> {
		try {
			const response = await axios.post<RefreshTokenResponse>(
				`${this.baseUrl}/refresh`,
				data
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async logout(
		data: LogoutRequest,
		accessToken: string
	): Promise<SuccessResponse> {
		try {
			const response = await axios.post<SuccessResponse>(
				`${this.baseUrl}/logout`,
				data,
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

	async validateToken(
		data: ValidateTokenRequest
	): Promise<ValidateTokenResponse> {
		try {
			const response = await axios.post<ValidateTokenResponse>(
				`${this.baseUrl}/validate`,
				data
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async getCurrentUser(accessToken: string): Promise<any> {
		try {
			const response = await axios.get(`${this.baseUrl}/me`, {
				headers: {
					Authorization: `Bearer ${accessToken}`,
				},
			});
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async checkEmailExists(data: CheckEmailRequest): Promise<CheckExistsResponse> {
		try {
			const response = await axios.post<CheckExistsResponse>(
				`${this.baseUrl}/check-email`,
				data
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	async checkUsernameExists(
		data: CheckUsernameRequest
	): Promise<CheckExistsResponse> {
		try {
			const response = await axios.post<CheckExistsResponse>(
				`${this.baseUrl}/check-username`,
				data
			);
			return response.data;
		} catch (error) {
			this.handleError(error as AxiosError);
		}
	}

	generateGoogleOAuthUrl(): string {
		const params = new URLSearchParams({
			client_id: ENV.GOOGLE_CLIENT_ID,
			redirect_uri: ENV.GOOGLE_REDIRECT_URI,
			scope: 'openid email profile',
			response_type: 'code',
			access_type: 'offline',
			prompt: 'consent',
		});

		return `https://accounts.google.com/o/oauth2/v2/auth?${params.toString()}`;
	}
}

export const authService = new AuthService();
