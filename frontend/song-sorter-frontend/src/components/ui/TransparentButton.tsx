
export default function TransparentButton({ children, onClick }: { children: React.ReactNode, onClick?: () => void }) {
    return (
        <button onClick={onClick} className="p-2 rounded-lg hover:bg-black/10 dark:hover:bg-white/10 transition-colors text-black dark:text-white">
            {children}
        </button>
    );
}