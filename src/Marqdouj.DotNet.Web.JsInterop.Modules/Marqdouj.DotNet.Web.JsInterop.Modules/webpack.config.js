const path = require('path');

module.exports = {
    mode: 'production',
    entry: {
        geolocation: "./tsgen/Geolocation.js",
        observer: "./tsgen/Observer.js",
    },
    output: {
        filename: "[name].js",
        path: path.resolve(__dirname, 'wwwroot'),
        library: {
            type: "module",
        },
    },
    experiments: {
        outputModule: true,
    },
};
