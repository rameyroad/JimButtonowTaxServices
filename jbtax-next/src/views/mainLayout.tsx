"use client";

import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";

import styled from "styled-components";

import { Footer } from "./shared/footer";
import { Header } from "./shared/header";

import { AppDispatch, RootState } from "@/store";
import { fetchSiteData } from "@/store/Navigation/NavSlice";

export const MainLayout = ({ children }: { children: React.ReactNode }) => {
    const dispatch = useDispatch<AppDispatch>();

    const { currentSite } = useSelector((state: RootState) => state.navigation);

    useEffect(() => {
        if (!currentSite) {
            dispatch(fetchSiteData());
        }
    }, []);

    useEffect(() => {}, [currentSite]);

    const StyleWrapper = styled.div`
        ${currentSite?.cssStyles?.value}
    `;

    return (
        <StyleWrapper>
            <Header />
            <main>{children}</main>
            <Footer />
        </StyleWrapper>
    );
};
