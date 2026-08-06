import { useEffect, useState } from 'react';

interface PlaylistSnippet {
    title: string;
    // Сюди можна буде додати description, thumbnails тощо, якщо знадобиться
}

interface YouTubePlaylist {
    id: string;
    snippet: PlaylistSnippet;
}

interface PlaylistsListProps {
    email: string;
}

function PlaylistsList({ email }: PlaylistsListProps) {
    const [playlists, setPlaylists] = useState<YouTubePlaylist[]>([]);
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
        <div className='flex flex-col justify-center w-full'>
            <p>{statusMessage}</p>
            {playlists.length > 0 && (
                <ul style={{ textAlign: 'left', marginTop: '20px' }}>
                    {playlists.map((pl: YouTubePlaylist) => (
                        <li key={pl.id}>{pl.snippet.title}</li>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default PlaylistsList;