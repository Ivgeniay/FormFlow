import React from "react";
import { useTranslation } from "react-i18next";
import { RegisterForm } from "../../modules/auth/components/RegisterForm";

interface RegisterPageProps {
	onSwitchToLogin?: () => void;
}

export const RegisterPage: React.FC<RegisterPageProps> = ({
	onSwitchToLogin,
}) => {
	const { t } = useTranslation();

	return (
		<div className="min-h-screen bg-background flex flex-col justify-center py-12 sm:px-6 lg:px-8">
			<div className="sm:mx-auto sm:w-full sm:max-w-md">
				<div className="text-center">
					<h2 className="text-3xl font-bold text-text">
						{t("appName") || "FormFlow"}
					</h2>
					<p className="mt-2 text-textMuted">
						{t("registerPageDescription", "Create your account to get started") ||
							"Create your account to get started"}
					</p>
				</div>
			</div>

			<div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
				<RegisterForm onSwitchToLogin={onSwitchToLogin} />
			</div>
		</div>
	);
};
