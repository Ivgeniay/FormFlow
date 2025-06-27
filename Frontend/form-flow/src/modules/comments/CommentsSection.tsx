import React from "react";
import { useTranslation } from "react-i18next";
import { CommentDto } from "../../shared/api_types";
import { CommentDisposer } from "./CommentDisposer";

interface CommentsSectionProps {
	templateId: string;
	isAuthenticated: boolean;
	className?: string;
}

export const CommentsSection: React.FC<CommentsSectionProps> = ({
	templateId,
	isAuthenticated,
	className = "",
}) => {
	const { t } = useTranslation();

	const handleAddComment = async (content: string): Promise<void> => {
		console.log("Adding comment:", content, "to template:", templateId);
	};

	const handleLoadComments = async (
		page: number,
		pageSize: number
	): Promise<CommentDto[]> => {
		console.log(
			"Loading comments for template:",
			templateId,
			"page:",
			page,
			"pageSize:",
			pageSize
		);

		return [
			{
				id: "1",
				templateId: templateId,
				authorId: "user1",
				authorName: "John Doe",
				content: "This is a test comment",
				createdAt: new Date().toISOString(),
				canDelete: false,
				isAuthor: false,
			},
			{
				id: "2",
				templateId: templateId,
				authorId: "200",
				authorName: "Kek",
				content: "This is awisome comment",
				createdAt: new Date().toISOString(),
				canDelete: false,
				isAuthor: false,
			},
			{
				id: "3",
				templateId: templateId,
				authorId: "200",
				authorName: "Kek",
				content: "This is awisome comment",
				createdAt: new Date().toISOString(),
				canDelete: false,
				isAuthor: false,
			},
			{
				id: "4",
				templateId: templateId,
				authorId: "201",
				authorName: "Kek22",
				content: "This is awisome comment",
				createdAt: new Date().toISOString(),
				canDelete: false,
				isAuthor: false,
			},
			{
				id: "5",
				templateId: templateId,
				authorId: "200",
				authorName: "Kek",
				content: "This is awisome comment",
				createdAt: new Date().toISOString(),
				canDelete: false,
				isAuthor: false,
			},
			{
				id: "6",
				templateId: templateId,
				authorId: "200",
				authorName: "Kek",
				content: "This is awisome comment",
				createdAt: new Date().toISOString(),
				canDelete: false,
				isAuthor: false,
			},
		];
	};

	return (
		<div className={`bg-surface border border-border rounded-lg ${className}`}>
			<CommentDisposer
				templateId={templateId}
				onAddComment={handleAddComment}
				onLoadComments={handleLoadComments}
				className="h-96"
				canComment={isAuthenticated}
			/>
		</div>
	);
};
