import { SIGNALR_CONFIG, SIGNALR_EVENTS, SIGNALR_METHODS } from "./const";
import {
	CommentAddedEvent,
	ConnectedEvent,
	JoinedTemplateEvent,
	LikeResultEvent,
	LikeToggledEvent,
	NewCommentEvent,
	TemplateActivityEvent,
	UserJoinedEvent,
	UserLeftEvent,
} from "./events";
import { signalRService } from "./SignalRService";

export class TemplateActivityService {
	private currentTemplateId: string | null = null;
	private isConnected = false;

	async connect(accessToken?: string): Promise<boolean> {
		try {
			const connected = await signalRService.connect(
				SIGNALR_CONFIG.HUB_URL,
				accessToken
			);
			this.isConnected = connected;
			return connected;
		} catch (error) {
			console.error("Failed to connect to template activity hub:", error);
			this.isConnected = false;
			return false;
		}
	}

	async disconnect(): Promise<void> {
		if (this.currentTemplateId) {
			await this.leaveTemplate(this.currentTemplateId);
		}
		await signalRService.disconnect();
		this.isConnected = false;
	}

	isSignalRConnected(): boolean {
		return this.isConnected && signalRService.isConnected();
	}

	async joinTemplate(templateId: string): Promise<void> {
		if (!this.isSignalRConnected()) {
			throw new Error("SignalR connection not established");
		}

		if (this.currentTemplateId && this.currentTemplateId !== templateId) {
			await this.leaveTemplate(this.currentTemplateId);
		}

		await signalRService.invoke(
			SIGNALR_METHODS.JOIN_TEMPLATE_GROUP,
			templateId
		);
		this.currentTemplateId = templateId;
	}

	async leaveTemplate(templateId: string): Promise<void> {
		if (!this.isSignalRConnected()) {
			return;
		}

		await signalRService.invoke(
			SIGNALR_METHODS.LEAVE_TEMPLATE_GROUP,
			templateId
		);

		if (this.currentTemplateId === templateId) {
			this.currentTemplateId = null;
		}
	}

	async addComment(templateId: string, content: string): Promise<void> {
		if (!this.isSignalRConnected()) {
			throw new Error("SignalR connection not established");
		}

		await signalRService.invoke(
			SIGNALR_METHODS.ADD_COMMENT,
			templateId,
			content
		);
	}

	async toggleLike(templateId: string): Promise<void> {
		if (!this.isSignalRConnected()) {
			throw new Error("SignalR connection not established");
		}
		await signalRService.invoke(SIGNALR_METHODS.TOGGLE_LIKE, templateId);
	}

	async getTemplateActivity(templateId: string): Promise<void> {
		if (!this.isSignalRConnected()) {
			throw new Error("SignalR connection not established");
		}

		await signalRService.invoke(
			SIGNALR_METHODS.GET_TEMPLATE_ACTIVITY,
			templateId
		);
	}

	onNewComment(callback: (event: NewCommentEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.NEW_COMMENT, callback);
	}

	onLikeToggled(callback: (event: LikeToggledEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.LIKE_TOGGLED, callback);
	}

	onUserJoined(callback: (event: UserJoinedEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.USER_JOINED, callback);
	}

	onUserLeft(callback: (event: UserLeftEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.USER_LEFT, callback);
	}

	onTemplateActivity(callback: (event: TemplateActivityEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.TEMPLATE_ACTIVITY, callback);
	}

	onError(callback: (event: ErrorEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.ERROR, callback);
	}

	onConnected(callback: (event: ConnectedEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.CONNECTED, callback);
	}

	onLikeResult(callback: (event: LikeResultEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.LIKE_RESULT, callback);
	}

	onCommentAdded(callback: (event: CommentAddedEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.COMMENT_ADDED, callback);
	}

	onJoinedTemplate(callback: (event: JoinedTemplateEvent) => void): void {
		signalRService.on(SIGNALR_EVENTS.JOINED_TEMPLATE, callback);
	}

	off(eventName: string, callback: (...args: any[]) => void): void {
		signalRService.off(eventName, callback);
	}

	offAll(): void {
		Object.values(SIGNALR_EVENTS).forEach((event) => {
			signalRService.kill(event);
		});
	}
}

export const templateActivityService = new TemplateActivityService();
