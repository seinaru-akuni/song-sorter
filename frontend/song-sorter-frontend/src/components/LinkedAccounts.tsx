import React, { useEffect, useState } from "react";
import { LinkedAccountCard } from "./ui/LinkedAccountCard";
import { linkedAccountsService } from "../services/linkedAccountService";
import PlaylistsList from "./PlaylistsList";

interface LinkedAccountDto {
    id: number;
    providerName: string;
    email: string;
}

// Додаємо інтерфейс для пропсів


export const LinkedAccounts: React.FC = () => {
    const [linkedAccounts, setLinkedAccounts] = useState<LinkedAccountDto[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true); 
    const [selectedEmail, setSelectedEmail] = useState<string | null>(null);

    // Видаляємо локальний clickedEmail, він тут більше не потрібен

    useEffect(() => {
        const fetchAccounts = async () => {
            try {
                const data = await linkedAccountsService.getLinkedAccounts();
                setLinkedAccounts(data);
            } catch (error) {
                console.error("Не вдалося завантажити акаунти:", error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchAccounts();
    }, []); 

    if (isLoading) {
        return <div>Loading...</div>; 
    }

    const handleClick = (email: string) => {
        // Перевіряємо токен і передаємо емейл наверх у HomePage
        if(localStorage.getItem(`youtube_access_token_${email}`) !== null) {
            setSelectedEmail(email);
        }
    }

    return (
        <div>
            {linkedAccounts.length === 0 ? (
                <p>У вас ще немає прив'язаних акаунтів.</p>
            ) : (
                linkedAccounts.map((account) => (
                    // Краще передавати email напряму у функцію, ніж читати з data-атрибута
                    <div key={account.id} onClick={() => handleClick(account.email)}>
                        <LinkedAccountCard
                            providerName={account.providerName}
                            email={account.email}
                            isLoggedIn={localStorage.getItem(`youtube_access_token_${account.email}`) !== null}
                        />
                    </div>
                ))
            )}

            {selectedEmail && <PlaylistsList email={selectedEmail} />}
        </div>
        
    );
};