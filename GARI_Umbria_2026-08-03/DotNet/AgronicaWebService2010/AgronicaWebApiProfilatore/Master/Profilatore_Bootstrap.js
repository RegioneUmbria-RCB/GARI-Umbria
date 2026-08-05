

//utilità varie.

function jQueryAddSelect(selector, selectValues) {

    $.each(selectValues, function (key, value) {
        $(selector)
            .append($("<option></option>")
                .attr("value", key)
                .text(value));
    });

}



function scrollTop() {
    try {
        $("html, body").animate({ scrollTop: 0 }, "slow");
    }
    catch (e) {
    }
}


function alertOk(messaggio) {

    scrollTop();
    MessaggioTuttoOK_Bootstrap(messaggio, "DIV_Messaggi");
}
