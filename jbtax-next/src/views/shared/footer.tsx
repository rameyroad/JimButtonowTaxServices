export const Footer = () => {
    return (
        <footer>
            {/* <!-- Blue Bar with Partner Images --> */}
            <div className="footer-top">
                <div className="partner-section">
                    <span>Featured In:</span>
                    <div className="partner-images">
                        <img src="/images/accounttoday.png" alt="Partner 1" />
                        <img src="/images/taxhandbook.png" alt="Partner 2" />
                        <img src="/images/wolterskluwerlogo.jpg" alt="Partner 3" />
                        <img src="/images/JacksonHewitt.png" alt="Blank Image" />
                    </div>
                </div>
            </div>
            {/* <!-- Footer Content --> */}
            <div className="footer-content">
                {/* <!-- Left Side: Logo, Name, Social Media Links --> */}
                <div className="footer-left">
                    <img src="/images/JL Buttonow banner logo.png" alt="Company Logo" className="footer-logo" />
                    <h4>We Resolve Problems</h4>
                    <div className="social-media">
                        <a href="#">Facebook</a>
                        <a href="#">Twitter</a>
                        <a href="#">LinkedIn</a>
                        {/* <!-- Add more social media links as needed --> */}
                    </div>
                </div>
                {/* <!-- Middle Columns --> */}
                <div className="footer-middle d-sm-none d-md-flex ">
                    <div className="footer-column">
                        <h3>Helpful Links</h3>
                        <ul>
                            <li>
                                <a href="#">Tax Problem Guides</a>
                            </li>
                            <li>
                                <a href="#">Articles</a>
                            </li>
                            <li>
                                <a href="#">Blog</a>
                            </li>
                            <li>
                                <a href="/about/partners">Partners</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                    <div className="footer-column">
                        <h3>Our Firm</h3>
                        <ul>
                            <li>
                                <a href="/about/our_firm">About Us</a>
                            </li>
                            <li>
                                <a href="/about/meet_tax_professionals">Our Team</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                    <div className="footer-column">
                        <h3>SERVICES</h3>
                        <ul>
                            <li>
                                <a href="/services/services_main">Our Services</a>
                            </li>
                            <li>
                                <a href="#">Audit Representation</a>
                            </li>
                            <li>
                                <a href="#">Tax Debt Services</a>
                            </li>
                            <li>
                                <a href="#">Tax Firm Consultations</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                    <div className="footer-column">
                        <h3>SUPPORT</h3>
                        <ul>
                            <li>
                                <a href="/contact">Contact Us</a>
                            </li>
                            <li>
                                <a href="#">FAQs</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                    <div className="footer-column">
                        <h3>POPULAR PAGES</h3>
                        <ul>
                            <li>
                                <a href="#">TBD</a>
                            </li>
                            <li>
                                <a href="#">TBD</a>
                            </li>
                            <li>
                                <a href="#">TBD</a>
                            </li>
                            <li>
                                <a href="#">TBD</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                </div>
                {/* <!-- Bottom: Phone Number and Location --> */}
                <div className="footer-bottom">
                    <p>Email: jim@buttonowcpa.com</p>
                    <p>Phone: (336) 256-8266 </p>
                    <p>User Agreement: <a href="#">TBD</a> </p>
                    <p>Privacy Policy: <a href="#">TBD</a> </p>
                </div>
            </div>
        </footer>
    );
};
