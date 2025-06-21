import React from "react";
import { useTranslation } from "react-i18next";
import { RegisterForm } from "../../modules/auth/components/RegisterForm";
import { useNavigate } from "react-router-dom";

interface RegisterPageProps {
	onSwitchToLogin?: () => void;
}

export const RegisterPage: React.FC<RegisterPageProps> = ({
	onSwitchToLogin,
}) => {
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
					<div className="text-center mb-8">
						<h2 className="text-3xl font-bold text-text">
							{t("appName") || "FormFlow"}
						</h2>
						<p className="mt-2 text-textMuted">
							{t(
								"registerPageDescription",
								"Create your account to get started"
							) || "Create your account to get started"}
						</p>
					</div>

					<RegisterForm onSwitchToLogin={onSwitchToLogin} />
				</div>
			</div>
		</div>
	);
};
