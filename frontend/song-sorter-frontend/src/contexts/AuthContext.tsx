import React, { createContext, useContext, useState, useEffect} from 'react';
import type { ReactNode } from 'react';
import { authService } from '../services/authService';
import type { IUser } from '../types/user';

// Описуємо, які дані будуть доступні всьому додатку
interface AuthContextType {
    user: IUser | null;       // Дані користувача (або null, якщо не залогінений)
    isLoading: boolean;     // Чи йде зараз перевірка
    checkAuth: () => void;  // Метод, щоб вручну оновити статус (наприклад, після логіну)
}

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [user, setUser] = useState<IUser | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const checkAuth = async () => {
        setIsLoading(true);
        try {
            const userData = await authService.getCurrentUser();
            setUser(userData); // Якщо успішно, записуємо юзера
        } catch (error) {
            setUser(null); // Якщо помилка, скидаємо юзера
        } finally {
            setIsLoading(false); // Перевірка завершена
        }
    };

    // Цей useEffect спрацює один раз при запуску (оновленні сторінки)
    useEffect(() => {
        checkAuth();
    }, []);

    return (
        <AuthContext.Provider value={{ user, isLoading, checkAuth }}>
            {children}
        </AuthContext.Provider>
    );
};

// Зручний хук для використання контексту в компонентах
export const useAuth = () => useContext(AuthContext);