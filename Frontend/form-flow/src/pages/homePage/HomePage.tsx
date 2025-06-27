import { LatestTemplatesSection } from "../../modules/templates/components/sections/LatestTemplatesSection";
import { MyTemplatesSection } from "../../modules/templates/components/sections/MyTemplatesSection";
import { TagCloudSection } from "../../modules/Tag/components/TagCloudSection";

export const HomePage: React.FC = () => {
	return (
		<>
			<LatestTemplatesSection />
			<TagCloudSection />
			<MyTemplatesSection />
		</>
	);
};
