import React from "react";
import { useTranslation } from "react-i18next";
import { CommentInput } from "./CommentInput";
import { CommentsList } from "./CommentsList";
import { CommentDto } from "../../shared/api_types";

interface CommentDisposerProps {
	comments: CommentDto[];
	initialCommentCounts: number;
	onAddComment: (content: string) => Promise<void>;
	onLoadMore?: () => void;
	isLoading: boolean;
	isLoadingMoreComments: boolean;
	isSubmitting: boolean;
	hasMore: boolean;
	error?: string | null;
	canComment: boolean;
	className?: string;
}

export const CommentDisposer: React.FC<CommentDisposerProps> = ({
	comments,
	initialCommentCounts,
	onAddComment,
	onLoadMore,
	isLoading,
	isLoadingMoreComments,
	isSubmitting,
	hasMore,
	error,
	canComment,
	className = "",
}) => {
	const { t } = useTranslation();

	return (
		<div className={`flex flex-col h-full ${className}`}>
			<div className="flex-1 overflow-y-auto p-4">
				<div className="mb-6">
					<h3 className="text-lg font-semibold text-text mb-2">
						{t("comments", "Comments") || "Comments"} (
						{comments.length > initialCommentCounts
							? comments.length
							: initialCommentCounts}
						)
					</h3>

					{error && (
						<div className="bg-error/10 border border-error/20 rounded-lg p-3 mb-4">
							<p className="text-error text-sm">{error}</p>
						</div>
					)}
				</div>

				{canComment && (
					<div className="border-t border-border p-4 bg-surface">
						<CommentInput
							onSubmit={onAddComment}
							isSubmitting={isSubmitting}
							disabled={!!error}
						/>
					</div>
				)}
				<CommentsList
					comments={comments}
					isLoading={isLoading}
					hasMore={hasMore}
					onLoadMore={onLoadMore}
					isLoadingMoreComments={isLoadingMoreComments}
				/>
			</div>
		</div>
	);
};
