export interface IUser {
    id: number;
    email: string;
    username: string;
    connectedServices: string[] | null;
}