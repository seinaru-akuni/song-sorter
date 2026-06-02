import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import {Moon, Sun} from "lucide-react";
import {useState, useEffect} from "react";

import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';


function App() {
  const [theme, setTheme] = useState<'light' | 'dark'>('light');

  useEffect(() => {
    const savedTheme = localStorage.getItem('theme') as 'light' | 'dark' | null;
    if (savedTheme) {
      setTheme(savedTheme);
      document.documentElement.classList.toggle('dark', savedTheme === 'dark');
    }
  }, []);

  function toggleTheme() {
    const newTheme = theme === 'light' ? 'dark' : 'light';
    setTheme(newTheme);
    localStorage.setItem('theme', newTheme);
    document.documentElement.classList.toggle('dark', newTheme === 'dark');
  }

  return (
    <div>
      <BrowserRouter>
        
        <nav className="dark:bg-gray-800 dark:text-white" style={{ padding: '10px', borderBottom: '1px solid #ccc', marginBottom: '20px' }}>
          <Link to="/" style={{ marginRight: '15px' }}>Головна</Link>
          <Link to="/login" style={{ marginRight: '15px' }}>Вхід</Link>
          <Link to="/register">Реєстрація</Link>
          <button onClick={toggleTheme}>{theme === 'light' ? <Moon /> : <Sun />}</button>
        </nav>

        {/* Routes - це місце, де будуть підмінятися сторінки */}
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Routes>
      </BrowserRouter>
        
    </div>
  )
}

export default App