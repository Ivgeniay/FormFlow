import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { TemplateDto } from "../../../../shared/api_types";
import {
	analyticsApi,
	BasicStatsDto,
	TimeAnalyticsDto,
	TimeAnalyticsQuery,
} from "../../../../api/analyticsApi";
import toast from "react-hot-toast";
import { RangeInput } from "../../../../ui/Input/RangeInput";
import { ChartContainer } from "../../../../ui/ChartContainer";
import { StatCard } from "../../../../ui/StatCard";

export interface TabProps {
	template: TemplateDto;
	accessToken: string | null;
}

export const AnalyticsTab: React.FC<TabProps> = ({ template, accessToken }) => {
	const { t } = useTranslation();

	const [basicStats, setBasicStats] = useState<BasicStatsDto | null>(null);
	const [timeAnalytics, setTimeAnalytics] = useState<TimeAnalyticsDto | null>(
		null
	);
	const [loading, setLoading] = useState(true);
	const [timeLoading, setTimeLoading] = useState(false);

	const [daysRange, setDaysRange] = useState({ start: 7, end: 0 });
	const [hoursRange, setHoursRange] = useState({ start: 0, end: 23 });
	const [monthsRange, setMonthsRange] = useState({ start: 1, end: 0 });

	useEffect(() => {
		if (accessToken) {
			loadBasicStats();
			loadTimeAnalytics();
		}
	}, [accessToken, template.id]);

	const loadBasicStats = async () => {
		if (!accessToken) return;

		try {
			const stats = await analyticsApi.getBasicStats(template.id, accessToken);
			setBasicStats(stats);
		} catch (error: any) {
			console.error("Error loading basic stats:", error);
			toast.error("Error loading analytics");
		}
	};

	const loadTimeAnalytics = async () => {
		if (!accessToken) return;

		try {
			setTimeLoading(true);
			const query: TimeAnalyticsQuery = {
				days: { startDaysAgo: daysRange.start, endDaysAgo: daysRange.end },
				hours: { startHour: hoursRange.start, endHour: hoursRange.end },
				months: {
					startMonthsAgo: monthsRange.start,
					endMonthsAgo: monthsRange.end,
				},
			};

			const analytics = await analyticsApi.getTimeAnalytics(
				template.id,
				query,
				accessToken
			);
			setTimeAnalytics(analytics);
		} catch (error: any) {
			console.error("Error loading time analytics:", error);
			toast.error("Error loading time analytics");
		} finally {
			setTimeLoading(false);
			setLoading(false);
		}
	};

	const handlePeriodChange = () => {
		loadTimeAnalytics();
	};

	if (loading) {
		return (
			<div className="flex items-center justify-center py-8">
				<div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
			</div>
		);
	}

	return (
		<div className="space-y-6">
			<div className="grid grid-cols-1 md:grid-cols-3 gap-4">
				<StatCard
					title={t("totalSubmissions", "Total Submissions")}
					value={basicStats?.totalSubmissions || 0}
				/>
				<StatCard
					title={t("firstSubmission")}
					value={
						basicStats?.firstSubmission
							? new Date(basicStats.firstSubmission).toLocaleDateString()
							: t("noSubmissions") || "No Submissions"
					}
				/>
				<StatCard
					title={t("lastSubmission")}
					value={
						basicStats?.lastSubmission
							? new Date(basicStats.lastSubmission).toLocaleDateString()
							: t("noSubmissions") || "No Submissions"
					}
				/>
			</div>

			<div className="bg-surface border border-border rounded-lg p-4">
				<h3 className="text-lg font-semibold text-text mb-4">
					{t("periodSettings")}
				</h3>

				<div className="grid grid-cols-1 md:grid-cols-3 gap-4">
					<RangeInput
						label={t("daysRange")}
						startValue={daysRange.start}
						onStartChange={(value) =>
							setDaysRange((prev) => ({ ...prev, start: value }))
						}
						min={1}
						max={365}
						suffix={t("daysAgo", "days ago") || "days ago"}
					/>

					<RangeInput
						label={t("hoursRange")}
						startValue={hoursRange.start}
						endValue={hoursRange.end}
						onStartChange={(value) =>
							setHoursRange((prev) => ({ ...prev, start: value }))
						}
						onEndChange={(value) =>
							setHoursRange((prev) => ({ ...prev, end: value }))
						}
						min={0}
						max={23}
						showEndInput={true}
						betweenText={t("to", "to") || "to"}
					/>

					<RangeInput
						label={t("monthsRange")}
						startValue={monthsRange.start}
						onStartChange={(value) =>
							setMonthsRange((prev) => ({ ...prev, start: value }))
						}
						min={1}
						max={24}
						suffix={t("monthsAgo", "months ago") || "months ago"}
					/>
				</div>

				<button
					onClick={handlePeriodChange}
					disabled={timeLoading}
					className="mt-4 px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50"
				>
					{timeLoading ? t("loading") : t("updateCharts")}
				</button>
			</div>

			{timeAnalytics && (
				<div className="space-y-6">
					{timeAnalytics.submissionsByDay.length > 0 && (
						<ChartContainer
							title={t("submissionsByDay")}
							chartId="dayChart"
							data={timeAnalytics.submissionsByDay.map((item) => ({
								date: new Date(item.date).toLocaleDateString(),
								count: item.count,
							}))}
							chartType="line"
						/>
					)}

					{timeAnalytics.submissionsByHour.length > 0 && (
						<ChartContainer
							title={t("submissionsByHour")}
							chartId="hourChart"
							data={timeAnalytics.submissionsByHour.map((item) => ({
								hour: item.hour,
								count: item.count,
							}))}
							chartType="column"
						/>
					)}

					{timeAnalytics.submissionsByMonth.length > 0 && (
						<ChartContainer
							title={t("submissionsByMonth")}
							chartId="monthChart"
							data={timeAnalytics.submissionsByMonth.map((item) => ({
								month: new Date(item.month).toLocaleDateString("en-US", {
									year: "numeric",
									month: "short",
								}),
								count: item.count,
							}))}
							chartType="line"
						/>
					)}
				</div>
			)}

			{!timeLoading &&
				timeAnalytics &&
				timeAnalytics.submissionsByDay.length === 0 &&
				timeAnalytics.submissionsByHour.length === 0 &&
				timeAnalytics.submissionsByMonth.length === 0 && (
					<div className="text-center py-8">
						<p className="text-textMuted">{t("noAnalyticsData")}</p>
					</div>
				)}
		</div>
	);
};
