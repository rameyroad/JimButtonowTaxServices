import { Fragment } from "react";

// import "../styles/get-started.css";

export default function Page() {
    return (
        <Fragment>
            <section className="get-started">
                <div className="container">
                    <h2>Get Started</h2>
                    <p>Welcome to Jim Buttonow Tax Services! To get started with our services, please follow the steps below:</p>
                    <ol>
                        <li>
                            Contact us through our <a href="contact.html">Contact page</a> to schedule a consultation.
                        </li>
                        <li>Provide us with your tax documents and information.</li>
                        <li>Meet with our tax experts to discuss your needs and get tailored advice.</li>
                        <li>Receive a comprehensive plan and begin benefiting from our tax services.</li>
                    </ol>
                    <p>We look forward to assisting you with all your tax preparation and planning needs.</p>
                    <br />
                    <a href="contact.html" className="cta-button">
                        Get in Touch
                    </a>
                </div>
            </section>
        </Fragment>
    );
}
