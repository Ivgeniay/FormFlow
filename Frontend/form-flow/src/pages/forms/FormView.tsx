import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { formApi } from "../../api/formApi";
import { FormDto } from "../../shared/api_types";
import toast from "react-hot-toast";
import { AppLoader } from "../../components/AppLoader";
import { FormattedTextDisplay } from "../../ui/Input/FormattedTextDisplay";

export const FormView: React.FC = () => {
	const { t } = useTranslation();
	const { formId } = useParams<{ formId: string }>();
	const navigate = useNavigate();
	const { accessToken, isAuthenticated } = useAuth();

	const [loading, setLoading] = useState(true);
	const [form, setForm] = useState<FormDto | null>(null);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		if (!isAuthenticated) {
			navigate("/login");
			return;
		}

		if (!formId || !accessToken) {
			setError(t("invalidFormId", "Invalid form ID"));
			setLoading(false);
			return;
		}

		loadForm();
	}, [formId, accessToken, isAuthenticated]);

	const loadForm = async () => {
		if (!formId || !accessToken) return;

		try {
			setLoading(true);
			setError(null);

			const formData = await formApi.getForm(formId, accessToken);
			setForm(formData);
		} catch (error: any) {
			const errorMessage =
				error.response?.data?.message || "Failed to load form";
			setError(errorMessage);
			toast.error(errorMessage);
		} finally {
			setLoading(false);
		}
	};

	const handleGoBack = () => {
		if (window.history.length > 1) {
			navigate(-1);
		} else {
			navigate("/");
		}
	};

	if (loading) {
		return <AppLoader isVisible={true} />;
	}

	if (error) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center p-4">
				<div className="max-w-md w-full bg-surface border border-border rounded-lg p-6 text-center">
					<div className="text-error text-4xl mb-4">⚠️</div>
					<h1 className="text-xl font-semibold text-text mb-2">
						{t("error", "Error")}
					</h1>
					<p className="text-textMuted mb-4">{error}</p>
					<button
						onClick={handleGoBack}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
					>
						{t("goBack", "Go Back")}
					</button>
				</div>
			</div>
		);
	}

	if (!form) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center p-4">
				<div className="max-w-md w-full bg-surface border border-border rounded-lg p-6 text-center">
					<h1 className="text-xl font-semibold text-text mb-2">
						{t("formNotFound", "Form not found")}
					</h1>
					<button
						onClick={handleGoBack}
						className="px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90 transition-opacity"
					>
						{t("goBack", "Go Back")}
					</button>
				</div>
			</div>
		);
	}

	return (
		<div className="min-h-screen bg-background p-8">
			<div className="max-w-2xl mx-auto">
				<button
					onClick={handleGoBack}
					className="inline-flex items-center gap-2 text-textMuted hover:text-text transition-colors mb-6"
				>
					<svg
						className="w-4 h-4"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M15 19l-7-7 7-7"
						/>
					</svg>
					{t("back", "Back")}
				</button>

				<div className="bg-surface border border-border rounded-lg p-6 mb-6">
					<div className="text-2xl font-bold text-text mb-2">
						<FormattedTextDisplay value={form.templateName} />
					</div>
					<div className="flex items-center gap-4 text-sm text-textMuted">
						<span>
							{t("submittedBy", "Submitted by")}:{" "}
							<FormattedTextDisplay value={form.userName} />
						</span>
						<span>
							{t("submittedOn", "Submitted on")}:{" "}
							{new Date(form.submittedAt).toLocaleDateString()}
						</span>
					</div>
				</div>

				<div className="bg-surface border border-border rounded-lg p-6">
					{form.questions.map((question) => (
						<div key={question.questionId} className="mb-6 last:mb-0">
							<div className="font-medium text-text mb-2">
								<FormattedTextDisplay value={question.title} />
							</div>
							{question.description && (
								<div className="text-textMuted text-sm mb-3">
									<FormattedTextDisplay value={question.description} />
								</div>
							)}
							<div className="p-2 bg-background border border-border rounded">
								<div className="text-text">
									<FormattedTextDisplay
										value={
											question.answer?.toString() || t("noAnswer", "No answer")
										}
									/>
								</div>
							</div>
						</div>
					))}
				</div>
			</div>
		</div>
	);
};
