Imports System.Text

Public Class SIGPA_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ElencaImpianti_DatixSipga( _
                                    ByVal strFiltro As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "ElencaImpianti_DatixSipga"

        Dim stb As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.Append(" Select distinct " & vbCrLf)
            stb.Append(" Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, Reg_Impianti.Sup_Imp, ")
            stb.Append(" AppezzamentiXParticelle.PROV, AppezzamentiXParticelle.COM, AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO, AppezzamentiXParticelle.SUBALTERNO, AppezzamentiXParticelle.AREA, ")
            stb.Append(" Reg_Impianti_Codici.val_cod AS Specie_Agea, Allegati_Documenti.Allegati_Documenti_Numero AS Fascicolo ")
            stb.Append(" FROM Reg_Impianti INNER JOIN ")
            stb.Append(" AppezzamentiXParticelle ON Reg_Impianti.PIVA = AppezzamentiXParticelle.PIVA AND Reg_Impianti.SA_COD = AppezzamentiXParticelle.SA_COD AND  ")
            stb.Append(" Reg_Impianti.APPEZZA = AppezzamentiXParticelle.APPEZZA INNER JOIN ")
            stb.Append(" Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND  ")
            stb.Append(" Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg INNER JOIN ")
            stb.Append(" Reg_Impianti_Programmazioni ON Reg_Impianti.SA_COD = Reg_Impianti_Programmazioni.Sa_Cod AND  ")
            stb.Append(" Reg_Impianti.PIVA = Reg_Impianti_Programmazioni.Piva AND Reg_Impianti.APPEZZA = Reg_Impianti_Programmazioni.Appezza AND  ")
            stb.Append(" Reg_Impianti.ID_REG = Reg_Impianti_Programmazioni.Id_Reg INNER JOIN ")
            stb.Append(" Allegati_EntitaxDocumenti ON Reg_Impianti_Programmazioni.Programmazione_Cod = Allegati_EntitaxDocumenti.Programmazione_Cod AND  ")
            stb.Append(" Reg_Impianti_Programmazioni.Programmazione_Entita_Cod = Allegati_EntitaxDocumenti.Programmazione_Entita_Cod INNER JOIN ")
            stb.Append(" Allegati_Documenti ON Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND  ")
            stb.Append(" Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            stb.Append("  " & vbCrLf)
            stb.Append(" WHERE id_cod=1133 ")
            stb.Append(" AND " & strFiltro)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function SIGPA_2015_05_AgendaDestinazioniErronee_CatVegSup( _
                                ByVal strFiltro As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        ) As DataTable

        Dim NomeRoutine As String = "SIGPA_2015_05_AgendaDestinazioniErronee_CatVegSup"

        Dim stb As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.Append(" Select  * " & vbCrLf)
            stb.Append(" FROM SIGPA_2015_05_AgendaDestinazioniErronee_CatVegSup ")
            stb.Append(" WHERE 1=1 ")
            stb.Append(" AND des_lib not like '%vite%'")

            If strFiltro <> "" Then
                stb.Append(" AND " & strFiltro)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class
