import axios, {
	AxiosResponse,
	AxiosError,
	InternalAxiosRequestConfig,
} from "axios";
import { useAuthStore } from "../modules/auth/store/authStore";

interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
	_retry?: boolean;
}

let isRefreshing = false;
let failedQueue: Array<{
	resolve: (value: any) => void;
	reject: (error: any) => void;
}> = [];

const processQueue = (error: any, token: string | null = null) => {
	failedQueue.forEach(({ resolve, reject }) => {
		if (error) {
			reject(error);
		} else {
			resolve(token);
		}
	});

	failedQueue = [];
};

const isAuthEndpoint = (url: string | undefined): boolean => {
	if (!url) return false;
	return (
		url.includes("/auth/refresh") ||
		url.includes("/auth/login") ||
		url.includes("/auth/register") ||
		url.includes("/auth/google-login")
	);
};

const setupAxiosInterceptors = () => {
	axios.interceptors.response.use(
		(response: AxiosResponse) => {
			return response;
		},
		async (error: AxiosError) => {
			const originalRequest = error.config as CustomAxiosRequestConfig;

			if (
				error.response?.status === 401 &&
				error.response?.data &&
				typeof error.response.data === "object" &&
				"requireTokenRefresh" in error.response.data &&
				error.response.data.requireTokenRefresh === true &&
				originalRequest &&
				!originalRequest._retry &&
				!isAuthEndpoint(originalRequest.url)
			) {
				if (isRefreshing) {
					return new Promise((resolve, reject) => {
						failedQueue.push({ resolve, reject });
					})
						.then((token) => {
							if (originalRequest.headers) {
								originalRequest.headers["Authorization"] = `Bearer ${token}`;
							}
							return axios(originalRequest);
						})
						.catch((err) => {
							return Promise.reject(err);
						});
				}

				originalRequest._retry = true;
				isRefreshing = true;

				try {
					const authStore = useAuthStore.getState();

					console.log("Attempting token refresh due to blacklist...");
					await authStore.refreshTokens();
					console.log("Token refresh successful");

					const newAccessToken = useAuthStore.getState().accessToken;
					console.log(`newAccessToken: ${newAccessToken}`);

					if (newAccessToken && originalRequest.headers) {
						originalRequest.headers[
							"Authorization"
						] = `Bearer ${newAccessToken}`;
					}

					processQueue(null, newAccessToken);
					isRefreshing = false;

					return axios(originalRequest);
				} catch (refreshError) {
					processQueue(refreshError, null);
					isRefreshing = false;

					const authStore = useAuthStore.getState();
					authStore.clearAuth();

					window.location.href = "/login";
					return Promise.reject(refreshError);
				}
			}

			return Promise.reject(error);
		}
	);
};

export default setupAxiosInterceptors;
