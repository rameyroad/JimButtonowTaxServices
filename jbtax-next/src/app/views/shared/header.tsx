import { Fragment } from "react";

export const Header = () => {
    return (
        <Fragment>
            {/* <header className="fixed-header">
                <div className="logo">
                    <a href="/" style={{ textDecoration: "none" }}>
                        <h1>Jim Buttonow's Tax Services</h1>
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
                        <div className="dropdown">
                            <a href="#" className="dropdown-toggle">
                                Resources
                            </a>
                            <div className="dropdown-menu">
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
                                                        {" "}
                                                        Access up-to-date articles, breaking news, deadline information and in-depth case studies on US expat
                                                        taxes.
                                                    </div>
                                                </div>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <script src="dropscript.js"></script>

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
            </header> */}
            {/* <nav className="navbar navbar-expand-lg navbar-light bg-light">
                <button
                    className="navbar-toggler"
                    type="button"
                    data-toggle="collapse"
                    data-target="#navbarNavDropdown"
                    aria-controls="navbarNavDropdown"
                    aria-expanded="false"
                    aria-label="Toggle navigation"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse" id="navbarNavDropdown">
                    <ul className="navbar-nav">
                        <li className="navbar-brand">
                            <a href="/" style={{ textDecoration: "none" }}>
                                <h1>Jim Buttonow's Tax Services</h1>
                            </a>
                        </li>
                        <li className="nav-item active">
                            <a href="/whowehelp">Who We Help</a>
                        </li>
                        <li className="nav-item">
                            <a className="nav-link" href="#">
                                Features
                            </a>
                        </li>
                        <li className="nav-item">
                            <a className="nav-link" href="#">
                                Pricing
                            </a>
                        </li>
                        <li className="nav-item dropdown">
                            <a
                                className="nav-link dropdown-toggle"
                                href="#"
                                id="navbarDropdownMenuLink"
                                data-toggle="dropdown"
                                aria-haspopup="true"
                                aria-expanded="false"
                            >
                                Dropdown link
                            </a>
                            <div className="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">
                                <a className="dropdown-item" href="#">
                                    Action
                                </a>
                                <a className="dropdown-item" href="#">
                                    Another action
                                </a>
                                <a className="dropdown-item" href="#">
                                    Something else here
                                </a>
                            </div>
                        </li>
                    </ul>
                </div>
            </nav> */}
            <nav className="navbar navbar-expand-lg navbar-light bg-light justify-content-between">
                <button
                    className="navbar-toggler"
                    type="button"
                    data-toggle="collapse"
                    data-target="#navbarNavDropdown"
                    aria-controls="navbarNavDropdown"
                    aria-expanded="false"
                    aria-label="Toggle navigation"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse" id="navbarNavDropdown">
                    <ul className="navbar-nav">
                        <li className="navbar-brand">
                            <a href="/" style={{ textDecoration: "none" }}>
                                <h1>Jim Buttonow's Tax Services</h1>
                            </a>
                        </li>
                        <li className="nav-item active">
                            <a href="/whowehelp">Who We Help</a>
                        </li>
                        <li className="nav-item active">
                            <a href="/#">Resources</a>
                        </li>
                        <li className="nav-item active">
                            <a href="/#">About</a>
                        </li>
                    </ul>
                </div>
                <div className="">
                    <div className="contact-info collapse navbar-collapse">
                        <span className="phone-number">123-456-7890</span>
                        <span className="email">email@example.com</span>
                    </div>
                    <a href="/get-started" className="cta-button">
                        Get Started
                    </a>
                </div>
            </nav>
        </Fragment>
    );
};
