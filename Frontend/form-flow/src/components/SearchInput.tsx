import { useTranslation } from "react-i18next";

interface SearchInputProps {
    placeholder: string;
    placeholderDefault: string;
    className?: string;
    handleSearchChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

export const SearchInput = (
    { placeholder, placeholderDefault, handleSearchChange, className }: SearchInputProps
) => {
    const { t } = useTranslation();

    return (
        <div className={`${className}`}>
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <svg className="h-5 w-5 text-textMuted" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
            </div>
            <input
                type="text"
                placeholder={t(placeholder) || placeholderDefault}
                onChange={handleSearchChange}
                className="w-full pl-10 pr-4 py-1.5 bg-background border border-border rounded-lg text-text placeholder-textMuted focus:outline-none focus:ring-2 focus:ring-primary transition-colors"
            />
        </div>
    );
}