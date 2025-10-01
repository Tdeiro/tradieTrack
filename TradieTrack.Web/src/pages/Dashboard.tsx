import { useQuery } from "@tanstack/react-query";
import { api } from "../lib/api";

type Forecast = { date: string; temperatureC: number; summary: string };

export default function Dashboard() {
  const { data, isLoading, error } = useQuery({
    queryKey: ["forecast"],
    queryFn: async () => {
      const r = await api.get<Forecast[]>("/weatherforecast");
      return r.data;
    },
  });

  if (isLoading) return <p>Loading…</p>;
  if (error) return <p className="text-red-600">Failed to load</p>;

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-4">Dashboard</h1>
      <ul className="space-y-2">
        {data?.map((f, i) => (
          <li key={i} className="rounded border p-3">
            <div className="font-medium">{f.summary}</div>
            <div className="text-sm text-slate-500">{f.date}</div>
            <div className="text-sm">{f.temperatureC}°C</div>
          </li>
        ))}
      </ul>
    </div>
  );
}
