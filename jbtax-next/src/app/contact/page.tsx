import { Fragment } from "react";

export default function Page() {
    return (
        <Fragment>
            {/* <!-- Main Content --> */}
            <main>
                {/* <!-- Contact Section --> */}
                <section className="contact">
                    <div className="container">
                        <h2>Contact Us</h2>
                        <p>We would love to hear from you! Please reach out with any questions or comments.</p>
                        <form action="submit_form.php" method="post">
                            <label htmlFor="name">Name:</label>
                            <input type="text" id="name" name="name" required />

                            <label htmlFor="email">Email:</label>
                            <input type="email" id="email" name="email" required />

                            <label htmlFor="message">Message:</label>
                            <textarea id="message" name="message" rows={4} required></textarea>

                            <button type="submit" className="cta-button">
                                Send Message
                            </button>
                        </form>
                    </div>
                </section>
            </main>
        </Fragment>
    );
}
