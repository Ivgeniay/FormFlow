export interface Language {
	id: string;
	code: string;
	shortCode: string;
	name: string;
	iconURL?: string | null;
	region: string;
	isDefault: boolean;
	isActive: boolean;
}
