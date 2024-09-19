import React, { Fragment, useEffect, useState } from "react";
import { useRouter } from "next/navigation";

import { Hero, HtmlBlock, ColumnBlock, ImageGalleryBlock, ImageBlock, SeparatorBlock, QuoteBlock } from ".";
import { Block, DynamicPage } from "../types/dynamicPage";
import { getPageBySlug } from "../services/contentApi";

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
            const pc = await getPageBySlug(pageSlug, "MainSite");
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

    const renderBlock = (block: Block) => {
        switch (block.$type) {
            case "Ramey.Cms.Content.Blocks.HtmlBlock":
                return <HtmlBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.ColumnBlock":
                return <ColumnBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.ImageGalleryBlock":
                return <ImageGalleryBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.ImageBlock":
                return <ImageBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.SeparatorBlock":
                return <SeparatorBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.QuoteBlock":
                return <QuoteBlock block={block} />;
            default:
                return (
                    <Fragment>
                        <div>No block renderer for block type {block.$type}</div>
                        <div>{JSON.stringify(block)}</div>
                    </Fragment>
                );
        }
    };

    const renderBlockContent = () => {
        if (activePage && activePage.blocks) {
            return (
                <div className="">
                    {activePage.blocks.map((block: Block, index: number) => {
                        return <p key={index}>{renderBlock(block)}</p>;
                    })}
                </div>
            );
        }
        return <Fragment />;
    };

    return (
        <div>
            {activePage?.primaryImage?.media?.publicUrl != null && <Hero page={activePage as DynamicPage} />}
            {!isLoading && renderBlockContent()}
        </div>
    );
};

export default Content;
