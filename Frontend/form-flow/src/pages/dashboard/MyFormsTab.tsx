import React, { useState, useEffect, useCallback } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import { FormDto } from "../../shared/api_types";
import { formApi } from "../../api/formApi";
import { SortableTable, SortConfig, TableColumn } from "../../ui/SortableTable";
import {
	ActionItem,
	ActionPanel,
} from "../../modules/templates/components/editorPageTabs/ActionPanel";

interface MyFormsTabProps {
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
	{
		key: "templateAuthor",
		label: "Template Author",
		visible: true,
		sortable: true,
	},
	{ key: "submittedAt", label: "Submitted", visible: true, sortable: true },
	{ key: "updatedAt", label: "Updated", visible: false, sortable: true },
];

export const MyFormsTab: React.FC<MyFormsTabProps> = ({ accessToken }) => {
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

	const pageSize = 20;

	useEffect(() => {
		loadInitialForms();
	}, [accessToken]);

	useEffect(() => {
		setSortedForms([...forms]);
	}, [forms]);

	const loadInitialForms = async () => {
		try {
			setLoading(true);
			const result = await formApi.getMyForms(1, pageSize, accessToken);
			setForms(result.data);
			setPage(1);
			setHasMore(result.data.length === pageSize);
		} catch (error: any) {
			toast.error(
				error.message || t("errorLoadingForms") || "Error loading forms"
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
			const result = await formApi.getMyForms(nextPage, pageSize, accessToken);

			if (result.data.length > 0) {
				setForms((prev) => [...prev, ...result.data]);
				setPage(nextPage);
				setHasMore(result.data.length === pageSize);
			} else {
				setHasMore(false);
			}
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorLoadingMoreForms") ||
					"Error loading more forms"
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
				case "templateAuthor":
					aValue = a.userName.toLowerCase();
					bValue = b.userName.toLowerCase();
					break;
				case "submittedAt":
					aValue = new Date(a.submittedAt);
					bValue = new Date(b.submittedAt);
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

		setSortedForms(sorted);
	};

	const toggleColumnVisibility = (columnKey: string) => {
		setColumns((prev) =>
			prev.map((col) =>
				col.key === columnKey ? { ...col, visible: !col.visible } : col
			)
		);
	};

	const handleFormView = (form: FormDto) => {
		navigate(`/form/view/${form.id}`);
	};

	const handleFormEdit = (form: FormDto) => {
		navigate(`/form/edit/${form.id}`);
	};

	const getFormActions = (form: FormDto): ActionItem[] => [
		{
			id: form.id + "_view",
			label: t("view") || "View",
			icon: "👁️",
			onClick: () => handleFormView(form),
		},
		{
			id: form.id + "_edit",
			label: t("editForm") || "Edit Form",
			icon: "✏️",
			onClick: () => handleFormEdit(form),
		},
	];

	const getTableColumns = (): TableColumn<FormDto>[] => {
		return columns
			.filter((col) => col.visible)
			.map((col) => ({
				key: col.key,
				label: col.label,
				sortable: col.sortable,
				render: (form: FormDto) => {
					switch (col.key) {
						case "templateName":
							return (
								<div
									className="font-medium text-text hover:text-primary cursor-pointer"
									onClick={() => handleFormView(form)}
								>
									{form.templateName}
								</div>
							);
						case "templateAuthor":
							return <span className="text-textMuted">{form.userName}</span>;
						case "submittedAt":
							return (
								<span className="text-textMuted">
									{new Date(form.submittedAt).toLocaleDateString()}
								</span>
							);
						case "updatedAt":
							return (
								<span className="text-textMuted">
									{new Date(form.updatedAt).toLocaleDateString()}
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
				<span className="text-textMuted">
					{t("loadingForms") || "Loading forms..."}
				</span>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<div className="flex items-center gap-4">
					{selectedForms.size > 0 && (
						<span className="text-textMuted">
							{t("selectedForms") || "Selected"}: {selectedForms.size}
						</span>
					)}
				</div>

				<div className="relative">
					<button
						onClick={() => setShowColumnDropdown(!showColumnDropdown)}
						className="px-3 py-2 border border-border rounded-lg text-text hover:bg-surface transition-colors text-sm"
					>
						{t("columns") || "Columns"} ▼
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
				data={sortedForms}
				columns={getTableColumns()}
				onSort={handleSort}
				selectable={true}
				selectedItems={selectedForms}
				onSelectionChange={setSelectedForms}
				getItemId={(form) => form.id}
				emptyMessage={t("noFormsFound") || "No forms found"}
				onRowHover={setHoveredRowId}
				onReachEnd={hasMore ? loadMoreForms : undefined}
				isLoadingMore={loadingMore}
				renderRow={(form) => (
					<td className="absolute inset-0 pointer-events-none">
						<ActionPanel
							visible={hoveredRowId === form.id}
							actions={getFormActions(form)}
							position="right"
							className="pointer-events-auto z-10"
						/>
					</td>
				)}
			/>
		</div>
	);
};
