import React from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { LoginRequest } from "../types/authTypes";
import { useAuth } from "../hooks/useAuth";

interface LoginFormProps {
	onSwitchToRegister?: () => void;
	className?: string;
}

export const LoginForm: React.FC<LoginFormProps> = ({
	onSwitchToRegister,
	className = "",
}) => {
	const { t } = useTranslation();
	const { login, googleLogin, isLoading, generateGoogleAuthUrl } = useAuth();

	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
	} = useForm<LoginRequest>({
		mode: "onBlur",
	});

	const onSubmit = async (data: LoginRequest) => {
		await login(data);
	};

	const handleGoogleLogin = () => {
		const googleAuthUrl = generateGoogleAuthUrl();
		window.location.href = googleAuthUrl;
	};

	const isFormLoading = isLoading || isSubmitting;

	return (
		<div className={`w-full max-w-md mx-auto ${className}`}>
			<div className="bg-surface border border-border rounded-lg shadow-lg p-6">
				<div className="text-center mb-6">
					<h1 className="text-2xl font-bold text-text mb-2">
						{t("login") || "Login"}
					</h1>
				</div>

				<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
					<div>
						<label
							htmlFor="email"
							className="block text-sm font-medium text-text mb-1"
						>
							{t("email") || "Email"}
						</label>
						<input
							{...register("email", {
								required:
									t("emailRequired", "Email is required") || "Email is required",
								pattern: {
									value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
									message:
										t("emailInvalid", "Invalid email address") || "Invalid email address",
								},
							})}
							type="email"
							id="email"
							disabled={isFormLoading}
							className={`
                w-full px-3 py-2 border rounded-md
                bg-background text-text placeholder-textMuted
                border-border focus:border-primary focus:ring-1 focus:ring-primary
                disabled:opacity-50 disabled:cursor-not-allowed
                transition-colors duration-200
                ${
																	errors.email
																		? "border-error focus:border-error focus:ring-error"
																		: ""
																}
              `}
							placeholder={
								t("emailPlaceholder", "Enter your email") || "Enter your email"
							}
						/>
						{errors.email && (
							<p className="mt-1 text-sm text-error">{errors.email.message}</p>
						)}
					</div>

					<div>
						<label
							htmlFor="password"
							className="block text-sm font-medium text-text mb-1"
						>
							{t("password") || "Password"}
						</label>
						<input
							{...register("password", {
								required:
									t("passwordRequired", "Password is required") ||
									"Password is required",
								minLength: {
									value: 3,
									message:
										t("passwordMinLength", "Password must be at least 3 characters") ||
										"Password must be at least 3 characters",
								},
							})}
							type="password"
							id="password"
							disabled={isFormLoading}
							className={`
                w-full px-3 py-2 border rounded-md
                bg-background text-text placeholder-textMuted
                border-border focus:border-primary focus:ring-1 focus:ring-primary
                disabled:opacity-50 disabled:cursor-not-allowed
                transition-colors duration-200
                ${
																	errors.password
																		? "border-error focus:border-error focus:ring-error"
																		: ""
																}
              `}
							placeholder={
								t("passwordPlaceholder", "Enter your password") || "Enter your password"
							}
						/>
						{errors.password && (
							<p className="mt-1 text-sm text-error">{errors.password.message}</p>
						)}
					</div>

					<button
						type="submit"
						disabled={isFormLoading}
						className={`
              w-full py-2 px-4 rounded-md font-medium
              bg-primary text-white hover:bg-primary/90
              disabled:opacity-50 disabled:cursor-not-allowed
              transition-colors duration-200
              focus:ring-2 focus:ring-primary focus:ring-offset-2
              ${isFormLoading ? "cursor-not-allowed" : "cursor-pointer"}
            `}
					>
						{isFormLoading ? (
							<div className="flex items-center justify-center">
								<div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2" />
								{t("loggingIn", "Signing in...") || "Signing in..."}
							</div>
						) : (
							t("login") || "Login"
						)}
					</button>
				</form>

				<div className="mt-4">
					<div className="relative">
						<div className="absolute inset-0 flex items-center">
							<div className="w-full border-t border-border" />
						</div>
						<div className="relative flex justify-center text-sm">
							<span className="px-2 bg-surface text-textMuted">
								{t("orContinueWith", "Or continue with") || "Or continue with"}
							</span>
						</div>
					</div>

					<button
						type="button"
						onClick={handleGoogleLogin}
						disabled={isFormLoading}
						className={`
              w-full mt-3 py-2 px-4 border border-border rounded-md
              bg-background text-text hover:bg-surface
              disabled:opacity-50 disabled:cursor-not-allowed
              transition-colors duration-200
              focus:ring-2 focus:ring-primary focus:ring-offset-2
              flex items-center justify-center gap-2
            `}
					>
						<svg className="w-5 h-5" viewBox="0 0 24 24">
							<path
								fill="currentColor"
								d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
							/>
							<path
								fill="currentColor"
								d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
							/>
							<path
								fill="currentColor"
								d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
							/>
							<path
								fill="currentColor"
								d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
							/>
						</svg>
						{t("Google", "Continue with Google") || "Google"}
					</button>
				</div>

				{onSwitchToRegister && (
					<div className="mt-6 text-center">
						<p className="text-textMuted">
							{t("noAccount", "Don't have an account?") || "Don't have an account?"}{" "}
							<button
								type="button"
								onClick={onSwitchToRegister}
								className="text-primary hover:underline font-medium"
							>
								{t("register") || "Register"}
							</button>
						</p>
					</div>
				)}
			</div>
		</div>
	);
};
