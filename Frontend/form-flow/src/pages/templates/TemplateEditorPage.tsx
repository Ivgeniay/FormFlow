import React, { use, useRef, useState } from "react";
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
import * as Dialog from "@radix-ui/react-dialog";

interface TemplateEditorPageProps {
	template: TemplateDto;
	className?: string;
}

type TabType = "questions" | "responses" | "versions" | "analytics";

export const TemplateEditorPage: React.FC<TemplateEditorPageProps> = ({
	template,
	className,
}) => {
	const { t } = useTranslation();
	const questionsTabRef = useRef<QuestionsTabRef>(null);
	const { accessToken } = useAuth();
	const [activeTab, setActiveTab] = useState<TabType>("questions");

	const [newTemplateState, setNewTemplateState] = useState({ ...template });
	const [isSaving, setIsSaving] = useState(false);
	const [hasChanges, setHasChange] = useState(false);

	const [showUnsavedDialog, setShowUnsavedDialog] = useState(false);
	const [pendingTab, setPendingTab] = useState<TabType | null>(null);

	const handleTemplateTobSave = () => {
		questionsTabRef.current?.save();
	};

	const handleTabChange = (newTab: TabType) => {
		if (hasChanges) {
			setPendingTab(newTab);
			setShowUnsavedDialog(true);
			return;
		}
		setActiveTab(newTab);
	};

	const renderTabContent = () => {
		switch (activeTab) {
			case "questions":
				return (
					<QuestionsTab
						hasChangeCallback={setHasChange}
						template={template}
						accessToken={accessToken}
						ref={questionsTabRef}
					/>
				);
			case "responses":
				return <ResponsesTab template={template} accessToken={accessToken} />;
			case "versions":
				return <VersionsTab template={template} accessToken={accessToken} />;
			case "analytics":
				return <AnalyticsTab template={template} accessToken={accessToken} />;
			default:
				return <QuestionsTab template={template} accessToken={accessToken} />;
		}
	};

	const getTabLabel = (tab: TabType) => {
		switch (tab) {
			case "questions":
				return t("questions", "Questions");
			case "responses":
				return `${t("responses", "Responses")} (${template.formsCount})`;
			case "versions":
				return t("versions", "Versions");
			case "analytics":
				return t("analytics", "Analytics");
		}
	};

	const isTabActive = (tab: TabType) => activeTab === tab;

	return (
		<div className={`space-y-6 ${className}`}>
			<div className="bg-surface border border-border rounded-lg p-6">
				<div className="flex items-center justify-between mb-4">
					<div className="flex items-center gap-4">
						<span
							className={`px-3 py-1 rounded-full text-sm font-medium ${
								newTemplateState.isPublished
									? "bg-success/10 text-success"
									: "bg-warning/10 text-warning"
							}`}
						>
							{newTemplateState.isPublished
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

			<div className="bg-surface border border-border rounded-lg">
				<div className="border-b border-border px-6 py-4">
					<nav className="flex space-x-8">
						{(
							["questions", "responses", "versions", "analytics"] as TabType[]
						).map((tab) => (
							<button
								key={tab}
								onClick={() => handleTabChange(tab)}
								className={`pb-2 font-medium transition-colors ${
									isTabActive(tab)
										? "text-primary border-b-2 border-primary"
										: "text-textMuted hover:text-text"
								}`}
							>
								{getTabLabel(tab)}
							</button>
						))}
					</nav>
				</div>

				<div className="p-6">{renderTabContent()}</div>
			</div>

			<Dialog.Root open={showUnsavedDialog} onOpenChange={setShowUnsavedDialog}>
				<Dialog.Portal>
					<Dialog.Overlay className="fixed inset-0 bg-black/50" />
					<Dialog.Content className="fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-surface border rounded-lg p-6">
						<Dialog.Title>Unsaved Changes</Dialog.Title>
						<Dialog.Description>
							You have unsaved changes. What would you like to do?
						</Dialog.Description>
						<div className="flex items-center justify-between gap-2 mt-4 ">
							<button
								className="bg-primary p-1.5 text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50 "
								onClick={() => {
									handleTemplateTobSave();
									setShowUnsavedDialog(false);
									setHasChange(false);
									if (pendingTab) setActiveTab(pendingTab);
								}}
							>
								Save & Switch
							</button>
							<button
								className="bg-warning p-1.5 text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50 "
								onClick={() => {
									setShowUnsavedDialog(false);
									setHasChange(false);
									if (pendingTab) setActiveTab(pendingTab);
								}}
							>
								Discard
							</button>
						</div>
					</Dialog.Content>
				</Dialog.Portal>
			</Dialog.Root>
		</div>
	);
};
