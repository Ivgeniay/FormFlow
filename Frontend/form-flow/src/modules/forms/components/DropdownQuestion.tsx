import React from "react";
import { useTranslation } from "react-i18next";
import { FormMode } from "./FormRenderer";

interface DropdownQuestionProps {
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

export const DropdownQuestion: React.FC<DropdownQuestionProps> = ({
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

			<select
				value={value || ""}
				onChange={(e) => onChange(e.target.value)}
				disabled={mode === "readonly"}
				className="w-full px-3 py-2 border border-border rounded bg-background text-text focus:border-primary focus:outline-none disabled:opacity-50"
			>
				<option value="">{t("selectOption", "Select an option")}</option>
				{options.map((option, index) => (
					<option key={index} value={option}>
						{option}
					</option>
				))}
			</select>
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
