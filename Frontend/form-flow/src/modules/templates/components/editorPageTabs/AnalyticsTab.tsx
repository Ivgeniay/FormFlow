import { TemplateDto } from "../../../../shared/api_types";

export interface TabProps {
	template: TemplateDto;
	accessToken: string | null;
}

export const AnalyticsTab: React.FC<TabProps> = ({}) => {
	return <></>;
};
