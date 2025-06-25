import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { TemplateDto } from "../../shared/api_types";
import { AppLoader } from "../../components/AppLoader";
import { TemplateCreatorPage } from "./TemplateCreatorPage";
import { TemplateEditorPage } from "./TemplateEditorPage";
import { templateApi } from "../../api/templateApi";
import { FormRenderer } from "../../modules/forms/components/FormRenderer";
import { TemplateHeader } from "../../modules/templates/components/editorPageTabs/TemplateHeader";
import { LikeButton } from "../../ui/Button/LikeButton";
import { CommentsSection } from "../../components/CommentsSection";

export const TemplatePage: React.FC = () => {
	const { id, sourceId } = useParams<{ id?: string; sourceId?: string }>();
	const { t } = useTranslation();
	const navigate = useNavigate();
	const { isAuthenticated, user, accessToken, isAdmin, isModerator } =
		useAuth();

	const [template, setTemplate] = useState<TemplateDto | null>(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		if (id) {
			fetchTemplate(id);
		} else if (sourceId) {
			fetchTemplate(sourceId);
		} else {
			if (!isAuthenticated) {
				setError("Access denied. Please log in to create templates.");
				setLoading(false);
				return;
			}
			setLoading(false);
		}
	}, [id, isAuthenticated, accessToken]);

	const fetchTemplate = async (templateId: string) => {
		try {
			setLoading(true);
			setError(null);

			if (accessToken) {
				const response = await templateApi.getTemplate(templateId, accessToken);
				console.log(response);
				setTemplate(response);
			}
		} catch (err: any) {
			setError(err.response?.data?.message || "Failed to load template");
		} finally {
			setLoading(false);
		}
	};

	const isOwnerOrModerator = () => {
		if (!user || !template) return false;
		return template.authorId === user.id || isAdmin || isModerator;
	};

	const canEdit = () => {
		if (!template) return false;
		return isOwnerOrModerator() && template.canUserEdit;
	};

	const canViewAsEditor = () => {
		if (!id) return true;
		return canEdit();
	};

	const canViewAsReader = () => {
		if (!id || !template) return false;
		return !canEdit();
	};

	const handleFillForm = (templateid: string) => {
		navigate(`/form/${templateid}`);
	};

	const handleSubmitComment = () => {};

	const handleLikeToggle = () => {
		console.log("Toogle like");
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
					<p className="text-textMuted">{error}</p>
				</div>
			</div>
		);
	}

	if (sourceId && template) {
		return <TemplateCreatorPage sourceTemplate={template} />;
	}

	if (!id) {
		return <TemplateCreatorPage />;
	}

	if (!template) {
		return (
			<div className="flex items-center justify-center py-16">
				<div className="text-center">
					<h1 className="text-2xl font-bold text-text mb-4">
						{t("templateNotFound", "Template not found")}
					</h1>
				</div>
			</div>
		);
	}

	if (canViewAsEditor()) {
		return (
			<>
				<TemplateEditorPage template={template} />;
				<CommentsSection
					commentsCount={template.commentsCount}
					isAuthenticated={isAuthenticated}
					onSubmitComment={handleSubmitComment}
				/>
			</>
		);
	}

	if (canViewAsReader()) {
		return (
			<div className="space-y-6">
				<div className="bg-surface border border-border rounded-lg p-6">
					<div className="flex flex-col sm:flex-row gap-4">
						<button
							onClick={() => handleFillForm(template.id)}
							disabled={!isAuthenticated}
							className="flex-1 px-6 py-3 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50 disabled:cursor-not-allowed font-medium"
						>
							{t("fillTheForm", "Fill the Form")}
						</button>
					</div>
				</div>

				<div className="bg-surface border border-border rounded-lg p-6">
					<h2 className="text-xl font-semibold text-text mb-4">
						{t("templatePreview", "Template Preview")}
					</h2>
					<TemplateHeader template={template}></TemplateHeader>
					<FormRenderer template={template} mode="readonly" />
					<div className="flex justify-between">
						<div></div>
						<div className="flex items-center gap-3">
							<LikeButton
								likesCount={template.likesCount}
								isUserLiked={template.isUserLiked}
								isAuthenticated={isAuthenticated}
								onLikeToggle={handleLikeToggle}
							/>
						</div>
					</div>
				</div>

				<CommentsSection
					commentsCount={template.commentsCount}
					isAuthenticated={isAuthenticated}
					onSubmitComment={handleSubmitComment}
				/>
			</div>
		);
	}

	return (
		<div className="flex items-center justify-center py-16">
			<div className="text-center">
				<h1 className="text-2xl font-bold text-text mb-4">
					{t("accessDenied", "Access Denied")}
				</h1>
				<p className="text-textMuted">
					{t("noAccessToTemplate", "You don't have access to this template")}
				</p>
			</div>
		</div>
	);
};
