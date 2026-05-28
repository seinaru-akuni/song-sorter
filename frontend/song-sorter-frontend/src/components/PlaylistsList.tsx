import {useEffect, useState} from 'react';

function PlaylistsList() {

    const [playlists, setPlaylists] = useState<any[]>([]);
    const [statusMessage, setStatusMessage] = useState<string>('');

    useEffect(() => {
        fetchPlaylists();
    }, []);

    const fetchPlaylists = () => {
        const sevedToken = localStorage.getItem('youtube_access_token');

        if (!sevedToken) {
            alert('Будь ласка, спочатку авторизуйтеся через Google!');
            return;
        }

        fetch('https://localhost:7197/api/playlists/my-playlists', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${sevedToken}`
            }
        })
        .then (response => {
            if(!response.ok) throw new Error('Помилка при отриманні плейлистів');
            return response.json();
        })
        .then (data => {
            console.log('Отримані плейлисти:', data);
            
            if(data.items){
                setPlaylists(data.items);
                setStatusMessage('Плейлисти успішно завантажені!');
            }
        })
        .catch (error => {
            console.error(error);
            setStatusMessage('Не вдалося завантажити плейлисти');
        });

    }

    return (
        <div className='playlists-container'>
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