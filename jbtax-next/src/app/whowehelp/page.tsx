import { Fragment } from "react";

export default function Page() {
    return (
        <Fragment>
            {/* Who We Help Hero Section */}
            <section className="wwh-hero">
                <img src="/images/peru-scaled.jpeg" alt="Hero Background" className="wwh-hero-img" />
                <div className="wwh-hero-content">
                    <h1>Who We Help</h1>
                </div>
            </section>

            {/* Target Images Section */}
            <section className="target-images">
                <div className="target-container">
                    <div className="target-item">
                        <a href="whowehelp/IRS_audits">
                            <img src="/images/Home_-_father_son.webp" alt="Target 1" width={200} height={100} />
                            <h2>IRS Audits</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="/images/Home_-_backpacking_couple.webp" alt="Target 2" width={200} height={100} />
                            <h2>Tax Debt Issues</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="/images/Home_-_woman_eiffel-3.webp" alt="Target 3" width={200} height={100} />
                            <h2>Unfiled Back Tax Returns</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="https://placehold.co/300x200" alt="Target 4" width={300} height={200} />
                            <h2>IRS Penalties</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="https://placehold.co/300x200" alt="Target 5" width={300} height={200} />
                            <h2>Payroll Tax Issues and</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="https://placehold.co/300x200" alt="Target 6" width={300} height={200} />
                            <h2>Business Tax Issues</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="https://placehold.co/300x200" alt="Target 7" width={300} height={200} />
                            <h2>Dealing with the IRS</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="https://placehold.co/300x200" alt="Target 8" width={300} height={200} />
                            <h2>IRS Account Issues</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="https://placehold.co/300x200" alt="Target 9" width={300} height={200} />
                            <h2>Tax Firms</h2>
                            <p>Small text</p>
                        </a>
                    </div>
                </div>
            </section>
        </Fragment>
    );
}
