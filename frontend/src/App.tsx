import { useEffect } from "react";
import { RouterProvider } from "react-router-dom";
import { router } from "./app/routes";

export default function App() {
  useEffect(() => {
    const saved = localStorage.getItem("theme");
    if (saved === "dark") {
      document.documentElement.setAttribute("data-theme", "dark");
    }
  }, []);

  return <RouterProvider router={router} />;
}