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
        
        <div className="border border-gray-300 rounded-lg p-4 bg-white w-[90%] md:w-[70%] lg:w-1/2 mx-auto min-w-fit transition-all duration-300 ease-in-out wrap-anywhere" style={{ border: "1px solid #ddd", borderRadius: 8, padding: 16, background: "#fff" }}>
            <div style={{ marginBottom: 8, fontSize: 14, color: "#888" }}>Linked Account</div>
            <div style={{ fontSize: 18, fontWeight: 600, marginBottom: 4 }}>{email}</div>
            <div style={{ fontSize: 14, color: "#555" }}>{providerName}</div>
        </div>
        
        
    );
};
