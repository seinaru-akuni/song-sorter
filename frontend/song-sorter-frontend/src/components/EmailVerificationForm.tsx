import React, { useState } from 'react';
import { authService } from '../services/authService';
import { useNavigate } from 'react-router-dom';

interface EmailVerificationFormProps {
    email: string;
    navigateTo?: string;
    newPassword?: string;
    confirmNewPassword?: string;
}

export default function EmailVerificationForm({ email, navigateTo = '/', newPassword, confirmNewPassword }: EmailVerificationFormProps) {

    const navigate = useNavigate();
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const [verificationCode, setVerificationCode] = useState('');


    const handleVerificationSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setMessage('');
        try {
            if (newPassword && confirmNewPassword) {
                await authService.verifyEmail(email, verificationCode, newPassword, confirmNewPassword);
            } else {
                await authService.verifyEmail(email, verificationCode);
            }
            setMessage('Успіх! Перенаправляємо...');
            // Після успіху і отримання куки перекидаємо на головну
            setTimeout(() => navigate(navigateTo), 1500); 
        } catch (err: any) {
            setError(err.message);
        }
    };

    return(
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
    )
}