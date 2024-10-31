// lib/csrf.ts
import { nextCsrf } from "next-csrf";

export const { csrf } = nextCsrf({
    secret: process.env.CSRF_SECRET || "defaultSecret", // Replace with a secure, random secret
});
