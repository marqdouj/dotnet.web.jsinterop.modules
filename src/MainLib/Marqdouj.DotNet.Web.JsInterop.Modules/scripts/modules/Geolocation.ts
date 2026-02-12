import { Logger, LogLevel } from "./Logger"

export class Geolocation {
    static #watchIDs: Map<string, number> = new Map<string, number>;
    static readonly #logger: Logger = new Logger();

    static setLogLevel(level: LogLevel) {
        Geolocation.#logger.setLogLevel(level);
    }

    //https://developer.mozilla.org/en-US/docs/Web/API/Geolocation/getCurrentPosition
    static async getLocation(options?: PositionOptions): Promise<GetLocationResult> {
        const result: GetLocationResult = { position: null, error: null };
        const getCurrentPositionPromise = new Promise((resolve, reject) => {
            if (!navigator.geolocation) {
                reject({ code: 0, message: 'This device does not support geolocation.' });
            } else {
                navigator.geolocation.getCurrentPosition(resolve, reject, Geolocation.#buildOptions(options));
            }
        });
        await getCurrentPositionPromise.then(
            (position: GeolocationPosition) => { result.position = position; }
        ).catch(
            (error: any) => {
                result.error = { code: error.code, message: error.message };
                Geolocation.#logger.logMessage(LogLevel.Error, "getLocation error:", result);
            }
        );
        return result;
    }

    //https://developer.mozilla.org/en-US/docs/Web/API/Geolocation/watchPosition
    static async watchPosition(dotNetRef: any, key: string, options?: PositionOptions, callbackMethod?: string): Promise<number> {
        if (!navigator.geolocation) return null;

        if (Geolocation.isWatched(key)) {
            return;
        }

        callbackMethod ??= EventNotification.NotifyGeolocationWatch;

        const id = navigator.geolocation.watchPosition(
            (position: GeolocationPosition) => {
                if (Geolocation.isWatched(key)) {
                    const result: GetLocationResult = { position: position, error: null };
                    const args: GeolocationEventArgs = { key: key, reason: GeolocationEventReason.WatchSuccess, result: result };
                    dotNetRef.invokeMethodAsync(callbackMethod, args);
                }
            },
            (error: any) => {
                const result: GetLocationResult = { position: null, error: { code: error.code, message: error.message } };
                const args: GeolocationEventArgs = { key: key, reason: GeolocationEventReason.WatchError, result: result };

                Geolocation.#logger.logMessage(LogLevel.Error, "watchPosition error:", args);
                dotNetRef.invokeMethodAsync(callbackMethod, args);
            },

            Geolocation.#buildOptions(options)
        );

        Geolocation.#watchIDs.set(key, id);
        return id;
    }

    //https://developer.mozilla.org/en-US/docs/Web/API/Geolocation/clearWatch
    static clearWatch(key: string) {
        if (!navigator.geolocation) return;

        if (Geolocation.#watchIDs.has(key)) {
            const id = Geolocation.#watchIDs.get(key);
            navigator.geolocation.clearWatch(id);
            Geolocation.#watchIDs.delete(key);
        }
        else
            Geolocation.#logger.logMessage(LogLevel.Debug, `clearWatch. key was not found: ${key} `);
    }

    static clearWatches() {
        const keys = Array.from(Geolocation.#watchIDs.keys());

        Geolocation.#logger.logMessage(LogLevel.Debug, "Clearing watches:", keys);

        keys.forEach((key) => {
            Geolocation.clearWatch(key);
        });
    }

    static isWatched(key: string) {
        return Geolocation.#watchIDs.has(key);
    }

    //If options are passed by JsInterop and timeout is null, it usually fails.
    //To workaround this, create an object that has only the values that contain an actual value.
    static #buildOptions(options?: PositionOptions) {
        if (options) {
            const result: PositionOptions = {};

            if (options.enableHighAccuracy) {
                result.enableHighAccuracy = options.enableHighAccuracy;
            }
            if (options.maximumAge) {
                result.maximumAge = options.maximumAge;
            }
            if (options.timeout) {
                result.timeout = options.timeout;
            }

            return result;
        }

        return undefined;
    }
}

enum EventNotification {
    NotifyGeolocationWatch = 'NotifyGeolocationWatch',
}

enum GeolocationEventReason {
    WatchSuccess = "WatchSuccess",
    WatchError = "WatchError"
}

type GeolocationEventArgs = {
    key: string;
    reason: string;
    result: GetLocationResult;
}

type GetLocationResult = {
    position: any;
    error: any;
}