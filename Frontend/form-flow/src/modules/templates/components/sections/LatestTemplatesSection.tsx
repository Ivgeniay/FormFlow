import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { TemplateGallery } from "../homepageComponents/TemplateGallery";
import { TemplateDto } from "../../../../shared/api_types";
import { templateApi } from "../../../../api/templateApi";

export const LatestTemplatesSection: React.FC = () => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const [templates, setTemplates] = useState<TemplateDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		loadLatestTemplates();
	}, []);

	const loadLatestTemplates = async () => {
		try {
			setLoading(true);
			setError(null);
			const latestTemplates = await templateApi.getLatestTemplates(6);
			setTemplates(latestTemplates);
		} catch (err: any) {
			setError(err.message || "Failed to load templates");
			console.error("Error loading latest templates:", err);
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
		console.log("View more latest templates");
	};

	if (loading) {
		return (
			<div className="space-y-4">
				<h2 className="text-2xl font-bold text-text">
					{t("latestTemplates", "Latest Templates")}
				</h2>
				<div className="text-textMuted">{t("loading", "Loading...")}</div>
			</div>
		);
	}

	if (error) {
		return (
			<div className="space-y-4">
				<h2 className="text-2xl font-bold text-text">
					{t("latestTemplates", "Latest Templates")}
				</h2>
				<div className="text-error">
					{t("errorLoadingTemplates", "Error loading templates")}: {error}
				</div>
			</div>
		);
	}

	return (
		<TemplateGallery
			templates={templates}
			title={t("latestTemplates", "Latest Templates") || "Latest Templates"}
			maxItems={6}
			mode="compact"
			columns={6}
			showViewMore={true}
			onViewMore={handleViewMore}
			onTemplateClick={handleTemplateClick}
			onTagClick={handleTagClick}
			onAuthorClick={handleAuthorClick}
		/>
	);
};
