import { UserDto } from '../../../shared/api_types';

export interface RegisterRequest {
	userName: string;
	email: string;
	password: string;
}

export interface LoginRequest {
	email: string;
	password: string;
}

export interface GoogleLoginRequest {
	code: string;
}

export interface RefreshTokenRequest {
	refreshToken: string;
}

export interface LogoutRequest {
	refreshToken?: string;
	logoutFromAllDevices?: boolean;
}

export interface ValidateTokenRequest {
	token: string;
}

export interface CheckEmailRequest {
	email: string;
}

export interface CheckUsernameRequest {
	userName: string;
}

export interface AuthResponse {
	user: UserDto;
	accessToken: string;
	refreshToken: string;
	accessTokenExpiry: string;
	refreshTokenExpiry: string;
	authType: string;
}

export interface RefreshTokenResponse {
	accessToken: string;
	refreshToken: string;
	accessTokenExpiry: string;
	refreshTokenExpiry: string;
}

export interface ValidateTokenResponse {
	isValid: boolean;
	claims?: any;
}

export interface ErrorResponse {
	message: string;
}

export interface SuccessResponse {
	message: string;
}

export interface CheckExistsResponse {
	exists: boolean;
}

export interface AuthState {
	user: UserDto | null;
	accessToken: string | null;
	refreshToken: string | null;
	accessTokenExpiry: string | null;
	refreshTokenExpiry: string | null;
	authType: string | null;
	isAuthenticated: boolean;
	isLoading: boolean;
	error: string | null;
	redirectPath: string | null;
}

export interface AuthActions {
	login: (credentials: LoginRequest) => Promise<void>;
	register: (credentials: RegisterRequest) => Promise<void>;
	googleLogin: (code: string) => Promise<void>;
	logout: (options?: LogoutRequest) => Promise<void>;
	refreshTokens: () => Promise<void>;
	validateToken: (token: string) => Promise<boolean>;
	clearAuth: () => void;
	clearError: () => void;
	setRedirectPath: (path: string | null) => void;
	checkEmailExists: (email: string) => Promise<boolean>;
	checkUsernameExists: (username: string) => Promise<boolean>;
}
