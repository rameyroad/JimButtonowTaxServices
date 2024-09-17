"use client";

import { useEffect, useState, Fragment } from "react";

// Hero Component
const Hero = () => {
    const [currentImageIndex, setCurrentImageIndex] = useState(0);

    const images = ["/images/Home_-_father_son.webp", "/images/Home_-_backpacking_couple.webp", "/images/Home_-_woman_eiffel-3.webp"];

    useEffect(() => {
        const interval = setInterval(() => {
            setCurrentImageIndex((prevIndex) => (prevIndex + 1) % images.length);
        }, 3000); // Change image every 3 seconds

        return () => clearInterval(interval); // Cleanup on component unmount
    }, [images.length]);

    return (
        <section className="hero">
            <div className="hero-content">
                <div className="header-images">
                    {images.map((src, index) => (
                        <img key={index} src={src} alt={`Tax Services Image ${index + 1}`} className={currentImageIndex === index ? "active" : ""} />
                    ))}
                </div>
                <div className="text-content">
                    <h1>US Expat tax services and preparation done right.</h1>
                    <p>We take the hassle out of filing your American Expat taxes so you can get back to your adventure abroad.</p>
                </div>
            </div>
        </section>
    );
};

// BusinessStats Component
const BusinessStats = () => {
    return (
        <section className="business-stats">
            <div className="stats-item">
                <h3>10+ Years</h3>
                <p>Experience in Expat Tax Services</p>
            </div>
            <div className="stats-item">
                <h3>500+</h3>
                <p>Clients Served Globally</p>
            </div>
            <div className="stats-item">
                <h3>99%</h3>
                <p>Client Satisfaction Rate</p>
            </div>
        </section>
    );
};

// HomeContent Component
const HomeContent = () => {
    return (
        <div className="content-sidebar-wrapper">
            <main>
                <div className="main-content">
                    <div className="container-form-home" style={{ minHeight: "736px" }}>
                        <h2>Start Your US Expat Tax Return</h2>
                        <form method="post" encType="multipart/form-data" noValidate>
                            <div className="gform-body">
                                <ul className="gform_fields">
                                    <li className="gfield">
                                        <label htmlFor="first-name">First Name *</label>
                                        <input type="text" id="first-name" name="first-name" required />
                                    </li>
                                    <li className="gfield">
                                        <label htmlFor="last-name">Last Name *</label>
                                        <input type="text" id="last-name" name="last-name" required />
                                    </li>
                                    <li className="gfield">
                                        <label htmlFor="email">Email Address *</label>
                                        <input type="email" id="email" name="email" required />
                                    </li>
                                    <li className="gfield">
                                        <label htmlFor="country">Country You Currently Live In *</label>
                                        <select id="country" name="country" required>
                                            <option value="">--Select Country--</option>
                                            {/* Add more countries as options */}
                                        </select>
                                    </li>
                                    <li className="gfield">
                                        <label htmlFor="comments">Questions or Comments</label>
                                        <textarea id="comments" name="comments" rows={4}></textarea>
                                    </li>
                                </ul>
                                <div className="gform_footer">
                                    <input type="submit" className="gform_button" value="Get Started Now" />
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </main>

            <aside className="sidebar">
                <h1>Bigger Title</h1>
                <ul>
                    <li>
                        <h3>Title</h3>
                        <p>subtext</p>
                    </li>
                    <li>
                        <h3>Title</h3>
                        <p>subtext</p>
                    </li>
                    <li>
                        <h3>Title</h3>
                        <p>subtext</p>
                    </li>
                    <li>
                        <h3>Title</h3>
                        <p>subtext</p>
                    </li>
                </ul>
            </aside>
        </div>
    );
};

const DifferenceContent = () => {
    return (
        <div className="difference-section">
            <div className="difference-image">
                <img src="/images/JL Buttonow CPA Difference logo.png" alt="JL Buttonow CPA Difference logo" />
            </div>
            <div className="difference-content">
                <h1>The JL Buttonow Difference</h1>
                <div className="difference-item">
                    <h4>Bold Text Title</h4>
                    <p>Smaller Descriptive non bold text</p>
                </div>
                <div className="difference-item">
                    <h4>Bold Text Title</h4>
                    <p>Smaller Descriptive non bold text</p>
                </div>
                <div className="difference-item">
                    <h4>Bold Text Title</h4>
                    <p>Smaller Descriptive non bold text</p>
                </div>
            </div>
        </div>

    );

};

// ReviewSection Component
const ReviewSection = () => {
    return (
        <section className="reviews-section">
            <div className="reviews-wrapper">
                {/* Text Bubble */}
                <div className="testimonial-intro">
                    <p>Well take of your problems and make your life stressless.</p>
                </div>

                {/* Customer Reviews */}
                <div className="individual-reviews">
                    <div className="review">
                        <span>- Susan L. (Germany)</span>
                        <p>“Samuel was a huge help with our complicated tax situation. Highly recommend!”</p>

                    </div>
                    <div className="review">
                        <span>- Michael K. (UK)</span>
                        <p>“Great service and attention to detail. The best tax services for expats!”</p>

                    </div>
                </div>
            </div>
        </section>
    );
};

const WhatWeDeliver = () => {
    return (

        <section className="what-we-deliver">
            <h1>What We Deliver</h1>
            <div className="deliver-content">
                <div className="deliver-left">
                    <ul className="deliver-list">
                        <li className="deliver-item">
                            <h3>Expert Guidance</h3>
                            <p>We provide detailed tax advice tailored for expats.</p>
                        </li>
                        <li className="deliver-item">
                            <h3>Global Coverage</h3>
                            <p>Serving clients in over 100 countries around the world.</p>
                        </li>
                        <li className="deliver-item">
                            <h3>Client Satisfaction</h3>
                            <p>Our services are trusted by thousands of expats worldwide.</p>
                        </li>
                    </ul>
                </div>
                <div className="deliver-right">
                    <img src="/images/JL Buttonow CPA logo 2.png" alt="What We Deliver Image" />
                </div>
            </div>
        </section>




    );
};


// Main Component
export default function Home() {
    return (
        <Fragment>
            <Hero />
            <BusinessStats />
            <HomeContent />
            <DifferenceContent />
            <ReviewSection />
            <WhatWeDeliver />
        </Fragment>
    );
}
