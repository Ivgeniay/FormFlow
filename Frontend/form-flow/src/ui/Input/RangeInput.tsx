interface RangeInputProps {
	label: string;
	startValue: number;
	endValue?: number;
	onStartChange: (value: number) => void;
	onEndChange?: (value: number) => void;
	min: number;
	max: number;
	startWidth?: string;
	endWidth?: string;
	suffix?: string;
	showEndInput?: boolean;
	betweenText?: string;
}

export const RangeInput: React.FC<RangeInputProps> = ({
	label,
	startValue,
	endValue,
	onStartChange,
	onEndChange,
	min,
	max,
	startWidth = "w-20",
	endWidth = "w-20",
	suffix,
	showEndInput = false,
	betweenText = "to",
}) => {
	return (
		<div>
			<label className="block text-sm font-medium text-text mb-2">
				{label}
			</label>
			<div className="flex items-center gap-2">
				<input
					type="number"
					value={startValue}
					onChange={(e) => onStartChange(parseInt(e.target.value) || min)}
					className={`${startWidth} px-3 py-2 border border-border rounded-md bg-background text-text focus:border-primary focus:outline-none transition-colors`}
					min={min}
					max={max}
				/>
				{showEndInput ? (
					<>
						<span className="text-sm text-textMuted">{betweenText}</span>
						<input
							type="number"
							value={endValue}
							onChange={(e) => onEndChange?.(parseInt(e.target.value) || min)}
							className={`${endWidth} px-3 py-2 border border-border rounded-md bg-background text-text focus:border-primary focus:outline-none transition-colors`}
							min={min}
							max={max}
						/>
					</>
				) : (
					<span className="text-sm text-textMuted">{suffix}</span>
				)}
			</div>
		</div>
	);
};
