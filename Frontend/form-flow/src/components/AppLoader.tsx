import React from 'react';

interface AppLoaderProps {
    showError?: boolean;
    onRetry?: () => void;
}

export const AppLoader: React.FC<AppLoaderProps> = ({ showError = false, onRetry }) => {
    return (
        <div className="min-h-screen bg-gray-100 flex items-center justify-center">
            <div className="flex flex-col items-center">
                <h1 className="text-4xl font-bold text-primary mb-8">FormFlow</h1>

                {showError ? (
                <div className="flex flex-col items-center">
                    <div className="text-red-600 mb-4">
                    <svg className="w-12 h-12" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    </div>
                    <button onClick={onRetry} className="px-6 py-3 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
                    ↻
                    </button>
                </div>
                ) : (
                <div className="animate-spin rounded-full h-12 w-12 border-4 border-gray-300 border-t-primary"></div>
                )}
        </div>
    </div>
    );
}