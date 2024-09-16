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
                    <h1>Contact Us</h1>
                    <p>Please fill in the form below</p>
                </div>
            </div>
        </section>
    );
};

// Spacer Component
const Spacer = () => (
    <div className="spacer"></div>
);

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
        </div>
    );
};

// Main Contact Page Component
export default function ContactPage() {
    return (
        <Fragment>
            <Hero />
            <HomeContent />
            <Spacer /> {/* Added To make site look clean */}
            <Spacer />
            <Spacer />
            <Spacer />
            <Spacer />
            <Spacer />
        </Fragment>
    );
}
