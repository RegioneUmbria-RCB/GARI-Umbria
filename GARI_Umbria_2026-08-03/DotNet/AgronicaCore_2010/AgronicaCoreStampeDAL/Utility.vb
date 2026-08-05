Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Public Class Utility
    Inherits AgronicaCoreDataProvider.DataProvider

    'Hide column headers available in page header section
    'HideColonneCrystal(rptBolla, "MyPageHeaderSectionName", ReportObjectKind.FieldHeadingObject, hideColumnNames)

    'Hide column details available in details section
    'HideColonneCrystal(rptBolla, "MyDetailsSectionName", ReportObjectKind.FieldObject, hideColumnNames)

    ''' <summary>
    ''' Hide given section's objects which are type equal to given type
    ''' </summary>
    ''' <param name="myReportDocument">Crystal Report Object</param>
    ''' <param name="sectionName">Nome Sezione</param>
    ''' <param name="hideReportObjectKind">Tipo di Report Object da nascondere</param>
    ''' <param name="hideColumnNames">Lista delle colonne da nascondere</param>
    ''' <param name="objParametri">Parametri Server</param>
    Public Sub HideColonneCrystal(ByRef myReportDocument As ReportDocument,
                                  ByVal sectionName As String,
                                  ByVal hideReportObjectKind As ReportObjectKind,
                                  ByVal hideColumnNames As List(Of String),
                                  ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Utility.HideColonneCrystal()"
        Dim messaggioErrore As String = ""

        Try

            'Cicla tra le colonne di una determinata sezione
            For Each ro As ReportObject In myReportDocument.ReportDefinition.Sections(sectionName).ReportObjects
                'Hide only given type of objects only
                If ro.Kind = hideReportObjectKind And hideColumnNames.Contains(ro.Name) Then
                    'Hide the object by moving too far away
                    ro.Left = 20000

                    'Clear formular field for FieldObject objects
                    If hideReportObjectKind = ReportObjectKind.FieldObject Then
                        Dim myFiledObject As FieldObject = ro

                        Dim myFormulaFieldDefinition As FormulaFieldDefinition = myReportDocument.DataDefinition.FormulaFields(myFiledObject.DataSource.Name)
                        If Not myFormulaFieldDefinition Is Nothing Then
                            'Clear formular field formula text (if available)
                            myFormulaFieldDefinition.Text = String.Empty
                        End If
                    End If
                End If
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


    Public Sub HideShowObjCrystalParameter(ByRef report As Object,
                                           ByVal parameterName As String,
                                           ByVal suppress As Boolean,
                                           ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Utility.HideShowObjCrystalParameter()"
        Dim messaggioErrore As String = ""

        Try
            Dim reportObject = DirectCast(report, ReportClass)
            reportObject.ReportDefinition.ReportObjects(parameterName).ObjectFormat.EnableSuppress = suppress
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    ''' <summary>
    ''' Hide given section's objects which are type equal to given type
    ''' </summary>
    ''' <param name="myReportDocument">Crystal Report Object</param>
    ''' <param name="sectionName">Nome Sezione</param>
    ''' <param name="hideReportObjectKind">Tipo di Report Object da nascondere</param>
    ''' <param name="hideObjNames">Lista degli oggetti da nascondere per questo tipo</param>
    ''' <param name="show">True = Show, False = Hide</param>
    ''' <param name="objParametri">Parametri Server</param>
    Public Sub HideShowObjCrystal(ByRef myReportDocument As ReportDocument,
                                  ByVal sectionName As String,
                                  ByVal hideReportObjectKind As ReportObjectKind,
                                  ByVal hideObjNames As List(Of String),
                                  ByVal show As Boolean,
                                  ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Utility.HideObjCrystal()"
        Dim messaggioErrore As String = ""

        Dim found As Integer = 0

        Try
            Dim repObjs As ReportObjects = Nothing

            'se non mi è arrivata una sezione lo cerco dovunque
            If Not String.IsNullOrEmpty(sectionName) Then
                repObjs = myReportDocument.ReportDefinition.Sections(sectionName).ReportObjects
            Else
                repObjs = myReportDocument.ReportDefinition.ReportObjects
            End If

            'Cicla tra le colonne di una determinata sezione
            For Each ro As ReportObject In repObjs
                'Hide only given type of objects only
                If ro.Kind = hideReportObjectKind And hideObjNames.Contains(ro.Name) Then
                    If show = False Then
                        ro.ObjectFormat.EnableSuppress = True
                    Else
                        ro.ObjectFormat.EnableSuppress = False
                    End If
                    found += 1
                End If
            Next

            If found = 0 Then
                'Scrivi_LOG(objParametri, NomeRoutine, "Impossibile nascondere gli oggetti di tipo " & hideReportObjectKind.ToString & " perché non presenti nella sezione " & sectionName)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    ''' <summary>
    ''' Crea url da usare per lanciare stampa quando non c'è session, passando dalla scrittura della tabella Web_Parametri
    ''' (in cui scrive anche le variabili di sessione)
    ''' </summary>
    Public Function GetUrlStampaDocumento(ByVal piva As String,
                                          ByVal idAgenda As Integer,
                                          ByVal lavCod As Integer,
                                          ByVal report As enum_CodificaStampe,
                                          ByVal progressivoGias As Integer,
                                          ByRef objParametriSuperServer As AgronicaCoreParametri,
                                          ByRef objParametriServer As AgronicaCoreParametri,
                                          ByRef objParametriUtenti As AgronicaCoreParametri,
                                          Optional ByVal sitoOrigine As Enum_SiteRedirector = Enum_SiteRedirector.GiasLan
                                          ) As String

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Utility.GetUrlStampaDocumento()"
        Dim messaggioErrore As String = ""
        Dim url As String = ""

        Try

            Dim objStampe As New AgronicaCoreXML.XML_Stampe
            Dim xmlString As String = objStampe.GeneraXmlStampaDocumento(piva, idAgenda, lavCod, report,
                                                                         progressivoGias, objParametriSuperServer,
                                                                         objParametriServer, objParametriUtenti,
                                                                         sitoOrigine)

            Dim objSatelliti As New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
            Dim uid As String = objSatelliti.ScriviParametriGias(xmlString, objParametriServer, "")

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim base As String = objConfigSiti.Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametriServer)
            Dim stampeUrl As String = objConfigSiti.Leggi_Valore(0, "LinkAgronicaStampe_2010", "", "", objParametriServer)

            Dim objConnessioni As New Connessioni
            Dim databaseId As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_SERVER,
                                                                     "", objParametriServer.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            url = base & stampeUrl & "?idle=idle" &
                  "&unid=" & Sicurezza.Stringa_Codifica(uid, AgroKey_EncoderDecoder, Nothing) &
                  "&cn=" & Sicurezza.Stringa_Codifica(CStr(databaseId), AgroKey_EncoderDecoder, Nothing) &
                  "&StrConSup=" & Sicurezza.Stringa_Codifica(objParametriSuperServer.StringaConnessione, AgroKey_EncoderDecoder, Nothing)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return url

    End Function


    Public Function GetUrlStampaSchedaCampagna(ByVal piva As String,
                                               ByVal report As enum_CodificaStampe,
                                               ByVal VariabiliStampeFiltroXMl As String,
                                               ByVal progressivoGias As Integer,
                                               ByRef objParametriSuperServer As AgronicaCoreParametri,
                                               ByRef objParametriServer As AgronicaCoreParametri,
                                               ByRef objParametriUtenti As AgronicaCoreParametri,
                                               Optional ByVal sitoOrigine As Enum_SiteRedirector = Enum_SiteRedirector.GiasLan
                                               ) As String

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Utility.GetUrlStampaSchedaCampagna()"
        Dim messaggioErrore As String = ""
        Dim url As String = ""

        Try

            Dim objStampe As New AgronicaCoreXML.XML_Stampe
            Dim xmlString As String = objStampe.GeneraXmlStampaSchedaCampagna(piva, VariabiliStampeFiltroXMl, report,
                                                                         progressivoGias, objParametriSuperServer,
                                                                         objParametriServer, objParametriUtenti,
                                                                         sitoOrigine)

            Dim objSatelliti As New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
            Dim uid As String = objSatelliti.ScriviParametriGias(xmlString, objParametriServer, "")

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim base As String = objConfigSiti.Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametriServer)
            Dim stampeUrl As String = objConfigSiti.Leggi_Valore(0, "LinkAgronicaStampe_2010", "", "", objParametriServer)

            Dim objConnessioni As New Connessioni
            Dim databaseId As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_SERVER,
                                                                     "", objParametriServer.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            'base = ""
            'stampeUrl = "http://localhost/AgronicaStampe_2010/GestioneRichieste.aspx"

            url = base & stampeUrl & "?idle=idle" &
                  "&unid=" & Sicurezza.Stringa_Codifica(uid, AgroKey_EncoderDecoder, Nothing) &
                  "&cn=" & Sicurezza.Stringa_Codifica(CStr(databaseId), AgroKey_EncoderDecoder, Nothing) &
                  "&StrConSup=" & Sicurezza.Stringa_Codifica(objParametriSuperServer.StringaConnessione, AgroKey_EncoderDecoder, Nothing)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return url

    End Function




    Public Function GetUrlStampaChecklistGlobalGap(ByVal piva As String,
                                                   ByVal report As enum_CodificaStampe,
                                                   ByVal VariabiliStampeFiltroXMl As String,
                                                   ByVal progressivoGias As Integer,
                                                   ByRef objParametriSuperServer As AgronicaCoreParametri,
                                                   ByRef objParametriServer As AgronicaCoreParametri,
                                                   ByRef objParametriUtenti As AgronicaCoreParametri,
                                                   Optional ByVal sitoOrigine As Enum_SiteRedirector = Enum_SiteRedirector.GiasLan
                                                   ) As String

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Utility.GetUrlStampaChecklistGlobalGap()"
        Dim messaggioErrore As String = ""
        Dim url As String = ""

        Try

            Dim objStampe As New AgronicaCoreXML.XML_Stampe
            Dim xmlString As String = objStampe.GeneraXmlStampaChecklistGlobalGap(piva, VariabiliStampeFiltroXMl, report,
                                                                                  progressivoGias, objParametriSuperServer,
                                                                                  objParametriServer, objParametriUtenti,
                                                                                  sitoOrigine)

            Dim objSatelliti As New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
            Dim uid As String = objSatelliti.ScriviParametriGias(xmlString, objParametriServer, "")

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim base As String = objConfigSiti.Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametriServer)
            Dim stampeUrl As String = objConfigSiti.Leggi_Valore(0, "LinkAgronicaStampe_2010", "", "", objParametriServer)

            Dim objConnessioni As New Connessioni
            Dim databaseId As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_SERVER,
                                                                     "", objParametriServer.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            'base = ""
            'stampeUrl = "http://localhost/AgronicaStampe_2010/GestioneRichieste.aspx"

            url = base & stampeUrl & "?idle=idle" &
                  "&unid=" & Sicurezza.Stringa_Codifica(uid, AgroKey_EncoderDecoder, Nothing) &
                  "&cn=" & Sicurezza.Stringa_Codifica(CStr(databaseId), AgroKey_EncoderDecoder, Nothing) &
                  "&StrConSup=" & Sicurezza.Stringa_Codifica(objParametriSuperServer.StringaConnessione, AgroKey_EncoderDecoder, Nothing)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return url

    End Function


End Class
