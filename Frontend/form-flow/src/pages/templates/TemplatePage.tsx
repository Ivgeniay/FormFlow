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
import { useTemplateActivity } from "../../modules/auth/hooks/useTemplateActivity";
import { CommentDisposer } from "../../modules/comments/CommentDisposer";

export const TemplatePage: React.FC = () => {
	const { t } = useTranslation();
	const { id, sourceId } = useParams<{ id?: string; sourceId?: string }>();
	const navigate = useNavigate();
	const { isAuthenticated, user, accessToken, isAdmin, isModerator } =
		useAuth();
	const [template, setTemplate] = useState<TemplateDto | null>(null);
	const [activityState, activityActions] = useTemplateActivity(template?.id);
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
	}, [id, isAuthenticated, accessToken, sourceId]);

	const fetchTemplate = async (templateId: string) => {
		try {
			setLoading(true);
			setError(null);

			const response = await templateApi.getTemplate(templateId, accessToken);
			setTemplate(response);
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
		navigate(`/form/from/${templateid}`);
	};

	const handleCloneTemplate = (templateid: string) => {
		navigate(`/template/from/${templateid}`);
	};

	const handleLikeToggle = () => {
		activityActions.toggleLike();
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
		return (
			<>
				<TemplateCreatorPage sourceTemplate={template} />;
			</>
		);
	}

	if (!id) {
		return (
			<>
				<TemplateCreatorPage />
			</>
		);
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
			<div className="space-y-6">
				<div className="bg-surface border border-border rounded-lg p-6">
					<TemplateEditorPage className="mb-6" template={template} />
				</div>

				<div className="bg-surface border border-border rounded-lg">
					<CommentDisposer
						comments={activityState.comments}
						initialCommentCounts={template.commentsCount}
						onAddComment={activityActions.addComment}
						onLoadMore={activityActions.loadComments}
						isLoading={activityState.isLoading}
						isLoadingMoreComments={activityState.isLoadingMore}
						isSubmitting={activityState.isSubmitting}
						hasMore={activityState.hasMoreComments}
						error={activityState.error}
						canComment={isAuthenticated}
						className="h-96"
					/>
				</div>
			</div>
		);
	}

	if (canViewAsReader()) {
		return (
			<div className="space-y-6">
				<div className="max-w-2xl mx-auto mt-8">
					<div className="bg-surface border border-border rounded-lg p-6 mb-6">
						<div className="flex flex-col sm:flex-row gap-4 mb-2">
							<button
								onClick={() => handleFillForm(template.id)}
								disabled={!isAuthenticated}
								className="flex-1 px-6 py-3 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50 disabled:cursor-not-allowed font-medium"
							>
								{t("fillTheForm", "Fill the Form")}
							</button>
						</div>
						<div className="flex flex-col sm:flex-row gap-4">
							<button
								onClick={() => handleCloneTemplate(template.id)}
								disabled={!isAuthenticated}
								className="flex-1 px-6 py-3 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50 disabled:cursor-not-allowed font-medium"
							>
								{t("cloneTemplate", "Clone template")}
							</button>
						</div>
					</div>

					<div className="bg-surface border border-border rounded-lg p-6 mb-6">
						<h2 className="text-xl font-semibold text-text mb-4">
							{t("templatePreview", "Template Preview")}
						</h2>
						<TemplateHeader template={template}></TemplateHeader>
						<FormRenderer template={template} mode="readonly" />
						<div className="flex justify-between pt-4">
							<div></div>
							<div className="flex items-center gap-3">
								<LikeButton
									likesCount={activityState.likesCount}
									isUserLiked={activityState.isUserLiked}
									isAuthenticated={isAuthenticated}
									onLikeToggle={handleLikeToggle}
								/>
							</div>
						</div>
					</div>

					<div className="bg-surface border border-border rounded-lg">
						<CommentDisposer
							comments={activityState.comments}
							initialCommentCounts={template.commentsCount}
							onAddComment={activityActions.addComment}
							onLoadMore={activityActions.loadComments}
							isLoading={activityState.isLoading}
							isLoadingMoreComments={activityState.isLoadingMore}
							isSubmitting={activityState.isSubmitting}
							hasMore={activityState.hasMoreComments}
							error={activityState.error}
							canComment={isAuthenticated}
							className="h-96"
						/>
					</div>
				</div>
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
