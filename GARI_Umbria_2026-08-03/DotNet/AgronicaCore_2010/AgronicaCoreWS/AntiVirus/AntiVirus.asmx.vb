Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Globalization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreScadenziario
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreScadenziario_BIZ
Imports AgronicaCoreUtentiDAL

Imports System
Imports System.Diagnostics
Imports Cloudmersive.APIClient.NET.VirusScan.Api
Imports Cloudmersive.APIClient.NET.VirusScan.Client
Imports Cloudmersive.APIClient.NET.VirusScan.Model



' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class AntiVirus
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CheckVirus(ByVal objP_super_server As String,
                               ByVal strObjJSON As String
                               ) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreScadenziario.AntiVirus.CheckVirus()"

        Dim r As New RispostaStandard()

        Dim AntiVirus_On As Integer = 0
        Dim AntiVirus_Id As Integer = 0
        Dim API_Key As String = ""


        Dim apiInstance = New ScanApi()
        Dim bBypass As Boolean = True


        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim objParametri_Super_Server As AgronicaCoreParametri
            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP_super_server)

            'Lettura Configurazione Antivirus
            Dim xLeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtws As DataTable = xLeggiConfSiti.Leggi(0, "Antivirus_Documentale", "", "", objParametri_Super_Server)

            If dtws.Rows.Count > 0 Then

                Dim valore As String
                Dim antivirusJS As JArray

                valore = dtws.Rows(0)("valore")

                If valore <> "" And valore <> "[]" Then

                    antivirusJS = JArray.Parse(valore)

                    For Each obj As JObject In antivirusJS


                        'Impostazione Parametri
                        If IsNumeric(obj("antivirus_on")) Then
                            AntiVirus_On = obj("antivirus_on")
                        End If
                        If IsNumeric(obj("antivirus_id")) Then
                            AntiVirus_Id = obj("antivirus_id")
                        End If

                        API_Key = obj("antivirus_key")


                        ' API_Key = "74773157-a8b6-487f-8759-cc5ebb854ff8"



                    Next

                End If


            End If

            If AntiVirus_On = 1 Then

                'Scompatto il JSON
                Dim objJson As JObject = JObject.Parse(strObjJSON)


                'Controllo Validità File
                If Trim(objJson("file_allegato")) <> "" Then

                    Dim file_allegato As String = objJson("file_allegato")

                    'Creazione ByteArray
                    Dim fileByteArray As Byte()
                    fileByteArray = Convert.FromBase64String(file_allegato)

                    Dim fs As MemoryStream = New MemoryStream(fileByteArray)


                    Select Case AntiVirus_Id

                        Case 1

                            'Configure API key authorization Apikey
                            Configuration.Default.AddApiKey("Apikey", API_Key)
                            'Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
                            'Configuration.Default.AddApiKeyPrefix("Apikey", "Bearer");


                            ' Scan a file for viruses
                            Dim result As VirusScanResult = apiInstance.ScanFile(fs)

                            Select Case result.CleanResult

                                Case True

                                    r.RispostaOK = True
                                    r.RispostaStringa = ""

                                Case False

                                    r.RispostaOK = True
                                    r.RispostaStringa = "Il file caricato è infetto dal virus: '" & result.FoundViruses(0).VirusName & "."

                            End Select

                        Case Else

                            r.RispostaOK = True
                            r.RispostaStringa = ""

                    End Select

                Else

                    r.RispostaOK = True
                    r.RispostaStringa = "Il file non può essere sottoposto a scansione antivirus."


                End If



            Else

                r.RispostaOK = True
                r.RispostaStringa = ""

            End If


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CheckAntiVirus_isON(ByVal objP_super_server As String
                                        ) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreScadenziario.AntiVirus.CheckAntiVirus_isON()"

        Dim AntiVirus_On As Integer = 0
        Dim r As New RispostaStandard


        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                r.RispostaOK = False
            End If

            Dim objParametri_Super_Server As AgronicaCoreParametri
            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP_super_server)

            'Lettura Configurazione Antivirus
            Dim xLeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtws As DataTable = xLeggiConfSiti.Leggi(0, "Antivirus_Documentale", "", "", objParametri_Super_Server)

            If dtws.Rows.Count > 0 Then

                Dim valore As String
                Dim antivirusJS As JArray

                valore = dtws.Rows(0)("valore")

                If valore <> "" And valore <> "[]" Then

                    antivirusJS = JArray.Parse(valore)

                    For Each obj As JObject In antivirusJS


                        'Impostazione Parametri
                        If IsNumeric(obj("antivirus_on")) Then
                            AntiVirus_On = obj("antivirus_on")
                        End If

                    Next
                End If
            End If

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        r.RispostaStringa = AntiVirus_On.ToString()
        r.RispostaOK = True

        Return r

    End Function

End Class