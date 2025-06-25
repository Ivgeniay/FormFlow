import React, { useRef, useState } from "react";
import ContentEditable, { ContentEditableEvent } from "react-contenteditable";
import { MarkdownToolbar } from "./MarkdownToolbar";

interface FormattedTextInputProps {
	value: string;
	onChange: (value: string) => void;
	placeholder?: string;
	multiline?: boolean;
	className?: string;
	isReadOnly?: boolean;
	maxLength?: number;
}

export const FormattedTextInput: React.FC<FormattedTextInputProps> = ({
	value,
	onChange,
	placeholder,
	multiline = false,
	className = "",
	isReadOnly = false,
	maxLength,
}) => {
	const [showToolbar, setShowToolbar] = useState(false);
	const contentRef = useRef<HTMLElement | null>(null);

	const handleChange = (evt: ContentEditableEvent) => {
		onChange(evt.target.value);
	};

	const formatText = (command: string) => {
		document.execCommand(command, false);
		if (contentRef.current) {
			contentRef.current.focus();
		}
	};

	const handleKeyDown = (e: React.KeyboardEvent) => {
		if (maxLength) {
			const textContent = contentRef.current?.textContent || "";
			if (
				textContent.length >= maxLength &&
				!["Backspace", "Delete", "ArrowLeft", "ArrowRight", "Tab"].includes(
					e.key
				) &&
				!e.ctrlKey &&
				!e.metaKey
			) {
				e.preventDefault();
				return;
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
		? "w-full bg-transparent text-text border-0 focus:outline-none resize-none p-1.5 -ml-1.5 min-h-[40px] break-words overflow-wrap-anywhere"
		: "w-full text-lg font-medium bg-transparent text-text border-0 focus:outline-none p-1.5 -ml-1.5 break-words overflow-wrap-anywhere";

	return (
		<div
			className={
				className ||
				`relative p-1.5 border border-gray-300/0 rounded-lg hover:border hover:border-gray-300/50 transition-all duration-200`
			}
		>
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
				maxLength={maxLength}
				style={{
					whiteSpace: multiline ? "pre-wrap" : "normal",
					overflow: multiline ? "auto" : "hidden",
					wordBreak: "break-word",
					overflowWrap: "break-word",
					maxWidth: "100%",
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
