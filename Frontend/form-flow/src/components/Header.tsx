import React from "react";
import { useTranslation } from "react-i18next";
import { ColorTheme } from "../shared/types/color-theme";
import { Language } from "../shared/types/language";
import { ThemeDropdown } from "./ThemeDropdown";
import { LanguageDropdown } from "./LanguageDropdown";
import { SearchInput } from "./SearchInput";

interface User {
	id: string;
	username: string;
	email: string;
	isAdmin: boolean;
}

interface HeaderProps {
	availableThemes: ColorTheme[];
	availableLanguages: Language[];
	user: User | null;
	isAuthenticated: boolean;
	onToggleNavMenu: () => void;
	onLogin: () => void;
	onLogout: () => void;
	onSearch: (query: string) => void;
}

export const Header: React.FC<HeaderProps> = ({
	availableThemes,
	availableLanguages,
	user,
	isAuthenticated,
	onToggleNavMenu,
	onLogin,
	onLogout,
	onSearch,
}) => {
	const { t } = useTranslation();

	const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		onSearch(e.target.value);
	};

	const handleUserMenuClick = () => {
		if (isAuthenticated) {
			console.log("User menu clicked");
		} else {
			onLogin();
		}
	};

	return (
		<header className="bg-surface border-b border-border px-4 py-1 sticky top-0 z-40">
			<div className="flex items-center justify-between">
				<div className="flex items-center gap-3">
					<button
						onClick={onToggleNavMenu}
						className="p-2 rounded-lg text-text hover:bg-background transition-colors"
						aria-label={t("toggleMenu") || "Toggle navigation menu"}
					>
						<svg
							className="w-6 h-6"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M4 6h16M4 12h16M4 18h16"
							/>
						</svg>
					</button>

					<div className="text-2xl font-bold text-primary cursor-pointer">
						{t("appName")}
					</div>
				</div>

				<div className="flex flex-1 max-w-md mx-4 gap-3 items-center">
					<SearchInput
						placeholder={t("searchPlaceholder")}
						placeholderDefault="Search..."
						handleSearchChange={handleSearchChange}
						className="relative w-full"
					/>
					<ThemeDropdown
						availableThemes={availableThemes}
						className="min-w-[120px] "
					/>
				</div>

				<div className="flex items-center gap-3">
					<LanguageDropdown
						availableLanguages={availableLanguages}
						className="min-w-[120px]"
					/>

					<div className="flex items-center gap-2 min-w-[240px] justify-end">
						{isAuthenticated ? (
							<>
								<span className="text-sm text-textMuted hidden md:inline truncate max-w-[180px]">
									{user?.username}
								</span>
								<button
									onClick={handleUserMenuClick}
									className="p-2 rounded-lg text-text hover:bg-background transition-colors flex-shrink-0"
									aria-label={t("userMenu") || "User menu"}
								>
									<svg
										className="w-6 h-6"
										fill="none"
										stroke="currentColor"
										viewBox="0 0 24 24"
									>
										<path
											strokeLinecap="round"
											strokeLinejoin="round"
											strokeWidth={2}
											d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
										/>
									</svg>
								</button>
							</>
						) : (
							<button
								onClick={onLogin}
								className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-colors min-w-[100px]"
							>
								{t("login")}
							</button>
						)}
					</div>
					{/* {isAuthenticated ? (
            <div className="flex items-center gap-2">
              <span className="text-sm text-textMuted hidden md:inline">
                {user?.username}
              </span>
              <button 
                onClick={handleUserMenuClick}
                className="p-2 rounded-lg text-text hover:bg-background transition-colors"
                aria-label={t('userMenu') || 'User menu'}
              >
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
              </button>
            </div>
          ) : (
            <button 
              onClick={onLogin}
              className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-colors"
            >
              {t('login')}
            </button>
          )} */}
				</div>
			</div>
		</header>
	);
};
