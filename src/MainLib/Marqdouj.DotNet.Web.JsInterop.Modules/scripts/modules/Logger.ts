export enum LogLevel {
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warn = 3,
    Error = 4,
    Critical = 5,
    None = 6
}

export class Logger {
    #logLevel: LogLevel = LogLevel.Information;

    setLogLevel(level: LogLevel) {
        this.#logLevel = level ?? LogLevel.Information;
        this.logMessage(LogLevel.Information, `LogLevel has been set to: '${LoggerHelper.logLevelName(this.#logLevel)}'`);
    }

    logMessage(level: LogLevel, message: string, ...optionalParams: any[]): void {
        if (level < this.#logLevel)
            return;

        const logOutput = `[${LoggerHelper.logLevelName(level)}] ${message}`;

        switch (level) {
            case LogLevel.Trace:
                console.trace(logOutput, ...optionalParams);
                break;
            case LogLevel.Debug:
                console.debug(logOutput, ...optionalParams);
                break;
            case LogLevel.Information:
                console.info(logOutput, ...optionalParams);
                break;
            case LogLevel.Warn:
                console.warn(logOutput, ...optionalParams);
                break;
            case LogLevel.Error:
                console.error(logOutput, ...optionalParams);
                break;
            case LogLevel.Critical:
                console.error(`CRITICAL: ${logOutput}`, ...optionalParams);
                break;
        }
    }
}

export class LoggerHelper {
    static logLevelName(level: LogLevel): string {
        switch (level) {
            case LogLevel.Critical:
                return "Critical";
            case LogLevel.Debug:
                return "Debug";
            case LogLevel.Error:
                return "Error";
            case LogLevel.Information:
                return "Information";
            case LogLevel.None:
                return "None";
            case LogLevel.Trace:
                return "Trace";
            case LogLevel.Warn:
                return "Warn";
            default:
        }
    }
}
