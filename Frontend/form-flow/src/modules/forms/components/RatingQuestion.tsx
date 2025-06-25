import React from "react";
import { FormMode } from "./FormRenderer";
import { useTranslation } from "react-i18next";

interface RatingQuestionProps {
	questionId: string;
	title: string;
	description?: string;
	isRequired: boolean;
	maxRating: number;
	value?: number;
	onChange: (value: number) => void;
	mode: FormMode;
	hasError?: boolean;
	errorMessage?: string;
}

export const RatingQuestion: React.FC<RatingQuestionProps> = ({
	questionId,
	title,
	description,
	isRequired,
	maxRating,
	value,
	onChange,
	mode,
	hasError = false,
	errorMessage,
}) => {
	const { t } = useTranslation();
	const baseClasses = `bg-surface border rounded-lg p-4 ${
		hasError ? "border-error" : "border-border"
	}`;

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

			<div className="flex items-center gap-1">
				{Array.from({ length: maxRating }, (_, index) => {
					const starValue = index + 1;
					const isSelected = value && value >= starValue;
					const isHovered = mode !== "readonly";

					return (
						<button
							key={index}
							type="button"
							onClick={() => onChange(starValue)}
							disabled={mode === "readonly"}
							className={`text-2xl transition-colors duration-200 ${
								isSelected ? "text-yellow-400" : "text-gray-300"
							} ${
								isHovered
									? "hover:text-yellow-300 hover:scale-110 cursor-pointer"
									: "cursor-default"
							} disabled:cursor-not-allowed disabled:hover:scale-100`}
						>
							★
						</button>
					);
				})}

				{value && (
					<span className="ml-3 text-textMuted font-medium">
						{value} / {maxRating}
					</span>
				)}
			</div>
			<div>
				{value && value > maxRating && (
					<div className="mt-2 p-2 bg-warning/10 border border-warning/30 rounded text-warning text-sm">
						⚠️ {t("questionMayHaveChanged", "Question may have been changed")}.{" "}
						{t("legacyAnswer", "Legacy answer")}: <strong>{value}</strong>
					</div>
				)}
			</div>

			{hasError && errorMessage && (
				<p className="text-error text-sm mt-1">{errorMessage}</p>
			)}
		</div>
	);
};
