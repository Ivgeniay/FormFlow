import React from 'react';
import { AppLoader } from './components/AppLoader';
import { ThemeToggle } from './ThemeToggle';
import { ColorTheme } from './shared/hooks/useTheme';
import { Language } from './shared/hooks/useLanguage';
import { ThemeDropdown } from './components/ThemeDropdown';
import { LanguageDropdown } from './components/LanguageDropdown';

function App() {

  const themes: ColorTheme[] = [
    {
      id: 'light',
      name: 'Light',
      cssClass: 'theme-light',
      colorVariables: '{"--primary-color": "#3b82f6", "--background-color": "#ffffff", "--surface-color": "#f8fafc", "--text-color": "#1e293b", "--text-muted-color": "#64748b", "--border-color": "#e2e8f0", "--success-color": "#10b981", "--warning-color": "#f59e0b", "--error-color": "#ef4444"}',
      isDefault: true,
      isActive: true
    },
    {
      id: 'dark',
      name: 'Dark',
      cssClass: 'theme-dark',
      colorVariables: '{"--primary-color": "#60a5fa", "--background-color": "#0f172a", "--surface-color": "#1e293b", "--text-color": "#f1f5f9", "--text-muted-color": "#94a3b8", "--border-color": "#334155", "--success-color": "#34d399", "--warning-color": "#fbbf24", "--error-color": "#f87171"}',
      isDefault: false,
      isActive: true
    },
    {
      id: 'purple',
      name: 'Purple',
      cssClass: 'theme-purple',
      colorVariables: '{"--primary-color": "#8b5cf6", "--background-color": "#faf5ff", "--surface-color": "#f3e8ff", "--text-color": "#581c87", "--text-muted-color": "#7c3aed", "--border-color": "#ddd6fe", "--success-color": "#10b981", "--warning-color": "#f59e0b", "--error-color": "#ef4444"}',
      isDefault: false,
      isActive: true
    },
    {
      id: 'orange',
      name: 'Orange',
      cssClass: 'theme-orange',
      colorVariables: '{"--primary-color": "#ea580c", "--background-color": "#fff7ed", "--surface-color": "#fed7aa", "--text-color": "#9a3412", "--text-muted-color": "#c2410c", "--border-color": "#fdba74", "--success-color": "#10b981", "--warning-color": "#f59e0b", "--error-color": "#ef4444"}',
      isDefault: false,
      isActive: true
    },
    {
      id: 'pink',
      name: 'Pink',
      cssClass: 'theme-pink',
      colorVariables: '{"--primary-color": "#ec4899", "--background-color": "#fdf2f8", "--surface-color": "#fce7f3", "--text-color": "#831843", "--text-muted-color": "#be185d", "--border-color": "#f9a8d4", "--success-color": "#10b981", "--warning-color": "#f59e0b", "--error-color": "#ef4444"}',
      isDefault: false,
      isActive: true
    }
  ];

  const languages: Language[] = [
    {
      id: 'en',
      code: 'en-US',
      shortCode: 'en',
      name: 'English',
      iconURL: null,
      region: 'United States',
      isDefault: true,
      isActive: true
    },
    {
      id: 'ru',
      code: 'ru-RU', 
      shortCode: 'ru',
      name: 'Русский',
      iconURL: null,
      region: 'Russia',
      isDefault: false,
      isActive: true
    },
    {
      id: 'es',
      code: 'es-ES',
      shortCode: 'es', 
      name: 'Español',
      iconURL: null,
      region: 'Spain',
      isDefault: false,
      isActive: false
    }
  ];

  return (
    <>
      <div className="p-5 bg-background text-text min-h-screen">
      <div className="flex gap-4 mb-8">
        <ThemeDropdown availableThemes={themes} />
        <LanguageDropdown availableLanguages={languages} />
        <ThemeToggle availableThemes={themes} />
      </div>
      
      <AppLoader onRetry={() => console.log("Retry")}/> 
    </div>
    </>
  );
}

export default App;
