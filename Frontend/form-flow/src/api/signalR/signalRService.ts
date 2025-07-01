import {
	HubConnection,
	HubConnectionBuilder,
	LogLevel,
} from "@microsoft/signalr";
import { ENV } from "../../config/env";

export class SignalRService {
	private connection: HubConnection | null = null;
	private reconnectAttempts = 0;
	private maxReconnectAttempts = 5;
	private isReconnecting = false;

	async connect(hubUrl: string, accessToken?: string): Promise<boolean> {
		try {
			if (this.connection && this.connection.state === "Connected") {
				return true;
			}

			const builder = new HubConnectionBuilder()
				.withUrl(`${ENV.SIGNALR_URL}${hubUrl}`, {
					accessTokenFactory: () => accessToken || "",
					withCredentials: false,
				})
				.withAutomaticReconnect({
					nextRetryDelayInMilliseconds: (retryContext) => {
						if (retryContext.previousRetryCount < this.maxReconnectAttempts) {
							return Math.min(
								1000 * Math.pow(2, retryContext.previousRetryCount),
								32000
							);
						}
						return null;
					},
				})
				.configureLogging(LogLevel.Information);

			this.connection = builder.build();

			this.setupEventHandlers();
			await this.connection.start();

			this.reconnectAttempts = 0;
			this.isReconnecting = false;

			return true;
		} catch (error) {
			console.error("SignalR Connection Error:", error);
			return false;
		}
	}

	private setupEventHandlers(): void {
		if (!this.connection) return;

		this.connection.onclose(async (error) => {
			console.error("SignalR Connection Closed:", error);
		});

		this.connection.onreconnecting((error) => {
			console.error("SignalR Reconnecting:", error);
			this.isReconnecting = true;
		});

		this.connection.onreconnected(() => {
			this.isReconnecting = false;
			this.reconnectAttempts = 0;
		});
	}

	async disconnect(): Promise<void> {
		if (this.connection) {
			await this.connection.stop();
			this.connection = null;
		}
	}

	isConnected(): boolean {
		return this.connection?.state === "Connected";
	}

	async invoke(methodName: string, ...args: any[]): Promise<void> {
		if (!this.connection || !this.isConnected()) {
			throw new Error("SignalR connection not established");
		}

		await this.connection.invoke(methodName, ...args);
	}

	on(eventName: string, callback: (...args: any[]) => void): void {
		this.connection?.on(eventName, callback);
	}

	off(eventName: string, callback: (...args: any[]) => void): void {
		this.connection?.off(eventName, callback);
	}

	kill(eventName: string): void {
		this.connection?.off(eventName);
	}
}

export const signalRService = new SignalRService();
