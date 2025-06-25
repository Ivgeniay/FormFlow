import React from "react";
import { FormMode } from "./FormRenderer";
import { useTranslation } from "react-i18next";

interface TimeQuestionProps {
	questionId: string;
	title: string;
	description?: string;
	isRequired: boolean;
	minTime?: string;
	maxTime?: string;
	value?: string;
	onChange: (value: string) => void;
	mode: FormMode;
	hasError?: boolean;
	errorMessage?: string;
}

export const TimeQuestion: React.FC<TimeQuestionProps> = ({
	questionId,
	title,
	description,
	isRequired,
	minTime,
	maxTime,
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
				type="time"
				value={value || ""}
				onChange={(e) => onChange(e.target.value)}
				disabled={mode === "readonly"}
				min={minTime}
				max={maxTime}
				className="w-full px-3 py-2 border border-border rounded bg-background text-text focus:border-primary focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed"
			/>
			<div>
				{value &&
					(minTime || maxTime) &&
					(() => {
						const timeValid =
							(!minTime || value >= minTime) && (!maxTime || value <= maxTime);

						return (
							!timeValid && (
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

			{(minTime || maxTime) && (
				<div className="flex items-center justify-between text-xs text-textMuted mt-2">
					{minTime && (
						<span>
							{t("fromTime", "From")}: {minTime}
						</span>
					)}
					{maxTime && (
						<span>
							{t("toTime", "To")}: {maxTime}
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
