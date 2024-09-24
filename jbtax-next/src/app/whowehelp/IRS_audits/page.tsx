"use client";

import { useEffect, useState, Fragment } from "react";

import { Block, DynamicPage } from "@/views/cms/types/dynamicPage";
import { getPageBySlug } from "@/views/cms/services/contentApi";

import {
    ColumnBlock,
    HtmlBlock,
    ImageBlock,
    ImageGalleryBlock,
    QuoteBlock,
    SeparatorBlock,
} from "@/views/cms/components";

import "../../../styles/articles.css"; // Assuming styles are in this file

const CmsBlocks = ({ page }: { page: DynamicPage }) => {
    const renderBlock = (block: Block) => {
        switch (block.$type) {
            case "Ramey.Cms.Content.Blocks.HtmlBlock":
                return <HtmlBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.ColumnBlock":
                return <ColumnBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.ImageGalleryBlock":
                return <ImageGalleryBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.ImageBlock":
                return <ImageBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.SeparatorBlock":
                return <SeparatorBlock block={block} />;
            case "Ramey.Cms.Content.Blocks.QuoteBlock":
                return <QuoteBlock block={block} />;
            default:
                return (
                    <Fragment>
                        <div>No block renderer for block type {block.$type}</div>
                        <div>{JSON.stringify(block)}</div>
                    </Fragment>
                );
        }
    };

    return (
        <Fragment>
            {page?.blocks.map((block: Block, index: number) => {
                return <Fragment key={index}>{renderBlock(block)}</Fragment>;
            })}
        </Fragment>
    );
};

const CmsFullPage = ({ pageSlug }: { pageSlug: string }) => {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [activePage, setActivePage] = useState<DynamicPage | null>(null);

    const getContent = async () => {
        setIsLoading(true);
        try {
            const pc = await getPageBySlug(pageSlug, "");
            if (pc != null) {
                setActivePage(pc);
            }
        } catch (error) {
            console.log(error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (pageSlug) {
            getContent();
        }
    }, [pageSlug]);

    return (
        <Fragment>
            <div className="hero-section">
                <div className="hero-title">
                    <h1>{activePage?.title}</h1>
                </div>
                <div className="hero-image">
                    {activePage?.primaryImage?.media?.publicUrl != null && (
                        <img src={activePage?.primaryImage?.media?.publicUrl} alt="Hero" />
                    )}
                </div>
            </div>
            <div className="container">
                <div className="content-wrapper">
                    <div className="article-content">
                        {!isLoading && <CmsBlocks page={activePage as DynamicPage} />}
                    </div>
                </div>
            </div>
        </Fragment>
    );
};

// const PreviousContent = () => {
//     return (
//         <Fragment>
//             <div className="hero-section">
//                 <div className="hero-title">
//                     <h1>Title of the Article</h1>
//                 </div>
//                 <div className="hero-image">
//                     <img src="/images/peru-scaled.jpeg" alt="Hero" />
//                 </div>
//             </div>

//             <div className="author-section">
//                 <img src="/images/madisonwhitfieldphotoshot.jpg" alt="Article Image" className="article-image" />
//                 <span className="author-name">Written by John Doe</span>
//             </div>

//             <div className="content-wrapper">
//                 <div className="article-content">
//                     <section id="section1">
//                         <h2>Section 1</h2>
//                         <p>Content for section 1...</p>
//                     </section>
//                     <section id="section2">
//                         <h2>Section 2</h2>
//                         <p>Content for section 2...</p>
//                     </section>
//                     <section id="section3">
//                         <h2>Section 3</h2>
//                         <p>Content for section 3...</p>
//                     </section>
//                 </div>

//                 <aside className="table-of-contents">
//                     <h3>Table of Contents</h3>
//                     <ul>
//                         <li>
//                             <a href="#section1">Section 1</a>
//                         </li>
//                         <li>
//                             <a href="#section2">Section 2</a>
//                         </li>
//                         <li>
//                             <a href="#section3">Section 3</a>
//                         </li>
//                     </ul>
//                 </aside>
//             </div>
//         </Fragment>
//     );
// };

export default function Page() {
    return (
        <Fragment>
            <CmsFullPage pageSlug="IRS_Audits" />
        </Fragment>
    );
}
