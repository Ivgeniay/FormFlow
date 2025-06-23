import { TagCloud } from "../../modules/Tag/components/TagCloud";
import { mockTags } from "../../shared/mock_data";
import { LatestTemplatesSection } from "../../modules/templates/components/sections/LatestTemplatesSection";
import { MyTemplatesSection } from "../../modules/templates/components/sections/MyTemplatesSection";

export const HomePage: React.FC = () => {
	return (
		<>
			<LatestTemplatesSection />
			<TagCloud
				tags={mockTags}
				onTagClick={(tagName) => console.log("Tag clicked:", tagName)}
			/>
			<MyTemplatesSection />
		</>
	);
	// const mockQuestions: QuestionData[] = [
	// 	{
	// 		id: "1",
	// 		order: 0,
	// 		title: "What is your name?",
	// 		description: "Please enter your full name",
	// 		type: QuestionType.ShortText,
	// 		isRequired: true,
	// 		showInResults: true,
	// 		typeSpecificData: {
	// 			maxLength: 100,
	// 			placeholder: "Enter your name",
	// 		},
	// 	},
	// 	{
	// 		id: "2",
	// 		order: 1,
	// 		title: "Tell us about yourself",
	// 		description: "Share your background and interests",
	// 		type: QuestionType.LongText,
	// 		isRequired: false,
	// 		showInResults: true,
	// 		typeSpecificData: {
	// 			maxLength: 500,
	// 			placeholder: "Describe yourself...",
	// 		},
	// 	},
	// 	{
	// 		id: "3",
	// 		order: 2,
	// 		title: "What is your favorite color?",
	// 		description: "",
	// 		type: QuestionType.SingleChoice,
	// 		isRequired: true,
	// 		showInResults: true,
	// 		typeSpecificData: {
	// 			options: ["Red", "Blue", "Green", "Yellow", "Other"],
	// 		},
	// 	},
	// 	{
	// 		id: "4",
	// 		order: 3,
	// 		title: "Which programming languages do you know?",
	// 		description: "Select all that apply",
	// 		type: QuestionType.MultipleChoice,
	// 		isRequired: false,
	// 		showInResults: true,
	// 		typeSpecificData: {
	// 			options: ["JavaScript", "Python", "Java", "C#", "Go", "Rust"],
	// 			maxSelections: 3,
	// 		},
	// 	},
	// 	{
	// 		id: "5",
	// 		order: 4,
	// 		title: "Rate your experience with React",
	// 		description: "Scale from 1 (beginner) to 5 (expert)",
	// 		type: QuestionType.Scale,
	// 		isRequired: true,
	// 		showInResults: true,
	// 		typeSpecificData: {
	// 			minValue: 1,
	// 			maxValue: 5,
	// 			minLabel: "Beginner",
	// 			maxLabel: "Expert",
	// 		},
	// 	},
	// ];
};
