import axios from "axios";
import { ENV } from "../config/env";

const API_BASE_URL = ENV.API_URL;

export interface BasicStatsDto {
	totalSubmissions: number;
	firstSubmission?: string;
	lastSubmission?: string;
}

export interface TimeAnalyticsDto {
	submissionsByDay: DaySubmissionDto[];
	submissionsByHour: HourSubmissionDto[];
	submissionsByMonth: MonthSubmissionDto[];
}

export interface DaySubmissionDto {
	date: string;
	count: number;
}

export interface HourSubmissionDto {
	hour: number;
	count: number;
}

export interface MonthSubmissionDto {
	month: string;
	count: number;
}

export interface TimeAnalyticsQuery {
	days?: BetweenDays;
	hours?: BetweenHours;
	months?: BetweenMonths;
}

export interface BetweenDays {
	startDaysAgo: number;
	endDaysAgo: number;
}

export interface BetweenHours {
	startHour: number;
	endHour: number;
}

export interface BetweenMonths {
	startMonthsAgo: number;
	endMonthsAgo: number;
}

class AnalyticsApi {
	async getBasicStats(
		templateId: string,
		accessToken: string
	): Promise<BasicStatsDto> {
		const response = await axios.get<BasicStatsDto>(
			`${API_BASE_URL}/Analytics/template/${templateId}/basic`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getTimeAnalytics(
		templateId: string,
		query: TimeAnalyticsQuery,
		accessToken: string
	): Promise<TimeAnalyticsDto> {
		const response = await axios.post<TimeAnalyticsDto>(
			`${API_BASE_URL}/Analytics/template/${templateId}/time`,
			query,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}
}

export const analyticsApi = new AnalyticsApi();
