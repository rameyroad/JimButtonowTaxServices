import React, { Fragment } from "react";

import { ItemHtmlBlock, ItemImageBlock, ItemPageBlock, QuoteBlock } from ".";
import { Block, Item } from "../types/dynamicPage";

export interface BlockProps {
    block: Block;
}

export interface ItemProps {
    item: Item;
}

export const ColumnBlock: React.FC<BlockProps> = ({ block }) => {
    const renderItem = (item: Item) => {
        switch (item.$type) {
            case "Ramey.Cms.Content.Blocks.HtmlBlock":
                return <ItemHtmlBlock item={item} />;
            case "Ramey.Cms.Content.Blocks.ImageBlock":
                return <ItemImageBlock item={item} />;
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

    const getColumWidth = (size: string, length: number) => {
        switch (size) {
            case "xs":
            case "sm":
                return 12;

            case "md":
            case "lg":
                switch (length % 12) {
                    case 1:
                        return 12;
                    case 2:
                        return 6;
                    case 3:
                        return 4;
                    case 4:
                        return 3;
                    case 5:
                    case 6:
                    case 7:
                        return 2;
                    default:
                        return 1;
                }
        }
    };

    return (
        <div className="row">
            {block.items?.map((item: Item, key: number) => {
                return (
                    <div
                        key={key}
                        className={`col col-sm-${getColumWidth("sm", block.items?.length ?? 0)} col-md-${getColumWidth(
                            "md",
                            block.items?.length ?? 0
                        )}`}
                        style={{ padding: "10px" }}
                    >
                        {renderItem(item)}
                    </div>
                );
            })}
        </div>
    );
};
