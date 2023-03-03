import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

@Component({
    selector: 'mc-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
    form: FormGroup

    constructor(private fb: FormBuilder) {
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
            isAgryAllVtatements: false
        })
    }

    onSubmit(): void {
        console.log(this.form.value)
    }
}