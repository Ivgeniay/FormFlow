import React from "react";
import { FormMode } from "./FormRenderer";
import { FormattedTextInput } from "../../../ui/Input/FormattedTextInput";

interface TextQuestionProps {
	questionId: string;
	title: string;
	description?: string;
	isRequired: boolean;
	placeholder?: string;
	maxLength?: number;
	multiline?: boolean;
	value?: string | null;
	onChange: (value: string) => void;
	mode: FormMode;
	hasError?: boolean;
	errorMessage?: string;
}

export const TextQuestion: React.FC<TextQuestionProps> = ({
	questionId,
	title,
	description,
	isRequired,
	placeholder,
	maxLength,
	multiline = false,
	value,
	onChange,
	mode,
	hasError = false,
	errorMessage,
}) => {
	const baseClasses = `bg-surface border rounded-lg p-4 ${
		hasError ? "border-error" : "border-border"
	}`;

	const getTextLength = (htmlString: string): number => {
		const textContent =
			new DOMParser().parseFromString(htmlString, "text/html").body
				.textContent || "";
		return textContent.length;
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
			</div>

			<FormattedTextInput
				value={value || ""}
				onChange={onChange}
				placeholder={placeholder || ""}
				isReadOnly={mode === "readonly"}
				multiline={multiline}
				maxLength={maxLength}
			/>

			{maxLength && (
				<div className="flex justify-end mt-1">
					<span className="text-xs text-textMuted">
						{getTextLength(value || "")} / {maxLength}
					</span>
				</div>
			)}

			{hasError && errorMessage && (
				<p className="text-error text-sm mt-1">{errorMessage}</p>
			)}
		</div>
	);
};
