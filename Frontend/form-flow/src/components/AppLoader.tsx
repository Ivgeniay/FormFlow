import React from 'react';

interface AppLoaderProps {
    showError?: boolean;
    onRetry?: () => void;
}

export const AppLoader: React.FC<AppLoaderProps> = ({ showError = false, onRetry }) => {
    return (
        <div className="min-h-screen bg-gray-100 dark:bg-gray-900 flex items-center justify-center">
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
                    <div className="animate-spin rounded-full h-12 w-12 border-4 border-gray-200 dark:border-gray-700 border-t-primary"></div>
                )}

<div className="p-5 bg-background text-text min-h-screen">
  <div className="bg-surface p-5 border border-border rounded-lg">
    <h1 className="text-primary text-3xl font-bold mb-4">Primary Color Heading</h1>
    
    <p className="text-text mb-2">Main text with primary text color</p>
    <p className="text-textMuted mb-4">Muted text for secondary information</p>
    
    <div className="bg-surface border border-border rounded p-4 mb-4">
      <h3 className="text-primary text-lg font-semibold mb-2">Card Example</h3>
      <p className="text-text mb-2">Card content with surface background</p>
      <p className="text-textMuted">Secondary text in card</p>
    </div>

    <div className="flex flex-wrap gap-4 mb-4">
      <button className="bg-primary text-white px-4 py-2 rounded hover:opacity-90">
        Primary Button
      </button>
      <button className="bg-surface border border-border text-text px-4 py-2 rounded hover:bg-background">
        Secondary Button
      </button>
    </div>

    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
      <div className="bg-success text-white p-3 rounded text-center">
        <span className="font-semibold">Success Message</span>
      </div>
      <div className="bg-warning text-white p-3 rounded text-center">
        <span className="font-semibold">Warning Message</span>
      </div>
      <div className="bg-error text-white p-3 rounded text-center">
        <span className="font-semibold">Error Message</span>
      </div>
    </div>

    <div className="border-l-4 border-primary bg-surface p-4 rounded">
      <h4 className="text-primary font-semibold">Information Block</h4>
      <p className="text-text">This is how information blocks will look</p>
      <p className="text-textMuted text-sm">With muted text for details</p>
    </div>
  </div>
</div>

        </div>
    </div>
    );
}