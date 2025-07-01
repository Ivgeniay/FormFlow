import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import { usersApi } from "../../api/usersApi";
import { tagsApi, TagStatsResponse } from "../../api/tagsApi";
import { searchApi } from "../../api/searchApi";

interface SystemToolsTabProps {
	accessToken: string;
}

interface UserStats {
	totalUsers: number;
	activeUsers: number;
	blockedUsers: number;
	adminUsers: number;
	regularUsers: number;
	newUsersThisWeek: number;
	newUsersThisMonth: number;
	usersByRole: Record<string, number>;
	generatedAt: string;
}

export const SystemToolsTab: React.FC<SystemToolsTabProps> = ({
	accessToken,
}) => {
	const { t } = useTranslation();
	const [userStats, setUserStats] = useState<UserStats | null>(null);
	const [tagStats, setTagStats] = useState<TagStatsResponse | null>(null);
	const [loadingUserStats, setLoadingUserStats] = useState(false);
	const [loadingTagStats, setLoadingTagStats] = useState(false);
	const [reindexing, setReindexing] = useState(false);
	const [cleaningTags, setCleaningTags] = useState(false);

	useEffect(() => {
		loadUserStats();
		loadTagStats();
	}, [accessToken]);

	const loadUserStats = async () => {
		try {
			setLoadingUserStats(true);
			const stats = await usersApi.getUserStats(accessToken);
			setUserStats(stats);
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorLoadingUserStats", "Error loading user statistics")
			);
		} finally {
			setLoadingUserStats(false);
		}
	};

	const loadTagStats = async () => {
		try {
			setLoadingTagStats(true);
			const stats = await tagsApi.getTagStats(accessToken);
			setTagStats(stats);
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorLoadingTagStats", "Error loading tag statistics")
			);
		} finally {
			setLoadingTagStats(false);
		}
	};

	const handleReindexElasticsearch = async () => {
		try {
			setReindexing(true);
			await searchApi.reindexTemplates(accessToken);
			toast.success(
				t("reindexStarted", "Template reindexing started successfully")
			);
		} catch (error: any) {
			toast.error(
				error.message || t("errorReindexing", "Error starting reindexing")
			);
		} finally {
			setReindexing(false);
		}
	};

	const handleCleanupUnusedTags = async () => {
		try {
			setCleaningTags(true);
			await tagsApi.cleanupUnusedTags(accessToken);
			toast.success(t("tagsCleanedUp", "Unused tags cleaned up successfully"));
			await loadTagStats();
		} catch (error: any) {
			toast.error(
				error.message || t("errorCleaningTags", "Error cleaning up tags")
			);
		} finally {
			setCleaningTags(false);
		}
	};

	const StatCard: React.FC<{
		title: string;
		value: string | number;
		subtitle?: string;
		variant?: "default" | "success" | "warning" | "error";
	}> = ({ title, value, subtitle, variant = "default" }) => {
		const variantClasses = {
			default: "bg-surface border-border",
			success: "bg-success/10 border-success/30",
			warning: "bg-warning/10 border-warning/30",
			error: "bg-error/10 border-error/30",
		};

		return (
			<div className={`border rounded-lg p-4 ${variantClasses[variant]}`}>
				<div className="text-2xl font-bold text-text">{value}</div>
				<div className="text-sm font-medium text-text mb-1">{title}</div>
				{subtitle && <div className="text-xs text-textMuted">{subtitle}</div>}
			</div>
		);
	};

	const ActionButton: React.FC<{
		onClick: () => void;
		loading: boolean;
		variant?: "default" | "danger" | "warning";
		children: React.ReactNode;
	}> = ({ onClick, loading, variant = "default", children }) => {
		const variantClasses = {
			default: "bg-primary hover:bg-primary/90 text-white",
			danger: "bg-error hover:bg-error/90 text-white",
			warning: "bg-warning hover:bg-warning/90 text-white",
		};

		return (
			<button
				onClick={onClick}
				disabled={loading}
				className={`px-4 py-2 rounded-lg font-medium transition-colors disabled:opacity-50 ${variantClasses[variant]}`}
			>
				{loading ? t("processing", "Processing...") : children}
			</button>
		);
	};

	return (
		<div className="space-y-6">
			<div className="bg-surface border border-border rounded-lg p-6">
				<h2 className="text-xl font-semibold text-text mb-4">
					{t("systemTools", "System Tools")}
				</h2>
				<p className="text-textMuted">
					{t(
						"systemToolsDescription",
						"Administrative tools for system maintenance and monitoring"
					)}
				</p>
			</div>

			<div className="bg-surface border border-border rounded-lg p-6">
				<div className="flex items-center justify-between mb-4">
					<h3 className="text-lg font-medium text-text">
						{t("userStatistics", "User Statistics")}
					</h3>
					<button
						onClick={loadUserStats}
						disabled={loadingUserStats}
						className="px-3 py-1 text-text text-sm bg-background border border-border rounded hover:bg-surface transition-colors"
					>
						{loadingUserStats
							? t("loading", "Loading...")
							: t("refresh", "Refresh")}
					</button>
				</div>

				{userStats ? (
					<div className="grid grid-cols-2 md:grid-cols-4 gap-4">
						<StatCard
							title={t("totalUsers", "Total Users")}
							value={userStats.totalUsers}
						/>
						<StatCard
							title={t("activeUsers", "Active Users")}
							value={userStats.activeUsers}
							variant="success"
						/>
						<StatCard
							title={t("blockedUsers", "Blocked Users")}
							value={userStats.blockedUsers}
							variant="error"
						/>
						<StatCard
							title={t("adminUsers", "Admin Users")}
							value={userStats.adminUsers}
							variant="warning"
						/>
						<StatCard
							title={t("newUsersThisWeek", "New This Week")}
							value={userStats.newUsersThisWeek}
						/>
						<StatCard
							title={t("newUsersThisMonth", "New This Month")}
							value={userStats.newUsersThisMonth}
						/>
					</div>
				) : (
					<div className="text-center py-8 text-textMuted">
						{loadingUserStats
							? t("loadingStats", "Loading statistics...")
							: t("noStatsAvailable", "No statistics available")}
					</div>
				)}
			</div>

			<div className="bg-surface border border-border rounded-lg p-6">
				<div className="flex items-center justify-between mb-4">
					<h3 className="text-lg font-medium text-text">
						{t("tagManagement", "Tag Management")}
					</h3>
					<button
						onClick={loadTagStats}
						disabled={loadingTagStats}
						className="px-3 py-1 text-text text-sm bg-background border border-border rounded hover:bg-surface transition-colors"
					>
						{loadingTagStats
							? t("loading", "Loading...")
							: t("refresh", "Refresh")}
					</button>
				</div>

				{tagStats ? (
					<div className="space-y-4">
						<div className="grid grid-cols-2 md:grid-cols-4 gap-4">
							<StatCard
								title={t("totalTags", "Total Tags")}
								value={tagStats.totalTags}
							/>
							<StatCard
								title={t("unusedTags", "Unused Tags")}
								value={tagStats.unusedTags}
								variant={tagStats.unusedTags > 0 ? "warning" : "success"}
							/>
							<StatCard
								title={t("averageUsage", "Average Usage")}
								value={tagStats.averageUsage.toFixed(1)}
							/>
						</div>

						<div className="flex gap-4">
							<ActionButton
								onClick={handleCleanupUnusedTags}
								loading={cleaningTags}
								variant="warning"
							>
								{t("cleanupUnusedTags", "Cleanup Unused Tags")}
							</ActionButton>
						</div>

						{tagStats.topTags.length > 0 && (
							<div>
								<h4 className="text-md font-medium text-text mb-2">
									{t("mostUsedTags", "Most Used Tags")}
								</h4>
								<div className="grid grid-cols-2 md:grid-cols-3 gap-2">
									{tagStats.topTags.slice(0, 6).map((tag, index) => (
										<div
											key={index}
											className="bg-background border border-border rounded px-3 py-2 text-sm text-text"
										>
											<span className="font-medium">{tag.name}</span>
											<span className="text-textMuted ml-2">
												({tag.usageCount})
											</span>
										</div>
									))}
								</div>
							</div>
						)}
					</div>
				) : (
					<div className="text-center py-8 text-textMuted">
						{loadingTagStats
							? t("loadingStats", "Loading statistics...")
							: t("noStatsAvailable", "No statistics available")}
					</div>
				)}
			</div>

			<div className="bg-surface border border-border rounded-lg p-6">
				<h3 className="text-lg font-medium text-text mb-4">
					{t("searchIndexing", "Search & Indexing")}
				</h3>
				<p className="text-textMuted mb-4">
					{t(
						"searchIndexingDescription",
						"Manage Elasticsearch search index for templates"
					)}
				</p>

				<ActionButton
					onClick={handleReindexElasticsearch}
					loading={reindexing}
					variant="warning"
				>
					{t("reindexTemplates", "Reindex All Templates")}
				</ActionButton>

				{reindexing && (
					<div className="mt-4 p-4 bg-warning/10 border border-warning/30 rounded-lg">
						<p className="text-sm text-warning">
							{t(
								"reindexingInProgress",
								"Reindexing is in progress. This may take several minutes to complete."
							)}
						</p>
					</div>
				)}
			</div>

			{/* <div className="bg-surface border border-border rounded-lg p-6">
				<h3 className="text-lg font-medium text-text mb-4">
					{t("systemMaintenance", "System Maintenance")}
				</h3>
				<p className="text-textMuted mb-4">
					{t(
						"systemMaintenanceDescription",
						"Additional system maintenance tools and utilities"
					)}
				</p>

				<div className="space-y-2 text-sm text-textMuted">
					<p>
						•{" "}
						{t(
							"maintenanceFeature1",
							"Database optimization tools (coming soon)"
						)}
					</p>
					<p>
						• {t("maintenanceFeature2", "Image storage cleanup (coming soon)")}
					</p>
					<p>
						•{" "}
						{t("maintenanceFeature3", "System health monitoring (coming soon)")}
					</p>
				</div>
			</div> */}
		</div>
	);
};
