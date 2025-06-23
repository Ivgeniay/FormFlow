import { useTranslation } from "react-i18next";

interface DragHandleProps {
	dragHandleProps?: any;
}

export const DragHandle: React.FC<DragHandleProps> = ({ dragHandleProps }) => {
	const { t } = useTranslation();

	return (
		<div
			{...dragHandleProps}
			className="absolute top-3 left-1/2 transform -translate-x-1/2 cursor-grab active:cursor-grabbing text-textMuted hover:text-text transition-colors z-10"
			title={t("dragToReorder") || "Drag to reorder"}
		>
			<svg className="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
				<circle cx="0" cy="6" r="1.5" />
				<circle cx="8" cy="6" r="1.5" />
				<circle cx="16" cy="6" r="1.5" />
				<circle cx="0" cy="12" r="1.5" />
				<circle cx="8" cy="12" r="1.5" />
				<circle cx="16" cy="12" r="1.5" />
			</svg>
		</div>
	);
};
