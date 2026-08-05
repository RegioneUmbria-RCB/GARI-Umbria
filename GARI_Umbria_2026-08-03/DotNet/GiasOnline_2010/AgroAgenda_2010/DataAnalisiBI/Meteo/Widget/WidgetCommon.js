


class WidgetCommon {

    constructor() {
    }


    static createElement(_tag, _class, _style, _id) {

        let ele = document.createElement(_tag);

        if (typeof _class === "string" && _class !== "") {

            ele.className = _class;
        }

        if (typeof _style === "string" && _style !== "") {

            ele.style.cssText = _style;
        }

        if (typeof _id === "string" && _id !== "") {

            ele.id = _id;
        }
        return ele;
    }


    static createWindow(title, window_class) {

        let winElem = WidgetCommon.createElement("div", "", "padding:5px;");
        document.body.appendChild(winElem);
        let $winElem = $(winElem);

        let styleElem = document.createElement('style');
        styleElem.type = "text/css";
        styleElem.innerHTML = ".body_overflow_hidden { overflow: hidden !important; }";
        winElem.appendChild(styleElem);

        let wincfg = {
            title: title,
            width: "90%",
            height: "90%",
            draggable: false,
            visible: false,
            modal: true,
            resizable: false,
            actions: [
                "Close"
            ],
            open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                e.sender.element.css("opacity", "0");
                $("body").addClass("body_overflow_hidden");

                if (typeof window_class === "string") {

                    e.sender.element.addClass(window_class);
                }
            },
            activate: function (e) {
                e.sender.element.css("opacity", "1");
            },
            close: function (e) {
                $("body").removeClass("body_overflow_hidden");
                this.destroy();
            }
        };

        $winElem.kendoWindow(wincfg);

        let parent = $winElem.parent();
        parent.find('.k-window-title').css("text-align", "center");
        parent.css("padding-top", "48px");
        let titlebar = parent.find(".k-window-titlebar");
        titlebar.css({
            "margin-top": "-48px",
            "font-size": "large"
        });

        return winElem;
    }


    static showMarquee(div_element, msg) {

        let style_id = "widget-marquee-stylesheet";

        let stylesheet = document.getElementById(style_id);

        if (!stylesheet) {

            let css = ".widget-marquee-overlay { ";
            css += "position: relative; ";
            css += "overflow: hidden; ";
            css += "height: 100%; ";
            css += "font-size: larger; ";
            css += "font-weight: bolder; ";
            css += "} ";
            css += ".widget-marquee-text { ";
            css += "position: absolute; ";
            css += "cursor: default; ";
            css += "white-space: nowrap; ";
            css += "top: 50%; ";
            css += "} ";

            let animation_name = "widget-marquee-animation";

            let css_noscroll = "";
            let css_scroll = "";
            let css_keyframes = ""

            $.each(['-moz-', '-webkit-', '-ms-', ''], function (i, p) {

                css_noscroll += p + "transform: translate(-50%, -50%); ";

                css_scroll += p + "transform: translate(0, -50%); ";
                css_scroll += p + "animation: " + animation_name + " 5s linear infinite; ";

                css_keyframes += "@" + p + "keyframes " + animation_name + " { 100% { " + p + "transform: translate(-100%, -50%); } } ";
            });

            css += ".widget-marquee-noscroll { ";
            css += "left: 50%; ";
            css += css_noscroll;
            css += "} ";
            css += ".widget-marquee-scroll { ";
            css += "padding-left: 100%; ";
            css += css_scroll;
            css += "} ";
            css += css_keyframes;

            let style = document.createElement('style');
            style.id = style_id;
            style.type = 'text/css';
            style.innerHTML = css;

            document.getElementsByTagName('head')[0].appendChild(style);
        }

        let divOverlay = WidgetCommon.createElement("div", "widget-marquee-overlay");
        let divText = WidgetCommon.createElement("div", "widget-marquee-text", "opacity:0;");

        divText.innerHTML = msg;
        divOverlay.appendChild(divText);
        div_element.appendChild(divOverlay);

        let contW = $(divOverlay).width();
        let textW = $(divText).width();

        if ((textW + 20) < contW) {

            $(divText).addClass("widget-marquee-noscroll");

        } else {

            $(divText).addClass("widget-marquee-scroll");

            let duration = ((parseInt(textW, 10) + parseInt(contW, 10)) / parseInt(textW, 10)) * 5;
            duration = Math.round(duration * 10) / 10;
            duration = duration + "s";

            $.each(['-moz-', '-webkit-', '-ms-', ''], function (i, p) {

                $(divText).css(p + "animation-duration", duration);
            });
        }

        divText.style.opacity = 1;
    }


    static ctrlClickMsg(e, msg, title) {

        if (typeof msg !== "string") {
            return;
        }
        if (msg === "") {
            return;
        }
        if (!e.ctrlKey) {
            return;
        }

        console.log(msg);

        let dlgElem = WidgetCommon.createElement("div");
        document.body.appendChild(dlgElem);
        let $dlgElem = $(dlgElem);

        let w = $(window).width() * 0.9;
        let h = $(window).height() * 0.9;

        $dlgElem.kendoDialog({
            title: title,
            maxWidth: w,
            maxHeight: h,
            closable: false,
            modal: true,
            visible: false,
            content: msg,
            actions: [
                { text: TraduzioneMultiResx(datiMeteoResx, "Chiudi", "Chiudi") }
            ],
            close: function (e) {
                this.destroy();
            }
        });

        $dlgElem.data("kendoDialog").open();
    }


    static getScriptLocation() {
        //Restituisce il path del file javascript corrente (WidgetCommon.js) 
        //attenzione ad utilizzarlo se il percorso che serve è diverso...
        var scriptPath = '';

        try {
            0(); //Throw an error to generate a stack trace
        }
        catch (e) {

            //Split the stack trace into each line
            var stackLines = e.stack.split('\n');

            //Now walk though each line until we find a path reference
            for (var line of stackLines) {

                if (line.match(/http[s]?:\/\//)) {

                    line.replace(/(http[s]?:\/\/[^\s]+\.js)/g, function (match) {
                        return scriptPath = match;
                    });
                    break;
                }
            }
        }

        return scriptPath.split('/').slice(0, -1).join('/') + '/';
    };


    static listenOnceAndReply(listenMsg, frameURI, replyObj) {

        window.addEventListener(
            "message",
            function listenFun(event) {

                if (verificaOriginSecondaria(self, location.href, event) == false) {
                    return false;
                }

                if (event.data === listenMsg) {

                    this.removeEventListener("message", listenFun);

                    let frames = document.getElementsByTagName("iframe");
                    $.each(frames, function (i, f) {

                        let found = false;

                        try {

                            if (f.contentDocument.baseURI.includes(frameURI)) {

                                f.contentWindow.postMessage(replyObj, ottieniTargetOrigin(window));

                                found = true;
                            }
                        }
                        catch (e) {
                        }

                        return !found;
                    });
                }
            },
            false
        );
    };


    static openFrameWindow(aspxPage, winTitle) {

        let url = WidgetCommon.getScriptLocation() + aspxPage;
        console.log(url);

        let winElem = WidgetCommon.createElement("div", "", "padding:5px;");
        document.body.appendChild(winElem);
        let $winElem = $(winElem);

        let wincfg = {
            title: winTitle,
            width: "90%",
            height: "90%",
            draggable: false,
            visible: false,
            modal: {
                preventScroll: true
            },
            resizable: false,
            iframe: true,
            content: url,
            actions: [
                "Close"
            ],
            open: function (e) {
                e.sender.element.css("opacity", "0");
            },
            activate: function (e) {
                e.sender.element.css("opacity", "1");
            },
            close: function (e) {
                this.destroy();
            }
        };

        $winElem.kendoWindow(wincfg);

        let parent = $winElem.parent();
        parent.find('.k-window-title').css("text-align", "center");
        parent.css("padding-top", "48px");
        let titlebar = parent.find(".k-window-titlebar");
        titlebar.css({
            "margin-top": "-48px",
            "font-size": "large"
        });

        $winElem.data("kendoWindow").center().open();

        return winElem;
    }
}



