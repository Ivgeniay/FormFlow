import { useTranslation } from "react-i18next";

export const ActionButton: React.FC<{
	onClick: () => void;
	loading: boolean;
	variant?: "default" | "danger" | "warning";
	children: React.ReactNode;
}> = ({ onClick, loading, variant = "default", children }) => {
	const { t } = useTranslation();

	const variantClasses = {
		default: "bg-primary hover:bg-primary/90 text-white",
		danger: "bg-error hover:bg-error/90 text-white",
		warning: "bg-warning hover:bg-warning/90 text-white",
	};

	return (
		<button
			onClick={onClick}
			disabled={loading}
			className={`px-4 py-2 rounded-lg font-medium transition-colors disabled:opacity-50 ${variantClasses[variant]}`}
		>
			{loading ? t("processing", "Processing...") : children}
		</button>
	);
};
