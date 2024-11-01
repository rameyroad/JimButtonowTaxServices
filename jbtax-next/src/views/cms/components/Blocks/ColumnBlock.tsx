import React, { Fragment } from "react";

import { Block, Item } from "../../types/dynamicPage";
import { ComponentBlock, HtmlBlock, ImageBlock, ItemPageBlock, QuoteBlock } from "../";

export interface BlockProps {
    block: Block;
}

export interface ItemProps {
    item: Item;
}

export const ColumnBlock: React.FC<BlockProps> = ({ block }) => {
    const renderItem = (item: Item) => {
        switch (item.$type) {
            case "Ramey.Cms.Content.Blocks.ComponentBlock":
                return <ComponentBlock item={item} />;
            case "Ramey.Cms.Content.Blocks.HtmlBlock":
                return <HtmlBlock item={item} />;
            case "Ramey.Cms.Content.Blocks.ImageBlock":
                return <ImageBlock item={item} />;
            case "Ramey.Cms.Content.Blocks.PageBlock":
                return <ItemPageBlock item={item} />;
            case "Ramey.Cms.Content.Blocks.QuoteBlock":
                return <QuoteBlock item={item} />;
            default:
                return (
                    <Fragment>
                        <div>No item renderer for {item.$type}</div>
                    </Fragment>
                );
        }
    };

    const getColumnStyle = (size: string, length: number) => {
        switch (size) {
            case "xs":
            case "sm":
                return `col-${size}-12`;

            case "md":
            case "lg":
            case "xl":
                switch (length % 12) {
                    case 1:
                        return `col-${size}-12`;
                    case 2:
                        return `col-${size}-6`;
                    case 3:
                        return `col-${size}-4`;
                        return 4;
                    case 4:
                        return `col-${size}-3`;
                    case 5:
                    case 6:
                    case 7:
                        return `col-${size}-2`;
                    default:
                        return `col-${size}-1`;
                }
        }
    };

    return (
        <div className="row">
            {block.items?.map((item: Item, key: number) => {
                const numCols = block.items?.length ?? 0;
                // const xs = getColumWidth("xs", numCols);
                // const sm = getColumWidth("sm", numCols);
                const md = getColumnStyle("md", numCols);
                const lg = getColumnStyle("lg", numCols);
                const xl = getColumnStyle("xl", numCols);
                return (
                    <div key={key} className={`col-12 ${md} ${lg} ${xl}`}>
                        {renderItem(item)}
                    </div>
                );
            })}
        </div>
    );
};
