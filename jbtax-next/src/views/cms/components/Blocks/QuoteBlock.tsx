import React from "react";

import { Block, Item } from "../../types/dynamicPage";

export interface QuoteBlockProps {
    block?: Block;
    item?: Item;
}

export const QuoteBlock: React.FC<QuoteBlockProps> = ({ block, item }) => {
    if (block != null) {
        return (
            <div className="card" style={{ width: "18rem" }}>
                <div className="card-body">
                    <span className="subtitle1">{block?.body?.value}</span>
                    <span className="subtitle1" style={{ fontStyle: "italic", textAlign: "end" }}>
                        -- {block?.author?.value}
                    </span>
                </div>
            </div>
        );
    }

    return (
        <div className="card" style={{ padding: "10px", maxWidth: "100%" }}>
            <span className="subtitle1">{item?.body?.value}</span>
            <span className="subtitle1" style={{ fontStyle: "italic", textAlign: "end" }}>
                -- {item?.author?.value}
            </span>
        </div>
    );
};
