import React, { useEffect } from "react";

export interface ColorTheme {
    id : string;
    name : string;
    cssClass : string;
    primaryColor : string;
    isDefault : boolean;
    isActive : boolean;
}

export const useTheme = (availableThemes: ColorTheme[] = []) => {
    const [currentTheme, setCurrentTheme] = React.useState<ColorTheme | null>(null);
    const themeName = 'formflow-theme';


    useEffect(() => {
        if (availableThemes.length === 0) return;

        const savedTheme = localStorage.getItem(themeName);
        if (savedTheme) {
            const savedThemeData = JSON.parse(savedTheme);
            const theme = availableThemes.find(t => t.id === savedThemeData.id);
            if (theme) {
                setCurrentTheme(theme);
                applyTheme(theme);
                return;
            }
        }

        const defaultTheme = availableThemes.find(t => t.isDefault);
        if (defaultTheme) {
            setCurrentTheme(defaultTheme);
            applyTheme(defaultTheme);
        }
    }, [availableThemes]);

    const applyTheme = (theme: ColorTheme) => {
        document.documentElement.className = theme.cssClass;
        document.documentElement.style.setProperty('--primary-color', theme.primaryColor);
        localStorage.setItem('formflow-theme', JSON.stringify(theme));
    }
    
    const setTheme = (theme: ColorTheme) => {
        setCurrentTheme(theme);
        applyTheme(theme);
        localStorage.setItem('formflow-theme', JSON.stringify(theme));
    }

    return {
        currentTheme,
        setTheme,
        primaryColor: currentTheme ? currentTheme.primaryColor : '#6f42c1',
    };
};


