import React from "react";

import DOMPurify from "dompurify";

import { Block, Item } from "../../types/dynamicPage";

export interface QuoteBlockProps {
    block?: Block;
    item?: Item;
}

export const QuoteBlock: React.FC<QuoteBlockProps> = ({ block, item }) => {
    return (
        <div className={`block quote-block ${block?.slug?.value}`} id={block?.slug?.value}>
            <div className="quote-content" dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(block?.body?.value as string) }} />
            <span className="subtitle1" style={{ fontStyle: "italic", textAlign: "end" }}>
                -- {block?.author?.value}
            </span>
        </div>
    );
};
