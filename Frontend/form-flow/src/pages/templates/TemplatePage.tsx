import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { TemplateDto } from "../../shared/api_types";
import { AppLoader } from "../../components/AppLoader";
import { TemplateCreatorPage } from "./TemplateCreatorPage";
import axios from "axios";
import { ENV } from "../../config/env";

export const TemplatePage: React.FC = () => {
	const { id } = useParams<{ id?: string }>();
	const { t } = useTranslation();
	const { isAuthenticated, user, accessToken, isAdmin, isModerator } =
		useAuth();

	const [template, setTemplate] = useState<TemplateDto | null>(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		if (id) {
			fetchTemplate(id);
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

			const headers: any = {};
			if (accessToken) {
				headers.Authorization = `Bearer ${accessToken}`;
			}

			const response = await axios.get<TemplateDto>(
				`${ENV.API_URL}/template/${templateId}`,
				{ headers }
			);

			setTemplate(response.data);
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
			<div className="space-y-6">
				<div className="flex items-center justify-between">
					<div>
						<h1 className="text-3xl font-bold text-text">{template.title}</h1>
						<p className="text-textMuted mt-2">
							{t("editTemplate", "Edit Template")} • {template.authorName}
						</p>
					</div>
					<div className="flex items-center gap-3">
						<span
							className={`px-3 py-1 rounded-full text-sm ${
								template.isPublished
									? "bg-success/10 text-success"
									: "bg-warning/10 text-warning"
							}`}
						>
							{template.isPublished
								? t("published", "Published")
								: t("draft", "Draft")}
						</span>
					</div>
				</div>

				<div className="bg-surface border border-border rounded-lg">
					<div className="border-b border-border px-6 py-4">
						<nav className="flex space-x-8">
							<button className="text-primary border-b-2 border-primary pb-2 font-medium">
								{t("questions", "Questions")}
							</button>
							<button className="text-textMuted hover:text-text pb-2">
								{t("responses", "Responses")} ({template.formsCount})
							</button>
							<button className="text-textMuted hover:text-text pb-2">
								{t("versions", "Versions")}
							</button>
							<button className="text-textMuted hover:text-text pb-2">
								{t("analytics", "Analytics")}
							</button>
						</nav>
					</div>

					<div className="p-6">
						<div className="text-textMuted">
							Template editor component will be here (TemplateEditor in edit
							mode)
							<div className="mt-4 pt-4 border-t border-border">
								<p className="text-xs text-textMuted">
									Template ID: {template.id}
								</p>
								<p className="text-xs text-textMuted">
									Author: {template.authorName}
								</p>
								<p className="text-xs text-textMuted">
									Published: {template.isPublished ? "Yes" : "No"}
								</p>
								<p className="text-xs text-textMuted">
									Can Edit: {template.canUserEdit ? "Yes" : "No"}
								</p>
							</div>
						</div>
					</div>
				</div>
			</div>
		);
	}

	if (canViewAsReader()) {
		return (
			<div className="space-y-6">
				<div className="bg-surface border border-border rounded-lg p-6">
					<h1 className="text-3xl font-bold text-text mb-4">
						{template.title}
					</h1>
					<p className="text-textMuted mb-4">{template.description}</p>
					<div className="flex items-center gap-4 text-sm text-textMuted">
						<span>By: {template.authorName}</span>
						<span>
							Created: {new Date(template.createdAt).toLocaleDateString()}
						</span>
						<span>Forms: {template.formsCount}</span>
					</div>
				</div>

				<div className="bg-surface border border-border rounded-lg p-6">
					<h2 className="text-xl font-semibold text-text mb-4">
						Template Preview
					</h2>
					<div className="text-textMuted">
						Template preview component will be here (read-only view of
						questions)
					</div>
				</div>

				<div className="bg-surface border border-border rounded-lg p-6">
					<div className="flex items-center justify-between mb-4">
						<h2 className="text-xl font-semibold text-text">
							Likes ({template.likesCount})
						</h2>
						{isAuthenticated && (
							<div className="flex items-center gap-2">
								<button className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity">
									{template.isUserLiked ? "Unlike" : "Like"}
								</button>
								<span className="text-sm text-textMuted">
									Fill form:{" "}
									<a
										href={`/form/${template.id}`}
										className="text-primary hover:underline"
									>
										Click here
									</a>
								</span>
							</div>
						)}
					</div>
					{!isAuthenticated && (
						<p className="text-textMuted">
							<a href="/login" className="text-primary hover:underline">
								Log in
							</a>{" "}
							to like this template or{" "}
							<a
								href={`/form/${template.id}`}
								className="text-primary hover:underline"
							>
								fill the form
							</a>
						</p>
					)}
				</div>

				<div className="bg-surface border border-border rounded-lg p-6">
					<h2 className="text-xl font-semibold text-text mb-4">
						Comments ({template.commentsCount})
					</h2>
					<div className="text-textMuted">Comments component will be here</div>
					{isAuthenticated && (
						<div className="mt-4 pt-4 border-t border-border">
							<div className="text-textMuted">
								Comment form will be here (for authenticated users)
							</div>
						</div>
					)}
					{!isAuthenticated && (
						<div className="mt-4 pt-4 border-t border-border">
							<p className="text-textMuted">
								<a href="/login" className="text-primary hover:underline">
									Log in
								</a>{" "}
								to leave a comment
							</p>
						</div>
					)}
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
