import { useEffect, useState } from 'react';
import { PlaylistCard } from './ui/PlaylistCard'

interface PlaylistSnippet {
    title: string;
    publishedAt: string; // Дата створення
    thumbnails: {
        medium: {
            url: string; // Посилання на обкладинку
        }
    }
}

interface PlaylistContentDetails {
    itemCount: number; // Кількість пісень
}

interface YouTubePlaylist {
    id: string;
    snippet: PlaylistSnippet;
    contentDetails: PlaylistContentDetails;
}

interface PlaylistsListProps {
    email: string;
    onClose: () => void; // Додаємо типізацію для нової функції
}

function PlaylistsList({ email, onClose }: PlaylistsListProps) {
    const [playlists, setPlaylists] = useState<YouTubePlaylist[]>([]);
    const [statusMessage, setStatusMessage] = useState<string>('');

    // Видалили стан isOpen, бо тепер компонент монтується/демонтується батьком

    useEffect(() => {
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
            credentials: 'include' 
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

    // Більше не робимо перевірку if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
            <div className="relative w-full max-w-lg bg-white rounded-xl shadow-2xl overflow-hidden flex flex-col">
                
                {/* Кнопка-хрестик тепер викликає функцію onClose від батька */}
                <button 
                    onClick={onClose}
                    className="absolute top-4 right-4 p-1 text-gray-400 hover:text-gray-800 hover:bg-gray-100 rounded-full transition-colors focus:outline-none"
                    aria-label="Закрити"
                >
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </button>

                <div className="p-6 pb-4 border-b border-gray-100">
                    <h2 className="text-xl font-bold text-gray-800 mb-2">Ваші плейлисти</h2>
                    <p className="bg-custom-green text-sm px-3 py-1 rounded-md inline-block">
                        {statusMessage}
                    </p>
                </div>

                <div className="p-6 overflow-y-auto max-h-[60vh]">
                    {playlists.length > 0 ? (
                        <ul className="space-y-2">
                            {playlists.map((pl: YouTubePlaylist) => (
                                <li 
                                    key={pl.id} 
                                    className="px-4 py-3 bg-gray-50 rounded-lg border border-gray-200 hover:border-blue-400 hover:bg-blue-50 cursor-pointer transition-all text-gray-700 font-medium"
                                >
                                    <PlaylistCard title={pl.snippet.title} publishedAt={pl.snippet.publishedAt} thumbnails={pl.snippet.thumbnails}></PlaylistCard>
                                    
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <div className="text-center text-gray-500 py-8">
                            Плейлисти відсутні
                        </div>
                    )}
                </div>
                
            </div>
        </div>
    );
}

export default PlaylistsList;