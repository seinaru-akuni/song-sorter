import React, { useState } from 'react';
import { authService } from '../services/authService'; // Перевір шлях до файлу
import { Link, useNavigate } from 'react-router-dom';
import EmailVerificationForm from './EmailVerificationForm';

    function ForgotPasswordForm() {

    const navigate = useNavigate();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [isAwaitingCode, setIsAwaitingCode] = useState(false);

    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    

    const handleChangePasswordSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage('');
        setError('');

        if (password !== confirmPassword) {
            setError('Паролі не співпадають.');
            return;
        }

        try {
            await authService.forgotPassword(email);
            setIsAwaitingCode(true); // Перемикаємо UI на ввід коду
            setMessage('Код відправлено на вашу пошту!');
        } catch (err: any) {
            setError(err.message);
        }
    };

    

    return (
        <div className="text-black dark:text-white bg-white dark:bg-gray-800 p-6 rounded-4xl max-w-xl m-4 flex-col w-1/3 min-w-[300px]">
            <h3 className="text-lg text-center font-semibold mb-4">Change Password</h3>
            {!isAwaitingCode ? (
                // КРОК 1: Звичайна форма зміни пароля
                <form onSubmit={handleChangePasswordSubmit} className="flex flex-col gap-3">
                    <input className="p-2 border border-gray-300 rounded-2xl" type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required />
                    <input className="p-2 border border-gray-300 rounded-2xl" type="password" placeholder="Password" value={password} onChange={e => setPassword(e.target.value)} required />
                    <input className="p-2 border border-gray-300 rounded-2xl" type="password" placeholder="Confirm Password" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} required />
                    <button type="submit" className="mt-2 bg-custom-green text-white py-2 px-4 rounded-2xl hover:bg-green-700">Змінити пароль</button>
                </form>
            ) : (
                <EmailVerificationForm 
                email={email} 
                navigateTo="/" 
                newPassword={password} 
                confirmNewPassword={confirmPassword} />
            )}
            
            <Link to="/register" className="text-blue-500 hover:underline">
                Don't have an account? Register here.
            </Link>

            {error && <p style={{ color: 'red' }}>{error}</p>}
            {message && <p style={{ color: 'green' }}>{message}</p>}
        </div>
    )};

    
export default ForgotPasswordForm;