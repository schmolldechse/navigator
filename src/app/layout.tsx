import type {Metadata} from "next";
import "./globals.css";

export const metadata: Metadata = {
    title: "Navigator",
    description: "Real-time train tracking and schedule updates",
};

export default async function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
    return (
        <html lang="en">
        <body
            className={`montserrat-regular`}
        >
        {children}
        </body>
        </html>
    );
}
