import { Fragment } from "react";
import "../../../styles/about.css";

export default function MeetOurProfessionals() {
    return (
        <Fragment>
            <section className="meet-our-professionals">
                <h2>Meet Our Tax Professionals</h2>
                <div className="professionals-content">
                    {/* Professional 1 */}
                    <div className="professional">
                        <img src="/images/jimbuttonowphotoshot.jpg" alt="Professional 1" className="professional-photo" />
                        <div className="professional-details">
                            <h3>John Doe</h3>
                            <p>John is a senior tax advisor with 10+ years of experience in expat tax preparation.</p>
                        </div>
                    </div>
                    
                    {/* Professional 2 */}
                    <div className="professional">
                        <img src="/images/madisonwhitfieldphotoshot.jpg" alt="Professional 2" className="professional-photo" />
                        <div className="professional-details">
                            <h3>Jane Smith</h3>
                            <p>Jane is a certified tax expert specializing in international tax law and compliance.</p>
                        </div>
                    </div>
                </div>
            </section>
        </Fragment>
    );
}
