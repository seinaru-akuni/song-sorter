import React, { useState } from 'react';
import { authService } from '../services/authService';

function LoginForm() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage('');
        setError('');

        try {
            const result = await authService.login({ email, password });
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
        <div style={{ padding: '20px', border: '1px solid black', maxWidth: '300px' }}>
            <h3>Вхід</h3>
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                <input type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required />
                <input type="password" placeholder="Пароль" value={password} onChange={e => setPassword(e.target.value)} required />
                
                <button type="submit">Увійти</button>
            </form>
            {error && <p style={{ color: 'red' }}>{error}</p>}
            {message && <p style={{ color: 'green' }}>{message}</p>}

            <hr />
            <button onClick={handleCheckMe} style={{ marginTop: '10px', width: '100%' }}>
                Перевірити сесію (/me)
            </button>
        </div>
    );
};

export default LoginForm;