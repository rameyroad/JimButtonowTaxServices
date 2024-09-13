'use client'; // This directive marks the component as a client-side component

import React from 'react';
import '../../styles/header.css'; // Import the CSS file

export const Header = () => {
  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text)
      .then(() => {
        alert(`Copied to clipboard: ${text}`);
      })
      .catch(err => {
        console.error('Failed to copy: ', err);
      });
  };

  return (
    <header className="fixed-header">
      <nav className="navbar navbar-expand-lg">
        <div className="container-fluid">
          <a className="navbar-brand d-flex align-items-center" href="/">
            <img 
              src="/images/JL Buttonow banner logo.png" 
              alt="JL Buttonow CPA PLLC Logo" 
            />
          </a>

          <button 
            className="navbar-toggler" 
            type="button" 
            data-bs-toggle="collapse" 
            data-bs-target="#navbarSupportedContent" 
            aria-controls="navbarSupportedContent" 
            aria-expanded="false" 
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse" id="navbarSupportedContent">
            <ul className="navbar-nav me-auto mb-2 mb-lg-0">
              <li className="nav-item">
                <a className="nav-link" href="/whowehelp">Who We Help</a>
              </li>
              <li className="nav-item dropdown">
                <a 
                  className="nav-link dropdown-toggle" 
                  href="#" 
                  role="button" 
                  data-bs-toggle="dropdown" 
                  aria-expanded="false"
                >
                  Services
                </a>
                <ul className="dropdown-menu">
                  <li><a className="dropdown-item" href="#">Service 1</a></li>
                  <li><a className="dropdown-item" href="#">Service 2</a></li>
                </ul>
              </li>
              <li className="nav-item dropdown">
                <a 
                  className="nav-link dropdown-toggle" 
                  href="#" 
                  role="button" 
                  data-bs-toggle="dropdown" 
                  aria-expanded="false"
                >
                  Resources
                </a>
                <ul className="dropdown-menu">
                  <li><a className="dropdown-item" href="#">Blog</a></li>
                  <li><a className="dropdown-item" href="#">Guides</a></li>
                </ul>
              </li>
              <li className="nav-item">
                <a className="nav-link" href="/contact">About</a>
              </li>
            </ul>
            <div className="contact-info d-flex align-items-center" style={{ marginLeft: 'auto' }}>
              <img 
                src="/images/phone_icon.png" 
                alt="Copy Phone Number" 
                onClick={() => copyToClipboard('123-456-7890')} 
              />
              <img 
                src="/images/email_icon.png" 
                alt="Copy Email" 
                onClick={() => copyToClipboard('email@example.com')} 
              />
              <a href="/get-started" className="get-started-button">Get Started</a>
            </div>
          </div>
        </div>
      </nav>
    </header>
  );
};
