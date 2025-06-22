import React, { useState, useRef } from "react";
import { useTranslation } from "react-i18next";

interface ImageUploaderProps {
	onImageSelect: (file: File | null) => void;
	currentImage?: string | null;
	className?: string;
}

export const ImageUploader: React.FC<ImageUploaderProps> = ({
	onImageSelect,
	currentImage,
	className = "",
}) => {
	const { t } = useTranslation();
	const [isDragOver, setIsDragOver] = useState(false);
	const [preview, setPreview] = useState<string | null>(currentImage || null);
	const fileInputRef = useRef<HTMLInputElement>(null);

	const supportedFormats = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
	const maxFileSize = 5 * 1024 * 1024; // 5MB

	const validateFile = (file: File): boolean => {
		const extension = `.${file.name.split(".").pop()?.toLowerCase()}`;

		if (!supportedFormats.includes(extension)) {
			alert(
				t("unsupportedFileFormat") ||
					"Unsupported file format. Please use: " + supportedFormats.join(", ")
			);
			return false;
		}

		if (file.size > maxFileSize) {
			alert(t("fileTooLarge") || "File is too large. Maximum size is 5MB.");
			return false;
		}

		return true;
	};

	const handleFileSelect = (file: File) => {
		if (!validateFile(file)) return;

		const reader = new FileReader();
		reader.onload = (e) => {
			const result = e.target?.result as string;
			setPreview(result);
			onImageSelect(file);
		};
		reader.readAsDataURL(file);
	};

	const handleDragOver = (e: React.DragEvent) => {
		e.preventDefault();
		setIsDragOver(true);
	};

	const handleDragLeave = (e: React.DragEvent) => {
		e.preventDefault();
		setIsDragOver(false);
	};

	const handleDrop = (e: React.DragEvent) => {
		e.preventDefault();
		setIsDragOver(false);

		const files = Array.from(e.dataTransfer.files);
		if (files.length > 0) {
			handleFileSelect(files[0]);
		}
	};

	const handleClick = () => {
		fileInputRef.current?.click();
	};

	const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const files = e.target.files;
		if (files && files.length > 0) {
			handleFileSelect(files[0]);
		}
	};

	const handleRemoveImage = () => {
		setPreview(null);
		onImageSelect(null);
		if (fileInputRef.current) {
			fileInputRef.current.value = "";
		}
	};

	return (
		<div className={`relative ${className}`}>
			<input
				ref={fileInputRef}
				type="file"
				accept={supportedFormats.join(",")}
				onChange={handleFileInputChange}
				className="hidden"
			/>

			{preview ? (
				<div className="relative group">
					<img
						src={preview}
						alt={t("uploadedImage") || "Uploaded image"}
						className="w-full h-32 object-cover rounded-lg border border-border"
					/>
					<div className="absolute inset-0 bg-black bg-opacity-50 opacity-0 group-hover:opacity-100 transition-opacity duration-200 rounded-lg flex items-center justify-center">
						<button
							onClick={handleRemoveImage}
							className="px-3 py-1 bg-error text-white rounded-md text-sm hover:bg-opacity-90 transition-colors"
						>
							{t("removeImage") || "Remove"}
						</button>
					</div>
				</div>
			) : (
				<div
					onClick={handleClick}
					onDragOver={handleDragOver}
					onDragLeave={handleDragLeave}
					onDrop={handleDrop}
					className={`
						w-full h-32 border-2 border-dashed rounded-lg cursor-pointer transition-all duration-200
						flex flex-col items-center justify-center gap-2 text-center p-4
						${
							isDragOver
								? "border-primary bg-primary bg-opacity-10 scale-105"
								: "border-border hover:border-primary hover:bg-surface"
						}
					`}
				>
					<svg
						className="w-8 h-8 text-textMuted"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
						/>
					</svg>
					<div className="text-sm text-text">
						<span className="font-medium">
							{t("clickToUpload") || "Click to upload"}
						</span>
						{" " + (t("orDragAndDrop") || "or drag and drop")}
					</div>
					<div className="text-xs text-textMuted">
						{t("supportedFormats") || "Supported formats"}:{" "}
						{supportedFormats.join(", ")}
					</div>
					<div className="text-xs text-textMuted">
						{t("maxFileSize") || "Maximum size"}: 5MB
					</div>
				</div>
			)}
		</div>
	);
};
