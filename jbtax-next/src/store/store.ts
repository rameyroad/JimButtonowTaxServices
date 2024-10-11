import { configureStore, ThunkAction, Action } from "@reduxjs/toolkit";
import { NavSlice } from "./Navigation/NavSlice";

export const makeStore = () =>
    configureStore({
        reducer: {
            [NavSlice.name]: NavSlice.reducer,
        },
        devTools: true,
    });

export type AppStore = ReturnType<typeof makeStore>;
export type AppState = ReturnType<AppStore["getState"]>;
export type AppThunk<ReturnType = void> = ThunkAction<ReturnType, AppState, unknown, Action>;
