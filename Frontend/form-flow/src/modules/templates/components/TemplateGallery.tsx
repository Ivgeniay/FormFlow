import React from 'react';
import { TemplateCard } from './TemplateCard';
import { TemplateDto } from '../../../shared/api_types';

export interface TemplateGalleryProps {
	templates: TemplateDto[];
	title?: string;
	maxItems?: number;
	mode?: 'compact' | 'detailed';
	columns?: 2 | 3 | 4 | 6;
	showViewMore?: boolean;
	onViewMore?: () => void;
	onTemplateClick?: (template: TemplateDto) => void;
	onTagClick?: (tagName: string) => void;
	onAuthorClick?: (authorId: string) => void;
	className?: string;
}

export const TemplateGallery: React.FC<TemplateGalleryProps> = ({
	templates,
	title,
	maxItems,
	mode = 'compact',
	columns = 3,
	showViewMore = true,
	onViewMore,
	onTemplateClick,
	onTagClick,
	onAuthorClick,
	className = ''
}) => {
	const displayTemplates = maxItems ? templates.slice(0, maxItems) : templates;
	const hasMore = maxItems && templates.length > maxItems;

	const getGridClasses = () => {
		const gridCols = {
			2: 'grid-cols-1 md:grid-cols-2',
			3: 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3',
			4: 'grid-cols-1 md:grid-cols-2 lg:grid-cols-4',
			6: 'grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6'
		};
		return gridCols[columns];
	};

	const getGapClasses = () => {
		return mode === 'compact' ? 'gap-4' : 'gap-6';
	};

	if (templates.length === 0) {
		return (
			<div className={`text-center py-8 ${className}`}>
				<div className="text-textMuted">
					<svg className="w-12 h-12 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
						<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
					</svg>
					<p className="text-lg font-medium">Шаблоны не найдены</p>
					<p className="text-sm">Попробуйте изменить критерии поиска</p>
				</div>
			</div>
		);
	}

	return (
		<div className={`space-y-4 ${className}` }>
			{title && (
				<div className="flex items-center justify-between">
					<h2 className="text-2xl font-bold text-text">{title}</h2>
					{hasMore && showViewMore && onViewMore && (
						<button
							onClick={onViewMore}
							className="flex items-center gap-2 text-primary hover:text-primary/80 font-medium transition-colors"
						>
							<span>Показать все</span>
							<svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
								<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
							</svg>
						</button>
					)}
				</div>
			)}

			<div className={`grid ${getGridClasses()} ${getGapClasses()}`}>
				{displayTemplates.map((template) => (
					<TemplateCard
						key={template.id}
						template={template}
						mode={mode}
						size={mode === 'compact' ? 'sm' : 'md'}
						onClick={onTemplateClick}
						onTagClick={onTagClick}
						onAuthorClick={onAuthorClick}
					/>
				))}
			</div>

			{hasMore && showViewMore && onViewMore && !title && (
				<div className="text-center pt-4">
					<button
						onClick={onViewMore}
						className="px-6 py-3 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity font-medium"
					>
						Показать больше ({templates.length - maxItems!} еще)
					</button>
				</div>
			)}
		</div>
	);
};