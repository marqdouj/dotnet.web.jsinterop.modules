import { Logger, LogLevel } from "./Logger"

export class Observer {
    static #observer: ResizeObserver;
    static readonly #logger: Logger = new Logger();

    static setLogLevel(level: LogLevel) {
        Observer.#logger.setLogLevel(level);
    }

    static #initialize(dotNetRef: any, callbackMethod?: string) {
        if (Observer.#observer)
            return;

        if (window.ResizeObserver) {
            Observer.#observer = new ResizeObserver(entries => {
                for (const entry of entries) {
                    let width:number;
                    let height:number;
                    if (entry.contentBoxSize[0]) {
                        width = entry.contentBoxSize[0].inlineSize;
                        height = entry.contentBoxSize[0].blockSize;
                    }
                    else {
                        width = entry.contentRect.width;
                        height = entry.contentRect.height;
                    }

                    callbackMethod ??= EventNotification.NotifyObserveResized;
                    const result = { id: entry.target.id, height: height, width: width };
                    Observer.#logger.logMessage(LogLevel.Trace, `Observer callbackMethod = '${callbackMethod}'`, result);

                    dotNetRef.invokeMethodAsync(callbackMethod, result);
                }
            });
        }
        else 
            Observer.#logger.logMessage(LogLevel.Warn, "Resize observer not supported!");
    }

    static addResizers(dotNetRef: any, ids:string[], callbackMethod?: string){
        Observer.#initialize(dotNetRef, callbackMethod);

        if (Observer.#observer) {
            if (!ids){
                console.warn('Observer.addResizer - missing id list.');
                Observer.#logger.logMessage(LogLevel.Warn, "Observer.addResizer - missing id list.");
                return;
            }

            ids.forEach((id, index) => {
                const elem = document.getElementById(id);
                if (elem)
                    Observer.#observer.observe(elem);
                else
                    Observer.#logger.logMessage(LogLevel.Warn, `Element where id = '${id}' does not exist and will not be observed.`);
            });
        }
    }

    static removeResizers(ids:string[]){
        if (Observer.#observer) {
            ids.forEach((id, index) => {
                const elem = document.getElementById(id);
                if (elem)
                    Observer.#observer.unobserve(elem);
                else
                    Observer.#logger.logMessage(LogLevel.Warn, `Element where id = '${id}' does not exist and will not be unobserved. `);
            });
        }
    }

    static disconnectResizers(){
        if (Observer.#observer) {
            Observer.#observer.disconnect();
            Observer.#observer = null;
            Observer.#logger.logMessage(LogLevel.Trace, "disconnectResizers was called.");
        }
    }
}

enum EventNotification {
    NotifyObserveResized = 'NotifyObserveResized',
}
