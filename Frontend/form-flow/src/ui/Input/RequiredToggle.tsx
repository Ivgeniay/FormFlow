import { useTranslation } from "react-i18next";

interface RequiredToggleProps {
	value: boolean;
	onChange: (value: boolean) => void;
	isReadOnly?: boolean;
}

export const RequiredToggle: React.FC<RequiredToggleProps> = ({
	value,
	onChange,
	isReadOnly = false,
}) => {
	const { t } = useTranslation();

	return (
		<div className="flex items-center gap-2">
			<span className="text-sm text-textMuted">
				{t("required") || "Required"}
			</span>
			<button
				onClick={() => onChange(!value)}
				disabled={isReadOnly}
				className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors ${
					value ? "bg-primary" : "bg-border"
				}`}
			>
				<span
					className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
						value ? "translate-x-6" : "translate-x-1"
					}`}
				/>
			</button>
		</div>
	);
};