(function ($) {

    $.FontAdjustLabel = function (elem, options) {

        var plugin = this;

        plugin.$element = $(elem); // reference to the jQuery version of DOM element
        plugin.element = elem; // reference to the actual DOM element
        //plugin.settings = {};

        //// plugin's default options this is private property and is accessible only from inside the plugin
        //let defaults = {
        //    msg: ""
        //};

        //plugin.settings = $.extend({}, defaults, options);

        let style_id = "FontAdjustLabel-stylesheet";

        let stylesheet = document.getElementById(style_id);

        if (!stylesheet) {

            let css = ".fal-overflow-ellipsis { ";
            css += "white-space: nowrap; ";
            css += "overflow: hidden; ";
            css += "text-overflow: ellipsis; ";
            css += "} ";

            let style = document.createElement('style');
            style.id = style_id;
            style.type = 'text/css';
            style.innerHTML = css;

            document.getElementsByTagName('head')[0].appendChild(style);
        }

        //plugin.$element.addClass("fal-overflow-ellipsis");
        plugin.span = document.createElement("span");
        plugin.$span = $(plugin.span);
        plugin.element.appendChild(plugin.span);


        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------


        plugin.show = function (msg) {

            if (typeof msg !== 'string') {
                msg = '';
            }

            plugin.$element.removeClass("fal-overflow-ellipsis");
            let font_size = plugin.$element.css("font-size")
            plugin.$span.css("font-size", font_size);
            plugin.$span.text(msg);

            let spanH = plugin.$span.innerHeight();

            if (spanH > 0) {

                let px = parseInt(font_size);
                let divH = plugin.$element.innerHeight();

                while (spanH > divH) {

                    px--;

                    if (px > 9) {

                        plugin.$span.css("font-size", px + "px");
                        spanH = plugin.$span.innerHeight();

                    } else {

                        spanH = 0;
                    }
                }
            }
            plugin.$element.addClass("fal-overflow-ellipsis");
        };

        //-----------------------------------------------------------------------------------------
        // fire up the plugin!
        //-----------------------------------------------------------------------------------------


        plugin.show(options.msg);


    }; //FontAdjustLabel

    $.fn.FontAdjustLabel= function (options) {

        if (options === undefined) {
            options = { msg: "" };
        }

        return this.each(function () {

            if (undefined == $(this).data("FontAdjustLabel")) {

                var plugin = new $.FontAdjustLabel(this, options);

                $(this).data("FontAdjustLabel", plugin);
            }
        });
    };
})(jQuery);


