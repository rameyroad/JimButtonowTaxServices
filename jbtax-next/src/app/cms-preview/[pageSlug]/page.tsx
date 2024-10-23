"use client";

import Content from "@/views/cms/components/Content";

export default function Page({ params }: { params: { pageSlug: string } }) {
    return (
        <div className="container">
            <Content slugName={params.pageSlug} className="" />
        </div>
    );
}
