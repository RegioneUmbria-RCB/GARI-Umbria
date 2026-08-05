//variabili globali
var nascondi = "Piva|SA_COD|APPEZZA|ID_REG|CUL_COD";

//fine variabili globali


function TipoExport() {
    //return $('#' + cRadioButtonListEsporta + ' input:checked').val();
    return $('input[name=list_esporta]:checked', '#RadioButtonListEsporta').val();
}

function nascondiColonne() {
    var nascondi1 = nascondi.split ("|");
    for (var i = 0; i <  nascondi1.length; i++)    
    {
        nascondiColonnaTabellaByName("#tabellaEsitoRicerca", nascondi1[i]);	
    }
}

function media(jSData, colonnaDatiXMedia, colonnaXGroupBy) {
}

function avg(MyData){ 
    var sum = 0;
    for(var i = 0; i < MyData.length; i++){
        sum += parseInt(MyData[i], 10); //don't forget to add the base 
    }

    var avg = sum/MyData.length;
    return avg;
};

function valida_ricerca() {
    var flag = true;

    // Azzero tutte le label custom_val
    $("#more_search").find('select').each(function (i, obj) {
        $(this).css('border', '1px solid #ccc');
        $(this).parent().children().css('border-color', '#ccc');
        $(this).parent().children('label.error').remove();
    });


    switch (TipoExport()) {
        case "5":
            if ($('#'+azienda).val() == "null") {
                $('#' + azienda).parent().append('<label id="' + azienda + '-error" class="custom_val error" for="' + azienda + '>">Il campo deve essere compilato</label>');
                $('#' + azienda).parent().children(".input-group").css('border', '1px solid #D41E1A');
                flag = false;
            }
            break;
        case "6":
            if ($('#' + varieta).val() == "null") {
                $('#' + varieta).parent().append('<label id="' + varieta + '-error" class="custom_val error" for="' + varieta + '">Il campo deve essere compilato</label>');
                $('#' + varieta).parent().children(".input-group").css('border', '1px solid #D41E1A');
                flag = false;
            }
            break;
        case "7":
            if ($('#' + varieta).val() == "null") {
                $('#' + varieta).parent().append('<label id="' + varieta + '-error" class="custom_val error" for="' + varieta + '">Il campo deve essere compilato</label>');
                $('#' + varieta).parent().children(".input-group").css('border', '1px solid #D41E1A');
                flag = false;
            }
            if ($('#' + tecnico).val() == "null") {
                $('#' + tecnico).parent().append('<label id="' + tecnico + '-error" class="custom_val error" for="' + tecnico + '">Il campo deve essere compilato</label>');
                $('#' + tecnico).parent().children(".input-group").css('border', '1px solid #D41E1A');
                flag = false;
            }
            break;
    }

    return flag;

}
