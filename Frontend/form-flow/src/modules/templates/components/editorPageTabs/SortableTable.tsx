import React, { useState } from "react";

export interface TableColumn<T> {
	key: keyof T | string;
	label: string;
	sortable?: boolean;
	visible?: boolean;
	render?: (item: T) => React.ReactNode;
	width?: string;
}

export interface SortConfig {
	key: string;
	direction: "asc" | "desc";
}

interface SortableTableProps<T> {
	data: T[];
	columns: TableColumn<T>[];
	onSort?: (sortConfig: SortConfig) => void;
	renderRow?: (item: T, index: number) => React.ReactNode;
	selectable?: boolean;
	selectedItems?: Set<string>;
	onSelectionChange?: (selectedItems: Set<string>) => void;
	getItemId?: (item: T) => string;
	emptyMessage?: string;
	className?: string;
	onRowHover?: (itemId: string | null) => void;
	maxColumnLabelLength?: number;
	maxColumnsWithoutScroll?: number;
	onReachEnd?: () => void;
	isLoadingMore?: boolean;
}

export function SortableTable<T>({
	data,
	columns,
	onSort,
	renderRow,
	selectable = false,
	selectedItems = new Set(),
	onSelectionChange,
	getItemId,
	emptyMessage = "No data available",
	className = "",
	onRowHover,
	maxColumnLabelLength = 15,
	maxColumnsWithoutScroll = 6,
}: SortableTableProps<T>) {
	const [sortConfig, setSortConfig] = useState<SortConfig | null>(null);

	const visibleColumns = columns.filter((col) => col.visible !== false);
	const needsHorizontalScroll = visibleColumns.length > maxColumnsWithoutScroll;

	const handleSort = (columnKey: string) => {
		const column = columns.find((col) => col.key === columnKey);
		if (!column?.sortable) return;

		let direction: "asc" | "desc" = "asc";
		if (
			sortConfig &&
			sortConfig.key === columnKey &&
			sortConfig.direction === "asc"
		) {
			direction = "desc";
		}

		const newSortConfig = { key: columnKey, direction };
		setSortConfig(newSortConfig);
		onSort?.(newSortConfig);
	};

	const truncateLabel = (label: string) => {
		if (label.length <= maxColumnLabelLength) return label;
		return label.substring(0, maxColumnLabelLength) + "...";
	};

	const toggleSelectAll = () => {
		if (!selectable || !onSelectionChange || !getItemId) return;

		if (selectedItems.size === data.length) {
			onSelectionChange(new Set());
		} else {
			onSelectionChange(new Set(data.map(getItemId)));
		}
	};

	const toggleItemSelection = (itemId: string) => {
		if (!selectable || !onSelectionChange) return;

		const newSelection = new Set(selectedItems);
		if (newSelection.has(itemId)) {
			newSelection.delete(itemId);
		} else {
			newSelection.add(itemId);
		}
		// console.log(newSelection);
		onSelectionChange(newSelection);
	};

	const getSortIcon = (columnKey: string) => {
		if (!sortConfig || sortConfig.key !== columnKey) {
			return <span className="text-textMuted ml-1" />;
		}
		return sortConfig.direction === "asc" ? (
			<span className="text-primary ml-1">↑</span>
		) : (
			<span className="text-primary ml-1">↓</span>
		);
	};

	const defaultRenderCell = (item: T, column: TableColumn<T>) => {
		if (column.render) {
			return column.render(item);
		}

		const value = item[column.key as keyof T];
		if (value instanceof Date) {
			return value.toLocaleDateString();
		}
		return String(value || "");
	};

	return (
		<div
			className={`bg-surface border border-border rounded-lg overflow-hidden ${className}`}
		>
			<div className="overflow-x-auto">
				<table className="w-full min-w-max">
					<thead className="bg-background border-b border-border">
						<tr>
							{selectable && (
								<th className="w-12 px-4 py-3 text-left">
									<input
										type="checkbox"
										checked={
											selectedItems.size === data.length && data.length > 0
										}
										onChange={toggleSelectAll}
										className="text-primary"
									/>
								</th>
							)}
							{visibleColumns.map((column) => (
								<th
									key={String(column.key)}
									className={`px-4 py-3 text-left text-sm font-medium text-text ${
										column.width || ""
									}`}
								>
									{column.sortable ? (
										<button
											onClick={() => handleSort(String(column.key))}
											className="flex items-center hover:text-primary transition-colors"
											title={column.label}
										>
											{truncateLabel(column.label)}
											{/* {column.label} */}
											{getSortIcon(String(column.key))}
										</button>
									) : (
										<span title={column.label}>
											{truncateLabel(column.label)}
										</span>
									)}
								</th>
							))}
						</tr>
					</thead>
					<tbody>
						{data.map((item, index) => {
							const itemId = getItemId?.(item) || String(index);
							return (
								<tr
									key={itemId}
									onMouseEnter={() => onRowHover?.(itemId)}
									onMouseLeave={() => onRowHover?.(null)}
									className="border-b border-border hover:bg-background/50 group relative"
								>
									{selectable && (
										<td className="px-4 py-3">
											<input
												type="checkbox"
												checked={selectedItems.has(itemId)}
												onChange={() => toggleItemSelection(itemId)}
												className="text-primary"
											/>
										</td>
									)}
									{visibleColumns.map((column) => (
										<td
											key={String(column.key)}
											className={`px-4 py-3 text-sm text-text ${
												needsHorizontalScroll
													? "min-w-32 whitespace-nowrap"
													: ""
											}`}
										>
											{defaultRenderCell(item, column)}
										</td>
									))}
									{renderRow?.(item, index)}
								</tr>
							);
						})}
					</tbody>
				</table>
			</div>

			{/* <div ref={endTriggerRef} className="h-1">
      {isLoadingMore && (
        <div className="flex justify-center py-4 border-t border-border">
          <span className="text-textMuted text-sm">Loading more...</span>
        </div>
      )}
    </div> */}

			{data.length === 0 && (
				<div className="text-center py-8 text-textMuted">{emptyMessage}</div>
			)}
		</div>
	);
}
