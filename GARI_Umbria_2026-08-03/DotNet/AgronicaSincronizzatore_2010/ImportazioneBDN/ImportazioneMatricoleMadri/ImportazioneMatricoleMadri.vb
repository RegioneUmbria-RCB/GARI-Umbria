Imports System.Web.UI.WebControls
Imports System.Xml
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreMapper
Imports AgronicaCoreAnagrafeDAL

Public Class ImportazioneMatricoleMadri

    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti
    End Sub

    Public Sub Avvia_Importazione_AnagrafeExcel(ByVal PivaSelezionata As String,
                                                ByVal listaFile As List(Of String),
                                            ByVal Directory_Log As String,
                                            anno As Integer,
                                            ByVal wkt_georiferimento_cod As String,
                                            ByRef lbl_conclusione As Label,
                                            objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ASG_Utente_Username As String,
                                            ASG_Utente_Password As String,
                                            ASG_ProgressivoGIAS As String,
                                            CodiceChiaveCliente As String,
                                            LinkWSImportaGIAS As String,
                                            flag_CentroUnico As Boolean)

        'Creazione file di log
        Dim NomeImport As String = ""
        Dim NomeFile As String = ""

        'Verifico l'esistenza della Directory passata dalle stampe (indicata nel web config)
        'Altrimenti metto di default la directory C:\GIASLAN
        If Directory_Log = "" Then
            Directory_Log = "C:\GIASLAN"

        End If

        Directory_Log = Directory_Log & "\Importazione_" + NomeImport + "_Log"

        'Verifico esistenza Cartella Importazione_Anagrafiche_Log
        If System.IO.Directory.Exists(Directory_Log) = False Then
            System.IO.Directory.CreateDirectory(Directory_Log)

        End If

        Dim Path_File_Log As String
        Dim SW As System.IO.StreamWriter

        NomeFile = "Importazione_Anagrafiche_MatricoleMadri" + NomeImport + "__" & Now.ToString("yyyy-MM-dd__(hh.mm.ss)") & ".txt"
        Path_File_Log = Directory_Log & "\Importazione_Anagrafiche_MatricoleMadri" + NomeImport + "__" & Now.ToString("yyyy-MM-dd__(hh.mm.ss)") & ".txt"
        SW = System.IO.File.CreateText(Path_File_Log)
        SW.Close()

        lbl_conclusione.Visible = True

        Dim risultato As String = ""
        Dim risultatoFinale As String = ""

        For Each Path_FileSoci_XLS In listaFile
            Dim provider As String
            Dim ObjImporta As New Importazione_MatricoleMadri(objParametriServer, objParametriUtenti)
            risultato = ""
            Dim Messaggio As String = ""
            Dim DatiImportati As Boolean = False
            Dim LogCodificheMancantiVarieta As String = "Codifica_Varieta_Mancanti_AGEA"
            Dim LogCodificheMancantiSpecie As String = "Codifica_Specie_Vegetali_Mancanti_AGEA"
            Dim extension As String = Path_FileSoci_XLS.Split(".")(1)

            If Environment.Is64BitProcess Then
                provider = "PROVIDER=Microsoft.ACE.OLEDB.12.0"

            Else
                provider = "PROVIDER=Microsoft.Jet.OLEDB.4.0"

            End If

            Dim StringaConnessione As String = provider

            Select Case extension
                Case "xls"
                    StringaConnessione &= ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';data source="
                    StringaConnessione &= "'" & Path_FileSoci_XLS & "'"

                Case "xlsx"
                    StringaConnessione &= ";Extended Properties='Excel 12.0;HDR=YES;';data source="
                    StringaConnessione &= "'" & Path_FileSoci_XLS & "'"

                Case Else
                    StringaConnessione &= ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';data source="
                    StringaConnessione &= "'" & Path_FileSoci_XLS & "'"

            End Select

            Dim cuaa = System.IO.Path.GetFileNameWithoutExtension(Path_FileSoci_XLS).Split("_")(2)
            Messaggio = "Importo file: " + cuaa
            'Dim spe_codice As String = allevamento("SPE_CODICE")
            'Dim id_fiscale_detentore As String = allevamento("ID_FISCALE_DETEN")
            Try
                DatiImportati = ObjImporta.importaCapi(PivaSelezionata,
                                                       StringaConnessione,
                                                       ASG_Utente_Username,
                                                       ASG_Utente_Password,
                                                       ASG_ProgressivoGIAS,
                                                       CodiceChiaveCliente,
                                                       Directory_Log,
                                                       NomeFile,
                                                       Messaggio)

                If Messaggio <> "" Then
                    risultato = "<h5 class=""red"">File " + cuaa + "</h5>" & vbCrLf & "Importa_Dati: " & Messaggio & vbCrLf & "<br/><br/>"
                End If

            Catch ex As Exception
                risultato = "<h5 class=""red"">CUAA " + cuaa + "</h5>:" & vbCrLf & "Errore Importa_Dati: " & ex.Message & vbCrLf & "<br/><br/>"

            End Try

            risultatoFinale = risultatoFinale & risultato

            'Cancella il file excel
            Try
                'TODO: Rimuovere il commento
                System.IO.File.Delete(Path_FileSoci_XLS)

            Catch ex As Exception

            End Try

        Next

        lbl_conclusione.Visible = True
        lbl_conclusione.Text = "<h4>Import ParmaFrance: </h4>" + vbCrLf + risultatoFinale

    End Sub

End Class
