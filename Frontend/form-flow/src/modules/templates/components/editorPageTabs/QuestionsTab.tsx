import { forwardRef, useEffect, useImperativeHandle, useState } from "react";
import { TemplateEditor } from "../../../../pages/templates/TemplateEditor";
import { QuestionDto, TemplateDto } from "../../../../shared/api_types";
import {
	UpdateQuestionDto,
	templateApi,
	UpdateTemplateRequest,
} from "../../../../api/templateApi";
import toast from "react-hot-toast";
import { FormTemplate, QuestionData } from "../../types/types";
import { QuestionType } from "../../../../shared/domain_types";
import { useTranslation } from "react-i18next";
import { imageApi } from "../../../../api/imageApi";

interface TabProps {
	template: TemplateDto;
	accessToken: string | null;
	hasChangeCallback?: (hasChhange: boolean) => void;
}

export interface QuestionsTabRef {
	save: () => Promise<void>;
}

export const convertQuestionDtoToQuestionData = (
	questionDto: QuestionDto
): QuestionData => {
	let parsedData;
	try {
		parsedData = JSON.parse(questionDto.data);
	} catch {
		parsedData = {
			title: "",
			description: "",
			type: QuestionType.ShortText,
			typeSpecificData: {},
		};
	}

	return {
		id: questionDto.id,
		order: questionDto.order,
		isDeleted: false,
		title: parsedData.title || "",
		description: parsedData.description || "",
		type: parsedData.type || QuestionType.ShortText,
		isRequired: questionDto.isRequired,
		showInResults: questionDto.showInResults,
		typeSpecificData: parsedData.typeSpecificData || {},
	};
};

export const downloadImageFromUrl = async (
	imageUrl?: string
): Promise<File | null> => {
	if (!imageUrl) return null;

	try {
		const file = await imageApi.getImageProxy(imageUrl);
		return file;
	} catch {
		return null;
	}
};

export const convertQuestionDataToUpdateRequest = (
	questionData: QuestionData
): UpdateQuestionDto => {
	try {
		const isNewQuestion =
			!/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(
				questionData.id
			);
		const result = {
			id: questionData.id,
			order: questionData.order,
			showInResults: questionData.showInResults,
			isNewQuestion: isNewQuestion,
			isDeleted: questionData.isDeleted,
			isRequired: questionData.isRequired,
			data: JSON.stringify({
				title: questionData.title,
				description: questionData.description,
				type: questionData.type,
				typeSpecificData: questionData.typeSpecificData,
			}),
		};
		return result;
	} catch (error) {
		console.error("Error converting question:", error, questionData);
		throw error;
	}
};

export function convertTemplateToFormTemplate(
	template: TemplateDto
): FormTemplate {
	return {
		title: template.title,
		description: template.description,
		image: null,
		topicId: template.topicId || "",
		accessType: template.accessType,
		tags: template.tags.map((tag) => tag.name),
		allowedUserIds: template.allowedUsers.map((user) => user.id),
		questions: template.questions
			.sort((a, b) => a.order - b.order)
			.map(convertQuestionDtoToQuestionData),
	};
}

export const QuestionsTab = forwardRef<QuestionsTabRef, TabProps>(
	({ template, accessToken, hasChangeCallback }, ref) => {
		const { t } = useTranslation();
		const [formTemplate, setFormTemplate] = useState<FormTemplate | null>(null);
		const [isSaving, setIsSaving] = useState(false);
		const [hasChanges, setHasChanges] = useState(false);
		const [originalImage, setOriginalImage] = useState<File | null>(null);

		const handleFormTemplateChange = (updatedTemplate: FormTemplate) => {
			setFormTemplate(updatedTemplate);
			setHasChanges(true);
			if (hasChangeCallback) hasChangeCallback(true);
		};

		useImperativeHandle(ref, () => ({
			save: handleSave,
		}));

		const handleSave = async () => {
			if (!accessToken || !hasChanges) return;
			if (formTemplate === null) return;

			setIsSaving(true);
			try {
				const updateRequest: UpdateTemplateRequest = {
					id: template.id,
					title: formTemplate.title,
					description: formTemplate.description,
					topicId: formTemplate.topicId || "",
					accessType: formTemplate.accessType,
					tags: formTemplate.tags,
					allowedUserIds: formTemplate.allowedUserIds,
					questions: formTemplate.questions.map(
						convertQuestionDataToUpdateRequest
					),
				};
				// console.log("About to call updateTemplate with:", updateRequest);
				await templateApi.updateTemplate(
					template.id,
					updateRequest,
					accessToken
				);

				if (formTemplate.image !== originalImage) {
					if (formTemplate.image) {
						await imageApi.uploadTemplateImage(
							template.id,
							formTemplate.image,
							accessToken
						);
					} else {
						await imageApi.deleteTemplateImage(template.id, accessToken);
					}
				}

				toast.success(t("questionsSaved", "Questions saved successfully"));
				setHasChanges(false);
				if (hasChangeCallback) hasChangeCallback(false);
			} catch (error: any) {
				console.error(error);
				toast.error(
					error.message || t("errorSavingQuestions", "Error saving questions")
				);
			} finally {
				setIsSaving(false);
			}
		};

		useEffect(() => {
			const initializeFormTemplate = async () => {
				const baseTemplate = convertTemplateToFormTemplate(template);
				const imageFile = await downloadImageFromUrl(template.imageUrl);
				setOriginalImage(imageFile);
				setFormTemplate({
					...baseTemplate,
					image: imageFile,
				});
			};

			initializeFormTemplate();
		}, [template]);

		if (!formTemplate) {
			return (
				<div className="text-center py-8">{t("loading", "Loading...")}</div>
			);
		}

		return (
			<div className="space-y-4">
				{hasChanges && (
					<div className="flex items-center justify-between bg-warning/10 border border-warning/20 rounded-lg p-4">
						<div className="flex items-center gap-2">
							<span className="text-warning font-medium">*</span>
							<span className="text-text">
								{t("unsavedChanges", "You have unsaved changes")}
							</span>
						</div>
						<button
							onClick={handleSave}
							disabled={isSaving}
							className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50"
						>
							{isSaving
								? t("saving", "Saving...")
								: t("saveChanges", "Save Changes")}
						</button>
					</div>
				)}

				<TemplateEditor
					formTemplate={formTemplate}
					onFormTemplateChange={handleFormTemplateChange}
					mode="edit"
					isDebug={false}
				/>
			</div>
		);
	}
);
