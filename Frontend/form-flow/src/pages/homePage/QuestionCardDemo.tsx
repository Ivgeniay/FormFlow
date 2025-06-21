import React, { useState } from "react";
import { QuestionType } from "../../shared/domain_types";
import {
	QuestionCard,
	QuestionData,
} from "../../modules/templates/components/questions/QuestionCard";

const mockQuestions: QuestionData[] = [
	{
		id: "1",
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

	const handleQuestionChange = (
		questionId: string,
		updatedQuestion: QuestionData
	) => {
		setQuestions((prev) =>
			prev.map((q) => (q.id === questionId ? updatedQuestion : q))
		);
	};

	const handleDeleteQuestion = (questionId: string) => {
		setQuestions((prev) => prev.filter((q) => q.id !== questionId));
	};

	const addNewQuestion = () => {
		const newQuestion: QuestionData = {
			id: Date.now().toString(),
			title: "",
			description: "",
			type: QuestionType.ShortText,
			isRequired: false,
			showInResults: true,
			typeSpecificData: {},
		};
		setQuestions((prev) => [...prev, newQuestion]);
		setActiveQuestionId(newQuestion.id);
	};

	return (
		<div className="min-h-screen bg-background p-8">
			<div className="max-w-4xl mx-auto">
				<div className="mb-8">
					<h1 className="text-3xl font-bold text-text mb-2">
						Question Card Demo
					</h1>
					<p className="text-textMuted">
						Test the QuestionCard component with different question types
					</p>
				</div>

				<div className="space-y-6">
					{questions.map((question, index) => (
						<QuestionCard
							key={question.id}
							question={question}
							onQuestionChange={(updatedQuestion) =>
								handleQuestionChange(question.id, updatedQuestion)
							}
							onDelete={() => handleDeleteQuestion(question.id)}
							isActive={activeQuestionId === question.id}
							onActivate={() => setActiveQuestionId(question.id)}
							className="transition-all duration-200"
						/>
					))}

					<div className="flex justify-center">
						<button
							onClick={addNewQuestion}
							className="flex items-center gap-2 px-6 py-3 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
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
									d="M12 4v16m8-8H4"
								/>
							</svg>
							Add Question
						</button>
					</div>
				</div>

				<div className="mt-12 p-6 bg-surface border border-border rounded-lg">
					<h2 className="text-xl font-semibold text-text mb-4">Debug Info</h2>
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
	);
};
