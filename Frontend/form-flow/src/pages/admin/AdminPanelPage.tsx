import React from "react";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { Tab, TabbedContainer } from "../../ui/TabbedContainer";
import { FormsTab } from "../../modules/admin/FormsTab";
import { TemplatesTab } from "../../modules/admin/TemplatesTab";
import { UsersTab } from "../../modules/admin/UsersTab";
import { PromoteToRoleButtons } from "../../modules/auth/components/PromoteToRoleButton";

export const AdminPanelPage: React.FC = () => {
	const { t } = useTranslation();
	const { isAdmin, user, accessToken } = useAuth();

	if (!isAdmin || !user || !accessToken) {
		return (
			<div className="flex items-center justify-center py-16">
				<div className="text-center">
					<h1 className="text-2xl font-bold text-error mb-4">
						{t("accessDenied", "Access Denied")}
					</h1>
					<p className="text-textMuted">
						{t("adminAccessRequired", "Administrator access required")}
					</p>
				</div>
			</div>
		);
	}

	const tabs: Tab[] = [
		{
			id: "users",
			label: t("usersManagement", "Users Management"),
			content: <UsersTab accessToken={accessToken} />,
		},
		{
			id: "templates",
			label: t("templatesOverview", "Templates Overview"),
			content: <TemplatesTab accessToken={accessToken} />,
		},
		{
			id: "forms",
			label: t("formsOverview", "Forms Overview"),
			content: <FormsTab accessToken={accessToken} />,
		},
	];

	return (
		<div className="space-y-6">
			<div className="bg-surface border border-border rounded-lg p-6">
				<div className="flex items-center justify-between mb-4">
					<div>
						<h1 className="text-2xl font-bold text-text">
							{t("adminPanel", "Admin Panel")}
						</h1>
					</div>
					<div className="flex items-center gap-2">
						<span className="px-3 py-1 rounded-full text-sm font-medium bg-primary/10 text-primary">
							{t("administrator", "Administrator")}
						</span>
					</div>
				</div>

				<div className="flex items-center gap-6 text-sm mb-2 mt-2">
					<div className="text-text">
						<PromoteToRoleButtons />
					</div>
				</div>
			</div>

			<TabbedContainer
				tabs={tabs}
				defaultTab="users"
				showUnsavedDialog={false}
			/>
		</div>
	);
};
