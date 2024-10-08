"use client";

import { useEffect } from "react";
import { useDispatch } from "react-redux";

import { Footer } from "./shared/footer";
import { Header } from "./shared/header";

import { AppDispatch } from "@/store";
import { fetchSiteData } from "@/store/Navigation/NavSlice";

export const MainLayout = ({ children }: { children: React.ReactNode }) => {
    const dispatch = useDispatch<AppDispatch>();
    useEffect(() => {
        dispatch(fetchSiteData());
    }, [dispatch]);

    return (
        <div>
            <Header />
            <main>{children}</main>
            <Footer />
        </div>
    );
};
