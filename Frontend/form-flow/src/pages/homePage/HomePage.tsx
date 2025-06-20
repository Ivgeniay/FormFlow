import { useEffect, useState } from "react";
import { AppLoader } from "../../components/AppLoader";
import { TagCloud } from "../../modules/Tag/components/TagCloud";
import { TemplateGallery } from "../../modules/templates/components/TemplateGallery";
import { mockTags, mockTemplates } from "../../shared/mock_data";

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
