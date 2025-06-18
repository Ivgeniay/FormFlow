import React, { useRef, useLayoutEffect, useState, useTransition } from 'react';
import * as am5 from "@amcharts/amcharts5";
import * as am5hierarchy from "@amcharts/amcharts5/hierarchy";
import am5themes_Animated from "@amcharts/amcharts5/themes/Animated";
import { TagDto } from '../../../shared/api_types';
import { useTranslation } from 'react-i18next';

export interface TagCloudProps {
	tags: TagDto[];
	onTagClick: (tagName: string) => void;
	className?: string;
}

export const TagCloud: React.FC<TagCloudProps> = ({
	tags,
	onTagClick,
	className = ''
}) => {
	const [isExpanded, setIsExpanded] = useState(false);
	const chartRef = useRef<HTMLDivElement>(null);
	const rootRef = useRef<am5.Root | null>(null);
	const timeoutRef = useRef<NodeJS.Timeout | null>(null);
    const { t } = useTranslation();

	const handleMouseClick = () => {
		if (timeoutRef.current) {
			clearTimeout(timeoutRef.current);
			timeoutRef.current = null;
		}
		setIsExpanded(true);
	};

	const handleMouseLeave = () => {
		timeoutRef.current = setTimeout(() => {
			setIsExpanded(false);
		}, 300);
	};

	const handleCloudMouseEnter = () => {
		if (timeoutRef.current) {
			clearTimeout(timeoutRef.current);
			timeoutRef.current = null;
		}
	};

	const handleCloudMouseLeave = () => {
		setIsExpanded(false);
	};

	useLayoutEffect(() => {
		if (!isExpanded || !chartRef.current) return;

		const prepareData = () => {
			const colors = [
				"#3b82f6", "#ef4444", "#10b981", "#f59e0b", "#8b5cf6",
				"#ec4899", "#06b6d4", "#84cc16", "#f97316", "#6366f1"
			];

			return {
				name: "Теги",
				children: tags.map((tag, index) => ({
					name: tag.name,
					value: tag.usageCount,
					fill: am5.color(colors[index % colors.length]),
					tagData: tag
				}))
			};
		};

		const root = am5.Root.new(chartRef.current);
		rootRef.current = root;

		root.setThemes([am5themes_Animated.new(root)]);

		const container = root.container.children.push(
			am5.Container.new(root, {
				width: am5.percent(100),
				height: am5.percent(100),
				layout: root.verticalLayout
			})
		);

		const series = container.children.push(
			am5hierarchy.ForceDirected.new(root, {
				singleBranchOnly: false,
				downDepth: 1,
				topDepth: 1,
				initialDepth: 1,
				valueField: "value",
				categoryField: "name",
				childDataField: "children",
				idField: "name",
				linkWithField: "linkWith",
				manyBodyStrength: -15,
				centerStrength: 0.8,
				minRadius: 20,
				maxRadius: 80
			})
		);

		series.get("colors")?.set("colors", [
			am5.color("#3b82f6"),
			am5.color("#ef4444"), 
			am5.color("#10b981"),
			am5.color("#f59e0b"),
			am5.color("#8b5cf6"),
			am5.color("#ec4899"),
			am5.color("#06b6d4"),
			am5.color("#84cc16"),
			am5.color("#f97316"),
			am5.color("#6366f1")
		]);

		series.nodes.template.setAll({
			cursorOverStyle: "pointer"
		});

		series.nodes.template.events.on("click", (e) => {
			const dataItem = e.target.dataItem;
			if (dataItem && dataItem.dataContext) {
				const tagName = (dataItem.dataContext as any).name;
				onTagClick(tagName);
				setIsExpanded(false);
			}
		});

		series.nodes.template.states.create("hover", {
			scale: 1.2
		});

		series.labels.template.setAll({
			centerX: am5.percent(50),
			centerY: am5.percent(50),
			fontSize: "0.8em",
			fontWeight: "500",
			fill: am5.color("#ffffff")
		});

		const data = prepareData();
		series.data.setAll([data]);

		series.set("selectedDataItem", series.dataItems[0]);

		return () => {
			root.dispose();
			rootRef.current = null;
		};
	}, [isExpanded, tags, onTagClick]);

	return (
		<div className={`relative ${className}`}>
			<div
				className="inline-flex items-center gap-2 px-6 py-3 bg-surface border border-border rounded-full cursor-pointer hover:bg-background hover:border-primary/50 transition-all duration-200 shadow-sm"
				onMouseDown={handleMouseClick}
				onMouseLeave={handleMouseLeave}
			>
				<svg className="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
					<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
				</svg>
				<span className="text-text font-medium">{t('tagSearch')}</span>
			</div>

			{isExpanded && (
				<div 
					className="fixed inset-0 z-50 bg-black/10 backdrop-blur-sm"
					onMouseEnter={handleCloudMouseEnter}
				>
                    <div 
                        onMouseLeave={handleCloudMouseLeave}
                        className="absolute top-1/4 left-1/4 w-1/2 h-1/2 bg-surface/50 rounded-lg shadow-2xl border border-border overflow-hidden"
                    >
                    	<div 
							ref={chartRef}
							className="w-full h-full"
							style={{ minHeight: '400px' }}
						/>
					</div>
				</div>
			)}
		</div>
	);
};