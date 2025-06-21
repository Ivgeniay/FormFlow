import React from "react";
import { useTranslation } from "react-i18next";
import { LoginForm } from "../../modules/auth/components/LoginForm";
import { useNavigate } from "react-router-dom";

interface LoginPageProps {
	onSwitchToRegister?: () => void;
}

export const LoginPage: React.FC<LoginPageProps> = ({ onSwitchToRegister }) => {
	const { t } = useTranslation();
	const navigate = useNavigate();

	const goBack = () => {
		if (window.history.length > 1) {
			navigate(-1);
		} else {
			navigate("/");
		}
	};

	return (
		<div className="min-h-screen bg-background">
			<div className="container mx-auto px-4 py-8 max-w-4xl">
				<button
					onClick={goBack}
					className="inline-flex items-center gap-2 text-textMuted hover:text-text transition-colors mb-6"
				>
					<svg
						className="w-4 h-4"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M15 19l-7-7 7-7"
						/>
					</svg>
					{t("back") || "Back"}
				</button>

				<div className="flex flex-col justify-center min-h-[calc(100vh-12rem)]">
					<div className="sm:mx-auto sm:w-full sm:max-w-md">
						<div className="text-center">
							<h2 className="text-3xl font-bold text-text">
								{t("appName") || "FormFlow"}
							</h2>
							<p className="mt-2 text-textMuted">
								{t("loginPageDescription", "Sign in to access your account") ||
									"Sign in to access your account"}
							</p>
						</div>
					</div>

					<div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
						<LoginForm onSwitchToRegister={onSwitchToRegister} />
					</div>
				</div>
			</div>
		</div>
	);
};
