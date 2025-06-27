import React from "react";
import { useTranslation } from "react-i18next";
import { CommentItem } from "./CommentItem";
import { CommentDto } from "../../shared/api_types";
import { LittleSpinner } from "../../ui/Spinner/LittleSpinner";

interface CommentsListProps {
	comments: CommentDto[];
	isLoading?: boolean;
	hasMore?: boolean;
	onLoadMore?: () => void;
	className?: string;
}

export const CommentsList: React.FC<CommentsListProps> = ({
	comments,
	isLoading = false,
	hasMore = false,
	onLoadMore,
	className = "",
}) => {
	const { t } = useTranslation();

	if (comments.length === 0 && !isLoading) {
		return (
			<div className={`text-center py-8 ${className}`}>
				<div className="text-textMuted">
					<p className="text-lg mb-2">
						{t("noCommentsYet", "No comments yet") || "No comments yet"}
					</p>
					<p className="text-sm">
						{t("beFirstToComment", "Be the first to leave a comment!") ||
							"Be the first to leave a comment!"}
					</p>
				</div>
			</div>
		);
	}

	return (
		<div className={`space-y-4 ${className}`}>
			{comments.map((comment) => (
				<CommentItem key={comment.id} comment={comment} />
			))}
			<LittleSpinner isLoading={isLoading} />
			{hasMore && !isLoading && onLoadMore && (
				<div className="text-center py-4">
					<button
						onClick={onLoadMore}
						className="px-4 py-2 bg-surface border border-border rounded-lg text-text hover:bg-background transition-colors"
					>
						{t("loadMore", "Load more comments") || "Load more comments"}
					</button>
				</div>
			)}
		</div>
	);
};
