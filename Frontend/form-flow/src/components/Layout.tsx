import { useState } from "react";
import { Header } from "./Header";
import { NavMenu } from "../modules/navMenu/NavMenu";
import { Language } from "../shared/types/language";
import { ColorTheme } from "../shared/types/color-theme";

interface User {
	id: string;
	username: string;
	email: string;
	isAdmin: boolean;
}

interface LayoutProps {
	children: React.ReactNode;
	availableThemes: ColorTheme[];
	availableLanguages: Language[];
}

export const Layout: React.FC<LayoutProps> = ({
	children,
	availableThemes,
	availableLanguages,
}) => {
	const [isNavMenuOpen, setIsNavMenuOpen] = useState(false);
	const [user, setUser] = useState<User | null>(null);
	const [isAuthenticated, setIsAuthenticated] = useState(false);

	const toggleNavMenu = () => {
		setIsNavMenuOpen(!isNavMenuOpen);
	};

	const closeNavMenu = () => {
		setIsNavMenuOpen(false);
	};

	const handleLogin = () => {
		console.log("Login clicked");
	};

	const handleLogout = () => {
		setUser(null);
		setIsAuthenticated(false);
	};

	const handleSearch = (query: string) => {
		console.log("Search query:", query);
	};

	const handleNavigate = (path: string) => {
		console.log("Navigate to:", path);
	};

	return (
		<div className="min-h-screen bg-background">
			<Header
				availableThemes={availableThemes}
				availableLanguages={availableLanguages}
				user={user}
				isAuthenticated={isAuthenticated}
				onToggleNavMenu={toggleNavMenu}
				onLogin={handleLogin}
				onLogout={handleLogout}
				onSearch={handleSearch}
			/>

			<NavMenu
				isOpen={isNavMenuOpen}
				isAuthenticated={isAuthenticated}
				user={user}
				onClose={closeNavMenu}
				onNavigate={handleNavigate}
			/>

			<main className="container mx-auto px-4 py-6">{children}</main>
		</div>
	);
};
