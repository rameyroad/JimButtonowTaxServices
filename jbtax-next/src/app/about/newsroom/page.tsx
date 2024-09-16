"use client";

import { Fragment } from "react";
import "../../../styles/about.css";

// NewsroomPage Component
const NewsroomPage = () => {
    // Sample articles data
    const articles = [
        { title: "Article One", date: "September 16, 2024", summary: "This is a summary of the first article." },
        { title: "Article Two", date: "September 15, 2024", summary: "This is a summary of the second article." },
        { title: "Article Three", date: "September 14, 2024", summary: "This is a summary of the third article." }
    ];

    return (
        <Fragment>
            <section className="hero-newsroom">
                <div className="hero-content">
                    <img src="/images/peru-scaled.jpeg" alt="Newsroom Hero" />
                    <div className="hero-text">
                        <h1>Newsroom</h1>
                    </div>
                </div>
            </section>
            <section className="articles">
                <div className="container">
                    {articles.map((article, index) => (
                        <div key={index} className="article">
                            <h2>{article.title}</h2>
                            <p><strong>Date:</strong> {article.date}</p>
                            <p>{article.summary}</p>
                        </div>
                    ))}
                </div>
            </section>
        </Fragment>
    );
};

export default NewsroomPage;
