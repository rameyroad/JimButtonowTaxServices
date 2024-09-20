"use client";

import { Fragment } from "react";
import "../../../styles/resources.css"; // Reusing the same CSS for consistency

// Data for services items
const services = [
  {
    title: "IRS Audit Representation",
    description: "Professional representation for IRS audits.",
    link: "/irs-audit-representation",
  },
  {
    title: "IRS and State Collection Agreements",
    description: "Assist with collection agreements at federal and state levels.",
    link: "/irs-state-collection-agreements",
  },
  {
    title: "Unfiled Back Tax Returns",
    description: "Help you file your back taxes and resolve any associated issues.",
    link: "/unfiled-back-tax-returns",
  },
  {
    title: "IRS and State Penalty Relief",
    description: "Relief from penalties imposed by the IRS or state tax authorities.",
    link: "/irs-state-penalty-relief",
  },
  {
    title: "IRS Account Review",
    description: "Comprehensive review of your IRS account and tax filings.",
    link: "/irs-account-review",
  },
  {
    title: "Tax Firm Consultation",
    description: "Get expert tax advice from experienced professionals.",
    link: "/tax-firm-consultation",
  },
  {
    title: "IRS Notice Consultation",
    description: "Expert consultation on IRS notices you've received.",
    link: "/irs-notice-consultation",
  },
  {
    title: "Business and Payroll Representation and Notice Resolution",
    description: "Resolve business tax and payroll issues with expert representation.",
    link: "/business-payroll-representation",
  },
];

export default function Page() {
  return (
    <Fragment>
      {/* Hero Section */}
      <section className="hero-section">
        <img
          src="/images/Home_-_father_son.webp" // Replace with your image path
          alt="Hero"
          className="hero-image"
        />
        <h1 className="hero-title">Our Tax Services</h1>
      </section>

      {/* Service List Section */}
      <section className="resource-list">
        {services.map((service, index) => (
          <div key={index} className="resource-item">
            <div className="resource-text">
              <h2 className="resource-title">{service.title}</h2>
              <p className="resource-description">{service.description}</p>
            </div>
            <button
              onClick={() => window.location.href = service.link}
              className="get-started-button"
            >
              Get Started
            </button>
          </div>
        ))}
      </section>
    </Fragment>
  );
}
