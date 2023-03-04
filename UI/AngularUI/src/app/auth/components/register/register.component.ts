import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Store } from "@ngrx/store";
import { registerAction } from "../../store/actions/register.action";

@Component({
    selector: 'mc-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
    form: FormGroup

    constructor(private fb: FormBuilder, private store: Store) {
    }

    ngOnInit(): void {
        this.initializeForm()
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