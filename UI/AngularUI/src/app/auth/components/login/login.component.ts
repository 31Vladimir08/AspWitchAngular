import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { select, Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { BackendErrorsInterface } from "src/app/shared/types/backendErrors.interface";
import { loginAction } from "../../store/actions/login.action";
import { isSubmittingSelector, validationErrorsSelector } from "../../store/selector";
import { LoginFormInterface } from "../../types/loginForm.interface";
import { LoginRequestInterface } from "../../types/loginRequest.interface";

@Component({
    selector: 'mc-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
    form: FormGroup
    isSubmitting$: Observable<boolean>
    backendErrors$: Observable<BackendErrorsInterface | null>

    constructor(private fb: FormBuilder, private store: Store) {
    }

    ngOnInit(): void {
        this.initializeForm()
        this.initialazeValues()
    }

    initialazeValues(): void {
        this.isSubmitting$ = this.store.pipe(select(isSubmittingSelector));
        this.backendErrors$ = this.store.pipe(select(validationErrorsSelector))
    }

    initializeForm(): void {
        this.form = this.fb.group({
            email: '',
            password: ''
        })
    }

    onSubmit(): void {
        const loginForm: LoginFormInterface = this.form.value
        
        const loginRequest: LoginRequestInterface = {
            username: loginForm.email,
            password: loginForm.password
        }
        this.store.dispatch(loginAction({request: loginRequest}))
    }
}