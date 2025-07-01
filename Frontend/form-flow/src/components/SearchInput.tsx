import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { searchApi } from "../api/searchApi";
import toast from "react-hot-toast";

interface SearchInputProps {
	placeholder: string;
	placeholderDefault: string;
	className?: string;
	onSearch?: (query: string) => void;
}

export const SearchInput = ({
	placeholder,
	placeholderDefault,
	onSearch,
	className,
}: SearchInputProps) => {
	const { t } = useTranslation();
	const [query, setQuery] = useState("");
	const [suggestions, setSuggestions] = useState<string[]>([]);
	const [showSuggestions, setShowSuggestions] = useState(false);
	const [loading, setLoading] = useState(false);
	const inputRef = useRef<HTMLInputElement>(null);
	const dropdownRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (
				dropdownRef.current &&
				!dropdownRef.current.contains(event.target as Node) &&
				!inputRef.current?.contains(event.target as Node)
			) {
				setShowSuggestions(false);
			}
		};

		document.addEventListener("mousedown", handleClickOutside);
		return () => document.removeEventListener("mousedown", handleClickOutside);
	}, []);

	const loadSuggestions = async (searchQuery: string) => {
		if (searchQuery.trim().length < 2) {
			setSuggestions([]);
			setShowSuggestions(false);
			return;
		}

		try {
			setLoading(true);
			const query = searchQuery.trim();
			const results = await searchApi.getSearchSuggestions(query, 5);
			setSuggestions(results);
			setShowSuggestions(results.length > 0);
		} catch (error: any) {
			console.error("Error loading suggestions:", error);
			toast.error("Error loading suggestions:", error.code);
			setSuggestions([]);
			setShowSuggestions(false);
		} finally {
			setLoading(false);
		}
	};

	const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const value = e.target.value;
		setQuery(value);

		const timeoutId = setTimeout(() => {
			loadSuggestions(value);
		}, 300);

		return () => clearTimeout(timeoutId);
	};

	const handleSearch = (searchQuery: string) => {
		setShowSuggestions(false);
		if (onSearch) {
			onSearch(searchQuery);
		}
	};

	const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
		if (e.key === "Enter") {
			handleSearch(query);
		} else if (e.key === "Escape") {
			setShowSuggestions(false);
		}
	};

	const handleSuggestionClick = (suggestion: string) => {
		setQuery(suggestion);
		handleSearch(suggestion);
	};

	return (
		<div className={`relative ${className}`}>
			<div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
				<svg
					className="h-5 w-5 text-textMuted"
					fill="none"
					stroke="currentColor"
					viewBox="0 0 24 24"
				>
					<path
						strokeLinecap="round"
						strokeLinejoin="round"
						strokeWidth={2}
						d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
					/>
				</svg>
			</div>
			<input
				ref={inputRef}
				type="text"
				value={query}
				placeholder={placeholder || placeholderDefault}
				onChange={handleInputChange}
				onKeyDown={handleKeyDown}
				onFocus={() =>
					query.length >= 2 &&
					suggestions.length > 0 &&
					setShowSuggestions(true)
				}
				className="w-full pl-10 pr-4 py-1.5 bg-background border border-border rounded-lg text-text placeholder-textMuted focus:outline-none focus:ring-2 focus:ring-primary transition-colors"
			/>

			{showSuggestions && (
				<div
					ref={dropdownRef}
					className="absolute top-full left-0 right-0 mt-1 bg-surface border border-border rounded-lg shadow-lg z-50 max-h-60 overflow-y-auto"
				>
					{loading ? (
						<div className="px-4 py-2 text-textMuted text-sm">
							{t("loading", "Loading...")}
						</div>
					) : (
						suggestions.map((suggestion, index) => (
							<button
								key={index}
								onClick={() => handleSuggestionClick(suggestion)}
								className="w-full px-4 py-2 text-left hover:bg-background transition-colors text-text text-sm border-b border-border last:border-b-0"
							>
								{suggestion}
							</button>
						))
					)}
				</div>
			)}
		</div>
	);
};
