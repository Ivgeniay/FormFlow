import { CommentDto } from "../../shared/api_types";

export interface SignalREventData {
	templateId: string;
	updatedAt: string;
}

export interface NewCommentEvent {
	comment: CommentDto;
	templateId: string;
	addedAt: string;
}

export interface LikeToggledEvent {
	templateId: string;
	totalLikes: number;
	isLiked: boolean;
	action: string;
	userId: string;
	userName: string;
	updatedAt: string;
}

export interface UserJoinedEvent extends SignalREventData {
	userId: string;
	userName: string;
	joinedAt: string;
}

export interface UserLeftEvent extends SignalREventData {
	userId: string;
	userName: string;
	leftAt: string;
}

export interface TemplateActivityEvent {
	templateId: string;
	recentComments: CommentDto[];
	likesCount: number;
	userLiked: boolean;
	loadedAt: string;
}

export interface ErrorEvent {
	message: string;
	errorCode?: string;
	occurredAt: string;
}

export interface ConnectedEvent {
	connectionId: string;
	userId?: string;
	userName?: string;
	isAuthenticated: boolean;
	connectedAt: string;
}

export interface LikeResultEvent {
	success: boolean;
	result: {
		isLiked: boolean;
		totalLikes: number;
		action: string;
	};
}

export interface CommentAddedEvent {
	success: boolean;
	comment: CommentDto;
}

export interface JoinedTemplateEvent {
	templateId: string;
	message: string;
}
