import React from "react";
import { FormMode } from "./FormRenderer";
import { useTranslation } from "react-i18next";

interface ScaleQuestionProps {
	questionId: string;
	title: string;
	description?: string;
	isRequired: boolean;
	minValue: number;
	maxValue: number;
	minLabel?: string;
	maxLabel?: string;
	value?: number;
	onChange: (value: number) => void;
	mode: FormMode;
	hasError?: boolean;
	errorMessage?: string;
}

export const ScaleQuestion: React.FC<ScaleQuestionProps> = ({
	questionId,
	title,
	description,
	isRequired,
	minValue,
	maxValue,
	minLabel,
	maxLabel,
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

	const currentValue = value || minValue;

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

			<div className="space-y-3">
				<div className="flex items-center justify-between text-sm text-textMuted">
					<span>{minLabel || minValue}</span>
					<span>{maxLabel || maxValue}</span>
				</div>

				<input
					type="range"
					min={minValue}
					max={maxValue}
					value={currentValue}
					onChange={(e) => onChange(parseInt(e.target.value))}
					disabled={mode === "readonly"}
					className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer slider disabled:cursor-not-allowed disabled:opacity-50"
				/>
				<div>
					{value && (value < minValue || value > maxValue) && (
						<div className="mt-2 p-2 bg-warning/10 border border-warning/30 rounded text-warning text-sm">
							⚠️ {t("questionMayHaveChanged", "Question may have been changed")}
							. {t("legacyAnswer", "Legacy answer")}: <strong>{value}</strong>
						</div>
					)}
				</div>

				<div className="text-center">
					<span className="text-text font-medium text-lg">{currentValue}</span>
				</div>
			</div>

			{hasError && errorMessage && (
				<p className="text-error text-sm mt-1">{errorMessage}</p>
			)}
		</div>
	);
};
