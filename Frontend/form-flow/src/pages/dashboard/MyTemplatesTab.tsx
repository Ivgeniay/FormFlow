import React, { useState, useEffect, useCallback } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import { TemplateDto } from "../../shared/api_types";
import { templateApi } from "../../api/templateApi";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { SortableTable, SortConfig, TableColumn } from "../../ui/SortableTable";
import {
	ActionItem,
	ActionPanel,
} from "../../modules/templates/components/editorPageTabs/ActionPanel";
import * as Dialog from "@radix-ui/react-dialog";

interface MyTemplatesTabProps {
	accessToken: string;
}

interface ColumnConfig {
	key: string;
	label: string;
	visible: boolean;
	sortable: boolean;
}

const defaultColumns: ColumnConfig[] = [
	{ key: "title", label: "Title", visible: true, sortable: true },
	{ key: "topic", label: "Topic", visible: true, sortable: true },
	{ key: "isPublished", label: "Status", visible: true, sortable: true },
	{ key: "accessType", label: "Access", visible: true, sortable: true },
	{ key: "formsCount", label: "Forms", visible: true, sortable: true },
	{ key: "likesCount", label: "Likes", visible: false, sortable: true },
	{ key: "commentsCount", label: "Comments", visible: false, sortable: true },
	{ key: "createdAt", label: "Created", visible: true, sortable: true },
	{ key: "updatedAt", label: "Updated", visible: false, sortable: true },
];

