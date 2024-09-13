export const Footer = () => {
    return (
        <footer>
            {/* <!-- Blue Bar with Partner Images --> */}
            <div className="footer-top">
                <div className="partner-section">
                    <span>Featured In:</span>
                    <div className="partner-images">
                        <img src="https://placehold.co/64x64" alt="Partner 1" />
                        <img src="https://placehold.co/64x64" alt="Partner 2" />
                        <img src="https://placehold.co/64x64" alt="Partner 3" />
                        {/* <!-- Add more partner images as needed --> */}
                        <img src="https://placehold.co/64x64" alt="Blank Image" />
                    </div>
                </div>
            </div>
            {/* <!-- Footer Content --> */}
            <div className="footer-content">
                {/* <!-- Left Side: Logo, Name, Social Media Links --> */}
                <div className="footer-left">
                    <img src="https://placehold.co/128x64" alt="Company Logo" className="footer-logo" />
                    <h2>Company Name</h2>
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
                                <a href="#">Link 1</a>
                            </li>
                            <li>
                                <a href="#">Link 2</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                    <div className="footer-column">
                        <h3>Our Firm</h3>
                        <ul>
                            <li>
                                <a href="#">About Us</a>
                            </li>
                            <li>
                                <a href="#">Careers</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                    <div className="footer-column">
                        <h3>SERVICES</h3>
                        <ul>
                            <li>
                                <a href="#">Service 1</a>
                            </li>
                            <li>
                                <a href="#">Service 2</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                    <div className="footer-column">
                        <h3>SUPPORT</h3>
                        <ul>
                            <li>
                                <a href="#">Contact Us</a>
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
                                <a href="#">Page 1</a>
                            </li>
                            <li>
                                <a href="#">Page 2</a>
                            </li>
                            {/* <!-- Add more links as needed --> */}
                        </ul>
                    </div>
                </div>
                {/* <!-- Bottom: Phone Number and Location --> */}
                <div className="footer-bottom">
                    <p>Phone: (123) 456-7890</p>
                    <p>Location: XYZ</p>
                </div>
            </div>
        </footer>
    );
};
