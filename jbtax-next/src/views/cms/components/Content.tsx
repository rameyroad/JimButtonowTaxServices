import { useRouter } from "next/navigation";

import React, { Fragment, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { DynamicPage } from "../types/dynamicPage";
import { getPageBySlug } from "../services/contentApi";
import { CmsBlocks } from "./CmsBlocks";
import { AppDispatch, RootState } from "@/store";
import { setIsLoading } from "@/store/Navigation/NavSlice";

interface Props {
    pageSlug: string;
}

const Content = ({ pageSlug }: Props): JSX.Element => {
    const dispatch = useDispatch<AppDispatch>();

    const { isLoading } = useSelector((state: RootState) => state.navigation);

    const [activePage, setActivePage] = useState<DynamicPage | null>(null);

    const router = useRouter();

    const getContent = async () => {
        setIsLoading(true);
        try {
            const pc = await getPageBySlug(pageSlug, "");
            if (pc == null) {
                router.push("/not-found");
            } else {
                setActivePage(pc);
            }
        } catch (error) {
            console.log(error);
            router.push("/not-found");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (pageSlug) {
            getContent();
        }
    }, [pageSlug]);

    if (isLoading) return <Fragment />;

    return (
        // Top level class that will control how an article displays. This contains minimal styling and is meant to be used as a wrapper for the article content.
        <div className="article-main">
            <CmsBlocks page={activePage as DynamicPage} />
        </div>
    );
};

export default Content;
