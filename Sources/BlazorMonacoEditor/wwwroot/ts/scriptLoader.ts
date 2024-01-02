// TypeScript ESM Module
type ScriptLoadStatus = 'loaded' | 'failed';

// Map to track loaded scripts
const loaded: Map<string, ScriptLoadStatus> = new Map();

/**
 * loadScript - Loads a JavaScript file dynamically and returns a promise that completes when the script loads.
 * @param scriptPath - The path to the JavaScript file to load.
 * @returns A promise that resolves with the script path when the script is loaded successfully.
 */
export function loadScript(scriptPath: string): Promise<string> {
    // Check if already loaded
    if (loaded.get(scriptPath)) {
        console.log(`${scriptPath} already loaded`);
        return Promise.resolve(scriptPath);
    }

    return new Promise<string>((resolve, reject) => {
        // Create JS library script element
        const script: HTMLScriptElement = document.createElement("script");
        script.src = scriptPath;
        script.type = "text/javascript";
        console.log(`${scriptPath} created`);

        // Flag as loading
        loaded.set(scriptPath, 'loaded');

        // Resolve or reject the promise based on script load success/failure
        script.onload = () => {
            console.log(`${scriptPath} loaded ok`);
            resolve(scriptPath);
        };
        script.onerror = () => {
            console.log(`${scriptPath} load failed`);
            loaded.set(scriptPath, 'failed');
            reject(new Error(`${scriptPath} failed to load`));
        };

        // Append script to the end of body
        document.body.appendChild(script);
    });
}
