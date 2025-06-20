import React from "react";
import { useTranslation } from "react-i18next";
import { LoginForm } from "../../modules/auth/components/LoginForm";

interface LoginPageProps {
	onSwitchToRegister?: () => void;
}

export const LoginPage: React.FC<LoginPageProps> = ({ onSwitchToRegister }) => {
	const { t } = useTranslation();

	return (
		<div className="min-h-screen bg-background flex flex-col justify-center py-12 sm:px-6 lg:px-8">
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
	);
};
