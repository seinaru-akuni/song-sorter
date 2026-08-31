

interface PlaylistsCardProps {
    title: string;
    publishedAt: string;
    thumbnails: {
        medium: {
            url: string;
        }
    }
}

export function PlaylistCard({ title, publishedAt, thumbnails }: PlaylistsCardProps) {
    const url = thumbnails.medium.url; // Отримуємо посилання
    
    return (
        <div>
            <img src={url} alt={title} />
            <p>{title}</p>
            <p>{publishedAt}</p>
        </div>
    );
}