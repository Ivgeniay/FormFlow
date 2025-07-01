import React, { useState } from "react";
import toast from "react-hot-toast";
import { useAuth } from "../hooks/useAuth";
import { usersApi } from "../../../api/usersApi";
import { SimpleButton } from "../../../ui/Button/SimpleButton";
import { useAuthStore } from "../store/authStore";
import { useTranslation } from "react-i18next";

interface PromoteToAdminButtonProps {
	className?: string;
}

export const PromoteToRoleButtons: React.FC<PromoteToAdminButtonProps> = ({
	className = "",
}) => {
	const { t } = useTranslation();
	const { accessToken, isAdmin, user } = useAuth();
	const [isLoading, setIsLoading] = useState(false);
	const { updateAuthData } = useAuthStore();

	const handlePromoteTo = async (role: number) => {
		if (!accessToken) {
			toast.error("Please log in");
			return;
		}

		if (user?.role === role) {
			toast.error(t("alreadyHasRole") || "Already hase this role");
			return;
		}

		setIsLoading(true);

		try {
			const result = await usersApi.promoteToRole(accessToken, role);

			updateAuthData({
				user: result.user,
				accessToken: result.accessToken,
				refreshToken: result.refreshToken,
				accessTokenExpiry: result.accessTokenExpiry,
				refreshTokenExpiry: result.refreshTokenExpiry,
				authType: result.authType,
				isAuthenticated: true,
			});

			toast.success(t("succesPromote") || "Successfully promoted!");
		} catch (error) {
			console.error("Failed to promote:", error);
			toast.error(t("failedPromote") || "Failed to promote");
		} finally {
			setIsLoading(false);
		}
	};

	return (
		<>
			{!isAdmin && (
				<SimpleButton
					onClick={() => {
						handlePromoteTo(2);
					}}
					localKey={isLoading ? t("promoting") : t("promoteToAdmin")}
					className={className}
				/>
			)}
			{isAdmin && (
				<SimpleButton
					onClick={() => {
						handlePromoteTo(1);
					}}
					localKey={isLoading ? t("promoting") : t("promoteToUser")}
					className={className}
				/>
			)}
		</>
	);
};
