

class LoaderBase {

    static createDiv(_class, _style) {

        let ele = document.createElement("div");

        if (typeof _class === "string" && _class !== "") {

            ele.className = _class;
        }

        if (typeof _style === "string" && _style !== "") {

            ele.style.cssText = _style;
        }

        return ele;
    }

    constructor(name, cssObj) {

        this.name = name;

        // creo lo styleSheet che mi serve (se non c'è già)
        if (document.getElementsByTagName('head').length == 0)
            return;

        let stylesheet = document.getElementById(this.name + "-stylesheet");

        if (!stylesheet) {

            let css = "";

            $.each(cssObj.styles, function (i, s) {

                css += s.selector + " {";

                if (!s.css.includes("--prefix--")) {

                    css += s.css;

                } else {

                    $.each(['-moz-', '-webkit-', '-ms-', ''], function (j, p) {

                        css += s.css.replace("--prefix--", p);
                    });

                }
                if (s.animation) {

                    $.each(['-moz-', '-webkit-', '-ms-', ''], function (j, p) {

                        css += p + "animation:" + s.animation.name + " " + s.animation.props + ";";
                    });
                }

                css += "} ";
            });

            if (cssObj.keyframes) {

                $.each(cssObj.keyframes, function (i, kf) {

                    $.each(['-moz-', '-webkit-', '-ms-', ''], function (j, p) {

                        css += "@" + p + "keyframes " + kf.animation + " {";

                        $.each(kf.frames, function (k, f) {

                            css += " " + f.replace("--prefix--", p);
                        });

                        css += "}";
                    });

                });
            }

            let style = document.createElement('style');
            style.id = this.name + "-stylesheet";
            style.type = 'text/css';
            style.innerHTML = css;

            document.getElementsByTagName('head')[0].appendChild(style);
        }
    }

    loaderColor(name, color) {

        if (typeof color === "string") {

            let s = new Option().style;
            s.color = color;

            if (s.color === '') {

                color = "#00BFFF";
            }

        } else {

            color = "#00BFFF";
        }

        return name + ": " + color;
    }
}


