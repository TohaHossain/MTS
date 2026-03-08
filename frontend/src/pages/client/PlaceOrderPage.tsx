import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { getInstruments } from "../../api/instruments.api";
import { placeOrder } from "../../api/orders.api";
import type { Instrument } from "../../types/instrument";
import type { OrderSide, OrderType, TimeInForce } from "../../types/order";

export default function PlaceOrderPage() {
  const [instruments, setInstruments] = useState<Instrument[]>([]);
  const [symbol, setSymbol] = useState<string>("");

  const [side, setSide] = useState<OrderSide>("Buy");
  const [type, setType] = useState<OrderType>("Limit");
  const [tif, setTif] = useState<TimeInForce>("Day");

  const [qty, setQty] = useState<number>(10);
  const [limitPrice, setLimitPrice] = useState<number>(0);

  const [msg, setMsg] = useState<string | null>(null);
  const [err, setErr] = useState<string | null>(null);

  const [busy, setBusy] = useState(false);
  const [loadingInst, setLoadingInst] = useState(false);

  const selectedInstrument = useMemo(
    () => instruments.find((i) => i.symbol === symbol) ?? null,
    [instruments, symbol]
  );

  useEffect(() => {
    let mounted = true;

    (async () => {
      try {
        setErr(null);
        setLoadingInst(true);

        const data = await getInstruments();
        if (!mounted) return;

        setInstruments(data);

        if (data.length > 0) {
          setSymbol(data[0].symbol);
          setLimitPrice(data[0].lastPrice);
        }
      } catch (ex: any) {
        if (!mounted) return;
        setErr(ex?.response?.data?.error ?? "Failed to load instruments");
      } finally {
        if (mounted) setLoadingInst(false);
      }
    })();

    return () => {
      mounted = false;
    };
  }, []);

  function onSymbolChange(v: string) {
    setSymbol(v);
    const inst = instruments.find((i) => i.symbol === v);
    if (inst) setLimitPrice(inst.lastPrice);
  }

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setErr(null);
    setMsg(null);

    if (!symbol) {
      setErr("Please select an instrument");
      return;
    }
    if (!Number.isFinite(qty) || qty <= 0) {
      setErr("Quantity must be greater than 0");
      return;
    }
    if (type === "Limit" && (!Number.isFinite(limitPrice) || limitPrice <= 0)) {
      setErr("Limit price must be greater than 0");
      return;
    }

    setBusy(true);
    try {
      const res = await placeOrder({
        symbol,
        side,
        type,
        timeInForce: tif,
        quantity: qty,
        limitPrice: type === "Limit" ? limitPrice : null,
      });

      setMsg(`Order placed: ${res.id} | Status: ${res.status}`);
    } catch (ex: any) {
      setErr(ex?.response?.data?.error ?? "Place order failed");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="container">
      <div className="row" style={{ justifyContent: "space-between", alignItems: "center" }}>
        <h2>Place Order</h2>
        <Link to="/">Back</Link>
      </div>

      <form className="card" onSubmit={submit}>
        <div className="row">
          <div style={{ flex: 1, minWidth: 220 }}>
            <label>Instrument</label>
            <select
              className="input"
              value={symbol}
              onChange={(e) => onSymbolChange(e.target.value)}
              disabled={loadingInst || instruments.length === 0}
            >
              {instruments.map((i) => (
                <option key={i.id} value={i.symbol}>
                  {i.symbol} — {i.name} (Last: {i.lastPrice})
                </option>
              ))}
            </select>
            {loadingInst && <div style={{ marginTop: 6 }}>Loading instruments...</div>}
            {!loadingInst && instruments.length === 0 && (
              <div style={{ marginTop: 6, color: "crimson" }}>No instruments available</div>
            )}
          </div>

          <div style={{ width: 160 }}>
            <label>Side</label>
            <select className="input" value={side} onChange={(e) => setSide(e.target.value as any)}>
              <option value="Buy">Buy</option>
              <option value="Sell">Sell</option>
            </select>
          </div>

          <div style={{ width: 160 }}>
            <label>Type</label>
            <select className="input" value={type} onChange={(e) => setType(e.target.value as any)}>
              <option value="Limit">Limit</option>
              <option value="Market">Market</option>
            </select>
          </div>

          <div style={{ width: 160 }}>
            <label>TIF</label>
            <select className="input" value={tif} onChange={(e) => setTif(e.target.value as any)}>
              <option value="Day">Day</option>
              <option value="GTC">GTC</option>
              <option value="IOC">IOC</option>
              <option value="FOK">FOK</option>
            </select>
          </div>
        </div>

        <div className="row" style={{ marginTop: 12 }}>
          <div style={{ width: 160 }}>
            <label>Quantity</label>
            <input
              className="input"
              type="number"
              min={1}
              max={selectedInstrument?.maxQuantity}
              value={qty}
              onChange={(e) => {
                let val = Number(e.target.value);
                if (selectedInstrument && val > selectedInstrument.maxQuantity) {
                  val = selectedInstrument.maxQuantity;
                }
                setQty(val);
              }}
            />
          </div>

          {type === "Limit" && (
            <div style={{ width: 200 }}>
              <label>Limit Price</label>
              <input
                className="input"
                type="number"
                step="0.01"
                value={limitPrice}
                onChange={(e) => setLimitPrice(Number(e.target.value))}
              />
            </div>
          )}

          {/* optional hint, no UI change required */}
          {type === "Limit" && selectedInstrument && (
            <div style={{ alignSelf: "flex-end", marginLeft: 8, opacity: 0.8 }}>
              Last: {selectedInstrument.lastPrice}
            </div>
          )}
        </div>

        {err && <div style={{ marginTop: 10, color: "crimson" }}>{err}</div>}
        {msg && <div style={{ marginTop: 10, color: "green" }}>{msg}</div>}

        <button
          className="btn"
          style={{ marginTop: 14 }}
          type="submit"
          disabled={busy || loadingInst || instruments.length === 0}
        >
          {busy ? "Placing..." : "Place Order"}
        </button>
      </form>

      <div className="card">
        <b>Tip:</b> To test matching, place a <span className="badge">Buy Limit</span> from one user and a{" "}
        <span className="badge">Sell Limit</span> at a matchable price from another user.
      </div>
    </div>
  );
}