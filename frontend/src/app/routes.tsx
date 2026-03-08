import { createBrowserRouter } from "react-router-dom";
import ProtectedRoute from "../components/common/protectedRoute";

import LoginPage from "../pages/auth/LoginPage";
import SignupPage from "../pages/auth/SignupPage";

import ClientDashboard from "../pages/client/ClientDashboard";
import PlaceOrderPage from "../pages/client/PlaceOrderPage";
import MyOrdersPage from "../pages/client/MyOrdersPage";
import MyTradesPage from "../pages/client/MyTradesPage";

import AdminDashboard from "../pages/admin/AdminDashboard";
import InstrumentsAdminPage from "../pages/admin/InstrumentsAdminPage";
import AllTradesPage from "../pages/admin/AllTradesPage";
import PurchasePowerAdminPage from "../pages/admin/PurchasePowerAdminPage";
import PurchasePowerPage from "../pages/client/PurchasePowerPage";

export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  { path: "/signup", element: <SignupPage /> },

  // Home (both roles allowed)
  {
    path: "/",
    element: (
      <ProtectedRoute allow={["Client", "Admin"]}>
        <ClientDashboard />
      </ProtectedRoute>
    ),
  },

  // Client-only routes (matches backend: Orders + Trades/me)
  {
    path: "/client/place-order",
    element: (
      <ProtectedRoute allow={["Client"]}>
        <PlaceOrderPage />
      </ProtectedRoute>
    ),
  },
  {
    path: "/client/orders",
    element: (
      <ProtectedRoute allow={["Client"]}>
        <MyOrdersPage />
      </ProtectedRoute>
    ),
  },
  {
    path: "/client/trades",
    element: (
      <ProtectedRoute allow={["Client"]}>
        <MyTradesPage />
      </ProtectedRoute>
    ),
  },

  {
    path: "/client/power",
    element: (
      <ProtectedRoute allow={["Client"]}>
        <PurchasePowerPage />
      </ProtectedRoute>
    ),
  },

  // Admin-only routes (matches backend: Instruments POST + Trades GET)
  {
    path: "/admin",
    element: (
      <ProtectedRoute allow={["Admin"]}>
        <AdminDashboard />
      </ProtectedRoute>
    ),
  },
  {
    path: "/admin/power",
    element: (
      <ProtectedRoute allow={["Admin"]}>
        <PurchasePowerAdminPage />
      </ProtectedRoute>
    ),
  },
  {
    path: "/admin/instruments",
    element: (
      <ProtectedRoute allow={["Admin"]}>
        <InstrumentsAdminPage />
      </ProtectedRoute>
    ),
  },
  {
    path: "/admin/trades",
    element: (
      <ProtectedRoute allow={["Admin"]}>
        <AllTradesPage />
      </ProtectedRoute>
    ),
  },
]);