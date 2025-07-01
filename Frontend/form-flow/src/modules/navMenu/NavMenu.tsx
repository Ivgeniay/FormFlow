import { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";
import { NavMenuButton } from "./components/NavMenuButton";
import { UserDto } from "../../shared/api_types";
import { useAuth } from "../auth/hooks/useAuth";

interface NavMenuProps {
	isOpen: boolean;
	isAuthenticated: boolean;
	user: UserDto | null;
	onClose: () => void;
	onNavigate: (path: string) => void;
}

export const NavMenu: React.FC<NavMenuProps> = ({
	isOpen,
	isAuthenticated,
	user,
	onClose,
	onNavigate,
}) => {
	const { t } = useTranslation();
	const navRef = useRef<HTMLElement>(null);
	const { isAdmin } = useAuth();

	useEffect(() => {
		const handlerClickOutside = (e: MouseEvent) => {
			if (
				isOpen &&
				navRef.current &&
				!navRef.current.contains(e.target as Node)
			)
				onClose();
		};

		if (isOpen) document.addEventListener("mousedown", handlerClickOutside);

		return () => {
			document.removeEventListener("mousedown", handlerClickOutside);
		};
	}, [isOpen, onClose]);

	const handleNavigate = (path: string) => {
		onNavigate(path);
		onClose();
	};

	const homeIcon = (
		<svg
			className="w-5 h-5"
			fill="none"
			stroke="currentColor"
			viewBox="0 0 24 24"
		>
			<path
				strokeLinecap="round"
				strokeLinejoin="round"
				strokeWidth={2}
				d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"
			/>
		</svg>
	);

	const profileIcon = (
		<svg
			className="w-5 h-5"
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
	);

	const searchIcon = (
		<svg
			className="w-5 h-5"
			fill="none"
			stroke="currentColor"
			viewBox="0 0 24 24"
		>
			<path
				strokeLinecap="round"
				strokeLinejoin="round"
				strokeWidth={2}
				d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z"
			/>
		</svg>
	);

	const templatesIcon = (
		<svg
			className="w-5 h-5"
			fill="none"
			stroke="currentColor"
			viewBox="0 0 24 24"
		>
			<path
				strokeLinecap="round"
				strokeLinejoin="round"
				strokeWidth={2}
				d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
			/>
		</svg>
	);

	const settingsIcon = (
		<svg
			xmlns="http://www.w3.org/2000/svg"
			fill="none"
			viewBox="0 0 24 24"
			stroke-width="1.5"
			stroke="currentColor"
			className="size-6"
		>
			<path
				stroke-linecap="round"
				stroke-linejoin="round"
				d="M9.594 3.94c.09-.542.56-.94 1.11-.94h2.593c.55 0 1.02.398 1.11.94l.213 1.281c.063.374.313.686.645.87.074.04.147.083.22.127.325.196.72.257 1.075.124l1.217-.456a1.125 1.125 0 0 1 1.37.49l1.296 2.247a1.125 1.125 0 0 1-.26 1.431l-1.003.827c-.293.241-.438.613-.43.992a7.723 7.723 0 0 1 0 .255c-.008.378.137.75.43.991l1.004.827c.424.35.534.955.26 1.43l-1.298 2.247a1.125 1.125 0 0 1-1.369.491l-1.217-.456c-.355-.133-.75-.072-1.076.124a6.47 6.47 0 0 1-.22.128c-.331.183-.581.495-.644.869l-.213 1.281c-.09.543-.56.94-1.11.94h-2.594c-.55 0-1.019-.398-1.11-.94l-.213-1.281c-.062-.374-.312-.686-.644-.87a6.52 6.52 0 0 1-.22-.127c-.325-.196-.72-.257-1.076-.124l-1.217.456a1.125 1.125 0 0 1-1.369-.49l-1.297-2.247a1.125 1.125 0 0 1 .26-1.431l1.004-.827c.292-.24.437-.613.43-.991a6.932 6.932 0 0 1 0-.255c.007-.38-.138-.751-.43-.992l-1.004-.827a1.125 1.125 0 0 1-.26-1.43l1.297-2.247a1.125 1.125 0 0 1 1.37-.491l1.216.456c.356.133.751.072 1.076-.124.072-.044.146-.086.22-.128.332-.183.582-.495.644-.869l.214-1.28Z"
			/>
			<path
				stroke-linecap="round"
				stroke-linejoin="round"
				d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"
			/>
		</svg>
	);

	const adminIcon = (
		<svg
			className="w-5 h-5"
			fill="none"
			stroke="currentColor"
			viewBox="0 0 24 24"
		>
			<path
				strokeLinecap="round"
				strokeLinejoin="round"
				strokeWidth={2}
				d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"
			/>
		</svg>
	);

	return (
		<nav
			ref={navRef}
			className={`fixed top-0 left-0 z-40 w-60 max-w-sm h-full bg-surface border-r border-border shadow-lg 
      transform transition-transform duration-200 ease-in-out ${
				isOpen ? "translate-x-0" : "-translate-x-full"
			}`}
		>
			<div className="flex flex-col h-full">
				<div className="flex items-center justify-between p-4 border-b border-border">
					<h2 className="text-lg font-semibold text-text">{t("appName")}</h2>
					<button
						onClick={onClose}
						className="p-2 rounded-lg text-text hover:bg-background transition-colors"
					>
						<svg
							className="w-5 h-5"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M6 18L18 6M6 6l12 12"
							/>
						</svg>
					</button>
				</div>

				{isAuthenticated && user && (
					<div className="p-4 border-b border-border bg-background">
						<div className="flex items-center gap-3">
							<div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center text-white font-semibold">
								{user.userName.charAt(0).toUpperCase()}
							</div>
							<div>
								<p className="font-medium text-text">{user.userName}</p>
								{/* <p className="text-sm text-textMuted">{user.contacts}</p> */}
							</div>
						</div>
					</div>
				)}

				<div className="flex-1 overflow-y-auto p-4">
					<ul className="space-y-2">
						<NavMenuButton
							icon={homeIcon}
							label={t("home") || "Home"}
							onClick={() => handleNavigate("/")}
						/>

						<NavMenuButton
							icon={searchIcon}
							label={t("search") || "Search"}
							onClick={() => handleNavigate("/search")}
						/>

						<NavMenuButton
							icon={profileIcon}
							label={t("profile") || "Profile"}
							onClick={() => handleNavigate("/profile")}
							isVisible={isAuthenticated}
						/>

						<NavMenuButton
							icon={templatesIcon}
							label={t("myDashboard") || "My Dashboard"}
							onClick={() => handleNavigate("/mydashboard")}
							isVisible={isAuthenticated}
						/>

						<NavMenuButton
							icon={settingsIcon}
							label={t("settings") || "Settings"}
							onClick={() => handleNavigate("/settings")}
							isVisible={isAuthenticated}
						/>

						<NavMenuButton
							icon={adminIcon}
							label={t("adminPanel") || "Admin Panel"}
							onClick={() => handleNavigate("/admin_panel")}
							isVisible={isAuthenticated && isAdmin}
						/>
					</ul>
				</div>

				<div className="p-4 border-t border-border">
					<p className="text-xs text-textMuted text-center">
						{t("appName") || "FormFlow"} v1.0
					</p>
				</div>
			</div>
		</nav>
	);
};
