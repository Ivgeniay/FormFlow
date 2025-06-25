import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../modules/auth/hooks/useAuth";
import { formApi, FormAccessResponse } from "../../api/formApi";
import toast from "react-hot-toast";
import { AppLoader } from "../../components/AppLoader";
import { FormRenderer } from "../../modules/forms/components/FormRenderer";

interface FormPageProps {}

export const FormPage: React.FC<FormPageProps> = () => {
	const { t } = useTranslation();
	const { id } = useParams<{ id: string }>();
	const navigate = useNavigate();
	const { accessToken, isAuthenticated } = useAuth();

	const [loading, setLoading] = useState(true);
	const [imageError, setImageError] = useState(false);
	const [formAccess, setFormAccess] = useState<FormAccessResponse | null>(null);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		if (!isAuthenticated) {
			navigate("/login");
			return;
		}

		if (!id || !accessToken) {
			setError("Invalid template ID");
			setLoading(false);
			return;
		}

		loadFormAccess();
	}, [id, accessToken, isAuthenticated]);

	const loadFormAccess = async () => {
		if (!id || !accessToken) return;

		try {
			setLoading(true);
			setError(null);

			const access = await formApi.getFormAccess(id, accessToken);
			console.log(access);
			setFormAccess(access);
		} catch (error: any) {
			const errorMessage =
				error.response?.data?.message || "Failed to load form access";
			setError(errorMessage);
			toast.error(errorMessage);
		} finally {
			setLoading(false);
		}
	};

	const submitForm = async (
		answers: Record<string, any>,
		sendCopyToEmail: boolean
	): Promise<void> => {
		console.log(answers, sendCopyToEmail);
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

	if (!formAccess) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center p-4">
				<div className="max-w-md w-full bg-surface border border-border rounded-lg p-6 text-center">
					<h1 className="text-xl font-semibold text-text mb-2">
						{t("noDataAvailable", "No data available")}
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

	if (!formAccess.canFillForm && !formAccess.hasAlreadySubmitted) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center p-4">
				<div className="max-w-md w-full bg-surface border border-border rounded-lg p-6 text-center">
					<div className="text-warning text-4xl mb-4">🚫</div>
					<h1 className="text-xl font-semibold text-text mb-2">
						{t("accessDenied", "Access Denied")}
					</h1>
					<p className="text-textMuted mb-4">
						{formAccess.denialReason ||
							t("noAccessToForm", "You don't have access to this form")}
					</p>
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

	if (formAccess.hasAlreadySubmitted && formAccess.existingForm) {
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
						<div className="flex items-center gap-3 mb-4">
							<div className="text-success text-2xl">✅</div>
							<div>
								<h1 className="text-xl font-semibold text-text">
									{t("formAlreadySubmitted", "Form Already Submitted")}
								</h1>
								<p className="text-textMuted">
									{t("formSubmittedOn", "You submitted this form on")}{" "}
									{new Date(
										formAccess.existingForm.submittedAt
									).toLocaleDateString()}
								</p>
							</div>
						</div>
					</div>

					<div className="text-textMuted text-center">
						{t(
							"formRendererReadonlyPlaceholder",
							"FormRenderer component will be here in readonly mode"
						)}
					</div>
				</div>
			</div>
		);
	}

	if (formAccess.canFillForm && formAccess.template) {
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

					<div className="bg-surface border border-border rounded-lg mb-2">
						<div className="w-full h-48 mb-4 rounded-lg overflow-hidden">
							{formAccess.template.imageUrl ? (
								<img
									src={formAccess.template.imageUrl}
									alt={formAccess.template.title}
									className="w-full h-full object-cover"
									onError={() => setImageError(true)}
								/>
							) : (
								<div className="w-full h-full bg-gradient-to-b from-primary to-primary/70 flex items-center justify-center">
									<svg
										className="w-16 h-16 text-white/70"
										fill="none"
										stroke="currentColor"
										viewBox="0 0 24 24"
									>
										<path
											strokeLinecap="round"
											strokeLinejoin="round"
											strokeWidth={1.5}
											d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
										/>
									</svg>
								</div>
							)}
						</div>
					</div>

					<div className="bg-surface border border-border rounded-lg p-6 mb-6">
						<h1 className="text-2xl font-bold text-text mb-2">
							{formAccess.template.title}
						</h1>
						{formAccess.template.description && (
							<p className="text-textMuted mb-4">
								{formAccess.template.description}
							</p>
						)}
						<div className="flex items-center gap-4 text-sm text-textMuted">
							<span>
								{t("author", "Author")}: {formAccess.template.authorName}
							</span>
							<span>
								{t("topic", "Topic")}: {formAccess.template.topic}
							</span>
						</div>
					</div>

					<FormRenderer
						template={formAccess.template}
						mode="fillable"
						onSubmit={submitForm}
					></FormRenderer>
				</div>
			</div>
		);
	}

	return null;
};
