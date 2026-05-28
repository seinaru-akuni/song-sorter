import { useState, useEffect } from 'react';
import { useGoogleLogin } from '@react-oauth/google';

function GoogleLoginButton() {

    const [statusMessage, setStatusMessage] = useState<string>('');
    const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);

    useEffect(() => {
        const savedToken = localStorage.getItem('youtube_access_token');
        if (savedToken) {
            setIsLoggedIn(true);
            setStatusMessage('Ви успішно авторизовані (дані відновлено).');
        }
    }, [])


    const login = useGoogleLogin({
        scope: 'https://www.googleapis.com/auth/youtube',
        onSuccess: tokenResponse => {
            localStorage.setItem('youtube_access_token', tokenResponse.access_token);

            setStatusMessage('Google авторизація успішна! Відправляємо токен на бекенд...');

            setIsLoggedIn(true);

            fetch('https://localhost:7197/api/googleauth/google-login', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({
                    accessToken: tokenResponse.access_token
                })

            })

            .then (response => {
                if (!response.ok) throw new Error('Помилка запиту до бекенду');
                return response.json();
            })
            
            .then (data => {
                setStatusMessage(`Відповідь сервера: ${data.message}`);
            })

            .catch (error => {
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
                <p style={{ margin: 0, fontWeight: 'bold', color: '#555' }}>
                    {statusMessage}
                </p>
            )}
        </div>
    );

}

export default GoogleLoginButton;