import { useEffect, useState } from 'react';

interface PlaylistsListProps {
    email: string;
}

function PlaylistsList({ email }: PlaylistsListProps) {
    const [playlists, setPlaylists] = useState<any[]>([]);
    const [statusMessage, setStatusMessage] = useState<string>('');

    useEffect(() => {
        // Запускаємо fetchPlaylists щоразу, коли змінюється переданий email
        fetchPlaylists();
    }, [email]); 

    const fetchPlaylists = () => {
        if (!email) {
            setStatusMessage('Email не надано. Неможливо отримати плейлисти.');
            return;
        }

        // ФІКС: додано нижнє підкреслення "_" перед емейлом, щоб співпадало з ключем при збереженні
        const savedToken = localStorage.getItem(`youtube_access_token_${email}`);

        fetch(`/api/playlists/my-playlists?email=${encodeURIComponent(email)}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include' // Залишаємо це, щоб передавалася кука сесії твого додатку
        })
        .then(response => {
            if(!response.ok) throw new Error('Помилка при отриманні плейлистів');
            return response.json();
        })
        .then(data => {
            console.log('Отримані плейлисти:', data);
            if(data.items){
                setPlaylists(data.items);
                setStatusMessage('Плейлисти успішно завантажені!');
            }
        })
        .catch(error => {
            console.error(error);
            setStatusMessage('Не вдалося завантажити плейлисти');
        });
    }

    return (
        <div className='playlists-container'>
            <p>{statusMessage}</p>
            {playlists.length > 0 && (
                <ul style={{ textAlign: 'left', marginTop: '20px' }}>
                    {playlists.map((pl: any) => (
                        <li key={pl.id}>{pl.snippet.title}</li>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default PlaylistsList;