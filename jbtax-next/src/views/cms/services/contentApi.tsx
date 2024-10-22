export const getSiteData = async () => {
    const myHeaders = new Headers({
        Authorization: `Basic ${process.env.NEXT_PUBLIC_RAMEY_API_AUTH}`,
    });

    const url = `${process.env.NEXT_PUBLIC_RAMEY_API_URL}/api/content/sites/${process.env.NEXT_PUBLIC_RAMEY_API_SITE_ID}`;
    const resp = await fetch(url, {
        headers: myHeaders,
        next: { revalidate: 300 },
    });
    if (resp.ok) {
        const m = await resp.json();
        return m;
    }
    return null;
};

export const getSiteMap = async (siteId: string) => {
    const myHeaders = new Headers({
        Authorization: `Basic ${process.env.NEXT_PUBLIC_RAMEY_API_AUTH}`,
    });
    const url = `${process.env.NEXT_PUBLIC_RAMEY_API_URL}/api/content/site/map?siteId=${siteId}`;

    const resp = await fetch(url, {
        headers: myHeaders,
        next: { revalidate: 300 },
    });
    if (resp.ok) {
        const m = await resp.json();
        return m;
    }
    return null;
};

export const getPageBySlug = async (permaLink: string, site: string) => {
    if (permaLink == null) return null;

    const myHeaders = new Headers({
        Authorization: `Basic ${process.env.NEXT_PUBLIC_RAMEY_API_AUTH}`,
    });
    const url = `${process.env.NEXT_PUBLIC_RAMEY_API_URL}/api/content/pages/${permaLink}?siteId=${site}`;
    const resp = await fetch(url, {
        headers: myHeaders,
        next: { revalidate: 300 },
    });
    if (resp.ok) {
        try {
            const m = await resp.json();
            return m;
        } catch (err) {
            console.log("error", err);
        }
    }
    return null;
};

export const getAllBlogPosts = async () => {
    const myHeaders = new Headers({
        Authorization: `Basic ${process.env.NEXT_PUBLIC_RAMEY_API_AUTH}`,
    });
    const url = `${process.env.NEXT_PUBLIC_RAMEY_API_URL}/api/content/blog`;
    const resp = await fetch(url, {
        headers: myHeaders,
        next: { revalidate: 300 },
    });
    if (resp.ok) {
        try {
            const m = await resp.json();
            return m;
        } catch (err) {
            console.log("error", err);
        }
    }
    return null;
};

export const getBlogPostBySlug = async (slug: string) => {
    const myHeaders = new Headers({
        Authorization: `Basic ${process.env.NEXT_PUBLIC_RAMEY_API_AUTH}`,
    });
    const url = `${process.env.NEXT_PUBLIC_RAMEY_API_URL}/api/content/blog/${slug}`;
    const resp = await fetch(url, {
        headers: myHeaders,
        next: { revalidate: 300 },
    });
    if (resp.ok) {
        try {
            const m = await resp.json();
            return m;
        } catch (err) {
            console.log("error", err);
        }
    }
    return null;
};

export const getBlogPostsByTag = async (tags: Array<string>) => {
    const myHeaders = new Headers({
        Authorization: `Basic ${process.env.NEXT_PUBLIC_RAMEY_API_AUTH}`,
    });
    const url = `${process.env.NEXT_PUBLIC_RAMEY_API_URL}/api/content/posts/tags/${tags.join(",")}`;
    const resp = await fetch(url, {
        headers: myHeaders,
        next: { revalidate: 300 },
    });
    if (resp.ok) {
        try {
            const m = await resp.json();
            return m;
        } catch (err) {
            console.log("error", err);
        }
    }
    return null;
};
