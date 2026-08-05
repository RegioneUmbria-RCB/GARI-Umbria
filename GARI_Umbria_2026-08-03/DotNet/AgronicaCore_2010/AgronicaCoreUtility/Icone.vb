Imports System.Web


Public Class Icone

    '############################################################################
    Public Shared Sub Recupera_Elenco_Icone_SpecieVegetali(ByRef objServer As System.Web.HttpServerUtility, _
                                                           ByVal Cartella As String, _
                                                        ByRef Elenco_Icone_SpecieVegetali As String _
                                                    )

        Dim Percorso As String
        Dim NomeFile As String
        Dim Str_Veg_Cod As String

        'Inizializzo
        Elenco_Icone_SpecieVegetali = ""

        'Recupero il percorso reale della directory
        Percorso = objServer.MapPath(Cartella).ToString

        'Recupero 
        NomeFile = FileSystem.Dir(Percorso & "\*.ico")

        Do While NomeFile <> ""

            'Elimino l'estensione
            Str_Veg_Cod = Microsoft.VisualBasic.Strings.Left(NomeFile, 7)

            'Tolgo gli zeri di troppo
            Str_Veg_Cod = CInt(Str_Veg_Cod).ToString

            'Aggiungo il nuovo Veg_Cod all'elenco
            Elenco_Icone_SpecieVegetali += ":" & Str_Veg_Cod

            'Avanti un altro ...
            NomeFile = FileSystem.Dir()

        Loop

        'Tolgo il separatore iniziale
        Elenco_Icone_SpecieVegetali = Mid(Elenco_Icone_SpecieVegetali, 2)

    End Sub

    '############################################################################
    Public Shared Sub Recupera_Elenco_Icone_SpecieVegetali(ByRef objServer As System.Web.HttpServerUtility, _
                                                    ByRef Elenco_Icone_SpecieVegetali As String)
        Recupera_Elenco_Icone_SpecieVegetali(objServer, "../AB_Immagini/IconeVegetali", Elenco_Icone_SpecieVegetali)
    End Sub

End Class
