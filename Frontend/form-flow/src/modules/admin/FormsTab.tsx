import React, { useState, useEffect, useCallback } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import {
	SortableTable,
	SortConfig,
	TableColumn,
} from "../templates/components/editorPageTabs/SortableTable";
import { FormDto } from "../../shared/api_types";
import { formApi } from "../../api/formApi";
import {
	ActionItem,
	ActionPanel,
} from "../templates/components/editorPageTabs/ActionPanel";
import * as Dialog from "@radix-ui/react-dialog";

interface FormsTabProps {
	accessToken: string;
}

interface ColumnConfig {
	key: string;
	label: string;
	visible: boolean;
	sortable: boolean;
}

const defaultColumns: ColumnConfig[] = [
	{ key: "templateName", label: "Template", visible: true, sortable: true },
	{ key: "userName", label: "Author", visible: true, sortable: true },
	{
		key: "templateCreator",
		label: "Template Creator",
		visible: true,
		sortable: true,
	},
	{ key: "submittedAt", label: "Submitted", visible: true, sortable: true },
];

export const FormsTab: React.FC<FormsTabProps> = ({ accessToken }) => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const [forms, setForms] = useState<FormDto[]>([]);
	const [sortedForms, setSortedForms] = useState<FormDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [loadingMore, setLoadingMore] = useState(false);
	const [hasMore, setHasMore] = useState(true);
	const [page, setPage] = useState(1);
	const [selectedForms, setSelectedForms] = useState<Set<string>>(new Set());
	const [columns, setColumns] = useState<ColumnConfig[]>(defaultColumns);
	const [showColumnDropdown, setShowColumnDropdown] = useState(false);
	const [hoveredRowId, setHoveredRowId] = useState<string | null>(null);
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);
	const [formToDelete, setFormToDelete] = useState<FormDto | null>(null);

	const pageSize = 2;

	useEffect(() => {
		loadInitialForms();
	}, [accessToken]);

	useEffect(() => {
		setSortedForms([...forms]);
	}, [forms]);

	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (showColumnDropdown) {
				setShowColumnDropdown(false);
			}
		};

		document.addEventListener("click", handleClickOutside);
		return () => document.removeEventListener("click", handleClickOutside);
	}, [showColumnDropdown]);

	const loadInitialForms = async () => {
		try {
			setLoading(true);
			const result = await formApi.getAllFormsForAdmin(
				1,
				pageSize,
				accessToken
			);
			console.log(result);
			setForms(result.data);
			setPage(1);
			setHasMore(result.data.length === pageSize);
		} catch (error: any) {
			toast.error(
				error.message || t("errorLoadingForms", "Error loading forms")
			);
		} finally {
			setLoading(false);
		}
	};

	const loadMoreForms = useCallback(async () => {
		if (!accessToken || loadingMore) return;

		try {
			setLoadingMore(true);
			const nextPage = page + 1;
			const result = await formApi.getAllFormsForAdmin(
				nextPage,
				pageSize,
				accessToken
			);

			if (result.data.length > 0) {
				setForms((prev) => [...prev, ...result.data]);
				setPage(nextPage);
				setHasMore(result.data.length === pageSize);
			} else {
				setHasMore(false);
			}
		} catch (error: any) {
			toast.error(
				error.message || t("errorLoadingMoreForms", "Error loading more forms")
			);
		} finally {
			setLoadingMore(false);
		}
	}, [accessToken, page, loadingMore]);

	const handleSort = (sortConfig: SortConfig) => {
		const sorted = [...forms].sort((a, b) => {
			let aValue: any;
			let bValue: any;

			switch (sortConfig.key) {
				case "templateName":
					aValue = a.templateName.toLowerCase();
					bValue = b.templateName.toLowerCase();
					break;
				case "userName":
					aValue = a.userName.toLowerCase();
					bValue = b.userName.toLowerCase();
					break;
				case "submittedAt":
					aValue = new Date(a.submittedAt);
					bValue = new Date(b.submittedAt);
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

		setSortedForms(sorted);
	};

	const handleViewForm = (form: FormDto) => {
		navigate(`/form/view/${form.id}`);
	};

	const handleEditForm = (form: FormDto) => {
		navigate(`/form/edit/${form.id}`);
	};

	const handleViewTemplate = (form: FormDto) => {
		navigate(`/template/${form.templateId}`);
	};

	const handleDeleteForm = async (form: FormDto) => {
		try {
			await formApi.deleteForm(form.id, accessToken);
			setForms((prev) => prev.filter((f) => f.id !== form.id));
			toast.success(t("formDeleted", "Form has been deleted"));
		} catch (error: any) {
			toast.error(
				error.message || t("errorDeletingForm", "Error deleting form")
			);
		}
	};

	const toggleColumnVisibility = (key: string) => {
		setColumns((prev) =>
			prev.map((col) =>
				col.key === key ? { ...col, visible: !col.visible } : col
			)
		);
	};

	const tableColumns: TableColumn<FormDto>[] = columns
		.filter((col) => col.visible)
		.map((col) => ({
			key: col.key,
			label: col.label,
			sortable: col.sortable,
			render: (form: FormDto) => {
				switch (col.key) {
					case "templateName":
						return (
							<button
								onClick={() => handleViewTemplate(form)}
								className="text-primary hover:underline text-left"
							>
								{form.templateName}
							</button>
						);
					case "userName":
						return form.userName;
					case "submittedAt":
						return new Date(form.submittedAt).toLocaleDateString();
					default:
						return String(form[col.key as keyof FormDto] || "");
				}
			},
		}));

	const getFormActions = (form: FormDto): ActionItem[] => {
		const actions: ActionItem[] = [
			{
				id: form.id + "View",
				label: t("viewForm", "View Form"),
				icon: "👁️",
				onClick: () => handleViewForm(form),
			},
			{
				id: form.id + "Edit",
				label: t("editForm", "Edit Form"),
				icon: "✏️",
				onClick: () => handleEditForm(form),
			},
			{
				id: form.id + "ViewTemplate",
				label: t("viewTemplate", "View Template"),
				icon: "📋",
				onClick: () => handleViewTemplate(form),
			},
		];

		actions.push({
			id: form.id + "Delete",
			label: t("delete", "Delete"),
			icon: "🗑️",
			onClick: () => {
				setFormToDelete(form);
				setShowDeleteDialog(true);
			},
			variant: "danger",
		});

		return actions;
	};

	if (loading) {
		return (
			<div className="flex justify-center items-center py-8">
				<div className="text-textMuted">
					{t("loadingForms", "Loading forms...")}
				</div>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<h2 className="text-xl font-semibold text-text">
					{t("forms", "Forms")} ({forms.length})
				</h2>

				<div className="flex items-center gap-4">
					<div className="relative">
						<button
							onClick={(e) => {
								e.stopPropagation();
								setShowColumnDropdown(!showColumnDropdown);
							}}
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
			</div>

			<SortableTable
				data={sortedForms}
				columns={tableColumns}
				onSort={handleSort}
				selectable={true}
				selectedItems={selectedForms}
				onSelectionChange={setSelectedForms}
				getItemId={(form) => form.id}
				emptyMessage={t("noFormsFound", "No forms found") || "No forms found"}
				onRowHover={setHoveredRowId}
				onReachEnd={hasMore ? loadMoreForms : undefined}
				isLoadingMore={loadingMore}
				renderRow={(form) => (
					<td className="absolute inset-0 pointer-events-none">
						<ActionPanel
							visible={hoveredRowId === form.id}
							actions={getFormActions(form)}
							position="right"
							className="pointer-events-auto"
						/>
					</td>
				)}
			/>

			<Dialog.Root open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
				<Dialog.Portal>
					<Dialog.Overlay className="fixed inset-0 bg-black/50 z-50" />
					<Dialog.Content className="fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-surface border border-border rounded-lg p-6 z-50 w-96">
						<Dialog.Title className="text-lg font-semibold text-text mb-4">
							{t("confirmDelete", "Confirm Delete")}
						</Dialog.Title>
						<p className="text-textMuted mb-6">
							{t(
								"deleteFormConfirm",
								"Are you sure you want to delete this form? This action cannot be undone."
							)}
						</p>
						<div className="flex justify-end gap-3">
							<Dialog.Close asChild>
								<button className="px-4 py-2 border border-border rounded-lg text-text hover:bg-background transition-colors">
									{t("cancel", "Cancel")}
								</button>
							</Dialog.Close>
							<button
								onClick={() => {
									if (formToDelete) {
										handleDeleteForm(formToDelete);
										setShowDeleteDialog(false);
										setFormToDelete(null);
									}
								}}
								className="px-4 py-2 bg-error text-white rounded-lg hover:bg-error/90 transition-colors"
							>
								{t("delete", "Delete")}
							</button>
						</div>
					</Dialog.Content>
				</Dialog.Portal>
			</Dialog.Root>
		</div>
	);
};
