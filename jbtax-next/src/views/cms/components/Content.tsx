import { useRouter } from "next/navigation";

import React, { Fragment, useEffect, useState } from "react";

import { CmsBlocks } from "./CmsBlocks";

import { DynamicPage } from "../types/dynamicPage";
import { getPageBySlug } from "../services/contentApi";

interface Props {
    pageSlug: string;
    className: string;
    renderHero?: (activePage: DynamicPage) => JSX.Element;
}

const Content = ({ pageSlug, className, renderHero }: Props): JSX.Element => {
    const [activePage, setActivePage] = useState<DynamicPage | null>(null);

    const router = useRouter();

    const getContent = async () => {
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
        }
    };

    useEffect(() => {
        if (pageSlug) {
            getContent();
        }
    }, [pageSlug]);

    if (activePage == null) return <Fragment />;

    return (
        <div className={className}>
            {renderHero ? renderHero(activePage) : <Fragment />}
            <CmsBlocks page={activePage} />
        </div>
    );
};

export default Content;
