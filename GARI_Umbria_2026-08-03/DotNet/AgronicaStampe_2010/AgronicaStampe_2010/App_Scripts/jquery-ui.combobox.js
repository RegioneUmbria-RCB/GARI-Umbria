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

                    input.data("autocomplete")._renderItem = function (ul, item) {
                        return $("<li></li>")
						.data("item.autocomplete", item)
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