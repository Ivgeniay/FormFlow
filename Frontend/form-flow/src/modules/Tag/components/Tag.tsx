import React from 'react';

export interface TagProps {
	name: string;
	count?: number;
	size?: 'sm' | 'md' | 'lg';
	variant?: 'default' | 'outlined' | 'filled';
	clickable?: boolean;
	onClick?: (name: string) => void;
	className?: string;
}

export const Tag: React.FC<TagProps> = ({
	name,
	count,
	size = 'md',
	variant = 'default',
	clickable = true,
	onClick,
	className = ''
}) => {
	const handleClick = () => {
		if (clickable && onClick) {
			onClick(name);
		}
	};

	const sizeClasses = {
		sm: 'px-2 py-1 text-xs',
		md: 'px-3 py-1.5 text-sm',
		lg: 'px-4 py-2 text-base'
	};

	const variantClasses = {
		default: 'bg-surface border border-border text-text hover:bg-background',
		outlined: 'bg-transparent border border-primary text-primary hover:bg-primary hover:text-white',
		filled: 'bg-primary text-white hover:opacity-90'
	};

	const baseClasses = `
		inline-flex items-center gap-1.5 rounded-full font-medium transition-all duration-200
		${sizeClasses[size]}
		${variantClasses[variant]}
		${clickable ? 'cursor-pointer select-none' : 'cursor-default'}
		${className}
	`.trim().replace(/\s+/g, ' ');

	return (
		<span
			className={baseClasses}
			onClick={handleClick}
			role={clickable ? 'button' : undefined}
			tabIndex={clickable ? 0 : undefined}
			onKeyDown={clickable ? (e) => {
				if (e.key === 'Enter' || e.key === ' ') {
					e.preventDefault();
					handleClick();
				}
			} : undefined}
		>
			<span>{name}</span>
			{count !== undefined && (
				<span className="opacity-75 font-normal">
					{count}
				</span>
			)}
		</span>
	);
};