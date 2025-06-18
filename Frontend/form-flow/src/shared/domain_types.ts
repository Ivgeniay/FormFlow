export interface User {
    id: string;
    userName: string;
    role: UserRole;
    isBlocked: boolean;
    isDeleted: boolean;
    createdAt: Date;
    updatedAt: Date;
}

export enum UserRole {
    None = 0,
    User = 1,
    Admin = 2,
    Moderator = 4,
    SuperAdmin = 8
}

export interface Topic {
    id: string;
    name: string;
    isActive: boolean;
}

export interface Tag {
    id: string;
    name: string;
    usageCount: number;
    createdAt: Date;
}

export interface TemplateTag {
    id: string;
    templateId: string;
    tagId: string;
    tag: Tag;
}

export interface TemplateAllowedUser {
    id: string;
    templateId: string;
    userId: string;
    user: User;
}

export interface Template {
    id: string;
    authorId: string;
    author: User;
    title: string;
    description: string;
    topicId: string;
    imageUrl?: string;
    accessType: TemplateAccess;
    isDeleted: boolean;
    isArchived: boolean;
    isPublished: boolean;
    version: number;
    isCurrentVersion: boolean;
    baseTemplateId?: string;
    previousVersionId?: string;
    createdAt: Date;
    updatedAt: Date;
    topic: Topic;
    questions: Question[];
    forms: Form[];
    comments: Comment[];
    likes: Like[];
    tags: TemplateTag[];
    allowedUsers: TemplateAllowedUser[];
    likesCount: number;
    formsCount: number;
    commentsCount: number;
}

export enum TemplateAccess {
    Public = 1,
    Restricted = 2
}

export enum QuestionType {
    ShortText = 1,
    LongText = 2,
    SingleChoice = 3,
    MultipleChoice = 4,
    Dropdown = 5,
    Scale = 6,
    Rating = 7,
    Date = 8,
    Time = 9
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
}

export interface DropdownDetails extends QuestionDetails {
    options: string[];
}

export interface ScaleDetails extends QuestionDetails {
    minValue: number;
    maxValue: number;
    minLabel?: string;
    maxLabel?: string;
}

export interface RatingDetails extends QuestionDetails {
    maxRating: number;
}

export interface DateDetails extends QuestionDetails {
    minDate?: string;
    maxDate?: string;
}

export interface TimeDetails extends QuestionDetails {
      format24Hour?: boolean;
}

export interface Question {
    id: string;
    templateId: string;
    order: number;
    showInResults: boolean;
    isRequired: boolean;
    data: string;
    isDeleted: boolean;
    createdAt: Date;
    updatedAt: Date;
    template: Template;
}

export interface Form {
    id: string;
    templateId: string;
    template: Template;
    userId: string;
    user: User;
    answersData: string;
    submittedAt: Date;
    isDeleted: boolean;
}

export interface Comment {
    id: string;
    templateId: string;
    userId: string;
    user: User;
    content: string;
    isDeleted: boolean;
    createdAt: Date;
}

export interface Like {
    id: string;
    templateId: string;
    userId: string;
    user: User;
    isDeleted: boolean;
    createdAt: Date;
}

export interface TemplateSearchDocument {
    id: string;
    title: string;
    description: string;
    topic: string;
    questionsText: string;
    commentsText: string;
    authorName: string;
    tags: string[];
    createdAt: Date;
    updatedAt: Date;
    isArchived: boolean;
    isPublished: boolean;
    isDeleted: boolean;
    formsCount: number;
    likesCount: number;
    commentsCount: number;
}

export interface TemplateSettings {
    title: string;
    description: string;
    topicId: string;
    imageUrl?: string;
    tags: string[];
    accessType: TemplateAccess;
    allowedUserIds: string[];
}

export interface TemplatePermissions {
    accessType: TemplateAccess;
    allowedUsers: User[];
    canView: boolean;
    canEdit: boolean;
    canFill: boolean;
}

export interface SearchFilters {
    query?: string;
    topicId?: string;
    tagIds?: string[];
    authorId?: string;
    isPublic?: boolean;
}

export interface PaginationParams {
    page: number;
    limit: number;
    sortBy?: string;
    sortOrder?: 'asc' | 'desc';
}

export interface PaginatedResponse<T> {
    data: T[];
    total: number;
    page: number;
    limit: number;
    totalPages: number;
}

export interface TemplateStats {
    totalForms: number;
    totalLikes: number;
    totalComments: number;
    lastActivityAt?: Date;
}

export interface QuestionStats {
    questionId: string;
    question: Question;
    totalAnswers: number;
    averageValue?: number;
    mostCommonValue?: string | number | boolean;
    uniqueValues: Array<{
        value: string | number | boolean;
        count: number;
        percentage: number;
    }>;
}

export interface FormResultColumn {
    questionId: string;
    questionTitle: string;
    questionType: QuestionType;
    displayValue: string;
}

export interface TemplateActivityEvent {
    templateId: string;
    recentComments: Comment[];
    likesCount: number;
    userLiked: boolean;
    loadedAt: Date;
}

export interface UserContact {
    id: string;
    userId: string;
    type: ContactType;
    value: string;
    isPrimary: boolean;
}

export enum ContactType {
    Email = 1,
    Phone = 2
}