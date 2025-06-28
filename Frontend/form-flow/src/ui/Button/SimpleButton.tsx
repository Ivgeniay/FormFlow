import React from "react";
import { useTranslation } from "react-i18next";

interface SimpleButtonProps {
	onClick: () => void;
	localKey: string;
	className?: string;
	title?: string;
	disabled?: boolean;
}

export const SimpleButton: React.FC<SimpleButtonProps> = ({
	onClick,
	localKey,
	className,
	title,
	disabled,
}) => {
	const { t } = useTranslation();

	return (
		<button
			onClick={onClick}
			title={title}
			disabled={disabled}
			className={`${className} px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-colors min-w-[100px]`}
		>
			{t(localKey)}
		</button>
	);
};
