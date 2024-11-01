import { Fragment } from "react";

import { Block, DynamicPage } from "@/views/cms/types/dynamicPage";

import { ColumnBlock, ComponentBlock, HtmlBlock, ImageBlock, ImageGalleryBlock, QuoteBlock, SeparatorBlock } from "@/views/cms/components";

export const CmsBlocks = ({ page }: { page: DynamicPage }) => {
    const renderBlock = (block: Block) => {
        switch (block.$type) {
            case "Ramey.Cms.Content.Blocks.ColumnBlock":
                return <ColumnBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.ComponentBlock":
                return <ComponentBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.HtmlBlock":
                return <HtmlBlock block={block} />;
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

    return (
        <div className={`cms-blocks`} id={page?.slug}>
            {page?.blocks.map((block: Block, index: number) => {
                return <Fragment key={index}>{renderBlock(block)}</Fragment>;
            })}
        </div>
    );
};
