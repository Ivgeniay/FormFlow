import React, { useEffect, useRef, useState } from "react";
import { QuestionType } from "../../shared/domain_types";
import {
	QuestionCard,
	QuestionData,
} from "../../modules/templates/components/questions/QuestionCard";
import { DndContext, closestCenter, DragEndEvent } from "@dnd-kit/core";
import {
	SortableContext,
	verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { restrictToVerticalAxis } from "@dnd-kit/modifiers";
import { QuestionToolbar } from "../../ui/Input/QuestionToolbar";
import { ImageUploader } from "../../ui/Input/ImageUploader";
import { FormattedTextInput } from "../../ui/Input/FormattedTextInput";
import { useTranslation } from "react-i18next";

const mockQuestions: QuestionData[] = [
	{
		id: "1",
		order: 0,
		title: "What is your name?",
		description: "Please enter your full name",
		type: QuestionType.ShortText,
		isRequired: true,
		showInResults: true,
		typeSpecificData: {
			maxLength: 100,
			placeholder: "Enter your name",
		},
	},
	{
		id: "2",
		order: 1,
		title: "Tell us about yourself",
		description: "Share your background and interests",
		type: QuestionType.LongText,
		isRequired: false,
		showInResults: true,
		typeSpecificData: {
			maxLength: 500,
			placeholder: "Describe yourself...",
		},
	},
	{
		id: "3",
		order: 2,
		title: "What is your favorite color?",
		description: "",
		type: QuestionType.SingleChoice,
		isRequired: true,
		showInResults: true,
		typeSpecificData: {
			options: ["Red", "Blue", "Green", "Yellow", "Other"],
		},
	},
	{
		id: "4",
		order: 3,
		title: "Which programming languages do you know?",
		description: "Select all that apply",
		type: QuestionType.MultipleChoice,
		isRequired: false,
		showInResults: true,
		typeSpecificData: {
			options: ["JavaScript", "Python", "Java", "C#", "Go", "Rust"],
			maxSelections: 3,
		},
	},
	{
		id: "5",
		order: 4,
		title: "Rate your experience with React",
		description: "Scale from 1 (beginner) to 5 (expert)",
		type: QuestionType.Scale,
		isRequired: true,
		showInResults: true,
		typeSpecificData: {
			minValue: 1,
			maxValue: 5,
			minLabel: "Beginner",
			maxLabel: "Expert",
		},
	},
];

export const QuestionCardDemo: React.FC = () => {
	const [questions, setQuestions] = useState<QuestionData[]>(mockQuestions);
	const [activeQuestionId, setActiveQuestionId] = useState<string | null>(null);
	const toolbarRef = useRef<HTMLDivElement>(null);
	const cardRefs = useRef<Map<string, HTMLDivElement>>(new Map());
	const [formTitle, setFormTitle] = useState("");
	const [formDescription, setFormDescription] = useState("");
	const [formImage, setFormImage] = useState<File | null>(null);
	const { t } = useTranslation();

	const handleImageSelect = (file: File | null) => {
		setFormImage(file);
	};

	const handleQuestionChange = (
		questionId: string,
		updatedQuestion: QuestionData
	) => {
		setQuestions((prev) =>
			prev.map((q) => (q.id === questionId ? updatedQuestion : q))
		);
	};

	const setCardRef = (questionId: string, element: HTMLDivElement | null) => {
		console.log("setCardRef called:", questionId, element);
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

		const questionToDuplicate = questions.find(
			(q) => q.id === activeQuestionId
		);
		if (!questionToDuplicate) return;

		const newQuestion: QuestionData = {
			...questionToDuplicate,
			id: Date.now().toString(),
			title: questionToDuplicate.title + " (Copy)",
		};

		const activeIndex = questions.findIndex((q) => q.id === activeQuestionId);
		const updatedQuestions = [...questions];
		updatedQuestions.splice(activeIndex + 1, 0, newQuestion);
		setQuestions(updatedQuestions);
		setActiveQuestionId(newQuestion.id);
	};

	const deleteActiveQuestion = () => {
		if (!activeQuestionId) return;
		handleDeleteQuestion(activeQuestionId);
		setActiveQuestionId(null);
	};

	const handleDeleteQuestion = (questionId: string) => {
		setQuestions((prev) => {
			const filtered = prev.filter((q) => q.id !== questionId);
			return reorderQuestions(filtered);
		});
	};

	const addNewQuestion = () => {
		const activeIndex = activeQuestionId
			? questions.findIndex((q) => q.id === activeQuestionId)
			: -1;
		const newOrder =
			activeIndex !== -1
				? questions[activeIndex].order + 1
				: questions.length + 1;

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

		setQuestions((prev) => {
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
			const oldIndex = questions.findIndex((q) => q.id === active.id);
			const newIndex = questions.findIndex((q) => q.id === over.id);

			const updatedQuestions = [...questions];
			const [movedQuestion] = updatedQuestions.splice(oldIndex, 1);
			updatedQuestions.splice(newIndex, 0, movedQuestion);

			updatedQuestions.forEach((question, index) => {
				question.order = index + 1;
			});
			setQuestions(updatedQuestions);
		}
	};

	return (
		<DndContext
			collisionDetection={closestCenter}
			onDragEnd={handleDragEnd}
			modifiers={[restrictToVerticalAxis]}
		>
			<SortableContext
				items={questions.map((q) => q.id)}
				strategy={verticalListSortingStrategy}
			>
				<div className="min-h-screen bg-background p-8">
					<div className="max-w-2xl mx-auto">
						<div className="mb-8 p-6 bg-surface border border-border rounded-lg">
							<h2 className="text-xl font-semibold text-text mb-4">
								{t("formSettings") || "Form Settings"}
							</h2>

							<div className="space-y-3">
								<div>
									<ImageUploader
										onImageSelect={handleImageSelect}
										currentImage={null}
									/>
								</div>

								<div>
									<FormattedTextInput
										value={formTitle || t("formTitle") || "Form Title"}
										onChange={setFormTitle}
										placeholder={t("enterFormTitle") || "Enter form title"}
									/>
								</div>

								<div>
									<FormattedTextInput
										value={
											formDescription ||
											t("formDescription") ||
											"Form Description"
										}
										onChange={setFormDescription}
										placeholder={
											t("enterFormDescription") || "Enter form description"
										}
										multiline
									/>
								</div>
							</div>
						</div>

						<div className="space-y-2">
							{questions.map((question, index) => (
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
									<strong>Total Questions:</strong> {questions.length}
								</div>
								<details className="mt-4">
									<summary className="cursor-pointer text-primary">
										View Questions Data (JSON)
									</summary>
									<pre className="mt-2 p-4 bg-background border border-border rounded text-xs overflow-auto">
										{JSON.stringify(questions, null, 2)}
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
