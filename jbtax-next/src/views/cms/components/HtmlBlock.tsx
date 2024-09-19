import React from "react";

import { Block } from "../types/dynamicPage";

export interface BlockProps {
    block: Block;
}

export const HtmlBlock: React.FC<BlockProps> = ({ block }) => {
    return (
        <div>
            <div className="container">
                <div className="block html-block">
                    <div className="container">
                        <div
                            dangerouslySetInnerHTML={{
                                __html: block?.body?.value ?? "",
                            }}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
};
