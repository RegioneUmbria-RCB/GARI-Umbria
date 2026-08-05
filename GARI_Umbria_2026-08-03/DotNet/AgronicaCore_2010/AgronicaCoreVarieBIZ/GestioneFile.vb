Imports System.IO
Imports CrystalDecisions.Shared

Public Class GestioneFile
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Function SalvaReportPdf(ByVal rpt As CrystalDecisions.CrystalReports.Engine.ReportClass,
                                    ByVal CatCod As AgronicaCoreDataProvider.TipiEnumerativi.enum_CategorieDocumenti,
                                    ByVal Sottocartella As String,
                                    ByVal NomeFile As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objAgroWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                    Optional ByVal Bypass_PATH As Boolean = False) As String

        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions

        exportOpts = rpt.ExportOptions

        ' Imposta il formato di esportazione.
        exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

        ' Imposta le opzioni relative al file del disco.
        Dim strPath As String
        Dim PathCartella As String

        ' leggo la sottocartella da CategorieDocumenti
        If Sottocartella = String.Empty Then
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(CatCod, "", "", objParametri)
            objCatDoc = Nothing
        End If

        Dim PathAllegati As String


        If Not Bypass_PATH Then

            'Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri, True)
            If objAgroWeb.GestioneAllegati_Repository = String.Empty Then
                PathAllegati = "C:\GIASLAN\AgronicaStampe_Allegati\" 'default
            Else
                PathAllegati = objAgroWeb.GestioneAllegati_Repository
            End If

            If Not PathAllegati.EndsWith("\") Then
                PathAllegati &= "\"
            End If

        Else

            PathAllegati = "" 'il path è già nel percorso

        End If

        PathCartella = PathAllegati & Sottocartella

        ' se il path non esiste, creo tutte le cartelle e sottocartelle
        If Not System.IO.Directory.Exists(PathCartella) Then
            System.IO.Directory.CreateDirectory(PathCartella)
        End If

        If Not PathCartella.EndsWith("\") Then
            PathCartella &= "\"
        End If

        'per evitare di creare sottocartelle a causa della barra nel nome del file
        NomeFile = Replace(NomeFile, "\", "-")
        NomeFile = Replace(NomeFile, "/", "-")

        strPath = PathCartella & NomeFile

        'lunghezza percorsi e nome file in windows:
        'unità:\ + 256 caratteri (tra path e nome file)

        If strPath.Length > 259 Then
            Throw New Exception("Superata la lunghezza tra path e nome del file: " & CStr(strPath.Length) & "caratteri (max 259 caratteri).")
        End If

        diskOpts.DiskFileName = strPath
        exportOpts.DestinationOptions = diskOpts

        ' Esportazione del report.
        rpt.Export()

        'Giulia 20/10/2021: MAI fare il dispose qua dentro perché il chiamante avrà ancora bisogno dell'rpt! 
        'rpt.Close()
        'rpt.Dispose()


        Return strPath

    End Function

    '################################################################################
    Public Function SalvaReportDocumentPdf(ByVal rpt As CrystalDecisions.CrystalReports.Engine.ReportDocument,
                                           ByVal CatCod As AgronicaCoreDataProvider.TipiEnumerativi.enum_CategorieDocumenti,
                                           ByVal Sottocartella As String,
                                           ByVal NomeFile As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objAgroWeb As AgronicaCoreGestioneRichieste.AgroWebConfig
                                           ) As String

        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions

        exportOpts = rpt.ExportOptions

        ' Imposta il formato di esportazione.
        exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

        ' Imposta le opzioni relative al file del disco.
        Dim strPath As String
        Dim PathCartella As String

        ' leggo la sottocartella da CategorieDocumenti
        If Sottocartella = String.Empty Then
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(CatCod, "", "", objParametri)
            objCatDoc = Nothing
        End If

        Dim PathAllegati As String

        'Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri, True)
        If objAgroWeb.GestioneAllegati_Repository = String.Empty Then
            PathAllegati = "C:\GIASLAN\AgronicaStampe_Allegati\" 'default
        Else
            PathAllegati = objAgroWeb.GestioneAllegati_Repository
        End If

        If Not PathAllegati.EndsWith("\") Then
            PathAllegati &= "\"
        End If

        PathCartella = PathAllegati & Sottocartella

        ' se il path non esiste, creo tutte le cartelle e sottocartelle
        If Not System.IO.Directory.Exists(PathCartella) Then
            System.IO.Directory.CreateDirectory(PathCartella)
        End If

        If Not PathCartella.EndsWith("\") Then
            PathCartella &= "\"
        End If

        'per evitare di creare sottocartelle a causa della barra nel nome del file
        NomeFile = Replace(NomeFile, "\", "-")
        NomeFile = Replace(NomeFile, "/", "-")

        strPath = PathCartella & NomeFile

        'lunghezza percorsi e nome file in windows:
        'unità:\ + 256 caratteri (tra path e nome file)

        If strPath.Length > 259 Then
            Throw New Exception("Superata la lunghezza tra path e nome del file: " & CStr(strPath.Length) & "caratteri (max 259 caratteri).")
        End If

        diskOpts.DiskFileName = strPath
        exportOpts.DestinationOptions = diskOpts

        ' Esportazione del report.
        rpt.Export()

        Return strPath

    End Function
End Class
