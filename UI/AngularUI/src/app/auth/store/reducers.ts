import { Action, createReducer, on } from "@ngrx/store";
import { AuthStateInterface } from "../types/authState.interface";
import { getCurrentUserAction, getCurrentUserFailureAction, getCurrentUserSuccessAction } from "./actions/getCurrentUser.action";
import { loginAction, loginFailureAction, loginSuccessAction } from "./actions/login.action";
import { registerAction, registerFailureAction, registerSuccessAction } from "./actions/register.action";

const initialState: AuthStateInterface = {
    isSubmitting: false,
    currentUser: null,
    isLoggedIn: null,
    validationErrors: null,
    isLoading: false
}

const authReducer = createReducer(
    initialState,
    on(
        registerAction, 
        (state): AuthStateInterface =>({
            ...state,
            isSubmitting: true,
            validationErrors: null
        })
    ),
    on(
        registerSuccessAction,
        (satate, action): AuthStateInterface => ({
            ...satate,
            isSubmitting: false,
            isLoggedIn: true,
            currentUser: action.currentUser
        })
    ),
    on(
        registerFailureAction,
        (state, action): AuthStateInterface =>({
            ...state,
            isSubmitting: false,
            validationErrors: action.errors,
        })
    ),
    on(
        loginAction,
        (state): AuthStateInterface => ({
            ...state,
            isSubmitting: true,
            validationErrors: null
        })
    ),
    on(
        loginSuccessAction,
        (state, action): AuthStateInterface => ({
            ...state,
            isSubmitting: false,
            currentUser: action.currentUser,
            isLoggedIn: true
        })
    ),
    on(
        loginFailureAction,
        (state, action): AuthStateInterface => ({
            ...state,
            isSubmitting: false,
            isLoading: false,
            validationErrors: action.errors
        })
    ),
    on(
        getCurrentUserAction,
        (state): AuthStateInterface => ({
            ...state,
            isLoading: true
        })
    ),
    on(
        getCurrentUserSuccessAction,
        (state, action): AuthStateInterface => ({
            ...state,
            isLoading: false,
            isLoggedIn: true,
            currentUser: action.currentUser
        })
    ),
    on(
        getCurrentUserFailureAction,
        (state): AuthStateInterface => ({
            ...state,
            isLoading: false,
            isLoggedIn: false,
            currentUser: null
        })
    )
)

export function reducers(state: AuthStateInterface, action: Action) {
    return authReducer(state, action)
}