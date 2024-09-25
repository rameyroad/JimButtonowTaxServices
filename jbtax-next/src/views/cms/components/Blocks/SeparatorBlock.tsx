import React from "react";

import { Block } from "../../types/dynamicPage";

export interface BlockProps {
    block: Block;
}

export const SeparatorBlock: React.FC<BlockProps> = () => {
    return <hr style={{ margin: "5px 10px;" }} />;
};
