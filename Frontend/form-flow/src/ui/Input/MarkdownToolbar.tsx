import React from "react";

interface ToolbarProps {
	onFormat: (command: string) => void;
	className?: string;
}

export const MarkdownToolbar: React.FC<ToolbarProps> = ({
	onFormat,
	className = "",
}) => {
	return (
		<div
			className={`toolbar flex items-center gap-1 mt-2 p-2 bg-background border border-border rounded shadow-sm ${className}`}
		>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("bold")}
				className="p-2 hover:bg-surface rounded text-sm font-bold text-text transition-colors"
				title="Bold (Ctrl+B)"
			>
				B
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("italic")}
				className="p-2 hover:bg-surface rounded text-sm italic text-text transition-colors"
				title="Italic (Ctrl+I)"
			>
				I
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("underline")}
				className="p-2 hover:bg-surface rounded text-sm underline text-text transition-colors"
				title="Underline (Ctrl+U)"
			>
				U
			</button>
			<div className="w-px h-6 bg-border mx-1"></div>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("strikeThrough")}
				className="p-2 hover:bg-surface rounded text-sm line-through text-text transition-colors"
				title="Strikethrough"
			>
				S
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("subscript")}
				className="p-2 hover:bg-surface rounded text-xs text-text transition-colors"
				title="Subscript"
			>
				X₂
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("superscript")}
				className="p-2 hover:bg-surface rounded text-xs text-text transition-colors"
				title="Superscript"
			>
				X²
			</button>
			<div className="w-px h-6 bg-border mx-1"></div>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("insertUnorderedList")}
				className="p-2 hover:bg-surface rounded text-sm text-text transition-colors"
				title="Bullet List"
			>
				⫷
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("justifyCenter")}
				className="p-2 hover:bg-surface rounded text-sm text-text transition-colors"
				title="Align Center"
			>
				≡
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("justifyRight")}
				className="p-2 hover:bg-surface rounded text-sm text-text transition-colors"
				title="Align Right"
			>
				⫸
			</button>
			<div className="w-px h-6 bg-border mx-1"></div>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("createLink")}
				className="p-2 hover:bg-surface rounded text-sm text-text transition-colors"
				title="Create Link"
			>
				✕
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("undo")}
				className="p-2 hover:bg-surface rounded text-sm text-text transition-colors"
				title="Undo"
			>
				↶
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("redo")}
				className="p-2 hover:bg-surface rounded text-sm text-text transition-colors"
				title="Redo"
			>
				↷
			</button>
		</div>
	);
};
