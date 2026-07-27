import { useState } from "react";
import GoogleLoginButton from "../components/ui/GoogleLoginButton";
import { LinkedAccounts } from "../components/LinkedAccounts";
import PlaylistsList from "../components/PlaylistsList";

function HomePage() {
    // Створюємо спільний стан тут
    const [selectedEmail, setSelectedEmail] = useState<string | null>(null);

    return (
        <div>
            <GoogleLoginButton />
            
            {/* Передаємо функцію для зміни стану */}
            <LinkedAccounts onAccountSelect={setSelectedEmail} />
            
            {/* Показуємо плейлисти ТІЛЬКИ якщо емейл вибрано, і передаємо його як пропс */}
            {selectedEmail && <PlaylistsList email={selectedEmail} />}
        </div>
    );
}

export default HomePage;