import React, { useState } from 'react';
import { authService } from '../services/authService'; // Перевір шлях до файлу

    function RegisterForm() {
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage('');
        setError('');

        try {
            const result = await authService.register({
                email,
                username,
                password,
                confirmPassword
            });
            setMessage(result.message || 'Реєстрація успішна! Перевірте кукі.');
        } catch (err: any) {
            setError(err.message);
        }
    };

    return (
        <div style={{ padding: '20px', border: '1px solid black', maxWidth: '300px', marginBottom: '20px' }}>
            <h3>Реєстрація</h3>
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                <input type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required />
                <input type="text" placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} required />
                <input type="password" placeholder="Пароль" value={password} onChange={e => setPassword(e.target.value)} required />
                <input type="password" placeholder="Підтвердіть пароль" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} required />
                
                <button type="submit">Зареєструватися</button>
            </form>
            {error && <p style={{ color: 'red' }}>{error}</p>}
            {message && <p style={{ color: 'green' }}>{message}</p>}
        </div>
    )};

    
export default RegisterForm;