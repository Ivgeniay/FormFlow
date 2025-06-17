import React from 'react';
import { ColorTheme, useTheme } from './shared/hooks/useTheme';

interface ThemeToggleProps {
  availableThemes: ColorTheme[];
}

export const ThemeToggle: React.FC<ThemeToggleProps> = ({ availableThemes }) => {
  const { currentTheme, setNext } = useTheme(availableThemes);

  return (
    <button
      onClick={setNext}
      className="p-2 rounded-lg bg-gray-100 dark:bg-gray-800 hover:bg-gray-200 dark:hover:bg-gray-700 transition-colors"
      aria-label={`Switch theme. Current: ${currentTheme?.name || 'Unknown'}`}
    >
      <svg className="w-5 h-5 text-gray-700 dark:text-gray-300" fill="currentColor" viewBox="0 0 20 20">
        <path fillRule="evenodd" d="M4 2a2 2 0 00-2 2v11a2 2 0 002 2h2a2 2 0 002-2V4a2 2 0 00-2-2H4zm0 2h2v11H4V4zm6-2a2 2 0 00-2 2v11a2 2 0 002 2h2a2 2 0 002-2V4a2 2 0 00-2-2h-2zm0 2h2v11h-2V4zm6-2a2 2 0 00-2 2v11a2 2 0 002 2h2a2 2 0 002-2V4a2 2 0 00-2-2h-2zm0 2h2v11h-2V4z" clipRule="evenodd" />
      </svg>
    </button>
  );
};