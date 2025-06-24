import React from "react";
import { useTranslation } from "react-i18next";
import { mode } from "../../../../pages/templates/TemplateEditor";

interface QuestionToolbarProps {
	onAddQuestion: () => void;
	onDuplicateQuestion: () => void;
	onDeleteQuestion: () => void;
	isVisible: boolean;
	mode: mode;
	binDisable?: boolean;
}

export const QuestionToolbar = React.forwardRef<
	HTMLDivElement,
	QuestionToolbarProps
>(
	(
		{
			onAddQuestion,
			onDuplicateQuestion,
			onDeleteQuestion,
			isVisible,
			mode,
			binDisable = false,
		},
		ref
	) => {
		const { t } = useTranslation();

		const isOnlyView = mode === "readonly" || mode === "edit";
		// const isReadOnly = mode === "readonly";
		if (!isVisible) return null;

		return (
			<div
				ref={ref}
				className="flex flex-col bg-surface border border-border shadow-lg p-0 gap-1 z-30"
			>
				<button
					onClick={onAddQuestion}
					disabled={isOnlyView}
					className={`p-3 rounded-lg transition-colors group ${
						isOnlyView
							? "text-textMuted cursor-not-allowed opacity-50"
							: "text-text hover:bg-background"
					}`}
					title={t("addQuestion") || "Add Question"}
				>
					<svg
						className="w-5 h-5"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M12 4v16m8-8H4"
						/>
					</svg>
				</button>

				<button
					disabled={isOnlyView}
					onClick={onDuplicateQuestion}
					className={`p-3 rounded-lg transition-colors group ${
						isOnlyView
							? "text-textMuted cursor-not-allowed opacity-50"
							: "text-text hover:bg-background"
					}`}
					title={t("duplicateQuestion") || "Duplicate Question"}
				>
					<svg
						className="w-5 h-5"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"
						/>
					</svg>
				</button>

				<button
					disabled={binDisable || isOnlyView}
					onClick={onDeleteQuestion}
					className={`p-3 rounded-lg transition-colors group ${
						binDisable || isOnlyView
							? "text-textMuted cursor-not-allowed opacity-50"
							: "text-textMuted hover:text-error hover:bg-background"
					}`}
					title={t("deleteQuestion") || "Delete Question"}
				>
					<svg
						className="w-5 h-5"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
						/>
					</svg>
				</button>
			</div>
		);
	}
);
