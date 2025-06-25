import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { TemplateDto } from "../../../../shared/api_types";
import { FormattedTextDisplay } from "../../../../ui/Input/FormattedTextDisplay";
import { TemplatePlaceholder } from "./TemplateImagePlaceholder";

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
						<TemplatePlaceholder />
					)}
				</div>
			</div>

			<div className="bg-surface border border-border rounded-lg p-6 mb-6">
				<div className="text-2xl font-bold text-text mb-2">
					<FormattedTextDisplay value={template.title} />
				</div>
				{template.description && (
					<div className="text-textMuted mb-4">
						<FormattedTextDisplay value={template.description} />
					</div>
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
