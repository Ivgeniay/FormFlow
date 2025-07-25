import { QuestionType } from "../../../../shared/domain_types";

import React from "react";
import { useTranslation } from "react-i18next";
import { DragHandle } from "./DragHandle";
import { FormattedTextInput } from "../../../../ui/Input/FormattedTextInput";
import { QuestionTypeSelect } from "./QuestionTypeSelect";
import { QuestionSettings } from "./QuestionSettings";
import { RequiredToggle } from "../../../../ui/Input/RequiredToggle";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { QuestionData } from "../../types/types";
import { mode } from "../../../../pages/templates/TemplateEditor";

interface QuestionCardProps {
	question: QuestionData;
	onQuestionChange: (question: QuestionData) => void;
	onDelete: () => void;
	isActive?: boolean;
	onActivate?: () => void;
	dragHandleProps?: any;
	className?: string;
	mode: mode;
}

export const QuestionCard = React.forwardRef<HTMLDivElement, QuestionCardProps>(
	(
		{
			question,
			onQuestionChange,
			onDelete,
			isActive = false,
			onActivate,
			dragHandleProps,
			className = "",
			mode,
		},
		ref
	) => {
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

		const {
			attributes,
			listeners,
			setNodeRef,
			transform,
			transition,
			isDragging,
		} = useSortable({ id: question.id, animateLayoutChanges: () => false });

		const style = {
			transform: CSS.Transform.toString(transform),
			transition: isDragging ? "none" : undefined,
			// transition,
		};

		const combinedRef = (element: HTMLDivElement | null) => {
			setNodeRef(element);
			if (ref && typeof ref === "function") {
				ref(element);
			} else if (ref) {
				ref.current = element;
			}
		};

		return (
			<div
				ref={combinedRef}
				style={style}
				className={`relative bg-surface border-2 ${
					isActive
						? "border-primary shadow-md"
						: "border-border hover:border-primary/50"
				} ${isDragging ? "opacity-50" : "opacity-100"} ${className}`}
				onClick={onActivate}
			>
				<>
					<DragHandle
						dragHandleProps={{ ...attributes, ...listeners }}
						mode={mode}
					/>

					<div className="p-4 pt-10">
						<div className="space-y-3">
							<div className="flex items-center gap-3">
								<div className="flex-1">
									<FormattedTextInput
										value={question.title}
										onChange={(value) => handleFieldChange("title", value)}
										placeholder={t("questionPlaceholder") || "Question"}
										isReadOnly={mode === "readonly"}
									/>
								</div>

								<QuestionTypeSelect
									value={question.type}
									onChange={handleTypeChange}
									isDisable={mode !== "create"}
								/>
							</div>

							<FormattedTextInput
								value={question.description}
								onChange={(value) => handleFieldChange("description", value)}
								placeholder={
									t("descriptionPlaceholder") || "Description (optional)"
								}
								multiline
								isReadOnly={mode === "readonly"}
							/>

							<QuestionSettings
								question={question}
								onChange={handleTypeSpecificDataChange}
								mode={mode}
							/>
						</div>

						<div className="flex items-center justify-between mt-4 pt-3 border-t border-border">
							<RequiredToggle
								value={question.isRequired}
								onChange={(value) => handleFieldChange("isRequired", value)}
								isReadOnly={mode === "readonly"}
							/>
						</div>
					</div>
				</>
			</div>
		);
	}
);
