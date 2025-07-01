import React, { useState, useEffect, useCallback } from "react";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import { SortableTable, SortConfig, TableColumn } from "../../ui/SortableTable";
import { UserDto } from "../../shared/api_types";
import { usersApi } from "../../api/usersApi";
import {
	ActionItem,
	ActionPanel,
} from "../templates/components/editorPageTabs/ActionPanel";
import * as Dialog from "@radix-ui/react-dialog";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/hooks/useAuth";
import { useAuthStore } from "../auth/store/authStore";

interface UsersTabProps {
	accessToken: string;
}

interface ColumnConfig {
	key: string;
	label: string;
	visible: boolean;
	sortable: boolean;
}

const defaultColumns: ColumnConfig[] = [
	{ key: "userName", label: "Username", visible: true, sortable: true },
	{ key: "email", label: "Email", visible: true, sortable: true },
	{ key: "role", label: "Role", visible: true, sortable: true },
	{ key: "isBlocked", label: "Status", visible: true, sortable: true },
	{ key: "createdAt", label: "Created", visible: true, sortable: true },
	{ key: "updatedAt", label: "Last Login", visible: false, sortable: true },
	{ key: "templatesCount", label: "Templates", visible: false, sortable: true },
	{ key: "formsCount", label: "Forms", visible: false, sortable: true },
];

