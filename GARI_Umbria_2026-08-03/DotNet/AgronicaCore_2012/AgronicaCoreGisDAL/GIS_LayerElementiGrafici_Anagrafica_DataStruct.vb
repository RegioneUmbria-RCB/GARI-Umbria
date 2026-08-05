Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class GIS_LayerElementiGrafici_Anagrafica_DataStruct_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
        ByVal PivaSuperUser As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal TipologiaLayer_cod As Int32,
        ByVal TipologiaLayer_struct_cod As Int32,
        ByVal Etichetta As String,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine("SELECT")
                    StrSQL.AppendLine("    LayerDataStruct.PivaSuperUser,")
                    StrSQL.AppendLine("    LayerDataStruct.LayerElementiGrafici_Cod,")
                    StrSQL.AppendLine("    LayerDataStruct.TipologiaLayer_cod,")
                    StrSQL.AppendLine("    LayerDataStruct.TipologiaLayer_struct_cod,")
                    StrSQL.AppendLine("    LayerDataStruct.LayerElementiGrafici_Etichetta,")
                    StrSQL.AppendLine("    CASE WHEN trans.LayerElementiGrafici_Etichetta IS NULL OR trans.LayerElementiGrafici_Etichetta = '' THEN LayerDataStruct.LayerElementiGrafici_Etichetta ELSE trans.LayerElementiGrafici_Etichetta END AS LayerElementiGrafici_Etichetta_Des,") 
                    StrSQL.AppendLine("    LayerDataStruct.LayerElementiGrafici_TipoDato,")
                    StrSQL.AppendLine("    LayerDataStruct.inviato,")
                    StrSQL.AppendLine("    LayerDataStruct.datainvio,")
                    StrSQL.AppendLine("    LayerDataStruct.Data_Creazione,")
                    StrSQL.AppendLine("    LayerDataStruct.Data_Modifica,")
                    StrSQL.AppendLine("    LayerDataStruct.Username_Creazione,")
                    StrSQL.AppendLine("    LayerDataStruct.Username_Modifica,")
                    StrSQL.AppendLine("    LayerDataStruct.Validita_Inizio,")
                    StrSQL.AppendLine("    LayerDataStruct.Validita_Fine,")
                    StrSQL.AppendLine("    LayerDataStruct.CampoChiave,")
                    StrSQL.AppendLine("    LayerDataStruct.EtichettaVisibile,")
                    StrSQL.AppendLine("    LayerDataStruct.TipologiaLayer_struct_GUID")
                    StrSQL.AppendLine("FROM GIS_LayerElementiGrafici_Anagrafica_DataStruct LayerDataStruct")
                    StrSQL.AppendLine("    LEFT JOIN GIS_LayerElementiGrafici_Anagrafica_DataStruct_XLingue trans")
                    StrSQL.AppendLine("        ON (trans.LayerElementiGrafici_Cod = LayerDataStruct.LayerElementiGrafici_Cod")
                    StrSQL.AppendLine("        AND trans.TipologiaLayer_cod = LayerDataStruct.TipologiaLayer_cod")
                    StrSQL.AppendLine("        AND trans.TipologiaLayer_struct_cod = LayerDataStruct.TipologiaLayer_struct_cod")
                    StrSQL.AppendLine("        AND Lingua_Cod = " & Agro_SQL_SaveNum(objParametri_Utenti.Lingua_Cod) & ")")
                    StrSQL.AppendLine("WHERE LayerDataStruct.Validita_inizio < " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("AND LayerDataStruct.Validita_Fine > " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

                    StrSQL.AppendLine("AND LayerDataStruct.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

                    If LayerElementiGrafici_Cod <> 0 Then
                        StrSQL.AppendLine(" AND LayerDataStruct.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
                    End If

                    If TipologiaLayer_cod <> 0 Then
                        StrSQL.AppendLine(" AND LayerDataStruct.TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod) & " ")
                    End If

                    If TipologiaLayer_struct_cod <> 0 Then
                        StrSQL.AppendLine(" AND LayerDataStruct.TipologiaLayer_struct_cod = " & Agro_SQL_SaveNum(TipologiaLayer_struct_cod) & " ")
                    End If

                    If Etichetta <> "" Then
                        StrSQL.AppendLine(" AND UPPER(LayerDataStruct.LayerElementiGrafici_Etichetta) = '" & Agro_SQL_SaveText(Etichetta.ToUpper) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
                    End If

                    Select Case objParametri_Server.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   LayerDataStruct.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   LayerDataStruct.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine("SELECT")
                    StrSQL.AppendLine("    LayerDataStruct.PivaSuperUser,")
                    StrSQL.AppendLine("    LayerDataStruct.LayerElementiGrafici_Cod,")
                    StrSQL.AppendLine("    LayerDataStruct.TipologiaLayer_cod,")
                    StrSQL.AppendLine("    LayerDataStruct.TipologiaLayer_struct_cod,")
                    StrSQL.AppendLine("    LayerDataStruct.LayerElementiGrafici_Etichetta,")
                    StrSQL.AppendLine("    CASE WHEN trans.LayerElementiGrafici_Etichetta IS NULL OR trans.LayerElementiGrafici_Etichetta = '' THEN LayerDataStruct.LayerElementiGrafici_Etichetta ELSE trans.LayerElementiGrafici_Etichetta END AS LayerElementiGrafici_Etichetta_Des,")
                    StrSQL.AppendLine("    LayerDataStruct.LayerElementiGrafici_TipoDato,")
                    StrSQL.AppendLine("    LayerDataStruct.inviato,")
                    StrSQL.AppendLine("    LayerDataStruct.datainvio,")
                    StrSQL.AppendLine("    LayerDataStruct.Data_Creazione,")
                    StrSQL.AppendLine("    LayerDataStruct.Data_Modifica,")
                    StrSQL.AppendLine("    LayerDataStruct.Username_Creazione,")
                    StrSQL.AppendLine("    LayerDataStruct.Username_Modifica,")
                    StrSQL.AppendLine("    LayerDataStruct.Validita_Inizio,")
                    StrSQL.AppendLine("    LayerDataStruct.Validita_Fine,")
                    StrSQL.AppendLine("    LayerDataStruct.CampoChiave,")
                    StrSQL.AppendLine("    LayerDataStruct.TipologiaLayer_struct_GUID")
                    StrSQL.AppendLine("FROM GIS_LayerElementiGrafici_Anagrafica_DataStruct LayerDataStruct")

                    StrSQL.AppendLine("    LEFT JOIN GIS_LayerElementiGrafici_Anagrafica_DataStruct_XLingue trans")
                    StrSQL.AppendLine("        ON (trans.LayerElementiGrafici_Cod = LayerDataStruct.LayerElementiGrafici_Cod")
                    StrSQL.AppendLine("        AND trans.TipologiaLayer_cod = LayerDataStruct.TipologiaLayer_cod")
                    StrSQL.AppendLine("        AND trans.TipologiaLayer_struct_cod = LayerDataStruct.TipologiaLayer_struct_cod")
                    StrSQL.AppendLine("        AND Lingua_Cod = " & Agro_SQL_SaveNum(objParametri_Utenti.Lingua_Cod) & ")")

                    StrSQL.AppendLine("inner Join(")
                    StrSQL.AppendLine("    select g.LayerElementiGrafici_Cod")
                    StrSQL.AppendLine("    From GIS_LayerElementiGrafici g ")
                    StrSQL.AppendLine("    WHERE g.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
                    StrSQL.AppendLine("    And g.Utente = '" & objParametri_Server.UtenteUsername & "' ")
                    StrSQL.AppendLine("    And g.TipologiaLayer_cod = 1 ")
                    StrSQL.AppendLine("    And g.Flag_Visibile = 1 ")
                    StrSQL.AppendLine(" ) uu ")
                    StrSQL.AppendLine("     On uu.LayerElementiGrafici_Cod = LayerDataStruct.LayerElementiGrafici_Cod ")

'                    StrSQL.AppendLine("WHERE LayerDataStruct.LayerElementiGrafici_Etichetta like '^%'")
                    StrSQL.AppendLine("WHERE LayerDataStruct.EtichettaVisibile = 1")
                    StrSQL.AppendLine("AND LayerDataStruct.Validita_inizio < " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("AND LayerDataStruct.Validita_Fine > " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

                    StrSQL.AppendLine("AND LayerDataStruct.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

                    If LayerElementiGrafici_Cod <> 0 Then
                        StrSQL.AppendLine(" AND LayerDataStruct.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
                    End If

                    If TipologiaLayer_cod <> 0 Then
                        StrSQL.AppendLine(" AND LayerDataStruct.TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod) & " ")
                    End If

                    If TipologiaLayer_struct_cod <> 0 Then
                        StrSQL.AppendLine(" AND LayerDataStruct.TipologiaLayer_struct_cod = " & Agro_SQL_SaveNum(TipologiaLayer_struct_cod) & " ")
                    End If

                    If Etichetta <> "" Then
                        StrSQL.AppendLine(" AND UPPER(LayerDataStruct.LayerElementiGrafici_Etichetta) = '" & Agro_SQL_SaveText(Etichetta.ToUpper) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
                    End If

                    Select Case objParametri_Server.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   LayerDataStruct.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   LayerDataStruct.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
                    End If

            End Select

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class

Public Class GIS_LayerElementiGrafici_Anagrafica_DataStruct_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function EliminaAttributo(ByVal PivaSuperUser As String,
                                     ByVal ProgressivoDataStruct As String,
                                     ByVal TipoLayer As Int32,
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W.EliminaAttributo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Dim progressivoInt As Int32

            If Not Int32.TryParse(ProgressivoDataStruct, progressivoInt) Then
                Throw New Exception("Il progressivo dell'attributo deve essere numerico")
            End If

            Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_R

            Dim DTread = xRead.Leggi(PivaSuperUser,
                                     0,
                                     TipoLayer,
                                     progressivoInt,
                                     "",
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "",
                                     "",
                                     objParametri_Server,
                                     objParametri_Utenti)

            If DTread.Rows.Count = 0 Then
                Throw New Exception(String.Format("Impossibile trovare un attributo con progressivo {0}.", progressivoInt.ToString))
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)

            xRisp = Elimina(PivaSuperUser,
                           TipoLayer,
                           progressivoInt,
                           objParametri_Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return xRisp

    End Function

    Public Function SalvaGUIDStruct(ByVal tipologiaLayer_struct_cod As Int32,
                                    ByVal newGuidParam As String,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W.SalvaGUIDStruct()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerElementiGrafici_Anagrafica_DataStruct SET ")

            StrSQL.AppendLine(String.Format(" TipologiaLayer_struct_GUID = '{0}' ", Agro_SQL_SaveText(newGuidParam)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" TipologiaLayer_struct_cod = {0} ", Agro_SQL_SaveNum(tipologiaLayer_struct_cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not xRisp Then Return xRisp

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig_DataStruct SET ")

            StrSQL.AppendLine(String.Format(" TipologiaLayer_struct_GUID = '{0}' ", Agro_SQL_SaveText(newGuidParam)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" TipologiaLayer_struct_cod = {0} ", Agro_SQL_SaveNum(tipologiaLayer_struct_cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function AggiornaAttributo(ByVal PivaSuperUser As String,
                                      ByVal IdLayer As String,
                                      ByVal Nome As String,
                                      ByVal TipoDato As String,
                                      ByVal TipoLayer As Int32,
                                      ByVal CampoChiave As Int32,
                                      ByVal etichettaVisibile As Nullable(Of Boolean),
                                      ByVal ProgressivoDataStruct As String,
                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W.AggiornaAttributo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Dim progressivoInt As Int32

            If Not Int32.TryParse(ProgressivoDataStruct, progressivoInt) Then
                Throw New Exception("Il progressivo dell'attributo deve essere numerico")
            End If

            Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_R

            Dim DTread = xRead.Leggi(PivaSuperUser,
                                     Int32.Parse(IdLayer),
                                     TipoLayer,
                                     progressivoInt,
                                     "",
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "",
                                     "",
                                     objParametri_Server,
                                     objParametri_Utenti)

            If DTread.Rows.Count = 0 Then
                Throw New Exception(String.Format("Impossibile trovare un attributo con progressivo {0}.", progressivoInt.ToString))
            End If

            If Not String.IsNullOrWhiteSpace(Nome) Then
                DTread = xRead.Leggi(PivaSuperUser,
                                     Int32.Parse(IdLayer),
                                     TipoLayer,
                                     0,
                                     Nome,
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "",
                                     "",
                                     objParametri_Server,
                                     objParametri_Utenti)

                If DTread.Rows.Count > 0 Then
                    Dim found As Boolean = False
                    Dim row As DataRow

                    For Each row In DTread.Rows
                        If Not row("TipologiaLayer_struct_cod").ToString.Equals(ProgressivoDataStruct) Then
                            found = True
                        End If
                    Next

                    If found Then
                        Throw New Exception(String.Format("Esiste già un attributo di nome {0} per il layer {1}.", Nome, IdLayer))
                    End If
                End If
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)

            xRisp = Aggiorna(PivaSuperUser,
                           Int32.Parse(IdLayer),
                           TipoLayer,
                           progressivoInt,
                           Nome,
                           TipoDato,
                           CampoChiave,
                             etichettaVisibile,
                           AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                           AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE,
                           objParametri_Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return xRisp

    End Function

    Public Function InserisciNuovoAttributo(
                                           ByVal PivaSuperUser As String,
                                            ByVal IdLayer As String,
                                            ByVal Nome As String,
                                            ByVal TipoDato As String,
                                            ByVal TipoLayer As Int32,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W.InserisciNuovoAttributo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_R

            Dim DTread = xRead.Leggi(PivaSuperUser,
                                     Int32.Parse(IdLayer),
                                     TipoLayer,
                                     0,
                                     Nome,
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "",
                                     "",
                                     objParametri_Server,
                                     objParametri_Utenti)

            If DTread.Rows.Count > 0 Then
                Throw New Exception(String.Format("Esiste già un attributo {0} per il Layer {1} dell'utente.", Nome, IdLayer))
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)

            Dim sequenza_tabelle As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim maxDataStructCode As Integer = sequenza_tabelle.NuovoId_Tabella("gis_layerelementigrafici_anagrafica_datastruct",
                                                                                0,
                                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                                objParametri_Server)

            xRisp = Scrivi(PivaSuperUser,
                           Int32.Parse(IdLayer),
                           TipoLayer,
                           maxDataStructCode,
                           Nome,
                           TipoDato,
                           AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                           AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE,
                           objParametri_Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return xRisp

    End Function

    Public Function Elimina(
        ByVal PivaSuperUser As String,
        ByVal TipologiaLayer_cod As Int32,
        ByVal TipologiaLayer_struct_cod As Int32,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W.Elimina()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            'Controlli
            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella update (PivaSuperUser obbligatorio)")
            End If
            If (TipologiaLayer_cod = 0) Then
                Throw New Exception("Parametro non corretto nella update (TipologiaLayer_cod obbligatorio)")
            End If
            If (TipologiaLayer_struct_cod = 0) Then
                Throw New Exception("Parametro non corretto nella update (TipologiaLayer_struct_cod obbligatorio)")
            End If
            'Comando
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM GIS_LayerElementiGrafici_Anagrafica_DataStruct ")
            StrSQL.AppendLine("WHERE ")
            StrSQL.AppendLine("TipologiaLayer_struct_cod = " & Agro_SQL_SaveNum(TipologiaLayer_struct_cod) & " ")
            StrSQL.AppendLine("AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

            'Esecuzione
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DeleteWholeLayerDataStruct(pivaSuperUser As String,
                                               layerCod As Int32,
                                               objParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGarfici_Anagrafica_DataStruct_W.DeleteLayerDataStruct()"
        Dim strSQL As New Text.StringBuilder

        Try
            strSQL.Length = 0

            If layerCod = 0 Then
                Throw New Exception("Parametro non corretto: layerCod")
            End If

            If String.IsNullOrWhiteSpace(pivaSuperUser) AndAlso pivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto: pivaSuperUser")
            End If

            strSQL.AppendLine("DELETE FROM GIS_LayerElementiGrafici_Anagrafica_DataStruct")
            strSQL.AppendLine($"WHERE PivaSuperUser = '{Agro_SQL_SaveText(pivaSuperUser)}'")
            strSQL.AppendLine($"    AND LayerElementiGrafici_Cod = {Agro_SQL_SaveNum(layerCod)}")

            Return EseguiQuery_Scrittura(objParametriServer, strSQL.ToString, nomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametriServer, nomeRoutine, ex.Message)
            Throw New Exception($"[{nomeRoutine}] : {ex.Message}")
        End Try
    End Function

    Public Function Aggiorna(
        ByVal PivaSuperUser As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal TipologiaLayer_cod As Int32,
        ByVal TipologiaLayer_struct_cod As Int32,
        ByVal LayerElementiGrafici_Etichetta As String,
        ByVal LayerElementiGrafici_TipoDato As String,
        ByVal CampoChiave As Int32,
        ByVal etichettaVisibile As Nullable(Of Boolean),
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W.Aggiorna()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            'Controlli
            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella update (PivaSuperUser obbligatorio)")
            End If
            If (LayerElementiGrafici_Cod = 0) Then
                Throw New Exception("Parametro non corretto nella update (LayerElementiGrafici_Cod obbligatorio)")
            End If
            If (TipologiaLayer_cod = 0) Then
                Throw New Exception("Parametro non corretto nella update (TipologiaLayer_cod obbligatorio)")
            End If
            If (TipologiaLayer_struct_cod = 0) Then
                Throw New Exception("Parametro non corretto nella update (TipologiaLayer_struct_cod obbligatorio)")
            End If
            'Comando
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE GIS_LayerElementiGrafici_Anagrafica_DataStruct ")
            'Colonne
            StrSQL.AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            If Not LayerElementiGrafici_Etichetta = "" Then
                StrSQL.AppendLine(", LayerElementiGrafici_Etichetta = '" & Agro_SQL_SaveText(LayerElementiGrafici_Etichetta) & "' ")
            End If

            If CampoChiave > -1 Then
                StrSQL.AppendLine(", CampoChiave = " & Agro_SQL_SaveNum(CampoChiave) & " ")
            End If
            
            If Not (etichettaVisibile Is Nothing) Then
                StrSQL.AppendLine(", EtichettaVisibile = " & Agro_SQL_SaveNum(Convert.ToInt32(etichettaVisibile)) & " ")
            End If

            If Not LayerElementiGrafici_TipoDato = "" Then
                StrSQL.AppendLine(", LayerElementiGrafici_TipoDato = '" & Agro_SQL_SaveText(LayerElementiGrafici_TipoDato) & "' ")
            End If

            StrSQL.AppendLine("WHERE ")
            StrSQL.AppendLine("TipologiaLayer_struct_cod = " & Agro_SQL_SaveNum(TipologiaLayer_struct_cod) & " ")

            'Esecuzione
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Scrivi(
        ByVal PivaSuperUser As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal TipologiaLayer_cod As Int32,
        ByVal TipologiaLayer_struct_cod As Int32,
        ByVal LayerElementiGrafici_Etichetta As String,
        ByVal LayerElementiGrafici_TipoDato As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            'Controlli
            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella insert (PivaSuperUser obbligatorio)")
            End If
            If (LayerElementiGrafici_Cod = 0) Then
                Throw New Exception("Parametro non corretto nella insert (LayerElementiGrafici_Cod obbligatorio)")
            End If
            If (TipologiaLayer_cod = 0) Then
                Throw New Exception("Parametro non corretto nella insert (TipologiaLayer_cod obbligatorio)")
            End If
            If (TipologiaLayer_struct_cod = 0) Then
                Throw New Exception("Parametro non corretto nella insert (TipologiaLayer_struct_cod obbligatorio)")
            End If
            'Comando
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO GIS_LayerElementiGrafici_Anagrafica_DataStruct ")
            'Colonne
            StrSQL.AppendLine("(PivaSuperUser")
            StrSQL.AppendLine(",LayerElementiGrafici_Cod")
            StrSQL.AppendLine(",TipologiaLayer_cod")
            StrSQL.AppendLine(",TipologiaLayer_struct_cod")
            StrSQL.AppendLine(",LayerElementiGrafici_Etichetta")
            StrSQL.AppendLine(",LayerElementiGrafici_TipoDato")
            StrSQL.AppendLine(",Inviato")
            StrSQL.AppendLine(",DataInvio")
            StrSQL.AppendLine(",Data_Creazione")
            StrSQL.AppendLine(",Data_Modifica")
            StrSQL.AppendLine(",UserName_Creazione")
            StrSQL.AppendLine(",UserName_Modifica")
            StrSQL.AppendLine(",Validita_Inizio")
            StrSQL.AppendLine(",Validita_Fine")
            StrSQL.AppendLine(") ")
            'Valori
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine(String.Format(" '{0}'", Agro_SQL_SaveText(PivaSuperUser)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(TipologiaLayer_cod)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(TipologiaLayer_struct_cod)))
            StrSQL.AppendLine(String.Format(",'{0}'", Agro_SQL_SaveText(LayerElementiGrafici_Etichetta)))
            StrSQL.AppendLine(String.Format(",'{0}'", Agro_SQL_SaveText(LayerElementiGrafici_TipoDato)))
            StrSQL.AppendLine(", 0 ")
            StrSQL.AppendLine(", NULL ")
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveDate(Date.Now)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveDate(Date.Now)))
            StrSQL.AppendLine(String.Format(",'{0}'", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(",'{0}'", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveDate(Validita_Inizio)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveDate(Validita_Fine)))
            StrSQL.AppendLine(")")
            'Esecuzione
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
