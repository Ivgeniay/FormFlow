import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { templateApi } from "../../api/templateApi";
import { TemplateDto } from "../../shared/api_types";
import { AppLoader } from "../../components/AppLoader";
import { FormRenderer } from "../../modules/forms/components/FormRenderer";
import toast from "react-hot-toast";
import { TemplateHeader } from "../../modules/templates/components/editorPageTabs/TemplateHeader";

export const PreviewFormPage: React.FC = () => {
	const { t } = useTranslation();
	const { templateId } = useParams<{ templateId: string }>();
	const navigate = useNavigate();
	const { accessToken, isAuthenticated, isAdmin } = useAuth();

	const [loading, setLoading] = useState(true);
	const [template, setTemplate] = useState<TemplateDto | null>(null);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		if (!isAuthenticated) {
			navigate("/login");
			return;
		}

		if (!templateId || !accessToken) {
			setError(t("invalidTemplateId", "Invalid template ID"));
			setLoading(false);
			return;
		}

		loadTemplate();
	}, [templateId, accessToken, isAuthenticated]);

	const loadTemplate = async () => {
		if (!templateId || !accessToken) return;

		try {
			setLoading(true);
			setError(null);

			const templateData = await templateApi.getTemplate(templateId);

			if (isAdmin || templateData.canUserEdit) {
				setTemplate(templateData);
				return;
			}

			setError(
				t("noAccessToPreview", "You don't have access to preview this template")
			);
			return;
		} catch (error: any) {
			const errorMessage =
				error.response?.data?.message ||
				t("failedToLoadTemplate", "Failed to load template");
			setError(errorMessage);
			toast.error(errorMessage);
		} finally {
			setLoading(false);
		}
	};

	const handleGoBack = () => {
		if (template) {
			navigate(`/template/${template.id}`);
		} else {
			navigate(-1);
		}
	};

	if (loading) return <AppLoader isVisible={true} />;

	if (error) {
		return (
			<div className="min-h-screen bg-background p-8">
				<div className="max-w-2xl mx-auto text-center">
					<div className="text-error text-6xl mb-4">⚠️</div>
					<h1 className="text-2xl font-bold text-text mb-2">
						{t("error", "Error")}
					</h1>
					<p className="text-textMuted mb-6">{error}</p>
					<button
						onClick={handleGoBack}
						className="px-6 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
					>
						{t("goBack", "Go Back")}
					</button>
				</div>
			</div>
		);
	}

	if (!template) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center p-4">
				<div className="max-w-md w-full bg-surface border border-border rounded-lg p-6 text-center">
					<h1 className="text-xl font-semibold text-text mb-2">
						{t("noDataAvailable", "No data available")}
					</h1>
					<button
						onClick={handleGoBack}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
					>
						{t("goBack", "Go Back")}
					</button>
				</div>
			</div>
		);
	}

	return (
		<div className="min-h-screen bg-background p-8">
			<div className="max-w-2xl mx-auto">
				<div className="bg-warning/10 border border-warning/30 rounded-lg p-4 mb-6">
					<div className="flex items-center gap-3">
						<div className="text-warning text-xl">👁️</div>
						<div>
							<h3 className="font-medium text-warning">
								{t("previewMode", "Preview Mode")}
							</h3>
							<p className="text-sm text-textMuted">
								{t(
									"previewModeDescription",
									"This is how your form will look to users. No data will be saved."
								)}
							</p>
						</div>
					</div>
				</div>

				<button
					onClick={handleGoBack}
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
					{t("backToTemplate", "Back to Template")}
				</button>

				<TemplateHeader template={template} />
				<FormRenderer template={template} mode="preview" />
			</div>
		</div>
	);
};
