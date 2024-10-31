// lib/csrfUtils.ts
import { randomBytes, createHmac } from "crypto";

const CSRF_SECRET = process.env.CSRF_SECRET || "defaultSecret";

export function generateCsrfToken() {
    // Generate a random token and hash it with the CSRF secret
    const token = randomBytes(32).toString("hex");
    const hash = createHmac("sha256", CSRF_SECRET).update(token).digest("hex");
    return `${token}:${hash}`;
}

export function validateCsrfToken(token: string | null) {
    if (!token) return false;

    const [tokenValue, hash] = token.split(":");
    if (!tokenValue || !hash) return false;

    const expectedHash = createHmac("sha256", CSRF_SECRET).update(tokenValue).digest("hex");
    return hash === expectedHash;
}
