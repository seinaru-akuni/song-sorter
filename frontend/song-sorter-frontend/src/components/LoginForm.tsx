import React, { useState } from 'react';
import { authService } from '../services/authService';
import { Link } from 'react-router-dom';

function LoginForm() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);
    
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage('');
        setError('');

        try {
            const result = await authService.login({ email, password, rememberMe });
            setMessage(result.message || 'Вхід успішний! Перевірте кукі.');
        } catch (err: any) {
            setError(err.message);
        }
    };

    // Кнопка для тестування ендпоінту /api/auth/me
    const handleCheckMe = async () => {
        try {
            const user = await authService.getMe();
            alert(`Сервер бачить вас! ID: ${user.id}, Email: ${user.email}`);
        } catch (err) {
            alert('Сервер вас не впізнав. Куки немає або вона недійсна.');
        }
    };

    return (
        <div className="text-black dark:text-white bg-white dark:bg-gray-800 p-6 rounded-4xl max-w-xl m-4 flex-col w-1/3 min-w-[300px]">
            <h3 className="text-lg text-center font-semibold mb-4">Login</h3>
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                <input className="my-1 p-2 border border-gray-300 rounded-2xl" type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required />
                <input className="my-1 p-2 border border-gray-300 rounded-2xl" type="password" placeholder="Password" value={password} onChange={e => setPassword(e.target.value)} required />
                <div className="flex justify-between ...">
                    <label className="flex items-center text-sm">
                        <input type="checkbox" checked={rememberMe} onChange={e => setRememberMe(e.target.checked)} className="mr-1" />
                        Remember me
                    </label>

                    <Link to="/forgot-password" className="text-blue-500 hover:underline">
                        Forgot password?
                    </Link>
                </div>
                

                <button type="submit" className="my-5 bg-custom-green text-white py-2 px-4 rounded-2xl hover:bg-green-700">
                    Sign In
                </button>

                
            </form>

            <Link to="/register" className="text-blue-500 hover:underline">
                    Don't have an account? Register here.
            </Link>

            {error && <p style={{ color: 'red' }}>{error}</p>}
            {message && <p style={{ color: 'green' }}>{message}</p>}

            <button onClick={handleCheckMe} style={{ marginTop: '10px', width: '100%' }}>
                Перевірити сесію (/me)
            </button>
        </div>
    );
};

export default LoginForm;