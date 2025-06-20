import { useMemo } from "react";
import { useAuthStore } from "../store/authStore";
import { authService } from "../services/authService";

export const useAuth = () => {
	const store = useAuthStore();

	const authData = useMemo(
		() => ({
			user: store.user,
			isAuthenticated: store.isAuthenticated,
			isLoading: store.isLoading,
			error: store.error,
			accessToken: store.accessToken,
			authType: store.authType,
		}),
		[store]
	);

	const authActions = useMemo(
		() => ({
			login: store.login,
			register: store.register,
			googleLogin: store.googleLogin,
			logout: store.logout,
			refreshTokens: store.refreshTokens,
			clearAuth: store.clearAuth,
			clearError: store.clearError,
			setRedirectPath: store.setRedirectPath,
			checkEmailExists: store.checkEmailExists,
			checkUsernameExists: store.checkUsernameExists,
		}),
		[store]
	);

	const authHelpers = useMemo(
		() => ({
			isAdmin: Boolean(store.user?.role && store.user.role & 2),
			isModerator: Boolean(store.user?.role && store.user.role & 4),
			isSuperAdmin: Boolean(store.user?.role && store.user.role & 8),
			isUser: Boolean(store.user?.role && store.user.role & 1),
			hasRole: (role: number) =>
				Boolean(store.user?.role && store.user.role & role),
			hasAnyRole: (roles: number[]) =>
				Boolean(
					store.user?.role &&
						roles.some((role) => store.user?.role && store.user.role & role)
				),
			hasAllRoles: (roles: number[]) =>
				Boolean(
					store.user?.role &&
						roles.every((role) => store.user?.role && store.user.role & role)
				),
			getRoleNames: () => {
				if (!store.user?.role) return [];
				const userRole = store.user.role;
				const roleNames: string[] = [];
				if (userRole & 1) roleNames.push("User");
				if (userRole & 2) roleNames.push("Admin");
				if (userRole & 4) roleNames.push("Moderator");
				if (userRole & 8) roleNames.push("SuperAdmin");
				return roleNames;
			},
			isEmailAuth: store.authType === "Internal",
			isGoogleAuth: store.authType === "Google",
			userDisplayName: store.user?.userName || "Unknown User",
			userEmail: store.user?.primaryEmail || "",
			hasTokens: Boolean(store.accessToken && store.refreshToken),
			isTokenExpired: () => {
				if (!store.accessTokenExpiry) return true;
				return new Date(store.accessTokenExpiry) <= new Date();
			},
			generateGoogleAuthUrl: () => authService.generateGoogleOAuthUrl(),
			redirectToLogin: (returnUrl?: string) => {
				if (returnUrl) {
					store.setRedirectPath(returnUrl);
				}
				window.location.href = "/login";
			},
			redirectToRegister: (returnUrl?: string) => {
				if (returnUrl) {
					store.setRedirectPath(returnUrl);
				}
				window.location.href = "/register";
			},
		}),
		[store]
	);

	return {
		...authData,
		...authActions,
		...authHelpers,
	};
};

export const useAuthActions = () => {
	const store = useAuthStore();

	return useMemo(
		() => ({
			login: store.login,
			register: store.register,
			googleLogin: store.googleLogin,
			logout: store.logout,
			refreshTokens: store.refreshTokens,
			clearAuth: store.clearAuth,
			clearError: store.clearError,
			setRedirectPath: store.setRedirectPath,
			checkEmailExists: store.checkEmailExists,
			checkUsernameExists: store.checkUsernameExists,
		}),
		[store]
	);
};

export const useAuthData = () => {
	const store = useAuthStore();

	return useMemo(
		() => ({
			user: store.user,
			isAuthenticated: store.isAuthenticated,
			isLoading: store.isLoading,
			error: store.error,
			accessToken: store.accessToken,
			authType: store.authType,
			isAdmin: Boolean(store.user?.role && store.user.role & 2),
			isModerator: Boolean(store.user?.role && store.user.role & 4),
			isSuperAdmin: Boolean(store.user?.role && store.user.role & 8),
			isUser: Boolean(store.user?.role && store.user.role & 1),
			userDisplayName: store.user?.userName || "Unknown User",
			userEmail: store.user?.primaryEmail || "",
		}),
		[store]
	);
};
