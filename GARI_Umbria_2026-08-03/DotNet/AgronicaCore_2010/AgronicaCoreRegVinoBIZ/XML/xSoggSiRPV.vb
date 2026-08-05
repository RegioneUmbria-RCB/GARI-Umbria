Imports AgronicaCoreDataProvider
Imports AgronicaCoreRegVinoDAL
Imports System.Xml.Serialization
Imports System.IO
Imports System.Text
Imports System.Xml
Imports AgronicaCoreUtility


Public Class xSoggSiRPV

    Dim soggReader As xDBSoggSiRPV_R
    Dim soggWriter As xDBSoggSiRPV_W
    Dim teleregistriManager As TeleregistriManager

    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Sub New(server As AgronicaCoreDataProvider.AgronicaCoreParametri, utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, manager As TeleregistriManager)
        ObjParametri_Server = server
        ObjParametri_Utenti = utenti
        teleregistriManager = manager
        soggReader = New AgronicaCoreRegVinoDAL.xDBSoggSiRPV_R()
        soggWriter = New AgronicaCoreRegVinoDAL.xDBSoggSiRPV_W()
    End Sub

    Public Function sSoggSiRPV(ByVal Username As String, ByVal Password As String, _
                                      ByVal codiciSoggetti As List(Of String), _
                                      ByVal tipoRichiesta As Integer, _
                                      ByVal codOperCF As String, _
                                      ByVal codOperPersonaFisica As Boolean) As String

        Dim xml = Utility.getIntestazione(Username, Password, Utility.SoggSiRPV)

        Dim soggsirpvInput As SoggSiRPVInput = New SoggSiRPVInput()

        'CodOper
        soggsirpvInput.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        'TipoRichiesta
        soggsirpvInput.TipoRichiesta = tipoRichiesta
        'Soggetto[]
        soggsirpvInput.Soggetto = getSoggetti(codiciSoggetti, codOperCF)

        Dim req = XMLUtility.getBodyRequest(soggsirpvInput, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)

        Utility.cleanXmlSend(xml)

        Return xml.ToString

    End Function

    Public Function sGetSoggSiRPV(ByVal Username As String, ByVal Password As String, ByVal idTrasmissione As String) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetSoggSiRPV)

        Dim getSogg As GetSoggSiRPVInput = New GetSoggSiRPVInput()

        getSogg.IdTrasmissione = idTrasmissione
        'Dim tagTrasmissione = "<wsm:IdTrasmissione>" & idTrasmissione & "</wsm:IdTrasmissione>"

        Dim req = XMLUtility.getBodyRequest(getSogg, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)

        Utility.cleanXmlSend(xml)

        Return xml.ToString

    End Function

    Public Function sCancSoggSiRPV(ByVal Username As String, ByVal Password As String, ByVal codiciSoggetti As List(Of String), _
                                          ByVal codOperCF As String, _
                                           ByVal codOperPersonaFisica As Boolean) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.CancSoggSiRPV)
        Try
            Dim soggCancSiRPV As CancSoggSiRPVInput = New CancSoggSiRPVInput()

            soggCancSiRPV.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)

            Dim soggettiEl As List(Of SoggettoElimina) = New List(Of SoggettoElimina)
            For Each codiceSoggetto As String In codiciSoggetti
                Try
                    Dim soggEl As SoggettoElimina = New SoggettoElimina
                    soggEl.CodiceSoggetto = codiceSoggetto
                    soggettiEl.Add(soggEl)
                    soggWriter.aggiornaStato(ObjParametri_Server, codiceSoggetto, codOperCF, Utility.StatoGIAS.valida_per_invio)
                Catch ex As Exception
                    soggWriter.aggiornaStato(ObjParametri_Server, codiceSoggetto, codOperCF, Utility.StatoGIAS.non_valida_per_invio)
                End Try
            Next
            soggCancSiRPV.SoggettoElimina = soggettiEl.ToArray

            Dim req = XMLUtility.getBodyRequest(soggCancSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

            XMLUtility.addBody(xml, req, Utility.getSoapenv)
            Utility.cleanXmlSend(xml)
        Catch ex As Exception
            Throw New TeleregistriExceptionFormattazioneDati("xSoggSiRPV", "sCancSoggSiRPV")
        End Try
        Return xml.ToString
    End Function

    Public Function sGetCancSoggSiRPV(ByVal Username As String, ByVal Password As String, _
                                             ByVal idTrasmissione As String) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetCancSoggSiRPV)

        Dim getCancSogg As GetCancSoggSiRPVInput = New GetCancSoggSiRPVInput()

        getCancSogg.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getCancSogg, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

