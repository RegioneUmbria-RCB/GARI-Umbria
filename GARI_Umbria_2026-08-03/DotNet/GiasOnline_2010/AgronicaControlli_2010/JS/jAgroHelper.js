
//jAgroHelper


String.prototype.aEndsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};


function DatiFormToJson(FormDaLeggere) {

    var rval = "{ ";

    $(FormDaLeggere + " input").each(function () {

        rval = rval + '"' + $(this).attr("id") + '": "' + $(this).val() + '", ';

    });

    rval = rval.substring(0, rval.length - 2)

    return rval + " }";
}

