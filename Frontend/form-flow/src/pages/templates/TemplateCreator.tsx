import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { TemplateEditor } from "../../pages/homePage/TemplateEditor";
import { FormTemplate } from "../../modules/templates/types/types";
import { QuestionType, TemplateAccess } from "../../shared/domain_types";
import {
	templateApi,
	CreateTemplateRequest,
	QuestionCreateRequest,
} from "../../api/templateApi";
import toast from "react-hot-toast";

export const TemplateCreator: React.FC = () => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const { accessToken } = useAuth();

	const [formTemplate, setFormTemplate] = useState<FormTemplate>({
		title: "",
		description: "",
		image: null,
		topicId: "",
		accessType: TemplateAccess.Public,
		tags: [],
		allowedUserIds: [],
		questions: [
			{
				id: "0",
				order: 0,
				title: "",
				description: "",
				type: QuestionType.ShortText,
				isRequired: true,
				showInResults: true,
				typeSpecificData: {
					maxLength: 100,
					placeholder: "",
				},
			},
			{
				id: "1",
				order: 1,
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
				order: 2,
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
				order: 3,
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
				order: 4,
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
				order: 5,
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
		],
	});

	const [publishImmediately, setPublishImmediately] = useState(false);
	const [isCreating, setIsCreating] = useState(false);

	const handleFormTemplateChange = (updatedTemplate: FormTemplate) => {
		setFormTemplate(updatedTemplate);
	};

	const validateTemplate = (): string | null => {
		if (!formTemplate.title.trim()) {
			return "Template title is required";
		}
		if (!formTemplate.topicId) {
			return "Please select a topic";
		}
		if (formTemplate.questions.length === 0) {
			return "At least one question is required";
		}
		return null;
	};

	const convertQuestionsToCreateRequests = (): QuestionCreateRequest[] => {
		return formTemplate.questions.map((question) => ({
			order: question.order,
			showInResults: question.showInResults,
			isRequired: question.isRequired,
			data: JSON.stringify({
				title: question.title,
				description: question.description,
				type: question.type,
				typeSpecificData: question.typeSpecificData,
			}),
		}));
	};

	const handleCreateTemplate = async () => {
		if (!accessToken) {
			toast.error("Authentication required");
			return;
		}

		const validationError = validateTemplate();
		if (validationError) {
			toast.error(validationError);
			return;
		}

		setIsCreating(true);

		try {
			if (!formTemplate.topicId) return;

			const createRequest: CreateTemplateRequest = {
				title: formTemplate.title,
				description: formTemplate.description,
				topicId: formTemplate.topicId,
				accessType: formTemplate.accessType,
				tags: formTemplate.tags,
				allowedUserIds: formTemplate.allowedUserIds,
				questions: convertQuestionsToCreateRequests(),
			};

			const createdTemplate = await templateApi.createTemplate(
				createRequest,
				accessToken
			);

			if (publishImmediately) {
				await templateApi.publishTemplate(createdTemplate.id, accessToken);
				toast.success("Template created and published successfully!");
			} else {
				toast.success("Template created successfully!");
			}

			navigate(`/template/${createdTemplate.id}`);
		} catch (error: any) {
			const errorMessage =
				error.response?.data?.message || "Failed to create template";
			toast.error(errorMessage);
		} finally {
			setIsCreating(false);
		}
	};

	const handleCancel = () => {
		navigate(-1);
	};

	return (
		<div className="min-h-screen bg-background">
			<div className="container mx-auto px-4 py-8">
				<div className="mb-8">
					<h1 className="text-3xl font-bold text-text mb-2">
						{t("createTemplate", "Create New Template")}
					</h1>
					<p className="text-textMuted">
						{t("createTemplateDescription", "Design your custom form template")}
					</p>
				</div>

				<TemplateEditor
					formTemplate={formTemplate}
					onFormTemplateChange={handleFormTemplateChange}
					mode="create"
				/>

				<div className="max-w-2xl mx-auto mt-8">
					<div className="bg-surface border border-border rounded-lg p-6">
						<div className="flex items-center justify-between">
							<div className="flex items-center gap-4">
								<label className="flex items-center gap-2 cursor-pointer">
									<input
										type="checkbox"
										checked={publishImmediately}
										onChange={(e) => setPublishImmediately(e.target.checked)}
										className="w-4 h-4 text-primary bg-background border-border rounded focus:ring-primary"
									/>
									<span className="text-text">
										{t("publishImmediately", "Publish immediately")}
									</span>
								</label>
								<span className="text-sm text-textMuted">
									{publishImmediately
										? t(
												"publishedTemplateNote",
												"Template will be visible to users immediately"
										  )
										: t("draftTemplateNote", "Template will be saved as draft")}
								</span>
							</div>
						</div>

						<div className="flex items-center justify-end gap-3 mt-6">
							<button
								onClick={handleCancel}
								disabled={isCreating}
								className="px-6 py-2 text-textMuted hover:text-text border border-border rounded-lg hover:bg-background transition-colors disabled:opacity-50"
							>
								{t("cancel", "Cancel")}
							</button>
							<button
								onClick={handleCreateTemplate}
								disabled={isCreating}
								className="px-6 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50 flex items-center gap-2"
							>
								{isCreating ? (
									<>
										<svg
											className="w-4 h-4 animate-spin"
											fill="none"
											stroke="currentColor"
											viewBox="0 0 24 24"
										>
											<path
												strokeLinecap="round"
												strokeLinejoin="round"
												strokeWidth={2}
												d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
											/>
										</svg>
										{t("creating", "Creating...")}
									</>
								) : (
									t("createTemplate", "Create Template")
								)}
							</button>
						</div>
					</div>
				</div>
			</div>
		</div>
	);
};
