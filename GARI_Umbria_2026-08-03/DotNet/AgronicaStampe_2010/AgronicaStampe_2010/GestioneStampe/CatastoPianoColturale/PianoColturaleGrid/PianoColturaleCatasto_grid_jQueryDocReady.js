var customGrid = null;
var firstTime = true;
var lastReport = "";
var tabReport = 890; //Codice impostazione utente


$(document).ready(function () {
    docReady();
    $("#btn_salvaReport").click(function () { gestioneBtnSalva(); });
    $("#btn_eliminaReport").click(function () { gestioneBtnCancella(); });

})

async function docReady() {
    WaitFrame.show();
    Cmb_Report = await get_Cmb_Report("Cmb_Report", tabReport);
    setKendoDropDownHeight();

}

