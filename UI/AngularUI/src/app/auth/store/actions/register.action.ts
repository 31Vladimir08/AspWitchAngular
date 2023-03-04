import { props, createAction } from "@ngrx/store";
import { CurrentUserInterface } from "src/app/shared/types/currentUser.interface";
import { RegisterRequestInterface } from "../../types/registerRequest.interface";

import { ActionType } from "../action.type";

export const registerAction = createAction(
    ActionType.REGISTER,
    props<{request: RegisterRequestInterface}>()
)

export const registerSuccessAction = createAction(
    ActionType.REGISTER_SUCCESS,
    props<{currentUser: CurrentUserInterface}>()
)

export const registerFailureAction = createAction(
    ActionType.REGISTER_FAILURE
)