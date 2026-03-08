import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getBalance, getMyPowerRequests, requestPower } from "../../api/power.api";
import type { PowerRequestDto } from "../../types/power";

export default function PurchasePowerPage() {
    const [balance, setBalance] = useState<number | null>(null);
    const [requests, setRequests] = useState<PowerRequestDto[]>([]);
    const [amount, setAmount] = useState<number>(1000);

    const [loading, setLoading] = useState(false);
    const [busy, setBusy] = useState(false);
    const [err, setErr] = useState<string | null>(null);
    const [msg, setMsg] = useState<string | null>(null);

    async function loadData() {
        try {
            setLoading(true);
            setErr(null);
            const b = await getBalance();
            setBalance(b);
            const r = await getMyPowerRequests();
            setRequests(r);
        } catch (ex: any) {
            setErr(ex?.response?.data?.error ?? "Failed to load power data");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        let mounted = true;
        if (mounted) loadData();
        return () => { mounted = false; };
    }, []);

    async function onSubmit(e: React.FormEvent) {
        e.preventDefault();
        setErr(null);
        setMsg(null);

        if (amount < 1000 || amount > 10000000) {
            setErr("Amount must be between 1,000 and 10,000,000 Taka");
            return;
        }

        try {
            setBusy(true);
            const res = await requestPower({ amount });
            setMsg(res.message);
            setAmount(1000);
            await loadData();
        } catch (ex: any) {
            setErr(ex?.response?.data?.error ?? "Failed to request power");
        } finally {
            setBusy(false);
        }
    }

    return (
        <div className="container">
            <div className="row" style={{ justifyContent: "space-between", alignItems: "center" }}>
                <h2>Purchase Power</h2>
                <Link to="/">Back to Dashboard</Link>
            </div>

            <div className="card" style={{ padding: "20px", marginBottom: "20px" }}>
                <h3>Current Balance</h3>
                <p style={{ fontSize: "2rem", fontWeight: "bold", margin: "10px 0", color: "#2E7D32" }}>
                    {balance !== null ? `৳ ${balance.toLocaleString()}` : "Loading..."}
                </p>
            </div>

            <form className="card" onSubmit={onSubmit}>
                <h3>Request Deposit</h3>
                <p style={{ marginBottom: 15 }}>Enter an amount between 1,000 and 10,000,000 Taka</p>

                <div className="row">
                    <div style={{ flex: 1, minWidth: 200 }}>
                        <label>Amount (Taka)</label>
                        <input
                            className="input"
                            type="number"
                            min={1000}
                            max={10000000}
                            step={1000}
                            value={amount}
                            onChange={(e) => setAmount(Number(e.target.value))}
                        />
                    </div>
                </div>

                {err && <div style={{ marginTop: 10, color: "crimson" }}>{err}</div>}
                {msg && <div style={{ marginTop: 10, color: "green" }}>{msg}</div>}

                <button className="btn" style={{ marginTop: 14 }} type="submit" disabled={busy || loading}>
                    {busy ? "Requesting..." : "Submit Request"}
                </button>
            </form>

            <h3>My Requests</h3>
            {loading ? (
                <div>Loading requests...</div>
            ) : (
                <table className="table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Amount (Taka)</th>
                            <th>Status</th>
                            <th>Date</th>
                        </tr>
                    </thead>
                    <tbody>
                        {requests.map(r => (
                            <tr key={r.id}>
                                <td>{r.id.substring(r.id.length - 6)}</td>
                                <td>৳ {r.amount.toLocaleString()}</td>
                                <td>
                                    <span className="badge" style={{
                                        backgroundColor: r.status === 'Approved' ? '#2E7D32' : r.status === 'Declined' ? '#C62828' : '#F57F17'
                                    }}>
                                        {r.status}
                                    </span>
                                </td>
                                <td>{new Date(r.createdUtc).toLocaleString()}</td>
                            </tr>
                        ))}
                        {requests.length === 0 && (
                            <tr>
                                <td colSpan={4}>No requests found</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            )}
        </div>
    );
}
