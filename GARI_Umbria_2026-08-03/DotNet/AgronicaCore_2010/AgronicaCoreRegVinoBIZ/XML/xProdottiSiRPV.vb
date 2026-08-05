Imports AgronicaCoreDataProvider
Imports AgronicaCoreRegVinoDAL
Imports System.Xml.Serialization
Imports System.IO
Imports System.Text
Imports System.Xml
Imports AgronicaCoreUtility

Public Class xProdottiSiRPV
    Dim prodReader As xDBProdottiSiRPV_R
    Dim prodWriter As xDBProdottiSiRPV_W

    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Sub New(server As AgronicaCoreDataProvider.AgronicaCoreParametri, utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        ObjParametri_Server = server
        ObjParametri_Utenti = utenti
        prodReader = New AgronicaCoreRegVinoDAL.xDBProdottiSiRPV_R()
        prodWriter = New AgronicaCoreRegVinoDAL.xDBProdottiSiRPV_W()
    End Sub

    Public Function sProdottiSiRPV(ByVal Username As String, ByVal Password As String, _
                                      ByVal codiciProdotti As List(Of String), _
                                      ByVal tipoRichiesta As Integer, _
                                      ByVal codIcqrf As String,
                                      ByVal codOperCF As String, _
                                      ByVal codOperPersonaFisica As Boolean) As String

        Dim xml = Utility.getIntestazione(Username, Password, Utility.ProdSiRPV)

        Dim prodSiRPVInput As ProdSiRPVInput = New ProdSiRPVInput()

        'CodOper
        prodSiRPVInput.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        'TipoRichiesta
        prodSiRPVInput.TipoRichiesta = tipoRichiesta
        prodSiRPVInput.CodiceIcqrf = codIcqrf
        'Prodotto[]
        prodSiRPVInput.ProdCatalogo = getProdotti(codiciProdotti, codOperCF, codIcqrf)

        Dim req = XMLUtility.getBodyRequest(prodSiRPVInput, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)

        Utility.cleanXmlSend(xml)

        Return xml.ToString

    End Function

    Public Function sGetProdottiSiRPV(ByVal Username As String, ByVal Password As String, ByVal idTrasmissione As String) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetProdSiRPV)

        Dim getprodSiRPV As New GetProdSiRPVInput

        getprodSiRPV.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getprodSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sCancProdottiSiRPV(ByVal Username As String, ByVal Password As String, _
                                          ByVal codIcqrf As String, _
                                          ByVal codOperCF As String, _
                                          ByVal codOperPersonaFisica As Boolean, _
                                          ByVal codiciProdotti As List(Of String)) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.CancProdSiRPV)

        Dim cancProdSiRPV As New CancProdSiRPVInput

        cancProdSiRPV.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        cancProdSiRPV.CodiceIcqrf = codIcqrf
        cancProdSiRPV.ProdCatalogoElimina = getProdCatalogoElimina(codiciProdotti, codIcqrf, codOperCF)
        Dim req = XMLUtility.getBodyRequest(cancProdSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sGetCancProdSiRPV(ByVal Username As String, ByVal Password As String, ByVal idTrasmissione As String) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetCancProdSiRPV)

        Dim getcancprodSiRPV As New GetCancProdSiRPVInput

        getcancprodSiRPV.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getcancprodSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

