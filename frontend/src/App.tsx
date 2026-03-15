import { useEffect } from "react";
import { RouterProvider } from "react-router-dom";
import { router } from "./app/routes";
import InstallPWAButton from "./components/InstallPWAButton";

export default function App() {
  useEffect(() => {
    const saved = localStorage.getItem("theme");
    if (saved === "dark") {
      document.documentElement.setAttribute("data-theme", "dark");
    }
  }, []);

  return (
    <>
      <RouterProvider router={router} />

      {/* Floating Install Button */}
      <div style={{ position: "fixed", bottom: 20, right: 20 }}>
        <InstallPWAButton />
      </div>
    </>
  );
}