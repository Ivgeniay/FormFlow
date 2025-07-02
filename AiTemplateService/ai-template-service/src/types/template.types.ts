export enum QuestionType {
    ShortText = 1,
    LongText = 2,
    SingleChoice = 3,
    MultipleChoice = 4,
    Dropdown = 5,
    Scale = 6,
    Rating = 7,
    Date = 8,
    Time = 9,
}

export interface QuestionDetails {
    type: QuestionType;
    title: string;
    description: string;
}

export interface ShortTextDetails extends QuestionDetails {
    maxLength?: number;
    placeholder?: string;
}

export interface LongTextDetails extends QuestionDetails {
    maxLength?: number;
    placeholder?: string;
}

export interface SingleChoiceDetails extends QuestionDetails {
    options: string[];
}

export interface MultipleChoiceDetails extends QuestionDetails {
    options: string[];
    maxSelections?: number;
    minSelections?: number;
}

export interface DropdownDetails extends QuestionDetails {
    options: string[];
    defaultOption?: string;
}

export interface ScaleDetails extends QuestionDetails {
    minValue: number;
    maxValue: number;
    minLabel?: string;
    maxLabel?: string;
}

export interface RatingDetails extends QuestionDetails {
    maxRating: number;
    ratingLabel?: string;
}

export interface DateDetails extends QuestionDetails {
    startDate?: string;
    pastDate?: string;
}

export interface TimeDetails extends QuestionDetails {
    startDate?: string;
    pastDate?: string;
}

export interface GeneratedQuestion {
    order: number;
    showInResults: boolean;
    isRequired: boolean;
    data: AnyQuestionDetails;
}

export interface GeneratedTemplate {
    title: string;
    description: string;
    topic: string;
    questions: GeneratedQuestion[];
    suggestedTags: string[];
}

export type AnyQuestionDetails =
    | ShortTextDetails
    | LongTextDetails
    | SingleChoiceDetails
    | MultipleChoiceDetails
    | DropdownDetails
    | ScaleDetails
    | RatingDetails
    | DateDetails
    | TimeDetails;
