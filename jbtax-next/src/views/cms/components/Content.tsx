import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";

import { Hero } from ".";
import { DynamicPage } from "../types/dynamicPage";
import { getPageBySlug } from "../services/contentApi";
import { CmsBlocks } from "./CmsBlocks";

interface Props {
    pageSlug: string;
}

const Content = ({ pageSlug }: Props): JSX.Element => {
    const [isLoading, setIsLoading] = useState<boolean>(false);
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

    return (
        <div>
            {activePage?.primaryImage?.media?.publicUrl != null && <Hero page={activePage as DynamicPage} />}
            {!isLoading && <CmsBlocks page={activePage as DynamicPage} />}
        </div>
    );
};

export default Content;
