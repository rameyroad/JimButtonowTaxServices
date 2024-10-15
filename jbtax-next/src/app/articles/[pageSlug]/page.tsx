"use client";

import Content from "@/views/cms/components/Content";

import { DynamicPage } from "@/views/cms/types/dynamicPage";
import { Fragment } from "react";

// import "./article.css"; // Import the CSS file for testing purposes

export default function Page({ params }: { params: { pageSlug: string } }) {
    return (
        <Content
            pageSlug={params.pageSlug}
            className="article-main"
            renderHero={(activePage: DynamicPage) => (
                <Fragment>
                    <div className="row header">
                        <div className="col-12 col-md-6">
                            <div className="article-title">{activePage.title}</div>
                        </div>
                        <div className="col-12 col-md-6">
                            {activePage?.primaryImage?.media?.publicUrl != null && (
                                <div className="article-hero">
                                    <img alt={activePage?.title} src={activePage?.primaryImage?.media?.publicUrl} />
                                </div>
                            )}
                        </div>
                    </div>
                </Fragment>
            )}
        />
    );
}
