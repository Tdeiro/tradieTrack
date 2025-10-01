import { Outlet, NavLink } from "react-router-dom";

export default function App() {
  return (
    <div className="min-h-screen">
      <nav className="border-b bg-white/70 backdrop-blur sticky top-0 z-10">
        <div className="mx-auto max-w-5xl px-4 py-3 flex gap-4">
          <NavLink
            to="/"
            className={({ isActive }) => (isActive ? "font-semibold" : "")}
          >
            Dashboard
          </NavLink>
          <NavLink
            to="/customers"
            className={({ isActive }) => (isActive ? "font-semibold" : "")}
          >
            Customers
          </NavLink>
        </div>
      </nav>
      <main className="mx-auto max-w-5xl px-4 py-6">
        <Outlet />
      </main>
    </div>
  );
}
