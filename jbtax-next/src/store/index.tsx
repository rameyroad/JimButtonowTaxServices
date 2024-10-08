import { configureStore } from "@reduxjs/toolkit";
import navReducer from "./Navigation/NavSlice";

export const store = configureStore({
    reducer: {
        navigation: navReducer,
    },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
