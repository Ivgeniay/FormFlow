import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import { TemplateDto } from "../../../../shared/api_types";
import {
	formSubscribeApi,
	UserSearchDto,
} from "../../../../api/formSubscribeApi";
import { useAuth } from "../../../auth/hooks/useAuth";
import {
	SortableTable,
	SortConfig,
	TableColumn,
} from "../../../../ui/SortableTable";

export interface TabProps {
	template: TemplateDto;
	accessToken: string | null;
}

export const SubscriptionsTab: React.FC<TabProps> = ({
	template,
	accessToken,
}) => {
	const { t } = useTranslation();
	const { user, isAdmin } = useAuth();
	const [isSubscribed, setIsSubscribed] = useState(false);
	const [subscribers, setSubscribers] = useState<UserSearchDto[]>([]);
	const [sortedSubscribers, setSortedSubscribers] = useState<UserSearchDto[]>(
		[]
	);
	const [loading, setLoading] = useState(true);
	const [subscribersLoading, setSubscribersLoading] = useState(true);
	const [toggleLoading, setToggleLoading] = useState(false);

	const isTemplateAuthor = user?.id === template.authorId;
	const canManageSubscriptions = isTemplateAuthor || isAdmin;

	const subscribersColumns: TableColumn<UserSearchDto>[] = [
		{
			key: "userName",
			label: t("subscribers") || "Subscribers",
			sortable: true,
			render: (subscriber) => (
				<div className="flex items-center gap-3">
					<div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center">
						<span className="text-sm font-medium text-primary">
							{subscriber.userName.charAt(0).toUpperCase()}
						</span>
					</div>
					<div>
						<span className="font-medium text-text">{subscriber.userName}</span>
						{subscriber.primaryEmail && (
							<div className="text-sm text-textMuted">
								{subscriber.primaryEmail}
							</div>
						)}
					</div>
				</div>
			),
		},
	];

	useEffect(() => {
		if (accessToken) {
			loadSubscriptionStatus();
			if (canManageSubscriptions) {
				loadSubscribers();
			}
		}
	}, [accessToken, template.id, canManageSubscriptions]);

	useEffect(() => {
		setSortedSubscribers([...subscribers]);
	}, [subscribers]);

	const loadSubscriptionStatus = async () => {
		if (!accessToken) return;

		try {
			setLoading(true);
			const status = await formSubscribeApi.getSubscriptionStatus(
				template.id,
				accessToken
			);
			setIsSubscribed(status.subscribed);
		} catch (error) {
			console.error("Error loading subscription status:", error);
		} finally {
			setLoading(false);
		}
	};

	const loadSubscribers = async () => {
		if (!accessToken || !canManageSubscriptions) return;

		try {
			setSubscribersLoading(true);
			const subscribersList = await formSubscribeApi.getTemplateSubscribers(
				template.id,
				accessToken
			);
			setSubscribers(subscribersList);
		} catch (error) {
			console.error("Error loading subscribers:", error);
		} finally {
			setSubscribersLoading(false);
		}
	};

	const handleToggleSubscription = async () => {
		if (!accessToken) return;

		try {
			setToggleLoading(true);
			const result = await formSubscribeApi.toggleSubscription(
				template.id,
				accessToken
			);

			setIsSubscribed(result.subscribed);

			if (result.action === "subscribed") {
				toast.success(
					t("subscribedSuccessfully") ||
						"Successfully subscribed to notifications"
				);
			} else {
				toast.success(
					t("unsubscribedSuccessfully") ||
						"Successfully unsubscribed from notifications"
				);
			}

			if (canManageSubscriptions) {
				loadSubscribers();
			}
		} catch (error: any) {
			toast.error(
				error.message || t("subscriptionError") || "Error managing subscription"
			);
		} finally {
			setToggleLoading(false);
		}
	};

	const handleSort = (sortConfig: SortConfig) => {
		const sorted = [...subscribers].sort((a, b) => {
			let aValue: any;
			let bValue: any;

			switch (sortConfig.key) {
				case "userName":
					aValue = a.userName.toLowerCase();
					bValue = b.userName.toLowerCase();
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

		setSortedSubscribers(sorted);
	};

	return (
		<div className="space-y-6">
			<div className="bg-surface border border-border rounded-lg p-6">
				<h3 className="text-lg font-semibold text-text mb-4">
					{t("notificationSettings") || "Notification Settings"}
				</h3>

				<div className="flex items-center gap-3">
					<label className="flex items-center gap-2 cursor-pointer">
						<input
							type="checkbox"
							checked={isSubscribed}
							onChange={handleToggleSubscription}
							disabled={loading || toggleLoading || !accessToken}
							className="text-primary focus:ring-primary"
						/>
						<span className="text-text">
							{t("subscribeToNotifications") ||
								"Subscribe to form submission notifications"}
						</span>
					</label>

					{toggleLoading && (
						<div className="animate-spin rounded-full h-4 w-4 border-b-2 border-primary"></div>
					)}
				</div>

				<p className="text-sm text-textMuted mt-2">
					{t("subscriptionDescription") ||
						"You will receive email notifications when someone fills out this form"}
				</p>
			</div>

			{canManageSubscriptions && (
				<div className="bg-surface border border-border rounded-lg p-6">
					<div className="flex items-center justify-between mb-4">
						<h3 className="text-lg font-semibold text-text">
							{t("subscribers") || "Subscribers"}
						</h3>
						<span className="px-3 py-1 rounded-full text-sm font-medium bg-primary/10 text-primary">
							{subscribers.length} {t("total") || "total"}
						</span>
					</div>

					{subscribersLoading ? (
						<div className="flex items-center justify-center py-8">
							<div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
						</div>
					) : subscribers.length > 0 ? (
						<SortableTable
							data={sortedSubscribers}
							columns={subscribersColumns}
							onSort={handleSort}
							// loading={false}
							emptyMessage={t("noSubscribers") || "No subscribers yet"}
							// itemKey={(subscriber) => subscriber.id}
						/>
					) : (
						<div className="text-center py-8">
							<p className="text-textMuted">
								{t("noSubscribers") || "No subscribers yet"}
							</p>
							<p className="text-sm text-textMuted mt-1">
								{t("noSubscribersDescription") ||
									"Users who subscribe will appear here"}
							</p>
						</div>
					)}
				</div>
			)}

			{!canManageSubscriptions && (
				<div className="bg-surface border border-border rounded-lg p-6">
					<p className="text-sm text-textMuted text-center">
						{t("subscribersVisibleToAuthor") ||
							"Subscribers list is only visible to template author and administrators"}
					</p>
				</div>
			)}
		</div>
	);
};
