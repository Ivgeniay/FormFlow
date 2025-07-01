import React from "react";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { Tab, TabbedContainer } from "../../ui/TabbedContainer";
import { MyTemplatesTab } from "./MyTemplatesTab";
import { MyFormsTab } from "./MyFormsTab";

export const MyDashboardPage: React.FC = () => {
	const { t } = useTranslation();
	const { isAuthenticated, user, accessToken } = useAuth();

	if (!isAuthenticated || !user || !accessToken) {
		return (
			<div className="flex items-center justify-center py-16">
				<div className="text-center">
					<h1 className="text-2xl font-bold text-text mb-4">
						{t("accessDenied", "Access Denied")}
					</h1>
					<p className="text-textMuted">
						{t("loginRequired", "Please log in to view your dashboard")}
					</p>
				</div>
			</div>
		);
	}

	const tabs: Tab[] = [
		{
			id: "templates",
			label: t("myTemplates", "My Templates"),
			content: <MyTemplatesTab accessToken={accessToken} />,
		},
		{
			id: "forms",
			label: t("myForms", "My Forms"),
			content: <MyFormsTab accessToken={accessToken} />,
		},
	];

	return (
		<div className="space-y-6">
			<div className="bg-surface border border-border rounded-lg p-6">
				<div className="flex items-center justify-between mb-4">
					<div>
						<h1 className="text-2xl font-bold text-text">
							{t("myDashboard", "My Dashboard")}
						</h1>
						<p className="text-textMuted mt-1">
							{t(
								"dashboardDescription",
								"Manage your templates and submitted forms"
							)}
						</p>
					</div>
				</div>
			</div>

			<TabbedContainer
				tabs={tabs}
				defaultTab="templates"
				showUnsavedDialog={false}
			/>
		</div>
	);
};
