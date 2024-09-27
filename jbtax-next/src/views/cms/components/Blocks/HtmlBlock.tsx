import React from "react";

import DOMPurify from "dompurify";
import styled from "styled-components";

import { Block, Item } from "../../types/dynamicPage";

export interface BlockProps {
    block?: Block;
    item?: Item;
}

export const HtmlBlock: React.FC<BlockProps> = ({ block, item }) => {
    const htmlContent = block?.body?.value ?? item?.body?.value ?? "No Content";
    const styles = block?.cssStyles?.value ?? item?.cssStyles?.value ?? "";

    const StyleWrapper = styled.div`
        ${styles}
    `;

    return (
        <div className="block html-block">
            <StyleWrapper className="block-content" dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(htmlContent) }} />
        </div>
    );
};
