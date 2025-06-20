import { ColorTheme } from "../shared/types/color-theme";
import { Language } from "./../shared/types/language";
import { create } from "zustand";

interface AppStore {
	languages: Language[];
	currentLanguage: Language;
	getSaftyLanguages: () => Language[];
	setLanguages: (value: Language[]) => void;
	setCurrentLanguage: (id: string) => void;

	themes: ColorTheme[];
	currentTheme: ColorTheme;
	getSaftyThemes: () => ColorTheme[];
	setThemes: (value: ColorTheme[]) => void;
	setCurrentTheme: (id: string) => void;
}

const defaultTheme: ColorTheme = {
	id: "light",
	name: "Light",
	cssClass: "theme-light",
	colorVariables:
		'{"--primary-color": "#3b82f6", "--background-color": "#ffffff", "--surface-color": "#f8fafc", "--text-color": "#1e293b", "--text-muted-color": "#64748b", "--border-color": "#e2e8f0", "--success-color": "#10b981", "--warning-color": "#f59e0b", "--error-color": "#ef4444"}',
	isDefault: true,
	isActive: true,
};

const defaultLanguage: Language = {
	id: "en",
	code: "en-US",
	shortCode: "en",
	name: "English",
	iconURL: null,
	region: "United States",
	isDefault: true,
	isActive: true,
};

export const useAppStore = create<AppStore>()((set, get) => ({
	themes: [defaultTheme],
	languages: [defaultLanguage],
	currentLanguage: defaultLanguage,
	currentTheme: defaultTheme,

	setLanguages: (value) => {
		if (value.length === 0) set({ languages: [defaultLanguage] });
		else set({ languages: value });
	},
	getSaftyLanguages: () => {
		const languages = get().languages;
		return languages.length === 0 ? [defaultLanguage] : languages;
	},
	setCurrentLanguage: (id) => {
		const language = get().languages.find((l) => l.id === id);
		if (language) {
			set({ currentLanguage: language });
		}
	},

	setThemes: (value) => {
		if (value.length === 0) set({ themes: [defaultTheme] });
		else set({ themes: value });
	},
	getSaftyThemes: () => {
		const themes = get().themes;
		return themes.length === 0 ? [defaultTheme] : themes;
	},
	setCurrentTheme: (id) => {
		const theme = get().themes.find((t) => t.id === id);
		if (theme) {
			set({ currentTheme: theme });
		}
	},
}));
