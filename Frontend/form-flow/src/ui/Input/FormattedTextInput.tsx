import React, { useRef, useState } from "react";
import ContentEditable from "react-contenteditable";
import { MarkdownToolbar } from "./MarkdownToolbar";

interface FormattedTextInputProps {
	value: string;
	onChange: (value: string) => void;
	placeholder?: string;
	multiline?: boolean;
	className?: string;
	isReadOnly?: boolean;
}

export const FormattedTextInput: React.FC<FormattedTextInputProps> = ({
	value,
	onChange,
	placeholder,
	multiline = false,
	className = "",
	isReadOnly = false,
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
		? "w-full bg-transparent text-text border-0 focus:outline-none resize-none p-1.5 -ml-1.5 min-h-[40px]"
		: "w-full text-lg font-medium bg-transparent text-text border-0 focus:outline-none p-1.5 -ml-1.5";

	return (
		<div className={`relative ${className}`}>
			<ContentEditable
				contentEditable={!isReadOnly}
				suppressContentEditableWarning={true}
				innerRef={contentRef as any}
				html={value}
				disabled={isReadOnly}
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
				<div className="absolute inset-0 pointer-events-none text-textMuted p-1.5 -ml-1.5 text-sm">
					{placeholder}
				</div>
			)}

			{/* {showToolbar && <MarkdownToolbar onFormat={formatText} />} */}
			<div
				className={`transition-all duration-200 ease-in-out overflow-hidden ${
					showToolbar
						? "opacity-100 translate-y-0 max-h-20"
						: "opacity-0 -translate-y-2 pointer-events-none max-h-0"
				}`}
			>
				{!isReadOnly && <MarkdownToolbar onFormat={formatText} />}
			</div>
		</div>
	);
};
