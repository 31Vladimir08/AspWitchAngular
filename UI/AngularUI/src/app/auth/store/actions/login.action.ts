import { props, createAction } from "@ngrx/store";

import { BackendErrorsInterface } from "src/app/shared/types/backendErrors.interface";
import { CurrentUserInterface } from "src/app/shared/types/currentUser.interface";
import { LoginRequestInterface } from "../../types/loginRequest.interface";
import { ActionType } from "../action.type";

export const loginAction = createAction(
    ActionType.LOGIN,
    props<{request: LoginRequestInterface}>()
)

export const loginSuccessAction = createAction(
    ActionType.LOGIN_SUCCESS,
    props<{currentUser: CurrentUserInterface}>()
)

export const loginFailureAction = createAction(
    ActionType.LOGIN_FAILURE,
    props<{errors: BackendErrorsInterface}>()
)