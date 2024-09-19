import React from "react";

import { Block, Item } from "../types/dynamicPage";

export interface BlockProps {
    block: Block;
}

export interface ItemProps {
    item: Item;
}

export const ItemImageBlock: React.FC<ItemProps> = ({ item }) => {
    return (
        <div className="block image-block">
            <img
                src={item.body?.media?.publicUrl ?? ""}
                alt={item.body?.media?.altText ?? ""}
                style={{ maxWidth: "100%" }}
            />
        </div>
    );
};
