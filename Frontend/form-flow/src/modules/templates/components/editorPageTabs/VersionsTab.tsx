import React, {
	useState,
	useEffect,
	forwardRef,
	useImperativeHandle,
} from "react";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import { TemplateDto } from "../../../../shared/api_types";
import { templateApi } from "../../../../api/templateApi";
import { SortableTable, SortConfig, TableColumn } from "./SortableTable";
import { ActionItem, ActionPanel } from "./ActionPanel";

interface VersionsTabProps {
	template: TemplateDto;
	accessToken: string | null;
	hasChangeCallback?: (hasChanges: boolean) => void;
}

export interface VersionsTabRef {
	save: () => Promise<void>;
}

interface ColumnConfig {
	key: string;
	label: string;
	visible: boolean;
	sortable: boolean;
}

const defaultColumns: ColumnConfig[] = [
	{ key: "version", label: "Version", visible: true, sortable: true },
	{ key: "current", label: "Current", visible: true, sortable: true },
	{ key: "published", label: "Published", visible: true, sortable: true },
	{ key: "createdAt", label: "Created", visible: true, sortable: true },
	{ key: "updatedAt", label: "Updated", visible: true, sortable: true },
	{ key: "author", label: "Author", visible: false, sortable: true },
	{ key: "formsCount", label: "Forms", visible: false, sortable: true },
	{ key: "likesCount", label: "Likes", visible: false, sortable: true },
	{ key: "commentsCount", label: "Comments", visible: false, sortable: true },
];

