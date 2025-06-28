import React from "react";
import { FormattedTextDisplay } from "../../ui/Input/FormattedTextDisplay";
import { CommentDto } from "../../shared/api_types";

interface CommentItemProps {
	comment: CommentDto;
	className?: string;
}

export const CommentItem: React.FC<CommentItemProps> = ({
	comment,
	className = "",
}) => {
	return (
		<div
			className={`p-4 border border-border rounded-lg bg-surface ${className}`}
		>
			<div className="flex justify-between items-start mb-3">
				<div className="flex items-center gap-2">
					<span className="font-medium text-text">{comment.authorName}</span>
				</div>
				<span className="text-xs text-textMuted">
					{new Date(comment.createdAt).toLocaleDateString()}
				</span>
			</div>

			<FormattedTextDisplay value={comment.content} className="text-text" />
		</div>
	);
};
