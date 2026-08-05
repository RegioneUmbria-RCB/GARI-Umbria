var diBootstrapMasterResx = [{}];
var resxArrPathMaster = [
    //"App_GlobalResources/AgronicaAgenda_2010.resx",
    //"Master/App_LocalResources/AgendaBootstrap.Master.resx"
];



//DOCUMENT READY
jQuery(function () {

    $('.datepicker').datepicker({ format: 'dd/mm/yyyy', autoclose: true });

    $('.kendoDatePicker').kendoDatePicker();

    if (Array.isArray(resxArrPathMaster) && resxArrPathMaster.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPathMaster.forEach(function (resxSinglePath) {
            diBootstrapMasterResx.unshift(readResxFile(resxSinglePath, "Master"));
        });
    }

    try {

        AgroMeteoLatitudine = masterAgroMeteoLatitudine;
        AgroMeteoLongitudine = masterAgroMeteoLongitudine;
        AgroMeteoDescrizione = masterAgroMeteoDescrizione;

    } catch (e) {

    }


});