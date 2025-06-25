export interface AuthResponse {
	user: UserDto;
	accessToken: string;
	refreshToken: string;
	accessTokenExpiry: string;
	refreshTokenExpiry: string;
	authType: string;
}

export interface RefreshTokenResponse {
	accessToken: string;
	refreshToken: string;
	accessTokenExpiry: string;
	refreshTokenExpiry: string;
}

export interface UserDto {
	id: string;
	userName: string;
	role: number;
	isBlocked: boolean;
	createdAt: string;
	updatedAt: string;
	primaryEmail?: string;
	contacts?: ContactDto[];
}

export interface ContactDto {
	id: string;
	type: number;
	value: string;
	isPrimary: boolean;
}

export interface TemplateDto {
	id: string;
	title: string;
	description: string;
	topicId?: string;
	topic: string;
	imageUrl?: string;
	accessType: number;
	authorId: string;
	authorName: string;
	createdAt: string;
	updatedAt: string;
	version: number;
	isPublished: boolean;
	isArchived: boolean;
	baseTemplateId?: string;
	previousVersionId?: string;
	questions: QuestionDto[];
	tags: TagDto[];
	allowedUsers: UserSearchDto[];
	formsCount: number;
	likesCount: number;
	commentsCount: number;
	isUserLiked: boolean;
	hasUserAccess: boolean;
	canUserEdit: boolean;
}

export interface QuestionDto {
	id: string;
	order: number;
	showInResults: boolean;
	isRequired: boolean;
	data: string;
	createdAt: string;
	updatedAt?: string;
}

export interface TagDto {
	id: string;
	name: string;
	usageCount: number;
	createdAt: string;
}

export interface TagCloudItemDto {
	id: string;
	name: string;
	usageCount: number;
	weight: number;
	createdAt: string;
}

export interface CloudTagDto {
	tags: TagCloudItemDto[];
	maxUsageCount: number;
	minUsageCount: number;
	generatedAt: string;
}

export interface UserSearchDto {
	id: string;
	userName: string;
	primaryEmail: string;
}

export interface FormDto {
	id: string;
	templateId: string;
	template: TemplateDto;
	userId: string;
	user: UserDto;
	answersData: string;
	submittedAt: string;
	canEdit: boolean;
	canView: boolean;
}

export interface CommentDto {
	id: string;
	templateId: string;
	authorId: string;
	authorName: string;
	content: string;
	createdAt: string;
	canDelete: boolean;
	isAuthor: boolean;
}

export interface LikeDto {
	id: string;
	templateId: string;
	userId: string;
	userName: string;
	createdAt: string;
}

export interface TopicDto {
	id: string;
	name: string;
	isActive: boolean;
}

export interface ColorThemeDto {
	id: string;
	name: string;
	cssClass: string;
	colorVariables: string;
	isDefault: boolean;
	isActive: boolean;
}

export interface LanguageDto {
	id: string;
	code: string;
	shortCode: string;
	name: string;
	iconURL?: string;
	region: string;
	isDefault: boolean;
	isActive: boolean;
}

export interface PaginatedResponse<T> {
	data: T[];
	currentPage: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
	hasNext: boolean;
	hasPrevious: boolean;
}

export interface SearchResponse {
	templates: TemplateDto[];
	pagination: {
		currentPage: number;
		pageSize: number;
		totalCount: number;
		totalPages: number;
		hasNext: boolean;
		hasPrevious: boolean;
	};
	searchInfo: {
		query: string;
		topic?: string;
		tags: string[];
		author?: string;
		sortBy: string;
		searchTime: number;
	};
}

export interface QuickSearchResult {
	id: string;
	title: string;
	authorName: string;
	tags: string[];
}

export interface LikeStatsResponse {
	templateId: string;
	likesCount: number;
	userLiked: boolean;
	isAuthenticated: boolean;
}

export interface CommentsCountResponse {
	templateId: string;
	commentsCount: number;
}

export interface CanCommentResponse {
	templateId: string;
	canComment: boolean;
}

export interface CanUserLikedResponse {
	templateId: string;
	hasLiked: boolean;
}

export interface FormSubmissionResponse {
	message?: string;
	isFormSubmitted: boolean;
}

export interface MessageResponse {
	message: string;
}

export interface UserBlockResponse {
	message: string;
	userId: string;
}

export interface UserRoleChangeResponse {
	message: string;
	user: UserDto;
	accessToken: string;
	refreshToken: string;
	accessTokenExpiry: string;
	refreshTokenExpiry: string;
}

export interface ReindexResponse {
	message: string;
	note: string;
	startedAt: string;
	startedBy: string;
}

export interface TopicExistsResponse {
	exists: boolean;
}

export interface ErrorResponse {
	message: string;
}
