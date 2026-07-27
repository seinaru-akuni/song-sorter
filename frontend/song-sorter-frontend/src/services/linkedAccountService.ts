const API_BASE_URL = '/api/linkedaccounts';

export const linkedAccountsService = {
    
    getLinkedAccounts: async () => {
        const response = await fetch(`${API_BASE_URL}/get-list`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include', // КРИТИЧНО ВАЖЛИВО ДЛЯ КУК!
        });
            

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Помилка авторизації');
        }
        return response.json();
    }
}