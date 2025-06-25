import React from "react";
import { useTranslation } from "react-i18next";

interface LikeButtonProps {
	likesCount: number;
	isUserLiked: boolean;
	isAuthenticated: boolean;
	onLikeToggle: () => void;
	className?: string;
}

export const LikeButton: React.FC<LikeButtonProps> = ({
	likesCount,
	isUserLiked,
	isAuthenticated,
	onLikeToggle,
	className = "",
}) => {
	const { t } = useTranslation();

	return (
		<button
			onClick={onLikeToggle}
			disabled={!isAuthenticated}
			className={`flex items-center gap-2 px-4 py-3 rounded-lg border transition-all disabled:opacity-50 disabled:cursor-not-allowed ${
				isUserLiked
					? "bg-red-50 border-red-200 text-red-600 hover:bg-red-100"
					: "bg-gray-50 border-border text-textMuted hover:bg-primary hover:text-white"
			} ${className}`}
			title={
				!isAuthenticated
					? t("logInToLike", "Log in to like") || "Log in to like"
					: isUserLiked
					? t("unlike", "Unlike") || "Unlike"
					: t("like", "Like") || "Like"
			}
		>
			<svg
				className="w-5 h-5"
				fill={isUserLiked ? "currentColor" : "none"}
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
			<span className="font-medium">{likesCount}</span>
		</button>
	);
};
