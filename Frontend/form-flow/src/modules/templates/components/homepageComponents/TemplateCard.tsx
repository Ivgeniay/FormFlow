import React from "react";
import { TemplateDto } from "../../../../shared/api_types";
import { Tag } from "../../../Tag/components/Tag";
import { FormattedTextDisplay } from "../../../../ui/Input/FormattedTextDisplay";
import { TemplatePlaceholder } from "../editorPageTabs/TemplateImagePlaceholder";

export interface TemplateCardProps {
	template: TemplateDto;
	mode?: "compact" | "detailed";
	showAuthor?: boolean;
	showStats?: boolean;
	showTags?: boolean;
	showTopic?: boolean;
	maxTags?: number;
	size?: "sm" | "md" | "lg";
	onClick?: (template: TemplateDto) => void;
	onTagClick?: (tagName: string) => void;
	onAuthorClick?: (authorId: string) => void;
	className?: string;
}

export const TemplateCard: React.FC<TemplateCardProps> = ({
	template,
	mode = "compact",
	showAuthor = true,
	showStats = true,
	showTags = true,
	showTopic = true,
	maxTags = 3,
	size = "md",
	onClick,
	onTagClick,
	onAuthorClick,
	className = "",
}) => {
	const handleCardClick = (e: React.MouseEvent) => {
		if (onClick) {
			e.preventDefault();
			onClick(template);
		}
	};

	const handleAuthorClick = (e: React.MouseEvent) => {
		e.stopPropagation();
		if (onAuthorClick) {
			onAuthorClick(template.authorId);
		}
	};

	const handleTagClick = (tagName: string) => {
		if (onTagClick) {
			onTagClick(tagName);
		}
	};

	const formatDate = (dateString: string) => {
		return new Intl.DateTimeFormat("ru-RU", {
			day: "numeric",
			month: "short",
			year: "numeric",
		}).format(new Date(dateString));
	};

	const sizeClasses = {
		sm: mode === "compact" ? "p-3" : "p-4",
		md: mode === "compact" ? "p-4" : "p-6",
		lg: mode === "compact" ? "p-5" : "p-8",
	};

	const imageClasses = {
		sm: mode === "compact" ? "h-24" : "h-32",
		md: mode === "compact" ? "h-32" : "h-40",
		lg: mode === "compact" ? "h-40" : "h-48",
	};

	const titleClasses = {
		sm: mode === "compact" ? "text-sm" : "text-lg",
		md: mode === "compact" ? "text-base" : "text-xl",
		lg: mode === "compact" ? "text-lg" : "text-2xl",
	};

	const displayTags = template.tags.slice(0, maxTags);
	const remainingTagsCount = template.tags.length - maxTags;

	if (mode === "compact") {
		return (
			<div
				className={`
					bg-surface border border-border rounded-lg shadow-sm hover:shadow-md 
					transition-all duration-200 cursor-pointer hover:border-primary/50
					${sizeClasses[size]} ${className}
				`
					.trim()
					.replace(/\s+/g, " ")}
				onClick={handleCardClick}
			>
				<div
					className={`mb-3 ${imageClasses[size]} overflow-hidden rounded-lg`}
				>
					{template.imageUrl ? (
						<img
							src={template.imageUrl}
							alt={template.title}
							className="w-full h-full object-cover"
						/>
					) : (
						<TemplatePlaceholder />
					)}
				</div>

				<div className="space-y-2">
					<h3
						className={`font-semibold text-text line-clamp-2 ${titleClasses[size]}`}
					>
						<FormattedTextDisplay value={template.title} />
					</h3>

					{showTopic && template.topic && (
						<div className="flex items-center gap-1 text-xs text-textMuted">
							<svg
								className="w-3 h-3"
								fill="none"
								stroke="currentColor"
								viewBox="0 0 24 24"
							>
								<path
									strokeLinecap="round"
									strokeLinejoin="round"
									strokeWidth={2}
									d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
								/>
							</svg>
							<span>{template.topic}</span>
						</div>
					)}

					{showStats && (
						<div className="flex items-center gap-3 text-xs text-textMuted">
							<div className="flex items-center gap-1">
								<svg
									className="w-3 h-3"
									fill="none"
									stroke="currentColor"
									viewBox="0 0 24 24"
								>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
									/>
								</svg>
								<span>{template.formsCount}</span>
							</div>

							<div className="flex items-center gap-1">
								<svg
									className="w-3 h-3"
									fill="none"
									stroke="currentColor"
									viewBox="0 0 24 24"
								>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
									/>
								</svg>
								<span>{template.likesCount}</span>
							</div>
						</div>
					)}
				</div>
			</div>
		);
	}

	return (
		<div
			className={`
				bg-surface border border-border rounded-lg shadow-sm hover:shadow-md 
				transition-all duration-200 cursor-pointer hover:border-primary/50
				${sizeClasses[size]} ${className}
			`
				.trim()
				.replace(/\s+/g, " ")}
			onClick={handleCardClick}
		>
			{template.imageUrl ? (
				<img
					src={template.imageUrl}
					alt={template.title}
					className="w-full h-full object-cover"
				/>
			) : (
				<TemplatePlaceholder />
			)}

			<div className="space-y-3">
				<div>
					<h3
						className={`font-semibold text-text mb-2 line-clamp-2 ${titleClasses[size]}`}
					>
						<FormattedTextDisplay value={template.title} />
					</h3>
					<p className="text-textMuted text-sm line-clamp-3">
						<FormattedTextDisplay value={template.description} />
					</p>
				</div>

				{showTopic && template.topic && (
					<div className="flex items-center gap-2 text-sm">
						<svg
							className="w-4 h-4 text-textMuted"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
							/>
						</svg>
						<span className="text-textMuted font-medium">{template.topic}</span>
					</div>
				)}

				{showTags && template.tags.length > 0 && (
					<div className="flex flex-wrap gap-2">
						{displayTags.map((tag) => (
							<Tag
								key={tag.id}
								name={tag.name}
								size="sm"
								variant="default"
								onClick={onTagClick}
							/>
						))}
						{remainingTagsCount > 0 && (
							<span className="text-xs text-textMuted px-2 py-1">
								+{remainingTagsCount} еще
							</span>
						)}
					</div>
				)}

				<div className="flex items-center justify-between pt-2 border-t border-border">
					{showAuthor && (
						<button
							onClick={handleAuthorClick}
							className="flex items-center gap-2 text-sm text-textMuted hover:text-text transition-colors"
						>
							<div className="w-6 h-6 bg-primary rounded-full flex items-center justify-center text-white text-xs font-medium">
								{template.authorName.charAt(0).toUpperCase()}
							</div>
							<span>{template.authorName}</span>
						</button>
					)}

					<div className="text-xs text-textMuted">
						{formatDate(template.createdAt)}
					</div>
				</div>

				{showStats && (
					<div className="flex items-center gap-4 text-sm text-textMuted">
						<div className="flex items-center gap-1">
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
									d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
								/>
							</svg>
							<span>{template.formsCount}</span>
						</div>

						<div className="flex items-center gap-1">
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
									d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
								/>
							</svg>
							<span>{template.likesCount}</span>
						</div>

						<div className="flex items-center gap-1">
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
									d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"
								/>
							</svg>
							<span>{template.commentsCount}</span>
						</div>
					</div>
				)}
			</div>
		</div>
	);
};
