import React from "react";

interface LinkedAccountCardProps {
    providerName: string;
    email: string;
    }

    export const LinkedAccountCard: React.FC<LinkedAccountCardProps> = ({
    providerName,
    email
    }) => {


    return (
        
        <div className="linked-account-card" style={{ border: "1px solid #ddd", borderRadius: 8, padding: 16, maxWidth: 320, background: "#fff" }}>
            <div style={{ marginBottom: 8, fontSize: 14, color: "#888" }}>Linked Account</div>
            <div style={{ fontSize: 18, fontWeight: 600, marginBottom: 4 }}>{email}</div>
            <div style={{ fontSize: 14, color: "#555" }}>{providerName}</div>
        </div>
        
        
    );
};
