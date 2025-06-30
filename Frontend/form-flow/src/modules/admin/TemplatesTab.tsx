import React, { useState, useEffect, useCallback } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import { TemplateDto } from "../../shared/api_types";
import { templateApi } from "../../api/templateApi";
import {
	ActionItem,
	ActionPanel,
} from "../templates/components/editorPageTabs/ActionPanel";
import {
	SortableTable,
	SortConfig,
	TableColumn,
} from "../templates/components/editorPageTabs/SortableTable";

interface TemplatesTabProps {
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
	{ key: "authorName", label: "Author", visible: true, sortable: true },
	{ key: "topic", label: "Topic", visible: true, sortable: true },
	{ key: "isPublished", label: "Status", visible: true, sortable: true },
	{ key: "accessType", label: "Access", visible: true, sortable: true },
	{ key: "formsCount", label: "Forms", visible: true, sortable: true },
	{ key: "likesCount", label: "Likes", visible: false, sortable: true },
	{ key: "commentsCount", label: "Comments", visible: false, sortable: true },
	{ key: "createdAt", label: "Created", visible: true, sortable: true },
	{ key: "updatedAt", label: "Updated", visible: false, sortable: true },
];

export const TemplatesTab: React.FC<TemplatesTabProps> = ({ accessToken }) => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const [templates, setTemplates] = useState<TemplateDto[]>([]);
	const [sortedTemplates, setSortedTemplates] = useState<TemplateDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [selectedTemplates, setSelectedTemplates] = useState<Set<string>>(
		new Set()
	);
	const [columns, setColumns] = useState<ColumnConfig[]>(defaultColumns);
	const [showColumnDropdown, setShowColumnDropdown] = useState(false);
	const [hoveredRowId, setHoveredRowId] = useState<string | null>(null);
	const [loadingMore, setLoadingMore] = useState(false);
	const [hasMore, setHasMore] = useState(true);
	const [page, setPage] = useState(1);
	const [totalCount, setTotalCount] = useState(0);

	const pageSize = 2;

	useEffect(() => {
		loadInitialTemplates();
	}, [accessToken]);

	useEffect(() => {
		setSortedTemplates([...templates]);
	}, [templates]);

	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (showColumnDropdown) {
				setShowColumnDropdown(false);
			}
		};

		document.addEventListener("click", handleClickOutside);
		return () => document.removeEventListener("click", handleClickOutside);
	}, [showColumnDropdown]);

	const loadInitialTemplates = async () => {
		try {
			setLoading(true);
			const result = await templateApi.getAllTemplatesForAdmin(
				1,
				pageSize,
				accessToken
			);
			console.log(result);
			setTemplates(result.data);
			setTotalCount(result.pagination.totalCount);
			setPage(1);
			setHasMore(result.data.length === pageSize);
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorLoadingTemplates", "Error loading templates") ||
					"Error loading templates"
			);
		} finally {
			setLoading(false);
		}
	};

	const loadMoreTemplates = useCallback(async () => {
		if (!accessToken || loadingMore) return;

		try {
			setLoadingMore(true);
			const nextPage = page + 1;
			const result = await templateApi.getAllTemplatesForAdmin(
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
					t("errorLoadingMoreTemplates", "Error loading more templates") ||
					"Error loading more templates"
			);
		} finally {
			setLoadingMore(false);
		}
	}, [accessToken, page, loadingMore]);

	const handleSort = (sortConfig: SortConfig) => {
		const sorted = [...templates].sort((a, b) => {
			let aValue: any;
			let bValue: any;

			switch (sortConfig.key) {
				case "title":
					aValue = a.title.toLowerCase();
					bValue = b.title.toLowerCase();
					break;
				case "authorName":
					aValue = a.authorName.toLowerCase();
					bValue = b.authorName.toLowerCase();
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
					bValue = b.updatedAt;
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

	const handleViewTemplate = (template: TemplateDto) => {
		navigate(`/template/${template.id}/preview`);
	};

	const handleEditTemplate = (template: TemplateDto) => {
		navigate(`/template/${template.id}`);
	};

	const handleTogglePublishTemplate = async (template: TemplateDto) => {
		try {
			if (template.isPublished) {
				await templateApi.unpublishTemplate(template.id, accessToken);
				toast.success(
					t(
						"templateUnpublishedSuccessfully",
						"Template unpublished successfully"
					) || "Template unpublished successfully"
				);
			} else {
				await templateApi.publishTemplate(template.id, accessToken);
				toast.success(
					t(
						"templatePublishedSuccessfully",
						"Template published successfully"
					) || "Template published successfully"
				);
			}
			loadInitialTemplates();
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorTogglePublishTemplate", "Error changing template status") ||
					"Error changing template status"
			);
		}
	};

	const handleArchiveTemplate = async (template: TemplateDto) => {
		try {
			await templateApi.patchArchiveVersions([template.id], accessToken);
			toast.success(
				t("templateArchivedSuccessfully", "Template archived successfully") ||
					"Template archived successfully"
			);
			loadInitialTemplates();
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorArchivingTemplate", "Error archiving template") ||
					"Error archiving template"
			);
		}
	};

	const handleToggleDeleteTemplate = async (template: TemplateDto) => {
		try {
			if (template.isDeleted) {
				await templateApi.undeleteTemplate(template.id, accessToken);
				toast.success(
					t("templateDeletedSuccessfully", "Template deleted successfully") ||
						"Template deleted successfully"
				);
			} else {
				await templateApi.deleteTemplate(template.id, accessToken);
				toast.success(
					t("templateDeletedSuccessfully", "Template deleted successfully") ||
						"Template deleted successfully"
				);
			}
			loadInitialTemplates();
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorDeletingTemplate", "Error deleting template") ||
					"Error deleting template"
			);
		}
	};

	const handlePublishSelected = async () => {
		try {
			await Promise.all(
				Array.from(selectedTemplates).map((templateId) =>
					templateApi.publishTemplate(templateId, accessToken)
				)
			);
			toast.success(
				t(
					"templatesPublishedSuccessfully",
					"Templates published successfully"
				) || "Templates published successfully"
			);
			setSelectedTemplates(new Set());
			loadInitialTemplates();
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorPublishingTemplates", "Error publishing templates") ||
					"Error publishing templates"
			);
		}
	};

	const handleUnpublishSelected = async () => {
		try {
			await Promise.all(
				Array.from(selectedTemplates).map((templateId) =>
					templateApi.unpublishTemplate(templateId, accessToken)
				)
			);
			toast.success(
				t(
					"templatesUnpublishedSuccessfully",
					"Templates unpublished successfully"
				) || "Templates unpublished successfully"
			);
			setSelectedTemplates(new Set());
			loadInitialTemplates();
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorUnpublishingTemplates", "Error unpublishing templates") ||
					"Error unpublishing templates"
			);
		}
	};

	const handleDeleteSelected = async () => {
		try {
			await Promise.all(
				Array.from(selectedTemplates).map((templateId) =>
					templateApi.deleteTemplate(templateId, accessToken)
				)
			);
			toast.success(
				t("templatesDeletedSuccessfully", "Templates deleted successfully") ||
					"Templates deleted successfully"
			);
			setSelectedTemplates(new Set());
			loadInitialTemplates();
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorDeletingTemplates", "Error deleting templates") ||
					"Error deleting templates"
			);
		}
	};

	const getTemplateActions = (template: TemplateDto): ActionItem[] => [
		{
			id: "view",
			icon: "👁️",
			label: t("viewTemplate", "View Template") || "View Template",
			onClick: () => handleViewTemplate(template),
		},
		{
			id: "edit",
			icon: "✏️",
			label: t("editTemplate", "Edit Template") || "Edit Template",
			onClick: () => handleEditTemplate(template),
		},
		{
			id: "togglePublish",
			icon: template.isPublished ? "📤" : "📢",
			label: template.isPublished
				? t("unpublishTemplate", "Unpublish Template") || "Unpublish Template"
				: t("publishTemplate", "Publish Template") || "Publish Template",
			onClick: () => handleTogglePublishTemplate(template),
		},
		{
			id: "archive",
			icon: "🗂️",
			label: t("archiveTemplate", "Archive Template") || "Archive Template",
			onClick: () => handleArchiveTemplate(template),
		},
		{
			id: "delete",
			icon: template.isDeleted ? "♥️" : "🗑️",
			label: template.isDeleted
				? t("restore", "Restore") || "Restore"
				: t("deleteTemplate", "Delete Template") || "Delete Template",
			onClick: () => handleToggleDeleteTemplate(template),
			variant: "danger",
		},
	];

	const getStatusLabel = (template: TemplateDto) => {
		if (template.isDeleted) {
			return t("deleted") || "Deleted";
		}
		return template.isPublished
			? t("published", "Published") || "Published"
			: t("draft", "Draft") || "Draft";
	};

	const getAccessTypeLabel = (accessType: number) => {
		return accessType === 1
			? t("publicAccess", "Public") || "Public"
			: t("privateAccess", "Private") || "Private";
	};

	const tableColumns: TableColumn<TemplateDto>[] = columns
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
								className="max-w-48 truncate font-medium"
								title={template.title}
							>
								{template.title}
							</div>
						);
					case "authorName":
						return (
							<div className="flex items-center gap-2">
								<div className="w-6 h-6 bg-primary rounded-full flex items-center justify-center text-white text-xs font-medium">
									{template.authorName.charAt(0).toUpperCase()}
								</div>
								<span className="truncate">{template.authorName}</span>
							</div>
						);
					case "topic":
						return template.topic || t("noTopic", "No topic") || "No topic";
					case "isPublished":
						return (
							<span
								className={`px-2 py-1 rounded-full text-xs font-medium ${
									template.isPublished
										? "bg-success/10 text-success"
										: "bg-warning/10 text-warning"
								}`}
							>
								{getStatusLabel(template)}
							</span>
						);
					case "accessType":
						return (
							<span
								className={`px-2 py-1 rounded-full text-xs font-medium ${
									template.accessType === 1
										? "bg-primary/10 text-primary"
										: "bg-textMuted/10 text-textMuted"
								}`}
							>
								{getAccessTypeLabel(template.accessType)}
							</span>
						);
					case "formsCount":
						return template.formsCount;
					case "likesCount":
						return template.likesCount;
					case "commentsCount":
						return template.commentsCount;
					case "createdAt":
						return new Date(template.createdAt).toLocaleDateString();
					case "updatedAt":
						return new Date(template.updatedAt).toLocaleDateString();
					default:
						return "";
				}
			},
		}));

	if (loading) {
		return (
			<div className="flex items-center justify-center py-8">
				<div className="text-textMuted">
					{t("loadingTemplates", "Loading templates...") ||
						"Loading templates..."}
				</div>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<div className="flex items-center gap-3">
					<span className="text-sm text-textMuted">
						{t("totalTemplates", "Total templates") || "Total templates"}:{" "}
						{totalCount}
					</span>
					{selectedTemplates.size > 0 && (
						<>
							<button
								onClick={handlePublishSelected}
								className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity text-sm"
							>
								{t("publishSelected", "Publish Selected") || "Publish Selected"}{" "}
								({selectedTemplates.size})
							</button>
							<button
								onClick={handleUnpublishSelected}
								className="px-4 py-2 bg-warning text-white rounded-lg hover:opacity-90 transition-opacity text-sm"
							>
								{t("unpublishSelected", "Unpublish Selected") ||
									"Unpublish Selected"}{" "}
								({selectedTemplates.size})
							</button>
							<button
								onClick={handleDeleteSelected}
								className="px-4 py-2 bg-error text-white rounded-lg hover:opacity-90 transition-opacity text-sm"
							>
								{t("deleteSelected", "Delete Selected") || "Delete Selected"} (
								{selectedTemplates.size})
							</button>
						</>
					)}
				</div>

				<div className="relative">
					<button
						onClick={(e) => {
							e.stopPropagation();
							setShowColumnDropdown(!showColumnDropdown);
						}}
						className="px-3 py-2 border border-border rounded-lg text-text hover:bg-surface transition-colors text-sm"
					>
						{t("columns", "Columns") || "Columns"} ▼
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
				columns={tableColumns}
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
		</div>
	);
};
