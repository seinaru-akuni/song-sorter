import React, { useState } from 'react';
import { authService } from '../services/authService'; // Перевір шлях до файлу
import { Link, useNavigate } from 'react-router-dom';

    function RegisterForm() {

    const navigate = useNavigate();

    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    
    const [isAwaitingCode, setIsAwaitingCode] = useState(false);
    const [verificationCode, setVerificationCode] = useState('');

    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    

    const handleRegisterSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage('');
        setError('');

        try {
            await authService.register({ email, username, password, confirmPassword });
            setIsAwaitingCode(true); // Перемикаємо UI на ввід коду
            setMessage('Код відправлено на вашу пошту!');
        } catch (err: any) {
            setError(err.message);
        }
    };

    const handleVerificationSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        
        try {
            await authService.verifyEmail(email, verificationCode);
            setMessage('Успіх! Перенаправляємо...');
            // Після успіху і отримання куки перекидаємо на головну
            setTimeout(() => navigate('/'), 1500); 
        } catch (err: any) {
            setError(err.message);
        }
    };

    return (
        <div className="text-black dark:text-white bg-white dark:bg-gray-800 p-6 rounded-4xl max-w-xl m-4 flex-col w-1/3 min-w-[300px]">
            <h3 className="text-lg text-center font-semibold mb-4">Registration</h3>
            {!isAwaitingCode ? (
                // КРОК 1: Звичайна форма реєстрації
                <form onSubmit={handleRegisterSubmit} className="flex flex-col gap-3">
                    <input className="p-2 border border-gray-300 rounded-2xl" type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required />
                    <input className="p-2 border border-gray-300 rounded-2xl" type="text" placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} required />
                    <input className="p-2 border border-gray-300 rounded-2xl" type="password" placeholder="Password" value={password} onChange={e => setPassword(e.target.value)} required />
                    <input className="p-2 border border-gray-300 rounded-2xl" type="password" placeholder="Confirm Password" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} required />
                    <button type="submit" className="mt-2 bg-custom-green text-white py-2 px-4 rounded-2xl hover:bg-green-700">Зареєструватися</button>
                </form>
            ) : (
                // КРОК 2: Форма вводу коду
                <form onSubmit={handleVerificationSubmit} className="flex flex-col gap-3">
                    <p className="text-sm text-center mb-2">Введіть 6-значний код, який ми відправили на <b>{email}</b></p>
                    <input 
                        className="p-2 border border-gray-300 rounded-2xl text-center text-xl tracking-widest font-mono" 
                        type="text" 
                        maxLength={6}
                        placeholder="000000" 
                        value={verificationCode} 
                        onChange={e => setVerificationCode(e.target.value)} 
                        required 
                    />
                    <button type="submit" className="mt-2 bg-blue-500 text-white py-2 px-4 rounded-2xl hover:bg-blue-600">Підтвердити код</button>
                </form>
            )}
            <Link to="/login" className="text-blue-500 hover:underline">
                    Already have an account? Login here.
            </Link>
            {error && <p style={{ color: 'red' }}>{error}</p>}
            {message && <p style={{ color: 'green' }}>{message}</p>}
        </div>
    )};

    
export default RegisterForm;