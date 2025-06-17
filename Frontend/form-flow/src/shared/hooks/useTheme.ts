import React, { useCallback, useEffect } from "react";

export interface ColorTheme {
    id: string;
    name: string;
    cssClass: string;
    colorVariables: string;
    isDefault: boolean;
    isActive: boolean;
}


export const useTheme = (availableThemes: ColorTheme[] = []) => {
    const [currentTheme, setCurrentTheme] = React.useState<ColorTheme | null>(null);
    const themeName = 'formflow-theme';

    const applyTheme = useCallback((theme: ColorTheme) => {
        document.documentElement.className = theme.cssClass;
        
        availableThemes.forEach(t => {
            document.documentElement.classList.remove(t.cssClass);
        });
            
        try {
            const colorVariables = JSON.parse(theme.colorVariables);
            
            document.documentElement.classList.add(theme.cssClass);
            Object.entries(colorVariables).forEach(([key, value]) => {
                document.documentElement.style.setProperty(key, value as string);
            });
        } catch (error) {
            console.error('Error parsing color variables:', error);
        }
        
        localStorage.setItem(themeName, JSON.stringify(theme));
    }, [availableThemes]);

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
    }, [availableThemes, applyTheme]);
    
    const setTheme = (theme: ColorTheme) => {
        setCurrentTheme(theme);
        applyTheme(theme);
    }

    const setNext = () => {
    if (!currentTheme || availableThemes.length === 0) return;
    
    const currentIndex = availableThemes.findIndex(theme => theme.id === currentTheme.id);
    const nextIndex = (currentIndex + 1) % availableThemes.length;
    const nextTheme = availableThemes[nextIndex];
    setTheme(nextTheme);
};

    return {
        currentTheme,
        setTheme,
        setNext
    };
};
