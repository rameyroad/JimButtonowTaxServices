"use client";

import Content from "@/views/cms/components/Content";

import "./article.css"; // Import the CSS file

export default function Page({ params }: { params: { pageSlug: string } }) {
    return <Content pageSlug={params.pageSlug} />;
}
