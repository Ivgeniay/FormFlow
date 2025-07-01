import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { formApi } from "../../api/formApi";
import { templateApi } from "../../api/templateApi";
import { FormDto, TemplateDto } from "../../shared/api_types";
import toast from "react-hot-toast";
import { AppLoader } from "../../components/AppLoader";
import { FormattedTextDisplay } from "../../ui/Input/FormattedTextDisplay";
import { FormRenderer } from "../../modules/forms/components/FormRenderer";

export const FormEditorPage: React.FC = () => {
	const { t } = useTranslation();
	const { formId } = useParams<{ formId: string }>();
	const navigate = useNavigate();
	const { accessToken, isAuthenticated } = useAuth();

	const [loading, setLoading] = useState(true);
	const [saving, setSaving] = useState(false);
	const [form, setForm] = useState<FormDto | null>(null);
	const [template, setTemplate] = useState<TemplateDto | null>(null);
	const [error, setError] = useState<string | null>(null);
	const [hasChanges, setHasChanges] = useState(false);
	const [answers, setAnswers] = useState<Record<string, any>>({});

	useEffect(() => {
		if (!isAuthenticated) {
			navigate("/login");
			return;
		}

		if (!formId || !accessToken) {
			setError(t("invalidFormId", "Invalid form ID"));
			setLoading(false);
			return;
		}

		loadForm();
	}, [formId, accessToken, isAuthenticated]);

	const loadForm = async () => {
		if (!formId || !accessToken) return;

		try {
			setLoading(true);
			setError(null);

			const formData = await formApi.getForm(formId, accessToken);

			if (!formData.canEdit) {
				setError(
					t("noEditPermission", "You don't have permission to edit this form")
				);
				setLoading(false);
				return;
			}

			setForm(formData);

			const templateData = await templateApi.getTemplate(
				formData.templateId,
				accessToken
			);
			setTemplate(templateData);

			const initialAnswers: Record<string, any> = {};
			formData.questions.forEach((question) => {
				initialAnswers[question.questionId] = question.answer;
			});
			setAnswers(initialAnswers);
		} catch (error: any) {
			const errorMessage =
				error.response?.data?.message ||
				t("failedToLoadForm", "Failed to load form");
			setError(errorMessage);
			toast.error(errorMessage);
		} finally {
			setLoading(false);
		}
	};

	const handleFormSubmit = async (
		submittedAnswers: Record<string, any>,
		sendCopyToEmail: boolean
	) => {
		if (!form || !accessToken || saving) return;

		try {
			setSaving(true);

			const updateRequest = {
				id: form.id,
				answers: submittedAnswers,
			};

			const updatedForm = await formApi.updateForm(updateRequest, accessToken);
			setForm(updatedForm);
			setHasChanges(false);
			toast.success(t("formSaved", "Form saved successfully"));
		} catch (error: any) {
			const errorMessage =
				error.response?.data?.message ||
				t("failedToSaveForm", "Failed to save form");
			toast.error(errorMessage);
		} finally {
			setSaving(false);
		}
	};

	const handleBack = () => {
		if (hasChanges) {
			const shouldDiscard = window.confirm(
				t(
					"unsavedChangesConfirm",
					"You have unsaved changes. Are you sure you want to leave?"
				) || "You have unsaved changes. Are you sure you want to leave?"
			);
			if (!shouldDiscard) return;
		}
		navigate(`/form/view/${formId}`);
	};

	if (loading) {
		return <AppLoader isVisible={true} />;
	}

	if (error) {
		return (
			<div className="flex items-center justify-center py-16">
				<div className="text-center">
					<h1 className="text-2xl font-bold text-error mb-4">
						{t("error", "Error")}
					</h1>
					<p className="text-textMuted mb-4">{error}</p>
					<button
						onClick={() => navigate("/")}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90"
					>
						{t("backToHome", "Back to Home")}
					</button>
				</div>
			</div>
		);
	}

	if (!form || !template) {
		return (
			<div className="text-center py-16">
				<p className="text-textMuted">{t("formNotFound", "Form not found")}</p>
			</div>
		);
	}

	return (
		<div className="max-w-4xl mx-auto">
			<div className="flex items-center gap-4 mb-6">
				<button
					onClick={handleBack}
					className="flex items-center gap-2 px-3 py-2 text-textMuted hover:text-text transition-colors"
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
					{t("back", "Back")}
				</button>

				<div className="flex-1">
					<h1 className="text-2xl font-bold text-text">
						{t("editForm", "Edit Form")}
					</h1>
				</div>
			</div>

			<div className="bg-surface border border-border rounded-lg p-6 mb-6">
				<div className="text-xl font-bold text-text mb-2">
					<FormattedTextDisplay value={form.templateName} />
				</div>
				<div className="flex items-center gap-4 text-sm text-textMuted">
					<span>
						{t("submittedBy", "Submitted by")}:{" "}
						<FormattedTextDisplay value={form.userName} />
					</span>
					<span>
						{t("submittedOn", "Submitted on")}:{" "}
						{new Date(form.submittedAt).toLocaleDateString()}
					</span>
					{form.updatedAt !== form.submittedAt && (
						<span>
							{t("lastUpdated", "Last updated")}:{" "}
							{new Date(form.updatedAt).toLocaleDateString()}
						</span>
					)}
				</div>
			</div>

			<FormRenderer
				template={template}
				mode="fillable"
				initialAnswers={answers}
				onSubmit={handleFormSubmit}
			/>

			{hasChanges && (
				<div className="fixed bottom-4 right-4 bg-warning/10 border border-warning/30 rounded-lg p-4 shadow-lg">
					<div className="flex items-center gap-2 text-warning text-sm">
						<svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
							<path
								fillRule="evenodd"
								d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
								clipRule="evenodd"
							/>
						</svg>
						{t("unsavedChanges", "You have unsaved changes")}
					</div>
				</div>
			)}
		</div>
	);
};
