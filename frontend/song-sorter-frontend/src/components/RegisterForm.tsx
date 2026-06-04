import React, { useState } from 'react';
import { authService } from '../services/authService'; // Перевір шлях до файлу
import { Link } from 'react-router-dom';

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
        <div className="text-black dark:text-white bg-white dark:bg-gray-800 p-6 rounded-4xl max-w-xl m-4 flex-col w-1/3 min-w-[300px]">
            <h3 className="text-lg text-center font-semibold mb-4">Registration</h3>
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                <input className="my-1 p-2 border border-gray-300 rounded-2xl" type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required />
                <input className="my-1 p-2 border border-gray-300 rounded-2xl" type="text" placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} required />
                <input className="my-1 p-2 border border-gray-300 rounded-2xl" type="password" placeholder="Password" value={password} onChange={e => setPassword(e.target.value)} required />
                <input className="my-1 p-2 border border-gray-300 rounded-2xl" type="password" placeholder="Confirm Password" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} required />
                
                <button type="submit" className="my-5 bg-custom-green text-white py-2 px-4 rounded-2xl hover:bg-green-700">
                    Register
                </button>
            </form>
            <Link to="/login" className="text-blue-500 hover:underline">
                    Already have an account? Login here.
            </Link>
            {error && <p style={{ color: 'red' }}>{error}</p>}
            {message && <p style={{ color: 'green' }}>{message}</p>}
        </div>
    )};

    
export default RegisterForm;