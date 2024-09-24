"use client";

import { useEffect, useState, Fragment } from "react";

import { DynamicPage } from "@/views/cms/types/dynamicPage";
import { getPageBySlug } from "@/views/cms/services/contentApi";

import { CmsBlocks } from "@/views/cms/components/CmsBlocks";

export const CmsPage = ({ pageSlug, showHero }: { pageSlug: string; showHero: boolean }) => {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [activePage, setActivePage] = useState<DynamicPage | null>(null);

    const getContent = async () => {
        setIsLoading(true);
        try {
            const pc = await getPageBySlug(pageSlug, "");
            if (pc != null) {
                setActivePage(pc);
            }
        } catch (error) {
            console.log(error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (pageSlug) {
            getContent();
        }
    }, [pageSlug]);

    return (
        <Fragment>
            {showHero && (
                <div className="hero-section">
                    <div className="hero-title">
                        <h1>{activePage?.title}</h1>
                    </div>
                    <div className="hero-image">
                        {activePage?.primaryImage?.media?.publicUrl != null && (
                            <img src={activePage?.primaryImage?.media?.publicUrl} alt="Hero" />
                        )}
                    </div>
                </div>
            )}
            <div className="container">
                <div className="content-wrapper">
                    <div className="article-content">
                        {!isLoading && <CmsBlocks page={activePage as DynamicPage} />}
                    </div>
                </div>
            </div>
        </Fragment>
    );
};