#Region "Supporto alle Operazioni"

    Public Function getProdotti(ByVal codiciProdotti As List(Of String), codOperCF As String, codIcqrf As String) As ProdSiRPVInputProdCatalogo()
        Dim s As List(Of ProdSiRPVInputProdCatalogo) = New List(Of ProdSiRPVInputProdCatalogo)
        For Each codiceProdotto As String In codiciProdotti
            Try
                s.Add(getProdotto(codiceProdotto.Split("|")(0), codiceProdotto.Split("|")(1), codOperCF, codIcqrf))
                prodWriter.aggiornaStato(ObjParametri_Server, codiceProdotto, codIcqrf, codOperCF, Utility.StatoGIAS.valida_per_invio)
            Catch ex As Exception
                prodWriter.aggiornaStato(ObjParametri_Server, codiceProdotto, codIcqrf, codOperCF, Utility.StatoGIAS.non_valida_per_invio)
            End Try
        Next
        Return s.ToArray
    End Function

    Public Function getProdotto(ByVal codPrimario As String, codSecondario As String, codOperCF As String, codIcqrf As String) As ProdSiRPVInputProdCatalogo
        Try
            Dim prod As ProdSiRPVInputProdCatalogo = New ProdSiRPVInputProdCatalogo
            Dim codProd As New CodiceProdotto
            codProd.CodPrimario = codPrimario
            codProd.CodSecondario = codSecondario
            prod.CodiceProdotto = codProd

            Dim prodTable As DataTable = prodReader.getProdotto(ObjParametri_Server, codOperCF, codIcqrf, codPrimario, codSecondario)
            Dim row = prodTable.Rows(0)
            Dim des As New ProdottoCatalogo
            des.CodCategoria = inserisciValoreLinear(row.Item("CodCategoria"))
            des.AttoCert = inserisciValoreLinear(row.Item("AttoCert"))
            des.CodClassificazione = inserisciValoreLinear(row.Item("CodClassificazione"))
            des.CodDopIgp = inserisciValoreLinear(row.Item("CodDopIgp"))
            des.CodEbacchus = inserisciValoreLinear(row.Item("CodEbacchus"))
            des.OrigineUve = inserisciValoreLinear(row.Item("OrigineUve"))
            Dim provenienza = inserisciValoreLinear(row.Item("Provenienza"))
            If provenienza = "IT" Then
                des.Provenienza = "01"
            Else
                des.Provenienza = inserisciValoreLinear(row.Item("Provenienza"))
            End If
            des.PaesiProvenienza = inserisciPaesiProvenienza(row.Item("PaesiProvenienza"))
            des.CodZonaViticola = inserisciValoreLinear(row.Item("CodZonaViticola"))
            des.Varieta = inserisciCod_ValorePerc(row.Item("Varieta"))
            des.AltreVarieta = inserisciValoreLinear(row.Item("AltreVarieta"))
            des.CodSottozona = inserisciValoreLinear(row.Item("CodSottozona"))
            des.CodVigna = inserisciValoreLinear(row.Item("CodVigna"))
            des.CodColore = inserisciValoreLinear(row.Item("CodColore"))
            des.Menzioni = inserisciMenzioni(row.Item("Menzioni"))
            Dim biologico As String = inserisciValoreLinear(row.Item("Biologico"))
            If biologico <> "0" Then
                des.Biologico = inserisciValoreLinear(row.Item("Biologico"))
            End If
            des.PraticheEnologiche = inserisciPraticheEnologiche(row.Item("PraticheEnologiche"))
            des.CodPartita = inserisciValoreLinear(row.Item("CodPartita"))
            des.Annata = inserisciTipoAnnata(row.Item("Annata"))
            des.MassaVolumica = CDbl(inserisciValoreLinear(row.Item("MassaVolumica")))
            des.CodStatoFisico = inserisciValoreLinear(row.Item("CodStatoFisico"))
            Dim datacertDop As Date? = inserisciValoreDate(row.Item("DataCertDOP"))
            If datacertDop IsNot Nothing Then
                des.DataCertDOP = datacertDop
            End If
            des.NumCertDOP = inserisciValoreLinear(row.Item("NumCertDOP"))
            prod.Designazione = des
            Return prod
        Catch ex As Exception
            'AGGIORNA STATO NON VALIDO PER L'INVIO
            prodWriter.aggiornaStato(ObjParametri_Server, codPrimario + "|" + codSecondario, codOperCF, codIcqrf, Utility.StatoGIAS.non_valida_per_invio)
            Throw New TeleregistriExceptionFormattazioneDati("xProdSiRPV", "getProdotto")
        End Try
    End Function

    Public Function inserisciValoreDate(ByRef val As Object) As Date?
        If Not IsDBNull(val) Then
            If val IsNot Nothing Then
                If CStr(val) <> "" Then
                    Return CDate(val)
                Else
                    Return Nothing
                End If
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciValoreLinear(ByRef val As Object) As String
        If Not IsDBNull(val) Then
            If val IsNot Nothing Then
                If CStr(val) <> "" Then
                    Return val
                Else
                    Return Nothing
                End If
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciPaesiProvenienza(ByRef val As Object) As PaesiProvenienza()
        If Not IsDBNull(val) Then
            If val IsNot Nothing Then
                Dim paesi As New List(Of PaesiProvenienza)
                Dim paesiVal As String = CStr(val)
                For Each strPaese In paesiVal.Split("|")
                    If strPaese IsNot Nothing AndAlso strPaese <> "" Then
                        Dim paese As New PaesiProvenienza
                        paese.Codice = strPaese
                        paesi.Add(paese)
                    End If
                Next
                Return paesi.ToArray
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciCod_ValorePerc(ByRef val As Object) As Cod_ValorePerc()
        If Not IsDBNull(val) Then
            Dim valori As New List(Of Cod_ValorePerc)
            Dim valoriStr = val.Split("|")
            For Each strValore In valoriStr
                If strValore IsNot Nothing AndAlso strValore <> "" Then
                    Dim strV = strValore.Split("*")
                    If strV.Length = 2 Then
                        Dim cod = strV(0)
                        Dim per = strV(1)
                        Dim valore As New Cod_ValorePerc
                        valore.Codice = cod
                        valore.Percentuale = per
                        valori.Add(valore)
                    ElseIf strV.Length = 1 Then
                        Dim cod = strV(0)
                        Dim per As Decimal = 100
                        Dim valore As New Cod_ValorePerc
                        valore.Codice = cod
                        valore.Percentuale = per
                        valori.Add(valore)
                    End If
                End If
            Next
            Return valori.ToArray
        End If
        Return Nothing
    End Function

    Public Function inserisciMenzioni(ByRef val As Object) As Menzioni()
        If Not IsDBNull(val) Then
            If val IsNot Nothing Then
                Dim menzioni As New List(Of Menzioni)
                Dim valStr = CStr(val)
                For Each strMenzione In valStr.Split("|")
                    If strMenzione IsNot Nothing AndAlso strMenzione <> "" Then
                        Dim menzione As New Menzioni
                        menzione.Codice = strMenzione
                        menzioni.Add(menzione)
                    End If
                Next
                Return menzioni.ToArray
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciPraticheEnologiche(ByRef val As Object) As PraticheEnologiche()
        If Not IsDBNull(val) Then
            If val IsNot Nothing Then
                Dim pratiche As New List(Of PraticheEnologiche)
                Dim valStr = CStr(val)
                For Each strPratica In valStr.Split("|")
                    If strPratica IsNot Nothing AndAlso strPratica <> "" Then
                        Dim pratica As New PraticheEnologiche
                        pratica.Codice = strPratica
                        pratiche.Add(pratica)
                    End If
                Next
                Return pratiche.ToArray
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciTipoAnnata(ByRef val As Object) As TipoAnnata
        If Not IsDBNull(val) Then
            If val IsNot Nothing AndAlso CStr(val) <> "" Then
                Dim annata As New TipoAnnata
                Dim strVal = val.Split("|")
                If strVal.Length = 2 Then
                    annata.Codice = strVal(0)
                    annata.Percentuale = strVal(1)
                ElseIf strVal.Length = 1 Then
                    annata.Codice = strVal(0)
                    annata.Percentuale = 100
                End If
            End If
            Return Nothing
        End If
        Return Nothing
    End Function

#End Region

    Private Function getProdCatalogoElimina(codiciProdotti As List(Of String), codIcqrf As String, codOper As String) As ProdElimina()
        Dim vasiElimina As New List(Of ProdElimina)
        For Each codiceProdotto As String In codiciProdotti
            vasiElimina.Add(getProdElimina(codiceProdotto, codIcqrf, codOper))
        Next
        Return vasiElimina.ToArray
    End Function

    Private Function getProdElimina(codiceProdotto As String, codiceIcqrf As String, codOper As String) As ProdElimina
        Dim prod As New ProdElimina
        Try
            Dim codProd As New CodiceProdotto
            codProd.CodPrimario = codiceProdotto.Split("|")(0)
            codProd.CodSecondario = codiceProdotto.Split("|")(1)
            prod.CodiceProdotto = codProd
            prodWriter.aggiornaStato(ObjParametri_Server, codiceProdotto, codiceIcqrf, codOper, Utility.StatoGIAS.valida_per_invio)
        Catch ex As Exception
            prodWriter.aggiornaStato(ObjParametri_Server, codiceProdotto, codiceIcqrf, codOper, Utility.StatoGIAS.non_valida_per_invio)
        End Try
        Return prod
    End Function

End Class
