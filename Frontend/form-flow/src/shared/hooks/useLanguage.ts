import React, { useCallback } from "react";
import i18n from "../../config/i18n-config";

export interface Language {
    id: string;
    code: string;
    shortCode: string;
    name: string;
    iconURL?: string | null;
    region: string;
    isDefault: boolean;
    isActive: boolean;
}

export const useLanguage = (availableLanguages: Language[] = []) => {
    //const { i18n } = useTranslation();
    const [currentLanguage, setCurrentLanguage] = React.useState<Language | null>(null);
    const languageKey = 'formflow-language';

    const applyLanguage = useCallback((language: Language) => {
        document.documentElement.setAttribute('lang', language.shortCode);

        if (i18n.isInitialized && typeof i18n.changeLanguage === 'function') {
            i18n.changeLanguage(language.shortCode);
        }
        else {
            console.warn('i18n is not initialized or changeLanguage is not a function');
        }
        //i18n.changeLanguage(language.shortCode);
        localStorage.setItem(languageKey, JSON.stringify(language));
    }, []);

    React.useEffect(() => {
        if (availableLanguages.length === 0) return;

        const savedLanguage = localStorage.getItem(languageKey);
        if (savedLanguage) {
            try {
                const savedLanguageData = JSON.parse(savedLanguage);
                const language = availableLanguages.find(l => l.id === savedLanguageData.id);
                if (language) {
                    setCurrentLanguage(language);
                    applyLanguage(language);
                    return;
                }
            } catch (error) {
                console.error('Error parsing saved language:', error);
                localStorage.removeItem(languageKey);
            }
        }

        const defaultLanguage = availableLanguages.find(l => l.isDefault);
        if (defaultLanguage) {
            setCurrentLanguage(defaultLanguage);
            applyLanguage(defaultLanguage);
        }
    }, [availableLanguages, applyLanguage]);

    const setLanguage = (language: Language) => {
        setCurrentLanguage(language);
        applyLanguage(language);
    };

    const setNext = () => {
        if (!currentLanguage || availableLanguages.length === 0) return;
        
        const currentIndex = availableLanguages.findIndex(l => l.id === currentLanguage.id);
        const nextIndex = (currentIndex + 1) % availableLanguages.length;
        const nextLanguage = availableLanguages[nextIndex];
        
        setLanguage(nextLanguage);
    };

    return { 
        currentLanguage, 
        setLanguage,
        setNext
    }
}