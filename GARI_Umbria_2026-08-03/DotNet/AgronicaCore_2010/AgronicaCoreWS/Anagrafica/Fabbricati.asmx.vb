Imports System.Web.Services
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreAnagrafeBIZ
Imports InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Fabbricati
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function LeggiFabbricati_NG(ByVal InData As CoreWS_Generic(Of LeggiFabbricati)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xFiltroAggiuntivo As String = ""
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try
            Dim dtUbic As New DataTable
            dtUbic.Columns.Add(New DataColumn("key_Dest", GetType(String)))
            dtUbic.Columns.Add(New DataColumn("Tipo_Destinazione", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Ubic_Des", GetType(String)))

            If InData.InData.CelleMagazzini <> "M" Then
                'Celle
                Dim dbAccessVasche As New Cantina_Vasche_R
                Dim dt = dbAccessVasche.Leggi(InData.InData.piva, InData.InData.sa_cod, 0, "", "", objParametri_Server)

                For Each drCella As DataRow In dt.Rows
                    Dim dr = dtUbic.NewRow
                    dr.Item("Tipo_Destinazione") = drCella.Item("Tipo_Destinazione")
                    dr.Item("Sa_Cod") = drCella.Item("Sa_Cod")
                    dr.Item("Id_Destinazione") = drCella.Item("Vas_Cod")
                    dr.Item("Ubic_Des") = drCella.Item("Identificativo")
                    dr.Item("key_Dest") = drCella.Item("Tipo_Destinazione").ToString + "_" + drCella.Item("Sa_Cod").ToString + "_" + drCella.Item("Vas_Cod").ToString
                    dtUbic.Rows.Add(dr)
                Next
            End If

            If InData.InData.CelleMagazzini <> "C" Then
                'Magazzini
                Select Case InData.InData.Tipo_Fabbricato

                    Case enum_FabbricatiTipi.essiccatoio
                        xFiltroAggiuntivo = " (Fabbricati.Tipo_Fabbricato_Cod =" & CInt(enum_FabbricatiTipi.essiccatoio) & ") "

                    Case AgronicaCoreDataProvider.CostantiPersonalizzate.MAGAZZINO
                        xFiltroAggiuntivo = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

                    Case AgronicaCoreDataProvider.CostantiPersonalizzate.STALLA
                        xFiltroAggiuntivo = " ( (Fabbricati.Tipo_Fabbricato_Cod >= 170 AND Fabbricati.Tipo_Fabbricato_Cod < 180) OR Fabbricati.Tipo_Fabbricato_Cod = 70 ) "

                    Case AgronicaCoreDataProvider.CostantiPersonalizzate.FABBRICATI_NO_STALLE
                        xFiltroAggiuntivo = " (Fabbricati.Tipo_Fabbricato_Cod < 170 OR Fabbricati.Tipo_Fabbricato_Cod >= 180) AND Fabbricati.Tipo_Fabbricato_Cod <> 70  "

                End Select

                Dim dbAccessFabbricati As New Fabbricati_R
                Dim dt = dbAccessFabbricati.Leggi_2(InData.InData.piva, InData.InData.sa_cod, 0, 0, xFiltroAggiuntivo, "", objParametri_Server)

                For Each drFab As DataRow In dt.Rows
                    Dim dr = dtUbic.NewRow

                    Dim tipoDestinazione As Integer = If(InData.InData.Forza_Tipo_Destinazione_Magazzino, MAGAZZINO, drFab.Item("Tipo_Fabbricato_Cod"))

                    dr.Item("Tipo_Destinazione") = tipoDestinazione
                    dr.Item("Sa_Cod") = drFab.Item("Sa_Cod")
                    dr.Item("Id_Destinazione") = drFab.Item("Fabbricato_Cod")
                    If String.IsNullOrEmpty(drFab.Item("Sa_Nome")) Then
                        dr.Item("Ubic_Des") = drFab.Item("Fabbricato_Des")
                    Else
                        dr.Item("Ubic_Des") = drFab.Item("Fabbricato_Des") & " (" & drFab.Item("Sa_Nome") & ")"
                    End If
                    dr.Item("key_Dest") = tipoDestinazione & "_" & drFab.Item("Sa_Cod") & "_" & drFab.Item("Fabbricato_Cod")
                    dtUbic.Rows.Add(dr)
                Next
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtUbic, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiFabbricati(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal sa_cod As Integer, ByVal CelleMagazzini As String, ByVal Tipo_Fabbricato As Integer, ByVal Forza_Tipo_Destinazione_Magazzino As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xFiltroAggiuntivo As String = ""
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim dtUbic As New DataTable
            dtUbic.Columns.Add(New DataColumn("key_Dest", GetType(String)))
            dtUbic.Columns.Add(New DataColumn("Tipo_Destinazione", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Ubic_Des", GetType(String)))

            If CelleMagazzini <> "M" Then
                'Celle
                Dim dbAccessVasche As New Cantina_Vasche_R
                Dim dt = dbAccessVasche.Leggi(piva, sa_cod, 0, "", "", objParametri_Server)

                For Each drCella As DataRow In dt.Rows
                    Dim dr = dtUbic.NewRow
                    dr.Item("Tipo_Destinazione") = drCella.Item("Tipo_Destinazione")
                    dr.Item("Sa_Cod") = drCella.Item("Sa_Cod")
                    dr.Item("Id_Destinazione") = drCella.Item("Vas_Cod")
                    dr.Item("Ubic_Des") = drCella.Item("Identificativo")
                    dr.Item("key_Dest") = drCella.Item("Tipo_Destinazione").ToString + "_" + drCella.Item("Sa_Cod").ToString + "_" + drCella.Item("Vas_Cod").ToString
                    dtUbic.Rows.Add(dr)
                Next
            End If

            If CelleMagazzini <> "C" Then
                'Magazzini
                Select Case Tipo_Fabbricato

                    Case enum_FabbricatiTipi.essiccatoio
                        xFiltroAggiuntivo = " (Fabbricati.Tipo_Fabbricato_Cod =" & CInt(enum_FabbricatiTipi.essiccatoio) & ") "

                    Case AgronicaCoreDataProvider.CostantiPersonalizzate.MAGAZZINO
                        xFiltroAggiuntivo = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

                    Case AgronicaCoreDataProvider.CostantiPersonalizzate.STALLA
                        xFiltroAggiuntivo = " ( (Fabbricati.Tipo_Fabbricato_Cod >= 170 AND Fabbricati.Tipo_Fabbricato_Cod < 180) OR Fabbricati.Tipo_Fabbricato_Cod = 70 ) "

                    Case AgronicaCoreDataProvider.CostantiPersonalizzate.FABBRICATI_NO_STALLE
                        xFiltroAggiuntivo = " (Fabbricati.Tipo_Fabbricato_Cod < 170 OR Fabbricati.Tipo_Fabbricato_Cod >= 180) AND Fabbricati.Tipo_Fabbricato_Cod <> 70  "

                End Select

                Dim dbAccessFabbricati As New Fabbricati_R
                Dim dt = dbAccessFabbricati.Leggi_2(piva, sa_cod, 0, 0, xFiltroAggiuntivo, "", objParametri_Server)

                For Each drFab As DataRow In dt.Rows
                    Dim dr = dtUbic.NewRow

                    Dim tipoDestinazione As Integer = If(Forza_Tipo_Destinazione_Magazzino, MAGAZZINO, drFab.Item("Tipo_Fabbricato_Cod"))

                    dr.Item("Tipo_Destinazione") = tipoDestinazione
                    dr.Item("Sa_Cod") = drFab.Item("Sa_Cod")
                    dr.Item("Id_Destinazione") = drFab.Item("Fabbricato_Cod")
                    If String.IsNullOrEmpty(drFab.Item("Sa_Nome")) Then
                        dr.Item("Ubic_Des") = drFab.Item("Fabbricato_Des")
                    Else
                        dr.Item("Ubic_Des") = drFab.Item("Fabbricato_Des") & " (" & drFab.Item("Sa_Nome") & ")"
                    End If
                    dr.Item("key_Dest") = tipoDestinazione & "_" & drFab.Item("Sa_Cod") & "_" & drFab.Item("Fabbricato_Cod")
                    dtUbic.Rows.Add(dr)
                Next
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtUbic, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    '<WebMethod(EnableSession:=True)>
    'Public Function LeggiFabbricatiOmni(ByVal objP_server As String, ByVal objP_utenti As String, piva As String, sa_cod As Integer, strfiltro As String) As RispostaStandard
    <WebMethod(EnableSession:=True)>
    Public Function LeggiFabbricatiOmni_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.LeggiFabbricatiOmni)) As RispostaStandard
        Dim Dt As DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Dim r As New RispostaStandard

        Try

            Dt = objFabbricati.Leggi(InData.InData.piva, InData.InData.sa_cod, 0, enumSelezioneVariabile.Selezione_LogOmni, InData.InData.strfiltro, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function LeggiFabbricatiOmni(ByVal objP_server As String, ByVal objP_utenti As String, piva As String, sa_cod As Integer, strfiltro As String) As RispostaStandard
        Dim Dt As DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Dim r As New RispostaStandard

        Try

            Dt = objFabbricati.Leggi(piva, sa_cod, 0, enumSelezioneVariabile.Selezione_LogOmni, strfiltro, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiMagazzini_APP(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal Data As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            ' Magazzini APP
            Dim filtroMagazziniAPP As Boolean = True
            Dim listaMagazziniAPP As New List(Of String)
            Dim leggi_fabbricati_codice As New Fabbricati_Codici_R
            Dim magAPP = leggi_fabbricati_codice.Leggi(piva, 0, 0, CInt(enum_CodiciAnagrafe.Visibile_da_App), "1", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            For Each row In magAPP.Rows
                listaMagazziniAPP.Add(row.Item("PIVA") & "_" & row.Item("sa_cod") & "_" & row.Item("Fabbricato_cod"))
            Next

            Dim dtUbic As New DataTable
            'dtUbic.Columns.Add(New DataColumn("key_Dest", GetType(String)))
            dtUbic.Columns.Add(New DataColumn("Piva", GetType(String)))
            dtUbic.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Tipo_Destinazione", GetType(Integer)))
            dtUbic.Columns.Add(New DataColumn("Ubic_Des", GetType(String)))
            dtUbic.Columns.Add(New DataColumn("Visibile_da_App", GetType(Integer)))

            Dim dbAccessFabbricati As New Fabbricati_R
            Dim xFiltroAggiuntivo As String = " Fabbricati.Tipo_Fabbricato_Cod IN (20,50,120,121,122,123) "
            Dim dt = dbAccessFabbricati.Leggi_3(piva, 0, 0, xFiltroAggiuntivo, "", objParametri_Server)

            For Each drFab As DataRow In dt.Rows
                Dim keyFab = drFab.Item("PIVA") & "_" & drFab.Item("sa_cod") & "_" & drFab.Item("Fabbricato_cod")
                If Not filtroMagazziniAPP OrElse listaMagazziniAPP.Contains(keyFab) Then
                    Dim dr = dtUbic.NewRow
                    dr.Item("Piva") = drFab.Item("Piva")
                    dr.Item("Sa_Cod") = drFab.Item("Sa_Cod")
                    dr.Item("Id_Destinazione") = drFab.Item("Fabbricato_Cod")
                    dr.Item("Tipo_Destinazione") = drFab.Item("Tipo_Fabbricato_Cod")
                    dr.Item("Ubic_Des") = drFab.Item("Fabbricato_Des")
                    dr.Item("Visibile_da_App") = If(listaMagazziniAPP.Contains(keyFab), 1, 0)
                    'dr.Item("key_Dest") = drFab.Item("Tipo_Fabbricato_Cod").ToString + "_" + drFab.Item("Sa_Cod").ToString + "_" + drFab.Item("Fabbricato_Cod").ToString
                    dtUbic.Rows.Add(dr)
                End If
            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtUbic, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod()>
    Public Function LeggiMagazziniAPP(InData As Object) As rispostaStandard(Of List(Of Fabbricato))

        Dim r As New rispostaStandard(Of List(Of Fabbricato))

        Try

            InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiMagazzini))(JsonConvert.SerializeObject(InData))

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim request As LeggiMagazzini = InData.InData
            Dim piva As String = request.impresa.partitaIva
            Dim filtroMagazziniAPP As Boolean = request.filtroMagazziniAPP

            Dim objFabbricati As New Fabbricato_R
            Dim listMagazzini = objFabbricati.Leggi_Fabbricati_APP(piva, 0, objParametri_Server, filtroMagazziniAPP)

            r.RispostaStringa = listMagazzini
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function ScriviMagazziniAPP(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of Fabbricato)))(JsonConvert.SerializeObject(InData))

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objFabbricati As New Fabbricato_W
            Dim magazzini As List(Of Fabbricato) = InData.InData
            objFabbricati.ScriviMagazzini(magazzini, objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = "Salvataggio eseguito correttamente"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Magazzini_Organismoreferente_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim Dt_Mag As DataTable

            Dim FiltroAggiuntivoMag As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

            Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente(piva,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    FiltroAggiuntivoMag,
                                                                    "",
                                                                    objParametriServer)


            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In Dt_Mag.Rows

                jArrayListaOp.Add(New JObject(New JProperty("text", CStr(dr("Fabbricato_Cod")) & "|" & CStr(dr("Sa_Cod") & "|" & CStr(dr("Piva")))),
                                              New JProperty("value", CStr(dr("Fabbricato_Des")) & " (" & CStr(dr("rag_soc")) & ")")))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Magazzini_Organismoreferente(ByVal objP_server As String,
                                           ByVal piva As String
                                           ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim Dt_Mag As DataTable

            Dim FiltroAggiuntivoMag As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

            Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente(piva,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    FiltroAggiuntivoMag,
                                                                    "",
                                                                    objParametriServer)


            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In Dt_Mag.Rows

                jArrayListaOp.Add(New JObject(New JProperty("text", CStr(dr("Fabbricato_Cod")) & "|" & CStr(dr("Sa_Cod") & "|" & CStr(dr("Piva")))),
                                              New JProperty("value", CStr(dr("Fabbricato_Des")) & " (" & CStr(dr("rag_soc")) & ")")))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Carica_Stalle(ByVal piva As String,
                                         ByVal sa_cod As Integer,
                                         ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim gefutils As New Gias_EF_Utility

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Dim lista_stalle As String

            If sa_cod = 0 Then
                lista_stalle = Newtonsoft.Json.JsonConvert.SerializeObject((From s In GiasContext.Stalla Where s.PIVA = piva).ToList)
            Else
                lista_stalle = Newtonsoft.Json.JsonConvert.SerializeObject((From s In GiasContext.Stalla Where s.PIVA = piva And s.sa_cod = sa_cod).ToList)
            End If

            r.RispostaStringa = lista_stalle
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally

            scope.Complete()
            scope.Dispose()
            GiasContext.Dispose()

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Carica_Stalle_NG(ByVal InData As CoreWS_Generic(Of Carica_Stalle)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim gefutils As New Gias_EF_Utility

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Dim lista_stalle As String

            If InData.InData.sa_cod = 0 Then
                lista_stalle = Newtonsoft.Json.JsonConvert.SerializeObject((From s In GiasContext.Stalla Where s.PIVA = InData.InData.Piva).ToList)
            Else
                lista_stalle = Newtonsoft.Json.JsonConvert.SerializeObject((From s In GiasContext.Stalla Where s.PIVA = InData.InData.Piva And s.sa_cod = InData.InData.sa_cod).ToList)
            End If

            r.RispostaStringa = lista_stalle
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally

            scope.Complete()
            scope.Dispose()
            GiasContext.Dispose()

        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)> <Script.Services.ScriptMethod()>
    Public Function Leggi_Fabbricati_Anagrafica(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametriAgenda = InData

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If InData.InData.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = InData.InData.Data.Date
                objParametri_Server.FinestraTemporaleFine = InData.InData.Data.Date
            End If

            Dim dt As DataTable = objFabbricati.Leggi_x_anagraficaNG(InData.InData.Piva, InData.InData.Sa_Cod, 0, "", "", objParametri_Server, objParametri_Utenti)

            If InData.InData.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)> <Script.Services.ScriptMethod()>
    Public Function Leggi_Fabbricati_Anagrafica_CodDescr(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If InData.InData.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = InData.InData.Data.Date
                objParametri_Server.FinestraTemporaleFine = InData.InData.Data.Date
            End If

            Dim stringOrDefault = Function(dr As DataRow, field As String) If(IsDBNull(dr(field)), String.Empty, CStr(dr(field)))

            Dim dt = objFabbricati.Leggi_x_anagraficaNG(
                InData.InData.Piva, InData.InData.Sa_Cod, 0,
                "", "", objParametri_Server, objParametri_Utenti
            )
            Dim magazzini As IEnumerable(Of BaseCodeDescrStr) = dt.Select.Select(Function(row) New BaseCodeDescrStr(
                row("chiave").ToString.Replace("_", "|"),
                stringOrDefault(row, "Fabbricato_Des") & " (" & stringOrDefault(row, "Sa_Nome") & ")"
            ))

            If InData.InData.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(magazzini, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Magazzini_QdC(InData As CoreWS_Generic(Of LeggiMagazzini_QdC)) As rispostaStandard(Of List(Of Fabbricato))

        Dim r As New rispostaStandard(Of List(Of Fabbricato))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva As String = InData.InData.impresa.partitaIva

            Dim Lav_Cod As Integer = InData.InData.lavorazione.primaryKey.codice

            Dim Data_Operazione As Date = CDate(InData.InData.data)

            Dim objFabbbricato As New AgronicaCoreAnagrafeBIZ.Fabbricato_R
            Dim magazziniList = objFabbbricato.Leggi_Magazzini_QdC(Piva,
                                                                   0,
                                                                   Lav_Cod,
                                                                   Data_Operazione,
                                                                   objParametri_Server,
                                                                   objParametri_Utenti)
            r.RispostaStringa = magazziniList
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Ultimo_Magazzino_Prodotto_Movimentato(InData As CoreWS_Generic(Of LeggiUltimo_Magazzino_Prodotto_Movimentato)) As rispostaStandard(Of Fabbricato)
        Dim r As New rispostaStandard(Of Fabbricato)

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try
            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)


            Dim data_fine = InData.InData.data

            Dim data_inizio = AGRODATAINIZIO

            Dim Piva = InData.InData.impresa.partitaIva

            Dim Pro_Cod = 0

            Dim Mat_Cod = 0

            Dim Elem_Cod = 0

            If Not IsNothing(InData.InData.dettaglioFertilizzazione) Then
                Pro_Cod = InData.InData.dettaglioFertilizzazione.prodotto.codice
                Elem_Cod = InData.InData.dettaglioFertilizzazione.prodotto.elemCod
            End If

            If Not IsNothing(InData.InData.dettaglioTrattamento) Then
                Pro_Cod = InData.InData.dettaglioTrattamento.prodotto.codice
                Elem_Cod = InData.InData.dettaglioTrattamento.prodotto.elemCod
            End If

            If Not IsNothing(InData.InData.dettaglioSemina) Then
                Mat_Cod = InData.InData.dettaglioSemina.prodotto.codice
                Elem_Cod = InData.InData.dettaglioSemina.prodotto.elemCod
            End If

            Dim objFabbricato As New AgronicaCoreAnagrafeBIZ.Fabbricato_R

            Dim Magazzino = objFabbricato.Ultimo_Magazzino_Prodotto_Movimentato(data_inizio, data_fine, Piva, 0, Elem_Cod, Pro_Cod, Mat_Cod, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True

            r.RispostaStringa = Magazzino

        Catch ex As Exception

            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

End Class