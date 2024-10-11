import type { Metadata } from "next";

// import localFont from "next/font/local";

import "bootstrap/dist/css/bootstrap.min.css";
import "../styles/styles2.css";
// import "../styles/media-updates.css";

import { ReduxProvider } from "@/store/ReduxProvider";
import { MainLayout } from "@/views/mainLayout";

// const geistSans = localFont({
//     src: "../fonts/GeistVF.woff",
//     variable: "--font-geist-sans",
//     weight: "100 900",
// });
// const geistMono = localFont({
//     src: "../fonts/GeistMonoVF.woff",
//     variable: "--font-geist-mono",
//     weight: "100 900",
// });

export const metadata: Metadata = {
    title: "Jim Buttonow Tax Services",
    description: "Find answers for your tax questions",
};

export default function RootLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="en">
            <head>
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>Jim Buttonow Tax Services</title>
                <link rel="stylesheet" href="https://rameycms-api-jbtax-eus-01.azurewebsites.net/api/content/sites/jbtax-services/site_styles.css" />
            </head>
            <body>
                <ReduxProvider>
                    <MainLayout>{children}</MainLayout>
                </ReduxProvider>
            </body>
        </html>
    );
}
