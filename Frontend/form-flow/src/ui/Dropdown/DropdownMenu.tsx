import React, { useEffect, useRef, useState } from "react";

export interface DropdownMenuItem {
	id: string;
	label: string;
	icon?: React.ReactNode;
	disabled?: boolean;
	variant?: "default" | "danger";
	onClick: () => void;
}

export interface DropdownMenuSeparator {
	type: "separator";
	id: string;
}

export type DropdownMenuItemType = DropdownMenuItem | DropdownMenuSeparator;

interface DropdownMenuProps {
	isOpen: boolean;
	onClose: () => void;
	trigger: React.ReactNode;
	items: DropdownMenuItemType[];
	align?: "left" | "right";
	className?: string;
}

export const DropdownMenu: React.FC<DropdownMenuProps> = ({
	isOpen,
	onClose,
	trigger,
	items,
	align = "right",
	className = "",
}) => {
	const [focusedIndex, setFocusedIndex] = useState(-1);
	const menuRef = useRef<HTMLDivElement>(null);
	const triggerRef = useRef<HTMLDivElement>(null);

	const menuItems = items.filter(
		(item) => !("type" in item)
	) as DropdownMenuItem[];

	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (
				isOpen &&
				menuRef.current &&
				triggerRef.current &&
				!menuRef.current.contains(event.target as Node) &&
				!triggerRef.current.contains(event.target as Node)
			) {
				onClose();
			}
		};

		const handleKeyDown = (event: KeyboardEvent) => {
			if (!isOpen) return;

			switch (event.key) {
				case "Escape":
					onClose();
					break;
				case "ArrowDown":
					event.preventDefault();
					setFocusedIndex((prev) =>
						prev < menuItems.length - 1 ? prev + 1 : 0
					);
					break;
				case "ArrowUp":
					event.preventDefault();
					setFocusedIndex((prev) =>
						prev > 0 ? prev - 1 : menuItems.length - 1
					);
					break;
				case "Enter":
					event.preventDefault();
					if (focusedIndex >= 0 && focusedIndex < menuItems.length) {
						const item = menuItems[focusedIndex];
						if (!item.disabled) {
							item.onClick();
							onClose();
						}
					}
					break;
			}
		};

		if (isOpen) {
			document.addEventListener("mousedown", handleClickOutside);
			document.addEventListener("keydown", handleKeyDown);
		}

		return () => {
			document.removeEventListener("mousedown", handleClickOutside);
			document.removeEventListener("keydown", handleKeyDown);
		};
	}, [isOpen, onClose, focusedIndex, menuItems]);

	useEffect(() => {
		if (isOpen) {
			setFocusedIndex(-1);
		}
	}, [isOpen]);

	const handleItemClick = (item: DropdownMenuItem) => {
		if (!item.disabled) {
			item.onClick();
			onClose();
		}
	};

	const getItemVariantClasses = (variant?: string) => {
		switch (variant) {
			case "danger":
				return "text-error hover:bg-error hover:text-white";
			default:
				return "text-text hover:bg-background";
		}
	};

	const alignmentClasses = align === "left" ? "left-0" : "right-0";

	return (
		<div className={`relative inline-block ${className}`}>
			<div ref={triggerRef}>{trigger}</div>

			{isOpen && (
				<div
					ref={menuRef}
					className={`absolute top-full mt-2 w-56 bg-surface border border-border rounded-lg shadow-lg z-50 py-1 ${alignmentClasses}`}
				>
					{items.map((item, index) => {
						if ("type" in item && item.type === "separator") {
							return <hr key={item.id} className="my-1 border-border" />;
						}

						const menuItem = item as DropdownMenuItem;
						const menuItemIndex = menuItems.indexOf(menuItem);
						const isFocused = menuItemIndex === focusedIndex;

						return (
							<button
								key={menuItem.id}
								onClick={() => handleItemClick(menuItem)}
								disabled={menuItem.disabled}
								className={`
                  w-full px-4 py-2 text-left text-sm flex items-center gap-3
                  transition-colors duration-150
                  ${getItemVariantClasses(menuItem.variant)}
                  ${isFocused ? "bg-background" : ""}
                  ${
										menuItem.disabled
											? "opacity-50 cursor-not-allowed"
											: "cursor-pointer"
									}
                `}
							>
								{menuItem.icon && (
									<span className="flex-shrink-0 w-4 h-4">{menuItem.icon}</span>
								)}
								<span>{menuItem.label}</span>
							</button>
						);
					})}
				</div>
			)}
		</div>
	);
};
