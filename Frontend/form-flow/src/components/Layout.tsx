import { useState } from "react";
import { Header } from "./Header";
import { NavMenu } from "../modules/navMenu/NavMenu";
import { useAuth } from "../modules/auth/hooks/useAuth";
import { useNavigate } from "react-router-dom";

interface LayoutProps {
	children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
	const [isNavMenuOpen, setIsNavMenuOpen] = useState(false);
	const navigate = useNavigate();
	const { user, isAuthenticated, logout } = useAuth();

	const toggleNavMenu = () => {
		setIsNavMenuOpen(!isNavMenuOpen);
	};

	const closeNavMenu = () => {
		setIsNavMenuOpen(false);
	};

	const handleLogin = () => {
		navigate("/login");
	};

	const handleRegister = () => {
		navigate("/register");
	};

	const handleLogout = async () => {
		try {
			await logout();
		} catch (error) {
			console.error("Logout failed:", error);
		}
	};

	const handleSearch = (query: string) => {
		console.log("Search query:", query);
	};

	const handleNavigate = (path: string) => {
		navigate(path);
		closeNavMenu();
	};

	return (
		<div className="min-h-screen bg-background">
			<Header
				user={user}
				isAuthenticated={isAuthenticated}
				onToggleNavMenu={toggleNavMenu}
				onLogin={handleLogin}
				onRegister={handleRegister}
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
