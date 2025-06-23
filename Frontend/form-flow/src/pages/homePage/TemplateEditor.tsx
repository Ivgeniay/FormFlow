import React, { useEffect, useRef, useState } from "react";
import { QuestionType } from "../../shared/domain_types";
import { QuestionCard } from "../../modules/templates/components/questions/QuestionCard";
import { DndContext, closestCenter, DragEndEvent } from "@dnd-kit/core";
import {
	SortableContext,
	verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { restrictToVerticalAxis } from "@dnd-kit/modifiers";
import { QuestionToolbar } from "../../modules/templates/components/questions/QuestionToolbar";
import {
	FormTemplate,
	QuestionData,
} from "../../modules/templates/types/types";
import { TemplateEditorHeader } from "../../modules/templates/components/questions/TemplateEditorHeader";

export type mode = "create" | "edit" | "readonly";

interface FormTemplateProps {
	formTemplate: FormTemplate;
	onFormTemplateChange: (template: FormTemplate) => void;
	mode: mode;
}

export const TemplateEditor: React.FC<FormTemplateProps> = ({
	formTemplate,
	onFormTemplateChange,
	mode,
}) => {
	// const [formTemplateState, setFormTemplate] = useState<FormTemplate>(formTemplate);

	const [activeQuestionId, setActiveQuestionId] = useState<string | null>(null);
	const toolbarRef = useRef<HTMLDivElement>(null);
	const cardRefs = useRef<Map<string, HTMLDivElement>>(new Map());

	const handleQuestionChange = (
		questionId: string,
		updatedQuestion: QuestionData
	) => {
		const updatedTemplate = {
			...formTemplate,
			questions: formTemplate.questions.map((q) =>
				q.id === questionId ? updatedQuestion : q
			),
		};
		onFormTemplateChange(updatedTemplate);
	};

	const updateQuestions = (
		updater: (prev: QuestionData[]) => QuestionData[]
	) => {
		const updatedTemplate = {
			...formTemplate,
			questions: updater(formTemplate.questions),
		};
		onFormTemplateChange(updatedTemplate);
	};

	const setCardRef = (questionId: string, element: HTMLDivElement | null) => {
		if (element) {
			cardRefs.current.set(questionId, element);
		} else {
			cardRefs.current.delete(questionId);
		}
	};

	const updateToolbarPosition = () => {
		if (!activeQuestionId || !toolbarRef.current) {
			return;
		}

		const activeCard = cardRefs.current.get(activeQuestionId);
		if (!activeCard) return;

		const cardRect = activeCard.getBoundingClientRect();
		const toolbar = toolbarRef.current;

		toolbar.style.position = "fixed";
		toolbar.style.top = `${cardRect.top + cardRect.height / 2 - 75}px`;
		toolbar.style.left = `${cardRect.right + 16}px`;
	};

	useEffect(() => {
		if (activeQuestionId) {
			setTimeout(updateToolbarPosition, 0);
			window.addEventListener("scroll", updateToolbarPosition);
			window.addEventListener("resize", updateToolbarPosition);
		}

		return () => {
			window.removeEventListener("scroll", updateToolbarPosition);
			window.removeEventListener("resize", updateToolbarPosition);
		};
	}, [activeQuestionId]);

	const duplicateQuestion = () => {
		if (!activeQuestionId) return;

		const questionToDuplicate = formTemplate.questions.find(
			(q) => q.id === activeQuestionId
		);
		if (!questionToDuplicate) return;

		const newId = getNextQuestionId();

		const newQuestion: QuestionData = {
			...questionToDuplicate,
			id: newId,
			order: questionToDuplicate.order + 1,
			title: questionToDuplicate.title + " (Copy)",
		};

		const activeIndex = formTemplate.questions.findIndex(
			(q) => q.id === activeQuestionId
		);

		updateQuestions((prev) => {
			const updatedQuestions = [...prev];
			updatedQuestions.splice(activeIndex + 1, 0, newQuestion);
			return reorderQuestions(updatedQuestions);
		});

		setActiveQuestionId(newQuestion.id);
	};

	const deleteActiveQuestion = () => {
		if (!activeQuestionId) return;
		handleDeleteQuestion(activeQuestionId);
		setActiveQuestionId(null);
	};

	const handleDeleteQuestion = (questionId: string) => {
		updateQuestions((prev) => {
			const filtered = prev.filter((q) => q.id !== questionId);
			return reorderQuestions(filtered);
		});
	};

	const getNextQuestionId = () => {
		if (formTemplate.questions.length === 0) {
			return "1";
		}
		const maxId = Math.max(
			...formTemplate.questions.map((q) => parseInt(q.id) || 0)
		);
		return (maxId + 1).toString();
	};

	const addNewQuestion = () => {
		const activeIndex = activeQuestionId
			? formTemplate.questions.findIndex((q) => q.id === activeQuestionId)
			: -1;
		const newOrder =
			activeIndex !== -1
				? formTemplate.questions[activeIndex].order + 1
				: formTemplate.questions.length + 1;

		const newId = getNextQuestionId();

		const newQuestion: QuestionData = {
			id: newId,
			order: newOrder,
			title: "",
			description: "",
			type: QuestionType.ShortText,
			isRequired: false,
			showInResults: true,
			typeSpecificData: {},
		};

		updateQuestions((prev) => {
			const updatedQuestions = activeQuestionId
				? insertQuestionAfterActive(prev, newQuestion, activeQuestionId)
				: [...prev, newQuestion];

			return reorderQuestions(updatedQuestions);
		});
		setActiveQuestionId(newQuestion.id);
	};

	const insertQuestionAfterActive = (
		questions: QuestionData[],
		newQuestion: QuestionData,
		activeId: string
	) => {
		const activeIndex = questions.findIndex((q) => q.id === activeId);
		if (activeIndex === -1) return [...questions, newQuestion];

		const updatedQuestions = [...questions];
		updatedQuestions.splice(activeIndex + 1, 0, newQuestion);
		return updatedQuestions;
	};

	const reorderQuestions = (questions: QuestionData[]) => {
		return questions.map((question, index) => ({
			...question,
			order: index + 1,
		}));
	};

	const handleDragEnd = (event: DragEndEvent) => {
		const { active, over } = event;

		if (over && active.id !== over.id) {
			updateQuestions((prev) => {
				const oldIndex = prev.findIndex((q) => q.id === active.id);
				const newIndex = prev.findIndex((q) => q.id === over.id);

				const updatedQuestions = [...prev];
				const [movedQuestion] = updatedQuestions.splice(oldIndex, 1);
				updatedQuestions.splice(newIndex, 0, movedQuestion);

				return reorderQuestions(updatedQuestions);
			});
		}
	};

	return (
		<DndContext
			collisionDetection={closestCenter}
			onDragEnd={mode === "readonly" ? undefined : handleDragEnd}
			modifiers={[restrictToVerticalAxis]}
		>
			<SortableContext
				items={formTemplate.questions.map((q) => q.id)}
				strategy={verticalListSortingStrategy}
			>
				<div className="min-h-screen bg-background p-8">
					<div className="max-w-2xl mx-auto">
						<TemplateEditorHeader
							data={{
								title: formTemplate.title,
								description: formTemplate.description,
								image: formTemplate.image,
								topicId: formTemplate.topicId,
								accessType: formTemplate.accessType,
								tags: formTemplate.tags,
								allowedUserIds: formTemplate.allowedUserIds,
							}}
							onDataChange={(headerData) => {
								onFormTemplateChange({ ...formTemplate, ...headerData });
							}}
							mode={mode}
						/>

						<div className="space-y-2">
							{formTemplate.questions.map((question, index) => (
								<QuestionCard
									key={question.id}
									ref={(el) => setCardRef(question.id, el)}
									question={question}
									onQuestionChange={(updatedQuestion) =>
										handleQuestionChange(question.id, updatedQuestion)
									}
									onDelete={() => handleDeleteQuestion(question.id)}
									isActive={activeQuestionId === question.id}
									onActivate={() => setActiveQuestionId(question.id)}
									mode={mode}
								/>
							))}
						</div>

						<QuestionToolbar
							ref={toolbarRef}
							isVisible={activeQuestionId !== null}
							onAddQuestion={addNewQuestion}
							onDuplicateQuestion={duplicateQuestion}
							onDeleteQuestion={deleteActiveQuestion}
							mode={mode}
							binDisable={formTemplate.questions.length < 2}
						/>

						<div className="mt-12 p-6 bg-surface border border-border rounded-lg">
							<h2 className="text-xl font-semibold text-text mb-4">
								Debug Info
							</h2>
							<div className="space-y-4">
								<div>
									<strong>Active Question:</strong> {activeQuestionId || "None"}
								</div>
								<div>
									<strong>Total Questions:</strong>{" "}
									{formTemplate.questions.length}
								</div>
								<details className="mt-4">
									<summary className="cursor-pointer text-primary">
										View Questions Data (JSON)
									</summary>
									<pre className="mt-2 p-4 bg-background border border-border rounded text-xs overflow-auto">
										{JSON.stringify(formTemplate.questions, null, 2)}
									</pre>
								</details>
							</div>
						</div>
					</div>
				</div>
			</SortableContext>
		</DndContext>
	);
};
