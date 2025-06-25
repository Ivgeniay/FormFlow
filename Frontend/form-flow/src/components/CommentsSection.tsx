import React from "react";
import { useTranslation } from "react-i18next";
import { CommentDto } from "../shared/api_types";

interface CommentsSectionProps {
	commentsCount: number;
	comments?: CommentDto[];
	isAuthenticated: boolean;
	onSubmitComment?: (content: string) => void;
	className?: string;
}

export const CommentsSection: React.FC<CommentsSectionProps> = ({
	commentsCount,
	comments = [],
	isAuthenticated,
	onSubmitComment,
	className = "",
}) => {
	const { t } = useTranslation();

	return (
		<div
			className={`bg-surface border border-border rounded-lg p-6 ${className}`}
		>
			<h2 className="text-xl font-semibold text-text mb-4">
				{t("comments", "Comments")} ({commentsCount})
			</h2>

			<div className="text-textMuted">
				{t("commentsPlaceholder", "Comments component will be here")}
			</div>

			{isAuthenticated && (
				<div className="mt-4 pt-4 border-t border-border">
					<div className="text-textMuted">
						{t(
							"commentFormPlaceholder",
							"Comment form will be here (for authenticated users)"
						)}
					</div>
				</div>
			)}

			{!isAuthenticated && (
				<div className="mt-4 pt-4 border-t border-border">
					<p className="text-textMuted">
						<a href="/login" className="text-primary hover:underline">
							{t("login", "Log in")}
						</a>{" "}
						{t("logInToComment", "to leave a comment")}
					</p>
				</div>
			)}
		</div>
	);
};
