import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { select, Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { BackendErrorsInterface } from "src/app/shared/types/backendErrors.interface";
import { registerAction } from "../../store/actions/register.action";
import { isSubmittingSelector, validationErrorsSelector } from "../../store/selector";
import { RegisterFormInterface } from "../../types/registerForm.interface";
import { RegisterRequestInterface } from "../../types/registerRequest.interface";

@Component({
    selector: 'mc-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
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
            userName: ['', Validators.required],
            displayName : '',
            password: '',
            repeatPassword: '',
            isAgryAllStatements: false
        })
    }

    onSubmit(): void {
        const registerForm: RegisterFormInterface = this.form.value
        /*this.form.setErrors({ ...this.form.errors, 'yourErrorName': true });
        return;*/
        
        const registerRequest: RegisterRequestInterface = {
            displayName: registerForm.displayName,
            email: registerForm.email,
            userName: registerForm.userName,
            password: registerForm.password
        }
        this.store.dispatch(registerAction({request: registerRequest}))
    }
}