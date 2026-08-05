Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL

Public Class GHG_Registrazioni_R

    Inherits AgronicaCoreDataProvider.DataProvider

    '#######################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Id_Agenda_GHG As Integer,
                          ByVal Id_GHG_Registrazioni As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '------------------------------------------------------------------
                    strSql.Length = 0
                    strSql.AppendLine("")
                    strSql.AppendLine(" SELECT GHG_Registrazioni.* ")
                    strSql.AppendLine(" FROM  GHG_Registrazioni ")
                    strSql.AppendLine(" WHERE GHG_Registrazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    strSql.AppendLine(" AND   GHG_Registrazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    strSql.AppendLine(" AND   GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")


                    If Piva <> "" Then
                        strSql.AppendLine(" AND GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Id_Agenda_GHG <> 0 Then
                        strSql.AppendLine(" AND GHG_Registrazioni.Id_Agenda_GHG = " & Agro_SQL_SaveNum(Id_Agenda_GHG) & "   ")
                    End If

                    If Id_GHG_Registrazioni <> 0 Then
                        strSql.AppendLine(" AND GHG_Registrazioni.Id_GHG_Registrazioni = " & Agro_SQL_SaveNum(Id_GHG_Registrazioni) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   GHG_Registrazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   GHG_Registrazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY GHG_Registrazioni.Id_Agenda_GHG ASC, GHG_Registrazioni.Id_GHG_Registrazioni ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_From_Id_Mov_Det(ByVal Piva As String,
                                          ByVal Id_Agenda As Integer,
                                          ByVal Id_Mov_Det As Integer,
                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni_R.Leggi_From_Id_Mov_Det()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '------------------------------------------------------------------
                    strSql.Length = 0
                    strSql.AppendLine("")
                    strSql.AppendLine(" SELECT GHG_Registrazioni.* ")
                    strSql.AppendLine(" FROM  GHG_Registrazioni, Mov_Dettagli_Riferimenti ")
                    strSql.AppendLine(" Where GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    strSql.AppendLine(" And   GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    strSql.AppendLine(" And   GHG_Registrazioni.Id_Agenda_GHG = Mov_Dettagli_Riferimenti.Id_Agenda_Rif   ")
                    strSql.AppendLine(" And   Mov_Dettagli_Riferimenti.Piva = GHG_Registrazioni.Piva   ")
                    strSql.AppendLine(" And   Mov_Dettagli_Riferimenti.Id_Agenda =  " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    strSql.AppendLine(" And   Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   GHG_Registrazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   GHG_Registrazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY GHG_Registrazioni.Id_Agenda_GHG ASC, GHG_Registrazioni.Id_GHG_Registrazioni ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiStandard_Factor(ByVal carCod As Integer,
                                         ByVal udmCod As Integer,
                                         ByVal direttivaCod As Integer,
                                         ByVal codiceStato As String,
                                         ByVal xfiltroaggiuntivo As String,
                                         ObjParametri_Server As AgronicaCoreParametri
                                         ) As DataTable

        Dim objCarburanti = New Carburanti_GHG_R()
        Dim dt As New DataTable

        dt = objCarburanti.LeggiStandard_Factor(carCod, udmCod, direttivaCod, codiceStato,
                                                xfiltroaggiuntivo, "", ObjParametri_Server)

        Return dt

    End Function


    Public Function CalcoloStandard_Factor(ByVal qty_trasporto_attuale As String,
                                           ByVal carCod As Integer,
                                           ByVal udmCod As Integer,
                                           ByVal direttivaCod As Integer,
                                           ByVal codiceStato As String,
                                           ByVal xfiltroaggiuntivo As String,
                                           ByVal objParametri_Server As AgronicaCoreParametri
                                           ) As Decimal

        Dim valoreCalcolato As Decimal = 0
        Dim delta As Decimal = 1
        Dim drSearch As DataRow()

        Try

            Dim leggi As New GHG_Registrazioni_R

            xfiltroaggiuntivo = "Udm_Cod in (" & enum_UnitaMisura.Chilometri & "," & enum_UnitaMisura.Miglia & ")"

            Dim dtCarburanti As DataTable = leggi.LeggiStandard_Factor(carCod, 0, direttivaCod, codiceStato,
                                                                       xfiltroaggiuntivo, objParametri_Server)

            If dtCarburanti.Rows.Count > 0 AndAlso IsNumeric(qty_trasporto_attuale) Then

                If udmCod = enum_UnitaMisura.Chilometri OrElse udmCod = enum_UnitaMisura.Miglia Then

                    drSearch = dtCarburanti.Select("Udm_Cod = " & udmCod)

                    If drSearch.Count > 0 Then
                        'Trovata Corrispondenza Udm_Cod                        
                        valoreCalcolato = Format(qty_trasporto_attuale * drSearch(0)("Standard_Factor"), "##0.##")
                    Else

                        delta = 1.60934

                        Select Case dtCarburanti(0)("Udm_Cod")

                            Case enum_UnitaMisura.Chilometri 'da KM a Miglia

                                valoreCalcolato = Format(qty_trasporto_attuale * dtCarburanti(0)("Standard_Factor") * delta, "##0.##")

                            Case enum_UnitaMisura.Miglia ' da Miglia a KM

                                valoreCalcolato = Format(qty_trasporto_attuale * dtCarburanti(0)("Standard_Factor") / delta, "##0.##")

                        End Select

                    End If

                Else
                    'Eccezione
                    valoreCalcolato = Format(qty_trasporto_attuale * drSearch(0)("Standard_Factor"), "##0.##")
                End If

            End If

        Catch ex As Exception
            Throw New Exception($"Impossibile calcolare il fattore di conversione a C02 per il carburante")
        End Try

        Return valoreCalcolato

    End Function

    Public Function LeggiGHG_Colture(ByVal Piva As String,
                                     ByVal Veg_Cod As Integer,
                                     ByRef objParametri_Server As AgronicaCoreParametri
                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni_R.LeggiGHG_Colture()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" Select ")
            strSql.AppendLine(" SUM(m_d.qta2) SUM_HA, sum (GHG_Reg.EEC_Intero) SUM_EEC, sum (GHG_Reg.EEC_Intero) /  SUM(m_d.qta2) EEC_HA ")
            strSql.AppendLine("  From GHG_Registrazioni GHG_Reg ")
            strSql.AppendLine("  Join agenda ag_GHG on ag_GHG.PIVA = GHG_Reg.Piva ")
            strSql.AppendLine("  And ag_GHG.Id_Agenda = GHG_Reg.Id_Agenda_GHG ")
            strSql.AppendLine("  Join Mov_Dettagli_Riferimenti mdr on mdr.Piva_Rif = ag_GHG.Piva ")
            strSql.AppendLine("  And mdr.Id_Agenda_Rif = ag_GHG.Id_Agenda ")
            strSql.AppendLine("  And mdr.Lav_Cod_Rif = ag_GHG.lav_cod ")
            strSql.AppendLine("  Join Agenda ag on mdr.Piva = ag.Piva ")
            strSql.AppendLine("  And mdr.Id_Agenda = ag.Id_Agenda ")
            strSql.AppendLine("  And mdr.Lav_Cod = ag.lav_cod ")
            strSql.AppendLine("  Join Movimenti mov on mov.Piva = ag.Piva ")
            strSql.AppendLine("  And mov.Id_Agenda = ag.Id_Agenda ")
            strSql.AppendLine("  Join Mov_Destinazioni m_d on m_d.Piva = mov.Piva ")
            strSql.AppendLine("  And m_d.Id_Agenda = mov.Id_Agenda ")
            strSql.AppendLine("  And m_d.Id_Mov = mov.Id_Mov ")
            strSql.AppendLine("  Join Reg_Impianti imp on imp.Piva = m_d.Piva ")
            strSql.AppendLine("  And imp.sa_cod = m_d.sa_cod ")
            strSql.AppendLine("  And imp.appezza = m_d.Appezza ")
            strSql.AppendLine("  And imp.id_reg = m_d.Id_Destinazione ")
            strSql.AppendLine("  Join cultivar   on cultivar.cul_cod = imp.cul_cod ")
            strSql.AppendLine("  Where ")
            strSql.AppendLine("  m_d.Tipo_Destinazione = 0 ")
            strSql.AppendLine("  And cultivar.veg_cod =  " & Agro_SQL_SaveNum(Veg_Cod))
            strSql.AppendLine("  And GHG_Reg.EEC_Intero != 0 ")
            strSql.AppendLine("  group by  cultivar.veg_cod  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] :  " & messaggioErrore)
        End Try

        Return dt

    End Function


End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class GHG_Registrazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Id_GHG_Registrazioni As Integer,
                           ByVal Id_Agenda_GHG As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Direttiva_Cod As Integer,
                           ByVal Origine_Imputazione As Integer,
                           ByVal Tipologia As Integer,
                           ByVal Elem_Cod As Integer,
                           ByVal Pro_Cod As Integer,
                           ByVal Mat_Cod As Integer,
                           ByVal Mac_Cod As Integer,
                           ByVal Lotto As String,
                           ByVal Udm_Trasporto_Attuale As Integer,
                           ByVal Qty_Trasporto_Attuale As Decimal,
                           ByVal ETD_Trasporto_Attuale As Decimal,
                           ByVal EEC_Intero As Decimal,
                           ByVal GHG_Total As Decimal,
                           ByVal Dettaglio_JSON As String,
                           ByVal Chain_Custody As Integer,
                           ByVal GHG_Method_Calculation As Integer,
                           ByVal Waste As Integer,
                           ByVal Art29_Compliant As Integer,
                           ByVal Iscc_Red_Compliant As Integer,
                           ByVal Sust_Decl_Date As DateTime,
                           ByVal Sust_Decl_Number As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO GHG_Registrazioni ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          PivaSuperUser, Piva, Id_GHG_Registrazioni, Id_Agenda_GHG, Id_Mov_Det, ")
            strSql.AppendLine("          Direttiva_Cod, Origine_Imputazione, Tipologia, Elem_Cod, Pro_Cod, Mat_Cod, Mac_Cod, ")
            strSql.AppendLine("          Lotto, Udm_Trasporto_Attuale, Qty_Trasporto_Attuale, ETD_Trasporto_Attuale, EEC_Intero, GHG_Total, Dettaglio_JSON, ")
            strSql.AppendLine("          Chain_Custody, GHG_Method_Calculation, Waste, Art29_Compliant, Iscc_Red_Compliant, Sust_Decl_Date, Sust_Decl_Number,  ")
            strSql.AppendLine("          Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_GHG_Registrazioni) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Agenda_GHG) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Direttiva_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Origine_Imputazione) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipologia) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Pro_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mac_Cod) & " ")

            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Lotto) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Udm_Trasporto_Attuale) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qty_Trasporto_Attuale) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ETD_Trasporto_Attuale) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(EEC_Intero) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(GHG_Total) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Dettaglio_JSON) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Chain_Custody) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(GHG_Method_Calculation) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Waste) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Art29_Compliant) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Iscc_Red_Compliant) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Sust_Decl_Date) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Sust_Decl_Number) & "' ")


            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            strSql.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Cancella(ByVal Piva As String,
                             ByVal Id_GHG_Registrazioni As Integer,
                             ByVal Id_Agenda_GHG As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE GHG_Registrazioni ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM GHG_Registrazioni ")
                strSql.AppendLine(" WHERE  1=1 ")

            End If

            strSql.AppendLine(" AND GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND GHG_Registrazioni.Id_Agenda_GHG = " & Agro_SQL_SaveNum(Id_Agenda_GHG) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If


            If Id_GHG_Registrazioni <> 0 Then
                strSql.AppendLine(" AND GHG_Registrazioni.Id_GHG_Registrazioni = " & Agro_SQL_SaveNum(Id_GHG_Registrazioni) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#################################################################
    Public Function ModificaPuntualeNumerico(ByVal Piva As String,
                                             ByVal Id_GHG_Registrazioni As Integer,
                                             ByVal Id_Agenda_GHG As Integer,
                                             ByVal Nome_Campo As String,
                                             ByVal Valore As Decimal,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                             Optional ByVal username_modifica As String = ""
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni.ModificaPuntualeNumerico()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE GHG_Registrazioni ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine("   , " & Nome_Campo & " = " & Agro_SQL_SaveNum(Valore) & " ")

            strSql.AppendLine(" WHERE GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND GHG_Registrazioni.Id_Agenda_GHG = " & Agro_SQL_SaveNum(Id_Agenda_GHG) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_GHG_Registrazioni <> 0 Then
                strSql.AppendLine(" AND GHG_Registrazioni.Id_GHG_Registrazioni = " & Agro_SQL_SaveNum(Id_GHG_Registrazioni) & "   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '#################################################################
    Public Function ModificaPuntualeStringa(ByVal Piva As String,
                                            ByVal Id_GHG_Registrazioni As Integer,
                                            ByVal Id_Agenda_GHG As Integer,
                                            ByVal Nome_Campo As String,
                                            ByVal Valore As String,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                            Optional ByVal username_modifica As String = ""
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni.ModificaPuntualeStringa()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE GHG_Registrazioni ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine("   , " & Nome_Campo & " = '" & Agro_SQL_SaveText(Valore) & "' ")

            strSql.AppendLine(" WHERE GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND GHG_Registrazioni.Id_Agenda_GHG = " & Agro_SQL_SaveNum(Id_Agenda_GHG) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_GHG_Registrazioni <> 0 Then
                strSql.AppendLine(" AND GHG_Registrazioni.Id_GHG_Registrazioni = " & Agro_SQL_SaveNum(Id_GHG_Registrazioni) & "   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '#################################################################
    Public Function ModificaPuntualeData(ByVal Piva As String,
                                         ByVal Id_GHG_Registrazioni As Integer,
                                         ByVal Id_Agenda_GHG As Integer,
                                         ByVal Nome_Campo As String,
                                         ByVal Valore As DateTime,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                         Optional ByVal username_modifica As String = ""
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni.ModificaPuntualeData()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE GHG_Registrazioni ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine("   , " & Nome_Campo & " = " & Agro_SQL_SaveDateTime(Valore) & " ")

            strSql.AppendLine(" WHERE GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND GHG_Registrazioni.Id_Agenda_GHG = " & Agro_SQL_SaveNum(Id_Agenda_GHG) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_GHG_Registrazioni <> 0 Then
                strSql.AppendLine(" AND GHG_Registrazioni.Id_GHG_Registrazioni = " & Agro_SQL_SaveNum(Id_GHG_Registrazioni) & "   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function ModificaProdotto(ByVal Piva As String,
                                     ByVal ID_Agenda_GHG As Integer,
                                     ByVal Id_GHG_Registrazioni As Integer,
                                     ByVal Elem_Cod As Integer,
                                     ByVal Pro_Cod As Integer,
                                     ByVal Mat_Cod As Integer,
                                     ByVal Lotto As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni.ModificaProdotto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE GHG_Registrazioni ")
            strSql.AppendLine(" SET Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            strSql.AppendLine("    ,Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
            strSql.AppendLine("    ,Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            strSql.AppendLine("    ,Lotto = '" & Agro_SQL_SaveText(Lotto) & "' ")

            strSql.AppendLine(" WHERE GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND GHG_Registrazioni.Id_Agenda_GHG = " & Agro_SQL_SaveNum(ID_Agenda_GHG) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_GHG_Registrazioni <> 0 Then
                strSql.AppendLine(" AND GHG_Registrazioni.Id_GHG_Registrazioni = " & Agro_SQL_SaveNum(Id_GHG_Registrazioni) & "   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function AggiornaGHGTotalPQ(ByVal Piva As String,
                                       ByVal Id_Agenda_GHG As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni.AggiornaGHGTotalPQ()"

        Dim messaggioErrore As String = ""
        Dim Scrivi_MPC As New AgronicaCoreContabDAL.Materie_Prime_Campion_W
        Dim oghgtotal As Decimal = 0
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            'Lettura dei parametri della tabella materie_prime_campionature dello scarico
            Dim Leggi_MPC As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R
            Dim DT_MPC As DataTable

            Dim xFiltroAggiuntivo = "lower(Tipo) In ('oghgforec', 'oghgforel', 'oghgforesca', 'oghgforetd')"
            DT_MPC = Leggi_MPC.LeggiPQxGHG(Piva, Id_Agenda_GHG, xFiltroAggiuntivo, "", objParametri)

            'Modifica Parametri ETD
            If DT_MPC.Rows.Count > 0 Then

                For Each dr_MPC In DT_MPC.Rows

                    If IsNumeric(dr_MPC("Val_Cod")) Then
                        oghgtotal = oghgtotal + Replace(dr_MPC("Val_Cod"), ".", ",")
                    End If

                Next

            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE GHG_Registrazioni ")
            strSql.AppendLine(" SET GHG_Total = " & Agro_SQL_SaveNum(oghgtotal) & " + ETD_Trasporto_Attuale ")

            strSql.AppendLine(" WHERE GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND GHG_Registrazioni.Id_Agenda_GHG = " & Agro_SQL_SaveNum(Id_Agenda_GHG) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function ModificaStandardFactor(ByVal Piva As String,
                                           ByVal ID_Agenda As Integer,
                                           ByVal Udm_Trasporto_Attuale As Integer,
                                           ByVal Qty_Trasporto_Attuale As Decimal,
                                           ByVal ETD_Trasporto_Attuale As Decimal,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.GHG_Registrazioni.ModificaStandardFactor()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False
        Dim dt As DataTable

        Try
            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE GHG_Registrazioni ")
            strSql.AppendLine(" SET Udm_Trasporto_Attuale = " & Agro_SQL_SaveNum(Udm_Trasporto_Attuale) & " ")
            strSql.AppendLine("    ,Qty_Trasporto_Attuale = " & Agro_SQL_SaveNum(Qty_Trasporto_Attuale) & " ")
            strSql.AppendLine("    ,ETD_Trasporto_Attuale = " & Agro_SQL_SaveNum(ETD_Trasporto_Attuale) & " ")

            strSql.AppendLine(" WHERE GHG_Registrazioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" And GHG_Registrazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            strSql.AppendLine(" AND GHG_Registrazioni.Id_Agenda_GHG In ( ")

            strSql.AppendLine(" Select Id_Agenda_Rif From Mov_Dettagli_Riferimenti Where Lav_Cod_Rif = " & LAVCOD_GHG)
            strSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" And ID_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & ")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp Then

                'Aggiornamento GHG_Total per tutte le righe del documento
                strSql.Length = 0

                strSql.AppendLine(" Select Id_Agenda_Rif From Mov_Dettagli_Riferimenti Where Lav_Cod_Rif = " & LAVCOD_GHG)
                strSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine(" And ID_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & " ")

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

                If dt.Rows.Count <> 0 Then
                    For Each dr In dt.Rows
                        xRisp = AggiornaGHGTotalPQ(Piva, dr("Id_Agenda_Rif"), objParametri)
                    Next
                End If

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
