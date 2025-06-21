import React from "react";
import * as Select from "@radix-ui/react-select";
import { QuestionType } from "../../shared/domain_types";

interface QuestionTypeSelectProps {
	value: QuestionType;
	onChange: (value: QuestionType) => void;
}

export const QuestionTypeSelect: React.FC<QuestionTypeSelectProps> = ({
	value,
	onChange,
}) => {
	const questionTypeOptions = [
		{ value: QuestionType.ShortText, label: "Short Text", icon: "📝" },
		{ value: QuestionType.LongText, label: "Long Text", icon: "📄" },
		{ value: QuestionType.SingleChoice, label: "Single Choice", icon: "🔘" },
		{
			value: QuestionType.MultipleChoice,
			label: "Multiple Choice",
			icon: "☑️",
		},
		{ value: QuestionType.Dropdown, label: "Dropdown", icon: "📋" },
		{ value: QuestionType.Scale, label: "Scale", icon: "📊" },
		{ value: QuestionType.Rating, label: "Rating", icon: "⭐" },
		{ value: QuestionType.Date, label: "Date", icon: "📅" },
		{ value: QuestionType.Time, label: "Time", icon: "🕒" },
	];

	const selectedOption = questionTypeOptions.find((opt) => opt.value === value);

	return (
		<Select.Root
			value={value.toString()}
			onValueChange={(val) => onChange(Number(val) as QuestionType)}
		>
			<Select.Trigger className="inline-flex items-center justify-between px-3 py-2 border border-border rounded-md bg-background text-text focus:border-primary focus:outline-none min-w-[150px]">
				<Select.Value>
					{selectedOption?.icon} {selectedOption?.label}
				</Select.Value>
				<Select.Icon>
					<svg
						className="w-4 h-4"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M19 9l-7 7-7-7"
						/>
					</svg>
				</Select.Icon>
			</Select.Trigger>
			<Select.Portal>
				<Select.Content className="bg-surface border border-border rounded-md shadow-lg p-1 z-50">
					<Select.Viewport>
						{questionTypeOptions.map((option) => (
							<Select.Item
								key={option.value}
								value={option.value.toString()}
								className="flex items-center px-3 py-2 text-sm text-text hover:bg-background rounded cursor-pointer focus:bg-background focus:outline-none"
							>
								<Select.ItemText>
									{option.icon} {option.label}
								</Select.ItemText>
							</Select.Item>
						))}
					</Select.Viewport>
				</Select.Content>
			</Select.Portal>
		</Select.Root>
	);
};
