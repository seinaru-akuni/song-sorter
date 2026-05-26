import { useEffect, useState } from 'react'
import './App.css'

// Описуємо інтерфейс даних, які повертає C# бекенд
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

function App() {
  const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // Зверни увагу: заміни "7197" на реальний порт твого бекенду зі Swagger
    fetch('https://localhost:7197/weatherforecast')
      .then(response => {
        if (!response.ok) {
          throw new Error('Не вдалося отримати дані від сервера');
        }
        return response.json();
      })
      .then(data => {
        setForecasts(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <div className="loading">Завантаження даних із бекенду...</div>;
  if (error) return <div className="error" style={{ color: 'red' }}>Помилка з'єднання: {error}</div>;

  return (
    <div style={{ padding: '40px', fontFamily: 'Arial, sans-serif' }}>
      <h1>Тест інтеграції: Прогноз погоди</h1>
      <p>Ці дані успішно завантажені з нашого ASP.NET Core Web API сервісу:</p>
      
      <table border={1} cellPadding={12} style={{ borderCollapse: 'collapse', marginTop: '20px', width: '100%', textAlign: 'left' }}>
        <thead>
          <tr style={{ backgroundColor: '#f2f2f2' }}>
            <th>Дата</th>
            <th>Температура (°C)</th>
            <th>Температура (°F)</th>
            <th>Опис погоди</th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map((forecast, index) => (
            <tr key={index}>
              <td>{forecast.date}</td>
              <td>{forecast.temperatureC}°C</td>
              <td>{forecast.temperatureF}°F</td>
              <td>{forecast.summary}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default App