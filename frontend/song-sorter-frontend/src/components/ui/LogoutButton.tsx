import React from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services/authService'; // Перевір шлях

export const LogoutButton = () => {
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            // 1. Звертаємося до сервера. Він відправить у відповідь заголовок, 
            // який накаже браузеру фізично видалити куку jwt_token.
            await authService.logout();

            // 2. Перенаправляємо користувача на сторінку логіну
            // (Або на головну '/', як тобі більше подобається)
            navigate('/login');
            
        } catch (error) {
            console.error('Помилка при виході:', error);
            alert('Не вдалося вийти з акаунту');
        }
    };

    return (
        <button 
            onClick={handleLogout} 
            // Тут можеш додати свої класи Tailwind, якщо вже налаштував його:
            // className="px-4 py-2 bg-red-500 text-white rounded hover:bg-red-600"
            style={{ padding: '8px 16px', cursor: 'pointer', background: '#ff4d4d', color: 'white', border: 'none', borderRadius: '4px' }}
        >
            Вийти з акаунту
        </button>
    );
};