import { Fragment } from "react";

const Hero = () => {
    return (
        <section className="hero">
            <div className="hero-content">
                <div className="header-images">
                    <img src="css/images/Home_-_father_son.webp" alt="Tax Services Image 1" className="active" />
                    <img src="css/images/Home_-_backpacking_couple.webp" alt="Tax Services Image 2" />
                    <img src="css/images/Home_-_woman_eiffel-3.webp" alt="Tax Services Image 3" />
                </div>
                <div className="text-content">
                    <h1>US Expat tax services and preparation done right.</h1>
                    <p>We take the hassle out of filing your American Expat taxes so you can get back to your adventure abroad.</p>
                </div>
            </div>
        </section>
    );
};

const BusinessStats = () => {
    return (
        // <!-- Business Stats Section -->
        <section className="business-stats">
            <div className="stats-item">
                <h3>10+ Years</h3>
                <p>Experience in Expat Tax Services</p>
            </div>
            <div className="stats-item">
                <h3>500+</h3>
                <p>Clients Served Globally</p>
            </div>
            <div className="stats-item">
                <h3>99%</h3>
                <p>Client Satisfaction Rate</p>
            </div>
        </section>
    );
};

const HomeContent = () => {
    return (
        // <!-- Main Content and Sidebar Section -->
        <div className="content-sidebar-wrapper">
            <main>
                <div className="main-content">
                    <div className="container-form-home active" style={{ minHeight: "736px" }}>
                        <h2>Start Your US Expat Tax Return</h2>
                        <form method="post" encType="multipart/form-data" noValidate>
                            <div className="gform-body">
                                <ul className="gform_fields">
                                    <li className="gfield">
                                        <label htmlFor="first-name">First Name *</label>
                                        <input type="text" id="first-name" name="first-name" required />
                                    </li>
                                    <li className="gfield">
                                        <label htmlFor="last-name">Last Name *</label>
                                        <input type="text" id="last-name" name="last-name" required />
                                    </li>
                                    <li className="gfield">
                                        <label htmlFor="email">Email Address *</label>
                                        <input type="email" id="email" name="email" required />
                                    </li>
                                    <li className="gfield">
                                        <label htmlFor="country">Country You Currently Live In *</label>
                                        <select id="country" name="country" required>
                                            <option value="">--Select Country--</option>
                                            {/* <!-- Add more countries as options --> */}
                                        </select>
                                    </li>
                                    <li className="gfield">
                                        <label htmlFor="comments">Questions or Comments</label>
                                        <textarea id="comments" name="comments" rows={4}></textarea>
                                    </li>
                                </ul>
                                <div className="gform_footer">
                                    <input type="submit" className="gform_button" value="Get Started Now" />
                                </div>
                            </div>
                        </form>
                    </div>
                </div>

               
            </main>

            <aside className="sidebar">
                {/* <!-- Add sidebar content here --> */}
                <h1>Bigger Title</h1>
                    <ul>
                        <li>
                            <h3>Title</h3>
                            <p>subtext</p>
                        </li>
                        <li>
                            <h3>Title</h3>
                            <p>subtext</p>
                        </li>
                        <li>
                            <h3>Title</h3>
                            <p>subtext</p>
                        </li>
                        <li>
                            <h3>Title</h3>
                            <p>subtext</p>
                        </li>
                    </ul>
            </aside>
        </div>
    );
};

export default function Home() {
    return (
        <Fragment>
            <Hero />
            <BusinessStats />
            <HomeContent />
        </Fragment>
    );
}
