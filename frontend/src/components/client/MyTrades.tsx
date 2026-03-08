import { useEffect, useState, useImperativeHandle, forwardRef } from "react";
import { getMyTrades } from "../../api/trades.api";
import type { Trade } from "../../types/trade";

function formatUtc(iso: string) {
    const d = new Date(iso);
    return d.toLocaleString();
}

export interface MyTradesRef {
    load: () => void;
}

const MyTrades = forwardRef<MyTradesRef, {}>((_, ref) => {
    const [trades, setTrades] = useState<Trade[]>([]);
    const [err, setErr] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    async function load() {
        try {
            setErr(null);
            setLoading(true);
            const data = await getMyTrades();
            setTrades(data);
        } catch (ex: any) {
            setErr(ex?.response?.data?.error ?? "Failed to load trades");
        } finally {
            setLoading(false);
        }
    }

    useImperativeHandle(ref, () => ({
        load
    }));

    useEffect(() => {
        load();
    }, []);

    return (
        <div className="card" style={{ margin: 0, height: "100%", display: "flex", flexDirection: "column" }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
                <h3>Recent Trades</h3>
                <button className="btn" style={{ padding: "4px 8px", fontSize: 12 }} onClick={load} disabled={loading}>
                    ↻ Refresh
                </button>
            </div>

            {err && <div style={{ color: "crimson", margin: "10px 0" }}>{err}</div>}

            <div style={{ overflowY: "auto", flex: 1, maxHeight: 300 }}>
                <table className="table" style={{ fontSize: 13 }}>
                    <thead>
                        <tr>
                            <th>Symbol</th>
                            <th>Price</th>
                            <th>Qty</th>
                            <th>Executed</th>
                        </tr>
                    </thead>
                    <tbody>
                        {trades.map((t) => (
                            <tr key={t.id}>
                                <td><span className="badge">{t.symbol}</span></td>
                                <td>{t.price}</td>
                                <td>{t.quantity}</td>
                                <td style={{ fontSize: 11 }}>{formatUtc(t.executedUtc)}</td>
                            </tr>
                        ))}
                        {!loading && trades.length === 0 && (
                            <tr>
                                <td colSpan={4} style={{ textAlign: "center", padding: 20 }}>No trades found</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
});

export default MyTrades;