export const UsersTab: React.FC<UsersTabProps> = ({ accessToken }) => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const { user } = useAuth();
	const { updateAuthData } = useAuthStore();
	const [users, setUsers] = useState<UserDto[]>([]);
	const [sortedUsers, setSortedUsers] = useState<UserDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [selectedUsers, setSelectedUsers] = useState<Set<string>>(new Set());
	const [columns, setColumns] = useState<ColumnConfig[]>(defaultColumns);
	const [showColumnDropdown, setShowColumnDropdown] = useState(false);
	const [hoveredRowId, setHoveredRowId] = useState<string | null>(null);
	// const [currentPage, setCurrentPage] = useState(1);
	// const [totalPages, setTotalPages] = useState(1);
	const [totalCount, setTotalCount] = useState(0);
	const [loadingMore, setLoadingMore] = useState(false);
	const [hasMore, setHasMore] = useState(true);
	const [page, setPage] = useState(1);
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);
	const [userToDelete, setUserToDelete] = useState<UserDto | null>(null);

	const pageSize = 3;

	useEffect(() => {
		loadInitialUsers();
	}, [accessToken]);

	useEffect(() => {
		setSortedUsers([...users]);
	}, [users]);

	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (showColumnDropdown) {
				setShowColumnDropdown(false);
			}
		};

		document.addEventListener("click", handleClickOutside);
		return () => document.removeEventListener("click", handleClickOutside);
	}, [showColumnDropdown]);

	const loadInitialUsers = async () => {
		try {
			setLoading(true);
			const result = await usersApi.getUsers(1, pageSize, accessToken);
			setUsers(result.data);
			setPage(1);
			setTotalCount(result.pagination.totalCount);
			setHasMore(result.data.length === pageSize);
		} catch (error: any) {
			toast.error(
				error.message || t("errorLoadingUsers", "Error loading users")
			);
		} finally {
			setLoading(false);
		}
	};

	const loadMoreUsers = useCallback(async () => {
		if (!accessToken || loadingMore) return;

		try {
			setLoadingMore(true);
			const nextPage = page + 1;
			const result = await usersApi.getUsers(nextPage, pageSize, accessToken);

			if (result.data.length > 0) {
				setUsers((prev) => [...prev, ...result.data]);
				setPage(nextPage);
				setHasMore(result.data.length === pageSize);
			} else {
				setHasMore(false);
			}
		} catch (error: any) {
			toast.error(
				error.message || t("errorLoadingMoreUsers", "Error loading more users")
			);
		} finally {
			setLoadingMore(false);
		}
	}, [accessToken, page, loadingMore]);

	//#region Handlers

	const handleSort = (sortConfig: SortConfig) => {
		const sorted = [...users].sort((a, b) => {
			let aValue: any;
			let bValue: any;

			switch (sortConfig.key) {
				case "userName":
					aValue = a.userName.toLowerCase();
					bValue = b.userName.toLowerCase();
					break;
				case "email":
					aValue = a.contacts?.[0]?.value?.toLowerCase() || "";
					bValue = b.contacts?.[0]?.value?.toLowerCase() || "";
					break;
				case "role":
					aValue = a.role;
					bValue = b.role;
					break;
				case "isBlocked":
					aValue = a.isBlocked ? 1 : 0;
					bValue = b.isBlocked ? 1 : 0;
					break;
				case "createdAt":
					aValue = new Date(a.createdAt);
					bValue = new Date(b.createdAt);
					break;
				case "updatedAt":
					aValue = new Date(a.updatedAt);
					bValue = new Date(b.updatedAt);
					break;
				case "templatesCount":
					aValue = a.templatesCount;
					bValue = b.templatesCount;
					break;
				case "formsCount":
					aValue = a.formsCount;
					bValue = b.formsCount;
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

		setSortedUsers(sorted);
	};

	const handleProfileUser = async (user: UserDto) => {
		navigate(`/profile/${user.id}`);
	};

	const handleBlockUser = async (user: UserDto) => {
		try {
			if (user.isBlocked) {
				await usersApi.unblockUser(user.id, accessToken);
				toast.success(
					t("userUnblockedSuccessfully", "User unblocked successfully")
				);
			} else {
				await usersApi.blockUser(user.id, accessToken);
				toast.success(
					t("userBlockedSuccessfully", "User blocked successfully")
				);
			}
			loadInitialUsers();
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorBlockingUser", "Error blocking/unblocking user")
			);
		}
	};

	const handleToggleAdmin = async (otherUser: UserDto) => {
		try {
			if (user && user.id === otherUser.id) {
				const result = await usersApi.toggleAdminRoleMyself(
					otherUser.id,
					accessToken
				);
				updateAuthData({
					user: result.user,
					accessToken: result.accessToken,
					refreshToken: result.refreshToken,
					accessTokenExpiry: result.accessTokenExpiry,
					refreshTokenExpiry: result.refreshTokenExpiry,
					authType: result.authType,
					isAuthenticated: true,
				});
			} else {
				await usersApi.toggleAdminRole(otherUser.id, accessToken);
			}
			toast.success("Successfully promoted!");
			loadInitialUsers();
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorManagingAdminRole", "Error managing admin role")
			);
		}
	};

	const handleBlockSelected = async () => {
		try {
			await Promise.all(
				Array.from(selectedUsers).map((userId) =>
					usersApi.blockUser(userId, accessToken)
				)
			);
			toast.success(
				t("usersBlockedSuccessfully", "Users blocked successfully")
			);
			setSelectedUsers(new Set());
			loadInitialUsers();
		} catch (error: any) {
			toast.error(
				error.message || t("errorBlockingUsers", "Error blocking users")
			);
		}
	};

	const handleUnblockSelected = async () => {
		try {
			await Promise.all(
				Array.from(selectedUsers).map((userId) =>
					usersApi.unblockUser(userId, accessToken)
				)
			);
			toast.success(
				t("usersUnblockedSuccessfully", "Users unblocked successfully")
			);
			setSelectedUsers(new Set());
			loadInitialUsers();
		} catch (error: any) {
			toast.error(
				error.message || t("errorUnblockingUsers", "Error unblocking users")
			);
		}
	};
	//#endregion

	const toggleColumnVisibility = (columnKey: string) => {
		setColumns((prev) =>
			prev.map((col) =>
				col.key === columnKey ? { ...col, visible: !col.visible } : col
			)
		);
	};

	const confirmDeleteUser = async () => {
		if (!userToDelete) return;
		// request
		setShowDeleteDialog(false);
		setUserToDelete(null);
	};

	const getUserActions = (user: UserDto): ActionItem[] => [
		{
			id: "view",
			icon: "👁️",
			label: t("viewProfile", "View Profile"),
			onClick: () => handleProfileUser(user),
		},
		{
			id: "block",
			icon: user.isBlocked ? "✅" : "🚫",
			label: user.isBlocked
				? t("unblockUser", "Unblock User")
				: t("blockUser", "Block User"),
			onClick: () => handleBlockUser(user),
			variant: user.isBlocked ? "default" : "danger",
		},
		{
			id: "admin",
			icon: user.role === 2 ? "⭐" : "👑",
			label:
				user.role === 2
					? t("removeAdmin", "Remove Admin")
					: t("makeAdmin", "Make Admin"),
			onClick: () => handleToggleAdmin(user),
		},
	];

	const getRoleLabel = (role: number) => {
		switch (role) {
			case 1:
				return t("user", "User");
			case 2:
				return t("admin", "Admin");
			case 4:
				return t("superAdmin", "Super Admin");
			default:
				return t("unknown", "Unknown");
		}
	};

	const getStatusLabel = (isBlocked: boolean) => {
		return isBlocked ? t("blocked", "Blocked") : t("active", "Active");
	};

	const tableColumns: TableColumn<UserDto>[] = columns
		.filter((col) => col.visible)
		.map((col) => ({
			key: col.key,
			label: col.label,
			sortable: col.sortable,
			render: (user: UserDto) => {
				switch (col.key) {
					case "userName":
						return (
							<div className="flex items-center gap-2">
								<div className="w-8 h-8 bg-primary rounded-full flex items-center justify-center text-white text-sm font-medium">
									{user.userName.charAt(0).toUpperCase()}
								</div>
								<span className="font-medium">{user.userName}</span>
							</div>
						);
					case "email":
						return user.contacts?.[0]?.value || t("noEmail", "No email");
					case "role":
						return (
							<span
								className={`px-2 py-1 rounded-full text-xs font-medium ${
									user.role === 2
										? "bg-primary/10 text-primary"
										: "bg-background text-textMuted"
								}`}
							>
								{getRoleLabel(user.role)}
							</span>
						);
					case "isBlocked":
						return (
							<span
								className={`px-2 py-1 rounded-full text-xs font-medium ${
									user.isBlocked
										? "bg-error/10 text-error"
										: "bg-success/10 text-success"
								}`}
							>
								{getStatusLabel(user.isBlocked)}
							</span>
						);
					case "createdAt":
						return new Date(user.createdAt).toLocaleDateString();
					case "updatedAt":
						return new Date(user.updatedAt).toLocaleDateString();
					case "templatesCount":
						return user.templatesCount;
					case "formsCount":
						return user.formsCount;
					default:
						return "";
				}
			},
		}));

	if (loading) {
		return (
			<div className="flex items-center justify-center py-8">
				<div className="text-textMuted">
					{t("loadingUsers", "Loading users...")}
				</div>
			</div>
		);
	}

	return (
		<>
			<div className="space-y-4">
				<div className="flex items-center justify-between">
					<div className="flex items-center gap-3">
						<span className="text-sm text-textMuted">
							{t("totalUsers", "Total users")}: {totalCount}
						</span>
						{selectedUsers.size > 0 && (
							<>
								<button
									onClick={handleBlockSelected}
									className="px-4 py-2 bg-warning text-white rounded-lg hover:opacity-90 transition-opacity text-sm"
								>
									{t("blockSelected", "Block Selected")} ({selectedUsers.size})
								</button>
								<button
									onClick={handleUnblockSelected}
									className="px-4 py-2 bg-success text-white rounded-lg hover:opacity-90 transition-opacity text-sm"
								>
									{t("unblockSelected", "Unblock Selected")} (
									{selectedUsers.size})
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
					data={sortedUsers}
					columns={tableColumns}
					onSort={handleSort}
					selectable={true}
					selectedItems={selectedUsers}
					onSelectionChange={setSelectedUsers}
					getItemId={(user) => user.id}
					emptyMessage={t("noUsersFound", "No users found") || "No users found"}
					onRowHover={setHoveredRowId}
					onReachEnd={hasMore ? loadMoreUsers : undefined}
					isLoadingMore={loadingMore}
					renderRow={(user) => (
						<td className="absolute inset-0 pointer-events-none">
							<ActionPanel
								visible={hoveredRowId === user.id}
								actions={getUserActions(user)}
								position="right"
								className="pointer-events-auto"
							/>
						</td>
					)}
				/>
			</div>

			<Dialog.Root open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
				<Dialog.Portal>
					<Dialog.Overlay className="fixed inset-0 bg-black/50 z-50" />
					<Dialog.Content className="fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-surface border border-border rounded-lg p-6 max-w-md w-full mx-4 z-50">
						<Dialog.Title className="text-lg font-semibold text-text mb-2">
							{t("confirmDeleteUser", "Confirm Delete User") ||
								"Confirm Delete User"}
						</Dialog.Title>
						<Dialog.Description className="text-textMuted mb-6">
							{t(
								"confirmDeleteUserDescription",
								"Are you sure you want to delete this user? This action cannot be undone."
							) ||
								"Are you sure you want to delete this user? This action cannot be undone."}
						</Dialog.Description>
						<div className="flex justify-end gap-3">
							<button
								onClick={() => setShowDeleteDialog(false)}
								className="px-4 py-2 text-textMuted hover:text-text transition-colors"
							>
								{t("cancel", "Cancel") || "Cancel"}
							</button>
							<button
								onClick={confirmDeleteUser}
								className="px-4 py-2 bg-error text-white rounded-lg hover:opacity-90 transition-opacity"
							>
								{t("deleteUser", "Delete User") || "Delete User"}
							</button>
						</div>
					</Dialog.Content>
				</Dialog.Portal>
			</Dialog.Root>
		</>
	);
};
