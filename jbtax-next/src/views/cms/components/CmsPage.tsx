"use client";

import { useEffect, useState, Fragment } from "react";

import { DynamicPage } from "@/views/cms/types/dynamicPage";
import { getPageBySlug } from "@/views/cms/services/contentApi";

import { CmsBlocks } from "@/views/cms/components/CmsBlocks";

export const CmsPage = ({
    pageSlug,
    showHero,
    heroClassName,
    containerClassName,
    containerStyle,
}: {
    pageSlug: string;
    showHero: boolean;
    heroClassName: string;
    containerClassName: string;
    containerStyle: React.CSSProperties;
}) => {
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
                <div className={`${heroClassName}-section`}>
                    <div className={`${heroClassName}-title`}>
                        <h1>{activePage?.title}</h1>
                    </div>
                    <div className={`${heroClassName}-image`}>
                        {activePage?.primaryImage?.media?.publicUrl != null && (
                            <img src={activePage?.primaryImage?.media?.publicUrl} alt="Hero" />
                        )}
                    </div>
                </div>
            )}
            <div className={containerClassName} style={containerStyle}>
                {!isLoading && <CmsBlocks page={activePage as DynamicPage} />}
            </div>
        </Fragment>
    );
};
