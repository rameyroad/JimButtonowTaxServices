"use client";

import Content from "@/views/cms/components/Content";

import { DynamicPage } from "@/views/cms/types/dynamicPage";
import { Fragment, useEffect, useState } from "react";

import "./blog.css"; // Import the CSS file for testing purposes
import { getBlogPostsByTag } from "@/views/cms/services/contentApi";

export default function Page({ params }: { params: { pageSlug: string } }) {
    const [featuredPosts, setFeaturedPosts] = useState<Array<DynamicPage>>([]);

    const getContent = async () => {
        try {
            const posts = await getBlogPostsByTag(["Featured"]);
            if (posts != null) {
                setFeaturedPosts(posts);
            }
        } catch (error) {
            console.log(error);
        } finally {
        }
    };

    useEffect(() => {
        getContent();
    }, []);

    return (
        <Content
            pageSlug={"blog-home"}
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
            renderContent={(activePage: DynamicPage) => (
                <div className="container-lg blog-home-content">
                    {featuredPosts.map((post, index) => (
                        <div key={index} className="blog-home-post row" style={{ margin: "0 0 50px 0" }}>
                            <div className="col-4">
                                <img alt={post.title} src={post.primaryImage?.media?.publicUrl} style={{ width: "100%" }} />
                            </div>
                            <div className="col-8" style={{ margin: "50px 0" }}>
                                <div className="blog-home-post-title">{post.title}</div>
                                <div className="blog-home-post-author">by {post.author?.value ?? ""}</div>
                                <div className="blog-home-post-excerpt">{post.excerpt}</div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        />
    );
}
