export const StatCard: React.FC<{
	title: string;
	value: string | number;
	subtitle?: string;
	variant?: "default" | "success" | "warning" | "error";
}> = ({ title, value, subtitle, variant = "default" }) => {
	const variantClasses = {
		default: "bg-surface border-border",
		success: "bg-success/10 border-success/30",
		warning: "bg-warning/10 border-warning/30",
		error: "bg-error/10 border-error/30",
	};

	return (
		<div className={`border rounded-lg p-4 ${variantClasses[variant]}`}>
			<div className="text-2xl font-bold text-text">{value}</div>
			<div className="text-sm font-medium text-text mb-1">{title}</div>
			{subtitle && <div className="text-xs text-textMuted">{subtitle}</div>}
		</div>
	);
};
