import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { map, Observable } from "rxjs";

import { CurrentUserInterface } from "src/app/shared/types/currentUser.interface";
import { RegisterRequestInterface } from "../types/registerRequest.interface";
import { environment } from "src/environments/environment";
import { AuthResponseInterface } from "../types/authResponse.interface";
import { LoginRequestInterface } from "../types/loginRequest.interface";

@Injectable()
export class AuthService {
    constructor(private http: HttpClient) {
    }

    register(data: RegisterRequestInterface): Observable<CurrentUserInterface> {
        const url = environment.apiUrl + '/Auth/SignUp'
        let result: Observable<CurrentUserInterface> = this.http
        .post<AuthResponseInterface>(url, data)
        .pipe(map(this.getUser))
        return result
    }

    login(data: LoginRequestInterface): Observable<CurrentUserInterface> {
        const url = environment.apiUrl + '/Auth/SignIn'
        return this.http
        .post<AuthResponseInterface>(url, data)
        .pipe(map(this.getUser))
    }

    getUser(response: AuthResponseInterface): CurrentUserInterface {
        return response
    }
}