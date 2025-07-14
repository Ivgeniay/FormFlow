import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import { useAuth } from "./hooks/useAuth";
import { apiTokenApi, ApiTokenDto } from "../../api/apiTokenApi";

export const ApiKeySection: React.FC = () => {
	const { t } = useTranslation();
	const { accessToken } = useAuth();
	const [currentToken, setCurrentToken] = useState<ApiTokenDto | null>(null);
	const [isLoading, setIsLoading] = useState(true);
	const [isGenerating, setIsGenerating] = useState(false);

	useEffect(() => {
		if (accessToken) {
			loadCurrentToken();
		}
	}, [accessToken]);

	const loadCurrentToken = async () => {
		if (!accessToken) return;

		try {
			const token = await apiTokenApi.getCurrentToken(accessToken);
			setCurrentToken(token);
		} catch (error) {
			console.error("Failed to load current token:", error);
		} finally {
			setIsLoading(false);
		}
	};

	const handleGenerateApiKey = async () => {
		if (!accessToken) return;

		try {
			setIsGenerating(true);
			const newToken = await apiTokenApi.generateToken(accessToken);
			setCurrentToken(newToken);
			toast.success(t("apiKeyGenerated") || "API key generated successfully");
		} catch (error) {
							toast.error("Failed to generate API key");
			console.error("Failed to generate token:", error);
		} finally {
			setIsGenerating(false);
		}
	};

	const copyToClipboard = (text: string) => {
		navigator.clipboard.writeText(text);
		toast.success(t("apiKeyCopied") || "API key copied to clipboard");
	};

	if (isLoading) {
		return (
			<div className="max-w-4xl mx-auto p-6">
				<div className="bg-surface border border-border rounded-lg p-6">
					<div className="animate-pulse">
						<div className="h-6 bg-border rounded w-1/4 mb-4"></div>
						<div className="h-4 bg-border rounded w-3/4"></div>
					</div>
				</div>
			</div>
		);
	}

	return (
		<div className="max-w-4xl mx-auto p-6">
			<div className="bg-surface border border-border rounded-lg p-6">
				<p className="text-textMuted mb-6">
					{t("apiKeyDescription") || "Use API key for integration with external systems"}
				</p>

				{currentToken ? (
					<div className="space-y-4">
						<div className="flex items-center justify-between p-4 bg-background border border-border rounded-lg">
							<div>
								<p className="text-sm font-medium text-text">
									{t("apiKeyExists") || "API key already exists"}
								</p>
								<p className="text-xs text-textMuted">
									Created: {new Date(currentToken.createdAt).toLocaleDateString()}
								</p>
							</div>
							<button
								onClick={() => copyToClipboard(currentToken.token)}
								className="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary/90 transition-colors"
							>
								{t("copyApiKey") || "Copy API Key"}
							</button>
						</div>
					</div>
				) : (
					<button
						onClick={handleGenerateApiKey}
						disabled={isGenerating}
						className="px-6 py-3 bg-primary text-white rounded-lg hover:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
					>
						{isGenerating 
							? (t("generatingApiKey") || "Generating API key...")
							: (t("generateApiKey") || "Generate API Key")
						}
					</button>
				)}
			</div>
		</div>
	);
};