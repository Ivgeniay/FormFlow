import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { useAppStore } from "../../stores/appStore";
import { ThemeDropdown } from "../../components/ThemeDropdown";
import { LanguageDropdown } from "../../components/LanguageDropdown";
import {
	userSettingsApi,
	ColorThemeDto,
	LanguageDto,
} from "../../api/userSettingsApi";
import { ColorTheme } from "../../shared/types/color-theme";
import { Language } from "../../shared/types/language";
import toast from "react-hot-toast";
import { useTheme } from "../../shared/hooks/useTheme";
import { useLanguage } from "../../shared/hooks/useLanguage";

const mapColorThemeDtoToColorTheme = (dto: ColorThemeDto): ColorTheme => ({
	id: dto.id,
	name: dto.name,
	cssClass: dto.cssClass,
	colorVariables: dto.colorVariables,
	isDefault: dto.isDefault,
	isActive: dto.isActive,
});

const mapLanguageDtoToLanguage = (dto: LanguageDto): Language => ({
	id: dto.id,
	code: dto.code,
	shortCode: dto.shortCode,
	name: dto.name,
	iconURL: dto.iconURL || null,
	region: dto.region,
	isDefault: dto.isDefault,
	isActive: dto.isActive,
});

export const UserSettings: React.FC = () => {
	const { t } = useTranslation();
	const { getSaftyThemes, getSaftyLanguages, currentLanguage, currentTheme } =
		useAppStore();
	const { accessToken } = useAuth();
	const { setThemeById } = useTheme();
	const { setLanguageById } = useLanguage();
	const navigate = useNavigate();

	const [initialLanguage, setInitialLanguage] = useState<Language | null>(null);
	const [initialTheme, setInitialTheme] = useState<ColorTheme | null>(null);
	const [currentSettedLanguage, setCurrentSettedLanguage] =
		useState<Language | null>(null);
	const [currentSettedTheme, setCurrentSettedTheme] =
		useState<ColorTheme | null>(null);
	const [selectedLanguage, setSelectedLanguage] = useState<Language | null>(
		null
	);
	const [selectedTheme, setSelectedTheme] = useState<ColorTheme | null>(null);
	const [isLoading, setIsLoading] = useState(true);
	const [isSaving, setIsSaving] = useState(false);

	useEffect(() => {
		loadCurrentSettings();
	}, []);

	const loadCurrentSettings = async () => {
		setInitialLanguage(currentLanguage);
		setInitialTheme(currentTheme);

		if (!accessToken) return;
		try {
			setIsLoading(true);
			const settings = await userSettingsApi.getMySettings(accessToken);

			const mappedLanguage = mapLanguageDtoToLanguage(settings.language);
			const mappedTheme = mapColorThemeDtoToColorTheme(settings.colorTheme);

			setCurrentSettedLanguage(mappedLanguage);
			setCurrentSettedTheme(mappedTheme);
			setSelectedLanguage(mappedLanguage);
			setSelectedTheme(mappedTheme);
		} catch (error) {
			console.error("Failed to load settings:", error);
			toast.error(t("failedToLoadSettings") || "Failed to load settings");
		} finally {
			setIsLoading(false);
		}
	};

	const hasChanges = () => {
		return (
			selectedLanguage?.id !== currentSettedLanguage?.id ||
			selectedTheme?.id !== currentSettedTheme?.id
		);
	};

	const handleSaveChanges = async () => {
		if (!accessToken || !hasChanges()) return;

		try {
			setIsSaving(true);
			const promises = [];

			if (
				selectedLanguage?.id !== currentSettedLanguage?.id &&
				selectedLanguage
			) {
				promises.push(
					userSettingsApi.setLanguage(selectedLanguage.id, accessToken)
				);
			}

			if (selectedTheme?.id !== currentSettedTheme?.id && selectedTheme) {
				promises.push(
					userSettingsApi.setColorTheme(selectedTheme.id, accessToken)
				);
			}

			await Promise.all(promises);

			setCurrentSettedLanguage(selectedLanguage);
			setCurrentSettedTheme(selectedTheme);

			toast.success(t("settingsSaved") || "Settings saved successfully");
		} catch (error) {
			console.error("Failed to save settings:", error);
			toast.error(t("failedToSaveSettings") || "Failed to save settings");
		} finally {
			setIsSaving(false);
		}
	};

	const handleDiscardChanges = () => {
		if (initialTheme) {
			setThemeById(initialTheme.id);
		}
		if (initialLanguage) {
			setLanguageById(initialLanguage.id);
		}

		setSelectedLanguage(currentSettedLanguage);
		setSelectedTheme(currentSettedTheme);
	};

	const handleLanguageChange = (language: Language) => {
		setSelectedLanguage(language);
	};

	const handleThemeChange = (theme: ColorTheme) => {
		setSelectedTheme(theme);
	};

	const goBack = () => {
		if (window.history.length > 1) {
			navigate(-1);
		} else {
			navigate("/");
		}
	};

	if (isLoading) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center">
				<div className="text-textMuted">{t("loading") || "Loading..."}</div>
			</div>
		);
	}

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
									onThemeChange={handleThemeChange}
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
									onLanguageChange={handleLanguageChange}
									className="w-full"
								/>
								<p className="text-xs text-textMuted mt-1">
									{t("languageDescription") || "Select your preferred language"}
								</p>
							</div>
						</div>

						{hasChanges() && (
							<div className="mt-6 pt-6 border-t border-border">
								<div className="flex items-center justify-between">
									<div className="text-sm text-textMuted">
										{t("unsavedChanges") || "You have unsaved changes"}
									</div>
									<div className="flex gap-3">
										<button
											onClick={handleDiscardChanges}
											className="px-4 py-2 text-sm border border-border rounded-lg text-textMuted hover:text-text hover:border-text transition-colors"
											disabled={isSaving}
										>
											{t("discard") || "Discard"}
										</button>
										<button
											onClick={handleSaveChanges}
											disabled={isSaving}
											className="px-4 py-2 text-sm bg-primary text-white rounded-lg hover:bg-primary/90 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
										>
											{isSaving && (
												<svg
													className="w-4 h-4 animate-spin"
													fill="none"
													viewBox="0 0 24 24"
												>
													<circle
														className="opacity-25"
														cx="12"
														cy="12"
														r="10"
														stroke="currentColor"
														strokeWidth="4"
													/>
													<path
														className="opacity-75"
														fill="currentColor"
														d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
													/>
												</svg>
											)}
											{isSaving
												? t("saving") || "Saving..."
												: t("saveChanges") || "Save Changes"}
										</button>
									</div>
								</div>
							</div>
						)}
					</div>
				</div>
			</div>
		</div>
	);
};
