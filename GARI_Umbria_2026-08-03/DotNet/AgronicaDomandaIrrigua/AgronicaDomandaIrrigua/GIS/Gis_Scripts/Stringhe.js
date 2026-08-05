var stringhe = {
    startsWith: function (str, find) {
        return str.indexOf(find) == 0;
    },
    contains: function (str, find) {
        return str.indexOf(find) >= 0;
    }
    ,
    formattaStringaInfo: function (etichetta, valore) {
    if (valore != "")
        return " " + etichetta + ": " + valore + " <br/> ";
    else
        return "";
}
}



