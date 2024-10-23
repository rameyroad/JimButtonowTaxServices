"use client";

import Content from "@/views/cms/components/Content";

import { DynamicPage } from "@/views/cms/types/dynamicPage";
import { Fragment } from "react";

import "../blog.css"; // Import the CSS file for testing purposes

export default function Page({ params }: { params: { pageSlug: string } }) {
    return (
        <Content
            entityName="post"
            slugName={params.pageSlug}
            className="blog-home-main"
            renderHero={(activePage: DynamicPage) => (
                <Fragment>
                    {activePage?.primaryImage?.media?.publicUrl != null && (
                        <Fragment>
                            <div className="blog-home-hero" style={{ paddingBottom: "200px;" }}>
                                <img alt={activePage?.title} src={activePage?.primaryImage?.media?.publicUrl} />
                            </div>
                            <div className="blog-home-title">{activePage.title}</div>
                        </Fragment>
                    )}
                </Fragment>
            )}
        />
    );
}
