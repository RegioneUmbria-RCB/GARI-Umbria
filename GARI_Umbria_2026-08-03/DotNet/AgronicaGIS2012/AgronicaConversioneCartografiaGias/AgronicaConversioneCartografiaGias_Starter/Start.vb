Imports <xmlns="http://www.agronica.it/grafica/">


Imports AgronicaConversioneCartografiaGIAS.Biz
Imports System.Collections.ObjectModel

Imports Gias2Gias_LIB
Imports GIAS2GIAS_LOCALE

Imports System.Configuration.ConfigurationManager

Imports System.Text
Imports AgronicaGIS2012.Commons
Imports AgronicaCoreModello

Module Start

    Private head As String = _
            "<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">"

    Private tail As String = _
        "</DatiEntita>"

    Public Const AGRODATAINIZIO As Date = #1/1/1900#
    Public Const AGRODATAFINE As Date = #12/31/2100#

    Private _LayersImprese As New List(Of String)

    Private _g2g_set As clsOpzioni
    Sub main()
        Dim convertHelper As New ConvertiVecchioNuovo


        _g2g_set = New clsOpzioni()
        _g2g_set.Connessione_Server_GIAS_Origine = AppSettings("Connessione_SERVER_ORIGINE")
        _g2g_set.Connessione_Server_GIAS_Destinazione = AppSettings("Connessione_SERVER_DESTINAZIONE")

        _g2g_set.Connessione_Utenti_GIAS_Origine = AppSettings("Connessione_UTENTI_ORIGINE")
        _g2g_set.Connessione_Utenti_GIAS_Destinazione = AppSettings("Connessione_UTENTI_DESTINAZIONE")

        _g2g_set.ProgressivoGIAS_ORIGINE = AppSettings("ProgressivoGIAS_ORIGINE")
        _g2g_set.ProgressivoGIAS_DESTINAZIONE = AppSettings("ProgressivoGIAS_DESTINAZIONE")

        _g2g_set.PercorsoConnessioni = "C:\agroconnessioni\connessioni.ini"

        'super user
        _g2g_set.SuperUser_CodFiscale_ORIGINE = AppSettings("PivaSuperUser_ORIGINE")
        _g2g_set.SuperUser_CodFiscale_DESTINAZIONE = AppSettings("PivaSuperUser_DESTINAZIONE")
        _g2g_set.SuperUser_Username_ORIGINE = AppSettings("UsernameSuperUser_Origine")
        _g2g_set.SuperUser_Username_DESTINAZIONE = AppSettings("UsernameSuperUser_Destinazione")

        'importatore
        _g2g_set.Import_CodFiscale_ORIGINE = AppSettings("Import_CodFiscale_ORIGINE")
        _g2g_set.Import_CodFiscale_DESTINAZIONE = AppSettings("Import_CodFiscale_DESTINAZIONE")
        _g2g_set.Import_Username_ORIGINE = AppSettings("Import_Username_ORIGINE")
        _g2g_set.Import_Username_DESTINAZIONE = AppSettings("Import_Username_DESTINAZIONE")


        Dim log_G2G As New StringBuilder
        Dim log_Errori As New StringBuilder
        Dim log_Riepilogo As New StringBuilder

        Dim trans As New GIAS2GIAS_LOCALE.GIAS_2_GIAS

        Dim imprese = XDocument.Load(".\daImportare.xml")

        Dim listOfDataImport As List(Of clsImpresa) = GetImportImprese.getImprese(imprese)

        Dim letturaoldGraficaHelper As New Gias2Gias_LIB.Funzioni(Nothing)

        Dim NonImportatiSQL As String = AppSettings("DumpNonImportati")
        My.Computer.FileSystem.WriteAllText(NonImportatiSQL, "", False)

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim i As Integer = 1
        Dim percent As Single = 0
        Dim totale As Single = listOfDataImport.Count

        For Each impresa In listOfDataImport

            RenderProgressBar(totale, i, 1, 1)

            '  Vanni, 28/05/2016 11:43:39: todo: decommentare
            'trans.GIAS_2_GIAS_Completaopzioni(_g2g_set)
            'Dim fileContents As String = _
            '    letturaoldGraficaHelper.GetDatiXML_Grafica( _
            '        _g2g_set, _
            '        impresa.Piva_ORIGINE, _
            '        impresa.Opzionale_Sa_Cod_Origine _
            '    )

            Dim output As String

            Dim pivaimpresa As String = ""
            If Not impresa.Piva_ORIGINE.Contains("-") Then
                pivaimpresa = impresa.Piva_ORIGINE
            Else
                If impresa.Piva_ORIGINE.Split("-")(0) = impresa.Piva_ORIGINE.Split("-")(1) Then
                    pivaimpresa = impresa.Piva_ORIGINE.Split("-")(0)
                End If
            End If

            If pivaimpresa <> "" And Not _LayersImprese.Contains(pivaimpresa) Then
                _LayersImprese.Add(pivaimpresa & "|" & impresa.RagioneSociale)
            End If

            '  Vanni, 28/05/2016 11:43:39: todo: decommentare, usare file contest
            If "" <> "" Then

                Dim layersDaImportare As String() = AppSettings("layersDaImportare").Split(",")

                For Each Layer As String In layersDaImportare

                    '  Vanni, 28/05/2016 11:43:39: todo: decommentare, usare file contest
                    'output = convertHelper.ConvertiXml("", _g2g_set.SuperUser_CodFiscale_ORIGINE, impresa.PivaPadre_DESTINAZIONE, impresa.Piva_ORIGINE, impresa.Opzionale_Sa_Cod_Origine, _g2g_set, Layer, True, False, 1)


                    Dim gml As XNamespace = "http://www.opengis.net/gml"



                    Dim tmpDoc As XDocument = XDocument.Parse(output)
                    For Each elemento In ( _
                        From a In tmpDoc.<DatiEntita>.<Entita> _
                    Select a).ToList()



                        'per test formato database ...
                        Try

                            'transizione sulla singola chiamata di un entità grafica/GIAS

                            Dim idle As Integer
                            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)
                            ScriviElementiGrafici.scrivi(elemento.ToString, idle, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)

                            G2G_Chiusura_Transazione(1)

                        Catch ex As Exception

                            G2G_Chiusura_Transazione(2)

                            My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & "<Entita>" & elemento.Elements.FirstOrDefault.ToString & "</Entita>" & tail, True)

                        End Try


                    Next

                Next

            End If



            i += 1
        Next

        FinalizzaRisultato(NonImportatiSQL)

    End Sub

    Private Sub G2G_Chiusura_Transazione( _
                                ByVal Flag_Commit1_Rollback2 As Integer _
                                )

        Dim NomeRoutine As String = "G2G_Chiusura_Transazione"

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)

        Catch ex As Exception
        End Try

    End Sub

    Sub RenderProgressBar(ByVal intMaxValue As Integer, ByVal intProgress As Integer, ByVal intLeftPos As Integer, ByVal intTopPos As Integer)

        Dim strResult As String
        Dim intPercent As Integer

        Try

            If (intMaxValue > 0) Then
                intPercent = Math.Round((intProgress / intMaxValue) * 100, 0)
                If intPercent >= 10 Then
                    strResult = "| " & intPercent & "% |" & StrDup(intPercent \ 10, "=") & StrDup(10 - (intPercent \ 10), " ") & "|"
                Else
                    strResult = "| " & intPercent & "% |" & StrDup(10, " ") & "|"
                End If
            Else
                intPercent = intProgress
                strResult = "| " & FormatNumber(intPercent, 0) & " |"
            End If
            Console.CursorVisible = False
            Console.SetCursorPosition(intLeftPos, intTopPos)
            Console.Write(strResult)
            Console.CursorVisible = True
        Catch ex As Exception

        End Try

    End Sub




    Private Sub FinalizzaRisultato(ByVal fileDaSalvare As String)
        Dim finalData As String
        finalData = My.Computer.FileSystem.ReadAllText(fileDaSalvare)


        finalData = finalData.Replace(head, "")
        finalData = finalData.Replace(tail, "")

        finalData = head & finalData & tail

        Dim xmlFinaleElaborato As XDocument = XDocument.Parse(finalData)
        Dim layers = <layersdescrizioni>
                         <layer tipologia_layer="1" nome_layer="entità">
                             <valori codice="1">Azienda</valori>
                             <valori codice="2">Centro Aziendale</valori>
                             <valori codice="3">Appezzamento</valori>
                             <valori codice="13">Impianto</valori>
                         </layer>
                         <layer tipologia_layer="10" nome_layer="Conversione coordinate">
                             <valori codice="1">GPS Originali</valori>
                             <valori codice="2">Convertite</valori>
                         </layer>
                         <layer tipologia_layer="5" nome_layer="Specie Vegetali">
                             <valori codice="-1">Non specificata</valori>
                             <valori codice="5000021">Barbabietola da Foraggio</valori>
                             <valori codice="6">Barbabietola da zucchero</valori>
                             <valori codice="69">Bietola da coste</valori>
                             <valori codice="70">Bietola rossa (da Orto)</valori>
                             <valori codice="9">Carota</valori>
                             <valori codice="77">Cavolo broccolo</valori>
                             <valori codice="12">Cavolo cappuccio bianco</valori>
                             <valori codice="80">Cavolo cappuccio rosso</valori>
                             <valori codice="108">Cavolo cinese (brassica campestris)</valori>
                             <valori codice="227">Cavolo da foraggio</valori>
                             <valori codice="81">Cavolo di bruxelles</valori>
                             <valori codice="82">Cavolo laciniato (=nero)</valori>
                             <valori codice="83">Cavolo rapa</valori>
                             <valori codice="78">Cavolo verza</valori>
                             <valori codice="13">Cetriolo</valori>
                             <valori codice="14">Cicoria</valori>
                             <valori codice="16">Cipolla</valori>
                             <valori codice="35">Lattuga</valori>
                             <valori codice="51">Pisello</valori>
                             <valori codice="87">Rapa primaverile e autunnale</valori>
                             <valori codice="72">Ravanello</valori>
                             <valori codice="104">Senape</valori>
                             <valori codice="66">Zucchino</valori>
                         </layer>
                     </layersdescrizioni>

        Dim l = <layer tipologia_layer="100" nome_layer="Aziende Sementiere"></layer>
        For Each az In _LayersImprese
            Dim valore = <valori codice=<%= az.Split("|")(0) %>><%= az.Split("|")(1) %></valori>
            l.Add(valore)

        Next

        layers.Add(l)

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")
        xmlFinaleElaborato.Root.Add(layers)

        finalData = xmlHelper.RemoveNamespace(xmlFinaleElaborato, ListaNS).ToString.Replace("xmlns=""""", "")

        My.Computer.FileSystem.WriteAllText(fileDaSalvare, finalData, False)
    End Sub



End Module
