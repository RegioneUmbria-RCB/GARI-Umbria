function ottieniCAP(ISTAT_Prov, ISTAT_Com, callback) {
        var parametri = kendo.stringify({
            "ISTAT_Prov": ISTAT_Prov,
            "ISTAT_Com": ISTAT_Com
        });

        ajaxAgronica("Impresa_Edit.aspx/OttieniCAP",
            parametri,
            function (risposta) {
                callback(risposta);
            }, null, null, false);
}