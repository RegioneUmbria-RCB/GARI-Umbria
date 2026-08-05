using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.OutData.Pratiche
{
  /// <summary>
  /// Configurazione pratiche corrente di un utente, restituita dall'endpoint
  /// GET Profilazione/ConfigurazionePraticheUtente.
  /// </summary>
  public class ConfigurazionePraticheUtente
  {
    /// <summary>Username dell'utente a cui appartiene la configurazione.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Operatore logico applicato alle pratiche (AND / OR).</summary>
    public string OperatoreFiltri { get; set; } = "OR";

    /// <summary>True se il filtro per pratiche è attivo per l'utente.</summary>
    public bool FiltroPraticheAttivo { get; set; }

    /// <summary>Pratiche configurate per l'utente, arricchite con i dati del catalogo.</summary>
    public List<PraticaConfigurataUtente> Pratiche { get; set; } = new List<PraticaConfigurataUtente>();
  }

  /// <summary>
  /// Singola pratica presente nel profilo utente, arricchita con i dati del catalogo Servizi_Pratiche.
  /// </summary>
  public class PraticaConfigurataUtente
  {
    /// <summary>Codice servizio (Servizi.Servizio_Cod).</summary>
    public int Servizio_Cod { get; set; }

    /// <summary>Descrizione del servizio dal catalogo.</summary>
    public string ServizioDescrizione { get; set; } = string.Empty;

    /// <summary>Se true, applica anche il filtro sulla validità temporale della pratica.</summary>
    public bool ConsideraValiditaTemporale { get; set; }

    /// <summary>Data inizio validità del servizio nel catalogo.</summary>
    public DateTime? DataValiditaInizio { get; set; }

    /// <summary>Data fine validità del servizio nel catalogo.</summary>
    public DateTime? DataValiditaFine { get; set; }

    /// <summary>True se il servizio è attualmente valido (Validita_Fine >= oggi).</summary>
    public bool IsValida { get; set; }

    /// <summary>True: la pratica è selezionata nel profilo utente.</summary>
    public bool Selected { get; set; }
  }
}
