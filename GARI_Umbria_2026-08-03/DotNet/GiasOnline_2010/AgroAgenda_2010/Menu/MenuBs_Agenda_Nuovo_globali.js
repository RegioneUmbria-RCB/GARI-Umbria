var listaIdAgendaInPage = "";
var countListaIdAgendaInPage = 0;
//Get lsita operazioni per il controllo del lavCod per i costi 
var grigliaOperazioni = caricaGrigliaOperazioniWS("LAV_COD < 1000 AND (GRU_OP in (3,4,5) or Lav_cod = " + $("input[name$='hf_LavCodRaccolta']").val() + ")");