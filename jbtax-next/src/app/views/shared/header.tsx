"use client";

import { Fragment, useEffect, useState } from "react";

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
    return (
        <Fragment>
            <header className="fixed-header">
                <div className="logo">
                    <a href="/" style={{ textDecoration: "none" }}>
                        <h1>Jim Buttonow&apos;s Tax Services</h1>
                    </a>
                </div>
                <nav>
                    <ul>
                        <li>
                            <a href="/">Home</a>
                        </li>
                        <li>
                            <a href="/whowehelp">Who We Help</a>
                        </li>
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
                                                    Access up-to-date articles, breaking news, deadline information and in-depth case studies on US expat taxes.
                                                </div>
                                            </div>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </DropDownItem>
                        <DropDownItem title="Drop Test">
                            <div className="dropdown-submenu-icons">
                                <ul className="dropdown-submenu-icons--nav">
                                    <li className="nav-item">
                                        <a href="#" className="nav-link">
                                            <div className="nav-link--img">
                                                <img
                                                    width="40"
                                                    height="40"
                                                    src="https://www.greenbacktaxservices.com/wp-content/uploads/2022/12/noun-knowledge-2176167-1.svg"
                                                    alt=""
                                                />
                                            </div>
                                            <div className="nav-link--content">
                                                <div className="nav-link--title">Test Link</div>
                                                <div className="nav-link--description">A second drop-down just as an example.</div>
                                            </div>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </DropDownItem>
                        <li>
                            <a href="/contact">About</a>
                        </li>
                    </ul>
                    <div className="contact-info">
                        <span className="phone-number">123-456-7890</span>
                        <span className="email">email@example.com</span>
                    </div>
                    <a href="/get-started" className="cta-button">
                        Get Started
                    </a>
                </nav>
            </header>
        </Fragment>
    );
};
