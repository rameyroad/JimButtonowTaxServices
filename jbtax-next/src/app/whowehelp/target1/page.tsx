import { Fragment } from "react";

export default function Page() {
    return (
        <Fragment>
            {/* Who We Help Hero Section */}
            <section className="wwh-hero">
                <img src="/images/peru-scaled.jpeg" alt="Hero Background" className="wwh-hero-img" />
                <div className="wwh-hero-content">
                    <h1>Target1</h1>
                </div>
            </section>

            {/* Target Images Section */}
            <section className="target-images">
                <div className="target-container">
                    <div className="target-item">
                        <a href="#">
                            <img src="/images/Home_-_father_son.webp" alt="Target 1" width={200} height={100} />
                            <h2>Self Employeed</h2>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="/images/Home_-_backpacking_couple.webp" alt="Target 2" width={200} height={100} />
                            <h2>Some tax stuff</h2>
                        </a>
                    </div>
                    <div className="target-item">
                        <a href="#">
                            <img src="/images/Home_-_woman_eiffel-3.webp" alt="Target 3" width={200} height={100} />
                            <h2>More tax crap</h2>
                        </a>
                    </div>
                </div>
            </section>
        </Fragment>
    );
}
