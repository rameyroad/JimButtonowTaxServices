"use client";

import Content from "@/views/cms/components/Content";

import { DynamicPage } from "@/views/cms/types/dynamicPage";
import { Fragment } from "react";

import "./who-we-help.css"; // Import the CSS file for testing purposes

export default function Page({ params }: { params: { pageSlug: string } }) {
    return (
        <Content
            pageSlug={"who-we-help"}
            className="who-we-help-main"
            renderHero={(activePage: DynamicPage) => (
                <Fragment>
                    {activePage?.primaryImage?.media?.publicUrl != null && (
                        <Fragment>
                            <div className="who-we-help-hero" style={{ paddingBottom: "200px;" }}>
                                <img alt={activePage?.title} src={activePage?.primaryImage?.media?.publicUrl} />
                            </div>
                            <div className="who-we-help-title">{activePage.title}</div>
                        </Fragment>
                    )}
                </Fragment>
            )}
        />
    );
}
