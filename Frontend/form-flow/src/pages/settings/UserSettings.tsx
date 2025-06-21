import React from "react";
import { useTranslation } from "react-i18next";
import { useAppStore } from "../../stores/appStore";
import { ThemeDropdown } from "../../components/ThemeDropdown";
import { LanguageDropdown } from "../../components/LanguageDropdown";
import { useNavigate } from "react-router-dom";

export const UserSettings: React.FC = () => {
	const { t } = useTranslation();
	const { getSaftyThemes, getSaftyLanguages } = useAppStore();
	const navigate = useNavigate();

	const goBack = () => {
		if (window.history.length > 1) {
			navigate(-1);
		} else {
			navigate("/");
		}
	};

	return (
		<div className="min-h-screen bg-background">
			<div className="container mx-auto px-4 py-8 max-w-4xl">
				<button
					onClick={goBack}
					className="inline-flex items-center gap-2 text-textMuted hover:text-text transition-colors mb-6"
				>
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
							d="M15 19l-7-7 7-7"
						/>
					</svg>
					{t("back") || "Back"}
				</button>
				<div className="mb-8">
					<h1 className="text-3xl font-bold text-text mb-2">
						{t("settings") || "Settings"}
					</h1>
					<p className="text-textMuted">
						{t("settingsDescription") ||
							"Manage your account preferences and interface settings"}
					</p>
				</div>

				<div className="space-y-6">
					<div className="bg-surface border border-border rounded-lg p-6">
						<h2 className="text-xl font-semibold text-text mb-4">
							{t("interfaceSettings") || "Interface Settings"}
						</h2>

						<div className="grid grid-cols-1 md:grid-cols-2 gap-6">
							<div>
								<label className="block text-sm font-medium text-text mb-2">
									{t("theme") || "Theme"}
								</label>
								<ThemeDropdown
									availableThemes={getSaftyThemes()}
									className="w-full"
								/>
								<p className="text-xs text-textMuted mt-1">
									{t("themeDescription") ||
										"Choose your preferred color scheme"}
								</p>
							</div>

							<div>
								<label className="block text-sm font-medium text-text mb-2">
									{t("language") || "Language"}
								</label>
								<LanguageDropdown
									availableLanguages={getSaftyLanguages()}
									className="w-full"
								/>
								<p className="text-xs text-textMuted mt-1">
									{t("languageDescription") || "Select your preferred language"}
								</p>
							</div>
						</div>
					</div>

					<div className="bg-surface border border-border rounded-lg p-6">
						<h2 className="text-xl font-semibold text-text mb-4">
							{t("accountInfo") || "Account Information"}
						</h2>

						<div className="text-textMuted">
							<p>
								{t("accountInfoPlaceholder") ||
									"Account management features coming soon..."}
							</p>
						</div>
					</div>
				</div>
			</div>
		</div>
	);
};
