import { useTranslation } from "react-i18next";
import { Dropdown, DropdownItem } from "../ui/Dropdown/Dropdown";
import { useLanguage } from "../shared/hooks/useLanguage";
import { Language } from "../shared/types/language";
import { useAppStore } from "../stores/appStore";

interface LanguageDropdownProps {
	availableLanguages: Language[];
	className?: string;
}

export const LanguageDropdown: React.FC<LanguageDropdownProps> = ({
	availableLanguages,
	className,
}) => {
	const { t } = useTranslation();
	const { currentLanguage } = useAppStore();
	const { setLanguageById } = useLanguage();

	const languageItems: DropdownItem[] = availableLanguages.map((language) => ({
		id: language.id,
		label: language.name,
		value: language.shortCode,
		disabled: !language.isActive,
	}));

	const handleLanguageSelect = (item: DropdownItem) => {
		const selectedLanguage = availableLanguages.find(
			(language) => language.id === item.id
		);
		if (selectedLanguage) {
			setLanguageById(selectedLanguage.id);
		}
	};

	return (
		<Dropdown
			items={languageItems}
			selectedId={currentLanguage?.id}
			placeholder={t("language") || "Language"}
			onSelect={handleLanguageSelect}
			className={className}
		/>
	);
};
