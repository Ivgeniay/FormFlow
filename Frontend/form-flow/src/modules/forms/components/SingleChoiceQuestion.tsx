import React from "react";
import { FormMode } from "./FormRenderer";
import { useTranslation } from "react-i18next";

interface SingleChoiceQuestionProps {
	questionId: string;
	title: string;
	description?: string;
	isRequired: boolean;
	options: string[];
	value?: string;
	onChange: (value: string) => void;
	mode: FormMode;
	hasError?: boolean;
	errorMessage?: string;
}

export const SingleChoiceQuestion: React.FC<SingleChoiceQuestionProps> = ({
	questionId,
	title,
	description,
	isRequired,
	options,
	value,
	onChange,
	mode,
	hasError = false,
	errorMessage,
}) => {
	const baseClasses = `bg-surface border rounded-lg p-4 ${
		hasError ? "border-error" : "border-border"
	}`;
	const { t } = useTranslation();

	return (
		<div className={baseClasses}>
			<div className="mb-3">
				<h3 className="text-text font-medium">
					{title}
					{isRequired && <span className="text-error ml-1">*</span>}
				</h3>
				{description && (
					<p className="text-textMuted text-sm mt-1">{description}</p>
				)}
			</div>

			<div className="space-y-2">
				{options.map((option, index) => (
					<label
						key={index}
						className={`flex items-center gap-2 ${
							mode === "readonly" ? "cursor-default" : "cursor-pointer"
						}`}
					>
						<input
							type="radio"
							name={`question_${questionId}`}
							value={option}
							checked={value === option}
							onChange={(e) => onChange(e.target.value)}
							disabled={mode === "readonly"}
							className="w-4 h-4 text-primary disabled:cursor-not-allowed"
						/>
						<span className="text-text">{option}</span>
					</label>
				))}
			</div>

			{value && !options.includes(value) && (
				<div className="mt-2 p-2 bg-warning/10 border border-warning/30 rounded text-warning text-sm">
					⚠️ {t("questionMayHaveChanged", "Question may have been changed")}.{" "}
					{t("legacyAnswer", "Legacy answer")}: <strong>{value}</strong>
				</div>
			)}

			{hasError && errorMessage && (
				<p className="text-error text-sm mt-1">{errorMessage}</p>
			)}
		</div>
	);
};
