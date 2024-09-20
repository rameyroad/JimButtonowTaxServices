"use client";

import { Fragment } from "react";
import "../../../styles/resources.css";

// Dummy data for resource items
const resources = [
  {
    title: "Tax Problem Guides",
    description: "Learn how to resolve common tax issues.",
    link: "/tax-problem-guides",
  },
  {
    title: "Blog",
    description: "Read our latest insights and articles.",
    link: "/blog",
  },
  {
    title: "Frequently Asked Questions",
    description: "Find answers to common questions.",
    link: "/faq",
  },
  {
    title: "Our Published Articles",
    description: "Explore articles we’ve published on tax matters.",
    link: "/articles",
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
        <h1 className="hero-title">Resources and Guides</h1>
      </section>

      {/* Resource List Section */}
      <section className="resource-list">
        {resources.map((resource, index) => (
          <div key={index} className="resource-item">
            <div className="resource-text">
              <h2 className="resource-title">{resource.title}</h2>
              <p className="resource-description">{resource.description}</p>
            </div>
            <button
              onClick={() => window.location.href = resource.link}
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
