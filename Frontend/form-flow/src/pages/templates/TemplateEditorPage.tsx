import React, { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { TemplateDto } from "../../shared/api_types";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { AnalyticsTab } from "../../modules/templates/components/editorPageTabs/AnalyticsTab";
import { VersionsTab } from "../../modules/templates/components/editorPageTabs/VersionsTab";
import { ResponsesTab } from "../../modules/templates/components/editorPageTabs/ResponsesTab";
import {
	QuestionsTab,
	QuestionsTabRef,
} from "../../modules/templates/components/editorPageTabs/QuestionsTab";
import { Tab, TabbedContainer } from "../../ui/TabbedContainer";

interface TemplateEditorPageProps {
	template: TemplateDto;
	className?: string;
}

export const TemplateEditorPage: React.FC<TemplateEditorPageProps> = ({
	template,
	className,
}) => {
	const { t } = useTranslation();
	const questionsTabRef = useRef<QuestionsTabRef>(null);
	const { accessToken } = useAuth();
	const [hasChanges, setHasChange] = useState(false);

	const handleTemplateTobSave = () => {
		questionsTabRef.current?.save();
	};

	const tabs: Tab[] = [
		{
			id: "questions",
			label: t("questions", "Questions"),
			content: (
				<QuestionsTab
					hasChangeCallback={setHasChange}
					template={template}
					accessToken={accessToken}
					ref={questionsTabRef}
				/>
			),
		},
		{
			id: "responses",
			label: t("responses", "Responses"),
			badge: template.formsCount,
			content: <ResponsesTab template={template} accessToken={accessToken} />,
		},
		{
			id: "versions",
			label: t("versions", "Versions"),
			content: <VersionsTab template={template} accessToken={accessToken} />,
		},
		{
			id: "analytics",
			label: t("analytics", "Analytics"),
			content: <AnalyticsTab template={template} accessToken={accessToken} />,
		},
	];

	return (
		<div className={`space-y-6 ${className}`}>
			<div className="bg-surface border border-border rounded-lg p-6">
				<div className="flex items-center justify-between mb-4">
					<div className="flex items-center gap-4">
						<span
							className={`px-3 py-1 rounded-full text-sm font-medium ${
								template.isPublished
									? "bg-success/10 text-success"
									: "bg-warning/10 text-warning"
							}`}
						>
							{template.isPublished
								? t("published", "Published")
								: t("draft", "Draft")}
						</span>
						{hasChanges && <span className="text-warning font-medium">*</span>}
					</div>
				</div>

				<div className="flex items-center gap-6 text-sm text-textMuted">
					<span>
						{t("author", "Author")}: {template.authorName}
					</span>
					<span>
						{t("created", "Created")}:{" "}
						{new Date(template.createdAt).toLocaleDateString()}
					</span>
					<span>
						{t("version", "Version")}: {template.version}
					</span>
					<span>
						{t("forms", "Forms")}: {template.formsCount}
					</span>
					<span>
						{t("likes", "Likes")}: {template.likesCount}
					</span>
				</div>
			</div>

			<TabbedContainer
				tabs={tabs}
				defaultTab="questions"
				hasUnsavedChanges={hasChanges}
				onSaveChanges={handleTemplateTobSave}
				onDiscardChanges={() => setHasChange(false)}
				showUnsavedDialog={true}
				unsavedDialogTitle={
					t("unsavedChanges", "Unsaved Changes") || "Unsaved Changes"
				}
				unsavedDialogDescription={
					t(
						"unsavedChangesDescription",
						"You have unsaved changes. What would you like to do?"
					) || "You have unsaved changes. What would you like to do?"
				}
			/>
		</div>
	);
};
