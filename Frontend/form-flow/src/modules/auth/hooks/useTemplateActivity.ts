import { useEffect, useState, useCallback, useRef } from "react";
import toast from "react-hot-toast";
import { CommentDto } from "../../../shared/api_types";
import { useAuth } from "./useAuth";
import {
	CommentAddedEvent,
	LikeResultEvent,
	LikeToggledEvent,
	NewCommentEvent,
	TemplateActivityEvent,
	UserJoinedEvent,
	UserLeftEvent,
} from "../../../api/signalR/events";
import { templateActivityService } from "../../../api/signalR/templateActivityService";
import { commentApi } from "../../../api/commentApi";

interface TemplateActivityState {
	comments: CommentDto[];
	likesCount: number;
	isUserLiked: boolean;
	isConnected: boolean;
	isLoading: boolean;
	error: string | null;
	onlineUsers: Array<{
		userId: string;
		userName: string;
		joinedAt: string;
	}>;
}

interface TemplateActivityActions {
	addComment: (content: string) => Promise<void>;
	toggleLike: () => Promise<void>;
	loadComments: (page: number, pageSize: number) => Promise<CommentDto[]>;
	refreshActivity: () => Promise<void>;
}

export const useTemplateActivity = (
	templateId: string | undefined
): [TemplateActivityState, TemplateActivityActions] => {
	const { accessToken, isAuthenticated } = useAuth();
	const [state, setState] = useState<TemplateActivityState>({
		comments: [],
		likesCount: 0,
		isUserLiked: false,
		isConnected: false,
		isLoading: true,
		error: null,
		onlineUsers: [],
	});

	const templateIdRef = useRef<string | undefined>(templateId);
	const isJoinedRef = useRef(false);

	useEffect(() => {
		templateIdRef.current = templateId;
	}, [templateId]);

	const setupEventHandlers = useCallback(() => {
		templateActivityService.onNewComment((event: NewCommentEvent) => {
			if (event.templateId === templateIdRef.current) {
				setState((prev) => ({
					...prev,
					comments: [...prev.comments, event.comment],
				}));
			}
		});

		templateActivityService.onLikeToggled((event: LikeToggledEvent) => {
			if (event.templateId === templateIdRef.current) {
				setState((prev) => ({
					...prev,
					likesCount: event.totalLikes,
				}));
			}
		});

		templateActivityService.onUserJoined((event: UserJoinedEvent) => {
			if (event.templateId === templateIdRef.current) {
				setState((prev) => ({
					...prev,
					onlineUsers: [
						...prev.onlineUsers.filter((u) => u.userId !== event.userId),
						{
							userId: event.userId,
							userName: event.userName,
							joinedAt: event.joinedAt,
						},
					],
				}));
			}
		});

		templateActivityService.onUserLeft((event: UserLeftEvent) => {
			if (event.templateId === templateIdRef.current) {
				setState((prev) => ({
					...prev,
					onlineUsers: prev.onlineUsers.filter(
						(u) => u.userId !== event.userId
					),
				}));
			}
		});

		templateActivityService.onTemplateActivity(
			(event: TemplateActivityEvent) => {
				if (event.templateId === templateIdRef.current) {
					setState((prev) => ({
						...prev,
						comments: event.recentComments,
						likesCount: event.likesCount,
						isUserLiked: event.userLiked,
						isLoading: false,
					}));
				}
			}
		);

		templateActivityService.onLikeResult((event: LikeResultEvent) => {
			if (event.success) {
				setState((prev) => ({
					...prev,
					isUserLiked: event.result.isLiked,
					likesCount: event.result.totalLikes,
				}));
			}
		});

		templateActivityService.onCommentAdded((event: CommentAddedEvent) => {
			if (event.success) {
				toast.success("Comment added successfully");
			}
		});

		templateActivityService.onError((event: ErrorEvent) => {
			console.error("SignalR Error:", event);
			setState((prev) => ({
				...prev,
				error: event.message,
				isLoading: false,
			}));

			if (event.error.errorCode !== "ACCESS_DENIED") {
				toast.error(event.message);
			}
		});
	}, []);

	const connectSignalR = useCallback(async () => {
		try {
			setState((prev) => ({ ...prev, isLoading: true, error: null }));

			const connected = await templateActivityService.connect(
				accessToken || undefined
			);

			setState((prev) => ({
				...prev,
				isConnected: connected,
				isLoading: !connected,
			}));

			if (connected) {
				setupEventHandlers();
			}
		} catch (error) {
			console.error("Failed to connect to SignalR:", error);
			setState((prev) => ({
				...prev,
				isConnected: false,
				isLoading: false,
				error: "Failed to connect to real-time service",
			}));
		}
	}, [accessToken, setupEventHandlers]);

	const joinTemplateGroup = useCallback(async () => {
		if (
			!templateId ||
			!templateActivityService.isSignalRConnected() ||
			isJoinedRef.current
		) {
			return;
		}

		try {
			await templateActivityService.joinTemplate(templateId);
			await templateActivityService.getTemplateActivity(templateId);
			isJoinedRef.current = true;
		} catch (error) {
			console.error("Failed to join template group:", error);
			setState((prev) => ({
				...prev,
				error: "Failed to join template activity",
			}));
		}
	}, [templateId]);

	const leaveTemplateGroup = useCallback(async () => {
		if (!templateIdRef.current || !isJoinedRef.current) {
			return;
		}

		try {
			await templateActivityService.leaveTemplate(templateIdRef.current);
			isJoinedRef.current = false;
		} catch (error) {
			console.error("Failed to leave template group:", error);
		}
	}, []);

	const addComment = useCallback(
		async (content: string) => {
			if (!templateId || !isAuthenticated) {
				toast.error("Please log in to add comments");
				return;
			}

			if (!content.trim()) {
				toast.error("Comment cannot be empty");
				return;
			}

			try {
				await templateActivityService.addComment(templateId, content.trim());
			} catch (error) {
				console.error("Failed to add comment:", error);
				toast.error("Failed to add comment");
			}
		},
		[templateId, isAuthenticated]
	);

	const toggleLike = useCallback(async () => {
		if (!templateId || !isAuthenticated) {
			toast.error("Please log in to like templates");
			return;
		}

		try {
			await templateActivityService.toggleLike(templateId);
		} catch (error) {
			console.error("Failed to toggle like:", error);
			toast.error("Failed to update like");
		}
	}, [templateId, isAuthenticated]);

	const loadComments = useCallback(
		async (page: number, pageSize: number): Promise<CommentDto[]> => {
			if (!templateId) return [];

			try {
				const result = await commentApi.getCommentsByTemplate(
					templateId,
					page,
					pageSize
				);
				return result.data;
			} catch (error) {
				console.error("Failed to load comments:", error);
				return [];
			}
		},
		[templateId]
	);

	const refreshActivity = useCallback(async () => {
		if (!templateId || !templateActivityService.isSignalRConnected()) {
			return;
		}

		try {
			setState((prev) => ({ ...prev, isLoading: true }));
			await templateActivityService.getTemplateActivity(templateId);
		} catch (error) {
			console.error("Failed to refresh activity:", error);
			setState((prev) => ({
				...prev,
				error: "Failed to refresh activity",
				isLoading: false,
			}));
		}
	}, [templateId]);

	useEffect(() => {
		connectSignalR();

		return () => {
			leaveTemplateGroup();
		};
	}, [connectSignalR, leaveTemplateGroup]);

	useEffect(() => {
		if (
			state.isConnected &&
			templateId &&
			templateId !== templateIdRef.current
		) {
			leaveTemplateGroup();
			joinTemplateGroup();
		}
	}, [state.isConnected, templateId, joinTemplateGroup, leaveTemplateGroup]);

	useEffect(() => {
		return () => {
			leaveTemplateGroup();
			templateActivityService.offAll();
		};
	}, [leaveTemplateGroup]);

	return [
		state,
		{
			addComment,
			toggleLike,
			loadComments,
			refreshActivity,
		},
	];
};
