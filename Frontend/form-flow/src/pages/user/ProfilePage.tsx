import React, { useCallback, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { useParams } from "react-router-dom";
import { UserDto } from "../../shared/api_types";
import { usersApi } from "../../api/usersApi";
import { LittleSpinner } from "../../ui/Spinner/LittleSpinner";
import { ProfileElement } from "../../modules/auth/profile/ProfileElement";

export const ProfilePage: React.FC = () => {
	const { t } = useTranslation();
	const { id } = useParams<{ id?: string }>();
	const { user, isAuthenticated, authType, accessToken } = useAuth();
	const [otherUser, setOtherUser] = useState<UserDto | null>(null);
	const [isLoading, setIsLoading] = useState<boolean>(false);

	const isOwnProfile = useCallback((): boolean => {
		const _id = id?.toLocaleLowerCase();
		const ownId = user?.id.toLocaleLowerCase();
		const isEqual = _id === ownId;

		return isEqual || !id;
	}, [id, user]);

	const fetchUser = useCallback(
		async (userId: string) => {
			try {
				if (accessToken) {
					setIsLoading(true);
					const response = await usersApi.getUserById(userId, accessToken);
					console.log(response);
					setOtherUser(response);
				}
			} catch (error: any) {
			} finally {
				setIsLoading(false);
			}
		},
		[accessToken]
	);

	useEffect(() => {
		if (!isOwnProfile() && id) {
			fetchUser(id);
		} else {
		}
	}, [id, isOwnProfile, fetchUser]);

	const converRoleToStringFroUser = (user: UserDto): string => {
		if (!user) return "undefined";
		if (user.role & 1) return "User";
		if (user.role & 2) return "Admin";
		if (user.role & 4) return "Moderator";
		if (user.role & 8) return "SuperAdmin";
		return "undefined";
	};

	if (!isAuthenticated || !user) {
		return (
			<div className="flex items-center justify-center py-16">
				<div className="text-center">
					<h1 className="text-2xl font-bold text-text mb-4">
						{t("accessDenied", "Access Denied") || "Access Denied"}
					</h1>
					<p className="text-textMuted">
						{t("loginRequired", "Please log in to view your profile") ||
							"Please log in to view your profile"}
					</p>
				</div>
			</div>
		);
	}
	if (isLoading) {
		return (
			<>
				<div className="max-w-4xl mx-auto p-6 space-y-6">
					<div className="flex items-center justify-center bg-surface border border-border rounded-lg p-6">
						<LittleSpinner />
					</div>
				</div>
			</>
		);
	}

	if (isOwnProfile()) {
		return (
			<ProfileElement
				user={user}
				roleName={converRoleToStringFroUser(user)}
				authType={authType}
				isShowPromoteToRole={isOwnProfile()}
			/>
		);
	} else {
		return (
			<>
				{otherUser && (
					<ProfileElement
						user={otherUser}
						roleName={converRoleToStringFroUser(otherUser)}
						// authType={authType}
						isShowPromoteToRole={isOwnProfile()}
					/>
				)}
			</>
		);
	}
};