class LoaderDotPulse extends LoaderBase {
    constructor() {
        super("dot-pulse",
            {
                styles: [
                    {
                        selector: ".dot-pulse, .dot-pulse::before, .dot-pulse::after",
                        css: "width:10px; height:10px; border-radius:50%;"
                    },
                    {
                        selector: ".dot-pulse",
                        css: "position:relative; left:-9999px; box-shadow:9999px 0 0 -5px var(--dot-pulse-color);",
                        animation: {
                            name: "dotPulse",
                            props: "1.5s infinite linear .25s"
                        }
                    },
                    {
                        selector: ".dot-pulse::before, .dot-pulse::after",
                        css: "content:''; position:absolute;"
                    },
                    {
                        selector: ".dot-pulse::before",
                        css: "box-shadow: 9984px 0 0 -5px var(--dot-pulse-color);",
                        animation: {
                            name: "dotPulseBefore",
                            props: "1.5s infinite linear 0s"
                        }
                    },
                    {
                        selector: ".dot-pulse::after",
                        css: "box-shadow: 10014px 0 0 -5px var(--dot-pulse-color);",
                        animation: {
                            name: "dotPulseAfter",
                            props: "1.5s infinite linear .5s"
                        }
                    }
                ],
                keyframes: [
                    {
                        animation: "dotPulseBefore",
                        frames: [
                            "0% { box-shadow: 9984px 0 0 -5px var(--dot-pulse-color); }",
                            "30% { box-shadow: 9984px 0 0 2px var(--dot-pulse-color); }",
                            "60%, 100% { box-shadow: 9984px 0 0 -5px var(--dot-pulse-color); }"
                        ]
                    },
                    {
                        animation: "dotPulse",
                        frames: [
                            "0% { box-shadow: 9999px 0 0 -5px var(--dot-pulse-color); }",
                            "30% { box-shadow: 9999px 0 0 2px var(--dot-pulse-color); }",
                            "60%, 100% { box-shadow: 9999px 0 0 -5px var(--dot-pulse-color); }"
                        ]
                    },
                    {
                        animation: "dotPulseAfter",
                        frames: [
                            "0% { box-shadow: 10014px 0 0 -5px var(--dot-pulse-color); }",
                            "30% { box-shadow: 10014px 0 0 2px var(--dot-pulse-color); }",
                            "60%, 100% { box-shadow: 10014px 0 0 -5px var(--dot-pulse-color); }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        return LoaderBase.createDiv("dot-pulse", this.loaderColor("--dot-pulse-color", color));
    }
}


class LoaderBallPulse extends LoaderBase {
    constructor() {
        super("ball-pulse",
            {
                styles: [
                    {
                        selector: ".ball-container",
                        css: "width:40px; height:40px;"
                    },
                    {
                        selector: ".ball-pulse",
                        css: "width:100%; height:100%; border-radius:50%; background-color: var(--ball-pulse-color);",
                        animation: {
                            name: "ball-pulse",
                            props: "1s infinite ease-in;"
                        }
                    }
                ],
                keyframes: [
                    {
                        animation: "ball-pulse",
                        frames: [
                            "from { --prefix--transform: scale(0); opacity:1; }",
                            "to { --prefix--transform: scale(1); opacity:0; }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        let divPulse = LoaderBase.createDiv("ball-container", this.loaderColor("--ball-pulse-color", color));
        divPulse.appendChild(LoaderBase.createDiv("ball-pulse"));
        return divPulse;
    }
}


class LoaderBarPulse extends LoaderBase {
    constructor() {
        super("bar-pulse",
            {
                styles: [
                    {
                        selector: ".bar-pulse",
                        css: "position:relative; height:50px;"
                    },
                    {
                        selector: ".bar-pulse div",
                        css: "background-color:var(--bar-pulse-color); height:100%; width:6px; margin:0px 2px; border-radius:3px; display: inline-block;"
                    },
                    {
                        selector: ".bar-pulse div",
                        css: "--prefix--transform:scaleY(0.4);",
                        animation: {
                            name: "stretchdelay",
                            props: "1.2s infinite ease-in-out"
                        }
                    },
                    {
                        selector: ".bar-pulse div:not(:nth-child(1))",
                        css: "--prefix--animation-delay:var(--bar-pulse-delay);"
                    }
                ],
                keyframes: [
                    {
                        animation: "stretchdelay",
                        frames: [
                            "0%, 40%, 100% { --prefix--transform: scaleY(0.4); }",
                            "20% { --prefix--transform: scaleY(1); }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        let divPulse = LoaderBase.createDiv("bar-pulse", this.loaderColor("--bar-pulse-color", color));
        let delay = -1200;
        let style = "";
        for (let c = 1; c <= 5; c++) {
            divPulse.appendChild(LoaderBase.createDiv("", style));
            delay += 100;
            style = "--bar-pulse-delay:" + delay + "ms;";
        }
        return divPulse;
    }
}


class LoaderRingPulse extends LoaderBase {
    constructor() {
        super("ring-pulse",
            {
                styles: [
                    {
                        selector: ".ring-pulse",
                        css: "height:50px; width:50px; opacity:0; border:1px solid var(--ring-pulse-color); border-radius:40px;",
                        animation: {
                            name: "pulsate",
                            props: "1s infinite ease-out"
                        }
                    }
                ],
                keyframes: [
                    {
                        animation: "pulsate",
                        frames: [
                            "0% { --prefix--transform:scale(0.1, 0.1); opacity:0.0; }",
                            "50% { opacity:1.0; }",
                            "100% { --prefix--transform:scale(1.3, 1.3); opacity:0.0; }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        return LoaderBase.createDiv("ring-pulse", this.loaderColor("--ring-pulse-color", color));
    }
}


class LoaderDoubleBounce extends LoaderBase {
    constructor() {
        super("double-bounce",
            {
                styles: [
                    {
                        selector: ".double-bounce",
                        css: "width:40px; height:40px;"
                    },
                    {
                        selector: ".double-bounce > div",
                        css: "width:100%; height:100%; border-radius:50%; background-color: var(--double-bounce-color); opacity:0.6; position:absolute; top:0; left:0;",
                        animation: {
                            name: "sk-bounce",
                            props: "2.0s infinite ease-in-out"
                        }
                    },
                    {
                        selector: ".double-bounce > div:nth-child(2)",
                        css: "--prefix--animation-delay:-1.0s;",
                    }
                ],
                keyframes: [
                    {
                        animation: "sk-bounce",
                        frames: [
                            "0%, 100% { --prefix--transform: scale(0.0); }",
                            "50% { --prefix--transform: scale(1.0); }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        let divPulse = LoaderBase.createDiv("double-bounce", this.loaderColor("--double-bounce-color", color));
        divPulse.appendChild(LoaderBase.createDiv());
        divPulse.appendChild(LoaderBase.createDiv());
        return divPulse;
    }
}


class LoaderSpinnerDot extends LoaderBase {
    constructor() {
        super("spinner-dot",
            {
                styles: [
                    {
                        selector: ".spinner-dot",
                        css: "width:50px; height:50px;"
                    },
                    {
                        selector: ".spinner-dot > div",
                        css: "position:absolute; left:7%; top:9%; width:15%; height:15%; border-radius:50%; background-color:var(--spinner-dot-color);"
                    },
                    {
                        selector: ".spinner-dot > div",
                        css: "--prefix--transform:rotateZ(50deg);",
                    },
                    {
                        selector: ".spinner-dot > div",
                        css: "--prefix--transform-origin:275% 275%;",
                        animation: {
                            name: "spinner-dot-rotate",
                            props: "1.3s infinite cubic-bezier(0.7, 0.3, 0.3, 0.7);"
                        }
                    },
                    {
                        selector: ".spinner-dot > div:not(:nth-child(1))",
                        css: "--prefix--animation-delay:var(--spinner-dot-delay);"
                    }
                ],
                keyframes: [
                    {
                        animation: "spinner-dot-rotate",
                        frames: [
                            "to { --prefix--transform: rotateZ(410deg); }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        let divPulse = LoaderBase.createDiv("spinner-dot", this.loaderColor("--spinner-dot-color", color));
        let delay = 0;
        let style = "";
        for (let c = 0; c < 5; c++) {
            divPulse.appendChild(LoaderBase.createDiv("", style));
            delay += 170;
            style = "--spinner-dot-delay:" + delay + "ms;";
        }
        return divPulse;
    }
}


class LoaderSpinner extends LoaderBase {
    constructor() {
        super("spinner",
            {
                styles: [
                    {
                        selector: ".spinner",
                        css: "width:50px; height:50px; border-top: 2px solid var(--spinner-color); border-radius: 50%;",
                        animation: {
                            name: "spinner-rotate",
                            props: "1s infinite"
                        }
                    }
                ],
                keyframes: [
                    {
                        animation: "spinner-rotate",
                        frames: [
                            "100% { --prefix--transform: rotate(360deg); }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        return LoaderBase.createDiv("spinner", this.loaderColor("--spinner-color", color));
    }
}


class LoaderSpinner2 extends LoaderBase {
    constructor() {
        super("spinner2",
            {
                styles: [
                    {
                        selector: ".spinner2",
                        css: "width:50px; height:50px; border-right:4px solid var(--spinner2-color); border-bottom:4px solid transparent; border-radius: 50%;",
                        animation: {
                            name: "spinner2-rotate",
                            props: "1500ms linear infinite"
                        }
                    }
                ],
                keyframes: [
                    {
                        animation: "spinner2-rotate",
                        frames: [
                            "100% { --prefix--transform: rotate(360deg); }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        return LoaderBase.createDiv("spinner2", this.loaderColor("--spinner2-color", color));
    }
}


class LoaderSpinnerSlice extends LoaderBase {
    constructor() {
        super("spinner-slice",
            {
                styles: [
                    {
                        selector: ".spinner-slice",
                        css: "position:relative; width:40px; height:40px;"
                    },
                    {
                        selector: ".spinner-slice",
                        css: "--prefix--transform-origin: center;",
                        animation: {
                            name: "spinnerSliceAnimation",
                            props: "1s ease infinite"
                        }
                    },
                    {
                        selector: ".spinner-slice > .slice",
                        css: "position:absolute; width:100%; height:100%; clip:rect(0, 40px, 40px, 20px); border-radius:100%;"
                    },
                    {
                        selector: ".spinner-slice > .slice:nth-child(2)",
                        css: "--prefix--transform:rotate(120deg);"
                    }
                    ,
                    {
                        selector: ".spinner-slice > .slice:nth-child(3)",
                        css: "--prefix--transform:rotate(240deg);"
                    },
                    {
                        selector: ".spinner-slice > .slice > div",
                        css: "position:absolute; box-sizing:border-box; width:100%; height:100%; border:5px solid var(--spinner-slice-color); border-radius:100%; clip:rect(0, 20px, 40px, 0);"
                    },
                    {
                        selector: ".spinner-slice > .slice > div",
                        css: "--prefix--transform:rotate(115deg);",
                        animation: {
                            name: "sliceAnimation",
                            props: "1s ease infinite"
                        }
                    }
                ],
                keyframes: [
                    {
                        animation: "sliceAnimation",
                        frames: [
                            "0% { --prefix--transform:rotate(20deg); }",
                            "25% { --prefix--transform:rotate(115deg); }",
                            "100% { --prefix--transform:rotate(20deg); }"
                        ]
                    },
                    {
                        animation: "spinnerSliceAnimation",
                        frames: [
                            "100% { --prefix--transform:rotate(360deg); }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        let divPulse = LoaderBase.createDiv("spinner-slice", this.loaderColor("--spinner-slice-color", color));
        for (let c = 0; c < 3; c++) {
            let slice = LoaderBase.createDiv("slice");
            slice.appendChild(LoaderBase.createDiv());
            divPulse.appendChild(slice);
        }
        return divPulse;
    }
}


class LoaderRipple extends LoaderBase {
    constructor() {
        super("ball-ripple",
            {
                styles: [
                    {
                        selector: ".ball-ripple",
                        css: "position: relative;"
                    },
                    {
                        selector: ".ball-ripple",
                        css: "--prefix--transform: translateY(-30px);"
                    },
                    {
                        selector: ".ball-ripple > div",
                        css: "background-color:var(--ball-ripple-color); border-radius:100%; position:absolute; left:-30px; top:0; opacity:0; margin:0; width:60px; height:60px;",
                        animation: {
                            name: "rippleAnimation",
                            props: "1s 0s linear infinite"
                        }
                    },
                    {
                        selector: ".ball-ripple > div:nth-child(2)",
                        css: "--prefix--animation-delay:-400ms;"
                    },
                    {
                        selector: ".ball-ripple > div:nth-child(3)",
                        css: "--prefix--animation-delay:-200ms;"
                    }
                ],
                keyframes: [
                    {
                        animation: "rippleAnimation",
                        frames: [
                            "0% { --prefix--transform:scale(0); opacity:0; }",
                            "5% { opacity:1; }",
                            "100% { --prefix--transform:scale(1); opacity:0; }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        let divPulse = LoaderBase.createDiv("ball-ripple", this.loaderColor("--ball-ripple-color", color));
        for (let c = 0; c < 3; c++) {
            divPulse.appendChild(LoaderBase.createDiv());
        }
        return divPulse;
    }
}


class LoaderDotCircle extends LoaderBase {
    constructor() {
        super("dot-circle",
            {
                styles: [
                    {
                        selector: ".dot-circle",
                        css: "width:40px; height:40px;"
                    },
                    {
                        selector: ".dot-circle > div",
                        css: "width:100%; height:100%; position:absolute; left:0; top:0;"
                    },
                    {
                        selector: ".dot-circle > div:before",
                        css: "content:''; display:block; margin:0 auto; width:15%; height:15%; background-color: var(--dot-circle-color); border-radius:100%;",
                        animation: {
                            name: "dot-circle-bounce",
                            props: "1.2s infinite ease-in-out both"
                        }
                    },
                    {
                        selector: ".dot-circle > div:not(:nth-child(1))",
                        css: "--prefix--transform:rotate(var(--dot-circle-rotate));"
                    },
                    {
                        selector: ".dot-circle > div:not(:nth-child(1)):before",
                        css: "--prefix--animation-delay:var(--dot-circle-delay);"
                    }
                ],
                keyframes: [
                    {
                        animation: "dot-circle-bounce",
                        frames: [
                            "0%, 80%, 100% { --prefix--transform: scale(0); }",
                            "40% { --prefix--transform: scale(1); }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        let divPulse = LoaderBase.createDiv("dot-circle", this.loaderColor("--dot-circle-color", color));

        let deg = 0;
        let delay = -1200
        let style = "";
        for (let c = 1; c <= 12; c++) {
            divPulse.appendChild(LoaderBase.createDiv("", style));
            deg += 30;
            delay += 100;
            style = "--dot-circle-rotate:" + deg + "deg; --dot-circle-delay:" + delay + "ms;"
        }
        return divPulse;
    }
}


class LoaderDrops extends LoaderBase {
    constructor() {
        super("drops",
            {
                styles: [
                    {
                        selector: ".drops",
                        css: "position:relative; height:10px; width:80px; margin:0 auto;"
                    },
                    {
                        selector: ".drops > div",
                        css: "position:absolute; top:0px; height:10px; width:10px; border-radius:50%; margin-left:-3000px; background-color:var(--drops-color);",
                        animation: {
                            name: "drops",
                            props: "2500ms ease-in-out infinite"
                        }
                    }
                ],
                keyframes: [
                    {
                        animation: "drops",
                        frames: [
                            "0% { margin-left: -3000px; }",
                            "30%, 70%{ margin-left: 0px; }",
                            "100% { margin-left: 3000px; }"
                        ]
                    }
                ]
            }
        );
    }

    elem(color) {
        let divDrops = LoaderBase.createDiv("drops", this.loaderColor("--drops-color", color));

        let left = 5;
        let delay = 250
        let style = "";
        for (let c = 1; c <= 5; c++) {
            style = "left:" + left + "px; animation-delay:" + delay + "ms;"
            divDrops.appendChild(LoaderBase.createDiv("", style));
            left += 15;
            delay += 250;
        }
        return divDrops;
    }
}


class LoaderFlip extends LoaderBase {
    constructor() {
        super("flip",
            {
                styles: [
                    {
                        selector: ".flip",
                        css: "width:40px; height:40px; background-color:var(--flip-color);",
                        animation: {
                            name: "flip",
                            props: "1.2s ease-in-out infinite"
                        }
                    }
                ],
                keyframes: [
                    {
                        animation: "flip",
                        frames: [
                            "0% { transform: perspective(120px) rotateX(0deg) rotateY(0deg); }",
                            "50% { transform: perspective(120px) rotateX(-180.1deg) rotateY(0deg); }",
                            "100% { transform: perspective(120px) rotateX(-180deg) rotateY(-179.9deg); }"
                        ]
                    }
                ]
            }
        );
    }
    elem(color) {
        return LoaderBase.createDiv("flip", this.loaderColor("--flip-color", color));
    }
}


(function ($) {
    $.StyleLoader = function (elem, options) {

        var plugin = this;

        plugin.elem = elem;
        plugin.wrapper = null;

        plugin.remove = function () {
            if (plugin.wrapper !== null) {
                plugin.wrapper.remove();
                plugin.wrapper = null;
            }
            $(plugin.elem).removeData("StyleLoader");
        };

        plugin.wrapper = LoaderBase.createDiv("", "position:relative; width:100%; height:100%;");

        let elemLoader = LoaderBase.createDiv("", "position:absolute; top:50%; left:50%; transform:translate(-50%,-50%);");

        plugin.wrapper.appendChild(elemLoader);

        let loader = null;

        switch (options.type) {
            case 'BallPulse':
                loader = new LoaderBallPulse();
                break;
            case 'BarPulse':
                loader = new LoaderBarPulse();
                break;
            case 'RingPulse':
                loader = new LoaderRingPulse();
                break;
            case 'DoubleBounce':
                loader = new LoaderDoubleBounce();
                break;
            case 'SpinnerDot':
                loader = new LoaderSpinnerDot();
                break;
            case 'Spinner':
                loader = new LoaderSpinner();
                break;
            case 'Spinner2':
                loader = new LoaderSpinner2();
                break;
            case 'SpinnerSlice':
                loader = new LoaderSpinnerSlice();
                break;
            case 'Ripple':
                loader = new LoaderRipple();
                break;
            case 'DotCircle':
                loader = new LoaderDotCircle();
                break;
            case "Drops":
                loader = new LoaderDrops();
                elemLoader.style.cssText += " width:100%; overflow:hidden;";
                break;
            case "Flip":
                loader = new LoaderFlip();
                break;
            case 'DotPulse':
            default:
                loader = new LoaderDotPulse();
        }

        elemLoader.appendChild(loader.elem(options.color));

        plugin.elem.appendChild(plugin.wrapper);

    }; // StyleLoader

    $.fn.StyleLoader = function (options) {

        if (undefined === options) {

            options = { type: "DotPulse" };

        } else {

            if (typeof options.type !== "string" || $.trim(options.type) === "") {

                options.type = "DotPulse";
            }
        }

        return this.each(function () {

            if (undefined == $(this).data('StyleLoader')) {

                var plugin = new $.StyleLoader(this, options);

                $(this).data('StyleLoader', plugin);
            }
        });
    };

})(jQuery);

