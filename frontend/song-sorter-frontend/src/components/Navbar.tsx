import { Link } from 'react-router-dom';
import ThemeToggle from './ui/ThemeToggle';
import GreenButton from './ui/GreenButton';
import TransparentButton from './ui/TransparentButton';
import { LogoutButton } from './ui/LogoutButton';

export default function Navbar() {
return (
    <nav className="bg-white border-b border-gray-300 text-gray-900 transition-colors duration-200 dark:border-gray-700 dark:bg-gray-800 dark:text-white">
    {/* Контейнер із відступами та вертикальним вирівнюванням */}
    <div className="mx-auto flex max-w-7xl items-center justify-between px-[clamp(10px,5vw,20px)] py-3">
        
        {/* Ліва частина: Логотип та Навігація */}
        <div className="flex items-center gap-6">
        {/* Логотип тепер є посиланням на головну — це стандарт для UX */}
        <Link to="/" className="text-xl font-bold text-custom-green transition-opacity hover:opacity-90">
            Song Sorter
        </Link>
        
        </div>

        {/* Права частина: Перемикач теми */}
        <div className="flex items-center gap-4 text-sm font-medium">
            
            <Link to="/login" className="p-2 rounded-lg hover:bg-black/10 dark:hover:bg-white/10 transition-colors text-black dark:text-white">
            Login
            </Link>
            
            
            <Link to="/register" className="bg-custom-green text-white px-4 py-2 rounded hover:bg-green-700 transition-colors cursor-pointer">
                Register
            </Link>
            
            <ThemeToggle />

            <LogoutButton />
        </div>
    </div>
    </nav>
);
}