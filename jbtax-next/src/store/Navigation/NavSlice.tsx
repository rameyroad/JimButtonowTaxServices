import { getSiteData } from "@/views/cms/services/contentApi";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import { createAsyncThunk } from "@reduxjs/toolkit";

export const fetchSiteData = createAsyncThunk("user/fetchSiteData", async () => {
    const response = await getSiteData();
    return response;
});

export const NavSlice = createSlice({
    name: "NavSlice",
    initialState: {
        currentSite: {} as any,
        isLoading: false,
    },
    reducers: {
        setCurrentSite(state, action: PayloadAction<any>) {
            state.currentSite = action.payload;
        },
        setIsLoading(state, action: PayloadAction<boolean>) {
            state.isLoading = action.payload;
        },
    },
    extraReducers: (builder) => {
        builder.addCase(fetchSiteData.pending, (state, action) => {});
        builder.addCase(fetchSiteData.fulfilled, (state, action) => {
            state.currentSite = action.payload;
        });
        builder.addCase(fetchSiteData.rejected, (state, action) => {
            state.currentSite = {} as any;
        });
    },
});

export const { setCurrentSite, setIsLoading } = NavSlice.actions;

export default NavSlice.reducer;
