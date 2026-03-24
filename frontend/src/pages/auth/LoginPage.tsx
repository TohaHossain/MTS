import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { login } from "../../api/auth.api";
import { setAuth } from "../../app/auth.store";

export default function LoginPage() {
  const nav = useNavigate();

  const [email, setEmail] = useState("admin@tradingsim.com");
  const [password, setPassword] = useState("Admin@12345");
  const [err, setErr] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErr(null);

    if (!email.trim() || !password) {
      setErr("Email and password are required");
      return;
    }

    try {
      setLoading(true);

      const res = await login({
        email: email.trim(),
        password,
      });

      // store auth info
      setAuth(res.token, res.role, res.email);
      localStorage.setItem("token", res.token);

      // redirect based on role
      nav(res.role === "Admin" ? "/admin" : "/");
    } catch (ex: any) {
      setErr(ex?.response?.data?.error ?? "Login failed");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{ maxWidth: 420, margin: "40px auto" }}>
      <h2>Login</h2>

      <form onSubmit={onSubmit}>
        <div>
          <label>Email</label>
          <input
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            style={{ width: "100%" }}
          />
        </div>

        <div style={{ marginTop: 10 }}>
          <label>Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={{ width: "100%" }}
          />
        </div>

        {err && (
          <div style={{ marginTop: 10, color: "crimson" }}>
            {err}
          </div>
        )}

        <button
          style={{ marginTop: 14 }}
          type="submit"
          disabled={loading}
        >
          {loading ? "Logging in..." : "Login"}
        </button>
      </form>

      <div style={{ marginTop: 12 }}>
        No account? <Link to="/signup">Signup</Link>
      </div>
    </div>
  );
}