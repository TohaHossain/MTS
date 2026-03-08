import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getPendingPowerRequests, reviewPowerRequest } from "../../api/power.api";
import type { PowerRequestDto } from "../../types/power";

export default function PurchasePowerAdminPage() {
    const [requests, setRequests] = useState<PowerRequestDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [err, setErr] = useState<string | null>(null);

    async function loadPending() {
        try {
            setErr(null);
            setLoading(true);
            const data = await getPendingPowerRequests();
            setRequests(data);
        } catch (ex: any) {
            setErr(ex?.response?.data?.error ?? "Failed to load requests");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        let mounted = true;
        if (mounted) loadPending();
        return () => { mounted = false; };
    }, []);

    async function handleReview(id: string, approve: boolean) {
        try {
            await reviewPowerRequest(id, { approve });
            await loadPending();
        } catch (ex: any) {
            alert(ex?.response?.data?.error ?? "Action failed");
        }
    }

    return (
        <div className="container">
            <div className="row" style={{ justifyContent: "space-between", alignItems: "center" }}>
                <h2>Review Power Requests</h2>
                <Link to="/admin">Back to Admin</Link>
            </div>

            <div className="card" style={{ marginTop: 20 }}>
                <h3>Pending Deposits</h3>
                {err && <div style={{ color: "crimson" }}>{err}</div>}

                {loading ? (
                    <div>Loading...</div>
                ) : (
                    <table className="table" style={{ marginTop: 10 }}>
                        <thead>
                            <tr>
                                <th>User ID</th>
                                <th>Email</th>
                                <th>Amount</th>
                                <th>Date</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {requests.map((r) => (
                                <tr key={r.id}>
                                    <td>{r.userId.substring(r.userId.length - 6)}</td>
                                    <td>{r.userEmail}</td>
                                    <td style={{ fontWeight: "bold" }}>৳ {r.amount.toLocaleString()}</td>
                                    <td>{new Date(r.createdUtc).toLocaleString()}</td>
                                    <td>
                                        <button
                                            className="btn"
                                            style={{ marginRight: 8, padding: "4px 8px", backgroundColor: "#2E7D32" }}
                                            onClick={() => handleReview(r.id, true)}
                                        >
                                            Approve
                                        </button>
                                        <button
                                            className="btn"
                                            style={{ padding: "4px 8px", backgroundColor: "#C62828" }}
                                            onClick={() => handleReview(r.id, false)}
                                        >
                                            Decline
                                        </button>
                                    </td>
                                </tr>
                            ))}
                            {requests.length === 0 && (
                                <tr>
                                    <td colSpan={5}>No pending requests</td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
}
