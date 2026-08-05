function CaricaKendo_Particelle(param) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            paramPart: param
        });

        ajaxAgronica("Appezzamento_Edit.aspx/CaricaKendo_Particelle",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);
    });
}