import { usePWAInstall } from "../hooks/usePWAInstall";

export default function InstallPWAButton() {
  const { installApp, installPrompt } = usePWAInstall();

  if (!installPrompt) return null;

  return (
    <button onClick={installApp}>
      Install App
    </button>
  );
}