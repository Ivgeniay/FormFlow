import React, { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { TemplateDto } from "../../shared/api_types";
import { searchApi, SearchSortBy, SearchRequest } from "../../api/searchApi";
import { AppLoader } from "../../components/AppLoader";
import {
	SortableTable,
	TableColumn,
} from "../../modules/templates/components/editorPageTabs/SortableTable";
import {
	ActionItem,
	ActionPanel,
} from "../../modules/templates/components/editorPageTabs/ActionPanel";

export const SearchPage: React.FC = () => {
	const { t } = useTranslation();
	const navigate = useNavigate();
	const { isAdmin, accessToken } = useAuth();
	const [searchParams, setSearchParams] = useSearchParams();
	const [hoveredRowId, setHoveredRowId] = useState<string | null>(null);

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

			const response =
				isAdmin && accessToken
					? await searchApi.adminSearch(request, accessToken)
					: await searchApi.searchTemplates(request);

			console.log(response);

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

	const tableColumns: TableColumn<TemplateDto>[] = [
		{
			key: "title",
			label: t("title", "Title"),
			sortable: true,
			render: (template: TemplateDto) => (
				<div className="flex items-center gap-2">
					{template.imageUrl && (
						<img
							src={template.imageUrl}
							alt={template.title}
							className="w-8 h-8 rounded object-cover"
						/>
					)}
					<div>
						<div className="font-medium text-text">{template.title}</div>
						<div className="text-sm text-textMuted truncate max-w-xs">
							{template.description}
						</div>
					</div>
				</div>
			),
		},
		{
			key: "authorName",
			label: t("author", "Author"),
			sortable: true,
			render: (template: TemplateDto) => (
				<span className="text-text">{template.authorName || "Unknown"}</span>
			),
		},
		{
			key: "topic",
			label: t("topic", "Topic"),
			sortable: true,
			render: (template: TemplateDto) => (
				<span className="px-2 py-1 bg-background rounded text-sm">
					{template.topic || t("noTopic", "No topic")}
				</span>
			),
		},
		{
			key: "tags",
			label: t("tags", "Tags"),
			sortable: false,
			render: (template: TemplateDto) => (
				<div className="flex flex-wrap gap-1">
					{template.tags.slice(0, 3).map((tag) => (
						<span
							key={tag.name}
							onClick={() => handleTagClick(tag.name)}
							className="px-2 py-1 bg-primary/10 text-primary text-xs rounded cursor-pointer hover:bg-primary/20"
						>
							{tag.name}
						</span>
					))}
					{template.tags.length > 3 && (
						<span className="text-textMuted text-xs">
							+{template.tags.length - 3}
						</span>
					)}
				</div>
			),
		},
		{
			key: "formsCount",
			label: t("responses", "Responses"),
			sortable: true,
			render: (template: TemplateDto) => (
				<span className="text-text">{template.formsCount}</span>
			),
		},
		{
			key: "likesCount",
			label: t("likes", "Likes"),
			sortable: true,
			render: (template: TemplateDto) => (
				<span className="text-text">{template.likesCount}</span>
			),
		},
		{
			key: "createdAt",
			label: t("created", "Created"),
			sortable: true,
			render: (template: TemplateDto) => (
				<span className="text-textMuted text-sm">
					{new Date(template.createdAt).toLocaleDateString()}
				</span>
			),
		},
	];
	const getTemplateActions = (template: TemplateDto): ActionItem[] => [
		{
			id: "view",
			icon: "👁️",
			label: t("viewTemplate", "View Template"),
			onClick: () => handleTemplateClick(template),
		},
		{
			id: "author",
			icon: "👤",
			label: t("viewAuthor", "View Author"),
			onClick: () => {
				handleAuthorClick(template.authorId);
			},
		},
	];

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
		navigate(`/profile/${authorId}`);
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
				</div>
			)}

			{templates.length > 0 ? (
				<SortableTable
					data={templates}
					columns={tableColumns}
					onRowHover={setHoveredRowId}
					emptyMessage={t("noResults", "No found") || "No templates found"}
					renderRow={(template) => (
						<>
							<td className="absolute inset-0 pointer-events-none">
								<ActionPanel
									visible={hoveredRowId === template.id}
									actions={getTemplateActions(template)}
									position="right"
									className="pointer-events-auto"
								/>
							</td>
						</>
					)}
					getItemId={(template) => template.id}
				/>
			) : (
				!loading && (
					<div className="text-center py-8 text-textMuted">
						<p>{t("noResults", "No templates found")}</p>
					</div>
				)
			)}

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
