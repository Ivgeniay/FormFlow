import React, { useState } from "react";
import { useTranslation } from "react-i18next";

interface CommentInputProps {
	onSubmit: (content: string) => Promise<void>;
	isSubmitting?: boolean;
	disabled?: boolean;
	className?: string;
	maxLength?: number;
	rows?: number;
}

export const CommentInput: React.FC<CommentInputProps> = ({
	onSubmit,
	isSubmitting = false,
	disabled = false,
	className = "",
	maxLength = 300,
	rows = 3,
}) => {
	const { t } = useTranslation();
	const [content, setContent] = useState("");

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!content.trim() || isSubmitting || disabled) return;

		await onSubmit(content.trim());
		setContent("");
	};

	return (
		<form onSubmit={handleSubmit} className={className}>
			<div className="space-y-3">
				<textarea
					value={content}
					onChange={(e) => setContent(e.target.value)}
					placeholder={
						t("addComment", "Add a comment...") || "Add a comment..."
					}
					className="w-full p-3 border border-border rounded-lg bg-background text-text placeholder-textMuted resize-none focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary"
					rows={rows < 0 ? 1 : rows}
					maxLength={maxLength}
					disabled={disabled || isSubmitting}
				/>
				<div className="flex justify-between items-center">
					<span className="text-xs text-textMuted">
						{content.length}/{maxLength}
					</span>
					<button
						type="submit"
						disabled={!content.trim() || disabled || isSubmitting}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
					>
						{isSubmitting
							? t("submitting", "Submitting...")
							: t("submit", "Submit")}
					</button>
				</div>
			</div>
		</form>
	);
};
