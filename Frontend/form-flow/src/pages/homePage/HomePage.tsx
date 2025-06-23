import { useEffect, useState } from "react";
import { TagCloud } from "../../modules/Tag/components/TagCloud";
import { TemplateGallery } from "../../modules/templates/components/homepageComponents/TemplateGallery";
import { mockTags, mockTemplates } from "../../shared/mock_data";
import { QuestionType, TemplateAccess } from "../../shared/domain_types";
import { QuestionData } from "../../modules/templates/types/types";
import { TemplateEditor } from "./TemplateEditor";
import { TemplatePage } from "../templates/TemplatePage";

export const HomePage: React.FC = () => {
	const [isVisible, setVisible] = useState(true);

	useEffect(() => {
		const foo = () => {
			setVisible(!isVisible);
		};

		document.addEventListener("click", foo);

		return () => {
			document.removeEventListener("click", foo);
		};
	}, []);

	const mockQuestions: QuestionData[] = [
		{
			id: "1",
			order: 0,
			title: "What is your name?",
			description: "Please enter your full name",
			type: QuestionType.ShortText,
			isRequired: true,
			showInResults: true,
			typeSpecificData: {
				maxLength: 100,
				placeholder: "Enter your name",
			},
		},
		{
			id: "2",
			order: 1,
			title: "Tell us about yourself",
			description: "Share your background and interests",
			type: QuestionType.LongText,
			isRequired: false,
			showInResults: true,
			typeSpecificData: {
				maxLength: 500,
				placeholder: "Describe yourself...",
			},
		},
		{
			id: "3",
			order: 2,
			title: "What is your favorite color?",
			description: "",
			type: QuestionType.SingleChoice,
			isRequired: true,
			showInResults: true,
			typeSpecificData: {
				options: ["Red", "Blue", "Green", "Yellow", "Other"],
			},
		},
		{
			id: "4",
			order: 3,
			title: "Which programming languages do you know?",
			description: "Select all that apply",
			type: QuestionType.MultipleChoice,
			isRequired: false,
			showInResults: true,
			typeSpecificData: {
				options: ["JavaScript", "Python", "Java", "C#", "Go", "Rust"],
				maxSelections: 3,
			},
		},
		{
			id: "5",
			order: 4,
			title: "Rate your experience with React",
			description: "Scale from 1 (beginner) to 5 (expert)",
			type: QuestionType.Scale,
			isRequired: true,
			showInResults: true,
			typeSpecificData: {
				minValue: 1,
				maxValue: 5,
				minLabel: "Beginner",
				maxLabel: "Expert",
			},
		},
	];

	return (
		<>
			<TemplateGallery
				templates={mockTemplates}
				title="Последние шаблоны"
				maxItems={4}
				mode="compact"
				columns={6}
			/>
			<TagCloud
				tags={mockTags}
				onTagClick={(tagName) => console.log("Tag clicked:", tagName)}
			/>
		</>
	);
};
