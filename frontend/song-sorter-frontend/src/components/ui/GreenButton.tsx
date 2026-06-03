
export default function GreenButton({ children, onClick }: { children: React.ReactNode, onClick: () => void }) {
    return (
        <button onClick={onClick} className="bg-custom-green text-white px-4 py-2 rounded hover:bg-green-700 transition-colors cursor-pointer">
            {children}
        </button>
    );

}
