Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Alert_Importazioni
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Patentini_Nuovi(ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_Patentini_Nuovi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            'StrSQL.AppendLine(" SELECT DISTINCT ru.piva, i.rag_soc, ru.Cod_Contatto, c.Nome, c.Cognome, ru.Patentino, ru.Data_Rilascio_Patentino, ru.Data_Scadenza_Patentino, ru.ente_di_rilascio ")
            'StrSQL.AppendLine(" FROM risorse_umane ru ")
            'StrSQL.AppendLine(" INNER JOIN contatti c ON ru.cod_contatto=c.cod_contatto AND ru.piva=c.piva ")
            'StrSQL.AppendLine(" INNER JOIN imprese i ON ru.piva=i.piva ")
            'StrSQL.AppendLine(" WHERE ru.Data_Scadenza_Patentino <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " ")
            'StrSQL.AppendLine(" AND ru.Data_Scadenza_Patentino <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & " ")
            'StrSQL.AppendLine(" AND ru.Data_Scadenza_Patentino IS NOT NULL ")
            'StrSQL.AppendLine(" AND GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine ")
            'StrSQL.AppendLine(" AND NOT EXISTS( ")
            'StrSQL.AppendLine(" 	SELECT el.data_scadenza, el.descrizione_scadenza, el.note, en.piva, en.cod_contatto  ")
            'StrSQL.AppendLine(" 	FROM alert_elenco el ")
            'StrSQL.AppendLine(" 	INNER JOIN alert_entita en ")
            'StrSQL.AppendLine(" 	ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            'StrSQL.AppendLine(" 	WHERE en.Piva=ru.piva AND en.Cod_Contatto=ru.Cod_Contatto ")
            'StrSQL.AppendLine(" 	AND ID_Tipologia = -1 ")
            'StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" SELECT DISTINCT ru.piva, i.rag_soc, ru.Cod_Contatto, c.Nome, c.Cognome, ru.Patentino, ru.Data_Rilascio_Patentino, ru.Data_Scadenza_Patentino, ru.ente_di_rilascio, ")
            StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note,el.ID_Tipologia ")
            StrSQL.AppendLine(" FROM risorse_umane ru ")
            StrSQL.AppendLine(" INNER JOIN contatti c ON ru.cod_contatto=c.cod_contatto AND ru.piva=c.piva ")
            StrSQL.AppendLine(" INNER JOIN imprese i ON ru.piva=i.piva ")
            StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.Piva=ru.piva AND en.Cod_Contatto=ru.Cod_Contatto ")
            StrSQL.AppendLine(" LEFT JOIN alert_elenco el ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            StrSQL.AppendLine(" WHERE ru.Data_Scadenza_Patentino <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " ")
            StrSQL.AppendLine(" AND ru.Data_Scadenza_Patentino <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & " ")
            StrSQL.AppendLine(" AND ru.Data_Scadenza_Patentino IS NOT NULL ")
            StrSQL.AppendLine(" AND GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine ")
            StrSQL.AppendLine(" AND el.ID_Tipologia IS NULL ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ru.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ru.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'PER ORA I MODIFICATI SONO TUTTI QUELLI CHE HO GIA' IN QUANTO DEVO AGGIORNARE LA DESCRIZIONE
    Public Function Leggi_Patentini_Modificati(ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_Patentini_Modificati()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            'StrSQL.AppendLine(" SELECT DISTINCT ru.piva, i.rag_soc, ru.Cod_Contatto, c.Nome, c.Cognome, ru.Patentino, ru.Data_Rilascio_Patentino, ru.Data_Scadenza_Patentino, ru.ente_di_rilascio ")
            'StrSQL.AppendLine(" FROM risorse_umane ru ")
            'StrSQL.AppendLine(" INNER JOIN contatti c ON ru.cod_contatto=c.cod_contatto AND ru.piva=c.piva ")
            'StrSQL.AppendLine(" INNER JOIN imprese i ON ru.piva=i.piva ")
            'StrSQL.AppendLine(" WHERE ru.Data_Scadenza_Patentino <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " ")
            'StrSQL.AppendLine(" AND ru.Data_Scadenza_Patentino <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & " ")
            'StrSQL.AppendLine(" AND ru.Data_Scadenza_Patentino IS NOT NULL ")
            'StrSQL.AppendLine(" AND GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine ")
            'StrSQL.AppendLine(" AND EXISTS( ")
            'StrSQL.AppendLine(" 	SELECT el.data_scadenza, el.descrizione_scadenza, el.note, en.piva, en.cod_contatto  ")
            'StrSQL.AppendLine(" 	FROM alert_elenco el ")
            'StrSQL.AppendLine(" 	INNER JOIN alert_entita en ")
            'StrSQL.AppendLine(" 	ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            'StrSQL.AppendLine(" 	WHERE en.Piva=ru.piva AND en.Cod_Contatto=ru.Cod_Contatto ")
            'StrSQL.AppendLine(" 	AND ID_Tipologia = -1 ")
            'StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" SELECT DISTINCT ru.piva, i.rag_soc, ru.Cod_Contatto, c.Nome, c.Cognome, ru.Patentino, ru.Data_Rilascio_Patentino, ru.Data_Scadenza_Patentino, ru.ente_di_rilascio, ")
            StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note, el.ID_Tipologia, el.ID_Alert_Entita ")
            StrSQL.AppendLine(" FROM risorse_umane ru ")
            StrSQL.AppendLine(" INNER JOIN contatti c ON ru.cod_contatto=c.cod_contatto AND ru.piva=c.piva ")
            StrSQL.AppendLine(" INNER JOIN imprese i ON ru.piva=i.piva ")
            StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.Piva=ru.piva AND en.Cod_Contatto=ru.Cod_Contatto ")
            StrSQL.AppendLine(" LEFT JOIN alert_elenco el ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            StrSQL.AppendLine(" WHERE ru.Data_Scadenza_Patentino <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " ")
            StrSQL.AppendLine(" AND ru.Data_Scadenza_Patentino <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & " ")
            StrSQL.AppendLine(" AND ru.Data_Scadenza_Patentino IS NOT NULL ")
            StrSQL.AppendLine(" AND GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine ")
            StrSQL.AppendLine(" AND el.ID_Tipologia=" & TipiEnumerativi.enum_ID_Area_Tipologia.Patentino_trattamenti)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ru.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ru.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'cancellati se: contatto non più valido, contatto non più esistente
    'impresa chiusa, impresa non più esistente
    'data patentino ritornata 31/12/2100 o vuota
    'NB: se le risorse umane non hanno i dati del patentino allineati per la stessa piva-cod_contatto, cancella.. DA SISTEMARE
    Public Function Leggi_Patentini_Cancellati(ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_Patentini_Cancellati()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            'StrSQL.AppendLine(" SELECT el.data_scadenza, el.descrizione_scadenza, el.note, en.piva, en.cod_contatto ")
            'StrSQL.AppendLine(" FROM alert_elenco el ")
            'StrSQL.AppendLine(" INNER JOIN alert_entita en ")
            'StrSQL.AppendLine(" ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            'StrSQL.AppendLine(" WHERE ID_Tipologia=-1 ")
            'StrSQL.AppendLine(" AND NOT EXISTS( ")
            'StrSQL.AppendLine(" 	SELECT ru.piva, i.rag_soc, ru.Cod_Contatto, c.Nome, c.Cognome, ru.Patentino, ru.Data_Rilascio_Patentino, ru.Data_Scadenza_Patentino, ru.ente_di_rilascio ")
            'StrSQL.AppendLine(" 	FROM risorse_umane ru ")
            'StrSQL.AppendLine(" 	INNER JOIN contatti c ON ru.cod_contatto=c.cod_contatto AND ru.piva=c.piva ")
            'StrSQL.AppendLine(" 	INNER JOIN imprese i ON ru.piva=i.piva ")
            'StrSQL.AppendLine(" 	WHERE ru.Data_Scadenza_Patentino <> '31/12/2100' AND ru.Data_Scadenza_Patentino <> '01/01/1900' AND ru.Data_Scadenza_Patentino IS NOT NULL ")
            'StrSQL.AppendLine(" 	AND GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine ")
            'StrSQL.AppendLine(" 	AND en.Piva=ru.piva AND en.Cod_Contatto=ru.Cod_Contatto ")
            'StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" SELECT DISTINCT ru.piva, ru.Cod_Contatto, c.Nome, c.Cognome, ru.Patentino, ru.Data_Rilascio_Patentino, ru.Data_Scadenza_Patentino, ru.ente_di_rilascio, ")
            StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note, el.ID_Tipologia")
            StrSQL.AppendLine(" FROM alert_elenco el ")
            StrSQL.AppendLine(" INNER JOIN alert_entita en ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            StrSQL.AppendLine(" LEFT JOIN contatti c ON en.Piva=c.piva AND en.Cod_Contatto=c.Cod_Contatto ")
            StrSQL.AppendLine(" LEFT JOIN risorse_umane ru ON ru.cod_contatto=c.cod_contatto AND ru.piva=c.piva ")
            StrSQL.AppendLine(" WHERE el.ID_Tipologia=" & TipiEnumerativi.enum_ID_Area_Tipologia.Patentino_trattamenti)
            StrSQL.AppendLine(" AND (ru.Validita_Inizio IS NULL OR GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine) ")
            StrSQL.AppendLine(" AND (ru.Data_Scadenza_Patentino IS NULL OR ru.Data_Scadenza_Patentino = " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " OR ru.Data_Scadenza_Patentino = " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & ") ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   el.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   el.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function Leggi_AnalisiTerreno_Nuovi(ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_AnalisiTerreno_Nuovi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            'StrSQL.AppendLine(" SELECT ant.Analisi_Testata_Cod, ant.Analisi_Testata_Des, ant.Analisi_Certificato_Cod, ant.Analisi_Testata_Data_Inizio, ant.Analisi_Testata_Data_Fine,")
            'StrSQL.AppendLine(" antext.Analisi_Entita_Cod, antext.Piva, antext.Sa_Cod, antext.Campo_Cod, antext.Appezza, antext.Id_Imp, antext.Fabbricato_Cod, antext.Prov, antext.Com, antext.Sezione, antext.Foglio, antext.Numero, antext.Subalterno, antext.ID_Oggetto_Grafico,")
            'StrSQL.AppendLine(" i.rag_soc, ca.sa_nome, c.Campo_Des, app.APP_NOME, ri.Validita_Inizio as imp_Validita_Inizio, sv.Veg_Des, cul.Cul_Des, f.Fabbricato_Des, ")
            'StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note, el.ID_Tipologia ")
            'StrSQL.AppendLine(" FROM Analisi_Testata ant ")
            'StrSQL.AppendLine(" INNER JOIN Analisi_EntitaxTestata antext ON ant.Analisi_SuperUser = antext.Analisi_SuperUser AND ant.Analisi_Testata_Cod = antext.Analisi_Testata_Cod")
            'StrSQL.AppendLine(" INNER JOIN imprese i ON antext.piva=i.piva ")
            'StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca ON antext.piva=ca.piva AND antext.Sa_Cod=ca.sa_cod ")
            'StrSQL.AppendLine(" LEFT JOIN campi c ON antext.piva=c.piva AND antext.Sa_Cod=c.sa_cod AND antext.Campo_Cod=c.Campo_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN Appezzamento app ON antext.piva=app.piva AND antext.Sa_Cod=app.sa_cod AND antext.Appezza=app.APPEZZA ")
            'StrSQL.AppendLine(" LEFT JOIN Reg_Impianti ri ON antext.piva=ri.piva AND antext.Sa_Cod=ri.sa_cod AND antext.Appezza=ri.APPEZZA AND antext.Id_Imp=ri.ID_REG ")
            'StrSQL.AppendLine(" LEFT JOIN Cultivar cul ON ri.CUL_COD=cul.Cul_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN SpecieVegetali sv ON cul.Veg_Cod=sv.Veg_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN Fabbricati f ON antext.piva=f.piva AND antext.Sa_Cod=f.sa_cod AND antext.Fabbricato_Cod=f.Fabbricato_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.Piva=antext.piva AND en.Analisi_Testata_Cod=antext.Analisi_Testata_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN alert_elenco el ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            'StrSQL.AppendLine(" WHERE ant.Analisi_Testata_Data_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO))
            'StrSQL.AppendLine(" AND ant.Analisi_Testata_Data_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE))
            'StrSQL.AppendLine(" AND ant.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            'StrSQL.AppendLine(" AND ant.Analisi_Testata_Data_Fine IS NOT NULL ")
            'StrSQL.AppendLine(" AND ant.Analisi_Testata_Tipo = 1 ")
            ''StrSQL.AppendLine(" AND GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine ")
            'StrSQL.AppendLine(" AND (ri.Validita_Inizio IS NULL OR (ant.Analisi_Testata_Data_Inizio BETWEEN ri.Validita_Inizio AND ri.Validita_Fine)) ")
            'StrSQL.AppendLine(" AND el.ID_Tipologia IS NULL ")

            StrSQL.AppendLine(" SELECT ant.Analisi_Testata_Cod, ant.Analisi_Testata_Des, ant.Analisi_Certificato_Cod, ant.Analisi_Testata_Data_Inizio, ant.Analisi_Testata_Data_Fine,")
            StrSQL.AppendLine(" (SELECT TOP 1 piva FROM Analisi_EntitaxTestata antext WHERE ant.Analisi_SuperUser = antext.Analisi_SuperUser AND ant.Analisi_Testata_Cod = antext.Analisi_Testata_Cod) AS piva,")
            'StrSQL.AppendLine(" antext.Analisi_Entita_Cod, antext.Piva, antext.Sa_Cod, antext.Campo_Cod, antext.Appezza, antext.Id_Imp, antext.Fabbricato_Cod, antext.Prov, antext.Com, antext.Sezione, antext.Foglio, antext.Numero, antext.Subalterno, antext.ID_Oggetto_Grafico,")
            StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note, el.ID_Tipologia ")
            StrSQL.AppendLine(" FROM Analisi_Testata ant ")
            'StrSQL.AppendLine(" INNER JOIN Analisi_EntitaxTestata antext ON ant.Analisi_SuperUser = antext.Analisi_SuperUser AND ant.Analisi_Testata_Cod = antext.Analisi_Testata_Cod")
            'StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.Piva=antext.piva AND en.Analisi_Testata_Cod=antext.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.PivaSuperUser = ant.Analisi_SuperUser AND en.Analisi_Testata_Cod=ant.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" LEFT JOIN alert_elenco el ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            StrSQL.AppendLine(" WHERE ant.Analisi_Testata_Data_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO))
            StrSQL.AppendLine(" AND ant.Analisi_Testata_Data_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE))
            StrSQL.AppendLine(" AND ant.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND ant.Analisi_Testata_Data_Fine IS NOT NULL ")
            StrSQL.AppendLine(" AND ant.Analisi_Testata_Tipo = 1 ")
            StrSQL.AppendLine(" AND el.ID_Tipologia IS NULL ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ant.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ant.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'PER ORA I MODIFICATI SONO TUTTI QUELLI CHE HO GIA' IN QUANTO DEVO AGGIORNARE LA DESCRIZIONE
    Public Function Leggi_AnalisiTerreno_Modificati(ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_AnalisiTerreno_Modificati()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            'StrSQL.AppendLine(" SELECT ant.Analisi_Testata_Cod, ant.Analisi_Testata_Des, ant.Analisi_Certificato_Cod, ant.Analisi_Testata_Data_Inizio, ant.Analisi_Testata_Data_Fine,")
            'StrSQL.AppendLine(" antext.Analisi_Entita_Cod, antext.Piva, antext.Sa_Cod, antext.Campo_Cod, antext.Appezza, antext.Id_Imp, antext.Fabbricato_Cod, antext.Prov, antext.Com, antext.Sezione, antext.Foglio, antext.Numero, antext.Subalterno, antext.ID_Oggetto_Grafico,")
            'StrSQL.AppendLine(" i.rag_soc, ca.sa_nome, c.Campo_Des, app.APP_NOME, ri.Validita_Inizio AS imp_Validita_Inizio, sv.Veg_Des, cul.Cul_Des, f.Fabbricato_Des, ")
            'StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note, el.ID_Tipologia, el.ID_Alert_Entita ")
            'StrSQL.AppendLine(" FROM Analisi_Testata ant ")
            'StrSQL.AppendLine(" INNER JOIN Analisi_EntitaxTestata antext ON ant.Analisi_SuperUser = antext.Analisi_SuperUser AND ant.Analisi_Testata_Cod = antext.Analisi_Testata_Cod")
            'StrSQL.AppendLine(" INNER JOIN imprese i ON antext.piva=i.piva ")
            'StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca ON antext.piva=ca.piva AND antext.Sa_Cod=ca.sa_cod ")
            'StrSQL.AppendLine(" LEFT JOIN campi c ON antext.piva=c.piva AND antext.Sa_Cod=c.sa_cod AND antext.Campo_Cod=c.Campo_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN Appezzamento app ON antext.piva=app.piva AND antext.Sa_Cod=app.sa_cod AND antext.Appezza=app.APPEZZA ")
            'StrSQL.AppendLine(" LEFT JOIN Reg_Impianti ri ON antext.piva=ri.piva AND antext.Sa_Cod=ri.sa_cod AND antext.Appezza=ri.APPEZZA AND antext.Id_Imp=ri.ID_REG ")
            'StrSQL.AppendLine(" LEFT JOIN Cultivar cul ON ri.CUL_COD=cul.Cul_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN SpecieVegetali sv ON cul.Veg_Cod=sv.Veg_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN Fabbricati f ON antext.piva=f.piva AND antext.Sa_Cod=f.sa_cod AND antext.Fabbricato_Cod=f.Fabbricato_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.Piva=antext.piva AND en.Analisi_Testata_Cod=antext.Analisi_Testata_Cod ")
            'StrSQL.AppendLine(" LEFT JOIN alert_elenco el ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            'StrSQL.AppendLine(" WHERE ant.Analisi_Testata_Data_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO))
            'StrSQL.AppendLine(" AND ant.Analisi_Testata_Data_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE))
            'StrSQL.AppendLine(" AND ant.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            'StrSQL.AppendLine(" AND ant.Analisi_Testata_Data_Fine IS NOT NULL ")
            'StrSQL.AppendLine(" AND ant.Analisi_Testata_Tipo = 1 ")
            ''StrSQL.AppendLine(" AND GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine ")
            'StrSQL.AppendLine(" AND (ri.Validita_Inizio IS NULL OR (ant.Analisi_Testata_Data_Inizio BETWEEN ri.Validita_Inizio AND ri.Validita_Fine)) ")
            'StrSQL.AppendLine(" AND el.ID_Tipologia=" & TipiEnumerativi.enum_ID_Area_Tipologia.Analisi_terreno)

            StrSQL.AppendLine(" SELECT ant.Analisi_Testata_Cod, ant.Analisi_Testata_Des, ant.Analisi_Certificato_Cod, ant.Analisi_Testata_Data_Inizio, ant.Analisi_Testata_Data_Fine,")
            'StrSQL.AppendLine(" antext.Analisi_Entita_Cod, antext.Piva, antext.Sa_Cod, antext.Campo_Cod, antext.Appezza, antext.Id_Imp, antext.Fabbricato_Cod, antext.Prov, antext.Com, antext.Sezione, antext.Foglio, antext.Numero, antext.Subalterno, antext.ID_Oggetto_Grafico,")
            StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note, el.ID_Tipologia, el.ID_Alert_Entita ")
            StrSQL.AppendLine(" FROM Analisi_Testata ant ")
            'StrSQL.AppendLine(" INNER JOIN Analisi_EntitaxTestata antext ON ant.Analisi_SuperUser = antext.Analisi_SuperUser AND ant.Analisi_Testata_Cod = antext.Analisi_Testata_Cod")
            'StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.Piva=antext.piva AND en.Analisi_Testata_Cod=antext.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.PivaSuperUser = ant.Analisi_SuperUser AND en.Analisi_Testata_Cod=ant.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" LEFT JOIN alert_elenco el ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            StrSQL.AppendLine(" WHERE ant.Analisi_Testata_Data_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO))
            StrSQL.AppendLine(" AND ant.Analisi_Testata_Data_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE))
            StrSQL.AppendLine(" AND ant.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND ant.Analisi_Testata_Data_Fine IS NOT NULL ")
            StrSQL.AppendLine(" AND ant.Analisi_Testata_Tipo = 1 ")
            StrSQL.AppendLine(" AND el.ID_Tipologia=" & TipiEnumerativi.enum_ID_Area_Tipologia.Analisi_terreno)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ant.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ant.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'cancellati se: contatto non più valido, contatto non più esistente
    'impresa chiusa, impresa non più esistente
    'data patentino ritornata 31/12/2100 o vuota
    'NB: se le risorse umane non hanno i dati del patentino allineati per la stessa piva-cod_contatto, cancella.. DA SISTEMARE
    Public Function Leggi_AnalisiTerreno_Cancellati(ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_AnalisiTerreno_Cancellati()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ant.Analisi_Testata_Cod, ant.Analisi_Testata_Des, ant.Analisi_Certificato_Cod, ant.Analisi_Testata_Data_Inizio, ant.Analisi_Testata_Data_Fine,")
            'StrSQL.AppendLine(" antext.Analisi_Entita_Cod, antext.Piva, antext.Sa_Cod, antext.Campo_Cod, antext.Appezza, antext.Id_Imp, antext.Fabbricato_Cod, antext.Prov, antext.Com, antext.Sezione, antext.Foglio, antext.Numero, antext.Subalterno, antext.ID_Oggetto_Grafico,")
            'StrSQL.AppendLine(" i.rag_soc, ca.sa_nome, c.Campo_Des, app.APP_NOME, ri.Validita_Inizio as imp_Validita_Inizio, sv.Veg_Des, cul.Cul_Des, f.Fabbricato_Des, ")
            StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note, el.ID_Tipologia ")
            StrSQL.AppendLine(" FROM alert_elenco el ")
            StrSQL.AppendLine(" INNER JOIN alert_entita en ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            'StrSQL.AppendLine(" LEFT JOIN Analisi_EntitaxTestata antext ON en.Piva=antext.piva AND en.Analisi_Testata_Cod=antext.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Analisi_Testata ant ON ant.Analisi_SuperUser = en.PivaSuperUser AND ant.Analisi_Testata_Cod = en.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" WHERE el.ID_Tipologia=" & TipiEnumerativi.enum_ID_Area_Tipologia.Analisi_terreno)
            'StrSQL.AppendLine(" AND (ru.Validita_Inizio IS NULL OR GETDATE() BETWEEN ru.Validita_Inizio AND ru.Validita_Fine) ")
            StrSQL.AppendLine(" AND (ant.Analisi_Testata_Data_Fine IS NULL OR ant.Analisi_Testata_Data_Fine = " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " OR ant.Analisi_Testata_Data_Fine = " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & "  AND en.allegati_documenti_cod=0) ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   el.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   el.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    Public Function Leggi_TaratureUgelli_Nuovi(ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_TaratureUgelli_Nuovi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT pm.piva, pm.Sa_Cod, pm.Mac_Cod, i.rag_soc, ISNULL(ca.sa_nome,'') as sa_nome, m.CLASS_DESC, pm.mac_des, ISNULL(d.Ditta_Des,'') as Ditta_Des, pm.Modello, pm.Validita_Taratura_Inizio, pm.Validita_Taratura_Fine, ")
            StrSQL.AppendLine(" el.data_scadenza, el.descrizione_scadenza, el.note,el.ID_Tipologia ")
            StrSQL.AppendLine(" FROM parco_macchine pm ")
            StrSQL.AppendLine(" INNER JOIN imprese i ON pm.piva=i.piva ")
            StrSQL.AppendLine(" INNER JOIN Macchine m ON m.CLASS_CODE=pm.Class_Code  ")
            StrSQL.AppendLine(" LEFT JOIN centri_aziendali ca on ca.piva=pm.piva AND ca.sa_cod=pm.sa_cod ")
            StrSQL.AppendLine(" LEFT JOIN ditte d on d.Ditta_Cod=pm.Ditta_Cod ")
            StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.Piva=pm.piva AND en.Sa_Cod=pm.Sa_Cod AND en.Mac_Cod=pm.Mac_Cod ")
            StrSQL.AppendLine(" LEFT JOIN alert_elenco el ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            StrSQL.AppendLine(" WHERE pm.Validita_Taratura_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " ")
            StrSQL.AppendLine(" AND pm.Validita_Taratura_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & " ")
            StrSQL.AppendLine(" AND pm.Validita_Taratura_Fine IS NOT NULL ")
            StrSQL.AppendLine(" AND GETDATE() BETWEEN pm.Validita_Inizio AND pm.Validita_Fine ")
            StrSQL.AppendLine(" AND el.ID_Tipologia IS NULL ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   pm.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   pm.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'PER ORA I MODIFICATI SONO TUTTI QUELLI CHE HO GIA' IN QUANTO DEVO AGGIORNARE LA DESCRIZIONE
    Public Function Leggi_TaratureUgelli_Modificati(ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_TaratureUgelli_Modificati()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT pm.piva, pm.Sa_Cod, pm.Mac_Cod, i.rag_soc, ISNULL(ca.sa_nome,'') as sa_nome, m.CLASS_DESC, pm.mac_des, ISNULL(d.Ditta_Des,'') as Ditta_Des, pm.Modello, pm.Validita_Taratura_Inizio, pm.Validita_Taratura_Fine, ")
            StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note ,el.ID_Tipologia, el.ID_Alert_Entita ")
            StrSQL.AppendLine(" FROM parco_macchine pm ")
            StrSQL.AppendLine(" INNER JOIN imprese i ON pm.piva=i.piva ")
            StrSQL.AppendLine(" INNER JOIN Macchine m ON m.CLASS_CODE=pm.Class_Code  ")
            StrSQL.AppendLine(" LEFT JOIN centri_aziendali ca on ca.piva=pm.piva AND ca.sa_cod=pm.sa_cod ")
            StrSQL.AppendLine(" LEFT JOIN ditte d on d.Ditta_Cod=pm.Ditta_Cod ")
            StrSQL.AppendLine(" LEFT JOIN alert_entita en ON en.Piva=pm.piva AND en.Sa_Cod=pm.Sa_Cod AND en.Mac_Cod=pm.Mac_Cod ")
            StrSQL.AppendLine(" LEFT JOIN alert_elenco el ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            StrSQL.AppendLine(" WHERE pm.Validita_Taratura_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " ")
            StrSQL.AppendLine(" AND pm.Validita_Taratura_Fine <> " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & " ")
            StrSQL.AppendLine(" AND pm.Validita_Taratura_Fine IS NOT NULL ")
            StrSQL.AppendLine(" AND GETDATE() BETWEEN pm.Validita_Inizio AND pm.Validita_Fine ")
            StrSQL.AppendLine(" AND el.ID_Tipologia=" & TipiEnumerativi.enum_ID_Area_Tipologia.Taratura_ugelli)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   pm.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   pm.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'cancellati se: contatto non più valido, contatto non più esistente
    'impresa chiusa, impresa non più esistente
    'data patentino ritornata 31/12/2100 o vuota
    'NB: se le risorse umane non hanno i dati del patentino allineati per la stessa piva-cod_contatto, cancella.. DA SISTEMARE
    Public Function Leggi_TaratureUgelli_Cancellati(ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_TaratureUgelli_Cancellati()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT pm.piva, pm.Sa_Cod, pm.Mac_Cod, pm.mac_des, pm.Modello, pm.Validita_Taratura_Inizio, pm.Validita_Taratura_Fine, ")
            StrSQL.AppendLine(" el.id_elenco, el.data_scadenza, el.descrizione_scadenza, el.note, el.ID_Tipologia")
            StrSQL.AppendLine(" FROM alert_elenco el ")
            StrSQL.AppendLine(" INNER JOIN alert_entita en ON el.PivaSuperUser=en.PivaSuperUser AND el.id_alert_entita=en.id_alert_entita ")
            StrSQL.AppendLine(" LEFT JOIN parco_macchine pm ON en.Piva=pm.piva AND en.Sa_Cod=pm.Sa_Cod AND en.Mac_Cod=pm.Mac_Cod ")
            StrSQL.AppendLine(" WHERE el.ID_Tipologia=" & TipiEnumerativi.enum_ID_Area_Tipologia.Taratura_ugelli)
            StrSQL.AppendLine(" AND (pm.Validita_Inizio IS NULL OR GETDATE() BETWEEN pm.Validita_Inizio AND pm.Validita_Fine) ")
            StrSQL.AppendLine(" AND (pm.Validita_Taratura_Fine IS NULL OR pm.Validita_Taratura_Fine = " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " OR pm.Validita_Taratura_Fine = " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & ")")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   el.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   el.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_ContrattiAffitto_Nuovi(ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_ContrattiAffitto_Nuovi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim lavcodCA As Integer = CostantiPersonalizzate.LAVCOD_CONTRATTO_AFFITTO
        Dim areaTipologiaCA As Integer = TipiEnumerativi.enum_ID_Area_Tipologia.Contratti_Affitto

        Try

            StrSQL.Length = 0

            '------------------------------------------------------------------------------------------------------------------------------

            'CTE per contratti
            StrSQL.AppendLine("WITH contratti_cte")
            StrSQL.AppendLine("     (piva, id_agenda, des_lib, inviato, num_reg, data_reg, validitaC_dal, validitaC_al, validitaP_dal, validitaP_al)")
            StrSQL.AppendLine("AS (")
            StrSQL.AppendLine("    SELECT DISTINCT agenda.piva, agenda.id_agenda, agenda.des_lib, agenda.inviato,")
            StrSQL.AppendLine("                    m1.doc_numero num_reg, m1.data_registrazione data_reg, ")
            StrSQL.AppendLine("                    m2.Data_registrazione validitaC_dal, m2.Scadenza validitaC_al,")
            StrSQL.AppendLine("                    cip.validita_inizio validitaP_dal, cip.validita_fine validitaP_al")
            StrSQL.AppendLine("    FROM agenda")
            'primo JOIN Movimenti (cau_mov = 4000)
            StrSQL.AppendLine("    INNER JOIN Movimenti m1 ON ")
            StrSQL.AppendLine("               m1.PIVA = agenda.PIVA")
            StrSQL.AppendLine("               AND m1.Id_Agenda = agenda.id_agenda")
            StrSQL.AppendLine("               AND m1.Cau_Mov = " + CostantiPersonalizzate.CAU_REGISTRAZIONI)
            'secondo JOIN Movimenti (cau_mov = 4050)
            StrSQL.AppendLine("    INNER JOIN Movimenti m2 ON")
            StrSQL.AppendLine("               m2.PIVA = agenda.PIVA")
            StrSQL.AppendLine("               AND m2.Id_Agenda = agenda.id_agenda")
            StrSQL.AppendLine("               AND m2.Cau_Mov = " + CostantiPersonalizzate.CAU_REGISTRAZIONE_SECONDARIA)
            'JOIN ContrattiXImpreseXParticelle (su id_agenda)
            StrSQL.AppendLine("    INNER JOIN ContrattiXImpreseXParticelle cip ON")
            StrSQL.AppendLine("               cip.Id_Agenda = Agenda.Id_Agenda")
            'WHERE lav_cod = 2006 (Contratti d'affitto)
            StrSQL.AppendLine("    WHERE agenda.Lav_Cod = " + lavcodCA.ToString + "),")

            'CTE per JOIN Alert_Entita X Alert_Elenco
            StrSQL.AppendLine("     entita_elenco_cte (id_agenda, id_alert_entita, id_elenco, data_scadenza, descrizione_scadenza, note, ID_Tipologia, allegati_documenti_cod) AS (")
            StrSQL.AppendLine("         SELECT ent.id_agenda, ent.id_alert_entita, ele.id_elenco, ele.data_scadenza, ")
            StrSQL.AppendLine("                ele.descrizione_scadenza, ele.note, ele.ID_Tipologia, ent.allegati_documenti_cod")
            StrSQL.AppendLine("         FROM Alert_Entita ent")
            'JOIN Alert_Entita (su id_alert_entita)
            StrSQL.AppendLine("         INNER JOIN Alert_Elenco ele ON")
            StrSQL.AppendLine("                    ele.id_alert_entita = ent.id_alert_entita")
            StrSQL.AppendLine("         WHERE ent.ID_Agenda IN (SELECT id_agenda FROM contratti_cte))")

            'SELECT
            StrSQL.AppendLine("SELECT contratti_cte.piva, contratti_cte.id_agenda, contratti_cte.des_lib, contratti_cte.num_reg, contratti_cte.data_reg, ")
            StrSQL.AppendLine("       contratti_cte.validitaP_dal, contratti_cte.validitaP_al, contratti_cte.validitaC_dal, contratti_cte.validitaC_al,")
            StrSQL.AppendLine("       entita_elenco_cte.ID_Alert_Entita, entita_elenco_cte.id_elenco, entita_elenco_cte.Data_Scadenza, entita_elenco_cte.note, entita_elenco_cte.allegati_documenti_cod")
            StrSQL.AppendLine("FROM contratti_cte")

            'JOIN entita_elenco_cte (su id_agenda e data_scadenza)
            StrSQL.AppendLine("LEFT JOIN entita_elenco_cte ON")
            StrSQL.AppendLine("          entita_elenco_cte.id_agenda = contratti_cte.id_agenda")
            StrSQL.AppendLine("          AND entita_elenco_cte.data_scadenza = contratti_cte.validitaP_al")

            'WHERE: mostra contratti non presenti in alert_elenco, quindi non ancora aggiunti, ma non scaduti
            StrSQL.AppendLine("WHERE entita_elenco_cte.id_alert_entita IS NULL")
            StrSQL.AppendLine("      AND contratti_cte.validitaP_al > " + Agro_SQL_SaveDate(Date.Now))

            '----------------------------------------------------------------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND contratti_cte.inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND contratti_cte.inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '............................................................................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '----------------------------------------------------------------------------------------------------------------------------------

            '----------------------------------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '----------------------------------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_ContrattiAffitto_Cancellati(ByRef objParametri As AgronicaCoreParametri
                                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_ContrattiAffitto_Cancellati()"

        Dim lavcodCA As Integer = CostantiPersonalizzate.LAVCOD_CONTRATTO_AFFITTO
        Dim areaTipologiaCA As Integer = TipiEnumerativi.enum_ID_Area_Tipologia.Contratti_Affitto


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            '------------------------------------------------------------------------------------------------------------------------------

            'CTE per contratti
            StrSQL.AppendLine("WITH contratti_cte")
            StrSQL.AppendLine("     (piva, id_agenda, des_lib, inviato, num_reg, data_reg, validitaC_dal, validitaC_al, validitaP_dal, validitaP_al)")
            StrSQL.AppendLine("AS (")
            StrSQL.AppendLine("    SELECT DISTINCT agenda.piva, agenda.id_agenda, agenda.des_lib, agenda.inviato,")
            StrSQL.AppendLine("                    m1.doc_numero num_reg, m1.data_registrazione data_reg, ")
            StrSQL.AppendLine("                    m2.Data_registrazione validitaC_dal, m2.Scadenza validitaC_al,")
            StrSQL.AppendLine("                    cip.validita_inizio validitaP_dal, cip.validita_fine validitaP_al")
            StrSQL.AppendLine("    FROM agenda")
            'primo JOIN Movimenti (cau_mov = 4000)
            StrSQL.AppendLine("    INNER JOIN Movimenti m1 ON ")
            StrSQL.AppendLine("               m1.PIVA = agenda.PIVA")
            StrSQL.AppendLine("               AND m1.Id_Agenda = agenda.id_agenda")
            StrSQL.AppendLine("               AND m1.Cau_Mov = " + CostantiPersonalizzate.CAU_REGISTRAZIONI)
            'secondo JOIN Movimenti (cau_mov = 4050)
            StrSQL.AppendLine("    INNER JOIN Movimenti m2 ON")
            StrSQL.AppendLine("               m2.PIVA = agenda.PIVA")
            StrSQL.AppendLine("               AND m2.Id_Agenda = agenda.id_agenda")
            StrSQL.AppendLine("               AND m2.Cau_Mov = " + CostantiPersonalizzate.CAU_REGISTRAZIONE_SECONDARIA)
            'JOIN ContrattiXImpreseXParticelle (su id_agenda)
            StrSQL.AppendLine("    INNER JOIN ContrattiXImpreseXParticelle cip ON")
            StrSQL.AppendLine("               cip.Id_Agenda = Agenda.Id_Agenda")
            'WHERE lav_cod = 2006 (Contratti d'affitto)
            StrSQL.AppendLine("WHERE agenda.Lav_Cod = " + lavcodCA.ToString + ")")

            'SELECT
            StrSQL.AppendLine("SELECT contratti_cte.id_agenda, contratti_cte.des_lib, contratti_cte.num_reg, contratti_cte.data_reg, contratti_cte.validitaP_dal, contratti_cte.validitaP_al, alert_entita.id_alert_entita,")
            StrSQL.AppendLine("       alert_elenco.id_elenco, alert_elenco.data_scadenza, alert_elenco.descrizione_scadenza tipo_scad, alert_elenco.note, alert_elenco.ID_Tipologia, alert_entita.allegati_documenti_cod")
            StrSQL.AppendLine("FROM alert_elenco")

            'JOIN alert_entita (su id_alert_entita)
            StrSQL.AppendLine("INNER JOIN alert_entita ON")
            StrSQL.AppendLine("           alert_elenco.id_alert_entita = alert_entita.id_alert_entita")

            'JOIN contratti_cte (id_agenda AND data_scadenza)
            StrSQL.AppendLine("LEFT JOIN contratti_cte ON")
            StrSQL.AppendLine("          alert_entita.id_agenda = contratti_cte.id_agenda")
            StrSQL.AppendLine("          AND alert_elenco.data_scadenza = contratti_cte.validitaP_al")

            'WHERE: seleziona contratti presenti in alert_elenco, ma non più esistenti (con o senza allegato)
            StrSQL.AppendLine("WHERE alert_elenco.ID_Tipologia = " + areaTipologiaCA.ToString)
            StrSQL.AppendLine("      AND contratti_cte.id_agenda IS NULL")
            StrSQL.AppendLine("      AND alert_elenco.data_scadenza <> " + Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE))

            '----------------------------------------------------------------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND alert_elenco.inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND alert_elenco.inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '............................................................................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '----------------------------------------------------------------------------------------------------------------------------------

            '----------------------------------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '----------------------------------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_ParticelleImprese_Nuovi(ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_ParticelleImprese_Nuovi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim areaTipologiaPP As Integer = TipiEnumerativi.enum_ID_Area_Tipologia.Possesso_Particelle

        Try

            StrSQL.Length = 0

            '------------------------------------------------------------------------------------------------------------------------------

            'CTE per ricavare l'id delle particelle legate a contratti
            StrSQL.AppendLine("WITH impreseParticelleInContratti_CTE (idImpreseXParticelle)")
            StrSQL.AppendLine("AS ( SELECT DISTINCT ImpreseXParticelle.id")
            StrSQL.AppendLine("     FROM ContrattiXImpreseXParticelle")
            'JOIN ImpreseXParticelle
            StrSQL.AppendLine("     INNER JOIN ImpreseXParticelle ON")
            StrSQL.AppendLine("         ImpreseXParticelle.PIVA = ContrattiXImpreseXParticelle.PIVA")
            StrSQL.AppendLine("         AND ImpreseXParticelle.sa_cod = ContrattiXImpreseXParticelle.sa_cod")
            StrSQL.AppendLine("         AND ImpreseXParticelle.prov = ContrattiXImpreseXParticelle.prov")
            StrSQL.AppendLine("         AND ImpreseXParticelle.com = ContrattiXImpreseXParticelle.com")
            StrSQL.AppendLine("         AND ImpreseXParticelle.sezione = ContrattiXImpreseXParticelle.sezione")
            StrSQL.AppendLine("         AND ImpreseXParticelle.foglio = ContrattiXImpreseXParticelle.foglio")
            StrSQL.AppendLine("         AND ImpreseXParticelle.numero = ContrattiXImpreseXParticelle.numero")
            StrSQL.AppendLine("         AND ImpreseXParticelle.subalterno = ContrattiXImpreseXParticelle.subalterno)")

            'SELECT
            StrSQL.AppendLine("SELECT ImpreseXParticelle.piva, ImpreseXParticelle.id, ISTAT.LOCALITA, ISTAT.COMUNI_PROV,")
            StrSQL.AppendLine("       ImpreseXParticelle.sezione, ImpreseXParticelle.foglio, ImpreseXParticelle.numero,")
            StrSQL.AppendLine("       ImpreseXParticelle.SUBALTERNO, ImpreseXParticelle.TitoloPossesso, ImpreseXParticelle.Sup_Condotta,")
            StrSQL.AppendLine("       ImpreseXParticelle.Validita_Inizio, ImpreseXParticelle.Validita_Fine,")
            StrSQL.AppendLine("       alert_elenco.id_elenco, alert_elenco.data_scadenza, alert_elenco.descrizione_scadenza tipo_scad,")
            StrSQL.AppendLine("       alert_elenco.note, alert_elenco.ID_Tipologia, alert_entita.allegati_documenti_cod, alert_entita.Id_ImpreseXParticelle,")
            StrSQL.AppendLine("       ImpreseXParticelle.PROV, ImpreseXParticelle.COM")
            StrSQL.AppendLine("FROM ImpreseXParticelle")

            'JOIN Alert_Entita
            StrSQL.AppendLine("LEFT JOIN Alert_Entita ON")
            StrSQL.AppendLine("         ImpreseXParticelle.piva = Alert_Entita.Piva")
            StrSQL.AppendLine("         AND ImpreseXParticelle.ID = Alert_Entita.Id_ImpreseXParticelle")

            'JOIN Alert_Elenco
            StrSQL.AppendLine("LEFT JOIN Alert_Elenco ON")
            StrSQL.AppendLine("         Alert_Elenco.id_alert_entita = Alert_Entita.id_alert_entita")

            'JOIN ISTAT
            StrSQL.AppendLine("LEFT JOIN ISTAT ON")
            StrSQL.AppendLine("         ISTAT.PROV = ImpreseXParticelle.PROV")
            StrSQL.AppendLine("         AND ISTAT.COM = ImpreseXParticelle.COM")

            'WHERE: ricava particelle non presenti in Alert_Elenco, quindi non ancora aggiunte, ma non scadute;
            '       tratta solo quelle non legate a contratti       
            StrSQL.AppendLine("WHERE ImpreseXParticelle.TitoloPossesso NOT IN (0,1)")
            StrSQL.AppendLine("      AND Alert_Elenco.ID_Tipologia IS NULL")
            StrSQL.AppendLine("      AND ImpreseXParticelle.Validita_Fine <> " + Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE))
            StrSQL.AppendLine("      AND ImpreseXParticelle.Validita_Fine > " + Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("      AND id NOT IN (SELECT idImpreseXParticelle FROM impreseParticelleInContratti_CTE)")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND ImpreseXParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND ImpreseXParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri,
                                     StrSQL.ToString,
                                     NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri,
                       NomeRoutine,
                       MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_ParticelleImprese_Modificati(ByRef objParametri As AgronicaCoreParametri
                                                      ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_ParticelleImprese_Modificati()"

        Dim areaTipologiaPP As Integer = TipiEnumerativi.enum_ID_Area_Tipologia.Possesso_Particelle

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            '------------------------------------------------------------------------------------------------------------------------------

            'CTE per ricavare l'id delle particelle legate a contratti
            StrSQL.AppendLine("WITH impreseParticelleInContratti_CTE (idImpreseXParticelle)")
            StrSQL.AppendLine("AS ( SELECT DISTINCT ImpreseXParticelle.id")
            StrSQL.AppendLine("     FROM ContrattiXImpreseXParticelle")
            'JOIN ImpreseXParticelle
            StrSQL.AppendLine("     INNER JOIN ImpreseXParticelle ON")
            StrSQL.AppendLine("         ImpreseXParticelle.PIVA = ContrattiXImpreseXParticelle.PIVA")
            StrSQL.AppendLine("         AND ImpreseXParticelle.sa_cod = ContrattiXImpreseXParticelle.sa_cod")
            StrSQL.AppendLine("         AND ImpreseXParticelle.prov = ContrattiXImpreseXParticelle.prov")
            StrSQL.AppendLine("         AND ImpreseXParticelle.com = ContrattiXImpreseXParticelle.com")
            StrSQL.AppendLine("         AND ImpreseXParticelle.sezione = ContrattiXImpreseXParticelle.sezione")
            StrSQL.AppendLine("         AND ImpreseXParticelle.foglio = ContrattiXImpreseXParticelle.foglio")
            StrSQL.AppendLine("         AND ImpreseXParticelle.numero = ContrattiXImpreseXParticelle.numero")
            StrSQL.AppendLine("         AND ImpreseXParticelle.subalterno = ContrattiXImpreseXParticelle.subalterno)")

            'SELECT
            StrSQL.AppendLine("SELECT ImpreseXParticelle.piva, ImpreseXParticelle.id, ISTAT.LOCALITA, ISTAT.COMUNI_PROV,")
            StrSQL.AppendLine("       ImpreseXParticelle.sezione, ImpreseXParticelle.foglio, ImpreseXParticelle.numero,")
            StrSQL.AppendLine("       ImpreseXParticelle.SUBALTERNO, ImpreseXParticelle.TitoloPossesso, ImpreseXParticelle.Sup_Condotta,")
            StrSQL.AppendLine("       ImpreseXParticelle.Validita_Inizio, ImpreseXParticelle.Validita_Fine,")
            StrSQL.AppendLine("       alert_elenco.id_elenco, alert_elenco.data_scadenza, alert_elenco.descrizione_scadenza tipo_scad,")
            StrSQL.AppendLine("       alert_elenco.note, alert_elenco.ID_Tipologia, alert_entita.id_alert_entita, alert_entita.allegati_documenti_cod, alert_entita.Id_ImpreseXParticelle,")
            StrSQL.AppendLine("       ImpreseXParticelle.PROV, ImpreseXParticelle.COM")
            StrSQL.AppendLine("FROM ImpreseXParticelle")

            'JOIN Alert_Entita
            StrSQL.AppendLine("LEFT JOIN Alert_Entita ON")
            StrSQL.AppendLine("         ImpreseXParticelle.piva = Alert_Entita.Piva")
            StrSQL.AppendLine("         AND ImpreseXParticelle.ID = Alert_Entita.Id_ImpreseXParticelle")

            'JOIN Alert_Elenco
            StrSQL.AppendLine("LEFT JOIN Alert_Elenco ON")
            StrSQL.AppendLine("         Alert_Elenco.id_alert_entita = Alert_Entita.id_alert_entita")

            'JOIN ISTAT
            StrSQL.AppendLine("LEFT JOIN ISTAT ON")
            StrSQL.AppendLine("         ISTAT.PROV = ImpreseXParticelle.PROV")
            StrSQL.AppendLine("         AND ISTAT.COM = ImpreseXParticelle.COM")

            'WHERE: ricava particelle già presenti in Alert_Elenco, ma con data_scadenza da modificare o non inserita;
            '       tratta solo quelle non legate a contratti       
            StrSQL.AppendLine("WHERE ImpreseXParticelle.TitoloPossesso NOT IN (0,1)")
            StrSQL.AppendLine("      AND ImpreseXParticelle.Validita_Fine IS NOT NULL")
            StrSQL.AppendLine("      AND Alert_Elenco.ID_Tipologia = " + areaTipologiaPP.ToString)
            StrSQL.AppendLine("      AND (ImpreseXParticelle.Validita_Fine <> Alert_Elenco.data_scadenza")
            StrSQL.AppendLine("      OR Alert_Elenco.data_scadenza IS NULL)")
            StrSQL.AppendLine("      AND id NOT IN (SELECT idImpreseXParticelle FROM impreseParticelleInContratti_CTE)")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND ImpreseXParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND ImpreseXParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri,
                                     StrSQL.ToString,
                                     NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri,
                       NomeRoutine,
                       MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_ParticelleImprese_Cancellati(ByRef objParametri As AgronicaCoreParametri
                                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Importazioni.Leggi_ParticelleImprese_Cancellati()"

        Dim areaTipologiaPP As Integer = TipiEnumerativi.enum_ID_Area_Tipologia.Possesso_Particelle

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            '------------------------------------------------------------------------------------------------------------------------------

            'CTE per ricavare l'id delle particelle legate a contratti
            StrSQL.AppendLine("WITH impreseParticelleInContratti_CTE (idImpreseXParticelle)")
            StrSQL.AppendLine("AS ( SELECT DISTINCT ImpreseXParticelle.id")
            StrSQL.AppendLine("     FROM ContrattiXImpreseXParticelle")
            'JOIN ImpreseXParticelle
            StrSQL.AppendLine("     INNER JOIN ImpreseXParticelle ON")
            StrSQL.AppendLine("         ImpreseXParticelle.PIVA = ContrattiXImpreseXParticelle.PIVA")
            StrSQL.AppendLine("         AND ImpreseXParticelle.sa_cod = ContrattiXImpreseXParticelle.sa_cod")
            StrSQL.AppendLine("         AND ImpreseXParticelle.prov = ContrattiXImpreseXParticelle.prov")
            StrSQL.AppendLine("         AND ImpreseXParticelle.com = ContrattiXImpreseXParticelle.com")
            StrSQL.AppendLine("         AND ImpreseXParticelle.sezione = ContrattiXImpreseXParticelle.sezione")
            StrSQL.AppendLine("         AND ImpreseXParticelle.foglio = ContrattiXImpreseXParticelle.foglio")
            StrSQL.AppendLine("         AND ImpreseXParticelle.numero = ContrattiXImpreseXParticelle.numero")
            StrSQL.AppendLine("         AND ImpreseXParticelle.subalterno = ContrattiXImpreseXParticelle.subalterno)")

            'SELECT
            StrSQL.AppendLine("SELECT ImpreseXParticelle.piva, ImpreseXParticelle.id, ISTAT.LOCALITA, ISTAT.COMUNI_PROV,")
            StrSQL.AppendLine("       ImpreseXParticelle.sezione, ImpreseXParticelle.foglio, ImpreseXParticelle.numero,")
            StrSQL.AppendLine("       ImpreseXParticelle.SUBALTERNO, ImpreseXParticelle.TitoloPossesso, ImpreseXParticelle.Sup_Condotta,")
            StrSQL.AppendLine("       ImpreseXParticelle.Validita_Inizio, ImpreseXParticelle.Validita_Fine,")
            StrSQL.AppendLine("       alert_elenco.id_elenco, alert_elenco.data_scadenza, alert_elenco.descrizione_scadenza tipo_scad,")
            StrSQL.AppendLine("       alert_elenco.note, alert_elenco.ID_Tipologia, alert_entita.allegati_documenti_cod, alert_entita.Id_ImpreseXParticelle,")
            StrSQL.AppendLine("       alert_elenco.id_alert_entita ")
            StrSQL.AppendLine("FROM Alert_Elenco")

            'JOIN Alert_Entita
            StrSQL.AppendLine("INNER JOIN Alert_Entita ON")
            StrSQL.AppendLine("          Alert_Elenco.id_alert_entita = Alert_Entita.id_alert_entita")

            'JOIN ImpreseXParticelle
            '---- 1. Solo quelle diverse da Altro (0) e Proprietà (1)
            '---- 2. Solo quelle non legate a contratti di affitto
            '---- 3. Solo quelle con fine validità diversa da AGRODATAFINE
            StrSQL.AppendLine("LEFT JOIN ImpreseXParticelle ON")
            StrSQL.AppendLine("          ImpreseXParticelle.piva = Alert_Entita.Piva")
            StrSQL.AppendLine("          AND ImpreseXParticelle.ID = Alert_Entita.Id_ImpreseXParticelle")
            StrSQL.AppendLine("          AND ImpreseXParticelle.TitoloPossesso NOT IN (0,1)")
            StrSQL.AppendLine("          AND id NOT IN (SELECT idImpreseXParticelle FROM impreseParticelleInContratti_CTE)")
            StrSQL.AppendLine("          AND ImpreseXParticelle.Validita_Fine <> " + Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE))

            'JOIN ISTAT
            StrSQL.AppendLine("LEFT JOIN ISTAT ON")
            StrSQL.AppendLine("          ISTAT.PROV = ImpreseXParticelle.PROV")
            StrSQL.AppendLine("          AND ISTAT.COM = ImpreseXParticelle.COM")

            'WHERE: ricava le particelle presenti in Alert_Elenco, ma che non esistono più
            StrSQL.AppendLine("WHERE Alert_Elenco.ID_Tipologia = " + areaTipologiaPP.ToString)
            StrSQL.AppendLine("      AND Alert_Elenco.Validita_Fine <> " + Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE))
            StrSQL.AppendLine("      AND ImpreseXParticelle.ID IS NULL")

            '------------------------------------------------------------------------------------------------------------------------------

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND Alert_Elenco.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND Alert_Elenco.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '------------------------------------------------------------------------------------------------------------------------------

            '------------------------------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri,
                                     StrSQL.ToString,
                                     NomeRoutine)
            '------------------------------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri,
                       NomeRoutine,
                       MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class
