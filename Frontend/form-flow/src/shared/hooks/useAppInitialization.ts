import { useState, useEffect } from "react";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { useTheme } from "./useTheme";
import { useLanguage } from "./useLanguage";
import { userSettingsApi } from "../../api/user-settings";
import { useAppStore } from "../../stores/appStore";
import i18n from "../../config/i18n";

export const useAppInitialization = () => {
	const [isInitialized, setIsInitialized] = useState(false);
	const [isLoading, setIsLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	const { setThemes, setLanguages, getSaftyThemes, getSaftyLanguages } =
		useAppStore();
	const { refreshTokens, clearAuth, isAuthenticated, accessToken } = useAuth();
	const { setThemeById } = useTheme();
	const { setLanguageById } = useLanguage();

	useEffect(() => {
		initializeApp();
	}, []);

	const initializeApp = async () => {
		try {
			setIsLoading(true);
			setError(null);

			await initializeI18n();
			await loadAppData();
			await checkAuthentication();
			await loadUserSettings();

			setIsInitialized(true);
		} catch (error) {
			const errorMessage =
				error instanceof Error
					? error.message
					: "Application initialization failed";
			setError(errorMessage);
		} finally {
			setIsLoading(false);
		}
	};

	const initializeI18n = async () => {
		console.log("IS INITIALIZED: }", i18n.isInitialized);
		if (!i18n.isInitialized) await i18n.init();
	};

	const loadAppData = async () => {
		const [themes, languages] = await Promise.all([
			userSettingsApi.getAvailableColorThemes(),
			userSettingsApi.getAvailableLanguages(),
		]);

		setThemes(themes);
		setLanguages(languages);
	};

	const checkAuthentication = async () => {
		if (!isAuthenticated || !accessToken) {
			return;
		}

		try {
			await refreshTokens();
		} catch (error) {
			clearAuth();
		}
	};

	const loadUserSettings = async () => {
		if (!isAuthenticated || !accessToken) {
			const theme = getSaftyThemes().find((t) => t.isDefault);
			if (theme) setThemeById(theme?.id);
			const language = getSaftyLanguages().find((l) => l.isDefault);
			if (language) setLanguageById(language.id);
			return;
		}

		try {
			const userSettings = await userSettingsApi.getMySettings(accessToken);

			setThemeById(userSettings.colorThemeId);
			setLanguageById(userSettings.languageId);
		} catch (error) {}
	};

	return {
		isInitialized,
		isLoading,
		error,
		retry: initializeApp,
	};
};
