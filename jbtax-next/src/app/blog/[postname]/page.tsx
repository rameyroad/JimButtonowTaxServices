import { Fragment } from "react";

const DisplayPost = ({ postname }: { postname: string }) => {
    return (
        <Fragment>
            <h1>{postname}</h1>
            <p>Post content</p>
        </Fragment>
    );
};

export default function Page({ params }: { params: { postname: string } }) {
    return (
        <Fragment>
            <div className="container">
                <DisplayPost postname={params.postname} />
            </div>
        </Fragment>
    );
}
