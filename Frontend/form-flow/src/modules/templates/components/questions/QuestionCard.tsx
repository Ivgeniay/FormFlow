import { QuestionType } from "../../../../shared/domain_types";

import React from "react";
import { useTranslation } from "react-i18next";
import { DragHandle } from "../../../../ui/Input/DragHandle";
import { FormattedTextInput } from "../../../../ui/Input/FormattedTextInput";
import { QuestionTypeSelect } from "../../../../ui/Input/QuestionTypeSelect";
import { QuestionSettings } from "../../../../ui/Input/QuestionSettings";
import { RequiredToggle } from "../../../../ui/Input/RequiredToggle";

interface QuestionCardProps {
	question: QuestionData;
	onQuestionChange: (question: QuestionData) => void;
	onDelete: () => void;
	isActive?: boolean;
	onActivate?: () => void;
	dragHandleProps?: any;
	className?: string;
}

export interface QuestionData {
	id: string;
	title: string;
	description: string;
	type: QuestionType;
	isRequired: boolean;
	showInResults: boolean;
	typeSpecificData: Record<string, any>;
}

export const QuestionCard: React.FC<QuestionCardProps> = ({
	question,
	onQuestionChange,
	onDelete,
	isActive = false,
	onActivate,
	dragHandleProps,
	className = "",
}) => {
	const { t } = useTranslation();

	const handleFieldChange = (fieldName: keyof QuestionData, value: any) => {
		const updatedQuestion = {
			...question,
			[fieldName]: value,
		};
		onQuestionChange(updatedQuestion);
	};

	const handleTypeChange = (newType: QuestionType) => {
		const updatedQuestion = {
			...question,
			type: newType,
			typeSpecificData: {},
		};
		onQuestionChange(updatedQuestion);
	};

	const handleTypeSpecificDataChange = (data: Record<string, any>) => {
		const updatedQuestion = {
			...question,
			typeSpecificData: data,
		};
		onQuestionChange(updatedQuestion);
	};

	return (
		<div
			className={`bg-surface border-2 rounded-lg transition-all duration-200 ${
				isActive
					? "border-primary shadow-md"
					: "border-border hover:border-primary/50"
			} ${className}`}
			onClick={onActivate}
		>
			<div className="relative">
				<DragHandle dragHandleProps={dragHandleProps} />

				<div className="p-6 pt-12">
					<div className="space-y-4">
						<div className="flex items-center gap-4">
							<div className="flex-1">
								<FormattedTextInput
									value={question.title}
									onChange={(value) => handleFieldChange("title", value)}
									placeholder={t("questionPlaceholder") || "Question"}
								/>
							</div>

							<QuestionTypeSelect
								value={question.type}
								onChange={handleTypeChange}
							/>
						</div>

						<FormattedTextInput
							value={question.description}
							onChange={(value) => handleFieldChange("description", value)}
							placeholder={
								t("descriptionPlaceholder") || "Description (optional)"
							}
							multiline
						/>

						<QuestionSettings
							question={question}
							onChange={handleTypeSpecificDataChange}
						/>
					</div>

					<div className="flex items-center justify-between mt-6 pt-4 border-t border-border">
						<div className="flex items-center gap-4">
							<button
								onClick={onDelete}
								className="p-2 text-textMuted hover:text-error transition-colors"
								title={t("deleteQuestion") || "Delete question"}
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

						<RequiredToggle
							value={question.isRequired}
							onChange={(value) => handleFieldChange("isRequired", value)}
						/>
					</div>
				</div>
			</div>
		</div>
	);
};
