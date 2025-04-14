const loaded = new Map();

/**
 * Dynamically loads a JavaScript file and returns a promise that resolves when it finishes loading.
 * @param {string} scriptPath - The path to the JavaScript file to load.
 * @returns {Promise<string>} - Resolves with the script path when the script is loaded successfully.
 */
export function loadScript(scriptPath) {
    // If already loaded or attempted
    if (loaded.get(scriptPath)) {
        console.log(`${scriptPath} already loaded or attempted`);
        return Promise.resolve(scriptPath);
    }

    return new Promise((resolve, reject) => {
        const script = document.createElement("script");
        script.src = scriptPath;
        script.type = "text/javascript";

        console.log(`${scriptPath} created`);

        // Optimistically mark as loading
        loaded.set(scriptPath, 'loaded');

        script.onload = () => {
            console.log(`${scriptPath} loaded ok`);
            resolve(scriptPath);
        };

        script.onerror = () => {
            console.log(`${scriptPath} load failed`);
            loaded.set(scriptPath, 'failed');
            reject(new Error(`${scriptPath} failed to load`));
        };

        document.body.appendChild(script);
    });
}