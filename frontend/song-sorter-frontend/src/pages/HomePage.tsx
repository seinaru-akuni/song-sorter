import { useState } from "react";
import GoogleLoginButton from "../components/ui/GoogleLoginButton";
import { LinkedAccounts } from "../components/LinkedAccounts";
import PlaylistsList from "../components/PlaylistsList";

function HomePage() {
    
    return (
        <div>
            <GoogleLoginButton />
            
            <LinkedAccounts />
        </div>
    );
}

export default HomePage;