import React from "react";
import {
	BrowserRouter as Router,
	Routes,
	Route,
	Navigate,
} from "react-router-dom";
import { Layout } from "./components/Layout";
import { LoginPage } from "./pages/auth/LoginPage";
import { RegisterPage } from "./pages/auth/RegisterPage";
import { GoogleCallbackPage } from "./pages/auth/GoogleCallbackPage";
import { useAuth } from "./modules/auth/hooks/useAuth";
import { mockThemes, mockLanguages } from "./shared/mock_data";
import { HomePage } from "./pages/homePage/HomePage";

const AuthOnlyRoute: React.FC<{ children: React.ReactNode }> = ({
	children,
}) => {
	const { isAuthenticated } = useAuth();

	if (isAuthenticated) {
		return <Navigate to="/" replace />;
	}
	return <>{children}</>;
};

function App() {
	return (
		<Router>
			<Routes>
				<Route
					path="/login"
					element={
						<AuthOnlyRoute>
							<LoginPage
								onSwitchToRegister={() => (window.location.href = "/register")}
							/>
						</AuthOnlyRoute>
					}
				/>
				<Route
					path="/register"
					element={
						<AuthOnlyRoute>
							<RegisterPage
								onSwitchToLogin={() => (window.location.href = "/login")}
							/>
						</AuthOnlyRoute>
					}
				/>

				<Route path="/auth/google/callback" element={<GoogleCallbackPage />} />

				<Route
					path="/"
					element={
						<Layout availableLanguages={mockLanguages} availableThemes={mockThemes}>
							<HomePage />
						</Layout>
					}
				/>

				<Route path="*" element={<Navigate to="/" replace />} />
			</Routes>
		</Router>
	);
}

export default App;
