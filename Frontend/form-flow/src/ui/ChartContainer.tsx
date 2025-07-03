import React, { useEffect, useRef } from "react";
import * as am5 from "@amcharts/amcharts5";
import * as am5xy from "@amcharts/amcharts5/xy";
import am5themes_Animated from "@amcharts/amcharts5/themes/Animated";

interface ChartData {
	date?: string;
	hour?: number;
	month?: string;
	count: number;
}

interface ChartContainerProps {
	title: string;
	chartId: string;
	data: ChartData[];
	chartType: "line" | "column";
	height?: string;
}

export const ChartContainer: React.FC<ChartContainerProps> = ({
	title,
	chartId,
	data,
	chartType,
	height = "300px",
}) => {
	const chartRef = useRef<am5.Root | null>(null);

	useEffect(() => {
		if (!data || data.length === 0) return;

		const root = am5.Root.new(chartId);
		chartRef.current = root;

		root.setThemes([am5themes_Animated.new(root)]);

		const chart = root.container.children.push(
			am5xy.XYChart.new(root, {
				panX: true,
				panY: true,
				wheelX: "panX",
				wheelY: "zoomX",
				pinchZoomX: true,
			})
		);

		const getComputedColor = (
			className: string,
			property: string,
			fallback: string
		) => {
			const element = document.createElement("div");
			element.className = className;
			document.body.appendChild(element);
			const color =
				getComputedStyle(element).getPropertyValue(property) || fallback;
			document.body.removeChild(element);
			return color.trim();
		};

		const textColor = getComputedColor("text-text", "color", "#374151");
		const borderColor = getComputedColor(
			"border-border",
			"border-color",
			"#e5e7eb"
		);
		const primaryColor = getComputedColor(
			"bg-primary",
			"background-color",
			"#3b82f6"
		);

		const xRenderer = am5xy.AxisRendererX.new(root, {
			minGridDistance: 30,
		});
		xRenderer.labels.template.setAll({
			fontSize: 12,
			fill: am5.color(textColor),
		});
		xRenderer.grid.template.setAll({
			stroke: am5.color(borderColor),
			strokeOpacity: 0.5,
		});

		const yRenderer = am5xy.AxisRendererY.new(root, {});
		yRenderer.labels.template.setAll({
			fontSize: 12,
			fill: am5.color(textColor),
		});
		yRenderer.grid.template.setAll({
			stroke: am5.color(borderColor),
			strokeOpacity: 0.5,
		});

		const xAxis = chart.xAxes.push(
			am5xy.CategoryAxis.new(root, {
				categoryField: data[0].date
					? "date"
					: data[0].hour !== undefined
					? "hour"
					: "month",
				renderer: xRenderer,
			})
		);

		const yAxis = chart.yAxes.push(
			am5xy.ValueAxis.new(root, {
				renderer: yRenderer,
			})
		);

		let series: am5xy.LineSeries | am5xy.ColumnSeries;

		if (chartType === "line") {
			series = chart.series.push(
				am5xy.LineSeries.new(root, {
					name: "Submissions",
					xAxis: xAxis,
					yAxis: yAxis,
					valueYField: "count",
					categoryXField: data[0].date
						? "date"
						: data[0].hour !== undefined
						? "hour"
						: "month",
				})
			);
			(series as am5xy.LineSeries).set("stroke", am5.color(primaryColor));
			(series as am5xy.LineSeries).strokes.template.setAll({
				strokeWidth: 3,
			});
		} else {
			series = chart.series.push(
				am5xy.ColumnSeries.new(root, {
					name: "Submissions",
					xAxis: xAxis,
					yAxis: yAxis,
					valueYField: "count",
					categoryXField: data[0].date
						? "date"
						: data[0].hour !== undefined
						? "hour"
						: "month",
				})
			);
			(series as am5xy.ColumnSeries).set("fill", am5.color(primaryColor));
			(series as am5xy.ColumnSeries).columns.template.setAll({
				cornerRadiusTL: 4,
				cornerRadiusTR: 4,
			});
		}

		series.data.setAll(data);
		xAxis.data.setAll(data);

		chart.set("cursor", am5xy.XYCursor.new(root, {}));

		return () => {
			if (chartRef.current) {
				chartRef.current.dispose();
			}
		};
	}, [chartId, data, chartType]);

	return (
		<div className="bg-surface border border-border rounded-lg p-4">
			<h3 className="text-lg font-semibold text-text mb-4">{title}</h3>
			<div id={chartId} style={{ width: "100%", height }}></div>
		</div>
	);
};
