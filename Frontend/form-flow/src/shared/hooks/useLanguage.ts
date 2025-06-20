import { useTranslation } from "react-i18next";
import { useAppStore } from "../../stores/appStore";
import { useEffect } from "react";

export const useLanguage = () => {
	const { i18n } = useTranslation();
	const { currentLanguage, setCurrentLanguage, languages } = useAppStore();

	useEffect(() => {
		if (currentLanguage) {
			if (i18n.language !== currentLanguage.shortCode)
				i18n.changeLanguage(currentLanguage.shortCode);
		}
	}, [currentLanguage, i18n]);

	const setLanguageById = (id: string) => {
		setCurrentLanguage(id);
	};

	const setLanguageByName = (name: string) => {
		const language = languages.find((l) => l.name === name);
		if (language) setCurrentLanguage(language.id);
	};

	return {
		currentLanguage,
		setLanguageById,
		setLanguageByName,
	};
};
