import { Fragment } from "react";

import { Footer } from "./shared/footer";
import { Header } from "./shared/header";

export const MainLayout = ({ children }: { children: React.ReactNode }) => {
    return (
        <Fragment>
            <Header />
            <main>{children}</main>
            <Footer />
        </Fragment>
    );
};
