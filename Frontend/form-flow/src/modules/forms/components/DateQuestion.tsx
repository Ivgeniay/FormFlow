import React from "react";
import { FormMode } from "./FormRenderer";
import { useTranslation } from "react-i18next";

interface DateQuestionProps {
	questionId: string;
	title: string;
	description?: string;
	isRequired: boolean;
	minDate?: string;
	maxDate?: string;
	value?: string;
	onChange: (value: string) => void;
	mode: FormMode;
	hasError?: boolean;
	errorMessage?: string;
}

export const DateQuestion: React.FC<DateQuestionProps> = ({
	questionId,
	title,
	description,
	isRequired,
	minDate,
	maxDate,
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

			<input
				type="date"
				value={value || ""}
				onChange={(e) => onChange(e.target.value)}
				disabled={mode === "readonly"}
				min={minDate}
				max={maxDate}
				className="w-full px-3 py-2 border border-border rounded bg-background text-text focus:border-primary focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed"
			/>

			<div>
				{value &&
					(minDate || maxDate) &&
					(() => {
						const dateValue = new Date(value);
						const minValid = !minDate || dateValue >= new Date(minDate);
						const maxValid = !maxDate || dateValue <= new Date(maxDate);

						return (
							(!minValid || !maxValid) && (
								<div className="mt-2 p-2 bg-warning/10 border border-warning/30 rounded text-warning text-sm">
									⚠️{" "}
									{t(
										"questionMayHaveChanged",
										"Question may have been changed"
									)}
									. {t("legacyAnswer", "Legacy answer")}:{" "}
									<strong>{value}</strong>
								</div>
							)
						);
					})()}
			</div>

			{(minDate || maxDate) && (
				<div className="flex items-center justify-between text-xs text-textMuted mt-2">
					{minDate && (
						<span>
							{t("fromDate", "From")} {minDate}
						</span>
					)}
					{maxDate && (
						<span>
							{t("toDate", "To")}: {maxDate}
						</span>
					)}
				</div>
			)}

			{hasError && errorMessage && (
				<p className="text-error text-sm mt-1">{errorMessage}</p>
			)}
		</div>
	);
};
