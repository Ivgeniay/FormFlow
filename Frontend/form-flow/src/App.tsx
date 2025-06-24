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
import { HomePage } from "./pages/homePage/HomePage";
import { useAppInitialization } from "./shared/hooks/useAppInitialization";
import { AppLoader } from "./components/AppLoader";
import { I18nextProvider } from "react-i18next";
import i18n from "./config/i18n";
import { UserSettings } from "./pages/settings/UserSettings";
import { TemplatePage } from "./pages/templates/TemplatePage";
import { Toaster } from "react-hot-toast";
import { SearchPage } from "./pages/search/SearchPage";

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
	const { isInitialized, isLoading, error } = useAppInitialization();
	if (isLoading) return <AppLoader isVisible={!isInitialized} />;
	if (error) {
		return <div>Error {error}</div>;
	}

	return (
		<>
			<I18nextProvider i18n={i18n}>
				<Router>
					<Routes>
						<Route
							path="/login"
							element={
								<AuthOnlyRoute>
									<LoginPage
										onSwitchToRegister={() =>
											(window.location.href = "/register")
										}
									/>
								</AuthOnlyRoute>
							}
						/>
						<Route
							path="/search"
							element={
								<Layout>
									<SearchPage />
								</Layout>
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
						<Route
							path="/template"
							element={
								<Layout>
									<TemplatePage />
								</Layout>
							}
						/>
						<Route
							path="/template/from/:sourceId"
							element={
								<Layout>
									<TemplatePage />
								</Layout>
							}
						/>
						<Route
							path="/template/:id"
							element={
								<Layout>
									<TemplatePage />
								</Layout>
							}
						/>
						{/* <Route path="/form/:id" element={<FormPage />} /> */}
						<Route path="/settings" element={<UserSettings />} />
						<Route
							path="/auth/google/callback"
							element={<GoogleCallbackPage />}
						/>

						<Route
							path="/"
							element={
								<Layout>
									<HomePage />
								</Layout>
							}
						/>

						<Route path="*" element={<Navigate to="/" replace />} />
					</Routes>
				</Router>
			</I18nextProvider>
			<Toaster
				position="top-right"
				toastOptions={{
					duration: 4000,
				}}
			/>
		</>
	);
}

export default App;
