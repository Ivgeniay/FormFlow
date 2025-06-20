import { useEffect } from "react";
import { useAppStore } from "../../stores/appStore";

export const useTheme = () => {
	const { currentTheme, setCurrentTheme, themes } = useAppStore();

	useEffect(() => {
		if (currentTheme) {
			applyTheme(currentTheme);
		}
	}, [currentTheme]);

	const applyTheme = (theme: any) => {
		const root = document.documentElement;

		if (theme.colorVariables) {
			const variables = JSON.parse(theme.colorVariables);
			Object.entries(variables).forEach(([key, value]) => {
				root.style.setProperty(key, value as string);
			});
		}

		if (theme.cssClass) {
			document.body.className = theme.cssClass;
		}
	};

	const setThemeById = (id: string) => {
		setCurrentTheme(id);
	};

	const setThemeByName = (name: string) => {
		const theme = themes.find((t) => t.name === name);
		if (theme) setCurrentTheme(theme.id);
	};

	return {
		currentTheme,
		setThemeById,
		setThemeByName,
	};
};
