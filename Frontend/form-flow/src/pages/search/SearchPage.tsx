import React, { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { TemplateGallery } from "../../modules/templates/components/homepageComponents/TemplateGallery";
import { TemplateDto } from "../../shared/api_types";
import { searchApi, SearchSortBy, SearchRequest } from "../../api/searchApi";
import { AppLoader } from "../../components/AppLoader";
import { TemplateTable } from "../../modules/templates/components/homepageComponents/TemplateTable";

export const SearchPage: React.FC = () => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const { isAdmin, accessToken } = useAuth();
	const [searchParams, setSearchParams] = useSearchParams();

	const [templates, setTemplates] = useState<TemplateDto[]>([]);
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);
	const [pagination, setPagination] = useState<any>(null);
	const [searchInfo, setSearchInfo] = useState<any>(null);

	const [searchQuery, setSearchQuery] = useState(searchParams.get("q") || "");
	const [selectedTags, setSelectedTags] = useState<string[]>(
		searchParams.getAll("tags") || []
	);
	const [selectedAuthor, setSelectedAuthor] = useState(
		searchParams.get("author") || ""
	);
	const [selectedTopic, setSelectedTopic] = useState(
		searchParams.get("topic") || ""
	);
	const [sortBy, setSortBy] = useState<SearchSortBy>(
		parseInt(searchParams.get("sortBy") || "0") as SearchSortBy
	);
	const [includeArchived, setIncludeArchived] = useState(
		searchParams.get("includeArchived") === "true"
	);

	useEffect(() => {
		performSearch();
	}, [searchParams]);

	const performSearch = async () => {
		setLoading(true);
		setError(null);

		try {
			const request: SearchRequest = {
				q: searchParams.get("q") || "",
				tags: searchParams.getAll("tags"),
				author: searchParams.get("author") || undefined,
				topic: searchParams.get("topic") || undefined,
				sortBy: parseInt(searchParams.get("sortBy") || "0") as SearchSortBy,
				includeArchived: searchParams.get("includeArchived") === "true",
				page: parseInt(searchParams.get("page") || "1"),
				pageSize: parseInt(searchParams.get("pageSize") || "20"),
			};

			console.log(request);

			const response =
				isAdmin && accessToken
					? await searchApi.adminSearch(request, accessToken)
					: await searchApi.searchTemplates(request);

			setTemplates(response.templates);
			setPagination(response.pagination);
			setSearchInfo(response.searchInfo);
		} catch (err: any) {
			setError(err.message || "Search failed");
			console.error("Search error:", err);
		} finally {
			setLoading(false);
		}
	};

	const updateSearchParams = (
		updates: Record<string, string | string[] | null>
	) => {
		const newParams = new URLSearchParams(searchParams);

		Object.entries(updates).forEach(([key, value]) => {
			if (
				value === null ||
				value === "" ||
				(Array.isArray(value) && value.length === 0)
			) {
				newParams.delete(key);
			} else if (Array.isArray(value)) {
				newParams.delete(key);
				value.forEach((v) => newParams.append(key, v));
			} else {
				newParams.set(key, value);
			}
		});

		newParams.delete("page");
		setSearchParams(newParams);
	};

	const handleSearch = () => {
		updateSearchParams({
			q: searchQuery,
			tags: selectedTags,
			author: selectedAuthor,
			topic: selectedTopic,
			sortBy: sortBy.toString(),
			includeArchived: includeArchived.toString(),
		});
	};

	const handleTemplateClick = (template: TemplateDto) => {
		navigate(`/template/${template.id}`);
	};

	const handleTagClick = (tagName: string) => {
		if (!selectedTags.includes(tagName)) {
			setSelectedTags([...selectedTags, tagName]);
		}
	};

	const handleAuthorClick = (authorId: string) => {
		console.log("Author clicked:", authorId);
	};

	const removeTag = (tagToRemove: string) => {
		setSelectedTags(selectedTags.filter((tag) => tag !== tagToRemove));
	};

	const clearFilters = () => {
		setSearchQuery("");
		setSelectedTags([]);
		setSelectedAuthor("");
		setSelectedTopic("");
		setSortBy(SearchSortBy.Relevance);
		setIncludeArchived(false);
		setSearchParams({});
	};

	const handlePageChange = (page: number) => {
		const newParams = new URLSearchParams(searchParams);
		newParams.set("page", page.toString());
		setSearchParams(newParams);
	};

	if (loading && templates.length === 0) {
		return <AppLoader isVisible={true} />;
	}

	return (
		<div className="space-y-6">
			<div className="bg-surface border border-border rounded-lg p-6">
				<div className="flex items-center justify-between mb-4">
					<h1 className="text-2xl font-bold text-text">
						{t("search", "Search")}
					</h1>
					{(searchQuery ||
						selectedTags.length > 0 ||
						selectedAuthor ||
						selectedTopic) && (
						<button
							onClick={clearFilters}
							className="text-textMuted hover:text-text text-sm"
						>
							{t("clearFilters", "Clear filters")}
						</button>
					)}
				</div>

				<div className="space-y-4">
					<div className="flex gap-4">
						<div className="flex-1">
							<input
								type="text"
								value={searchQuery}
								onChange={(e) => setSearchQuery(e.target.value)}
								onKeyDown={(e) => e.key === "Enter" && handleSearch()}
								placeholder={
									t("searchPlaceholder", "Search templates...") ||
									"Search templates..."
								}
								className="w-full px-4 py-2 border border-border rounded-lg bg-background text-text focus:border-primary focus:outline-none"
							/>
						</div>
						<button
							onClick={handleSearch}
							disabled={loading}
							className="px-6 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50"
						>
							{loading ? t("searching", "Searching...") : t("search", "Search")}
						</button>
					</div>

					{selectedTags.length > 0 && (
						<div className="flex flex-wrap gap-2">
							<span className="text-sm text-textMuted">
								{t("tags", "Tags")}:
							</span>
							{selectedTags.map((tag) => (
								<span
									key={tag}
									className="inline-flex items-center gap-1 px-2 py-1 bg-primary text-white text-sm rounded-full"
								>
									{tag}
									<button
										onClick={() => removeTag(tag)}
										className="text-white hover:text-error"
									>
										×
									</button>
								</span>
							))}
						</div>
					)}

					<div className="flex flex-wrap gap-4">
						<div>
							<label className="block text-sm font-medium text-text mb-1">
								{t("sortBy", "Sort by")}
							</label>
							<select
								value={sortBy}
								onChange={(e) =>
									setSortBy(parseInt(e.target.value) as SearchSortBy)
								}
								className="px-3 py-2 border border-border rounded-md bg-background text-text focus:border-primary focus:outline-none"
							>
								<option value={SearchSortBy.Relevance}>
									{t("relevance", "Relevance")}
								</option>
								<option value={SearchSortBy.Date}>{t("date", "Date")}</option>
								<option value={SearchSortBy.Popularity}>
									{t("popularity", "Popularity")}
								</option>
								<option value={SearchSortBy.Title}>
									{t("title", "Title")}
								</option>
							</select>
						</div>

						{isAdmin && (
							<div>
								<label className="flex items-center gap-2 mt-6">
									<input
										type="checkbox"
										checked={includeArchived}
										onChange={(e) => setIncludeArchived(e.target.checked)}
										className="text-primary focus:ring-primary"
									/>
									<span className="text-sm text-text">
										{t("includeArchived", "Include archived")}
									</span>
								</label>
							</div>
						)}
					</div>
				</div>
			</div>

			{error && (
				<div className="bg-error/10 border border-error text-error rounded-lg p-4">
					{error}
				</div>
			)}

			{searchInfo && (
				<div className="flex items-center justify-between text-sm text-textMuted">
					<span>
						{t("searchResults", "Found {{count}} templates", {
							count: pagination?.totalCount || 0,
						})}
						{searchInfo.searchTime && (
							<> ({(searchInfo.searchTime / 1000).toFixed(2)}s)</>
						)}
					</span>
					{pagination && pagination.totalPages > 1 && (
						<span>
							{t("page", "Page")} {pagination.currentPage} {t("of", "of")}{" "}
							{pagination.totalPages}
						</span>
					)}
				</div>
			)}

			{/* <TemplateGallery
				templates={templates}
				mode="detailed"
				columns={2}
				showViewMore={false}
				onTemplateClick={handleTemplateClick}
				onTagClick={handleTagClick}
				onAuthorClick={handleAuthorClick}
			/> */}
			<TemplateTable
				templates={templates}
				onTemplateClick={handleTemplateClick}
				onTagClick={handleTagClick}
				onAuthorClick={handleAuthorClick}
			/>

			{pagination && pagination.totalPages > 1 && (
				<div className="flex justify-center gap-2">
					<button
						onClick={() => handlePageChange(pagination.currentPage - 1)}
						disabled={!pagination.hasPrevious}
						className="px-4 py-2 border border-border rounded-lg disabled:opacity-50 hover:bg-surface transition-colors"
					>
						{t("previous", "Previous")}
					</button>

					<span className="px-4 py-2 text-textMuted">
						{pagination.currentPage} / {pagination.totalPages}
					</span>

					<button
						onClick={() => handlePageChange(pagination.currentPage + 1)}
						disabled={!pagination.hasNext}
						className="px-4 py-2 border border-border rounded-lg disabled:opacity-50 hover:bg-surface transition-colors"
					>
						{t("next", "Next")}
					</button>
				</div>
			)}
		</div>
	);
};
