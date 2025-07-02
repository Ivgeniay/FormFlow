export default () => ({
    port: parseInt(process.env.PORT || `3001`, 10) || 3001,
    nodeEnv: process.env.NODE_ENV || 'development',

    openai: {
        apiKey: process.env.OPENAI_API_KEY,
        model: process.env.OPENAI_MODEL || 'gpt-4o-mini',
        maxTokens: parseInt(process.env.MAX_TOKENS || `10`, 10) || 2000,
        temperature: parseFloat(process.env.TEMPERATURE || `0,7`) || 0.7,
        topP: parseFloat(process.env.TOP_P || `1.0`) || 1.0,
    },

    backend: {
        apiUrl: process.env.BACKEND_API_URL || 'http://localhost:5030/api',
    },

    auth: {
        secret: process.env.AUTH_SECRET || 'fallback-secret',
    },
});
