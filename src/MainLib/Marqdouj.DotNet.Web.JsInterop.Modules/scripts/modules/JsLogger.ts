import { LoggerHelper, LogLevel } from "./Logger"

export class JsLogger {
    static test(config: JsLoggerConfig, message = 'Testing Logger') {
        const event = 'testLogger';
        config = config || new JsLoggerConfig('test', LogLevel.Trace, LogLevel.Critical);
        console.log(`${event}: Template [${config.template}]`);
        JsLogger.logTrace(config, message, event);
        JsLogger.logDebug(config, message, event);
        JsLogger.logInformation(config, message, event);
        JsLogger.logWarning(config, message, event);
        JsLogger.logError(config, message, event);
        JsLogger.logCritical(config, message, event);
    }

    static formatMessage(template: string, category: string, level: LogLevel, event: string, message: string) {
        let tCategory = template.includes("{category}") ? `${category} ` : '';
        let tLevel = template.includes("{level}") ? `${LoggerHelper.logLevelName(level)}} ` : '';
        let tEvent = template.includes("{event}") ? `${event} ` : '';
        let tTimestamp = template.includes("{timestamp}") ? `${new Date().toISOString()} ` : '';
        let tMessage = template.includes("{message}") ? `${JsLogger.#checkMessage(message)}` : '';
        var tTemplate = template
            .replace("{category}", tCategory)
            .replace("{level}", tLevel)
            .replace("{event}", tEvent)
            .replace("{timestamp}", tTimestamp)
            .replace("{message}", tMessage);
        return tTemplate;
    }

    static #checkMessage(message: string) {
        let messageCheck = '';
        if (typeof message !== 'string') {
            messageCheck = 'log requested but message is not a string';
        }
        if (message.length === 0) {
            if (messageCheck.length > 0) {
                messageCheck += '; ';
            }
            messageCheck += 'log requested but message is empty';
        }
        return messageCheck.length > 0 ? messageCheck : message;
    }

    static logTrace(config: JsLoggerConfig, message: string, event = "") {
        this.log(config, LogLevel.Trace, message, event);
    }
    static logDebug(config: JsLoggerConfig, message: string, event = "") {
        this.log(config, LogLevel.Debug, message, event);
    }
    static logInformation(config: JsLoggerConfig, message: string, event = "") {
        this.log(config, LogLevel.Information, message, event);
    }
    static logWarning(config: JsLoggerConfig, message: string, event = "") {
        this.log(config, LogLevel.Warn, message, event);
    }
    static logError(config: JsLoggerConfig, message: string, event = "") {
        this.log(config, LogLevel.Error, message, event);
    }
    static logCritical(config: JsLoggerConfig, message: string, event = "") {
        this.log(config, LogLevel.Critical, message, event);
    }
    static isEnabled(config: JsLoggerConfig, level: LogLevel) {
        return level != LogLevel.None && level >= config.minLevel && level <= config.maxLevel;
    }
    static logRaw(message: string, style = "") {
        if (typeof message !== 'string') {
            message = 'log requested but message is not a string';
        }
        if (message.length === 0) {
            message = 'log requested but message is empty';
        }
        console.log(`${"%c"}${message}`, style);
    }
    static log(config: JsLoggerConfig, level: LogLevel, message: string, event = "") {
        let fMessage = JsLogger.formatMessage(config.template, config.category, level, event, message);
        this.logMessage(config, level, fMessage);
    }
    static logMessage(config: JsLoggerConfig, level: LogLevel, message: string) {
        if (this.isEnabled(config, level)) {
            switch (level) {
                case LogLevel.Trace:
                    console.trace(message);
                    break;
                case LogLevel.Debug:
                    console.debug(message);
                    break;
                case LogLevel.Information:
                    console.info(message);
                    break;
                case LogLevel.Warn:
                    console.warn(message);
                    break;
                case LogLevel.Error:
                    console.error(message);
                    break;
                case LogLevel.Critical:
                    console.error(message);
                    break;
                case LogLevel.None:
                default:
                    // No logging
                    break;
            }
        }
    }
}

export class JsLoggerConfig {
    #category: string;
    #minLevel: LogLevel;
    #maxLevel: LogLevel;
    #template: string;

    constructor(category: string, minLevel = LogLevel.Information, maxLevel = LogLevel.Critical, template: string = "") {
        this.#category = category;
        this.#template = template.length == 0 ? "{category}{event}{timestamp}{level}: {message}" : template;
        this.#setLevel(minLevel, maxLevel);
    }
    get category() {
        return this.#category;
    }

    get minLevel() {
        return this.#minLevel;
    }

    get maxLevel() {
        return this.#maxLevel;
    }

    get template() {
        return this.#template;
    }

    #setLevel(min: LogLevel, max: LogLevel) {
        if (min > max) {
            throw (`Minimum log level ${min} cannot be greater than maximum log level ${max}.`);
        }
        this.#minLevel = min;
        this.#maxLevel = max;
    }
}