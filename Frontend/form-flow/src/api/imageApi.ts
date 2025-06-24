import axios from "axios";
import { ENV } from "../config/env";

const API_BASE_URL = ENV.API_URL;

export interface ImageUploadResponse {
	imageUrl: string;
}

export interface ImageExistsResponse {
	exists: boolean;
	imageUrl?: string;
}

export interface MessageResponse {
	message: string;
}

class ImageApi {
	async uploadTemplateImage(
		templateId: string,
		file: File,
		accessToken: string
	): Promise<ImageUploadResponse> {
		const formData = new FormData();
		formData.append("file", file);

		const response = await axios.post<ImageUploadResponse>(
			`${API_BASE_URL}/image/template/${templateId}/image`,
			formData,
			{
				headers: {
					Authorization: `Bearer ${accessToken}`,
					"Content-Type": "multipart/form-data",
				},
			}
		);
		return response.data;
	}

	async deleteTemplateImage(
		templateId: string,
		accessToken: string
	): Promise<MessageResponse> {
		const response = await axios.delete<MessageResponse>(
			`${API_BASE_URL}/image/template/${templateId}/image`,
			{
				headers: { Authorization: `Bearer ${accessToken}` },
			}
		);
		return response.data;
	}

	async getImageProxy(imageUrl: string): Promise<File> {
		const proxyUrl = `${API_BASE_URL}/image/proxy?imageUrl=${encodeURIComponent(
			imageUrl
		)}`;
		const response = await axios.get(proxyUrl, {
			responseType: "blob",
		});

		const filename = imageUrl.split("/").pop() || "template-image";
		return new File([response.data], filename, {
			type: response.headers["content-type"] || "image/jpeg",
		});
	}

	async checkTemplateImageExists(
		templateId: string
	): Promise<ImageExistsResponse> {
		const response = await axios.get<ImageExistsResponse>(
			`${API_BASE_URL}/image/template/${templateId}/image/exists`
		);
		return response.data;
	}

	async getTemplateImage(
		templateId: string
	): Promise<ImageUploadResponse | null> {
		try {
			const response = await axios.get<ImageUploadResponse>(
				`${API_BASE_URL}/image/template/${templateId}/image`
			);
			return response.data;
		} catch (error: any) {
			if (error.response?.status === 204) {
				return null;
			}
			throw error;
		}
	}
}

export const imageApi = new ImageApi();
