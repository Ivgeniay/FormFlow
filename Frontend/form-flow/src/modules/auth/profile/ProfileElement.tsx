import { useTranslation } from "react-i18next";
import { UserAvanar } from "../../../components/UserAvatar";
import { PromoteToRoleButtons } from "../components/PromoteToRoleButton";
import { UserDto } from "../../../shared/api_types";

interface ProfileElementProps {
	user: UserDto;
	isShowPromoteToRole?: boolean;
	authType?: string | null;
	roleName: string;
}
export const ProfileElement: React.FC<ProfileElementProps> = ({
	user,
	isShowPromoteToRole,
	authType,
	roleName,
}) => {
	const { t } = useTranslation();

	return (
		<div className="max-w-4xl mx-auto p-6 space-y-6">
			<div className="bg-surface border border-border rounded-lg p-6">
				<div className="flex items-center gap-4 mb-6">
					<UserAvanar
						size={16}
						literal={user.userName.charAt(0).toUpperCase()}
						mode={`picture`}
						// picture="https://lh3.googleusercontent.com/a/ACg8ocKf8FAOJtarltS-I7HM01-b77I_gudwHMjuQ3ux5XmaTNpB5Uy_=s96-c"
					/>
					<div>
						<h1 className="text-2xl font-bold text-text">{user.userName}</h1>
						{/* {userEmail && <p className="text-textMuted">{userEmail}</p>} */}
					</div>
				</div>

				<div className="grid grid-cols-1 md:grid-cols-2 gap-6">
					<div className="space-y-4">
						<div>
							<label className="block text-sm font-medium text-text mb-1">
								{t("username", "Username") || "Username"}
							</label>
							<p className="text-text">{user.userName}</p>
						</div>

						<div>
							{/* {userEmail && (
									<>
										<label className="block text-sm font-medium text-text mb-1">
											{t("email", "Email") || "Email"}
										</label>
										<p className="text-text">{user.primaryEmail}</p>
									</>
								)} */}
						</div>

						{authType && (
							<div>
								<label className="block text-sm font-medium text-text mb-1">
									{t("authMethod", "Authentication Method") ||
										"Authentication Method"}
								</label>
								<p className="text-text">{authType}</p>
							</div>
						)}
					</div>

					<div className="space-y-4">
						<div>
							<label className="block text-sm font-medium text-text mb-1">
								{t("roles", "Roles") || "Roles"}
							</label>
							<div className="flex flex-wrap gap-2">
								<span
									key={roleName}
									className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-primary/10 text-primary"
								>
									{roleName}
								</span>
							</div>
						</div>

						<div>
							<label className="block text-sm font-medium text-text mb-1">
								{t("memberSince", "Member Since") || "Member Since"}
							</label>
							<p className="text-text">
								{new Date(user.createdAt).toLocaleDateString()}
							</p>
						</div>

						<div className="pt-4">
							<PromoteToRoleButtons className="w-full" />
						</div>
					</div>
				</div>
			</div>
		</div>
	);
};
