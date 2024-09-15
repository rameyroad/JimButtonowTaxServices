"use client"; // This directive marks the component as a client-side component

import React, { useEffect, useState } from "react";

import "../../styles/header.css"; // Import the CSS file

const DropDownItem = ({
    title,
    children,
}: Readonly<{
    title: string;
    children: React.ReactNode;
}>) => {
    const [open, setOpen] = useState(false);

    const toggleOpen = () => {
        setOpen(!open);
    };

    const name = title.replace(" ", "_").toLowerCase();

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            const target = event.target as HTMLElement;
            const dropdown = document.querySelector("#" + name);

            if (dropdown && !dropdown.contains(target)) {
                setOpen(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);

        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, []);

    return (
        <div className="dropdown" id={name}>
            <a href="#" className={`dropdown-toggle ${open ? "show" : ""}`} onClick={() => toggleOpen()}>
                {title}
            </a>
            <div className={`dropdown-menu ${open ? "show" : ""}`}>
                <div className="container">{children}</div>
            </div>
        </div>
    );
};

export const Header = () => {
    const copyToClipboard = (text: string) => {
        navigator.clipboard
            .writeText(text)
            .then(() => {
                alert(`Copied to clipboard: ${text}`);
            })
            .catch((err) => {
                console.error("Failed to copy: ", err);
            });
    };

    return (
        <header className="fixed-header">
            <nav className="navbar navbar-expand-lg">
                <div className="container-fluid">
                    <a className="navbar-brand d-flex align-items-center" href="/">
                        <img src="/images/JL Buttonow banner logo.png" alt="JL Buttonow CPA PLLC Logo" />
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
                                <a className="nav-link" href="/whowehelp">
                                    Who We Help
                                </a>
                            </li>
                            <DropDownItem title="Services">
                                <div className="dropdown-submenu-icons">
                                    <ul className="dropdown-submenu-icons--nav">
                                        {/* Main Service */}
                                        <li className="nav-item main-service">
                                            <a href="/#" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt="Main Service"
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">Services</div>
                                                    <div className="nav-link--description">
                                                        <p>Discover all the tax services we offer</p>
                                                    </div>
                                                </div>
                                            </a>
                                        </li>

                                        {/* Grid of 9 other services */}
                                        <div className="services-grid">
                                            <li className="nav-item">
                                                <a href="/service/1" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="IRS Tax Stuff"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">IRS Tax Stuff</div>
                                                    </div>
                                                </a>
                                            </li>
                                            <li className="nav-item">
                                                <a href="/service/2" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="Tax Professional Training"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">Tax Professional Training</div>
                                                    </div>
                                                </a>
                                            </li>
                                            <li className="nav-item">
                                                <a href="/service/3" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="Service 3"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">Service 3</div>
                                                    </div>
                                                </a>
                                            </li>
                                            <li className="nav-item">
                                                <a href="/service/3" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="Service 3"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">Service 4</div>
                                                    </div>
                                                </a>
                                            </li>
                                            <li className="nav-item">
                                                <a href="/service/3" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="Service 3"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">Service 5</div>
                                                    </div>
                                                </a>
                                            </li>
                                            <li className="nav-item">
                                                <a href="/service/3" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="Service 3"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">Service 6</div>
                                                    </div>
                                                </a>
                                            </li>
                                            <li className="nav-item">
                                                <a href="/service/3" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="Service 3"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">Service 7</div>
                                                    </div>
                                                </a>
                                            </li>
                                            <li className="nav-item">
                                                <a href="/service/3" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="Service 3"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">Service 8</div>
                                                    </div>
                                                </a>
                                            </li>
                                            <li className="nav-item">
                                                <a href="/service/3" className="nav-link">
                                                    <div className="nav-link--img">
                                                        <img
                                                            width="40"
                                                            height="40"
                                                            src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                            alt="Service 3"
                                                        />
                                                    </div>
                                                    <div className="nav-link--content">
                                                        <div className="nav-link--title">Service 9</div>
                                                    </div>
                                                </a>
                                            </li>
                                            {/* Repeat similar blocks for the remaining services */}
                                        </div>
                                    </ul>
                                </div>
                            </DropDownItem>

                            <DropDownItem title="Resources">
                                <div className="dropdown-submenu-icons">
                                    <ul className="dropdown-submenu-icons--nav">
                                        <li className="nav-item">
                                            <a href="/blog" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt=""
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">Blog</div>
                                                    <div className="nav-link--description">
                                                        <h3>Blog description</h3>
                                                    </div>
                                                </div>
                                            </a>
                                        </li>

                                        <li className="nav-item">
                                            <a href="/knowledge_center" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt=""
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">Knowledge Center</div>
                                                    <div className="nav-link--description">
                                                        <h3>Blog description</h3>
                                                    </div>
                                                </div>
                                            </a>
                                        </li>

                                        <li className="nav-item">
                                            <a href="/articles" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt=""
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">Articles</div>
                                                    <div className="nav-link--description">
                                                        <h3>Articles description</h3>
                                                    </div>
                                                </div>
                                            </a>
                                        </li>

                                        <li className="nav-item">
                                            <a href="/tax_problem_guides" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt=""
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">Tax Problem Guides</div>
                                                    <div className="nav-link--description">
                                                        <h3>Guide description</h3>
                                                    </div>
                                                </div>
                                            </a>
                                        </li>

                                        <li className="nav-item">
                                            <a href="/faqs" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt=""
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">FAQS</div>
                                                    <div className="nav-link--description">
                                                        <h3>FAQS descriptiom</h3>
                                                    </div>
                                                </div>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </DropDownItem>
                            <li className="nav-item">
                                <a className="nav-link" href="/contact">
                                    About
                                </a>
                            </li>
                        </ul>
                        <div className="contact-info d-flex align-items-center" style={{ marginLeft: "auto" }}>
                            <img src="/images/phone_icon.png" alt="Copy Phone Number" onClick={() => copyToClipboard("123-456-7890")} />
                            <img src="/images/email_icon.png" alt="Copy Email" onClick={() => copyToClipboard("email@example.com")} />
                            <a href="/get-started" className="get-started-button">
                                Get Started
                            </a>
                        </div>
                    </div>
                </div>
            </nav>
        </header>
    );
};
