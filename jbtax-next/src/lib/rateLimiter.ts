// lib/rateLimiter.ts
const rateLimits: Record<string, { count: number; firstRequestTime: number }> = {};
const RATE_LIMIT_WINDOW = 15 * 60 * 1000; // 15 minutes
const MAX_REQUESTS = 3; // Maximum requests allowed within the time window

export function rateLimit(ip: string): boolean {
    const currentTime = Date.now();

    if (!rateLimits[ip]) {
        // Initialize the record for a new IP
        rateLimits[ip] = { count: 1, firstRequestTime: currentTime };
        return true; // Allow request
    }

    const { count, firstRequestTime } = rateLimits[ip];

    // Check if the time window has expired
    if (currentTime - firstRequestTime > RATE_LIMIT_WINDOW) {
        // Reset count and time
        rateLimits[ip] = { count: 1, firstRequestTime: currentTime };
        return true; // Allow request
    }

    // Check if the limit has been reached
    if (count < MAX_REQUESTS) {
        rateLimits[ip].count++;
        return true; // Allow request
    }

    return false; // Reject request
}
