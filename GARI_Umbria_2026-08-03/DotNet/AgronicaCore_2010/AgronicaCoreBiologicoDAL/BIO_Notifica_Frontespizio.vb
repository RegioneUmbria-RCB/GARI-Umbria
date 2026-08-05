Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class BIO_Notifica_Frontespizio_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Notifica_ID As Integer, _
                            ByVal Notifica_Fotografia_Data As Date, _
                            ByVal Notifica_Piva As String, _
                            ByVal Notifica_SaCod As Integer, _
                            ByVal Notifica_Ricevimento_Protocollo As String, _
                            ByVal Notifica_Ricevimento_Data As Date, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                Optional ByVal flagLettura_ecludiSa_cod As Boolean = False _
                            ) As DataTable


        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT BIO_Notifica_Frontespizio.* ")
            StrSQL.Append(" FROM  BIO_Notifica_Frontespizio ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND  Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Notifica_ID <> 0 Then
                StrSQL.Append(" AND BIO_Notifica_Frontespizio.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
            End If

            If Notifica_Fotografia_Data <> AGRODATAINIZIO Then
                StrSQL.Append(" AND Notifica_Fotografia_Data = " & Agro_SQL_SaveDate(Notifica_Fotografia_Data) & " ")
            End If

            If Notifica_Piva <> "" Then
                StrSQL.Append(" AND Notifica_Piva = '" & Agro_SQL_SaveText(Notifica_Piva) & "' ")
            End If

            If Notifica_SaCod <> 0 Then
                StrSQL.Append(" AND Notifica_SaCod = " & Agro_SQL_SaveNum(Notifica_SaCod) & " ")
            Else
                If flagLettura_ecludiSa_cod Then
                    StrSQL.Append(" AND Notifica_SaCod = 0 ")
                End If
            End If

            If Notifica_Ricevimento_Protocollo <> "" Then
                StrSQL.Append(" AND Notifica_Ricevimento_Protocollo = '" & Agro_SQL_SaveText(Notifica_Ricevimento_Protocollo) & "' ")
            End If

            If Notifica_Ricevimento_Data <> AGRODATAINIZIO Then
                StrSQL.Append(" AND Notifica_Ricevimento_Data = " & Agro_SQL_SaveDate(Notifica_Ricevimento_Data) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   BIO_Notifica_Frontespizio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   BIO_Notifica_Frontespizio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.Append(" ORDER BY Desc_CorpoEstraneo")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '##############################################################################################
    'Notifica_ID opzionale, posso passare 0 e filtrare per altri dati
    Public Function Esiste_Notifica(ByVal Notifica_ID As Integer,
                                     ByVal Notifica_Fotografia_Data As Date,
                                        ByVal Notifica_Piva As String,
                                        ByVal Notifica_SaCod As Integer,
                                        ByVal Notifica_Ricevimento_Protocollo As String,
                                        ByVal Notifica_Ricevimento_Data As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim dt As DataTable
        Dim Flag_Esiste As Boolean = False


        dt = Leggi(Notifica_ID,
                    Notifica_Fotografia_Data,
                    Notifica_Piva,
                    Notifica_SaCod,
                    Notifica_Ricevimento_Protocollo,
                    Notifica_Ricevimento_Data,
                     xFiltroAggiuntivo,
                    "",
                    objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Flag_Esiste = True
        End If

        Return Flag_Esiste

    End Function

    '##############################################################################################
    Public Function LeggiJoinPlanning(ByVal Notifica_ID As Int32,
                                      ByVal Notifica_Fotografia_Data As Date,
                                        ByVal Notifica_Piva As String,
                                        ByVal Programmazione_Cod As Int32,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_R.LeggiJoinPlanning()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSQL.Length = 0
            strSQL.Append(" SELECT  BIO_Notifica_Frontespizio.*,  " & vbCrLf)
            strSQL.Append(" Programmazione_Des, Programmazione_Des_Long, Note, Tipo_Pianificazione  " & vbCrLf)

            strSQL.Append(" FROM BIO_Notifica_Frontespizio  " & vbCrLf)

            strSQL.Append(" INNER JOIN Programmazione_Testata ON BIO_Notifica_Frontespizio.Notifica_SuperUser = Programmazione_Testata.Piva_SuperUser   " & vbCrLf)
            strSQL.Append(" AND BIO_Notifica_Frontespizio.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod " & vbCrLf)

            '----- Condizioni
            strSQL.Append(" WHERE Notifica_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' " & vbCrLf)

            If Notifica_ID <> 0 Then
                strSQL.Append(" AND Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  " & vbCrLf)
            End If

            If Notifica_Fotografia_Data <> AGRODATAINIZIO Then
                strSQL.Append(" AND Notifica_Fotografia_Data = " & Agro_SQL_SaveDate(Notifica_Fotografia_Data) & " ")
            End If

            If Notifica_Piva <> "" Then
                strSQL.Append(" AND Notifica_Piva = '" & Agro_SQL_SaveText(Notifica_Piva) & "' ")
            End If

            If Programmazione_Cod <> 0 Then
                strSQL.Append(" AND Programmazione_Testata.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   BIO_Notifica_Frontespizio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   BIO_Notifica_Frontespizio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                'Else
                'strSQL.Append(" ORDER BY " )
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    'filtrare in base al codice necessario
    Public Function Esiste_NotificaConPlanning(ByVal Notifica_ID As Integer,
                                               ByVal Notifica_Fotografia_Data As Date,
                                                ByVal Notifica_Piva As String,
                                                ByVal Programmazione_Cod As Int32,
                                                ByVal Flag_RitornaInfoNotifica As Boolean,
                                                ByRef InfoNotifica As String,
                                                ByVal Flag_RitornaInfoPlanning As Boolean,
                                                ByRef InfoPlanning As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_R.Esiste_NotificaConPlanning()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Flag_Esiste As Boolean = False

        InfoNotifica = ""
        InfoPlanning = ""

        dt = LeggiJoinPlanning(Notifica_ID,
                                Notifica_Fotografia_Data,
                                Notifica_Piva,
                                Programmazione_Cod,
                                xFiltroAggiuntivo,
                                "",
                                objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Flag_Esiste = True

            If Flag_RitornaInfoNotifica = True Or Flag_RitornaInfoPlanning = True Then

                'no ciclo, perché ci sono tante righe quante le righe della sezione F
                'Dim i As Integer
                'For i = 0 To DT.Rows.Count - 1

                'questo flag ha senso se si sta filtrando dalle chiavi del planning e si vuole sapere qual è la notifica collegata
                If Flag_RitornaInfoNotifica = True Then
                    InfoNotifica = "Notifica del " + dt.Rows(0).Item("Notifica_Fotografia_Data")
                End If

                'questo flag ha senso se si sta filtrando dalla chiave della notifica e si vuole sapere qual è il planning collegato
                If Flag_RitornaInfoPlanning = True Then
                    InfoPlanning = dt.Rows(0).Item("Programmazione_Des")
                End If

                ' Next
            End If
        End If

        Return Flag_Esiste

    End Function


    '##############################################################################################
    Public Function LeggiConUtenti(ByVal DB_Utenti As String,
                                    ByVal Notifica_ID As Integer,
                                    ByVal Notifica_Fotografia_Data As Date,
                                    ByVal Notifica_Piva As String,
                                    ByVal Notifica_SaCod As Integer,
                                    ByVal Notifica_Ricevimento_Protocollo As String,
                                    ByVal Notifica_Ricevimento_Data As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_R.LeggiConUtenti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT BIO_Notifica_Frontespizio.*, ")
            StrSQL.Append(" UD1.Cognome + ' ' + UD1.Nome + ' ' + UD1.Rag_Soc AS Utente_creazione, ")
            StrSQL.Append(" UD2.Cognome + ' ' + UD2.Nome + ' ' + UD2.Rag_Soc AS Utente_Modifica ")
            StrSQL.Append("  ")
            StrSQL.Append(" FROM  BIO_Notifica_Frontespizio ")

            StrSQL.Append(" INNER JOIN " & DB_Utenti & ".dbo.Utenti_Dettagli UD1")
            StrSQL.Append("  ON BIO_Notifica_Frontespizio.Username_Creazione= UD1.CodFisc ")
            StrSQL.Append(" INNER JOIN " & DB_Utenti & ".dbo.Utenti_Dettagli UD2")
            StrSQL.Append("  ON BIO_Notifica_Frontespizio.Username_Creazione= UD2.CodFisc ")

            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Notifica_ID <> 0 Then
                StrSQL.Append(" AND BIO_Notifica_Frontespizio.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
            End If

            If Notifica_Fotografia_Data <> AGRODATAINIZIO Then
                StrSQL.Append(" AND Notifica_Fotografia_Data = " & Agro_SQL_SaveDate(Notifica_Fotografia_Data) & " ")
            End If

            If Notifica_Piva <> "" Then
                StrSQL.Append(" AND Notifica_Piva = '" & Agro_SQL_SaveText(Notifica_Piva) & "' ")
            End If

            If Notifica_SaCod <> 0 Then
                StrSQL.Append(" AND Notifica_SaCod = " & Agro_SQL_SaveNum(Notifica_SaCod) & " ")
            End If

            If Notifica_Ricevimento_Protocollo <> "" Then
                StrSQL.Append(" AND Notifica_Ricevimento_Protocollo = '" & Agro_SQL_SaveText(Notifica_Ricevimento_Protocollo) & "' ")
            End If

            If Notifica_Ricevimento_Data <> AGRODATAINIZIO Then
                StrSQL.Append(" AND Notifica_Ricevimento_Data = " & Agro_SQL_SaveDate(Notifica_Ricevimento_Data) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   BIO_Notifica_Frontespizio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   BIO_Notifica_Frontespizio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.Append(" ORDER BY Desc_CorpoEstraneo")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function RappLegale_Flag_Maschio1_Femmina2_from_Sesso(ByVal Sesso As String) As Integer

        Dim RappLegale_Flag_Maschio1_Femmina2 As Integer = 1

        If Sesso.ToUpper = "M" Then
            RappLegale_Flag_Maschio1_Femmina2 = 1
        Else
            RappLegale_Flag_Maschio1_Femmina2 = 2
        End If

        Return RappLegale_Flag_Maschio1_Femmina2

    End Function



End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class BIO_Notifica_Frontespizio_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Data_Creazione e username_creazione sono passati come parametri per tenere traccia 
    'dei dati originali di creazione
    '(la modifica avviene con cancella e scrivi)
    Public Function Scrivi(ByVal Notifica_ID As Integer,
                          ByVal Notifica_Piva As String,
                            ByVal Notifica_SaCod As Integer,
                            ByVal Notifica_Ricevimento_Protocollo As String,
                              ByVal Notifica_Ricevimento_Data As Date,
                              ByVal Notifica_Fotografia_Data As Date,
                              ByVal Cod_Istat_Regione As String,
                              ByVal Flag_Regione As Integer,
                              ByVal Flag_Ministero_MiPAF As Integer,
                              ByVal Flag_Categoria_Produttore As Integer,
                              ByVal Flag_Categoria_Preparatore As Integer,
                              ByVal Flag_Categoria_Importatore As Integer,
                              ByVal Flag_PrimaNotifica1_Variazione2 As Integer,
                              ByVal Flag_Cause_ModificaDichiarante As Integer,
                              ByVal Flag_Cause_ModificaUnitaProduttive As Integer,
                              ByVal Flag_Cause_ModificaCategorie As Integer,
                              ByVal Flag_Cause_CambioOrganismo As Integer,
                              ByVal Flag_Cause_CambioOrganismo_Des As String,
                              ByVal Flag_Cause_Altro As Integer,
                              ByVal Flag_Cause_Altro_Des As String,
                              ByVal Dichiarante_CodiceFiscale As String,
                              ByVal Dichiarante_Piva As String,
                              ByVal Dichiarante_Flag_AziendaIndividuale As Integer,
                              ByVal Dichiarante_Flag_Societa As Integer,
                              ByVal Dichiarante_Flag_Cooperativa As Integer,
                              ByVal Dichiarante_Cognome_RagSoc As String,
                              ByVal Dichiarante_Nome As String,
                              ByVal Dichiarante_Nascita_Provincia_Sigla As String,
                              ByVal Dichiarante_Nascita_Provincia_ISTAT As String,
                              ByVal Dichiarante_Nascita_Comune As String,
                              ByVal Dichiarante_Nascita_Comune_ISTAT As String,
                              ByVal Dichiarante_Nascita_Data As Date,
                              ByVal Dichiarante_Flag_Maschio1_Femmina2_Giuridica3 As Integer,
                              ByVal Dichiarante_Domicilio_Provincia_Sigla As String,
                              ByVal Dichiarante_Domicilio_Provincia_ISTAT As String,
                              ByVal Dichiarante_Domicilio_Comune As String,
                              ByVal Dichiarante_Domicilio_Comune_ISTAT As String,
                              ByVal Dichiarante_Domicilio_Via As String,
                              ByVal Dichiarante_Domicilio_Numero As String,
                              ByVal Dichiarante_Domicilio_CAP As String,
                              ByVal Dichiarante_Domicilio_Telefono As String,
                              ByVal Dichiarante_Domicilio_Fax As String,
                              ByVal Dichiarante_Domicilio_Email As String,
                              ByVal RappLegale_Cognome As String,
                              ByVal RappLegale_Nome As String,
                              ByVal RappLegale_CodiceFiscale As String,
                              ByVal RappLegale_Nascita_Provincia_Sigla As String,
                              ByVal RappLegale_Nascita_Provincia_ISTAT As String,
                              ByVal RappLegale_Nascita_Comune As String,
                              ByVal RappLegale_Nascita_Comune_ISTAT As String,
                              ByVal RappLegale_Nascita_Data As Date,
                              ByVal RappLegale_Flag_Maschio1_Femmina2 As Integer,
                              ByVal RappLegale_Domicilio_Provincia_Sigla As String,
                              ByVal RappLegale_Domicilio_Provincia_ISTAT As String,
                              ByVal RappLegale_Domicilio_Comune As String,
                              ByVal RappLegale_Domicilio_Comune_ISTAT As String,
                              ByVal RappLegale_Domicilio_Via As String,
                              ByVal RappLegale_Domicilio_Numero As String,
                              ByVal RappLegale_Domicilio_CAP As String,
                              ByVal OrganismoControllo_1_Codice As String,
                              ByVal OrganismoControllo_1_CodiceOperatore As String,
                              ByVal Organismo1_Flag_Produttore As Integer,
                              ByVal Organismo1_Flag_Preparatore As Integer,
                              ByVal Organismo1_Flag_Importatore As Integer,
                              ByVal OrganismoControllo_2_Codice As String,
                              ByVal OrganismoControllo_2_CodiceOperatore As String,
                              ByVal Organismo2_Flag_Produttore As Integer,
                              ByVal Organismo2_Flag_Preparatore As Integer,
                              ByVal Organismo2_Flag_Importatore As Integer,
                              ByVal Flag_Notifica_Bloccata As Integer,
                               ByVal Programmazione_Cod As Integer,
                               ByVal Username_Creazione As String,
                               ByVal Data_Creazione As Date,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_modifica As String = ""
                                    ) As Boolean


        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If


            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            strSQL.Length = 0
            strSQL.Append(" INSERT INTO BIO_Notifica_Frontespizio " & vbCrLf)

            strSQL.Append("             (Notifica_SuperUser " & vbCrLf)
            strSQL.Append("             ,Notifica_ID " & vbCrLf)
            strSQL.Append("             ,Notifica_Piva" & vbCrLf)
            strSQL.Append("             ,Notifica_SaCod" & vbCrLf)
            strSQL.Append("             ,Notifica_Ricevimento_Protocollo" & vbCrLf)
            strSQL.Append("             ,Notifica_Ricevimento_Data" & vbCrLf)
            strSQL.Append("             ,Notifica_Fotografia_Data" & vbCrLf)
            strSQL.Append("             ,Cod_Istat_Regione" & vbCrLf)
            strSQL.Append("             ,Flag_Regione" & vbCrLf)
            strSQL.Append("             ,Flag_Ministero_MiPAF" & vbCrLf)
            strSQL.Append("             ,Flag_Categoria_Produttore" & vbCrLf)
            strSQL.Append("             ,Flag_Categoria_Preparatore" & vbCrLf)
            strSQL.Append("             ,Flag_Categoria_Importatore" & vbCrLf)
            strSQL.Append("             ,Flag_PrimaNotifica1_Variazione2" & vbCrLf)
            strSQL.Append("             ,Flag_Cause_ModificaDichiarante" & vbCrLf)
            strSQL.Append("             ,Flag_Cause_ModificaUnitaProduttive" & vbCrLf)
            strSQL.Append("             ,Flag_Cause_ModificaCategorie" & vbCrLf)
            strSQL.Append("             ,Flag_Cause_CambioOrganismo" & vbCrLf)
            strSQL.Append("             ,Flag_Cause_CambioOrganismo_Des" & vbCrLf)
            strSQL.Append("             ,Flag_Cause_Altro" & vbCrLf)
            strSQL.Append("             ,Flag_Cause_Altro_Des" & vbCrLf)
            strSQL.Append("             ,Dichiarante_CodiceFiscale" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Piva" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Flag_AziendaIndividuale" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Flag_Societa" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Flag_Cooperativa" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Cognome_RagSoc" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Nome" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Nascita_Provincia_Sigla" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Nascita_Provincia_ISTAT" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Nascita_Comune" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Nascita_Comune_ISTAT" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Nascita_Data" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Flag_Maschio1_Femmina2_Giuridica3" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Provincia_Sigla" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Provincia_ISTAT" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Comune" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Comune_ISTAT" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Via" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Numero" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_CAP" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Telefono" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Fax" & vbCrLf)
            strSQL.Append("             ,Dichiarante_Domicilio_Email" & vbCrLf)
            strSQL.Append("             ,RappLegale_Cognome" & vbCrLf)
            strSQL.Append("             ,RappLegale_Nome" & vbCrLf)
            strSQL.Append("             ,RappLegale_CodiceFiscale" & vbCrLf)
            strSQL.Append("             ,RappLegale_Nascita_Provincia_Sigla" & vbCrLf)
            strSQL.Append("             ,RappLegale_Nascita_Provincia_ISTAT" & vbCrLf)
            strSQL.Append("             ,RappLegale_Nascita_Comune" & vbCrLf)
            strSQL.Append("             ,RappLegale_Nascita_Comune_ISTAT" & vbCrLf)
            strSQL.Append("             ,RappLegale_Nascita_Data" & vbCrLf)
            strSQL.Append("             ,RappLegale_Flag_Maschio1_Femmina2" & vbCrLf)
            strSQL.Append("             ,RappLegale_Domicilio_Provincia_Sigla" & vbCrLf)
            strSQL.Append("             ,RappLegale_Domicilio_Provincia_ISTAT" & vbCrLf)
            strSQL.Append("             ,RappLegale_Domicilio_Comune" & vbCrLf)
            strSQL.Append("             ,RappLegale_Domicilio_Comune_ISTAT" & vbCrLf)
            strSQL.Append("             ,RappLegale_Domicilio_Via" & vbCrLf)
            strSQL.Append("             ,RappLegale_Domicilio_Numero" & vbCrLf)
            strSQL.Append("             ,RappLegale_Domicilio_CAP" & vbCrLf)
            strSQL.Append("             ,OrganismoControllo_1_Codice" & vbCrLf)
            strSQL.Append("             ,OrganismoControllo_1_CodiceOperatore" & vbCrLf)
            strSQL.Append("             ,Organismo1_Flag_Produttore" & vbCrLf)
            strSQL.Append("             ,Organismo1_Flag_Preparatore" & vbCrLf)
            strSQL.Append("             ,Organismo1_Flag_Importatore" & vbCrLf)
            strSQL.Append("             ,OrganismoControllo_2_Codice" & vbCrLf)
            strSQL.Append("             ,OrganismoControllo_2_CodiceOperatore" & vbCrLf)
            strSQL.Append("             ,Organismo2_Flag_Produttore" & vbCrLf)
            strSQL.Append("             ,Organismo2_Flag_Preparatore" & vbCrLf)
            strSQL.Append("             ,Organismo2_Flag_Importatore" & vbCrLf)
            strSQL.Append("             ,Flag_Notifica_Bloccata, Programmazione_Cod, " & vbCrLf)

            strSQL.Append("              Inviato,            datainvio, ")
            strSQL.Append("              Data_Creazione,     Data_Modifica, ")
            strSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            strSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock ")
            strSQL.Append("              ) ")

            strSQL.Append(" VALUES (")
            strSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(Trim(Notifica_ID)) & "" & vbCrLf)
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Notifica_Piva) & "'  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(Notifica_SaCod) & "" & vbCrLf)
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Notifica_Ricevimento_Protocollo) & "'  ")
            strSQL.Append(" , " & Agro_SQL_SaveDate(Notifica_Ricevimento_Data) & "  ")
            strSQL.Append(" , " & Agro_SQL_SaveDate(Notifica_Fotografia_Data) & "  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Cod_Istat_Regione) & "'  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Regione) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Ministero_MiPAF) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Categoria_Produttore) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Categoria_Preparatore) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Categoria_Importatore) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_PrimaNotifica1_Variazione2) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Cause_ModificaDichiarante) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Cause_ModificaUnitaProduttive) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Cause_ModificaCategorie) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Cause_CambioOrganismo) & "" & vbCrLf)
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Flag_Cause_CambioOrganismo_Des) & "'  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Cause_Altro) & "" & vbCrLf)
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Flag_Cause_Altro_Des) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_CodiceFiscale) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Piva) & "'  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(Dichiarante_Flag_AziendaIndividuale) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Dichiarante_Flag_Societa) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Dichiarante_Flag_Cooperativa) & "" & vbCrLf)
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Cognome_RagSoc) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Nome) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Nascita_Provincia_Sigla) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Nascita_Provincia_ISTAT) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Nascita_Comune) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Nascita_Comune_ISTAT) & "'  ")
            strSQL.Append(" , " & Agro_SQL_SaveDate(Dichiarante_Nascita_Data) & "  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(Dichiarante_Flag_Maschio1_Femmina2_Giuridica3) & "" & vbCrLf)
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Provincia_Sigla) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Provincia_ISTAT) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Comune) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Comune_ISTAT) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Via) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Numero) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_CAP) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Telefono) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Fax) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(Dichiarante_Domicilio_Email) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Cognome) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Nome) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_CodiceFiscale) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Nascita_Provincia_Sigla) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Nascita_Provincia_ISTAT) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Nascita_Comune) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Nascita_Comune_ISTAT) & "'  ")
            strSQL.Append(" , " & Agro_SQL_SaveDate(RappLegale_Nascita_Data) & "  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(RappLegale_Flag_Maschio1_Femmina2) & "" & vbCrLf)
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Domicilio_Provincia_Sigla) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Domicilio_Provincia_ISTAT) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Domicilio_Comune) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Domicilio_Comune_ISTAT) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Domicilio_Via) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Domicilio_Numero) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(RappLegale_Domicilio_CAP) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(OrganismoControllo_1_Codice) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(OrganismoControllo_1_CodiceOperatore) & "'  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(Organismo1_Flag_Produttore) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Organismo1_Flag_Preparatore) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Organismo1_Flag_Importatore) & "" & vbCrLf)
            strSQL.Append(" ,'" & Agro_SQL_SaveText(OrganismoControllo_2_Codice) & "'  ")
            strSQL.Append(" ,'" & Agro_SQL_SaveText(OrganismoControllo_2_CodiceOperatore) & "'  ")
            strSQL.Append(" ," & Agro_SQL_SaveNum(Organismo2_Flag_Produttore) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Organismo2_Flag_Preparatore) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Organismo2_Flag_Importatore) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Flag_Notifica_Bloccata) & "" & vbCrLf)
            strSQL.Append(" ," & Agro_SQL_SaveNum(Programmazione_Cod) & "" & vbCrLf)

            strSQL.Append("         , 0  ")
            strSQL.Append("         , Null  ")

            strSQL.Append("			, " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            strSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            strSQL.Append("			,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            strSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSQL.Append("         , 0  ")
            strSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function



    '########################################################
    Public Function Cancella(ByVal Notifica_ID As Int32,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Notifica_ID = 0 Then
                Throw New Exception("Parametro non corretto nella query (Notifica_ID obbligatorio)")
            End If

            '---------------------------------------------
            strSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSQL.Length = 0
                strSQL.Append(" UPDATE BIO_Notifica_Frontespizio ")
                strSQL.Append(" SET ")
                strSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                strSQL.Append("         ,Inviato = -1 ")
                strSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
                strSQL.Append(" AND     Inviato >= 0 ")

            Else

                strSQL.Length = 0
                strSQL.Append(" DELETE ")
                strSQL.Append(" FROM    BIO_Notifica_Frontespizio ")
                strSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function




End Class
