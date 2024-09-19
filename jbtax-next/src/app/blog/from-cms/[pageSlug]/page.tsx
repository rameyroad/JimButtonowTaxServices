"use client";

import Content from "@/views/cms/components/Content";
import { Fragment } from "react";

export default function Page({ params }: { params: { pageSlug: string } }) {
    return (
        <Fragment>
            <div className="container">
                <Content pageSlug={params.pageSlug} />
            </div>
        </Fragment>
    );
}
