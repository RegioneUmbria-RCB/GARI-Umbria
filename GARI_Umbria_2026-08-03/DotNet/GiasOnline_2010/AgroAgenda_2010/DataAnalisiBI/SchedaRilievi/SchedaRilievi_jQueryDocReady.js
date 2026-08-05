

$(document).ready(function () {

    $(".export_excel").hide();

    $("#btn_ricerca").click(function () {
        //eseguiRicerca();
        //$(".export_excel").show();

        letturaDatiRilievi();

    });


    function formattedDate(date, sep) {

        var d = new Date(date || Date.now()),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        return [day, month, year].join(sep);

    }



    var dateY = new Date();
    var dateStart = new Date(dateY.getFullYear(), 0, 1);
    $("#txt_DataDa").val(formattedDate(dateStart, '/'));
    $("#txt_DataA").val(formattedDate(dateY, '/'));



    $("#anteprima").click(function () {
        demo();
    });


    //$('.datepicker').datepicker({ format: 'dd/mm/yyyy', autoclose: true })


    if (lhideTabSelection)
        $("#tabSX").hide();

    $("#exportXls").click(function () {
        var datiEsportati = Esporta();
        var contentType = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,'
        var blob = new Blob([datiEsportati], { 'type': contentType });
        var data = new Date(Date.now());
        var nomeFile = 'AnalisiDatiCura_' + data.getFullYear() + '-' + data.getMonth() + 1 + '-' + data.getDate() + '_' + data.getHours() + '-' + data.getMinutes() + '-' + data.getSeconds() + '-' + data.getMilliseconds() + '.xls';

        var isIE = !!document.documentMode;
        if (isIE) { //se è IE
            navigator.msSaveOrOpenBlob(blob, nomeFile)
        }
        else { //Se sono gli altri...Chrome...
            var aLink = document.createElement('a');
            var evt = document.createEvent("HTMLEvents");
            evt.initEvent("click", true, false);
            aLink.href = window.URL.createObjectURL(blob);
            aLink.download = nomeFile
            aLink.dispatchEvent(evt);
        }

    });
});