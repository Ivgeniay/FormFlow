import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { TemplateEditor } from "./TemplateEditor";
import {
	FormTemplate,
	QuestionData,
} from "../../modules/templates/types/types";
import { QuestionType, TemplateAccess } from "../../shared/domain_types";
import {
	templateApi,
	CreateTemplateRequest,
	QuestionCreateRequest,
	CreateNewVersionRequest,
	AiTemplateDto,
} from "../../api/templateApi";
import toast from "react-hot-toast";
import { imageApi } from "../../api/imageApi";
import { TemplateDto } from "../../shared/api_types";
import {
	convertQuestionDtoToQuestionData,
	downloadImageFromUrl,
} from "../../modules/templates/components/editorPageTabs/QuestionsTab";
import { AiPromptForm } from "../../modules/templates/components/AiPromptForm";

interface TemplateCreatorProp {
	sourceTemplate?: TemplateDto;
}

export const TemplateCreatorPage: React.FC<TemplateCreatorProp> = ({
	sourceTemplate = null,
}) => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const { accessToken, user } = useAuth();

	const [formTemplate, setFormTemplate] = useState<FormTemplate>(() => {
		if (sourceTemplate) {
			const ft = {
				title: `${sourceTemplate.title}`,
				description: sourceTemplate.description,
				image: null,
				topicId: sourceTemplate.topicId || "",
				accessType: sourceTemplate.accessType,
				tags: sourceTemplate.tags.map((t) => t.name),
				allowedUserIds: sourceTemplate.allowedUsers.map((u) => u.id),
				questions: sourceTemplate.questions.map(
					convertQuestionDtoToQuestionData
				),
			};
			return ft;
		}
		return {
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
					isDeleted: false,
					type: QuestionType.ShortText,
					isRequired: true,
					showInResults: true,
					typeSpecificData: {
						maxLength: 100,
						placeholder: "",
					},
				},
			],
		};
	});

	useEffect(() => {
		if (sourceTemplate?.imageUrl) {
			downloadImageFromUrl(sourceTemplate.imageUrl).then((imageFile) => {
				setFormTemplate((prev) => ({ ...prev, image: imageFile }));
			});
		}
	}, [sourceTemplate]);

	const [publishImmediately, setPublishImmediately] = useState(false);
	const [isCreating, setIsCreating] = useState(false);

	const handleFormTemplateChange = (updatedTemplate: FormTemplate) => {
		setFormTemplate(updatedTemplate);
	};

	const handleAiGenerated = (aiTemplate: AiTemplateDto) => {
		const convertedQuestions: QuestionData[] = aiTemplate.questions.map(
			(aiQuestion, index) => {
				const aiData = aiQuestion.data;

				let typeSpecificData: Record<string, any> = {};

				switch (aiData.type) {
					case QuestionType.ShortText:
						typeSpecificData = {
							maxLength: aiData.maxLength || 100,
							placeholder: aiData.placeholder || "",
						};
						break;
					case QuestionType.LongText:
						typeSpecificData = {
							maxLength: aiData.maxLength || 500,
							placeholder: aiData.placeholder || "",
						};
						break;
					case QuestionType.SingleChoice:
					case QuestionType.Dropdown:
						typeSpecificData = {
							options: aiData.options || [],
						};
						break;
					case QuestionType.MultipleChoice:
						typeSpecificData = {
							options: aiData.options || [],
							maxSelections: aiData.maxSelections,
							minSelections: aiData.minSelections || 1,
						};
						break;
					case QuestionType.Scale:
						typeSpecificData = {
							minValue: aiData.minValue || 1,
							maxValue: aiData.maxValue || 5,
							minLabel: aiData.minLabel || "",
							maxLabel: aiData.maxLabel || "",
						};
						break;
					default:
						typeSpecificData = {};
				}

				return {
					id: `ai-${index}`,
					order: aiQuestion.order,
					title: aiData.title || "",
					description: aiData.description || "",
					isDeleted: false,
					type: aiData.type,
					isRequired: aiQuestion.isRequired,
					showInResults: aiQuestion.showInResults,
					typeSpecificData,
				};
			}
		);

		setFormTemplate((prev) => ({
			...prev,
			title: aiTemplate.title || "",
			description: aiTemplate.description || "",
			tags: aiTemplate.suggestedTags || [],
			questions: convertedQuestions,
		}));

		toast.success(t("aiTemplateApplied", "AI template applied successfully!"));
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
		if (!user) {
			toast.error("User undefined");
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

			let template: TemplateDto;

			if (sourceTemplate) {
				const createNewVersionRequest: CreateNewVersionRequest = {
					baseTemplateId: sourceTemplate.id,
					title: formTemplate.title,
					authorId: user?.id,
					description: formTemplate.description,
					topicId: formTemplate.topicId,
					accessType: formTemplate.accessType,
					tags: formTemplate.tags,
					allowedUserIds: formTemplate.allowedUserIds,
					questions: convertQuestionsToCreateRequests(),
				};

				template = await templateApi.createNewVersion(
					sourceTemplate.id,
					createNewVersionRequest,
					accessToken
				);
			} else {
				const createRequest: CreateTemplateRequest = {
					title: formTemplate.title,
					description: formTemplate.description,
					topicId: formTemplate.topicId,
					accessType: formTemplate.accessType,
					tags: formTemplate.tags,
					allowedUserIds: formTemplate.allowedUserIds,
					questions: convertQuestionsToCreateRequests(),
				};

				template = await templateApi.createTemplate(createRequest, accessToken);
			}

			if (formTemplate.image) {
				const response = await imageApi.uploadTemplateImage(
					template.id,
					formTemplate.image,
					accessToken
				);
			}

			if (publishImmediately) {
				await templateApi.publishTemplate(template.id, accessToken);
				toast.success("Template created and published successfully!");
			} else {
				toast.success("Template created successfully!");
			}

			navigate(`/template/${template.id}`);
		} catch (error: any) {
			const errorMessage =
				error.response?.data?.message || "Failed to create template";
			toast.error(errorMessage);
		} finally {
			setIsCreating(false);
		}
	};

	const handleCancel = () => {
		goBack();
	};

	const goBack = () => {
		if (window.history.length > 1) {
			navigate(-1);
		} else {
			navigate("/");
		}
	};

	return (
		<>
			<button
				onClick={goBack}
				className="inline-flex items-center gap-2 text-textMuted hover:text-text transition-colors mb-6"
			>
				<svg
					className="w-4 h-4"
					fill="none"
					stroke="currentColor"
					viewBox="0 0 24 24"
				>
					<path
						strokeLinecap="round"
						strokeLinejoin="round"
						strokeWidth={2}
						d="M15 19l-7-7 7-7"
					/>
				</svg>
				{t("back") || "Back"}
			</button>
			<div className="mb-8">
				<h1 className="text-3xl font-bold text-text mb-2">
					{t("createTemplate", "Create New Template")}
				</h1>
				<p className="text-textMuted">
					{t("createTemplateDescription", "Design your custom form template")}
				</p>
			</div>

			<AiPromptForm onGenerated={handleAiGenerated} accessToken={accessToken} />

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
		</>
	);
};
