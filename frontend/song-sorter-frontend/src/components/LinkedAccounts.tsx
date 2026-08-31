import React, { useEffect, useState } from "react";
import { LinkedAccountCard } from "./ui/LinkedAccountCard";
import { linkedAccountsService } from "../services/linkedAccountService";
import PlaylistsList from "./PlaylistsList";

interface LinkedAccountDto {
    id: number;
    providerName: string;
    email: string;
}

export const LinkedAccounts: React.FC = () => {
    const [linkedAccounts, setLinkedAccounts] = useState<LinkedAccountDto[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true); 
    const [selectedEmail, setSelectedEmail] = useState<string | null>(null);

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
        setSelectedEmail(email);
    }

    // Додаємо функцію для закриття модалки
    const closeModal = () => {
        setSelectedEmail(null);
    }

    return (
        <div className="justify-center flex flex-col">
            {linkedAccounts.length === 0 ? (
                <p>У вас ще немає прив'язаних акаунтів.</p>
            ) : (
                linkedAccounts.map((account) => (
                    <div key={account.id} onClick={() => handleClick(account.email)}>
                        <LinkedAccountCard
                            providerName={account.providerName}
                            email={account.email}
                        />
                    </div>
                ))
            )}

            {/* Передаємо closeModal через пропс onClose */}
            {selectedEmail && (
                <PlaylistsList 
                    email={selectedEmail} 
                    onClose={closeModal} 
                />
            )}
        </div>
    );
};