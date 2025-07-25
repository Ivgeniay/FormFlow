import ReactDOM from "react-dom/client";
import "./index.css";
import App from "./App";
import i18n from "./config/i18n";

const root = ReactDOM.createRoot(
	document.getElementById("root") as HTMLElement
);
root.render(<App />);
