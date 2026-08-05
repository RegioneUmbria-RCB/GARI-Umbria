var indirizzohttp = "./TestAPI.aspx"; //window.location.href 

function RegistraIrrigazione() {
    let url = indirizzohttp + "/RegistraIrrigazione";
    ajaxAgronica(url,
        JSON.stringify({ Piva: $("#Txt_Piva_Azienda").val(), Id_Agenda: $("#Txt_Id_Agenda").val() }),
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa);
            if (res.Status == 0) kendo.alert("Irrigazione registrata correttamente " + res.Message)
            else kendo.alert("ERRORE durante la registrazione " + res.Message);
        },
        function (risposta) {
            kendo.alert("ERRORE API Registrazione Irrigazione");
        }
    );
}

function RegistraUtente() {
    let url = indirizzohttp + "/RegistraUtente";
    ajaxAgronica(url,
        JSON.stringify({ Username: $("#Txt_UserName").val(), Password: $("#Txt_Password").val(), Id_User: $("#Txt_Id_User").val()  }),
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa);
            if (res.Status == 0) kendo.alert("Utente registrato correttamente " + res.Message)
            else kendo.alert("ERRORE durante la registrazione " + res.Message);
        },
        function (risposta) {
            kendo.alert("ERRORE API Registrazione Utente");
        }
    );
}

function RegistraImpresa() {
    let url = indirizzohttp + "/RegistraImpresa";
    ajaxAgronica(url,
        JSON.stringify({ Username: $("#Txt_UserName").val(), Piva: $("#Txt_Piva").val(), RagSoc: $("#Txt_RagSoc").val(), Cuaa: $("#Txt_Cuaa").val(), Id_Farm: $("#Txt_Id_Farm").val()  }),
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa);
            if (res.Status == 0) kendo.alert("Impresa registrata correttamente " + res.Message)
            else kendo.alert("ERRORE durante la registrazione " + res.Message);
        },
        function (risposta) {
            kendo.alert("ERRORE API Registrazione Impresa");
        }
    );
}

function RegistraAppezzamento() {
    let url = indirizzohttp + "/RegistraAppezzamento";
    ajaxAgronica(url,
        JSON.stringify({
            Username: $("#Txt_UserName").val(), Descrizione: $("#Txt_Desc_Plot").val(),
            Latitudine: $("#Txt_Latitudine").val(), Longitudine: $("#Txt_Longitudine").val(),
            Superficie: $("#Txt_Superficie").val(), Pendenza: $("#Txt_Pendenza").val(),
            Id_Farm: $("#Txt_Id_Farm").val(), Id_Plot: $("#Txt_Id_Plot").val()
        }),
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa);
            if (res.Status == 0) kendo.alert("Appezzamento registrato correttamente " + res.Message)
            else kendo.alert("ERRORE durante la registrazione " + res.Message);
        },
        function (risposta) {
            kendo.alert("ERRORE API Registrazione Appezzamento");
        }
    );
}

function RegistraImpianto() {
    let url = indirizzohttp + "/RegistraImpianto";
    ajaxAgronica(url,
        JSON.stringify({
            Username: $("#Txt_UserName").val(), Descrizione: $("#Txt_Desc_Crop").val(),
            Coltura: $("#Txt_Coltura").val(), Ciclo: $("#Txt_Ciclo").val(),
            Data_Inizio: $("#Txt_Data_Inizio").val(), Data_Raccolta: $("#Txt_Data_Raccolta").val(),
            Su_Fila: $("#Txt_Su_Fila").val(), Tra_Fila: $("#Txt_Tra_Fila").val(),
            Conduzione: $("#Txt_Conduzione").val(), Vigore: $("#Txt_Vigore").val(),
            Id_Plot: $("#Txt_Id_Plot_Crop").val(), Id_Crop: $("#Txt_Id_Crop").val()
        }),
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa);
            if (res.Status == 0) kendo.alert("Impianto registrato correttamente " + res.Message)
            else kendo.alert("ERRORE durante la registrazione " + res.Message);
        },
        function (risposta) {
            kendo.alert("ERRORE API Registrazione Impianto");
        }
    );
}