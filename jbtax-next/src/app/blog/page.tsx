import { Fragment } from "react";

import "../../styles/blogpage.css";
// import "../../styles/blog-post.css";

export default function Blog() {
    return (
        <Fragment>
            <section className="blog-header">
                <div className="header-content">
                    <div className="blog-post-title">
                        <h2>Understanding Expat Tax Requirements for 2024</h2>
                    </div>
                    <div className="blog-post-image">
                        <img src="css/images/foreign-trusts-600x400.jpeg" alt="Understanding Expat Tax Requirements for 2024" />
                    </div>
                </div>
            </section>

            <main>
                <section className="blog-post-content">
                    <p className="date">September 4, 2024</p>
                    <p>
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur nec est nec sapien bibendum fringilla. Proin sit amet semper
                        sapien...
                    </p>
                    <p>Vivamus accumsan, mi at aliquet feugiat, ligula arcu tincidunt est...</p>
                    <p>Suspendisse potenti. In at libero eu ligula dictum tincidunt...</p>
                </section>

                <section className="related-articles">
                    <h3>Related Articles</h3>
                    <div className="article-grid">
                        <article className="related-article">
                            <img src="https://via.placeholder.com/300x200" alt="Top 5 Tips for Filing Expat Taxes" />
                            <div className="related-article-content">
                                <h4>
                                    <a href="blog-post2.html">Top 5 Tips for Filing Expat Taxes</a>
                                </h4>
                                <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit...</p>
                            </div>
                        </article>

                        <article className="related-article">
                            <img src="https://via.placeholder.com/300x200" alt="How to Save on Expat Taxes: A Guide" />
                            <div className="related-article-content">
                                <h4>
                                    <a href="blog-post3.html">How to Save on Expat Taxes: A Guide</a>
                                </h4>
                                <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit...</p>
                            </div>
                        </article>

                        <article className="related-article">
                            <img src="https://via.placeholder.com/300x200" alt="Understanding Expat Tax Benefits" />
                            <div className="related-article-content">
                                <h4>
                                    <a href="blog-post4.html">Understanding Expat Tax Benefits</a>
                                </h4>
                                <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit...</p>
                            </div>
                        </article>
                    </div>
                </section>
            </main>
        </Fragment>
    );
}
