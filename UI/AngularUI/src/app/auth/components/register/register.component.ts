import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { select, Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { registerAction } from "../../store/actions/register.action";
import { isSubmittingSelector } from "../../store/selector";

@Component({
    selector: 'mc-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
    form: FormGroup
    isSubmitting$: Observable<boolean>

    constructor(private fb: FormBuilder, private store: Store) {
    }

    ngOnInit(): void {
        this.initializeForm()
        this.initialazeValues()
    }

    initialazeValues(): void {
        this.isSubmitting$ = this.store.pipe(select(isSubmittingSelector));
        console.log(this.isSubmitting$)
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
        console.log(this.form.value)
        this.store.dispatch(registerAction(this.form.value))
    }
}