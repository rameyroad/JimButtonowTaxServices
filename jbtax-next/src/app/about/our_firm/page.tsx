import { Fragment } from "react";
import "../../../styles/about.css";


export default function AboutOurFirm() {
    return (
        <Fragment>
            <section className="about-our-firm">
                <h2>About Our Firm</h2>
                <div className="about-content">
                    {/* Main Logo */}
                    <img src="/images/JL Buttonow CPA Difference logo.png" alt="Company Logo" className="main-logo" />

                    <div className="about-details">
                        {/* Title 1 */}
                        <div className="about-part">
                            <img src="/images/JL Buttonow CPA logo 3.png" alt="Small Logo" className="small-logo" />
                            <div className="part-text">
                                <h3>Title 1</h3>
                                <p>Small description for title 1 goes here. This should explain the first part of your firm's overview.</p>
                            </div>
                        </div>
                        
                        {/* Title 2 */}
                        <div className="about-part">
                            <img src="/images/JL Buttonow CPA logo 3.png" alt="Small Logo" className="small-logo" />
                            <div className="part-text">
                                <h3>Title 2</h3>
                                <p>Small description for title 2 goes here.</p>
                            </div>
                        </div>
                        
                        {/* Repeat similar structure for Title 3-6 */}
                    </div>
                </div>
            </section>
        </Fragment>
    );
}
