import React from 'react';
import { useTranslation } from 'react-i18next';
import { ColorTheme } from '../shared/hooks/useTheme';
import { Language } from '../shared/hooks/useLanguage';
import { ThemeDropdown } from './ThemeDropdown';
import { LanguageDropdown } from './LanguageDropdown';

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
  onSearch
}) => {
  const { t } = useTranslation();

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onSearch(e.target.value);
  };

  const handleUserMenuClick = () => {
    if (isAuthenticated) {
      console.log('User menu clicked');
    } else {
      onLogin();
    }
  };

  return (
    <header className="bg-surface border-b border-border px-4 py-3 sticky top-0 z-40">
      <div className="flex items-center justify-between">

        <button
          onClick={onToggleNavMenu}
          className="p-2 rounded-lg text-text hover:bg-background transition-colors"
          aria-label={t('toggleMenu') || 'Toggle navigation menu'}
        >
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>

        <div className="text-2xl font-bold text-primary cursor-pointer">
          {t('appName')}
        </div>

        <div className="flex-1 max-w-md mx-4">
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg className="h-5 w-5 text-textMuted" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
            <input
              type="text"
              placeholder={t('searchPlaceholder') || 'Search...'}
              onChange={handleSearchChange}
              className="w-full pl-10 pr-4 py-2 bg-background border border-border rounded-lg text-text placeholder-textMuted focus:outline-none focus:ring-2 focus:ring-primary transition-colors"
            />
          </div>
        </div>


        <div className="flex items-center gap-3">

          <ThemeDropdown availableThemes={availableThemes} className="min-w-[120px]" />
          
          <LanguageDropdown availableLanguages={availableLanguages} className="min-w-[120px]" />

          {isAuthenticated ? (
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
          )}
        </div>
      </div>
    </header>
  );
};