export const VersionsTab = forwardRef<VersionsTabRef, VersionsTabProps>(
	({ template, accessToken, hasChangeCallback }, ref) => {
		const { t } = useTranslation();
		const [versions, setVersions] = useState<TemplateDto[]>([]);
		const [sortedVersions, setSortedVersions] = useState<TemplateDto[]>([]);
		const [loading, setLoading] = useState(true);
		const [selectedVersions, setSelectedVersions] = useState<Set<string>>(
			new Set()
		);
		const [columns, setColumns] = useState<ColumnConfig[]>(defaultColumns);
		const [showColumnDropdown, setShowColumnDropdown] = useState(false);
		const [hoveredRowId, setHoveredRowId] = useState<string | null>(null);

		useImperativeHandle(ref, () => ({
			save: async () => {},
		}));

		useEffect(() => {
			loadVersions();
		}, [template.id, accessToken]);

		useEffect(() => {
			setSortedVersions([...versions]);
		}, [versions]);

		const loadVersions = async () => {
			if (!accessToken) return;

			try {
				setLoading(true);
				const versionsData = await templateApi.getTemplateVersions(
					template.id,
					accessToken
				);
				setVersions(versionsData);
			} catch (error: any) {
				toast.error(
					error.message ||
						t("errorLoadingVersions", "Error loading versions") ||
						"Error loading versions"
				);
			} finally {
				setLoading(false);
			}
		};

		const handleSort = (sortConfig: SortConfig) => {
			const sorted = [...versions].sort((a, b) => {
				let aValue: any;
				let bValue: any;

				switch (sortConfig.key) {
					case "version":
						aValue = a.version;
						bValue = b.version;
						break;
					case "current":
						aValue = a.isCurrentVersion ? 1 : 0;
						bValue = b.isCurrentVersion ? 1 : 0;
						break;
					case "published":
						aValue = a.isPublished ? 1 : 0;
						bValue = b.isPublished ? 1 : 0;
						break;
					case "createdAt":
						aValue = new Date(a.createdAt);
						bValue = new Date(b.createdAt);
						break;
					case "updatedAt":
						aValue = new Date(a.updatedAt);
						bValue = new Date(b.updatedAt);
						break;
					case "author":
						aValue = a.authorName;
						bValue = b.authorName;
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
					default:
						return 0;
				}

				if (aValue < bValue) return sortConfig.direction === "asc" ? -1 : 1;
				if (aValue > bValue) return sortConfig.direction === "asc" ? 1 : -1;
				return 0;
			});

			setSortedVersions(sorted);
		};

		const toggleColumnVisibility = (columnKey: string) => {
			setColumns((prev) =>
				prev.map((col) =>
					col.key === columnKey ? { ...col, visible: !col.visible } : col
				)
			);
		};

		const handleCreateNewVersion = () => {
			console.log("Create new version");
		};

		const handleArchiveSelected = () => {
			console.log("Archive selected versions:", Array.from(selectedVersions));
		};

		const handleDeleteSelected = () => {
			console.log("Delete selected versions:", Array.from(selectedVersions));
		};

		const handleViewVersion = (version: TemplateDto) => {
			window.open(`/template/${version.id}`, "_blank");
		};

		const handleEditVersion = (version: TemplateDto) => {
			window.location.href = `/template/${version.id}`;
		};

		const handleCloneVersion = (version: TemplateDto) => {
			console.log("Clone version:", version.id);
		};

		const handleDeleteVersion = (version: TemplateDto) => {
			console.log("Delete version:", version.id);
		};

		const getVersionActions = (version: TemplateDto): ActionItem[] => [
			{
				id: "view",
				icon: "👁️",
				label: t("view", "View") || "View",
				onClick: () => handleViewVersion(version),
			},
			{
				id: "edit",
				icon: "✏️",
				label: t("edit", "Edit") || "Edit",
				onClick: () => handleEditVersion(version),
			},
			{
				id: "clone",
				icon: "📋",
				label: t("clone", "Clone") || "Clone",
				onClick: () => handleCloneVersion(version),
			},
			{
				id: "delete",
				icon: "🗑️",
				label: t("delete", "Delete") || "Delete",
				onClick: () => handleDeleteVersion(version),
				variant: "danger" as const,
			},
		];

		const tableColumns: TableColumn<TemplateDto>[] = columns
			.filter((col) => col.visible)
			.map((col) => ({
				key: col.key,
				label: col.label,
				sortable: col.sortable,
				render: (version: TemplateDto) => {
					switch (col.key) {
						case "version":
							return `v${version.version}`;
						case "current":
							return version.isCurrentVersion ? (
								<span className="text-success">✅</span>
							) : (
								""
							);
						case "published":
							return version.isPublished ? (
								<span className="text-primary">🔵</span>
							) : (
								""
							);
						case "createdAt":
							return new Date(version.createdAt).toLocaleDateString();
						case "updatedAt":
							return new Date(version.updatedAt).toLocaleDateString();
						case "author":
							return version.authorName;
						case "formsCount":
							return version.formsCount;
						case "likesCount":
							return version.likesCount;
						case "commentsCount":
							return version.commentsCount;
						default:
							return "";
					}
				},
			}));

		if (loading) {
			return (
				<div className="flex items-center justify-center py-8">
					<div className="text-textMuted">
						{t("loading", "Loading...") || "Loading..."}
					</div>
				</div>
			);
		}

		return (
			<div className="space-y-4">
				<div className="flex items-center justify-between">
					<div className="flex items-center gap-3">
						<button
							onClick={handleCreateNewVersion}
							className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
						>
							{t("createNewVersion", "Create New Version") ||
								"Create New Version"}
						</button>

						{selectedVersions.size > 0 && (
							<>
								<button
									onClick={handleArchiveSelected}
									className="px-4 py-2 bg-warning text-white rounded-lg hover:opacity-90 transition-opacity"
								>
									{t("archiveSelected", "Archive Selected") ||
										"Archive Selected"}{" "}
									({selectedVersions.size})
								</button>
								<button
									onClick={handleDeleteSelected}
									className="px-4 py-2 bg-error text-white rounded-lg hover:opacity-90 transition-opacity"
								>
									{t("deleteSelected", "Delete Selected") || "Delete Selected"}{" "}
									({selectedVersions.size})
								</button>
							</>
						)}
					</div>

					<div className="relative">
						<button
							onClick={() => setShowColumnDropdown(!showColumnDropdown)}
							className="px-3 py-2 border border-border rounded-lg text-text hover:bg-surface transition-colors"
						>
							{t("columns", "Columns") || "Columns"} ▼
						</button>

						{showColumnDropdown && (
							<div className="absolute right-0 top-full mt-1 bg-surface border border-border rounded-lg shadow-lg z-10 min-w-48 p-2">
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
					data={sortedVersions}
					columns={tableColumns}
					onSort={handleSort}
					selectable={true}
					selectedItems={selectedVersions}
					onSelectionChange={setSelectedVersions}
					getItemId={(version) => version.id}
					emptyMessage={
						t("noVersionsYet", "No versions available") ||
						"No versions available"
					}
					onRowHover={setHoveredRowId}
					renderRow={(version) => (
						<td className="absolute inset-0 pointer-events-none">
							<ActionPanel
								visible={hoveredRowId === version.id}
								actions={getVersionActions(version)}
								position="right"
								className="pointer-events-auto"
							/>
						</td>
					)}
				/>
			</div>
		);
	}
);
