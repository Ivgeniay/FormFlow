export const SIGNALR_EVENTS = {
	NEW_COMMENT: "NewComment",
	LIKE_TOGGLED: "LikeToggled",
	USER_JOINED: "UserJoined",
	USER_LEFT: "UserLeft",
	ERROR: "Error",
	COMMENT_ADDED: "CommentAdded",
	USER_DISCONNECTED: "UserDisconnected",
	TEMPLATE_ACTIVITY: "TemplateActivity",
	LIKE_RESULT: "LikeResult",
	JOINED_TEMPLATE: "JoinedTemplate",
	CONNECTED: "Connected",
} as const;

export const SIGNALR_METHODS = {
	JOIN_TEMPLATE_GROUP: "JoinTemplateGroup",
	LEAVE_TEMPLATE_GROUP: "LeaveTemplateGroup",
	ADD_COMMENT: "AddComment",
	TOGGLE_LIKE: "ToggleLike",
	GET_TEMPLATE_ACTIVITY: "GetTemplateActivity",
} as const;

export const SIGNALR_CONFIG = {
	HUB_URL: "/template-activity",
	RECONNECT_ATTEMPTS: 5,
	RECONNECT_DELAY: 3000,
	MAX_RECONNECT_DELAY: 30000,
} as const;
