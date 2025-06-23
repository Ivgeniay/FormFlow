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

interface FormTemplateProps {
	formTemplate: FormTemplate;
}

export const TemplateEditor: React.FC<FormTemplateProps> = ({
	formTemplate,
}) => {
	const [formTemplateState, setFormTemplate] =
		useState<FormTemplate>(formTemplate);

	const [activeQuestionId, setActiveQuestionId] = useState<string | null>(null);
	const toolbarRef = useRef<HTMLDivElement>(null);
	const cardRefs = useRef<Map<string, HTMLDivElement>>(new Map());

	const handleQuestionChange = (
		questionId: string,
		updatedQuestion: QuestionData
	) => {
		setFormTemplate((p) => ({
			...p,
			questions: p.questions.map((q) =>
				q.id === questionId ? updatedQuestion : q
			),
		}));
	};

	const updateQuestions = (
		updater: (prev: QuestionData[]) => QuestionData[]
	) => {
		setFormTemplate((prev) => ({
			...prev,
			questions: updater(prev.questions),
		}));
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

		const questionToDuplicate = formTemplateState.questions.find(
			(q) => q.id === activeQuestionId
		);
		if (!questionToDuplicate) return;

		const newQuestion: QuestionData = {
			...questionToDuplicate,
			id: Date.now().toString(),
			order: questionToDuplicate.order + 1,
			title: questionToDuplicate.title + " (Copy)",
		};

		const activeIndex = formTemplateState.questions.findIndex(
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

	const addNewQuestion = () => {
		const activeIndex = activeQuestionId
			? formTemplateState.questions.findIndex((q) => q.id === activeQuestionId)
			: -1;
		const newOrder =
			activeIndex !== -1
				? formTemplateState.questions[activeIndex].order + 1
				: formTemplateState.questions.length + 1;

		const newQuestion: QuestionData = {
			id: Date.now().toString(),
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
			onDragEnd={handleDragEnd}
			modifiers={[restrictToVerticalAxis]}
		>
			<SortableContext
				items={formTemplateState.questions.map((q) => q.id)}
				strategy={verticalListSortingStrategy}
			>
				<div className="min-h-screen bg-background p-8">
					<div className="max-w-2xl mx-auto">
						<TemplateEditorHeader
							data={{
								title: formTemplateState.title,
								description: formTemplateState.description,
								image: formTemplateState.image,
								topicId: formTemplateState.topicId,
								accessType: formTemplateState.accessType,
								tags: formTemplateState.tags,
								allowedUserIds: formTemplateState.allowedUserIds,
							}}
							onDataChange={(headerData) => {
								setFormTemplate((prev) => ({
									...prev,
									...headerData,
								}));
							}}
						/>

						<div className="space-y-2">
							{formTemplateState.questions.map((question, index) => (
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
								/>
							))}
						</div>

						<QuestionToolbar
							ref={toolbarRef}
							isVisible={activeQuestionId !== null}
							onAddQuestion={addNewQuestion}
							onDuplicateQuestion={duplicateQuestion}
							onDeleteQuestion={deleteActiveQuestion}
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
									{formTemplateState.questions.length}
								</div>
								<details className="mt-4">
									<summary className="cursor-pointer text-primary">
										View Questions Data (JSON)
									</summary>
									<pre className="mt-2 p-4 bg-background border border-border rounded text-xs overflow-auto">
										{JSON.stringify(formTemplateState.questions, null, 2)}
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
