import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { TemplateDto } from "../../../../shared/api_types";

interface TemplateHeaderProps {
	template: TemplateDto;
}

export const TemplateHeader: React.FC<TemplateHeaderProps> = ({ template }) => {
	const { t } = useTranslation();
	const [imageError, setImageError] = useState(false);

	return (
		<>
			<div className="bg-surface border border-border rounded-lg mb-2">
				<div className="w-full h-48 mb-4 rounded-lg overflow-hidden">
					{template.imageUrl && !imageError ? (
						<img
							src={template.imageUrl}
							alt={template.title}
							className="w-full h-full object-cover"
							onError={() => setImageError(true)}
						/>
					) : (
						<div className="w-full h-full bg-gradient-to-b from-primary to-primary/70 flex items-center justify-center">
							<svg
								className="w-16 h-16 text-white/70"
								fill="none"
								stroke="currentColor"
								viewBox="0 0 24 24"
							>
								<path
									strokeLinecap="round"
									strokeLinejoin="round"
									strokeWidth={1.5}
									d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
								/>
							</svg>
						</div>
					)}
				</div>
			</div>

			<div className="bg-surface border border-border rounded-lg p-6 mb-6">
				<h1 className="text-2xl font-bold text-text mb-2">{template.title}</h1>
				{template.description && (
					<p className="text-textMuted mb-4">{template.description}</p>
				)}
				<div className="flex items-center gap-4 text-sm text-textMuted">
					<span>
						{t("author", "Author")}: {template.authorName}
					</span>
					<span>
						{t("topic", "Topic")}: {template.topic}
					</span>
				</div>
			</div>
		</>
	);
};
