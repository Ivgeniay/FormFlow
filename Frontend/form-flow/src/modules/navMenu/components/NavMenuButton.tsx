import React from 'react';

interface NavMenuButtonProps {
  icon: React.ReactNode;
  label: string;
  onClick: () => void;
  isVisible?: boolean;
  className?: string;
}

export const NavMenuButton: React.FC<NavMenuButtonProps> = ({
  icon,
  label,
  onClick,
  isVisible = true,
  className = ""
}) => {
  if (!isVisible) return null;

  return (
    <li>
      <button
        onClick={onClick}
        className={`w-full flex items-center gap-3 px-4 py-3 text-left text-text hover:bg-background rounded-lg transition-colors ${className}`}
      >
        {icon}
        {label}
      </button>
    </li>
  );
};