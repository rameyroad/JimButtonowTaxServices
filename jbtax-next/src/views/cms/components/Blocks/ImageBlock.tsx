import React from "react";

import { Block, Item } from "../../types/dynamicPage";

export interface BlockProps {
    block?: Block;
    item?: Item;
}

export const ImageBlock: React.FC<BlockProps> = ({ block, item }) => {
    const publicUrl = block?.body?.media?.publicUrl ?? item?.body?.media?.publicUrl ?? "";
    const altText = block?.body?.media?.altText ?? item?.body?.media?.altText ?? "";

    return (
        <div className="block image-block">
            <img src={publicUrl} alt={altText} style={{ maxWidth: "100%" }} />
        </div>
    );
};
