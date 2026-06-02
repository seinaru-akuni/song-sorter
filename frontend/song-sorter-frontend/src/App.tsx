import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';

import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';


function App() {
  
  

  return (
    <div>
      <BrowserRouter>
            {/* Навігаційне меню, яке буде видно на всіх сторінках */}
            <nav style={{ padding: '10px', borderBottom: '1px solid #ccc', marginBottom: '20px' }}>
                <Link to="/" style={{ marginRight: '15px' }}>Головна</Link>
                <Link to="/login" style={{ marginRight: '15px' }}>Вхід</Link>
                <Link to="/register">Реєстрація</Link>
            </nav>

            {/* Routes - це місце, де будуть підмінятися сторінки */}
            <Routes>
                {/* Якщо URL точно співпадає з "/", показуємо HomePage */}
                <Route path="/" element={<HomePage />} />
                
                {/* Якщо URL "/login", показуємо LoginPage */}
                <Route path="/login" element={<LoginPage />} />
                
                {/* Якщо URL "/register", показуємо RegisterPage */}
                <Route path="/register" element={<RegisterPage />} />
            </Routes>
        </BrowserRouter>
        
    </div>
  )
}

export default App