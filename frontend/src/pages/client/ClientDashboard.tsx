import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { getInstruments } from "../../api/instruments.api";
import type { Instrument } from "../../types/instrument";
import { clearAuth, getRole } from "../../app/auth.store";

export default function ClientDashboard() {
  const nav = useNavigate();

  const [items, setItems] = useState<Instrument[]>([]);
  const [err, setErr] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const role = getRole();

  useEffect(() => {
    let mounted = true;

    (async () => {
      try {
        setErr(null);
        setLoading(true);

        const data = await getInstruments();
        if (mounted) setItems(data);
      } catch (ex: any) {
        if (mounted) setErr(ex?.response?.data?.error ?? "Failed to load instruments");
      } finally {
        if (mounted) setLoading(false);
      }
    })();

    return () => {
      mounted = false;
    };
  }, []);

  function onLogout() {
    clearAuth();
    nav("/login", { replace: true });
  }

  return (
    <div className="container">
      <div className="row" style={{ justifyContent: "space-between", alignItems: "center" }}>
        <h2>TradingSim Dashboard</h2>
        <button className="btn" onClick={onLogout}>
          Logout
        </button>
      </div>

      <div className="card">
        <div className="row">
          <Link to="/client/place-order">Place Order</Link>
          <Link to="/client/orders">My Open Orders</Link>
          <Link to="/client/trades">My Trades</Link>
          <Link to="/client/power">Deposit Balance</Link>

          {role === "Admin" && (
            <>
              <Link to="/admin">Admin Home</Link>
              <Link to="/admin/instruments">Manage Instruments</Link>
              <Link to="/admin/trades">All Trades</Link>
              <Link to="/admin/power">Review Deposits</Link>
            </>
          )}
        </div>
      </div>

      <h3>Instruments</h3>
      {err && <div style={{ color: "crimson" }}>{err}</div>}

      {loading ? (
        <div>Loading instruments...</div>
      ) : (
        <table className="table">
          <thead>
            <tr>
              <th>Symbol</th>
              <th>Name</th>
              <th>Last Price</th>
              <th>Max Qty</th>
            </tr>
          </thead>
          <tbody>
            {items.map((x) => (
              <tr key={x.id}>
                <td>
                  <span className="badge">{x.symbol}</span>
                </td>
                <td>{x.name}</td>
                <td>{x.lastPrice}</td>
                <td>{x.maxQuantity}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}