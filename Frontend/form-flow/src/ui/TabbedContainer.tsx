import React, { useState, useCallback } from "react";
import * as Dialog from "@radix-ui/react-dialog";
import { useTranslation } from "react-i18next";

export interface Tab {
	id: string;
	label: string;
	content: React.ReactNode;
	badge?: string | number;
	disabled?: boolean;
}

export interface TabbedContainerProps {
	tabs: Tab[];
	defaultTab?: string;
	hasUnsavedChanges?: boolean;
	onTabChange?: (tabId: string, previousTabId: string) => void;
	onSaveChanges?: () => void;
	onDiscardChanges?: () => void;
	className?: string;
	tabsClassName?: string;
	contentClassName?: string;
	showUnsavedDialog?: boolean;
	unsavedDialogTitle?: string;
	unsavedDialogDescription?: string;
}

export const TabbedContainer: React.FC<TabbedContainerProps> = ({
	tabs,
	defaultTab,
	hasUnsavedChanges = false,
	onTabChange,
	onSaveChanges,
	onDiscardChanges,
	className = "",
	tabsClassName = "",
	contentClassName = "",
	showUnsavedDialog = false,
	unsavedDialogTitle,
	unsavedDialogDescription,
}) => {
	const { t } = useTranslation();
	const [activeTab, setActiveTab] = useState<string>(
		defaultTab || tabs[0]?.id || ""
	);
	const [showDialog, setShowDialog] = useState(false);
	const [pendingTab, setPendingTab] = useState<string | null>(null);

	const handleTabClick = useCallback(
		(newTabId: string) => {
			if (newTabId === activeTab) return;

			const targetTab = tabs.find((tab) => tab.id === newTabId);
			if (targetTab?.disabled) return;

			if (hasUnsavedChanges && showUnsavedDialog) {
				setPendingTab(newTabId);
				setShowDialog(true);
				return;
			}

			const previousTab = activeTab;
			setActiveTab(newTabId);
			onTabChange?.(newTabId, previousTab);
		},
		[activeTab, hasUnsavedChanges, showUnsavedDialog, tabs, onTabChange]
	);

	const handleSaveAndSwitch = useCallback(() => {
		if (onSaveChanges) {
			onSaveChanges();
		}
		setShowDialog(false);
		if (pendingTab) {
			const previousTab = activeTab;
			setActiveTab(pendingTab);
			onTabChange?.(pendingTab, previousTab);
			setPendingTab(null);
		}
	}, [onSaveChanges, pendingTab, activeTab, onTabChange]);

	const handleDiscardAndSwitch = useCallback(() => {
		if (onDiscardChanges) {
			onDiscardChanges();
		}
		setShowDialog(false);
		if (pendingTab) {
			const previousTab = activeTab;
			setActiveTab(pendingTab);
			onTabChange?.(pendingTab, previousTab);
			setPendingTab(null);
		}
	}, [pendingTab, activeTab, onTabChange, onDiscardChanges]);

	const handleDialogCancel = useCallback(() => {
		setShowDialog(false);
		setPendingTab(null);
	}, []);

	const isTabActive = useCallback(
		(tabId: string) => activeTab === tabId,
		[activeTab]
	);

	const formatTabLabel = useCallback((tab: Tab) => {
		if (tab.badge !== undefined) {
			return `${tab.label} (${tab.badge})`;
		}
		return tab.label;
	}, []);

	const activeTabContent = tabs.find((tab) => tab.id === activeTab)?.content;

	return (
		<div className={`bg-surface border border-border rounded-lg ${className}`}>
			<div className={`border-b border-border px-6 py-4 ${tabsClassName}`}>
				<nav className="flex space-x-8">
					{tabs.map((tab) => (
						<button
							key={tab.id}
							onClick={() => handleTabClick(tab.id)}
							disabled={tab.disabled}
							className={`pb-2 font-medium transition-colors ${
								isTabActive(tab.id)
									? "text-primary border-b-2 border-primary"
									: tab.disabled
									? "text-textMuted/50 cursor-not-allowed"
									: "text-textMuted hover:text-text"
							}`}
						>
							{formatTabLabel(tab)}
						</button>
					))}
				</nav>
			</div>

			<div className={`p-6 ${contentClassName}`}>{activeTabContent}</div>

			{showUnsavedDialog && (
				<Dialog.Root open={showDialog} onOpenChange={setShowDialog}>
					<Dialog.Portal>
						<Dialog.Overlay className="fixed inset-0 bg-black/50 z-50" />
						<Dialog.Content className="fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-surface border border-border rounded-lg p-6 max-w-md w-full mx-4 z-50">
							<Dialog.Title className="text-lg font-semibold text-text mb-2">
								{unsavedDialogTitle || t("unsavedChanges", "Unsaved Changes")}
							</Dialog.Title>
							<Dialog.Description className="text-textMuted mb-6">
								{unsavedDialogDescription ||
									t(
										"unsavedChangesDescription",
										"You have unsaved changes. What would you like to do?"
									)}
							</Dialog.Description>
							<div className="flex items-center justify-end gap-3">
								<button
									className="px-4 py-2 text-textMuted hover:text-text transition-colors"
									onClick={handleDialogCancel}
								>
									{t("cancel", "Cancel")}
								</button>
								<button
									className="px-4 py-2 bg-warning text-white rounded-lg hover:opacity-90 transition-opacity"
									onClick={handleDiscardAndSwitch}
								>
									{t("discard", "Discard")}
								</button>
								<button
									className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
									onClick={handleSaveAndSwitch}
								>
									{t("saveAndSwitch", "Save & Switch")}
								</button>
							</div>
						</Dialog.Content>
					</Dialog.Portal>
				</Dialog.Root>
			)}
		</div>
	);
};
