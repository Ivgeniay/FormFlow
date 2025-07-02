import React from "react";
import { useTranslation } from "react-i18next";

interface LikeButtonProps {
	likesCount: number;
	isUserLiked: boolean;
	isAuthenticated: boolean;
	onLikeToggle: () => void;
	className?: string;
	size?: number;
}

export const LikeButton: React.FC<LikeButtonProps> = ({
	likesCount,
	isUserLiked,
	isAuthenticated,
	onLikeToggle,
	className = "",
	size = 6,
}) => {
	const { t } = useTranslation();

	return (
		<button
			onClick={onLikeToggle}
			disabled={!isAuthenticated}
			className={`inline-flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${
				isUserLiked
					? "text-error hover:text-error/80"
					: "text-textMuted hover:text-text"
			} hover:bg-surface ${className}`}
			title={
				!isAuthenticated
					? t("logInToLike", "Log in to like") || "Log in to like"
					: isUserLiked
					? t("unlike", "Unlike") || "Unlike"
					: t("like", "Like") || "Like"
			}
		>
			<svg
				className={`w-${size} h-${size}`}
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
			<span>{likesCount}</span>
		</button>
	);
};
