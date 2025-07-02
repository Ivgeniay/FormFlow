import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import { AiTemplateDto, templateApi } from "../../../api/templateApi";

interface AiPromptFormProps {
	accessToken: string | null;
	onGenerated?: (aiTemplate: AiTemplateDto) => void;
}

export const AiPromptForm: React.FC<AiPromptFormProps> = ({
	accessToken,
	onGenerated,
}) => {
	const { t } = useTranslation();
	const [prompt, setPrompt] = useState("");
	const [isGenerating, setIsGenerating] = useState(false);
	const [isExpanded, setIsExpanded] = useState(false);

	const handleGenerate = async () => {
		if (!accessToken) {
			toast.error(t("authRequired", "Authentication required"));
			return;
		}

		if (!prompt.trim()) {
			toast.error(t("promptRequired", "Please enter a prompt"));
			return;
		}

		setIsGenerating(true);

		try {
			const result = await templateApi.generateTemplateFromAI(
				{ promt: prompt },
				accessToken
			);

			console.log("AI Generated Template:", result);
			toast.success(t("templateGenerated", "Template generated successfully!"));

			if (onGenerated) {
				onGenerated(result);
			}
		} catch (error: any) {
			const errorMessage =
				error.response?.data?.message ||
				t("generationFailed", "Failed to generate template");
			toast.error(errorMessage);
			console.error("AI Generation Error:", error);
		} finally {
			setIsGenerating(false);
		}
	};

	return (
		<div className="mb-6 bg-surface border border-border rounded-lg overflow-hidden">
			<button
				onClick={() => setIsExpanded(!isExpanded)}
				className="w-full p-4 flex items-center justify-between text-left hover:bg-background transition-colors"
			>
				<div className="flex items-center gap-3">
					<div className="w-8 h-8 bg-primary/10 rounded-lg flex items-center justify-center">
						<svg
							className="w-4 h-4 text-primary"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M13 10V3L4 14h7v7l9-11h-7z"
							/>
						</svg>
					</div>
					<div>
						<h3 className="font-medium text-text">
							{t("generateWithAI", "Generate with AI")}
						</h3>
						<p className="text-sm text-textMuted">
							{t(
								"aiGenerateDescription",
								"Let AI create your template from a description"
							)}
						</p>
					</div>
				</div>
				<svg
					className={`w-5 h-5 text-textMuted transition-transform ${
						isExpanded ? "rotate-180" : ""
					}`}
					fill="none"
					stroke="currentColor"
					viewBox="0 0 24 24"
				>
					<path
						strokeLinecap="round"
						strokeLinejoin="round"
						strokeWidth={2}
						d="M19 9l-7 7-7-7"
					/>
				</svg>
			</button>

			{isExpanded && (
				<div className="p-4 pt-0 border-t border-border">
					<div className="space-y-4">
						<div>
							<label className="block text-sm font-medium text-text mb-2">
								{t("promptLabel", "Describe your template")}
							</label>
							<textarea
								value={prompt}
								onChange={(e) => setPrompt(e.target.value)}
								placeholder={
									t(
										"promptPlaceholder",
										"Example: Create a customer satisfaction survey with rating questions and feedback fields"
									) ||
									"Example: Create a customer satisfaction survey with rating questions and feedback fields"
								}
								className="w-full h-24 px-3 py-2 bg-background border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent resize-none text-text placeholder-textMuted"
								disabled={isGenerating}
							/>
						</div>

						<div className="flex items-center justify-between">
							<div className="text-sm text-textMuted">
								{t(
									"aiGenerateHint",
									"Be specific about question types and structure you need"
								)}
							</div>
							<button
								onClick={handleGenerate}
								disabled={isGenerating || !prompt.trim()}
								className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity disabled:opacity-50 flex items-center gap-2"
							>
								{isGenerating ? (
									<>
										<svg
											className="w-4 h-4 animate-spin"
											fill="none"
											stroke="currentColor"
											viewBox="0 0 24 24"
										>
											<path
												strokeLinecap="round"
												strokeLinejoin="round"
												strokeWidth={2}
												d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
											/>
										</svg>
										{t("generating", "Generating...")}
									</>
								) : (
									<>
										<svg
											className="w-4 h-4"
											fill="none"
											stroke="currentColor"
											viewBox="0 0 24 24"
										>
											<path
												strokeLinecap="round"
												strokeLinejoin="round"
												strokeWidth={2}
												d="M13 10V3L4 14h7v7l9-11h-7z"
											/>
										</svg>
										{t("generate", "Generate")}
									</>
								)}
							</button>
						</div>
					</div>
				</div>
			)}
		</div>
	);
};
