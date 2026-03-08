import React, { useMemo, useState } from "react";
import { placeOrder } from "../../api/orders.api";
import type { Instrument } from "../../types/instrument";
import type { OrderSide, OrderType, TimeInForce } from "../../types/order";

interface Props {
    instruments: Instrument[];
    loadingInstruments: boolean;
    onOrderPlaced: () => void;
}

export default function PlaceOrder({ instruments, loadingInstruments, onOrderPlaced }: Props) {
    const [symbol, setSymbol] = useState<string>("");
    const [side, setSide] = useState<OrderSide>("Buy");
    const [type, setType] = useState<OrderType>("Limit");
    const [tif, setTif] = useState<TimeInForce>("Day");
    const [qty, setQty] = useState<number>(10);
    const [limitPrice, setLimitPrice] = useState<number>(0);
    const [msg, setMsg] = useState<string | null>(null);
    const [err, setErr] = useState<string | null>(null);
    const [busy, setBusy] = useState(false);

    const selectedInstrument = useMemo(
        () => instruments.find((i) => i.symbol === symbol) ?? null,
        [instruments, symbol]
    );

    React.useEffect(() => {
        if (instruments.length > 0 && !symbol) {
            setSymbol(instruments[0].symbol);
            setLimitPrice(instruments[0].lastPrice);
        }
    }, [instruments, symbol]);

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

            setMsg(`Order placed: ${res.status}`);
            onOrderPlaced();
        } catch (ex: any) {
            setErr(ex?.response?.data?.error ?? "Place order failed");
        } finally {
            setBusy(false);
        }
    }

    return (
        <form className="card" onSubmit={submit} style={{ margin: 0 }}>
            <h3>Place Order</h3>
            <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
                <div>
                    <label>Instrument</label>
                    <select
                        className="input"
                        value={symbol}
                        onChange={(e) => onSymbolChange(e.target.value)}
                        disabled={loadingInstruments || instruments.length === 0}
                    >
                        {instruments.map((i) => (
                            <option key={i.id} value={i.symbol}>
                                {i.symbol} — {i.name} (Last: {i.lastPrice})
                            </option>
                        ))}
                    </select>
                </div>

                <div style={{ display: "flex", gap: 8 }}>
                    <div style={{ flex: 1 }}>
                        <label>Side</label>
                        <select className="input" value={side} onChange={(e) => setSide(e.target.value as any)}>
                            <option value="Buy">Buy</option>
                            <option value="Sell">Sell</option>
                        </select>
                    </div>
                    <div style={{ flex: 1 }}>
                        <label>Type</label>
                        <select className="input" value={type} onChange={(e) => setType(e.target.value as any)}>
                            <option value="Limit">Limit</option>
                            <option value="Market">Market</option>
                        </select>
                    </div>
                </div>

                <div style={{ display: "flex", gap: 8 }}>
                    <div style={{ flex: 1 }}>
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
                    <div style={{ flex: 1 }}>
                        <label>TIF</label>
                        <select className="input" value={tif} onChange={(e) => setTif(e.target.value as any)}>
                            <option value="Day">Day</option>
                            <option value="GTC">GTC</option>
                            <option value="IOC">IOC</option>
                            <option value="FOK">FOK</option>
                        </select>
                    </div>
                </div>

                {type === "Limit" && (
                    <div>
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

                {err && <div style={{ color: "crimson", fontSize: 13 }}>{err}</div>}
                {msg && <div style={{ color: "green", fontSize: 13 }}>{msg}</div>}

                <button
                    className="btn"
                    type="submit"
                    disabled={busy || loadingInstruments || instruments.length === 0}
                    style={{ width: "100%", marginTop: 8 }}
                >
                    {busy ? "Placing..." : "Place Order"}
                </button>
            </div>
        </form>
    );
}
