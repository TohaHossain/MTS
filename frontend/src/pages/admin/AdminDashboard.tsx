import { Link, useNavigate } from "react-router-dom";
import { clearAuth } from "../../app/auth.store";
import ThemeToggle from "../../components/common/ThemeToggle";

export default function AdminDashboard() {
  const nav = useNavigate();

  function onLogout() {
    clearAuth();
    nav("/login", { replace: true });
  }

  return (
    <div className="container">
      <div className="row" style={{ justifyContent: "space-between", alignItems: "center" }}>
        <h2>Admin Dashboard</h2>
        <div className="row" style={{ gap: "12px" }}>
          <ThemeToggle />
          <button className="btn" onClick={onLogout}>
            Logout
          </button>
        </div>
      </div>

      <div className="card">
        <div className="row">
          <Link to="/">Instruments (client view)</Link>
          <Link to="/admin/instruments">Manage Instruments</Link>
          <Link to="/admin/trades">All Trades</Link>
          <Link to="/admin/power">Review Deposits</Link>
        </div>
      </div>

      <div className="card">
        <b>Admin powers (demo):</b>
        <ul>
          <li>Add new instruments + set the reference price</li>
          <li>See all executed trades</li>
        </ul>
      </div>
    </div>
  );
}