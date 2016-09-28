(function() {
    if (typeof (EventSource) !== "undefined") {

        var findScript = function() {
            var scripts = document.getElementsByTagName('script');
            for (var index = 0, len = scripts.length; index < len; index++) {
                var currentScript = scripts[index];
                var currentUrl = currentScript.src;
                if (currentUrl && currentUrl.indexOf("URL_QUERY") > -1) {
                    return currentScript;
                }
            }
        };


        var myScript = findScript();
        var scriptUrl = myScript.src;
        var urlPrefix = scriptUrl.substring(0, scriptUrl.lastIndexOf('/'));


        var streamUrl = urlPrefix + "/stream";
        var source = new EventSource(streamUrl);

        var defaultStyle = myScript.getAttribute("style");

        if (defaultStyle) {
            source.onmessage = function(event) {
                console.log("%c " + event.data, defaultStyle);
            };
        } else {
            source.onmessage = function(event) {
                console.log(event.data);
            };
        }

        var debugStyle = myScript.getAttribute("style-debug") || defaultStyle;
        if (debugStyle) {
            source.addEventListener("DEBUG",
                function(event) {
                    console.debug("%c " + event.data, debugStyle);
                });
        } else {
            source.addEventListener("DEBUG",
                function(event) {
                    console.debug(event.data);
                });
        }

        var infoStyle = myScript.getAttribute("style-info") || defaultStyle;
        if (infoStyle) {
            source.addEventListener("INFO",
                function(event) {
                    console.info("%c " + event.data, infoStyle);
                });
        } else {
            source.addEventListener("INFO",
                function(event) {
                    console.info(event.data);
                });
        }

        var warnStyle = myScript.getAttribute("style-warn") || defaultStyle;
        if (warnStyle) {
            source.addEventListener("WARN",
                function(event) {
                    console.warn("%c " + event.data, warnStyle);
                });
        } else {
            source.addEventListener("WARN",
                function(event) {
                    console.warn(event.data);
                });
        }

        var errorStyle = myScript.getAttribute("style-error") || defaultStyle;
        if (errorStyle) {
            source.addEventListener("ERROR",
                function(event) {
                    console.error("%c " + event.data, errorStyle);
                });
        } else {
            source.addEventListener("ERROR",
                function(event) {
                    console.error(event.data);
                });
        }

    } else {
        alert("Your browser does not support SSE, hence BrowserLog will not work properly");
    }
})();