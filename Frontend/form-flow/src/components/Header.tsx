import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { SearchInput } from "./SearchInput";
import { UserDto } from "../shared/api_types";
import {
	DropdownMenu,
	DropdownMenuItemType,
} from "../ui/Dropdown/DropdownMenu";

interface HeaderProps {
	user: UserDto | null;
	isAuthenticated: boolean;
	onToggleNavMenu: () => void;
	onLogin: () => void;
	onRegister: () => void;
	onLogout: () => void;
	onSearch: (query: string) => void;
}

export const Header: React.FC<HeaderProps> = ({
	user,
	isAuthenticated,
	onToggleNavMenu,
	onLogin,
	onRegister,
	onLogout,
	onSearch,
}) => {
	const { t } = useTranslation();
	const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
	const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		onSearch(e.target.value);
	};

	const handleUserMenuClick = () => {
		setIsUserMenuOpen(!isUserMenuOpen);
	};

	const closeUserMenu = () => {
		setIsUserMenuOpen(false);
	};

	const userMenuItems: DropdownMenuItemType[] = [
		{
			id: "profile",
			label: t("profile") || "Profile",
			icon: (
				<svg
					className="w-4 h-4"
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
			),
			onClick: () => console.log("Navigate to profile"),
		},
		{
			id: "settings",
			label: t("settings") || "Settings",
			icon: (
				<svg
					className="w-4 h-4"
					fill="none"
					stroke="currentColor"
					viewBox="0 0 24 24"
				>
					<path
						strokeLinecap="round"
						strokeLinejoin="round"
						strokeWidth={2}
						d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
					/>
					<path
						strokeLinecap="round"
						strokeLinejoin="round"
						strokeWidth={2}
						d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
					/>
				</svg>
			),
			onClick: () => console.log("Navigate to settings"),
		},
		{
			type: "separator",
			id: "sep1",
		},
		{
			id: "logout",
			label: t("logout") || "Logout",
			variant: "danger",
			icon: (
				<svg
					className="w-4 h-4"
					fill="none"
					stroke="currentColor"
					viewBox="0 0 24 24"
				>
					<path
						strokeLinecap="round"
						strokeLinejoin="round"
						strokeWidth={2}
						d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
					/>
				</svg>
			),
			onClick: onLogout,
		},
	];

	return (
		<header className="bg-surface border-b border-border px-4 py-1 sticky top-0 z-40">
			<div className="flex items-center justify-between">
				<div className="flex items-center gap-3">
					<button
						onClick={onToggleNavMenu}
						className="p-2 rounded-lg text-text hover:bg-background transition-colors"
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
					{/* <ThemeDropdown availableThemes={themes} className="min-w-[120px] " /> */}
				</div>

				<div className="flex items-center gap-3">
					{/* <LanguageDropdown
						availableLanguages={languages}
						className="min-w-[120px]"
					/> */}

					<div className="flex items-center gap-2 min-w-[240px] justify-end">
						{isAuthenticated ? (
							<>
								<span className="text-sm text-textMuted hidden md:inline truncate max-w-[180px]">
									{user?.userName}
								</span>
								<DropdownMenu
									isOpen={isUserMenuOpen}
									onClose={closeUserMenu}
									items={userMenuItems}
									align="right"
									trigger={
										<button
											onClick={handleUserMenuClick}
											className="p-2 rounded-lg text-text hover:bg-background transition-colors flex-shrink-0"
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
									}
								/>
							</>
						) : (
							<div>
								<button
									onClick={onLogin}
									className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-colors min-w-[100px]"
								>
									{t("login")}
								</button>
								<button
									onClick={onRegister}
									className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-colors min-w-[100px]"
								>
									{t("register")}
								</button>
							</div>
						)}
					</div>
				</div>
			</div>
		</header>
	);
};