#Region "Supporto alle Operazioni"

    Public Function getSoggetti(ByVal codiciSoggetti As List(Of String), codOperCF As String) As Soggetto()
        Dim s As List(Of Soggetto) = New List(Of Soggetto)
        For Each codiceSoggetto As String In codiciSoggetti
            Try
                s.Add(getSoggetto(codiceSoggetto, codOperCF))
            Catch ex As Exception
            End Try
        Next
        Return s.ToArray
    End Function

    Public Function getSoggetto(ByVal codiceSoggetto As String, codOperCF As String) As Soggetto
        Try
            Dim sogg As Soggetto = New Soggetto
            sogg.CodiceSoggetto = codiceSoggetto
            Dim soggTable = soggReader.LeggiSoggetti("", Nothing, ObjParametri_Server, codOperCF, codiceSoggetto, Nothing, Nothing)
            Dim row = soggTable.Rows(0)

            Dim tipoSoggStr As String = row.Item("TipoSoggetto")
            Select Case tipoSoggStr
                Case "IT"
                    sogg.TipoSoggetto = SoggettoTipoSoggetto.IT
                Case "UE"
                    sogg.TipoSoggetto = SoggettoTipoSoggetto.UE
                Case "EX"
                    sogg.TipoSoggetto = SoggettoTipoSoggetto.EX
            End Select
            If sogg.TipoSoggetto = SoggettoTipoSoggetto.IT Then
                Dim cuaa As New CUAA
                Dim cuaaFisico As Boolean = row.Item(5)
                If cuaaFisico Then
                    cuaa.ItemElementName = ItemChoiceType.PersonaFisica
                Else
                    cuaa.ItemElementName = ItemChoiceType.PersonaGiuridica
                End If
                cuaa.Item = row.Item(4)
                sogg.CodiceCUAA = cuaa
                If sogg.CodiceCUAA.ItemElementName = ItemChoiceType.PersonaFisica Then
                    sogg.Nome = row.Item(7)
                    sogg.Cognome = row.Item(8)
                Else
                    sogg.RagioneSociale = row.Item(9)
                End If
            Else
                If row.Item(7) = "" And row.Item(8) = "" Then
                    sogg.RagioneSociale = row.Item(9)
                Else
                    sogg.Nome = row.Item(7)
                    sogg.Cognome = row.Item(8)
                End If
            End If

            Dim ind As New Indirizzo
            Dim stato As String = row.Item("Indirizzo_Stato")
            ind.Indirizzo1 = row.Item("Indirizzo_Indirizzo")
            ind.Stato = stato
            If stato.Equals("380") Then
                ind.CAP = row.Item("Indirizzo_CAP")
                ind.Comune = row.Item("Indirizzo_Comune")
                ind.Provincia = row.Item("Indirizzo_Provincia")
            End If

            sogg.IndirizzoSede = ind
            'AGGIORNA STATO VALIDO PER L'INVIO
            soggWriter.aggiornaStato(ObjParametri_Server, codiceSoggetto, codOperCF, Utility.StatoGIAS.valida_per_invio)

            Return sogg
        Catch ex As Exception
            'AGGIORNA STATO NON VALIDO PER L'INVIO
            soggWriter.aggiornaStato(ObjParametri_Server, codiceSoggetto, codOperCF, Utility.StatoGIAS.non_valida_per_invio)
            Throw New TeleregistriExceptionFormattazioneDati("xSoggSiRPV", "getSoggetto")
        End Try
    End Function

#End Region

End Class
