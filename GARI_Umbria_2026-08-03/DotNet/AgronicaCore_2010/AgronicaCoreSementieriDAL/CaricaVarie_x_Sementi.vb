Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class CaricaVarie_x_Sementi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
            ByVal Lista_Regione_Cod As String,
            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""
        Dim Codice As String

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try
            stb.Append(" SELECT  DISTINCT ")
            stb.Append("          REG AS Regione_Cod, ")
            stb.Append("          'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Regione_Des,  ")
            stb.Append("          'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Regione_Sigla  ")
            stb.Append("  FROM    Lista_Province ")

            If Lista_Regione_Cod <> "" Then
                stb.Append(" WHERE   (REG IN (" & Agro_SQL_Save_Clausola_IN(Lista_Regione_Cod, True) & ")) ")
            End If

            stb.Append(" ORDER BY REG ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



            'Verifico la presenza di errori
            If dt.Rows.Count = 0 Then
                Throw New Exception("Errore [Lettura REGIONI] : " & MessaggioErrore)
                Return Nothing
            Else

                dt.Columns("Regione_Des").ReadOnly = False
                dt.Columns("Regione_Sigla").ReadOnly = False

                For i = 0 To dt.Rows.Count - 1
                    Codice = dt.Rows(i).Item("Regione_Cod")
                    Select Case Codice

                        Case "001"
                            dt.Rows(i).Item("Regione_Des") = "Piemonte"
                            dt.Rows(i).Item("Regione_Sigla") = "PIE"
                        Case "002"
                            dt.Rows(i).Item("Regione_Des") = "Valle d`Aosta"
                            dt.Rows(i).Item("Regione_Sigla") = "VdA"
                        Case "003"
                            dt.Rows(i).Item("Regione_Des") = "Lombardia"
                            dt.Rows(i).Item("Regione_Sigla") = "LOM"
                        Case "004"
                            dt.Rows(i).Item("Regione_Des") = "Trentino Alto Adige"
                            dt.Rows(i).Item("Regione_Sigla") = "TAA"
                        Case "005"
                            dt.Rows(i).Item("Regione_Des") = "Veneto"
                            dt.Rows(i).Item("Regione_Sigla") = "VEN"
                        Case "006"
                            dt.Rows(i).Item("Regione_Des") = "Friuli Venezia Giulia"
                            dt.Rows(i).Item("Regione_Sigla") = "FVG"
                        Case "007"
                            dt.Rows(i).Item("Regione_Des") = "Liguria"
                            dt.Rows(i).Item("Regione_Sigla") = "LIG"
                        Case "008"
                            dt.Rows(i).Item("Regione_Des") = "Emilia Romagna"
                            dt.Rows(i).Item("Regione_Sigla") = "EMR"
                        Case "009"
                            dt.Rows(i).Item("Regione_Des") = "Toscana"
                            dt.Rows(i).Item("Regione_Sigla") = "TOS"
                        Case "010"
                            dt.Rows(i).Item("Regione_Des") = "Umbria"
                            dt.Rows(i).Item("Regione_Sigla") = "UMB"
                        Case "011"
                            dt.Rows(i).Item("Regione_Des") = "Marche"
                            dt.Rows(i).Item("Regione_Sigla") = "MAR"
                        Case "012"
                            dt.Rows(i).Item("Regione_Des") = "Lazio"
                            dt.Rows(i).Item("Regione_Sigla") = "LAZ"
                        Case "013"
                            dt.Rows(i).Item("Regione_Des") = "Abruzzo"
                            dt.Rows(i).Item("Regione_Sigla") = "ABR"
                        Case "014"
                            dt.Rows(i).Item("Regione_Des") = "Molise"
                            dt.Rows(i).Item("Regione_Sigla") = "MOL"
                        Case "015"
                            dt.Rows(i).Item("Regione_Des") = "Campania"
                            dt.Rows(i).Item("Regione_Sigla") = "CAM"
                        Case "016"
                            dt.Rows(i).Item("Regione_Des") = "Puglia"
                            dt.Rows(i).Item("Regione_Sigla") = "PUG"
                        Case "017"
                            dt.Rows(i).Item("Regione_Des") = "Basilicata"
                            dt.Rows(i).Item("Regione_Sigla") = "BAS"
                        Case "018"
                            dt.Rows(i).Item("Regione_Des") = "Calabria"
                            dt.Rows(i).Item("Regione_Sigla") = "CAL"
                        Case "019"
                            dt.Rows(i).Item("Regione_Des") = "Sicilia"
                            dt.Rows(i).Item("Regione_Sigla") = "SIC"
                        Case "020"
                            dt.Rows(i).Item("Regione_Des") = "Sardegna"
                            dt.Rows(i).Item("Regione_Sigla") = "SAR"
                    End Select
                Next

            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function Regioni_Leggi(
            ByVal Lista_Regione_Cod As String,
            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Regioni_Leggi()"
        Dim MessaggioErrore As String = ""
        Dim Codice As String

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try
            stb.Append(" SELECT REG AS Regione_Cod, Regione_Des FROM Lista_Regioni " & vbCrLf)

            If Lista_Regione_Cod <> "" Then
                stb.Append(" WHERE   (REG IN (" & Agro_SQL_Save_Clausola_IN(Lista_Regione_Cod, True) & ")) " & vbCrLf)
            End If

            stb.Append(" ORDER BY REG ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



            'Verifico la presenza di errori
            If dt.Rows.Count = 0 Then
                Throw New Exception("Errore [Lettura REGIONI] : " & MessaggioErrore)
                Return Nothing
                'Else

                '    dt.Columns("Regione_Des").ReadOnly = False
                '    dt.Columns("Regione_Sigla").ReadOnly = False

                '    For i = 0 To dt.Rows.Count - 1
                '        Codice = dt.Rows(i).Item("Regione_Cod")
                '        Select Case Codice

                '            Case "001"
                '                dt.Rows(i).Item("Regione_Des") = "Piemonte"
                '                dt.Rows(i).Item("Regione_Sigla") = "PIE"
                '            Case "002"
                '                dt.Rows(i).Item("Regione_Des") = "Valle d`Aosta"
                '                dt.Rows(i).Item("Regione_Sigla") = "VdA"
                '            Case "003"
                '                dt.Rows(i).Item("Regione_Des") = "Lombardia"
                '                dt.Rows(i).Item("Regione_Sigla") = "LOM"
                '            Case "004"
                '                dt.Rows(i).Item("Regione_Des") = "Trentino Alto Adige"
                '                dt.Rows(i).Item("Regione_Sigla") = "TAA"
                '            Case "005"
                '                dt.Rows(i).Item("Regione_Des") = "Veneto"
                '                dt.Rows(i).Item("Regione_Sigla") = "VEN"
                '            Case "006"
                '                dt.Rows(i).Item("Regione_Des") = "Friuli Venezia Giulia"
                '                dt.Rows(i).Item("Regione_Sigla") = "FVG"
                '            Case "007"
                '                dt.Rows(i).Item("Regione_Des") = "Liguria"
                '                dt.Rows(i).Item("Regione_Sigla") = "LIG"
                '            Case "008"
                '                dt.Rows(i).Item("Regione_Des") = "Emilia Romagna"
                '                dt.Rows(i).Item("Regione_Sigla") = "EMR"
                '            Case "009"
                '                dt.Rows(i).Item("Regione_Des") = "Toscana"
                '                dt.Rows(i).Item("Regione_Sigla") = "TOS"
                '            Case "010"
                '                dt.Rows(i).Item("Regione_Des") = "Umbria"
                '                dt.Rows(i).Item("Regione_Sigla") = "UMB"
                '            Case "011"
                '                dt.Rows(i).Item("Regione_Des") = "Marche"
                '                dt.Rows(i).Item("Regione_Sigla") = "MAR"
                '            Case "012"
                '                dt.Rows(i).Item("Regione_Des") = "Lazio"
                '                dt.Rows(i).Item("Regione_Sigla") = "LAZ"
                '            Case "013"
                '                dt.Rows(i).Item("Regione_Des") = "Abruzzo"
                '                dt.Rows(i).Item("Regione_Sigla") = "ABR"
                '            Case "014"
                '                dt.Rows(i).Item("Regione_Des") = "Molise"
                '                dt.Rows(i).Item("Regione_Sigla") = "MOL"
                '            Case "015"
                '                dt.Rows(i).Item("Regione_Des") = "Campania"
                '                dt.Rows(i).Item("Regione_Sigla") = "CAM"
                '            Case "016"
                '                dt.Rows(i).Item("Regione_Des") = "Puglia"
                '                dt.Rows(i).Item("Regione_Sigla") = "PUG"
                '            Case "017"
                '                dt.Rows(i).Item("Regione_Des") = "Basilicata"
                '                dt.Rows(i).Item("Regione_Sigla") = "BAS"
                '            Case "018"
                '                dt.Rows(i).Item("Regione_Des") = "Calabria"
                '                dt.Rows(i).Item("Regione_Sigla") = "CAL"
                '            Case "019"
                '                dt.Rows(i).Item("Regione_Des") = "Sicilia"
                '                dt.Rows(i).Item("Regione_Sigla") = "SIC"
                '            Case "020"
                '                dt.Rows(i).Item("Regione_Des") = "Sardegna"
                '                dt.Rows(i).Item("Regione_Sigla") = "SAR"
                '        End Select
                '    Next

            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function Provincie_Leggi( _
            ByVal Lista_Regione_Cod As String, _
            ByVal Lista_Provincia_Cod As String, _
            ByVal Lista_Provincia_Sigla As String, _
                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri _
                ) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""
        Dim Codice As String

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try
            stb.Append(" SELECT  DISTINCT ")
            stb.Append("         REG AS Regione_Cod, ")
            stb.Append("         'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Regione_Des,  ")
            stb.Append("         'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Regione_Sigla,  ")
            stb.Append("         PROV AS Provincia_Cod, ")
            stb.Append("         SIGLA AS Provincia_Sigla, ")
            stb.Append("         Provincia as Provincia_Des ")
            stb.Append(" FROM    Lista_Province ")
            stb.Append(" WHERE   (Reg <> '') ")


            If Lista_Regione_Cod <> "" Then
                stb.Append(" AND   (REG IN (" & Agro_SQL_Save_Clausola_IN(Lista_Regione_Cod, True) & ")) ")
            End If

            If Lista_Provincia_Cod <> "" Then
                stb.Append(" AND   (Prov IN (" & Agro_SQL_Save_Clausola_IN(Lista_Provincia_Cod, True) & ")) ")
            Else
                If Lista_Provincia_Sigla <> "" Then
                    stb.Append(" AND   (Sigla IN (" & Agro_SQL_Save_Clausola_IN(Lista_Provincia_Sigla, True) & ")) ")
                End If
            End If

            stb.Append(" ORDER BY REG, Provincia ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            'Verifico la presenza di errori
            If dt.Rows.Count = 0 Then
                Throw New Exception("Errore [Lettura PROVINCIE] : " & MessaggioErrore)
                Return Nothing
            Else

                dt.Columns("Regione_Des").ReadOnly = False
                dt.Columns("Regione_Sigla").ReadOnly = False

                For i = 0 To dt.Rows.Count - 1
                    Codice = dt.Rows(i).Item("Regione_Cod")
                    Select Case Codice

                        Case "001"
                            dt.Rows(i).Item("Regione_Des") = "Piemonte"
                            dt.Rows(i).Item("Regione_Sigla") = "PIE"
                        Case "002"
                            dt.Rows(i).Item("Regione_Des") = "Valle d'Aosta"
                            dt.Rows(i).Item("Regione_Sigla") = "VdA"
                        Case "003"
                            dt.Rows(i).Item("Regione_Des") = "Lombardia"
                            dt.Rows(i).Item("Regione_Sigla") = "LOM"
                        Case "004"
                            dt.Rows(i).Item("Regione_Des") = "Trentino Alto Adige"
                            dt.Rows(i).Item("Regione_Sigla") = "TAA"
                        Case "005"
                            dt.Rows(i).Item("Regione_Des") = "Veneto"
                            dt.Rows(i).Item("Regione_Sigla") = "VEN"
                        Case "006"
                            dt.Rows(i).Item("Regione_Des") = "Friuli Venezia Giulia"
                            dt.Rows(i).Item("Regione_Sigla") = "FVG"
                        Case "007"
                            dt.Rows(i).Item("Regione_Des") = "Liguria"
                            dt.Rows(i).Item("Regione_Sigla") = "LIG"
                        Case "008"
                            dt.Rows(i).Item("Regione_Des") = "Emilia Romagna"
                            dt.Rows(i).Item("Regione_Sigla") = "EMR"
                        Case "009"
                            dt.Rows(i).Item("Regione_Des") = "Toscana"
                            dt.Rows(i).Item("Regione_Sigla") = "TOS"
                        Case "010"
                            dt.Rows(i).Item("Regione_Des") = "Umbria"
                            dt.Rows(i).Item("Regione_Sigla") = "UMB"
                        Case "011"
                            dt.Rows(i).Item("Regione_Des") = "Marche"
                            dt.Rows(i).Item("Regione_Sigla") = "MAR"
                        Case "012"
                            dt.Rows(i).Item("Regione_Des") = "Lazio"
                            dt.Rows(i).Item("Regione_Sigla") = "LAZ"
                        Case "013"
                            dt.Rows(i).Item("Regione_Des") = "Abruzzo"
                            dt.Rows(i).Item("Regione_Sigla") = "ABR"
                        Case "014"
                            dt.Rows(i).Item("Regione_Des") = "Molise"
                            dt.Rows(i).Item("Regione_Sigla") = "MOL"
                        Case "015"
                            dt.Rows(i).Item("Regione_Des") = "Campania"
                            dt.Rows(i).Item("Regione_Sigla") = "CAM"
                        Case "016"
                            dt.Rows(i).Item("Regione_Des") = "Puglia"
                            dt.Rows(i).Item("Regione_Sigla") = "PUG"
                        Case "017"
                            dt.Rows(i).Item("Regione_Des") = "Basilicata"
                            dt.Rows(i).Item("Regione_Sigla") = "BAS"
                        Case "018"
                            dt.Rows(i).Item("Regione_Des") = "Calabria"
                            dt.Rows(i).Item("Regione_Sigla") = "CAL"
                        Case "019"
                            dt.Rows(i).Item("Regione_Des") = "Sicilia"
                            dt.Rows(i).Item("Regione_Sigla") = "SIC"
                        Case "020"
                            dt.Rows(i).Item("Regione_Des") = "Sardegna"
                            dt.Rows(i).Item("Regione_Sigla") = "SAR"

                    End Select
                Next

                Return dt

            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function Comuni_Leggi( _
            ByVal Lista_Provincia_Cod As String, _
            ByVal Lista_Provincia_Sigla As String, _
            ByVal Provincia_Cod As String, _
            ByVal Comune_Cod As String, _
                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri _
                ) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""
        Dim Codice As String
        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try
            stb.Append(" SELECT  Lista_Province.REG AS Regione_Cod,  ")
            stb.Append("         'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Regione_Sigla,  ")
            stb.Append("         'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Regione_Des,  ")
            stb.Append("         Lista_Province.PROV AS Provincia_Cod,  ")
            stb.Append("         Lista_Province.SIGLA AS Provincia_Sigla,  ")
            stb.Append("         Lista_Province.PROVINCIA AS Provincia_Des,  ")
            stb.Append("         ISTAT.COM AS Comune_Cod,  ")
            stb.Append("         ISTAT.LOCALITA AS Comune_Des, ")
            stb.Append("         ISTAT.CAP ")

            stb.Append(" FROM    Lista_Province INNER JOIN ")
            stb.Append("         ISTAT ON Lista_Province.PROV = ISTAT.PROV ")

            stb.Append(" WHERE   (Lista_Province.REG<>'') ")


            If Lista_Provincia_Cod <> "" Then
                stb.Append(" AND     (Lista_Province.PROV IN (" & Agro_SQL_Save_Clausola_IN(Lista_Provincia_Cod, True) & "))  ")
            Else
                If Lista_Provincia_Sigla <> "" Then
                    stb.Append(" AND     (Lista_Province.SIGLA IN (" & Agro_SQL_Save_Clausola_IN(Lista_Provincia_Sigla, True) & "))  ")
                Else
                    If (Provincia_Cod <> "") And (Comune_Cod <> "") Then
                        stb.Append(" AND     (Lista_Province.PROV = '" & Agro_SQL_SaveText(Provincia_Cod) & "')  ")
                        stb.Append(" AND     (ISTAT.COM = '" & Agro_SQL_SaveText(Comune_Cod) & "') ")
                    End If
                End If
            End If

            stb.Append(" ORDER BY Lista_Province.REG, Lista_Province.PROVINCIA,  ISTAT.LOCALITA")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If dt.Rows.Count = 0 Then
                Throw New Exception("Errore [Lettura COMUNI] : " & MessaggioErrore)
                Return Nothing
            Else

                dt.Columns("Regione_Des").ReadOnly = False
                dt.Columns("Regione_Sigla").ReadOnly = False

                For i = 0 To dt.Rows.Count - 1
                    Codice = dt.Rows(i).Item("Regione_Cod")
                    Select Case Codice

                        Case "001"
                            dt.Rows(i).Item("Regione_Des") = "Piemonte"
                            dt.Rows(i).Item("Regione_Sigla") = "PIE"
                        Case "002"
                            dt.Rows(i).Item("Regione_Des") = "Valle d'Aosta"
                            dt.Rows(i).Item("Regione_Sigla") = "VdA"
                        Case "003"
                            dt.Rows(i).Item("Regione_Des") = "Lombardia"
                            dt.Rows(i).Item("Regione_Sigla") = "LOM"
                        Case "004"
                            dt.Rows(i).Item("Regione_Des") = "Trentino Alto Adige"
                            dt.Rows(i).Item("Regione_Sigla") = "TAA"
                        Case "005"
                            dt.Rows(i).Item("Regione_Des") = "Veneto"
                            dt.Rows(i).Item("Regione_Sigla") = "VEN"
                        Case "006"
                            dt.Rows(i).Item("Regione_Des") = "Friuli Venezia Giulia"
                            dt.Rows(i).Item("Regione_Sigla") = "FVG"
                        Case "007"
                            dt.Rows(i).Item("Regione_Des") = "Liguria"
                            dt.Rows(i).Item("Regione_Sigla") = "LIG"
                        Case "008"
                            dt.Rows(i).Item("Regione_Des") = "Emilia Romagna"
                            dt.Rows(i).Item("Regione_Sigla") = "EMR"
                        Case "009"
                            dt.Rows(i).Item("Regione_Des") = "Toscana"
                            dt.Rows(i).Item("Regione_Sigla") = "TOS"
                        Case "010"
                            dt.Rows(i).Item("Regione_Des") = "Umbria"
                            dt.Rows(i).Item("Regione_Sigla") = "UMB"
                        Case "011"
                            dt.Rows(i).Item("Regione_Des") = "Marche"
                            dt.Rows(i).Item("Regione_Sigla") = "MAR"
                        Case "012"
                            dt.Rows(i).Item("Regione_Des") = "Lazio"
                            dt.Rows(i).Item("Regione_Sigla") = "LAZ"
                        Case "013"
                            dt.Rows(i).Item("Regione_Des") = "Abruzzo"
                            dt.Rows(i).Item("Regione_Sigla") = "ABR"
                        Case "014"
                            dt.Rows(i).Item("Regione_Des") = "Molise"
                            dt.Rows(i).Item("Regione_Sigla") = "MOL"
                        Case "015"
                            dt.Rows(i).Item("Regione_Des") = "Campania"
                            dt.Rows(i).Item("Regione_Sigla") = "CAM"
                        Case "016"
                            dt.Rows(i).Item("Regione_Des") = "Puglia"
                            dt.Rows(i).Item("Regione_Sigla") = "PUG"
                        Case "017"
                            dt.Rows(i).Item("Regione_Des") = "Basilicata"
                            dt.Rows(i).Item("Regione_Sigla") = "BAS"
                        Case "018"
                            dt.Rows(i).Item("Regione_Des") = "Calabria"
                            dt.Rows(i).Item("Regione_Sigla") = "CAL"
                        Case "019"
                            dt.Rows(i).Item("Regione_Des") = "Sicilia"
                            dt.Rows(i).Item("Regione_Sigla") = "SIC"
                        Case "020"
                            dt.Rows(i).Item("Regione_Des") = "Sardegna"
                            dt.Rows(i).Item("Regione_Sigla") = "SAR"

                    End Select
                Next

            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function TipologiaVarietale_Leggi( _
          ByVal Veg_Cod As Integer, _
          ByVal Grva_Cod As Integer, _
               ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri _
               ) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try
            stb.Append(" SELECT  GruppoVarietale.Grva_Cod,  ")
            stb.Append("         GruppoVarietale.Grva_Des,  ")
            stb.Append("         SpecieVegetali.Veg_Cod,  ")
            stb.Append("         SpecieVegetali.Veg_Des ")
            stb.Append(" FROM    GruppoVarietale INNER JOIN ")
            stb.Append("         SpecieVegetalixGruppoVarietale ON GruppoVarietale.Grva_Cod = SpecieVegetalixGruppoVarietale.Grva_Cod INNER JOIN ")
            stb.Append("         SpecieVegetali ON SpecieVegetalixGruppoVarietale.Veg_Cod = SpecieVegetali.Veg_Cod ")
            stb.Append(" WHERE   (GruppoVarietale.Grva_Cod <> -999999)  ")

            If Grva_Cod <> -1 Then
                stb.Append(" AND     (GruppoVarietale.Grva_Cod = " & Grva_Cod & ")   ")
            End If

            If Veg_Cod <> -1 Then
                stb.Append(" AND     (SpecieVegetali.Veg_Cod = " & Veg_Cod & ")   ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If dt.Rows.Count = 0 Then
                Throw New Exception("Errore [Lettura TIPOLOGIA VARIETALE] : " & MessaggioErrore)
                Return Nothing
            Else
                Return dt
            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function



    'Se non ci sono Tipologie non voglio comunque che generi errore
    Public Function TipologiaVarietale_Leggi2( _
      ByVal Veg_Cod As Integer, _
      ByVal Grva_Cod As Integer, _
           ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri _
           ) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try
            stb.Append(" SELECT  GruppoVarietale.Grva_Cod,  ")
            stb.Append("         GruppoVarietale.Grva_Des,  ")
            stb.Append("         SpecieVegetali.Veg_Cod,  ")
            stb.Append("         SpecieVegetali.Veg_Des ")
            stb.Append(" FROM    GruppoVarietale INNER JOIN ")
            stb.Append("         SpecieVegetalixGruppoVarietale ON GruppoVarietale.Grva_Cod = SpecieVegetalixGruppoVarietale.Grva_Cod INNER JOIN ")
            stb.Append("         SpecieVegetali ON SpecieVegetalixGruppoVarietale.Veg_Cod = SpecieVegetali.Veg_Cod ")
            stb.Append(" WHERE   (GruppoVarietale.Grva_Cod <> -999999)  ")

            If Grva_Cod <> -1 Then
                stb.Append(" AND     (GruppoVarietale.Grva_Cod = " & Grva_Cod & ")   ")
            End If

            If Veg_Cod <> -1 Then
                stb.Append(" AND     (SpecieVegetali.Veg_Cod = " & Veg_Cod & ")   ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            'If dt.Rows.Count = 0 Then
            '    Throw New Exception("Errore [Lettura TIPOLOGIA VARIETALE] : " & MessaggioErrore)
            '    Return Nothing
            'Else
            Return dt
            'End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function




    Public Function Leggi_Gerarchia_x_sementieri( _
                            ByVal Flag_SoloPadri As Boolean, _
                            ByVal Flag_FigliConImpianti As Boolean, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            stb.Length = 0
            If (Flag_SoloPadri = True) Then

                If (Flag_FigliConImpianti = True) Then
                    stb.AppendLine(" SELECT DISTINCT ")
                    stb.AppendLine(" Imprese.PIVA AS Padre_Piva, Imprese.rag_soc AS Padre_RagSoc, ")
                    stb.AppendLine("          'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Figlio_Piva,  ")
                    stb.AppendLine("          'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Figlio_RagSoc ")
                    stb.AppendLine("  FROM    Imprese INNER JOIN ")
                    stb.AppendLine(" (   Select distinct CODICE_FISCALE_TECNICO As padre, '' as figlio ")
                    stb.AppendLine("     From Reg_Impianti ")
                    stb.AppendLine(" ) ")
                    stb.AppendLine("          GerarchiaImprese ON Imprese.PIVA = GerarchiaImprese.Padre ")
                    stb.AppendLine("          LEFT JOIN  ")
                    stb.AppendLine("          Reg_Impianti ON GerarchiaImprese.Figlio = Reg_Impianti.PIVA ")
                    stb.AppendLine("          Left Join")
                    stb.AppendLine("          programmazione_entita ON GerarchiaImprese.Figlio = programmazione_entita.PIVA  ")
                    stb.AppendLine("  ORDER BY Imprese.rag_soc ")

                Else
                    'Provvisorio ...
                End If
            Else
                'Provvisorio ...
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

 