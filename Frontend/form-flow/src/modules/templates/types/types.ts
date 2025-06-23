import { QuestionType, TemplateAccess } from "../../../shared/domain_types";

export interface QuestionData {
	id: string;
	order: number;
	title: string;
	description: string;
	type: QuestionType;
	isRequired: boolean;
	showInResults: boolean;
	typeSpecificData: Record<string, any>;
}

export interface FormTemplate {
	title: string;
	description: string;
	image: File | null;
	topicId: string | null;
	accessType: TemplateAccess;
	tags: string[];
	allowedUserIds: string[];
	questions: QuestionData[];
}
