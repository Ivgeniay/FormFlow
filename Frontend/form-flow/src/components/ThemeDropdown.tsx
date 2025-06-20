import React from "react";
import { useTranslation } from "react-i18next";
import { Dropdown, DropdownItem } from "../ui/Dropdown/Dropdown";
import { useTheme } from "../shared/hooks/useTheme";
import { ColorTheme } from "../shared/types/color-theme";
import { useAppStore } from "../stores/appStore";

interface ThemeDropdownProps {
	availableThemes: ColorTheme[];
	className?: string;
}

export const ThemeDropdown: React.FC<ThemeDropdownProps> = ({
	availableThemes,
	className,
}) => {
	const { t } = useTranslation();
	const { currentTheme } = useAppStore();
	const { setThemeById } = useTheme();

	const themeItems: DropdownItem[] = availableThemes.map((theme) => ({
		id: theme.id,
		label: t(theme.name.toLowerCase()),
		value: theme.name.toLowerCase(),
		disabled: !theme.isActive,
	}));

	const handleThemeSelect = (item: DropdownItem) => {
		const selectedTheme = availableThemes.find((theme) => theme.id === item.id);
		if (selectedTheme) {
			setThemeById(selectedTheme.id);
		}
	};

	return (
		<Dropdown
			items={themeItems}
			selectedId={currentTheme?.id}
			placeholder={t("theme") || "Theme"}
			onSelect={handleThemeSelect}
			className={className}
		/>
	);
};
