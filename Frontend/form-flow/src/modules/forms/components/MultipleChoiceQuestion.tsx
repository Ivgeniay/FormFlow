import React from "react";
import { FormMode } from "./FormRenderer";
import { useTranslation } from "react-i18next";

interface MultipleChoiceQuestionProps {
	questionId: string;
	title: string;
	description?: string;
	isRequired: boolean;
	options: string[];
	maxSelections?: number;
	value?: string[];
	onChange: (value: string[]) => void;
	mode: FormMode;
	hasError?: boolean;
	errorMessage?: string;
}

export const MultipleChoiceQuestion: React.FC<MultipleChoiceQuestionProps> = ({
	questionId,
	title,
	description,
	isRequired,
	options,
	maxSelections,
	value = [],
	onChange,
	mode,
	hasError = false,
	errorMessage,
}) => {
	const baseClasses = `bg-surface border rounded-lg p-4 ${
		hasError ? "border-error" : "border-border"
	}`;
	const { t } = useTranslation();

	const selectedOptions = Array.isArray(value) ? value : [];

	const handleOptionChange = (option: string, checked: boolean) => {
		if (checked) {
			if (maxSelections && selectedOptions.length >= maxSelections) {
				return;
			}
			onChange([...selectedOptions, option]);
		} else {
			onChange(selectedOptions.filter((opt) => opt !== option));
		}
	};

	const returnLegasy = (currentSelectedOptions: string[]) => {
		const invalidOptions = currentSelectedOptions.filter(
			(option) => !options.includes(option)
		);

		return (
			invalidOptions.length > 0 && (
				<div className="mt-2 p-2 bg-warning/10 border border-warning/30 rounded text-warning text-sm">
					⚠️ {t("questionMayHaveChanged", "Question may have been changed")}.{" "}
					{t("legacyAnswers", "Legacy answers")}:{" "}
					<strong>{invalidOptions.join(", ")}</strong>
				</div>
			)
		);
	};

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
				{maxSelections && (
					<p className="text-textMuted text-xs mt-1">
						{t("maxSelections", "Max selections")}: {selectedOptions.length} /{" "}
						{maxSelections}
					</p>
				)}
			</div>

			<div className="space-y-2">
				{options.map((option, index) => {
					const isChecked = selectedOptions.includes(option);
					const isDisabledByLimit =
						maxSelections !== undefined &&
						!isChecked &&
						selectedOptions.length >= maxSelections;

					return (
						<label
							key={index}
							className={`flex items-center gap-2 ${
								mode === "readonly" || isDisabledByLimit
									? "cursor-default"
									: "cursor-pointer"
							} ${isDisabledByLimit ? "opacity-50" : ""}`}
						>
							<input
								type="checkbox"
								value={option}
								checked={isChecked}
								onChange={(e) => handleOptionChange(option, e.target.checked)}
								disabled={mode === "readonly" || isDisabledByLimit}
								className="w-4 h-4 text-primary disabled:cursor-not-allowed"
							/>
							<span className="text-text">{option}</span>
						</label>
					);
				})}
			</div>
			{returnLegasy(selectedOptions)}

			{hasError && errorMessage && (
				<p className="text-error text-sm mt-1">{errorMessage}</p>
			)}
		</div>
	);
};
