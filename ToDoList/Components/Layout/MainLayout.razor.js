/**
 * A script to remove cookies cause fuck blazor signal R shit
 * @param {any} url
 */
export async function logoutApi(url) {
    const res = await fetch(url, {
        method: "POST",
        credentials: "include"
    });

    if (!res.ok)
        throw new Error("Logout failed");
}