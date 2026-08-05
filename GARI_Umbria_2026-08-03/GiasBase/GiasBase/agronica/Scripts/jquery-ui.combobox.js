//(function ($) {
//    $.widget("ui.combobox", {
//        _create: function () {
//            this.wrapper = $("<span>")
//					.addClass("custom-combobox")
//					.insertAfter(this.element);

//            this.element.hide();
//            this._createAutocomplete();
//            this._createShowAllButton();
//        },

//        _createAutocomplete: function () {
//            var selected = this.element.children(":selected"),
//					value = selected.val() ? selected.text() : "";

//            this.input = $("<input>")
//					.appendTo(this.wrapper)
//					.val(value)
//					.attr("title", "")
//					.addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
//					.autocomplete({
//					    delay: 0,
//					    minLength: 0,
//					    source: $.proxy(this, "_source")
//					})
//					.tooltip({
//					    tooltipClass: "ui-state-highlight"
//					});

//            this._on(this.input, {
//                autocompleteselect: function (event, ui) {
//                    ui.item.option.selected = true;
//                    this._trigger("select", event, {
//                        item: ui.item.option
//                    });
//                },

//                autocompletechange: "_removeIfInvalid"
//            });
//        },

//        _createShowAllButton: function () {
//            var input = this.input,
//					wasOpen = false;

//            $("<a>")
//					.attr("tabIndex", -1)
//					.attr("title", "Show All Items")
//					.tooltip()
//					.appendTo(this.wrapper)
//					.button({
//					    icons: {
//					        primary: "ui-icon-triangle-1-s"
//					    },
//					    text: false
//					})
//					.removeClass("ui-corner-all")
//					.addClass("custom-combobox-toggle ui-corner-right")
//					.mousedown(function () {
//					    wasOpen = input.autocomplete("widget").is(":visible");
//					})
//					.click(function () {
//					    input.focus();

//					    // Close if already visible
//					    if (wasOpen) {
//					        return;
//					    }

//					    // Pass empty string as value to search for, displaying all results
//					    input.autocomplete("search", "");
//					});
//        },

//        _source: function (request, response) {
//            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
//            response(this.element.children("option").map(function () {
//                var text = $(this).text();
//                if (this.value && (!request.term || matcher.test(text)))
//                    return {
//                        label: text,
//                        value: text,
//                        option: this
//                    };
//            }));
//        },

//        _removeIfInvalid: function (event, ui) {

//            // Selected an item, nothing to do
//            if (ui.item) {
//                return;
//            }

//            // Search for a match (case-insensitive)
//            var value = this.input.val(),
//					valueLowerCase = value.toLowerCase(),
//					valid = false;
//            this.element.children("option").each(function () {
//                if ($(this).text().toLowerCase() === valueLowerCase) {
//                    this.selected = valid = true;
//                    return false;
//                }
//            });

//            // Found a match, nothing to do
//            if (valid) {
//                return;
//            }

//            // Remove invalid value
//            this.input
//					.val("")
//					.attr("title", value + " didn't match any item")
//					.tooltip("open");
//            this.element.val("");
//            this._delay(function () {
//                this.input.tooltip("close").attr("title", "");
//            }, 2500);
//            this.input.data("ui-autocomplete").term = "";
//        },

