import { useTranslation } from "react-i18next";
import { QuestionData } from "../../modules/templates/components/questions/QuestionCard";
import { QuestionType } from "../../shared/domain_types";
import { FormattedTextInput } from "./FormattedTextInput";

interface QuestionSettingsProps {
	question: QuestionData;
	onChange: (data: Record<string, any>) => void;
}

export const QuestionSettings: React.FC<QuestionSettingsProps> = ({
	question,
	onChange,
}) => {
	const { t } = useTranslation();

	const updateSetting = (key: string, value: any) => {
		onChange({ ...question.typeSpecificData, [key]: value });
	};

	const renderSettings = () => {
		switch (question.type) {
			case QuestionType.ShortText:
				return (
					<div className="space-y-3">
						<div className="text-sm text-textMuted mb-2">
							{t("shortTextHint") || "Users will enter a short text response"}
						</div>
						<FormattedTextInput
							value={question.typeSpecificData.placeholder || ""}
							onChange={(value) => updateSetting("placeholder", value)}
							placeholder={t("placeholderText") || "Placeholder text"}
						/>
					</div>
				);

			case QuestionType.LongText:
				return (
					<div className="space-y-3">
						<div className="text-sm text-textMuted mb-2">
							{t("longTextHint") || "Users will enter a long text response"}
						</div>
						<FormattedTextInput
							value={question.typeSpecificData.placeholder || ""}
							onChange={(value) => updateSetting("placeholder", value)}
							placeholder={t("placeholderText") || "Placeholder text"}
						/>
					</div>
				);

			case QuestionType.SingleChoice:
			case QuestionType.MultipleChoice:
			case QuestionType.Dropdown:
				return (
					<div className="space-y-3">
						<div className="text-sm text-textMuted mb-2">
							{question.type === QuestionType.SingleChoice &&
								(t("singleChoiceHint") || "Users will select one option")}
							{question.type === QuestionType.MultipleChoice &&
								(t("multipleChoiceHint") ||
									"Users can select multiple options")}
							{question.type === QuestionType.Dropdown &&
								(t("dropdownHint") || "Users will choose from a dropdown list")}
						</div>
						<div className="space-y-2">
							{(question.typeSpecificData.options || ["Option 1"]).map(
								(option: string, index: number) => (
									<div key={index} className="flex items-center gap-2">
										<span className="text-sm text-textMuted w-4">
											{index + 1}.
										</span>
										<FormattedTextInput
											value={option}
											onChange={(value) => {
												const newOptions = [
													...(question.typeSpecificData.options || []),
												];
												newOptions[index] = value;
												updateSetting("options", newOptions);
											}}
											placeholder={`Option ${index + 1}`}
											className="flex-1"
										/>
										<button
											onClick={() => {
												const newOptions = (
													question.typeSpecificData.options || []
												).filter((_: any, i: number) => i !== index);
												updateSetting("options", newOptions);
											}}
											className="p-1 text-textMuted hover:text-error"
										>
											×
										</button>
									</div>
								)
							)}
							<button
								onClick={() => {
									const newOptions = [
										...(question.typeSpecificData.options || []),
										`Option ${
											(question.typeSpecificData.options?.length || 0) + 1
										}`,
									];
									updateSetting("options", newOptions);
								}}
								className="text-sm text-primary hover:underline"
							>
								+ {t("addOption") || "Add option"}
							</button>
						</div>
					</div>
				);

			case QuestionType.Scale:
				return (
					<div className="space-y-3">
						<div className="text-sm text-textMuted mb-2">
							{t("scaleHint") || "Users will rate on a scale"}
						</div>
						<div className="grid grid-cols-2 gap-4">
							<div>
								<label className="block text-xs text-textMuted mb-1">
									{t("minValue") || "Min Value"}
								</label>
								<input
									type="number"
									value={question.typeSpecificData.minValue || 1}
									onChange={(e) =>
										updateSetting("minValue", parseInt(e.target.value))
									}
									className="w-full px-2 py-1 text-sm border border-border rounded bg-background text-text focus:border-primary focus:outline-none"
								/>
							</div>
							<div>
								<label className="block text-xs text-textMuted mb-1">
									{t("maxValue") || "Max Value"}
								</label>
								<input
									type="number"
									value={question.typeSpecificData.maxValue || 5}
									onChange={(e) =>
										updateSetting("maxValue", parseInt(e.target.value))
									}
									className="w-full px-2 py-1 text-sm border border-border rounded bg-background text-text focus:border-primary focus:outline-none"
								/>
							</div>
						</div>
					</div>
				);

			case QuestionType.Rating:
				return (
					<div className="space-y-3">
						<div className="text-sm text-textMuted mb-2">
							{t("ratingHint") || "Users will give a star rating"}
						</div>
						<div>
							<label className="block text-xs text-textMuted mb-1">
								{t("maxRating") || "Max Rating"}
							</label>
							<input
								type="number"
								min="1"
								max="10"
								value={question.typeSpecificData.maxRating || 5}
								onChange={(e) =>
									updateSetting("maxRating", parseInt(e.target.value))
								}
								className="w-20 px-2 py-1 text-sm border border-border rounded bg-background text-text focus:border-primary focus:outline-none"
							/>
						</div>
					</div>
				);

			case QuestionType.Date:
				return (
					<div className="space-y-3">
						<div className="text-sm text-textMuted mb-2">
							{t("dateHint") || "Users will select a date"}
						</div>

						<div className="grid grid-cols-2 gap-4">
							<div>
								<label className="block text-xs text-textMuted mb-1">
									{t("minDate") || "Minimum Date"}
								</label>
								<input
									type="date"
									value={question.typeSpecificData.startDate || ""}
									onChange={(e) => updateSetting("startDate", e.target.value)}
									max={question.typeSpecificData.pastDate || undefined}
									className="w-full px-2 py-1.5 text-sm border border-border rounded bg-background text-text focus:border-primary focus:outline-none"
								/>
							</div>

							<div>
								<label className="block text-xs text-textMuted mb-1">
									{t("maxDate") || "Maximum Date"}
								</label>
								<input
									type="date"
									value={question.typeSpecificData.pastDate || ""}
									onChange={(e) => updateSetting("pastDate", e.target.value)}
									min={question.typeSpecificData.startDate || undefined}
									className="w-full px-2 py-1.5 text-sm border border-border rounded bg-background text-text focus:border-primary focus:outline-none"
								/>
							</div>
						</div>
					</div>
				);

			case QuestionType.Time:
				return (
					<div className="space-y-3">
						<div className="text-sm text-textMuted mb-2">
							{t("timeHint") || "Users will select a time"}
						</div>

						<div className="grid grid-cols-2 gap-4">
							<div>
								<label className="block text-xs text-textMuted mb-1">
									{t("minTime") || "Minimum Time"}
								</label>
								<input
									type="time"
									value={question.typeSpecificData.startTime || ""}
									onChange={(e) => updateSetting("startTime", e.target.value)}
									max={question.typeSpecificData.endTime || undefined}
									className="w-full px-2 py-1.5 text-sm border border-border rounded bg-background text-text focus:border-primary focus:outline-none"
								/>
							</div>

							<div>
								<label className="block text-xs text-textMuted mb-1">
									{t("maxTime") || "Maximum Time"}
								</label>
								<input
									type="time"
									value={question.typeSpecificData.endTime || ""}
									onChange={(e) => updateSetting("endTime", e.target.value)}
									min={question.typeSpecificData.startTime || undefined}
									className="w-full px-2 py-1.5 text-sm border border-border rounded bg-background text-text focus:border-primary focus:outline-none"
								/>
							</div>
						</div>
					</div>
				);

			default:
				return (
					<div className="text-sm text-textMuted">
						{t("configureQuestionSettings") ||
							"Question settings will appear here"}
					</div>
				);
		}
	};

	return <div className="border-t border-border pt-4">{renderSettings()}</div>;
};
