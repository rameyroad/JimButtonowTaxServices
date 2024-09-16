"use client";

import { Fragment } from "react";
import "../../../styles/about.css";


// OurPartnersPage Component
const OurPartnersPage = () => {
    // Sample partner data
    const partners = [
        { name: "Partner One", imageSrc: "/images/wolterskluwerlogo.jpg" },
        { name: "Partner Two", imageSrc: "/images/Home_-_woman_eiffel-3.webp" },
        { name: "Partner Three", imageSrc: "/images/Home_-_father_son.jpeg" },
    ];

    return (
        <Fragment>
            <section className="partners">
                <div className="container">
                    <h1>Our Partners</h1>
                    <div className="grid-container">
                        {partners.map((partner, index) => (
                            <div key={index} className="partner-card">
                                <img src={partner.imageSrc} alt={partner.name} />
                                <p>{partner.name}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </section>
        </Fragment>
    );
};

export default OurPartnersPage;
