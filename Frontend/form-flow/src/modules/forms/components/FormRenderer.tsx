import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { TemplateDto, FormDto } from "../../../shared/api_types";
import { QuestionType } from "../../../shared/domain_types";
import toast from "react-hot-toast";
import { TextQuestion } from "./TextQuestion";
import { DropdownQuestion } from "./DropdownQuestion";
import { ScaleQuestion } from "./ScaleQuestion";
import { RatingQuestion } from "./RatingQuestion";
import { DateQuestion } from "./DateQuestion";
import { TimeQuestion } from "./TimeQuestion";
import { SingleChoiceQuestion } from "./SingleChoiceQuestion";
import { MultipleChoiceQuestion } from "./MultipleChoiceQuestion";

export type FormMode = "fillable" | "readonly" | "preview";

interface FormRendererProps {
	template: TemplateDto;
	mode: FormMode;
	initialAnswers?: Record<string, any>;
	existingForm?: FormDto;
	onSubmit?: (
		answers: Record<string, any>,
		sendCopyToEmail: boolean
	) => Promise<void>;
}

interface ValidationError {
	questionId: string;
	message: string;
}

export const FormRenderer: React.FC<FormRendererProps> = ({
	template,
	mode,
	initialAnswers = {},
	existingForm,
	onSubmit,
}) => {
	const { t } = useTranslation();

	const [answers, setAnswers] = useState<Record<string, any>>(initialAnswers);
	const [validationErrors, setValidationErrors] = useState<ValidationError[]>(
		[]
	);
	const [sendCopyToEmail, setSendCopyToEmail] = useState(false);
	const [isSubmitting, setIsSubmitting] = useState(false);

	const updateAnswer = (questionId: string, value: any) => {
		if (mode === "readonly") return;

		setAnswers((prev) => ({
			...prev,
			[questionId]: value,
		}));

		setValidationErrors((prev) =>
			prev.filter((error) => error.questionId !== questionId)
		);
	};

	const validateForm = (): ValidationError[] => {
		const errors: ValidationError[] = [];

		template.questions.forEach((question) => {
			if (question.isRequired) {
				const answer = answers[question.id];
				const isEmpty =
					answer === undefined || answer === null || answer === "";

				if (isEmpty) {
					errors.push({
						questionId: question.id,
						message: t("fieldRequired", "This field is required"),
					});
				}
			}
		});

		return errors;
	};

	const getDefaultValueForQuestion = (question: any) => {
		const questionData =
			typeof question.data === "string"
				? JSON.parse(question.data)
				: question.data;

		switch (questionData.type) {
			case QuestionType.MultipleChoice:
				return [];
			default:
				return "";
		}
	};

	const handleSubmit = async () => {
		if (mode === "readonly" || !onSubmit) return;

		const errors = validateForm();
		if (errors.length > 0) {
			setValidationErrors(errors);
			toast.error(t("fixValidationErrors", "Please fix the validation errors"));
			return;
		}

		const completeAnswers: Record<string, any> = { ...answers };
		template.questions.forEach((question) => {
			if (!(question.id in completeAnswers)) {
				completeAnswers[question.id] = getDefaultValueForQuestion(question);
			}
		});

		try {
			setIsSubmitting(true);
			await onSubmit(completeAnswers, sendCopyToEmail);
			toast.success(
				t("formSubmittedSuccessfully", "Form submitted successfully!")
			);
		} catch (error) {
			toast.error(t("formSubmissionFailed", "Failed to submit form"));
		} finally {
			setIsSubmitting(false);
		}
	};

	const getAnswerValue = (questionId: string) => {
		if (mode === "fillable" && answers[questionId] !== undefined) {
			return answers[questionId];
		}

		if (existingForm?.questions && Array.isArray(existingForm.questions)) {
			const questionData = existingForm.questions.find(
				(q) => q.questionId === questionId
			);
			const answer = questionData?.answer || null;
			return answer;
		}

		console.log("No answer found for:", questionId);
		return null;
	};

	const renderQuestion = (question: any) => {
		const questionData =
			typeof question.data === "string"
				? JSON.parse(question.data)
				: question.data;
		const hasError = validationErrors.some(
			(error) => error.questionId === question.id
		);
		const errorMessage = validationErrors.find(
			(error) => error.questionId === question.id
		)?.message;

		const baseClasses = `bg-surface border rounded-lg p-4 ${
			hasError ? "border-error" : "border-border"
		}`;

		switch (questionData.type) {
			case QuestionType.ShortText:
				return (
					<TextQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						placeholder={questionData.typeSpecificData?.placeholder}
						maxLength={
							questionData.typeSpecificData?.maxLength
								? Number(questionData.typeSpecificData.maxLength)
								: undefined
						}
						multiline={false}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			case QuestionType.LongText:
				return (
					<TextQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						placeholder={questionData.typeSpecificData?.placeholder}
						maxLength={
							questionData.typeSpecificData?.maxLength
								? Number(questionData.typeSpecificData.maxLength)
								: undefined
						}
						multiline={true}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			case QuestionType.SingleChoice:
				return (
					<SingleChoiceQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						options={questionData.typeSpecificData?.options || []}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			case QuestionType.MultipleChoice:
				const multipleChoiceValue = getAnswerValue(question.id);
				console.log(
					"MultipleChoice value:",
					multipleChoiceValue,
					typeof multipleChoiceValue
				);

				return (
					<MultipleChoiceQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						options={questionData.typeSpecificData?.options || []}
						maxSelections={questionData.typeSpecificData?.maxSelections}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			case QuestionType.Dropdown:
				return (
					<DropdownQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						options={questionData.typeSpecificData?.options || []}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			case QuestionType.Scale:
				return (
					<ScaleQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						minValue={questionData.typeSpecificData?.minValue || 1}
						maxValue={questionData.typeSpecificData?.maxValue || 5}
						minLabel={questionData.typeSpecificData?.minLabel}
						maxLabel={questionData.typeSpecificData?.maxLabel}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			case QuestionType.Rating:
				return (
					<RatingQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						maxRating={questionData.typeSpecificData?.maxRating || 5}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			case QuestionType.Date:
				return (
					<DateQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						minDate={questionData.typeSpecificData?.startDate}
						maxDate={questionData.typeSpecificData?.pastDate}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			case QuestionType.Time:
				return (
					<TimeQuestion
						key={question.id}
						questionId={question.id}
						title={questionData.title}
						description={questionData.description}
						isRequired={question.isRequired}
						minTime={questionData.typeSpecificData?.startTime}
						maxTime={questionData.typeSpecificData?.endTime}
						value={getAnswerValue(question.id)}
						onChange={(value) => updateAnswer(question.id, value)}
						mode={mode}
						hasError={hasError}
						errorMessage={errorMessage}
					/>
				);

			default:
				return (
					<div key={question.id} className={baseClasses}>
						<div className="text-textMuted">
							{t("unsupportedQuestionType", "Unsupported question type")}:{" "}
							{questionData.type}
						</div>
					</div>
				);
		}
	};

	if (!template.questions || template.questions.length === 0) {
		return (
			<div className="bg-surface border border-border rounded-lg p-6 text-center">
				<p className="text-textMuted">
					{t("noQuestionsInTemplate", "This template has no questions")}
				</p>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			{template.questions.sort((a, b) => a.order - b.order).map(renderQuestion)}

			{(mode === "fillable" || mode === "preview") && (
				<div className="bg-surface border border-border rounded-lg p-6">
					<div className="flex items-center justify-between">
						<div className="flex items-center gap-3">
							<label className="flex items-center gap-2 cursor-pointer">
								<input
									type="checkbox"
									checked={sendCopyToEmail}
									onChange={(e) => setSendCopyToEmail(e.target.checked)}
									className="w-4 h-4 text-primary"
								/>
								<span className="text-text">
									{t("sendCopyToEmail", "Send copy to my email")}
								</span>
							</label>
						</div>

						<button
							onClick={mode === "preview" ? undefined : handleSubmit}
							disabled={isSubmitting || mode === "preview"}
							className="px-6 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50 flex items-center gap-2"
						>
							{isSubmitting && (
								<svg
									className="animate-spin w-4 h-4"
									fill="none"
									viewBox="0 0 24 24"
								>
									<circle
										cx="12"
										cy="12"
										r="10"
										stroke="currentColor"
										strokeWidth="4"
										className="opacity-25"
									/>
									<path
										fill="currentColor"
										d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
										className="opacity-75"
									/>
								</svg>
							)}
							{isSubmitting
								? t("submitting", "Submitting...")
								: t("submit", "Submit")}
						</button>
					</div>
				</div>
			)}

			{mode === "readonly" && existingForm && (
				<div className="bg-surface border border-border rounded-lg p-4">
					<div className="text-sm text-textMuted">
						{t("submittedOn", "Submitted on")}:{" "}
						{new Date(existingForm.submittedAt).toLocaleString()}
					</div>
				</div>
			)}
		</div>
	);
};