export const MyTemplatesTab: React.FC<MyTemplatesTabProps> = ({
	accessToken,
}) => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const { user } = useAuth();
	const [templates, setTemplates] = useState<TemplateDto[]>([]);
	const [sortedTemplates, setSortedTemplates] = useState<TemplateDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [loadingMore, setLoadingMore] = useState(false);
	const [hasMore, setHasMore] = useState(true);
	const [page, setPage] = useState(1);
	const [selectedTemplates, setSelectedTemplates] = useState<Set<string>>(
		new Set()
	);
	const [columns, setColumns] = useState<ColumnConfig[]>(defaultColumns);
	const [showColumnDropdown, setShowColumnDropdown] = useState(false);
	const [hoveredRowId, setHoveredRowId] = useState<string | null>(null);
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);
	const [templateToDelete, setTemplateToDelete] = useState<TemplateDto | null>(
		null
	);

	const pageSize = 20;

	useEffect(() => {
		loadInitialTemplates();
	}, [accessToken]);

	useEffect(() => {
		setSortedTemplates([...templates]);
	}, [templates]);

	const loadInitialTemplates = async () => {
		if (!user || !accessToken) return;

		try {
			setLoading(true);
			const result = await templateApi.getUserTemplates(
				user.id,
				1,
				pageSize,
				accessToken
			);
			setTemplates(result.data);
			setPage(1);
			setHasMore(result.data.length === pageSize);
		} catch (error: any) {
			toast.error(
				error.message || t("errorLoadingTemplates", "Error loading templates")
			);
		} finally {
			setLoading(false);
		}
	};

	const loadMoreTemplates = useCallback(async () => {
		if (!user || !accessToken || loadingMore) return;

		try {
			setLoadingMore(true);
			const nextPage = page + 1;
			const result = await templateApi.getUserTemplates(
				user.id,
				nextPage,
				pageSize,
				accessToken
			);

			if (result.data.length > 0) {
				setTemplates((prev) => [...prev, ...result.data]);
				setPage(nextPage);
				setHasMore(result.data.length === pageSize);
			} else {
				setHasMore(false);
			}
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorLoadingMoreTemplates", "Error loading more templates")
			);
		} finally {
			setLoadingMore(false);
		}
	}, [user, accessToken, page, loadingMore]);

	const handleSort = (sortConfig: SortConfig) => {
		const sorted = [...templates].sort((a, b) => {
			let aValue: any;
			let bValue: any;

			switch (sortConfig.key) {
				case "title":
					aValue = a.title.toLowerCase();
					bValue = b.title.toLowerCase();
					break;
				case "topic":
					aValue = a.topic?.toLowerCase() || "";
					bValue = b.topic?.toLowerCase() || "";
					break;
				case "isPublished":
					aValue = a.isPublished ? 1 : 0;
					bValue = b.isPublished ? 1 : 0;
					break;
				case "accessType":
					aValue = a.accessType;
					bValue = b.accessType;
					break;
				case "formsCount":
					aValue = a.formsCount;
					bValue = b.formsCount;
					break;
				case "likesCount":
					aValue = a.likesCount;
					bValue = b.likesCount;
					break;
				case "commentsCount":
					aValue = a.commentsCount;
					bValue = b.commentsCount;
					break;
				case "createdAt":
					aValue = new Date(a.createdAt);
					bValue = new Date(b.createdAt);
					break;
				case "updatedAt":
					aValue = new Date(a.updatedAt);
					bValue = new Date(b.updatedAt);
					break;
				default:
					return 0;
			}

			if (aValue < bValue) {
				return sortConfig.direction === "asc" ? -1 : 1;
			}
			if (aValue > bValue) {
				return sortConfig.direction === "asc" ? 1 : -1;
			}
			return 0;
		});

		setSortedTemplates(sorted);
	};

	const toggleColumnVisibility = (columnKey: string) => {
		setColumns((prev) =>
			prev.map((col) =>
				col.key === columnKey ? { ...col, visible: !col.visible } : col
			)
		);
	};

	const handleTemplateView = (template: TemplateDto) => {
		navigate(`/template/${template.id}/preview`);
	};

	const handleTemplateEdit = (template: TemplateDto) => {
		navigate(`/template/${template.id}`);
	};

	const handleTemplateDelete = (template: TemplateDto) => {
		setTemplateToDelete(template);
		setShowDeleteDialog(true);
	};

	const confirmDeleteTemplate = async () => {
		if (!templateToDelete) return;

		try {
			await templateApi.deleteTemplate(templateToDelete.id, accessToken);
			setTemplates((prev) => prev.filter((t) => t.id !== templateToDelete.id));
			toast.success(
				t("templateDeletedSuccessfully") || "Template deleted successfully"
			);
			setShowDeleteDialog(false);
			setTemplateToDelete(null);
		} catch (error: any) {
			toast.error(t("errorDeletingTemplate") || "Error deleting template");
		}
	};

	const handleTemplatePublishToggle = async (template: TemplateDto) => {
		try {
			if (template.isPublished) {
				await templateApi.unpublishTemplate(template.id, accessToken);
				toast.success(
					t("templateUnpublishedSuccessfully") ||
						"Template unpublished successfully"
				);
			} else {
				await templateApi.publishTemplate(template.id, accessToken);
				toast.success(
					t("templatePublishedSuccessfully") ||
						"Template published successfully"
				);
			}
			loadInitialTemplates();
		} catch (error: any) {
			toast.error(
				t("errorTogglePublishTemplate") || "Error changing template status"
			);
		}
	};

	const handleCreateTemplate = () => {
		navigate("/template");
	};

	const getTemplateActions = (template: TemplateDto): ActionItem[] => [
		{
			id: template.id + "view",
			label: t("view", "View"),
			icon: "👁️",
			onClick: () => handleTemplateView(template),
		},
		{
			id: template.id + "edit",
			label: t("edit", "Edit"),
			icon: "✏️",
			onClick: () => handleTemplateEdit(template),
		},
		{
			id: template.id + "publish",
			label: template.isPublished
				? t("unpublish", "Unpublish")
				: t("publish", "Publish"),
			icon: template.isPublished ? "📤" : "📢",
			onClick: () => handleTemplatePublishToggle(template),
		},
		{
			id: template.id + "delete",
			label: t("delete", "Delete"),
			icon: "🗑️",
			onClick: () => handleTemplateDelete(template),
			variant: "danger" as const,
		},
	];

	const getTableColumns = (): TableColumn<TemplateDto>[] => {
		return columns
			.filter((col) => col.visible)
			.map((col) => ({
				key: col.key,
				label: col.label,
				sortable: col.sortable,
				render: (template: TemplateDto) => {
					switch (col.key) {
						case "title":
							return (
								<div
									className="font-medium text-text hover:text-primary cursor-pointer"
									onClick={() => handleTemplateView(template)}
								>
									{template.title}
								</div>
							);
						case "topic":
							return (
								<span className="text-textMuted">
									{template.topic || t("noTopic", "No Topic")}
								</span>
							);
						case "isPublished":
							return (
								<span
									className={`px-2 py-1 rounded-full text-xs font-medium ${
										template.isPublished
											? "bg-success/10 text-success"
											: "bg-warning/10 text-warning"
									}`}
								>
									{template.isPublished
										? t("published", "Published")
										: t("draft", "Draft")}
								</span>
							);
						case "accessType":
							return (
								<span className="text-textMuted">
									{template.accessType === 1
										? t("publicAccess", "Public")
										: t("privateAccess", "Private")}
								</span>
							);
						case "formsCount":
							return (
								<span className="text-textMuted">{template.formsCount}</span>
							);
						case "likesCount":
							return (
								<span className="text-textMuted">{template.likesCount}</span>
							);
						case "commentsCount":
							return (
								<span className="text-textMuted">{template.commentsCount}</span>
							);
						case "createdAt":
							return (
								<span className="text-textMuted">
									{new Date(template.createdAt).toLocaleDateString()}
								</span>
							);
						case "updatedAt":
							return (
								<span className="text-textMuted">
									{new Date(template.updatedAt).toLocaleDateString()}
								</span>
							);
						default:
							return "";
					}
				},
			}));
	};

	if (loading) {
		return (
			<div className="flex justify-center py-8">
				<span className="text-textMuted">{t("loading", "Loading...")}</span>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<div className="flex items-center gap-4">
					<button
						onClick={handleCreateTemplate}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
					>
						{t("createTemplate", "Create Template")}
					</button>

					{selectedTemplates.size > 0 && (
						<span className="text-textMuted">
							{t("selectedTemplates", "Selected")}: {selectedTemplates.size}
						</span>
					)}
				</div>

				<div className="relative">
					<button
						onClick={() => setShowColumnDropdown(!showColumnDropdown)}
						className="px-3 py-2 border border-border rounded-lg text-text hover:bg-surface transition-colors text-sm"
					>
						{t("columns", "Columns")} ▼
					</button>

					{showColumnDropdown && (
						<div className="absolute right-0 top-full mt-1 bg-surface border border-border rounded-lg shadow-lg z-30 min-w-48 p-2">
							{columns.map((column) => (
								<label
									key={column.key}
									className="flex items-center gap-2 p-2 hover:bg-background rounded cursor-pointer"
								>
									<input
										type="checkbox"
										checked={column.visible}
										onChange={() => toggleColumnVisibility(column.key)}
										className="text-primary"
									/>
									<span className="text-sm text-text">{column.label}</span>
								</label>
							))}
						</div>
					)}
				</div>
			</div>

			<SortableTable
				data={sortedTemplates}
				columns={getTableColumns()}
				onSort={handleSort}
				selectable={true}
				selectedItems={selectedTemplates}
				onSelectionChange={setSelectedTemplates}
				getItemId={(template) => template.id}
				emptyMessage={
					t("noTemplatesFound", "No templates found") || "No templates found"
				}
				onRowHover={setHoveredRowId}
				onReachEnd={hasMore ? loadMoreTemplates : undefined}
				isLoadingMore={loadingMore}
				renderRow={(template) => (
					<td className="absolute inset-0 pointer-events-none">
						<ActionPanel
							visible={hoveredRowId === template.id}
							actions={getTemplateActions(template)}
							position="right"
							className="pointer-events-auto z-10"
						/>
					</td>
				)}
			/>

			<Dialog.Root open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
				<Dialog.Portal>
					<Dialog.Overlay className="fixed inset-0 bg-black/50 z-50" />
					<Dialog.Content className="fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-surface border border-border rounded-lg p-6 max-w-md w-full mx-4 z-50">
						<Dialog.Title className="text-lg font-semibold text-text mb-2">
							{t("confirmDeleteTemplate") || "Confirm Delete Template"}
						</Dialog.Title>
						<Dialog.Description className="text-textMuted mb-6">
							{t("deleteTemplateConfirm") ||
								"Are you sure you want to delete this template? This action cannot be undone."}
						</Dialog.Description>
						<div className="flex justify-end gap-3">
							<button
								onClick={() => setShowDeleteDialog(false)}
								className="px-4 py-2 text-textMuted hover:text-text transition-colors"
							>
								{t("cancel") || "Cancel"}
							</button>
							<button
								onClick={confirmDeleteTemplate}
								className="px-4 py-2 bg-error text-white rounded-lg hover:opacity-90 transition-opacity"
							>
								{t("delete") || "Delete"}
							</button>
						</div>
					</Dialog.Content>
				</Dialog.Portal>
			</Dialog.Root>
		</div>
	);
};
