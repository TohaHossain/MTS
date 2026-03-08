import { useEffect, useState, useImperativeHandle, forwardRef } from "react";
import { cancelOrder, getOpenOrders } from "../../api/orders.api";
import type { Order } from "../../types/order";

export interface MyOpenOrdersRef {
    load: () => void;
}

const MyOpenOrders = forwardRef<MyOpenOrdersRef, {}>((_, ref) => {
    const [orders, setOrders] = useState<Order[]>([]);
    const [err, setErr] = useState<string | null>(null);
    const [busyId, setBusyId] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    async function load() {
        try {
            setErr(null);
            setLoading(true);
            const data = await getOpenOrders();
            setOrders(data);
        } catch (ex: any) {
            setErr(ex?.response?.data?.error ?? "Failed to load orders");
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

    async function onCancel(id: string) {
        if (busyId) return;
        setBusyId(id);
        setErr(null);

        try {
            await cancelOrder(id);
            await load();
        } catch (ex: any) {
            setErr(ex?.response?.data?.error ?? "Cancel failed");
        } finally {
            setBusyId(null);
        }
    }

    return (
        <div className="card" style={{ margin: 0, height: "100%", display: "flex", flexDirection: "column" }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
                <h3>My Open Orders</h3>
                <button className="btn" style={{ padding: "4px 8px", fontSize: 12 }} onClick={load} disabled={loading}>
                    ↻ Refresh
                </button>
            </div>

            {err && <div style={{ color: "crimson", margin: "10px 0" }}>{err}</div>}

            <div style={{ overflowX: "auto", flex: 1 }}>
                <table className="table" style={{ fontSize: 13 }}>
                    <thead>
                        <tr>
                            <th>Symbol</th>
                            <th>Side</th>
                            <th>Type</th>
                            <th>Remaining</th>
                            <th>Limit</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders.map((o) => (
                            <tr key={o.id}>
                                <td><span className="badge">{o.symbol}</span></td>
                                <td style={{ color: o.side === "Buy" ? "green" : "crimson" }}>{o.side}</td>
                                <td>{o.type}</td>
                                <td>{o.remainingQuantity} / {o.quantity}</td>
                                <td>{o.limitPrice ?? "-"}</td>
                                <td>
                                    <button
                                        className="btn"
                                        disabled={loading || !!busyId}
                                        style={{ padding: "2px 6px", fontSize: 11 }}
                                        onClick={() => onCancel(o.id)}
                                    >
                                        {busyId === o.id ? "..." : "Cancel"}
                                    </button>
                                </td>
                            </tr>
                        ))}
                        {!loading && orders.length === 0 && (
                            <tr>
                                <td colSpan={6} style={{ textAlign: "center", padding: 20 }}>No open orders</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
});

export default MyOpenOrders;
