import { create } from "zustand";
import { persist } from "zustand/middleware";
import toast from "react-hot-toast";
import { authService } from "../services/authService";
import {
	AuthState,
	AuthActions,
	LoginRequest,
	RegisterRequest,
	LogoutRequest,
} from "../types/authTypes";

interface AuthStore extends AuthState, AuthActions {}

const initialState: AuthState = {
	user: null,
	accessToken: null,
	refreshToken: null,
	accessTokenExpiry: null,
	refreshTokenExpiry: null,
	authType: null,
	isAuthenticated: false,
	isLoading: false,
	error: null,
	redirectPath: null,
};

const createAuthActions = (set: any, get: any): AuthActions => ({
	login: async (credentials: LoginRequest) => {
		set({ isLoading: true, error: null });

		try {
			const response = await authService.login(credentials);

			set({
				user: response.user,
				accessToken: response.accessToken,
				refreshToken: response.refreshToken,
				accessTokenExpiry: response.accessTokenExpiry,
				refreshTokenExpiry: response.refreshTokenExpiry,
				authType: response.authType,
				isAuthenticated: true,
				isLoading: false,
				error: null,
			});

			// toast.success('Successfully logged in');

			const redirectPath = get().redirectPath;
			if (redirectPath) {
				set({ redirectPath: null });
				window.location.href = redirectPath;
			} else {
				window.location.href = "/";
			}
		} catch (error) {
			const errorMessage = error instanceof Error ? error.message : "Login failed";
			set({
				isLoading: false,
				error: errorMessage,
				isAuthenticated: false,
			});
			toast.error(errorMessage);
		}
	},

	register: async (credentials: RegisterRequest) => {
		set({ isLoading: true, error: null });

		try {
			const response = await authService.register(credentials);

			set({
				user: response.user,
				accessToken: response.accessToken,
				refreshToken: response.refreshToken,
				accessTokenExpiry: response.accessTokenExpiry,
				refreshTokenExpiry: response.refreshTokenExpiry,
				authType: response.authType,
				isAuthenticated: true,
				isLoading: false,
				error: null,
			});

			// toast.success('Registration successful');

			const redirectPath = get().redirectPath;
			if (redirectPath) {
				set({ redirectPath: null });
				window.location.href = redirectPath;
			} else {
				window.location.href = "/";
			}
		} catch (error) {
			const errorMessage =
				error instanceof Error ? error.message : "Registration failed";
			set({
				isLoading: false,
				error: errorMessage,
				isAuthenticated: false,
			});
			toast.error(errorMessage);
		}
	},

	googleLogin: async (code: string) => {
		set({ isLoading: true, error: null });

		try {
			const response = await authService.googleLogin({ code });

			set({
				user: response.user,
				accessToken: response.accessToken,
				refreshToken: response.refreshToken,
				accessTokenExpiry: response.accessTokenExpiry,
				refreshTokenExpiry: response.refreshTokenExpiry,
				authType: response.authType,
				isAuthenticated: true,
				isLoading: false,
				error: null,
			});

			// toast.success('Successfully logged in with Google');

			const redirectPath = get().redirectPath;
			if (redirectPath) {
				set({ redirectPath: null });
				window.location.href = redirectPath;
			} else {
				window.location.href = "/";
			}
		} catch (error) {
			const errorMessage =
				error instanceof Error ? error.message : "Google login failed";
			set({
				isLoading: false,
				error: errorMessage,
				isAuthenticated: false,
			});
			toast.error(errorMessage);
		}
	},

	logout: async (options?: LogoutRequest) => {
		const { accessToken, refreshToken } = get();

		if (!accessToken) {
			get().clearAuth();
			return;
		}

		set({ isLoading: true });

		try {
			const logoutData: LogoutRequest = {
				refreshToken: refreshToken || undefined,
				logoutFromAllDevices: options?.logoutFromAllDevices || false,
			};

			await authService.logout(logoutData, accessToken);

			get().clearAuth();
			// toast.success('Successfully logged out');
			window.location.href = "/";
		} catch (error) {
			get().clearAuth();
			toast.error("Logout error, but you have been logged out locally");
			window.location.href = "/";
		}
	},

	refreshTokens: async () => {
		const { refreshToken } = get();

		if (!refreshToken) {
			get().clearAuth();
			throw new Error("No refresh token available");
		}

		try {
			const response = await authService.refreshToken({ refreshToken });

			set({
				accessToken: response.accessToken,
				refreshToken: response.refreshToken,
				accessTokenExpiry: response.accessTokenExpiry,
				refreshTokenExpiry: response.refreshTokenExpiry,
				error: null,
			});
		} catch (error) {
			get().clearAuth();
			throw error;
		}
	},

	validateToken: async (token: string): Promise<boolean> => {
		try {
			const response = await authService.validateToken({ token });
			return response.isValid;
		} catch (error) {
			return false;
		}
	},

	clearAuth: () => {
		set({
			user: null,
			accessToken: null,
			refreshToken: null,
			accessTokenExpiry: null,
			refreshTokenExpiry: null,
			authType: null,
			isAuthenticated: false,
			isLoading: false,
			error: null,
		});
	},

	clearError: () => {
		set({ error: null });
	},

	setRedirectPath: (path: string | null) => {
		set({ redirectPath: path });
	},

	checkEmailExists: async (email: string): Promise<boolean> => {
		try {
			const response = await authService.checkEmailExists({ email });
			return response.exists;
		} catch (error) {
			return false;
		}
	},

	checkUsernameExists: async (username: string): Promise<boolean> => {
		try {
			const response = await authService.checkUsernameExists({
				userName: username,
			});
			return response.exists;
		} catch (error) {
			return false;
		}
	},
});

const persistConfig = {
	name: "auth-storage",
	partialize: (state: AuthStore) => ({
		user: state.user,
		accessToken: state.accessToken,
		refreshToken: state.refreshToken,
		accessTokenExpiry: state.accessTokenExpiry,
		refreshTokenExpiry: state.refreshTokenExpiry,
		authType: state.authType,
		isAuthenticated: state.isAuthenticated,
	}),
};

const createAuthStore = (set: any, get: any): AuthStore => ({
	...initialState,
	...createAuthActions(set, get),
});

export const useAuthStore = create<AuthStore>()(
	persist(createAuthStore, persistConfig)
);
