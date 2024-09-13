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

    var name = title.replace(" ", "_").toLowerCase();

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
                                        <li className="nav-item">
                                            <a href="/#" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt=""
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">Service 1</div>
                                                    <div className="nav-link--description">Service 1 description</div>
                                                </div>
                                            </a>
                                        </li>
                                        <li className="nav-item">
                                            <a href="/#" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt=""
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">Service 2</div>
                                                    <div className="nav-link--description">Service 2 description</div>
                                                </div>
                                            </a>
                                        </li>
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
                                                    <div className="nav-link--description">Blog description</div>
                                                </div>
                                            </a>
                                        </li>
                                        <li className="nav-item">
                                            <a href="/#" className="nav-link">
                                                <div className="nav-link--img">
                                                    <img
                                                        width="40"
                                                        height="40"
                                                        src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                        alt=""
                                                    />
                                                </div>
                                                <div className="nav-link--content">
                                                    <div className="nav-link--title">Guides</div>
                                                    <div className="nav-link--description">Guides description</div>
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
