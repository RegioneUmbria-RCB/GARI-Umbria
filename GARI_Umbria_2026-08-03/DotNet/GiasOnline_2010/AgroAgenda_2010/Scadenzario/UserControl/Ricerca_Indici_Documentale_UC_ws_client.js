

//////////////////////////////////////////////////////////
// Griglia Imputazione
//////////////////////////////////////////////////////////

var indirizzohttp = "./Scad_Anagrafiche.aspx";

function EmptyRead(options) { }
function EmptySubmit(options) { }

function Ricerca_Indici(options, parametriPerLettura) {

    ajaxAgronicaSync(indirizzohttp + "/Ricerca_Alert_Indici",
            "{}",
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);
            }, null);
    
}







function Elimina_Indice(ID_Indice) {

        var risultatoCancellazione;
        
        var param = "{piva: '" + $(cIdPiva).val() + "'," +            
            " id_indice: " + ID_Indice + "}";

            ajaxAgronicaSync(indirizzohttp + "/Delete_Alert_Indice",
            param,
            false,
            function (risposta) {
                risultatoCancellazione = risposta.Errore;
            },
            function (risposta) {
                risultatoCancellazione = risposta.Errore;
            });

        return risultatoCancellazione;
   
}
