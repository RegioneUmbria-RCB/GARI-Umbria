Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class Compliance_ISCC_R
    Inherits DataProvider

    Public Function LeggiElencoElaborazioni(ByVal ID As Integer,
                                            ByVal piva As String,
                                            ByVal esito As Integer,
                                            ByVal Data_Controllo As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Compliance_ISCC_R.LeggiElencoElaborazioni()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0

            Stb.AppendLine("Select ")
            Stb.AppendLine("    ID_Elaborazione,")
            Stb.AppendLine("    PIVA,")
            Stb.AppendLine("    Esito_Cod,")
            Stb.AppendLine("    Data_Controllo,")
            Stb.AppendLine("    USername_Controllo")
            Stb.AppendLine("from ")
            Stb.AppendLine("    Imprese_ISCCXElaborazioni")
            Stb.AppendLine("Where ")
            Stb.AppendLine("    1=1 ")

            If ID <> 0 Then
                Stb.AppendLine(String.Format(" and id_elaborazione={0}", Agro_SQL_SaveNum(ID)))
            End If

            If piva <> "" Then
                Stb.AppendLine(String.Format(" and piva='{0}'", Agro_SQL_SaveText(piva)))
            End If

            If esito <> -1 Then
                Stb.AppendLine(String.Format(" and esito_cod={0}", Agro_SQL_SaveNum(esito)))
            End If

            If Data_Controllo <> AGRODATAINIZIO Then
                Stb.AppendLine(String.Format(" and data_controllo={0}", Agro_SQL_SaveDate(Data_Controllo)))
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server)))
            End If

            If xOrderBy <> "" Then
                Stb.AppendLine(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Server)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function

    Public Function LeggiElencoRichiesteXElaborazione(ByVal ID As Integer,
                                                      ByVal piva As String,
                                                      ByVal reqnum As Integer,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByVal xOrderBy As String,
                                                      ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Compliance_ISCC_R.LeggiElencoRichiesteXElaborazione()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0

            Stb.AppendLine("Select ")
            Stb.AppendLine("    ID_Elaborazione,")
            Stb.AppendLine("    PIVA,")
            Stb.AppendLine("    GIS_LayerAnalysisConfig_Exec_Log_Cod, ")
            Stb.AppendLine("    Esito_Elaborazione ")
            Stb.AppendLine("from ")
            Stb.AppendLine("    Imprese_ISCCXElaborazioniXRichieste")
            Stb.AppendLine("Where ")
            Stb.AppendLine("    1=1 ")

            If ID <> 0 Then
                Stb.AppendLine(String.Format(" and id_elaborazione={0}", Agro_SQL_SaveNum(ID)))
            End If

            If piva <> "" Then
                Stb.AppendLine(String.Format(" and piva='{0}'", Agro_SQL_SaveText(piva)))
            End If

            If reqnum <> 0 Then
                Stb.AppendLine(String.Format(" and reqnum={0}", Agro_SQL_SaveNum(reqnum)))
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server)))
            End If

            If xOrderBy <> "" Then
                Stb.AppendLine(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Server)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function

    Public Function LeggiElencoEntitaDaPianoColturale(ByVal piva As String,
                                                      ByVal sa_Cod As Integer,
                                                      ByVal data_ref As Date,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByVal xOrderBy As String,
                                                      ByRef ObjParametri_Server As AgronicaCoreParametri
                                                      ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiElencoEntitaDaPianoColturale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" select ")
            StrSQL.AppendLine(" 	a.PIVA, ")
            StrSQL.AppendLine(" 	a.SA_COD, ")
            StrSQL.AppendLine(" 	a.APPEZZA, ")
            StrSQL.AppendLine(" 	a.ID_REG, ")
            StrSQL.AppendLine(" 	b.Entita_Cod ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine(" 	reg_impianti a inner join ")
            StrSQL.AppendLine(" 	GIS_Entita b ")
            StrSQL.AppendLine(" 	on (a.PIVA=b.Piva and a.SA_COD=b.Sa_Cod and a.APPEZZA=b.Appezza and a.ID_REG=b.Id_Imp) ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine(" 	1=1 ")

            If piva <> "" Then
                StrSQL.AppendLine(String.Format(" 	and a.PIVA='{0}' ", Agro_SQL_SaveText(piva)))
            End If
            If sa_Cod <> 0 Then
                StrSQL.AppendLine(String.Format(" 	and a.sa_cod={0} ", Agro_SQL_SaveNum(sa_Cod)))
            End If
            If data_ref <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format(" 	and (a.Validita_Inizio<={0} and a.Validita_Fine>={0}) ", Agro_SQL_SaveDate(data_ref)))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(String.Format(" order by {0} ", Agro_SQL_Save_xOrderBy(xOrderBy)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function


End Class
Public Class Compliance_ISCC_W
    Inherits DataProvider

    Public Function ScriviElaborazioneAziendaISCC(ByVal ID_Elaborazione As Integer,
                                                  ByVal piva As String,
                                                  ByVal DataControllo As DateTime,
                                                  ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                  Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                                  Optional ByVal Validita_Fine As DateTime = AGRODATAFINE
                                                  ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Compliance_ISCC_W.ScriviElaborazioneAziendaISCC()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            Stb.Length = 0

            Stb.AppendLine("insert into Imprese_ISCCXElaborazioni (")
            Stb.AppendLine("    ID_Elaborazione,")
            Stb.AppendLine("    PIVA,")
            Stb.AppendLine("    Esito_Cod,")
            Stb.AppendLine("    Data_Controllo,")
            Stb.AppendLine("    Username_Controllo,")
            Stb.AppendLine("    Username_Creazione,")
            Stb.AppendLine("    Data_Creazione,")
            Stb.AppendLine("    Username_Modifica,")
            Stb.AppendLine("    Data_Modifica")
            Stb.AppendLine(" ) VALUES ( ")
            Stb.AppendLine(String.Format("  {0} ", Agro_SQL_SaveNum(ID_Elaborazione)))
            Stb.AppendLine(String.Format(" ,'{0}' ", Agro_SQL_SaveText(piva)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(-1)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(DataControllo)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function ScriviRichiestaElaborazioneAlgoitmoPerCompliance_ISCC_Con_Chiave_Impianto(ByVal ID_Elaborazione As Integer,
                                                                                              ByVal piva As String,
                                                                                              ByVal sa_cod As Integer,
                                                                                              ByVal appezza As Integer,
                                                                                              ByVal id_reg As Integer,
                                                                                              ByVal ReqNum As Integer,
                                                                                              ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                                                              Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                                                                              Optional ByVal Validita_Fine As DateTime = AGRODATAFINE
                                                                                              ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Compliance_ISCC_W.ScriviLinkElaborazioneAziendaISCC_RequestAlgorithm()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            Stb.Length = 0

            Stb.AppendLine("insert into Imprese_ISCCXElaborazioniXRichieste (")
            Stb.AppendLine("    ID_Elaborazione,")
            Stb.AppendLine("    PIVA,")
            Stb.AppendLine("    Sa_Cod,")
            Stb.AppendLine("    Appezza,")
            Stb.AppendLine("    Id_Reg,")
            Stb.AppendLine("    GIS_LayerAnalysisConfig_Exec_Log_Cod,")
            Stb.AppendLine("    Esito_Elaborazione,")
            Stb.AppendLine("    Username_Creazione,")
            Stb.AppendLine("    Data_Creazione,")
            Stb.AppendLine("    Username_Modifica,")
            Stb.AppendLine("    Data_Modifica")
            Stb.AppendLine(" ) VALUES ( ")
            Stb.AppendLine(String.Format("  {0} ", Agro_SQL_SaveNum(ID_Elaborazione)))
            Stb.AppendLine(String.Format(" ,'{0}' ", Agro_SQL_SaveText(piva)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(sa_cod)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(appezza)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(id_reg)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(ReqNum)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(-1)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function ModificaElaborazioneAziendaISCC(ByVal ID_Elaborazione As Integer,
                                                    ByVal piva As String,
                                                    ByVal Esito_cod As Integer,
                                                    ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                    Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                                    Optional ByVal Validita_Fine As DateTime = AGRODATAFINE
                                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Compliance_ISCC_W.ModificaElaborazioneAziendaISCC()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            Stb.Length = 0

            Stb.AppendLine("update Imprese_ISCCXElaborazioni set")
            Stb.AppendLine(String.Format("      Esito_Cod={0} ", Agro_SQL_SaveNum(Esito_cod)))
            Stb.AppendLine(String.Format("    , Data_Modifica={0} ", Agro_SQL_SaveDate(DateTime.Now)))
            Stb.AppendLine(String.Format("    , Username_Modifica='{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine("Where ")
            Stb.AppendLine("    1=1 ")

            If ID_Elaborazione <> 0 Then
                Stb.AppendLine(String.Format(" and ID_Elaborazione={0}", Agro_SQL_SaveNum(ID_Elaborazione)))
            End If

            If piva <> "" Then
                Stb.AppendLine(String.Format(" and piva='{0}'", Agro_SQL_SaveText(piva)))
            End If

            'solo le elaborazioni con stato pending possono essere aggiornate
            Stb.AppendLine(String.Format(" and Esito_Cod={0}", Agro_SQL_SaveNum(-1)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function ModificaRichiestaElaborazioneEntitaISCC(ByVal ID_Elaborazione As Integer,
                                                            ByVal piva As String,
                                                            ByVal sa_cod As Integer,
                                                            ByVal appezza As Integer,
                                                            ByVal id_Reg As Integer,
                                                            ByVal reqnum As Integer,
                                                            ByVal Esito_cod As Integer,
                                                            ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                            Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                                            Optional ByVal Validita_Fine As DateTime = AGRODATAFINE
                                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Compliance_ISCC_W.ModificaRichiestaElaborazioneEntitaISCC()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            Stb.Length = 0

            Stb.AppendLine("update Imprese_ISCCXElaborazioniXRichieste set")
            Stb.AppendLine(String.Format("      Esito_Elaborazione={0} ", Agro_SQL_SaveNum(Esito_cod)))
            Stb.AppendLine(String.Format("    , Data_Modifica={0} ", Agro_SQL_SaveDate(DateTime.Now)))
            Stb.AppendLine(String.Format("    , Username_Modifica='{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine("Where ")
            Stb.AppendLine("    1=1 ")

            If ID_Elaborazione <> 0 Then
                Stb.AppendLine(String.Format(" and ID_Elaborazione={0}", Agro_SQL_SaveNum(ID_Elaborazione)))
            End If

            If piva <> "" Then
                Stb.AppendLine(String.Format(" and piva='{0}'", Agro_SQL_SaveText(piva)))
            End If

            If sa_cod <> 0 Then
                Stb.AppendLine(String.Format(" and sa_cod={0}", Agro_SQL_SaveNum(sa_cod)))
            End If

            If appezza <> 0 Then
                Stb.AppendLine(String.Format(" and appezza={0}", Agro_SQL_SaveNum(appezza)))
            End If

            If id_Reg <> 0 Then
                Stb.AppendLine(String.Format(" and id_Reg={0}", Agro_SQL_SaveNum(id_Reg)))
            End If

            If reqnum <> 0 Then
                Stb.AppendLine(String.Format(" and GIS_LayerAnalysisConfig_Exec_Log_Cod={0}", Agro_SQL_SaveNum(reqnum)))
            End If

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function AggiornaDatiComplianceISCCSuImpresa(ByVal piva As String,
                                                        ByVal Esito As Integer,
                                                        ByVal DataRiferimento As DateTime,
                                                        ByRef ObjParametri_Server As AgronicaCoreParametri
                                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Compliance_ISCC_W.AggiornaDatiComplianceISCCSuImpresa()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        If piva = "" Then
            Throw New Exception("Specificare la partita iva")
        End If

        Try

            Stb.Length = 0

            Stb.AppendLine("update Imprese set")
            Stb.AppendLine(String.Format("      Compliance_ISCC={0} ", Agro_SQL_SaveNum(Esito)))
            Stb.AppendLine(String.Format("    , Compliance_ISCC_DataRiferimento={0} ", Agro_SQL_SaveDateTime(DataRiferimento)))
            Stb.AppendLine(String.Format("    , Compliance_ISCC_Username='{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format("    , Data_Modifica={0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(String.Format("    , Username_Modifica='{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine("Where ")
            Stb.AppendLine("    1=1 ")

            If piva <> "" Then
                Stb.AppendLine(String.Format(" and piva='{0}'", Agro_SQL_SaveText(piva)))
            End If

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function Cancella() As Boolean

    End Function

End Class
