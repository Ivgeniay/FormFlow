import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { TemplateGallery } from "../homepageComponents/TemplateGallery";
import { TemplateDto } from "../../../../shared/api_types";
import { templateApi } from "../../../../api/templateApi";
import { useAuth } from "../../../auth/hooks/useAuth";

export const MyTemplatesSection: React.FC = () => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const { isAuthenticated, user, accessToken } = useAuth();
	const [templates, setTemplates] = useState<TemplateDto[]>([]);
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		if (isAuthenticated && user && accessToken) {
			loadMyTemplates();
		}
	}, [isAuthenticated, user, accessToken]);

	const loadMyTemplates = async () => {
		if (!user || !accessToken) return;

		try {
			setLoading(true);
			setError(null);
			const result = await templateApi.getUserTemplates(
				user.id,
				1,
				6,
				accessToken
			);
			setTemplates(result.data);
		} catch (err: any) {
			setError(err.message || "Failed to load your templates");
			console.error("Error loading user templates:", err);
		} finally {
			setLoading(false);
		}
	};

	const handleTemplateClick = (template: TemplateDto) => {
		navigate(`/template/${template.id}`);
	};

	const handleTagClick = (tagName: string) => {
		console.log("Tag clicked:", tagName);
	};

	const handleAuthorClick = (authorId: string) => {
		console.log("Author clicked:", authorId);
	};

	const handleViewMore = () => {
		console.log("View more my templates");
	};

	const handleCreateNew = () => {
		navigate("/template");
	};

	if (!isAuthenticated) {
		return null;
	}

	if (loading) {
		return (
			<div className="space-y-4">
				<div className="flex items-center justify-between">
					<h2 className="text-2xl font-bold text-text">
						{t("myTemplates", "My Templates")}
					</h2>
					<button
						onClick={handleCreateNew}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
					>
						{t("createTemplate", "Create Template")}
					</button>
				</div>
				<div className="text-textMuted">{t("loading", "Loading...")}</div>
			</div>
		);
	}

	if (error) {
		return (
			<div className="space-y-4">
				<div className="flex items-center justify-between">
					<h2 className="text-2xl font-bold text-text">
						{t("myTemplates", "My Templates")}
					</h2>
					<button
						onClick={handleCreateNew}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
					>
						{t("createTemplate", "Create Template")}
					</button>
				</div>
				<div className="text-error">
					{t("errorLoadingTemplates", "Error loading templates")}: {error}
				</div>
			</div>
		);
	}

	if (templates.length === 0) {
		return (
			<div className="space-y-4">
				<div className="flex items-center justify-between">
					<h2 className="text-2xl font-bold text-text">
						{t("myTemplates", "My Templates")}
					</h2>
					<button
						onClick={handleCreateNew}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
					>
						{t("createTemplate", "Create Template")}
					</button>
				</div>
				<div className="text-center py-8 bg-surface border border-border rounded-lg">
					<div className="text-textMuted">
						<svg
							className="w-12 h-12 mx-auto mb-4"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
							/>
						</svg>
						<p className="text-lg font-medium text-text mb-2">
							{t("noTemplatesYet", "No templates yet")}
						</p>
						<p className="text-sm">
							{t(
								"createFirstTemplate",
								"Create your first template to get started"
							)}
						</p>
					</div>
				</div>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<h2 className="text-2xl font-bold text-text">
					{t("myTemplates", "My Templates")}
				</h2>
				<button
					onClick={handleCreateNew}
					className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
				>
					{t("createTemplate", "Create Template")}
				</button>
			</div>

			<TemplateGallery
				templates={templates}
				maxItems={6}
				mode="compact"
				columns={6}
				showViewMore={templates.length >= 0}
				onViewMore={handleViewMore}
				onTemplateClick={handleTemplateClick}
				onTagClick={handleTagClick}
				onAuthorClick={handleAuthorClick}
			/>
		</div>
	);
};
