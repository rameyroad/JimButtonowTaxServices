import React from "react";

import { Block, Item } from "../types/dynamicPage";

export interface BlockProps {
    block: Block;
}

export interface ItemProps {
    item: Item;
}

export const ItemHtmlBlock: React.FC<ItemProps> = ({ item }) => {
    return (
        <div className="block html-block">
            <div className="container">
                <div
                    dangerouslySetInnerHTML={{
                        __html: item?.body?.value ?? "",
                    }}
                />
            </div>
        </div>
    );
};
