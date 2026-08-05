         

//VARIABILI GLOBALI
var UtenteAbilitatoLettura;


//FINE VARIABILI GLOBALI

function formattedDate(date, sep) {

    var d = new Date(date || Date.now()),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [day, month, year].join(sep);

}