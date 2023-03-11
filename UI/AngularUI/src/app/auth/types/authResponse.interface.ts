import { CurrentUserInterface } from "src/app/shared/types/currentUser.interface";

export interface AuthResponseInterface {
    userId: string,
    displayName:string,
    userName: string,
    roleCode: string,
    token: string
}