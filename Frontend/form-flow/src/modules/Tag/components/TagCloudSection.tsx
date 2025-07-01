import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { TagCloud } from "../../Tag/components/TagCloud";
import { TagDto } from "../../../shared/api_types";
import { tagsApi } from "../../../api/tagsApi";

export const TagCloudSection: React.FC = () => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const [tags, setTags] = useState<TagDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		loadPopularTags();
	}, []);

	const loadPopularTags = async () => {
		try {
			setLoading(true);
			setError(null);
			const popularTags = await tagsApi.getTagCloud(30);
			console.log(popularTags);
			setTags(
				popularTags.tags.map((e) => ({
					id: e.id,
					name: e.name,
					usageCount: e.usageCount,
					createdAt: e.createdAt,
				}))
			);
		} catch (err: any) {
			setError(err.message || "Failed to load tags");
			console.error("Error loading tags:", err);
		} finally {
			setLoading(false);
		}
	};

	const handleTagClick = (tagName: string) => {
		navigate(`/search?tags=${encodeURIComponent(tagName)}`);
	};

	if (loading) {
		return (
			<div className="space-y-4">
				<h2 className="text-2xl font-bold text-text">
					{t("popularTags", "Popular Tags")}
				</h2>
				<div className="text-textMuted">{t("loading", "Loading...")}</div>
			</div>
		);
	}

	if (error) {
		return (
			<div className="space-y-4">
				<h2 className="text-2xl font-bold text-text">
					{t("popularTags", "Popular Tags")}
				</h2>
				<div className="text-error">
					{t("errorLoadingTags", "Error loading tags")}: {error}
				</div>
			</div>
		);
	}

	if (tags.length === 0) {
		return (
			<div className="space-y-4">
				<h2 className="text-2xl font-bold text-text">
					{t("popularTags", "Popular Tags")}
				</h2>
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
								d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"
							/>
						</svg>
						<p className="text-lg font-medium text-text mb-2">
							{t("noTagsYet", "No tags yet")}
						</p>
						<p className="text-sm">
							{t("tagsWillAppear", "Tags will appear as templates are created")}
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
					{t("popularTags", "Popular Tags")}
				</h2>
				<button
					onClick={() => navigate("/search")}
					className="text-primary hover:text-primary/80 text-sm font-medium transition-colors"
				>
					{t("viewAllTags", "View all tags")}
				</button>
			</div>

			<TagCloud
				tags={tags}
				onTagClick={handleTagClick}
				isExpandedDefault={true}
			/>

			<div className="text-center">
				<p className="text-sm text-textMuted">
					{t(
						"tagCloudDescription",
						"Click on any tag to search for related templates"
					)}
				</p>
			</div>
		</div>
	);
};
