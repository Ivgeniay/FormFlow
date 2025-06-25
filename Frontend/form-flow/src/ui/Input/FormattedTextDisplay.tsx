import React from "react";
import ContentEditable from "react-contenteditable";

interface FormattedTextDisplayProps {
	value: string;
	className?: string;
}

export const FormattedTextDisplay: React.FC<FormattedTextDisplayProps> = ({
	value,
	className = "",
}) => {
	if (!value) return null;

	return (
		<ContentEditable
			html={value}
			disabled={true}
			onChange={() => {}}
			className={`${className}`}
			style={{
				outline: "none",
				border: "none",
				background: "transparent",
			}}
		/>
	);
};
