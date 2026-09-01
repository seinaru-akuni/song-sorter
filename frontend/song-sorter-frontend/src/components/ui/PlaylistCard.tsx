

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
    
    let newPublishedAt: string;

    if(publishedAt.length > 10)
    {
        newPublishedAt = publishedAt.slice(0, 10);
    }
    else{
        newPublishedAt = 'published at: something went wrong'
    }
     

    return (
        // Контейнер всієї картки (можна додати фон та закруглення за бажанням)
        <div className="overflow-hidden rounded-lg px-4 py-3 bg-gray-50 rounded-lg border border-gray-200 hover:border-blue-400 hover:bg-blue-50 cursor-pointer transition-all text-gray-700 font-medium justify-items-center
        w-50 dark:bg-white/5 dark:border-white/10 dark:text-white" >
            
            {/* 
               Контейнер для зображення, який робить його КВАДРАТНИМ
               aspect-square забезпечує пропорцію 1:1.
               overflow-hidden обрізає краї зображення.
               rounded-lg додає закруглення кутів лише до зображення.
            */}
            <div className="aspect-square overflow-hidden rounded-lg">
                <img 
                    src={url} 
                    alt={title} 
                    // image classes: розтягнути на весь контейнер і обрізати
                    className="w-50 h-50 object-cover object-center"
                />
            </div>
            
            {/* Текстовий блок знизу із невеликим відступом зверху (pt-3) */}
            <div className="pt-3 text-align-left justify-items-start">
                {/* Стиль заголовка: напівжирний, темний */}
                <p className="text-sm font-semibold line-clamp-2 dark:text-white/90">
                    {title}
                </p>
                
                {/* Стиль дати: трохи менший, сірий */}
                <p className="text-sm text-gray-500">
                    {newPublishedAt}
                </p>
            </div>
        </div>
    );
}