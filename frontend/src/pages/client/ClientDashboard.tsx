import { useEffect, useState, useRef } from "react";
import { Link, useNavigate } from "react-router-dom";
import { getInstruments } from "../../api/instruments.api";
import { getBalance } from "../../api/power.api";
import type { Instrument } from "../../types/instrument";
import { clearAuth, getRole } from "../../app/auth.store";
import ThemeToggle from "../../components/common/ThemeToggle";

import PlaceOrder from "../../components/client/PlaceOrder";
import MyOpenOrders from "../../components/client/MyOpenOrders";
import type { MyOpenOrdersRef } from "../../components/client/MyOpenOrders";
import MyTrades from "../../components/client/MyTrades";
import type { MyTradesRef } from "../../components/client/MyTrades";

export default function ClientDashboard() {
  const nav = useNavigate();
  const role = getRole();

  const [items, setItems] = useState<Instrument[]>([]);
  const [balance, setBalance] = useState<number | null>(null);
  const [err, setErr] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const ordersRef = useRef<MyOpenOrdersRef>(null);
  const tradesRef = useRef<MyTradesRef>(null);

  async function loadInitialData() {
    try {
      setErr(null);
      setLoading(true);

      const [insts, bal] = await Promise.all([
        getInstruments(),
        getBalance()
      ]);

      setItems(insts);
      setBalance(bal);
    } catch (ex: any) {
      setErr(ex?.response?.data?.error ?? "Failed to load dashboard data");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadInitialData();
  }, []);

  function onOrderPlaced() {
    // Refresh sidebar data
    ordersRef.current?.load();
    tradesRef.current?.load();
    // Refresh balance and instruments
    loadInitialData();
  }

  function onLogout() {
    clearAuth();
    nav("/login", { replace: true });
  }

  return (
    <div className="container" style={{ maxWidth: 1200 }}>
      {/* Header */}
      <div className="row" style={{ justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
        <div>
          <h2 style={{ margin: 0 }}>TradingSim Dashboard</h2>
          {role === "Client" && balance !== null && (
            <div style={{ fontSize: 14, marginTop: 4, opacity: 0.8 }}>
              Purchase Power: <b style={{ color: "#2E7D32" }}>৳ {balance.toLocaleString()}</b>
            </div>
          )}
        </div>
        <div className="row" style={{ gap: "12px" }}>
          <ThemeToggle />
          <button className="btn" onClick={onLogout}>
            Logout
          </button>
        </div>
      </div>

      {/* Admin Quick Nav */}
      {role === "Admin" && (
        <div className="card" style={{ marginBottom: 20 }}>
          <div className="row">
            <Link to="/admin">Admin Home</Link>
            <Link to="/admin/instruments">Manage Instruments</Link>
            <Link to="/admin/trades">All Trades</Link>
            <Link to="/admin/power">Review Deposits</Link>
          </div>
        </div>
      )}

      {/* Quick Actions (Deposit only) */}
      {role === "Client" && (
        <div className="card" style={{ marginBottom: 20, padding: "10px 14px" }}>
          <div className="row">
            <Link to="/client/power"><b>+ Deposit Balance</b></Link>
          </div>
        </div>
      )}

      {/* Main Grid */}
      <div style={{
        display: "grid",
        gridTemplateColumns: role === "Client" ? "350px 1fr" : "1fr",
        gap: 20
      }}>

        {/* Left Col: Place Order (Client Only) */}
        {role === "Client" && (
          <div style={{ display: "flex", flexDirection: "column", gap: 20 }}>
            <PlaceOrder
              instruments={items}
              loadingInstruments={loading}
              onOrderPlaced={onOrderPlaced}
            />
            <MyOpenOrders ref={ordersRef} />
          </div>
        )}

        {/* Right Col: Instruments & Trades */}
        <div style={{ display: "flex", flexDirection: "column", gap: 20 }}>

          <div className="card" style={{ margin: 0 }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
              <h3 style={{ margin: 0 }}>Market Instruments</h3>
              <button className="btn" style={{ padding: "4px 8px", fontSize: 12 }} onClick={loadInitialData}>↻</button>
            </div>

            {loading && items.length === 0 ? (
              <div>Loading market...</div>
            ) : (
              <div style={{ overflowX: "auto" }}>
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
                        <td><span className="badge">{x.symbol}</span></td>
                        <td>{x.name}</td>
                        <td>৳ {x.lastPrice}</td>
                        <td>{x.maxQuantity.toLocaleString()}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>

          {role === "Client" && (
            <MyTrades ref={tradesRef} />
          )}
        </div>
      </div>

      {err && (
        <div className="card" style={{ borderColor: "crimson", color: "crimson", marginTop: 20 }}>
          {err}
        </div>
      )}
    </div>
  );
}