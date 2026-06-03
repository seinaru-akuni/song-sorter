import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import {Moon, Sun} from "lucide-react";
import {useState, useEffect} from "react";

import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import Navbar from './components/NavBar';



function App() {

  return (
    <div>
      <BrowserRouter>
        
        <Navbar />

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