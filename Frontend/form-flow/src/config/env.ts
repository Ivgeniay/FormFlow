export const ENV = {
	API_URL: process.env.REACT_APP_API_URL!,
	SIGNALR_URL: process.env.REACT_APP_SIGNALR_URL!,
	GOOGLE_CLIENT_ID: process.env.REACT_APP_GOOGLE_CLIENT_ID || "",
	GOOGLE_REDIRECT_URI:
		process.env.REACT_APP_GOOGLE_REDIRECT_URI ||
		"http://localhost:3000/auth/google/callback",
};
