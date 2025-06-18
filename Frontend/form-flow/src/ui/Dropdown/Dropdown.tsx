import React, { useState, useRef, useEffect } from 'react';

export interface DropdownItem {
    id: string;
    label: string;
    value: string;
    disabled?: boolean;
}

interface DropdownProps {
    items: DropdownItem[];
    selectedId?: string;
    placeholder?: string;
    disabled?: boolean;
    onSelect: (item: DropdownItem) => void;
    className?: string;
}

export const Dropdown: React.FC<DropdownProps> = ({
    items,
    selectedId,
    placeholder = 'Select...',
    disabled = false,
    onSelect,
    className
}) => {
    const [isOpen, setIsOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    const selectedItem = items.find(item => item.id === selectedId);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
        if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
            setIsOpen(false);
        }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleItemClick = (item: DropdownItem) => {
        if (item.disabled) return;
        onSelect(item);
        setIsOpen(false);
    };

    const toggleDropdown = () => {
        if (!disabled) {
        setIsOpen(!isOpen);
        }
    };

    return (
        <div ref={dropdownRef} className={`relative inline-block text-left ${className}`}>
            <button type="button" onClick={toggleDropdown} disabled={disabled} className={`
                inline-flex items-center justify-between w-full px-4 py-2 text-sm font-medium bg-surface text-text border border-border rounded-lg hover:bg-background focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 transition-colors duration-200
                ${disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'}`}>
                <span className={selectedItem ? 'text-text' : 'text-textMuted'}>
                {selectedItem ? selectedItem.label : placeholder}
                </span>
                <svg className={`ml-2 h-4 w-4 transition-transform duration-200 ${isOpen ? 'rotate-180' : ''}`}
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                </svg>
            </button>

            {isOpen && (
                <div className="absolute right-0 z-10 mt-2 w-full bg-surface border border-border rounded-lg shadow-lg">
                <div className="py-1">
                    {items.map((item) => (
                    <button
                        key={item.id}
                        onClick={() => handleItemClick(item)}
                        disabled={item.disabled}
                        className={`
                        w-full text-left px-4 py-2 text-sm transition-colors duration-150
                        ${item.disabled 
                            ? 'text-textMuted cursor-not-allowed opacity-50' 
                            : 'text-text hover:bg-background cursor-pointer'
                        }
                        ${selectedId === item.id ? 'bg-primary text-white' : ''}
                        `}>
                        {item.label}
                    </button>
                    ))}
                </div>
                </div>
            )}
        </div>
    );
};