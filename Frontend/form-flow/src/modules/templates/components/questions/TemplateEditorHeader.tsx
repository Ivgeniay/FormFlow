import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../../auth/hooks/useAuth";
import { topicsApi } from "../../../../api/topicsApi";
import { tagsApi } from "../../../../api/tagsApi";
import { ImageUploader } from "../../../../ui/Input/ImageUploader";
import { FormattedTextInput } from "../../../../ui/Input/FormattedTextInput";
import { usersApi } from "../../../../api/usersApi";
import { TemplateAccess } from "../../../../shared/domain_types";
import { mode } from "../../../../pages/templates/TemplateEditor";

interface Topic {
	id: string;
	name: string;
}

interface User {
	id: string;
	userName: string;
	primaryEmail: string;
}

export interface TemplateHeaderData {
	title: string;
	description: string;
	image: File | null;
	topicId: string | null;
	accessType: TemplateAccess;
	tags: string[];
	allowedUserIds: string[];
}

interface TemplateEditorHeaderProps {
	data: TemplateHeaderData;
	onDataChange: (data: TemplateHeaderData) => void;
	mode: mode;
}

export const TemplateEditorHeader: React.FC<TemplateEditorHeaderProps> = ({
	data,
	onDataChange,
	mode,
}) => {
	const { t } = useTranslation();
	const { accessToken } = useAuth();

	const [topics, setTopics] = useState<Topic[]>([]);
	const [tagInput, setTagInput] = useState("");
	const [userSearchInput, setUserSearchInput] = useState("");
	const [showUserSearch, setShowUserSearch] = useState(false);
	const [availableTags, setAvailableTags] = useState<string[]>([]);
	const [availableUsers, setAvailableUsers] = useState<User[]>([]);
	const [loadingTopics, setLoadingTopics] = useState(false);
	const [loadingUsers, setLoadingUsers] = useState(false);

	useEffect(() => {
		const loadTopics = async () => {
			setLoadingTopics(true);
			try {
				const topicsData = await topicsApi.getTopicsList(100);
				setTopics(topicsData);
			} catch (error) {
				console.error("Error loading topics:", error);
			} finally {
				setLoadingTopics(false);
			}
		};

		if (data.image) {
		}

		loadTopics();
	}, [data.image]);

	const updateData = (updates: Partial<TemplateHeaderData>) => {
		onDataChange({ ...data, ...updates });
	};

	const handleImageSelect = (file: File | null) => {
		updateData({ image: file });
	};

	const handleTitleChange = (title: string) => {
		updateData({ title });
	};

	const handleDescriptionChange = (description: string) => {
		updateData({ description });
	};

	const handleTopicChange = (topicId: string) => {
		updateData({ topicId });
	};

	const handleAccessTypeChange = (accessType: TemplateAccess) => {
		updateData({
			accessType,
			allowedUserIds:
				accessType === TemplateAccess.Public ? [] : data.allowedUserIds,
		});
	};

	const handleAddTag = async (tag: string) => {
		const trimmedTag = tag.trim();
		if (!trimmedTag || data.tags.includes(trimmedTag)) {
			setTagInput("");
			setAvailableTags([]);
			return;
		}

		const existingTag = availableTags.includes(trimmedTag);
		if (!existingTag && accessToken) {
			try {
				await tagsApi.createTag({ name: trimmedTag }, accessToken);
				console.log(`Created new tag: ${trimmedTag}`);
			} catch (error) {
				console.error("Error creating tag:", error);
				setTagInput("");
				setAvailableTags([]);
				return;
			}
		}

		updateData({ tags: [...data.tags, trimmedTag] });
		setTagInput("");
		setAvailableTags([]);
	};

	const handleRemoveTag = (tagToRemove: string) => {
		updateData({ tags: data.tags.filter((tag) => tag !== tagToRemove) });
	};

	const handleTagInputChange = async (value: string) => {
		setTagInput(value);
		if (value.length >= 2) {
			try {
				const tags = await tagsApi.getTagsForAutocomplete(value, 5);
				setAvailableTags(tags.filter((tag) => !data.tags.includes(tag)));
			} catch (error) {
				console.error("Error loading tags:", error);
				setAvailableTags([]);
			}
		} else {
			setAvailableTags([]);
		}
	};

	const handleTagInputKeyDown = (e: React.KeyboardEvent) => {
		if (e.key === "Enter" || e.key === ",") {
			e.preventDefault();
			handleAddTag(tagInput);
		}
	};

	const handleUserSearchChange = async (value: string) => {
		setUserSearchInput(value);
		if (value.length >= 2 && accessToken) {
			setLoadingUsers(true);
			try {
				const users = await usersApi.searchUsers(value, 5, accessToken);
				setAvailableUsers(
					users.filter((user) => !data.allowedUserIds.includes(user.id))
				);
			} catch (error) {
				console.error("Error loading users:", error);
				setAvailableUsers([]);
			} finally {
				setLoadingUsers(false);
			}
		} else {
			setAvailableUsers([]);
		}
	};

	const handleAddUser = (userId: string) => {
		if (!data.allowedUserIds.includes(userId)) {
			updateData({ allowedUserIds: [...data.allowedUserIds, userId] });
		}
		setUserSearchInput("");
		setShowUserSearch(false);
		setAvailableUsers([]);
	};

	const handleRemoveUser = (userIdToRemove: string) => {
		updateData({
			allowedUserIds: data.allowedUserIds.filter((id) => id !== userIdToRemove),
		});
	};

	const selectedUsers =
		availableUsers.length > 0
			? availableUsers.filter((user) => data.allowedUserIds.includes(user.id))
			: data.allowedUserIds.map((id) => ({
					id,
					userName: `User ${id}`,
					primaryEmail: "",
			  }));

	const filteredUsers = availableUsers.filter(
		(user) =>
			(user.userName.toLowerCase().includes(userSearchInput.toLowerCase()) ||
				user.primaryEmail
					.toLowerCase()
					.includes(userSearchInput.toLowerCase())) &&
			!data.allowedUserIds.includes(user.id)
	);

	const filteredTags = availableTags.filter(
		(tag) =>
			tag.toLowerCase().includes(tagInput.toLowerCase()) &&
			!data.tags.includes(tag)
	);

	return (
		<div className="mb-8 p-6 bg-surface border border-border rounded-lg">
			<h2 className="text-xl font-semibold text-text mb-4">
				{t("templateSettings") || "Template Settings"}
			</h2>

			<div className="space-y-4">
				<div>
					<label className="block text-sm font-medium text-text mb-2">
						{t("templateImage") || "Template Image"}
					</label>
					<ImageUploader
						onImageSelect={handleImageSelect}
						supportedFormats={[".jpg", ".jpeg", ".png", ".gif", ".webp"]}
						currentImage={data.image}
					/>
				</div>

				<div>
					<label className="block text-sm font-medium text-text mb-2">
						{t("templateTitle") || "Template Title"}
					</label>
					<FormattedTextInput
						value={data.title}
						onChange={handleTitleChange}
						placeholder={t("enterTemplateTitle") || "Enter template title"}
						isReadOnly={mode === "readonly"}
					/>
				</div>

				<div>
					<label className="block text-sm font-medium text-text mb-2">
						{t("templateDescription") || "Template Description"}
					</label>
					<FormattedTextInput
						value={data.description}
						onChange={handleDescriptionChange}
						placeholder={
							t("enterTemplateDescription") || "Enter template description"
						}
						multiline
						isReadOnly={mode === "readonly"}
					/>
				</div>

				<div>
					<label className="block text-sm font-medium text-text mb-2">
						{t("topic") || "Topic"}
					</label>
					<select
						value={data.topicId || ""}
						onChange={(e) => handleTopicChange(e.target.value)}
						disabled={loadingTopics}
						className="w-full px-3 py-2 border border-border rounded-md bg-background text-text focus:border-primary focus:outline-none disabled:opacity-50"
					>
						<option value="">
							{loadingTopics
								? t("loadingTopics") || "Loading topics..."
								: t("selectTopic") || "Select a topic"}
						</option>
						{topics.map((topic) => (
							<option key={topic.id} value={topic.id}>
								{topic.name}
							</option>
						))}
					</select>
				</div>

				<div>
					<label className="block text-sm font-medium text-text mb-2">
						{t("accessType") || "Access Type"}
					</label>
					<div className="flex gap-4">
						<label className="flex items-center gap-2">
							<input
								type="radio"
								name="accessType"
								checked={data.accessType === TemplateAccess.Public}
								onChange={() => handleAccessTypeChange(TemplateAccess.Public)}
								className="text-primary focus:ring-primary"
							/>
							<span className="text-sm text-text">
								{t("publicAccess") || "Public"}
							</span>
						</label>
						<label className="flex items-center gap-2">
							<input
								type="radio"
								name="accessType"
								checked={data.accessType === TemplateAccess.Restricted}
								onChange={() =>
									handleAccessTypeChange(TemplateAccess.Restricted)
								}
								className="text-primary focus:ring-primary"
							/>
							<span className="text-sm text-text">
								{t("privateAccess") || "Private"}
							</span>
						</label>
					</div>
				</div>

				<div>
					<label className="block text-sm font-medium text-text mb-2">
						{t("tags") || "Tags"}
					</label>
					<div className="flex flex-wrap gap-2 mb-2">
						{data.tags.map((tag) => (
							<span
								key={tag}
								className="inline-flex items-center gap-1 px-2 py-1 bg-primary text-white text-sm rounded-full"
							>
								{tag}
								<button
									onClick={() => handleRemoveTag(tag)}
									className="text-white hover:text-error"
								>
									×
								</button>
							</span>
						))}
					</div>
					<div className="relative">
						<input
							type="text"
							value={tagInput}
							onChange={(e) => handleTagInputChange(e.target.value)}
							onKeyDown={handleTagInputKeyDown}
							placeholder={t("addTags") || "Add tags (press Enter or comma)"}
							className="w-full px-3 py-2 border border-border rounded-md bg-background text-text focus:border-primary focus:outline-none"
						/>
						{tagInput && filteredTags.length > 0 && (
							<div className="absolute top-full left-0 right-0 bg-surface border border-border rounded-md shadow-lg mt-1 max-h-40 overflow-y-auto z-10">
								{filteredTags.map((tag) => (
									<button
										key={tag}
										onClick={() => handleAddTag(tag)}
										className="w-full px-3 py-2 text-left text-text hover:bg-background"
									>
										{tag}
									</button>
								))}
							</div>
						)}
					</div>
				</div>

				{data.accessType === TemplateAccess.Restricted && (
					<div>
						<label className="block text-sm font-medium text-text mb-2">
							{t("allowedUsers") || "Allowed Users"}
						</label>

						{selectedUsers.length > 0 && (
							<div className="flex flex-wrap gap-2 mb-2">
								{selectedUsers.map((user) => (
									<span
										key={user.id}
										className="inline-flex items-center gap-1 px-2 py-1 bg-surface border border-border text-sm rounded-full"
									>
										{user.userName}
										<button
											onClick={() => handleRemoveUser(user.id)}
											className="text-textMuted hover:text-error"
										>
											×
										</button>
									</span>
								))}
							</div>
						)}

						{!accessToken ? (
							<div className="text-sm text-textMuted">
								{t("loginRequiredToSearchUser") ||
									"Please log in to search users"}
							</div>
						) : (
							<div className="relative">
								<input
									type="text"
									value={userSearchInput}
									onChange={(e) => handleUserSearchChange(e.target.value)}
									onFocus={() => setShowUserSearch(true)}
									placeholder={
										t("searchUsers") || "Search users by name or email"
									}
									className="w-full px-3 py-2 border border-border rounded-md bg-background text-text focus:border-primary focus:outline-none"
								/>
								{showUserSearch && userSearchInput && (
									<div className="absolute top-full left-0 right-0 bg-surface border border-border rounded-md shadow-lg mt-1 max-h-40 overflow-y-auto z-10">
										{loadingUsers ? (
											<div className="px-3 py-2 text-textMuted">
												{t("loadingUsers") || "Loading users..."}
											</div>
										) : filteredUsers.length > 0 ? (
											filteredUsers.map((user) => (
												<button
													key={user.id}
													onClick={() => handleAddUser(user.id)}
													className="w-full px-3 py-2 text-left text-text hover:bg-background"
												>
													<div>
														<div className="font-medium">{user.userName}</div>
														<div className="text-sm text-textMuted">
															{user.primaryEmail}
														</div>
													</div>
												</button>
											))
										) : userSearchInput.length >= 2 ? (
											<div className="px-3 py-2 text-textMuted">
												{t("noUsersFound") || "No users found"}
											</div>
										) : null}
									</div>
								)}
							</div>
						)}
					</div>
				)}
			</div>
		</div>
	);
};
