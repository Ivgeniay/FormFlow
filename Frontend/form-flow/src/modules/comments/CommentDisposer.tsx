import React, { useState, useEffect, useCallback } from "react";
import { useTranslation } from "react-i18next";
import { CommentInput } from "./CommentInput";
import { CommentsList } from "./CommentsList";
import { CommentDto } from "../../shared/api_types";

interface CommentDisposerProps {
	templateId: string;
	onAddComment: (content: string) => Promise<void>;
	onLoadComments: (page: number, pageSize: number) => Promise<CommentDto[]>;
	className?: string;
	pageSize?: number;
	canComment?: boolean;
}

export const CommentDisposer: React.FC<CommentDisposerProps> = ({
	templateId,
	onAddComment,
	onLoadComments,
	className = "",
	pageSize = 10,
	canComment = true,
}) => {
	const { t } = useTranslation();

	const [comments, setComments] = useState<CommentDto[]>([]);
	const [isLoading, setIsLoading] = useState(false);
	const [isSubmitting, setIsSubmitting] = useState(false);
	const [hasMore, setHasMore] = useState(true);
	const [page, setPage] = useState(1);
	const [error, setError] = useState<string | null>(null);

	const loadComments = useCallback(
		async (pageToLoad: number) => {
			if (isLoading) return;

			setIsLoading(true);
			setError(null);

			try {
				const newComments = await onLoadComments(pageToLoad, pageSize);

				if (pageToLoad === 1) {
					setComments(newComments);
				} else {
					setComments((prev) => [...prev, ...newComments]);
				}

				setHasMore(newComments.length === pageSize);
				setPage(pageToLoad);
			} catch (err) {
				setError(
					t("failedToLoadComments", "Failed to load comments") ||
						"Failed to load comments"
				);
				console.error("Failed to load comments:", err);
			} finally {
				setIsLoading(false);
			}
		},
		[onLoadComments, pageSize, isLoading, t]
	);

	const handleLoadMore = useCallback(() => {
		if (!isLoading && hasMore) {
			loadComments(page + 1);
		}
	}, [loadComments, page, isLoading, hasMore]);

	const handleAddComment = useCallback(
		async (content: string) => {
			setIsSubmitting(true);
			setError(null);

			try {
				await onAddComment(content);
				await loadComments(1);
			} catch (err) {
				setError(
					t("failedToAddComment", "Failed to add comment") ||
						"Failed to add comment"
				);
				console.error("Failed to add comment:", err);
			} finally {
				setIsSubmitting(false);
			}
		},
		[onAddComment, loadComments, t]
	);

	useEffect(() => {
		loadComments(1);
	}, [templateId]);

	return (
		<div className={`flex flex-col h-full ${className}`}>
			<div className="flex-1 overflow-y-auto p-4">
				<div className="mb-6">
					<h3 className="text-lg font-semibold text-text mb-2">
						{t("comments", "Comments") || "Comments"} ({comments.length})
					</h3>

					{error && (
						<div className="bg-error/10 border border-error/20 rounded-lg p-3 mb-4">
							<p className="text-error text-sm">{error}</p>
						</div>
					)}
				</div>

				<CommentsList
					comments={comments}
					isLoading={isLoading}
					hasMore={hasMore}
					onLoadMore={handleLoadMore}
				/>
			</div>

			{canComment && (
				<div className="border-t border-border p-4 bg-surface">
					<CommentInput
						onSubmit={handleAddComment}
						isSubmitting={isSubmitting}
						disabled={!!error}
					/>
				</div>
			)}
		</div>
	);
};
