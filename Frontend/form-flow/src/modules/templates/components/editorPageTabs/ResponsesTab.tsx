import React, { useState, useEffect, useCallback } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { formApi } from "../../../../api/formApi";
import {
	SortableTable,
	SortConfig,
	TableColumn,
} from "../../../../ui/SortableTable";
import { ActionItem, ActionPanel } from "./ActionPanel";
import toast from "react-hot-toast";
import { FormDto, TemplateDto } from "../../../../shared/api_types";
import { FormattedTextDisplay } from "../../../../ui/Input/FormattedTextDisplay";

export interface TabProps {
	template: TemplateDto;
	accessToken: string | null;
}

export const ResponsesTab: React.FC<TabProps> = ({ template, accessToken }) => {
	const { t } = useTranslation();
	const navigate = useNavigate();

	const [forms, setForms] = useState<FormDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [loadingMore, setLoadingMore] = useState(false);
	const [hasMore, setHasMore] = useState(true);
	const [page, setPage] = useState(1);
	const [sortedForms, setSortedForms] = useState<FormDto[]>([]);
	const [hoveredRowId, setHoveredRowId] = useState<string | null>(null);

	const pageSize = 3;

	useEffect(() => {
		loadInitialForms();
	}, [template.id, accessToken]);

	useEffect(() => {
		setSortedForms([...forms]);
	}, [forms]);

	useEffect(() => {
		const handleScroll = () => {
			if (
				window.innerHeight + document.documentElement.scrollTop >=
				document.documentElement.offsetHeight - 100
			) {
				if (!loadingMore && hasMore) {
					loadMoreForms();
				}
			}
		};

		window.addEventListener("scroll", handleScroll);
		return () => window.removeEventListener("scroll", handleScroll);
	}, [loadingMore, hasMore]);

	const loadInitialForms = async () => {
		if (!accessToken) return;

		try {
			setLoading(true);
			const response = await formApi.getFormsByTemplate(
				template.id,
				1,
				pageSize,
				accessToken
			);
			setForms(response.data);
			setPage(1);
			setHasMore(response.data.length === pageSize);
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorLoadingResponses", "Error loading responses") ||
					"Error loading responses"
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
			const response = await formApi.getFormsByTemplate(
				template.id,
				nextPage,
				pageSize,
				accessToken
			);

			if (response.data.length > 0) {
				setForms((prev) => [...prev, ...response.data]);
				setPage(nextPage);
				setHasMore(response.data.length === pageSize);
			} else {
				setHasMore(false);
			}
		} catch (error: any) {
			toast.error(
				error.message ||
					t("errorLoadingMoreResponses", "Error loading more responses") ||
					"Error loading more responses"
			);
		} finally {
			setLoadingMore(false);
		}
	}, [template.id, accessToken, page, loadingMore]);

	const handleSort = (sortConfig: SortConfig) => {
		const sorted = [...forms].sort((a, b) => {
			let aValue: any;
			let bValue: any;

			switch (sortConfig.key) {
				case "userName":
					aValue = a.userName;
					bValue = b.userName;
					break;
				case "submittedAt":
					aValue = new Date(a.submittedAt);
					bValue = new Date(b.submittedAt);
					break;
				default:
					if (sortConfig.key.length > 0) {
						const questionId = sortConfig.key;
						const aAnswer = getAnswerForQuestion(a, questionId);
						const bAnswer = getAnswerForQuestion(b, questionId);

						const aStr = aAnswer?.toString() || "";
						const bStr = bAnswer?.toString() || "";

						const aDate = new Date(aStr);
						const bDate = new Date(bStr);

						const aIsValidDate =
							!isNaN(aDate.getTime()) &&
							aStr.match(
								/\d{4}-\d{2}-\d{2}|\d{2}[.\/]\d{2}[.\/]\d{4}|\d{2}[.\/]\d{2}[.\/]\d{2}/
							);
						const bIsValidDate =
							!isNaN(bDate.getTime()) &&
							bStr.match(
								/\d{4}-\d{2}-\d{2}|\d{2}[.\/]\d{2}[.\/]\d{4}|\d{2}[.\/]\d{2}[.\/]\d{2}/
							);

						if (aIsValidDate && bIsValidDate) {
							aValue = aDate;
							bValue = bDate;
						} else {
							const aNum = parseFloat(aStr);
							const bNum = parseFloat(bStr);

							if (!isNaN(aNum) && !isNaN(bNum)) {
								aValue = aNum;
								bValue = bNum;
							} else {
								aValue = aStr.toLowerCase();
								bValue = bStr.toLowerCase();
							}
						}
					} else {
						return 0;
					}
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

	const getFormActions = (form: FormDto): ActionItem[] => [
		{
			id: "view",
			icon: "👁️",
			label: t("viewResponse", "View Response") || "View Response",
			onClick: () => handleViewForm(form),
		},
	];

	const getVisibleQuestions = () => {
		return template.questions.filter((q) => q.showInResults);
	};

	const getAnswerForQuestion = (form: FormDto, questionId: string) => {
		const question = form.questions.find((q) => q.questionId === questionId);
		return question?.answer || t("noAnswer", "No answer");
	};

	const buildTableColumns = (): TableColumn<FormDto>[] => {
		const baseColumns: TableColumn<FormDto>[] = [
			{
				key: "userName",
				label: t("user", "User") || "User",
				sortable: true,
				render: (form: FormDto) => (
					<div className="flex items-center gap-2">
						<span>{form.userName}</span>
					</div>
				),
			},
			{
				key: "submittedAt",
				label: t("submittedAt", "Submitted") || "Submitted",
				sortable: true,
				render: (form: FormDto) => (
					<span>{new Date(form.submittedAt).toLocaleDateString()}</span>
				),
			},
		];

		const questionColumns: TableColumn<FormDto>[] = getVisibleQuestions().map(
			(question) => ({
				key: `${question.id}`,
				label: question.data
					? JSON.parse(question.data).title || "Question"
					: "Question",
				sortable: true,
				render: (form: FormDto) => {
					const answer = getAnswerForQuestion(form, question.id);
					return (
						<div className="max-w-48 truncate" title={answer?.toString()}>
							<FormattedTextDisplay
								value={answer?.toString() || t("noAnswer", "No answer")}
							/>
						</div>
					);
				},
			})
		);

		return [...baseColumns, ...questionColumns];
	};

	if (loading) {
		return (
			<div className="flex items-center justify-center py-8">
				<div className="text-textMuted">
					{t("loadingResponses", "Loading responses...") ||
						"Loading responses..."}
				</div>
			</div>
		);
	}

	if (forms.length === 0) {
		return (
			<div className="text-center py-8">
				<div className="text-textMuted">
					<svg
						className="w-12 h-12 mx-auto mb-4"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
						/>
					</svg>
					<p className="text-lg font-medium text-text mb-2">
						{t("noResponsesYet", "No responses yet") || "No responses yet"}
					</p>
					<p className="text-sm">
						{t(
							"shareTemplateToGetResponses",
							"Share your template to start collecting responses"
						) || "Share your template to start collecting responses"}
					</p>
				</div>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<div className="text-sm text-textMuted">
					{t("totalResponses", "Total responses")}: {forms.length}
					{hasMore && " +"}
				</div>
			</div>

			<SortableTable
				data={sortedForms}
				columns={buildTableColumns()}
				onSort={handleSort}
				emptyMessage={
					t("noResponsesYet", "No responses yet") || "No responses yet"
				}
				onRowHover={setHoveredRowId}
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
				getItemId={(form) => form.id}
			/>

			{loadingMore && (
				<div className="text-center py-4">
					<div className="text-textMuted">
						{t("loadingMoreResponses", "Loading more responses...") ||
							"Loading more responses..."}
					</div>
				</div>
			)}
		</div>
	);
};
