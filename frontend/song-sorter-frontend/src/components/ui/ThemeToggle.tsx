import { useState, useEffect } from "react";
import { Moon, Sun } from "lucide-react";

export default function ThemeToggle() {
const [theme, setTheme] = useState<'light' | 'dark'>('light');

// Перевіряємо збережену тему при завантаженні компонента
useEffect(() => {
    const savedTheme = localStorage.getItem('theme') as 'light' | 'dark' | null;
    if (savedTheme) {
    setTheme(savedTheme);
    document.documentElement.classList.toggle('dark', savedTheme === 'dark');
    }
}, []);

// Функція перемикання
function toggleTheme() {    
    const newTheme = theme === 'light' ? 'dark' : 'light';
    setTheme(newTheme);
    localStorage.setItem('theme', newTheme);
    document.documentElement.classList.toggle('dark', newTheme === 'dark');
}

return (
    <button 
    onClick={toggleTheme} 
    className="p-2 rounded-lg hover:bg-black/10 dark:hover:bg-white/10 transition-colors"
    aria-label="Toggle theme"
    >
    {theme === 'light' ? <Sun size={20} /> : <Moon size={20} />}
    </button>
);
}