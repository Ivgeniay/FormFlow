import React, { useRef, useState } from "react";
import ContentEditable from "react-contenteditable";
import { MarkdownToolbar } from "./MarkdownToolbar";

interface FormattedTextInputProps {
	value: string;
	onChange: (value: string) => void;
	placeholder?: string;
	multiline?: boolean;
	className?: string;
}

export const FormattedTextInput: React.FC<FormattedTextInputProps> = ({
	value,
	onChange,
	placeholder,
	multiline = false,
	className = "",
}) => {
	const [showToolbar, setShowToolbar] = useState(false);
	const contentRef = useRef<HTMLElement | null>(null);

	const handleChange = (evt: any) => {
		onChange(evt.target.value);
	};

	const formatText = (command: string) => {
		document.execCommand(command, false);
		if (contentRef.current) {
			contentRef.current.focus();
		}
	};

	const handleKeyDown = (e: React.KeyboardEvent) => {
		if (e.ctrlKey || e.metaKey) {
			switch (e.key.toLowerCase()) {
				case "b":
					e.preventDefault();
					formatText("bold");
					break;
				case "i":
					e.preventDefault();
					formatText("italic");
					break;
				case "u":
					e.preventDefault();
					formatText("underline");
					break;
			}
		}
	};

	const handleFocus = () => {
		setShowToolbar(true);
	};

	const handleBlur = (e: any) => {
		if (!e.relatedTarget || !e.relatedTarget.closest(".toolbar")) {
			setShowToolbar(false);
		}
	};

	const baseClasses = multiline
		? "w-full bg-transparent text-text border-0 border-b border-transparent hover:border-border focus:border-primary focus:outline-none transition-colors resize-none p-2 -ml-2 min-h-[50px]"
		: "w-full text-lg font-medium bg-transparent text-text border-0 border-b-2 border-transparent hover:border-border focus:border-primary focus:outline-none transition-colors p-2 -ml-2";

	return (
		<div className={`relative ${className}`}>
			<ContentEditable
				innerRef={contentRef as any}
				html={value}
				disabled={false}
				onChange={handleChange}
				onKeyDown={handleKeyDown}
				onFocus={handleFocus}
				onBlur={handleBlur}
				tagName={multiline ? "div" : "span"}
				className={baseClasses}
				style={{
					whiteSpace: multiline ? "pre-wrap" : "nowrap",
					overflow: multiline ? "auto" : "hidden",
				}}
			/>

			{!value && placeholder && (
				<div className="absolute inset-0 pointer-events-none text-textMuted p-2 -ml-2">
					{placeholder}
				</div>
			)}

			{showToolbar && <MarkdownToolbar onFormat={formatText} />}
		</div>
	);
};