//        _destroy: function () {
//            this.wrapper.remove();
//            this.element.show();
//        }
//    });
//})(jQuery);













        //evento wrapper per richiamare una eventuale funzione presente nella pagina che include il file
        function wrapCallOnPage($objSelect,val) { if (typeof DoPostBack_Combo == 'function') { DoPostBack_Combo($objSelect,val); } }
        
        (function ($) {
            $.widget("ui.combobox", {
                _create: function () {
                    var self = this;
                    var select = this.element.hide(),
					selected = select.children(":selected"),
					value = selected.val() ? selected.text() : "";
                    var input = $("<input>")
					.insertAfter(select)
					.val(value)
					.autocomplete({
					    delay: 0,
					    minLength: 0,
					    source: function (request, response) {
                        
					        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
					        response(select.children("option").map(function () {
					            var text = $(this).text();
					            if (this.value && (!request.term || matcher.test(text)))
					                return {
					                    label: text.replace(
											new RegExp(
												"(?![^&;]+;)(?!<[^<>]*)(" +
												$.ui.autocomplete.escapeRegex(request.term) +
												")(?![^<>]*>)(?![^&;]+;)", "gi"
											), "<strong>$1</strong>"),
					                    value: text,
					                    option: this
					                };
					        }));
					    },
					    select: function (event, ui) {
					        ui.item.option.selected = true;
					        //select.val( ui.item.option.value );
					        self._trigger("selected", event, {
					            item: ui.item.option
					        });
                           //########################### MODIFICA FILIPPO: evento di selezione corretta ###########################################
                           var $selOptFilo = ui.item.option.value;
                           wrapCallOnPage(select,$selOptFilo);
                           //######################################################################################################################
					    },
					    change: function (event, ui) {
                       
					        if (!ui.item) {
					            var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"),
									valid = false;
					                select.children("option").each(function () {
					                    if (this.value.match(matcher)) {
					                        this.selected = valid = true;
					                        return false;
					                    }
					            });
					            if (!valid) {
					                // remove invalid value, as it didn't match anything
					                $(this).val("");
                                    //ricarico l'evento
					                select.val("");
                                    //########################### MODIFICA FILIPPO : evento selezione errata #######################################
                                    wrapCallOnPage(select,"");
                                    //##############################################################################################################
					                return false;
					            }

					        }


					    } //fine evento
					})
					.addClass("ui-widget ui-widget-content ui-corner-left");

                    input.data("ui-autocomplete")._renderItem = function (ul, item) {
                        return $("<li></li>")
						.data("item.ui-autocomplete", item)
						.append("<a>" + item.label + "</a>")
						.appendTo(ul);
                    };

                    $("<button>&nbsp;</button>")
					.attr("tabIndex", -1)
					.attr("title", "Clicca per visualizzare l'elenco o ricerca i dati scrivendo parte della descrizione nella casella a fianco")
					.insertAfter(input)
					.button({
					    icons: {
					        primary: "ui-icon-triangle-1-s"
					    },
					    text: false
					})
					.removeClass("ui-corner-all")
					.addClass("ui-corner-right ui-button-icon")
					.click(function () {
                        
					    // close if already visible
					    if (input.autocomplete("widget").is(":visible")) {
					        input.autocomplete("close");
					        return false;
					    }

					    // pass empty string as value to search for, displaying all results
					    input.autocomplete("search", "");
					    input.focus();
					    return false; //altrimenti mi fa il submit della form!
					});
                }
            });
        })(jQuery);







        //2 caratteri
          
        (function ($) {
            $.widget("ui.combobox2", {
                _create: function () {
 
                    var self = this;
                    var select = this.element.hide(),
					selected = select.children(":selected"),
					value = selected.val() ? selected.text() : "";
                    var input = $("<input>")
					.insertAfter(select)
					.val(value)
					.autocomplete({
					    delay: 0,
					    minLength: 2,
					    source: function (request, response) {
                        
					        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
					        response(select.children("option").map(function () {
					            var text = $(this).text();
					            if (this.value && (!request.term || matcher.test(text)))
					                return {
					                    label: text.replace(
											new RegExp(
												"(?![^&;]+;)(?!<[^<>]*)(" +
												$.ui.autocomplete.escapeRegex(request.term) +
												")(?![^<>]*>)(?![^&;]+;)", "gi"
											), "<strong>$1</strong>"),
					                    value: text,
					                    option: this
					                };
					        }));
					    },
					    select: function (event, ui) {
					        ui.item.option.selected = true;
					        //select.val( ui.item.option.value );
					        self._trigger("selected", event, {
					            item: ui.item.option
					        });
                           //########################### MODIFICA FILIPPO: evento di selezione corretta ###########################################
                           var $selOptFilo = ui.item.option.value;
                           wrapCallOnPage(select,$selOptFilo);
                           //######################################################################################################################
					    },
					    change: function (event, ui) {
                       
					        if (!ui.item) {
					            var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"),
									valid = false;
					                select.children("option").each(function () {
					                    if (this.value.match(matcher)) {
					                        this.selected = valid = true;
					                        return false;
					                    }
					            });
					            if (!valid) {
					                // remove invalid value, as it didn't match anything
					                $(this).val("");
                                    //ricarico l'evento
					                select.val("");
                                    //########################### MODIFICA FILIPPO : evento selezione errata #######################################
                                    wrapCallOnPage(select,"");
                                    //##############################################################################################################
					                return false;
					            }

					        }


					    } //fine evento
					})
					.addClass("ui-widget ui-widget-content ui-corner-left");

                    input.data("ui-autocomplete")._renderItem = function (ul, item) {
                        return $("<li></li>")
						.data("item.ui-autocomplete", item)
						.append("<a>" + item.label + "</a>")
						.appendTo(ul);
                    };

                    $("<button>&nbsp;</button>")
					.attr("tabIndex", -1)
					.attr("title", "Clicca per visualizzare l'elenco o ricerca i dati scrivendo parte della descrizione nella casella a fianco")
					.insertAfter(input)
					.button({
					    icons: {
					        primary: "ui-icon-triangle-1-s"
					    },
					    text: false
					})
					.removeClass("ui-corner-all")
					.addClass("ui-corner-right ui-button-icon")
					.click(function () {
                        
					    // close if already visible
					    if (input.autocomplete("widget").is(":visible")) {
					        input.autocomplete("close");
					        return false;
					    }

					    // pass empty string as value to search for, displaying all results
					    input.autocomplete("search", "");
					    input.focus();
					    return false; //altrimenti mi fa il submit della form!
					});
                }
            });
        })(jQuery);