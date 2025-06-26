import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { AppLoader } from "../../components/AppLoader";
import toast from "react-hot-toast";

export const GoogleCallbackPage: React.FC = () => {
	const { t } = useTranslation();
	const location = useLocation();
	const navigate = useNavigate();
	const { googleLogin, isAuthenticated } = useAuth();

	const [isProcessing, setIsProcessing] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		if (isAuthenticated) {
			navigate("/", { replace: true });
			return;
		}

		const processGoogleCallback = async () => {
			try {
				const urlParams = new URLSearchParams(location.search);
				const code = urlParams.get("code");
				// if (code) {
				// 	const encodedCode = encodeURIComponent(code);
				// 	await googleLogin(encodedCode);
				// }
				const error = urlParams.get("error");
				const errorDescription = urlParams.get("error_description");

				if (error) {
					let errorMessage = "Google authentication failed";

					switch (error) {
						case "access_denied":
							errorMessage =
								t(
									"googleAccessDenied",
									"Access denied. You need to grant permission to continue."
								) || "Access denied";
							break;
						case "invalid_request":
							errorMessage =
								t("googleInvalidRequest", "Invalid authentication request") ||
								"Invalid request";
							break;
						default:
							errorMessage =
								errorDescription ||
								t("googleAuthError", "Google authentication error") ||
								"Authentication error";
					}

					setError(errorMessage);
					toast.error(errorMessage);

					setTimeout(() => {
						navigate("/login", { replace: true });
					}, 3000);
					return;
				}

				if (!code) {
					setError(
						t("googleNoCode", "No authorization code received from Google") ||
							"No authorization code received"
					);
					toast.error("Authentication failed: No authorization code");

					setTimeout(() => {
						navigate("/login", { replace: true });
					}, 3000);
					return;
				}

				await googleLogin(code);
			} catch (error) {
				console.error("Google callback error:", error);

				const errorMessage =
					error instanceof Error
						? error.message
						: t("googleLoginFailed", "Google login failed") ||
						  "Google login failed";

				setError(errorMessage);
				toast.error(errorMessage);

				setTimeout(() => {
					navigate("/login", { replace: true });
				}, 3000);
			} finally {
				setIsProcessing(false);
			}
		};

		processGoogleCallback();
	}, [location.search, googleLogin, isAuthenticated, navigate, t]);

	if (isProcessing) {
		return (
			<>
				<AppLoader isVisible={true} />
			</>
		);
	}

	if (error) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center">
				<div className="max-w-md w-full text-center">
					<div className="bg-surface border border-border rounded-lg p-6">
						<div className="text-error text-6xl mb-4">⚠️</div>
						<h1 className="text-2xl font-bold text-text mb-2">
							{t("authenticationFailed", "Authentication Failed") ||
								"Authentication Failed"}
						</h1>
						<p className="text-textMuted mb-6">{error}</p>

						<div className="space-y-3">
							<button
								onClick={() => navigate("/login", { replace: true })}
								className="w-full px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
							>
								{t("backToLogin", "Back to Login") || "Back to Login"}
							</button>

							<button
								onClick={() => navigate("/", { replace: true })}
								className="w-full px-4 py-2 text-textMuted hover:text-text border border-border rounded-lg hover:bg-background transition-colors"
							>
								{t("backToHome", "Back to Home") || "Back to Home"}
							</button>
						</div>
					</div>
				</div>
			</div>
		);
	}

	return (
		<div className="min-h-screen bg-background flex items-center justify-center">
			<div className="text-center">
				{/* <div className="text-success text-6xl mb-4">✅</div> */}
				<h2 className="text-xl font-semibold text-text mb-2">
					{t("authenticationSuccessful", "Authentication Successful") ||
						"Authentication Successful"}
				</h2>
				<p className="text-textMuted">
					{t("redirectingToHome", "Redirecting to home...") || "Redirecting..."}
				</p>
			</div>
		</div>
	);
};
