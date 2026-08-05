
//DOCUMENT READY
$(document).ready(function () {
    docR();
});

async function docR() {    
    $.logThis("DocReady: INIZIO");

    var dtDatiAzienda = await LeggiRiepilogoDatiAzienda();
    SetCampiRiepilogoDatiAzienda(dtDatiAzienda);

    await ddlAnno_Load();

    $.logThis("DocReady: FINE");
}


async function ddlAnno_Load() {
    $('#ddlAnno').kendoDropDownList({
        filter: "contains",
        dataSource: {
            transport: {
                read: LeggiDdlAnno
            }
        },
        dataTextField: "Anno",
        dataValueField: "Anno",
        mapValueTo: "dataItem",
        autoWidth: true,
        dataBound: ddlAnno_OnDataBound        
    });
}

function ddlAnno_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 0) {
        //$('#ddlAnno').data("kendoDropDownList").enable(false);
        //HideCampiRiepilogoDatiCarburanti();
    } else if (ds.length == 1) {
        this.select(0);
        ddlAnno.onchange();
    } else if (QS_Anno != "") {
        this.value(QS_Anno);
        if (this.selectedIndex === -1) {
            this.select(0);
        }
        ddlAnno.onchange();
    }
}

async function ddlAnno_Change(loop) {
    var dtDatiCarburanti = await LeggiRiepilogoDatiCarburanti();
    SetCampiRiepilogoDatiCarburanti(dtDatiCarburanti);
}
