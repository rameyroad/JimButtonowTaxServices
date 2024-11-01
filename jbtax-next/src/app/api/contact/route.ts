// app/api/contact/route.ts
import { NextRequest, NextResponse } from "next/server";
import { cookies } from "next/headers";

import { generateCsrfToken, validateCsrfToken } from "../../../lib/csrfUtils";
import { rateLimit } from "../../../lib/rateLimiter";

export function GET() {
    // Generate a CSRF token
    const csrfToken = generateCsrfToken();

    // Set CSRF token in a cookie for client-side access
    return NextResponse.json(
        { csrfToken },
        {
            headers: { "Set-Cookie": `csrfToken=${csrfToken}; HttpOnly; Path=/;` },
        }
    );
}

export async function POST(req: NextRequest) {
    const ip = req.headers.get("x-real-ip") || req.ip || "unknown";

    // Rate limit check
    if (!rateLimit(ip)) {
        return NextResponse.json({ message: "Too many requests, please try again later." }, { status: 429 });
    }

    // Validate the incoming CSRF token
    const csrfTokenFromClient = req.headers.get("x-csrf-token");
    const csrfTokenFromCookie = cookies().get("csrfToken")?.value;

    // Validate the CSRF token from the client against the cookie value
    if (!csrfTokenFromClient || !validateCsrfToken(csrfTokenFromCookie as any) || csrfTokenFromClient !== csrfTokenFromCookie) {
        return NextResponse.json({ message: "Invalid CSRF token" }, { status: 403 });
    }

    // Handle form data
    const data = await req.json();

    // Process form data here (e.g., data.name, data.message, etc.)
    const jsonBody = createEmailBody(data);
    console.log("process.env.EMAIL_API_KEY", process.env.EMAIL_API_KEY);

    // Send email using an API post to https://api.postmarkapp.com/email
    const response = await fetch("https://api.postmarkapp.com/email", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "X-Postmark-Server-Token": process.env.EMAIL_API_KEY || "",
        },
        body: JSON.stringify(jsonBody),
    });

    if (!response.ok) {
        const errorData = await response.json();
        console.error("Failed to send email:", errorData);
    }

    return NextResponse.json({ message: "Question submitted successfully" });
}

const createEmailBody = (data: any) => {
    let htmlBody = `A new webform has been submitted with the following details: <br/><br/>`;
    htmlBody += `First Name: ${data.firstName}<br/>`;
    htmlBody += `Last Name: ${data.lastName}<br/>`;
    htmlBody += `Email: ${data.email}<br/>`;
    htmlBody += `Issue: ${data.issue}<br/>`;
    htmlBody += `Question: ${data.questionText}<br/>`;

    const emailJson = {
        From: "no-reply@rameycms.com",
        To: "jason@rameyroad.com",
        Bcc: "jason@rameyroad.com",
        Subject: "A new question has been submitted",
        Tag: "Email webform",
        HtmlBody: htmlBody,
        ReplyTo: data.email,
        MessageStream: "outbound",
    };
    return emailJson;
};
