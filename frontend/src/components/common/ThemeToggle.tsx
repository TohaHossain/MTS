import { useEffect, useState } from "react";

export default function ThemeToggle() {
    const [isDark, setIsDark] = useState(() => {
        return localStorage.getItem("theme") === "dark";
    });

    useEffect(() => {
        const root = document.documentElement;
        if (isDark) {
            root.setAttribute("data-theme", "dark");
            localStorage.setItem("theme", "dark");
        } else {
            root.removeAttribute("data-theme");
            localStorage.setItem("theme", "light");
        }
    }, [isDark]);

    return (
        <button
            className="btn"
            onClick={() => setIsDark(!isDark)}
            style={{
                display: "flex",
                alignItems: "center",
                gap: "6px",
                fontSize: "14px",
                padding: "6px 10px"
            }}
            title="Toggle Dark/Light Mode"
        >
            {isDark ? "🥐 Light Mode" : "🌙 Dark Mode"}
        </button>
    );
}
