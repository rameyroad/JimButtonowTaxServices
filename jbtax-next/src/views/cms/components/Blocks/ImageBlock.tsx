import React from "react";

import { Block } from "../../types/dynamicPage";

export interface BlockProps {
    block: Block;
}

export const ImageBlock: React.FC<BlockProps> = () => {
    return <div className="block image-block">image-block</div>;
};
