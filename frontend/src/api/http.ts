import axios from "axios";

export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL?.replace(/\/+$/, "") ?? "/api";

export const http = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Attach JWT token for protected endpoints
http.interceptors.request.use((config) => {
  const token = localStorage.getItem("token"); // keep same storage key
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// (Optional but useful) auto-logout if token is invalid/expired
http.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err?.response?.status === 401) {
      // token invalid/expired -> clear and let UI redirect via ProtectedRoute
      localStorage.removeItem("token");
    }
    return Promise.reject(err);
  }
);