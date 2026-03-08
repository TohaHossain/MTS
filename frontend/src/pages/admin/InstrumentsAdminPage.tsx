import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { createInstrument, getInstruments } from "../../api/instruments.api";
import type { Instrument } from "../../types/instrument";

export default function InstrumentsAdminPage() {
  const [items, setItems] = useState<Instrument[]>([]);
  const [symbol, setSymbol] = useState("");
  const [name, setName] = useState("");
  const [price, setPrice] = useState<number>(100);
  const [maxQuantity, setMaxQuantity] = useState<number>(10000);

  const [err, setErr] = useState<string | null>(null);
  const [msg, setMsg] = useState<string | null>(null);

  const [loading, setLoading] = useState(false);
  const [creating, setCreating] = useState(false);

  async function load() {
    try {
      setErr(null);
      setLoading(true);
      const data = await getInstruments();
      setItems(data);
    } catch (ex: any) {
      setErr(ex?.response?.data?.error ?? "Failed to load instruments");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    let mounted = true;

    (async () => {
      await load();
    })();

    return () => {
      mounted = false;
      void mounted;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function onCreate(e: React.FormEvent) {
    e.preventDefault();
    setErr(null);
    setMsg(null);

    const s = symbol.trim().toUpperCase();
    const n = name.trim();

    if (!s) {
      setErr("Symbol is required");
      return;
    }
    if (!n) {
      setErr("Name is required");
      return;
    }
    if (!Number.isFinite(price) || price <= 0) {
      setErr("Last price must be greater than 0");
      return;
    }
    if (!Number.isFinite(maxQuantity) || maxQuantity <= 0) {
      setErr("Quantity must be greater than 0");
      return;
    }

    try {
      setCreating(true);

      const res = await createInstrument({ symbol: s, name: n, lastPrice: price, maxQuantity });
      setMsg(res.message);

      setSymbol("");
      setName("");
      setPrice(100);
      setMaxQuantity(10000);

      await load();
    } catch (ex: any) {
      setErr(ex?.response?.data?.error ?? "Create failed");
    } finally {
      setCreating(false);
    }
  }

  return (
    <div className="container">
      <div className="row" style={{ justifyContent: "space-between", alignItems: "center" }}>
        <h2>Manage Instruments</h2>
        <Link to="/admin">Back</Link>
      </div>

      <form className="card" onSubmit={onCreate}>
        <h3>Add Instrument</h3>

        <div className="row">
          <div style={{ flex: 1, minWidth: 180 }}>
            <label>Symbol</label>
            <input
              className="input"
              value={symbol}
              onChange={(e) => setSymbol(e.target.value)}
              placeholder="e.g. AAPL"
            />
          </div>

          <div style={{ flex: 2, minWidth: 220 }}>
            <label>Name</label>
            <input
              className="input"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Apple Inc."
            />
          </div>

          <div style={{ width: 180 }}>
            <label>Last Price</label>
            <input
              className="input"
              type="number"
              step="0.01"
              value={price}
              onChange={(e) => setPrice(Number(e.target.value))}
            />
          </div>

          <div style={{ width: 180 }}>
            <label>Max Quantity</label>
            <input
              className="input"
              type="number"
              step="1"
              value={maxQuantity}
              onChange={(e) => setMaxQuantity(Number(e.target.value))}
            />
          </div>
        </div>

        {err && <div style={{ marginTop: 10, color: "crimson" }}>{err}</div>}
        {msg && <div style={{ marginTop: 10, color: "green" }}>{msg}</div>}

        <button className="btn" style={{ marginTop: 12 }} type="submit" disabled={creating}>
          {creating ? "Creating..." : "Create"}
        </button>
      </form>

      <h3>Current Instruments</h3>
      {loading && <div>Loading instruments...</div>}

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
          {items.map((i) => (
            <tr key={i.id}>
              <td>
                <span className="badge">{i.symbol}</span>
              </td>
              <td>{i.name}</td>
              <td>{i.lastPrice}</td>
              <td>{i.maxQuantity}</td>
            </tr>
          ))}
          {!loading && items.length === 0 && (
            <tr>
              <td colSpan={4}>No instruments</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}