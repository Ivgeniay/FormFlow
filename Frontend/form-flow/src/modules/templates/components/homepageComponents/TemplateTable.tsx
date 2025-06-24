import React from "react";
import { useTranslation } from "react-i18next";
import { TemplateDto } from "../../../../shared/api_types";

export interface TemplateTableProps {
	templates: TemplateDto[];
	onTemplateClick?: (template: TemplateDto) => void;
	onTagClick?: (tagName: string) => void;
	onAuthorClick?: (authorId: string) => void;
	className?: string;
}

export const TemplateTable: React.FC<TemplateTableProps> = ({
	templates,
	onTemplateClick,
	onTagClick,
	onAuthorClick,
	className = "",
}) => {
	const { t } = useTranslation();

	const formatDate = (dateString: string) => {
		return new Intl.DateTimeFormat("en-US", {
			year: "numeric",
			month: "short",
			day: "numeric",
		}).format(new Date(dateString));
	};

	const handleTitleClick = (template: TemplateDto) => {
		onTemplateClick?.(template);
	};

	const handleAuthorClick = (e: React.MouseEvent, authorId: string) => {
		e.stopPropagation();
		onAuthorClick?.(authorId);
	};

	const handleTagClick = (e: React.MouseEvent, tagName: string) => {
		e.stopPropagation();
		onTagClick?.(tagName);
	};

	if (templates.length === 0) {
		return (
			<div className={`text-center py-8 ${className}`}>
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
						{t("noTemplatesFound", "No templates found")}
					</p>
					<p className="text-sm">
						{t("tryDifferentSearch", "Try adjusting your search criteria")}
					</p>
				</div>
			</div>
		);
	}

	return (
		<div
			className={`bg-surface border border-border rounded-lg overflow-hidden ${className}`}
		>
			<div className="overflow-x-auto">
				<table className="w-full">
					<thead className="bg-background border-b border-border">
						<tr>
							<th className="px-4 py-3 text-left text-sm font-medium text-text">
								{t("title", "Title")}
							</th>
							<th className="px-4 py-3 text-left text-sm font-medium text-text">
								{t("author", "Author")}
							</th>
							<th className="px-4 py-3 text-left text-sm font-medium text-text">
								{t("topic", "Topic")}
							</th>
							<th className="px-4 py-3 text-left text-sm font-medium text-text">
								{t("tags", "Tags")}
							</th>
							<th className="px-4 py-3 text-left text-sm font-medium text-text">
								{t("createdAt", "Created")}
							</th>
							<th className="px-4 py-3 text-left text-sm font-medium text-text">
								{t("stats", "Stats")}
							</th>
							<th className="px-4 py-3 text-left text-sm font-medium text-text">
								{t("status", "Status")}
							</th>
						</tr>
					</thead>
					<tbody>
						{templates.map((template) => (
							<tr
								key={template.id}
								className="border-b border-border hover:bg-background/50 transition-colors cursor-pointer"
								onClick={() => handleTitleClick(template)}
							>
								<td className="px-4 py-3">
									<div className="flex items-start gap-3">
										{template.imageUrl && (
											<img
												src={template.imageUrl}
												alt={template.title}
												className="w-10 h-10 rounded object-cover flex-shrink-0"
											/>
										)}
										<div className="min-w-0 flex-1">
											<h3 className="font-medium text-text hover:text-primary transition-colors line-clamp-2">
												{template.title}
											</h3>
											{template.description && (
												<p className="text-sm text-textMuted mt-1 line-clamp-1">
													{template.description}
												</p>
											)}
										</div>
									</div>
								</td>
								<td className="px-4 py-3">
									<button
										onClick={(e) => handleAuthorClick(e, template.authorId)}
										className="text-sm text-textMuted hover:text-primary transition-colors"
									>
										{template.authorName}
									</button>
								</td>
								<td className="px-4 py-3">
									<span className="text-sm text-textMuted">
										{template.topic}
									</span>
								</td>
								<td className="px-4 py-3">
									<div className="flex flex-wrap gap-1">
										{template.tags.slice(0, 3).map((tag) => (
											<button
												key={tag.id}
												onClick={(e) => handleTagClick(e, tag.name)}
												className="px-2 py-1 bg-primary/10 text-primary text-xs rounded-full hover:bg-primary/20 transition-colors"
											>
												{tag.name}
											</button>
										))}
										{template.tags.length > 3 && (
											<span className="px-2 py-1 text-textMuted text-xs">
												+{template.tags.length - 3}
											</span>
										)}
									</div>
								</td>
								<td className="px-4 py-3">
									<span className="text-sm text-textMuted">
										{formatDate(template.createdAt)}
									</span>
								</td>
								<td className="px-4 py-3">
									<div className="flex items-center gap-3 text-sm text-textMuted">
										<span title={t("forms", "Forms") || "Forms"}>
											📝 {template.formsCount}
										</span>
										<span title={t("likes", "Likes") || "Likes"}>
											❤️ {template.likesCount}
										</span>
									</div>
								</td>
								<td className="px-4 py-3">
									<span
										className={`px-2 py-1 text-xs rounded-full ${
											template.isPublished
												? "bg-success/10 text-success"
												: "bg-warning/10 text-warning"
										}`}
									>
										{template.isPublished
											? t("published", "Published")
											: t("draft", "Draft")}
									</span>
								</td>
							</tr>
						))}
					</tbody>
				</table>
			</div>
		</div>
	);
};
