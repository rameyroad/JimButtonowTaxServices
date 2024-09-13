import { Fragment } from "react";

import * as BootStrap from "react-bootstrap";

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
                        <img src="/images/Home_-_father_son.webp" alt="Target 1" width={200} height={100} />
                        <h2>Target 1</h2>
                    </div>
                    <div className="target-item">
                        <img src="/images/Home_-_backpacking_couple.webp" alt="Target 2" width={200} height={100} />
                        <h2>Target 2</h2>
                    </div>
                    <div className="target-item">
                        <img src="/images/Home_-_woman_eiffel-3.webp" alt="Target 3" width={200} height={100} />
                        <h2>Target 3</h2>
                    </div>
                    <div className="target-item">
                        <img src="https://placehold.co/300x200" alt="Target 4" width={300} height={200} />
                        <h2>Target 4</h2>
                    </div>
                    <div className="target-item">
                        <img src="https://placehold.co/300x200" alt="Target 5" width={300} height={200} />
                        <h2>Target 5</h2>
                    </div>
                    <div className="target-item">
                        <img src="https://placehold.co/300x200" alt="Target 6" width={300} height={200} />
                        <h2>Target 6</h2>
                    </div>
                </div>
            </section>
        </Fragment>
    );
}
