import React, { Fragment } from "react";

import { Block, Item } from "../../types/dynamicPage";

import { MainContactForm } from "../MainContactForm";

export interface ComponentBlockProps {
    block?: Block;
    item?: Item;
}

export const ComponentBlock: React.FC<ComponentBlockProps> = ({ block, item }) => {
    const renderComponent = (block?: Block, item?: Item) => {
        const componentName = block?.componentName?.value ?? item?.componentName?.value ?? "";
        const componentProps = block?.componentProps?.value ?? item?.componentProps?.value ?? "";

        switch (componentName) {
            case "Main Contact Form":
                return <MainContactForm />;
            default:
                <Fragment />;
        }
    };

    return <Fragment>{renderComponent(block, item)}</Fragment>;
};
