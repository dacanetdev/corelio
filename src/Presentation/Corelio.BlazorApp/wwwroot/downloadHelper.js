/**
 * Triggers a browser file download from a base64-encoded byte array.
 * Called via Blazor JS interop (IJSRuntime.InvokeVoidAsync).
 * @param {string} base64 - Base64-encoded file content
 * @param {string} filename - Suggested download filename
 * @param {string} mimeType - MIME type (e.g., "application/pdf")
 */
window.downloadFile = (base64, filename, mimeType) => {
    const bytes = Uint8Array.from(atob(base64), c => c.charCodeAt(0));
    const blob = new Blob([bytes], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};
