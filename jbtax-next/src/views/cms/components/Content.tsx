import { useRouter } from "next/navigation";

import React, { Fragment, useEffect, useState } from "react";

import { CmsBlocks } from "./CmsBlocks";

import { DynamicPage } from "../types/dynamicPage";
import { getPageBySlug, getPostBySlug } from "../services/contentApi";

interface Props {
    entityName?: string;
    slugName: string;
    className: string;
    blockClassName?: string;
    renderHero?: (activePage: DynamicPage) => JSX.Element;
    renderContent?: (activePage: DynamicPage) => JSX.Element;
}

const Content = ({ entityName = "page", slugName, className, blockClassName, renderHero, renderContent }: Props): JSX.Element => {
    const [activePage, setActivePage] = useState<DynamicPage | null>(null);

    const router = useRouter();

    const getContent = async () => {
        try {
            let pc: any | null = null;
            if (entityName === "page") {
                pc = await getPageBySlug(slugName, "");
            } else {
                pc = await getPostBySlug(slugName);
            }
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
        if (slugName) {
            getContent();
        }
    }, [slugName]);

    if (activePage == null) return <Fragment />;

    return (
        <div className={className}>
            {renderHero ? renderHero(activePage) : <Fragment />}
            <div className={blockClassName}>{renderContent ? renderContent(activePage) : <CmsBlocks page={activePage} />}</div>
        </div>
    );
};

export default Content;
