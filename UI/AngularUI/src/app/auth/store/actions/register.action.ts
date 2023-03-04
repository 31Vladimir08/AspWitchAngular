import { props, createAction } from "@ngrx/store";
import { RegisterFormInterface } from "../../types/registerForm.interface";

import { ActionType } from "../action.type";

export const registerAction = createAction(
    ActionType.REGISTER,
    props<RegisterFormInterface>()
    )