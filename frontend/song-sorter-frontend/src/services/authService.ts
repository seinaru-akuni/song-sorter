// 1. Описуємо типи даних (DTO), які ми відправляємо на бекенд
export interface LoginRequest {
    email: string;
    password: string;
    rememberMe: boolean;
}

export interface RegisterRequest {
    email: string;
    username: string;
    password: string;
    confirmPassword: string;
}

// Тип даних, який повертає ендпоінт /api/auth/me
export interface UserProfile {
    id: number;
    email: string;
    connectedServices: string[];
}

const API_BASE_URL = 'https://localhost:7197/api/auth'; // Заміни порт на свій, якщо він інший

// 2. Головні функції для роботи з API
export const authService = {
    // Вхід
    login: async (data: LoginRequest) => {
        const response = await fetch(`${API_BASE_URL}/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include', // КРИТИЧНО ВАЖЛИВО ДЛЯ КУК!
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Помилка авторизації');
        }
        return response.json();
    },

    // Реєстрація
    register: async (data: RegisterRequest) => {
        const response = await fetch(`${API_BASE_URL}/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Помилка реєстрації');
        }
        return response.json();
    },

    // Перевірка поточної сесії (виклик при кожному оновленні сторінки F5)
    getMe: async (): Promise<UserProfile> => {
        const response = await fetch(`${API_BASE_URL}/me`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include', // Браузер сам прикріпить jwt_token куку
        });

        if (!response.ok) {
            throw new Error('Не авторизовано');
        }
        return response.json();
    },

    // Вихід
    logout: async () => {
        const response = await fetch(`${API_BASE_URL}/logout`, {
            method: 'POST',
            credentials: 'include',
        });
        
        if (!response.ok) {
            throw new Error('Помилка при виході');
        }
        return response.json();
    }
};