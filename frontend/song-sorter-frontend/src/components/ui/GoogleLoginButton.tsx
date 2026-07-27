import { useState, useEffect } from 'react';
import { useGoogleLogin } from '@react-oauth/google';
import { useAuth } from '../../contexts/AuthContext';

function GoogleLoginButton() {

    const [statusMessage, setStatusMessage] = useState<string>('');
    const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);
    const { user } = useAuth();
    

//     useEffect(() => {
//     if (user) {
//         // Якщо користувач залогінений в апці, перевіряємо токен YouTube
//         const savedToken = localStorage.getItem('youtube_access_token');
        
//         if (savedToken) {
//             setIsLoggedIn(true);
//             setStatusMessage('Ви успішно авторизовані (дані відновлено).');
//         } else {
//             // Якщо юзер є, але токена немає
//             setIsLoggedIn(false);
//         }
//     } else {
//         // Якщо користувача немає (вийшов з акаунта)
//         setIsLoggedIn(false);
//     }
// }, [user]);


    const login = useGoogleLogin({
        scope: 'https://www.googleapis.com/auth/youtube',
        // 1. КРИТИЧНО ВАЖЛИВО: Перемикаємо на Authorization Code Flow
        flow: 'auth-code', 
        
        // 2. Тепер Google повертає об'єкт з КОДОМ (codeResponse), а не з токеном
        onSuccess: codeResponse => { 
            setStatusMessage('Отримано код від Google! Відправляємо на бекенд...');

            // Ми більше не зберігаємо токен одразу, бо ми його ще не маємо!
            
            fetch('/api/googleauth/callback', { // <-- Використовуємо відносний шлях через Vite proxy
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include', // <-- ДОДАНО: обов'язково передаємо куку сесії
                body: JSON.stringify({
                    authCode: codeResponse.code 
                })  
            })
            .then(response => {
                if (!response.ok) throw new Error('Помилка запиту до бекенду');
                return response.json();
            })
            .then(data => {
                console.log("Відповідь бекенду:", data.google_email, data.access_token);
                // 3. Тепер БЕКЕНД повернув нам access_token. 
                // Ось тут ми його і зберігаємо в пам'ять браузера:
                const googleEmail = data.google_email;

                localStorage.setItem(`youtube_access_token_${googleEmail}`, data.access_token);
                
                setIsLoggedIn(true);
                setStatusMessage('Авторизація повністю успішна!');
            })
            .catch(error => {
                console.error(error);
                setStatusMessage('Не вдалося з\'єднатися з бекендом.');
            });
        },

        onError: () => {
            setStatusMessage('Помилка вікна авторизації Google.');
        },
    });

    return (
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '15px' }}>
            {!isLoggedIn ? (
                <button 
                    onClick={() => login()} 
                    style={{
                        padding: '10px 20px', 
                        fontSize: '16px', 
                        cursor: 'pointer',
                        backgroundColor: '#4285F4',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        fontWeight: 'bold'
                    }}
                >
                    Увійти через Google
                </button>
            ) : (
                <div>
                    <button 
                    onClick={() => login()} 
                    style={{
                        padding: '10px 20px', 
                        fontSize: '16px', 
                        cursor: 'pointer',
                        backgroundColor: '#4285F4',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        fontWeight: 'bold'
                    }}
                >
                    Увійти через Google
                </button>
                <p style={{ margin: 0, fontWeight: 'bold', color: '#555' }}>
                    {statusMessage}
                </p>
                </div>
                
            )}
        </div>
    );

}

export default GoogleLoginButton;