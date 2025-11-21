
/**
 * Script to make an api call to the login api and use register it as cookie cause fuck blazor
 * @param {any} url
 * @param {any} data
 * @returns
 */
export async function loginApi(url, data) {
    const res = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(data)
    });

    if (!res.ok) {
        throw new Error('Login failed');
    }

    return res;
}