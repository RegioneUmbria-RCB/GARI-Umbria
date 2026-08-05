Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMVVCommon

Public Class MVVElettronico_R : Inherits DALBase

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Function LeggiMVV(ByVal Piva As String, ByVal Id_Agenda As Integer, ByVal Data_Dal As Date, ByVal Data_Al As Date) As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.LeggiMVV"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT * FROM GIASMVVE ")
            StrSQL.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.Append(" AND Documento_DataDocumento <= " & Agro_SQL_SaveDate(Data_Al) & " ")
            StrSQL.Append(" AND Documento_DataDocumento >= " & Agro_SQL_SaveDate(Data_Dal) & " ")

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine("AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

    Public Function CercaSoggettiSian(ByVal CodiciSoggetto As List(Of String)) As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.CercaSoggettoSian"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim codici As String = String.Join(",", CodiciSoggetto)

        Try

            StrSQL.AppendLine("SELECT * FROM ws_RegVino_Soggetti ")
            StrSQL.AppendLine(" WHERE CodiceSoggetto IN (" & Agro_SQL_Save_Clausola_IN(codici, True) & ") ")
            StrSQL.AppendLine(" AND Gias_Stato = " & Agro_SQL_SaveNum(enum_WWorflow_WAnagraficaStati.Teleregistri_Valida_nel_SIAN))

            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

    Public Function CercaSoggettoSian(ByVal CodiceSoggetto As String) As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.CercaSoggettoSian"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT * FROM ws_RegVino_Soggetti ")
            StrSQL.AppendLine(" WHERE CodiceSoggetto = '" & Agro_SQL_SaveText(CodiceSoggetto) & "' ")
            StrSQL.AppendLine(" AND Gias_Stato = " & Agro_SQL_SaveNum(enum_WWorflow_WAnagraficaStati.Teleregistri_Valida_nel_SIAN))

            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

    Public Function CercaProdottoSian(ByVal codOper As String, ByVal mat_cod As Integer, ByVal lotto As String) As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.CercaProdottoSian"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT * FROM ws_RegVino_Prodotti ")
            StrSQL.AppendLine(" WHERE CodOper = '" & Agro_SQL_SaveText(codOper) & "' ")
            StrSQL.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(mat_cod))
            StrSQL.AppendLine(" AND Lotto = '" & Agro_SQL_SaveText(lotto) & "' ")

            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

    Public Function CaricaStatiMembri() As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.CaricaStatiMembri"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT * FROM ACCDAA_ANAG_T004_TabellaCodiciStatiMembri ")
            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try
    End Function

    Public Function CaricaPaesiTerzi() As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.CaricaPaesiTerzi"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT * FROM ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ")
            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try
    End Function
    Public Function CaricaUnitaTrasporto() As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.CaricaUnitaTrasporto"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT Codice, Descrizione, Sigla FROM ACCDAA_ANAG_T009_TabellaCodiciUnitaDiTrasporto  ")
            StrSQL.AppendLine(" WHERE Tipo = " & Agro_SQL_SaveNum(4))
            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

    Public Function CaricaMatriceProdottiSian() As DataTable


        Dim nomeProcedura As String = "MVVElettronico_R.CaricaMatriceProdottiSian"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT * FROM MatriceProdottiSian  ")
            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

    Public Function CausaliTrasportoSian() As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.CausaliTrasportoSian"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT * FROM Causali_Trasporto  where Tipo = 4 ")
            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

    Public Function LeggiIndirizzoConsegnaDiverso(ByVal codIndirizzo As Integer) As MVV_Indirizzo

        Dim nomeProcedura As String = "MVVElettronico_R.LeggiIndirizzoConsegnaDiverso"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim indirizzo As MVV_Indirizzo = Nothing

        Try

            StrSQL.AppendLine("select ")
            StrSQL.AppendLine(" COALESCE(Indirizzi_Consegna.ind_des,'') AS Luogo_Consegna_Indirizzo ")
            StrSQL.AppendLine(" , COALESCE(Indirizzi_Consegna.frz_des,'') AS Luogo_Consegna_Frazione ")
            StrSQL.AppendLine(" , COALESCE(Indirizzi_Consegna.CAP,'') AS Luogo_Consegna_CAP ")
            StrSQL.AppendLine(" , COALESCE(Indirizzi_Consegna.stato  collate SQL_Latin1_General_CP850_CI_AS,'') AS Luogo_Consegna_Stato ")
            StrSQL.AppendLine(" , COALESCE(Istat_Indirizzo_Consegna.LOCALITA,'') AS Luogo_Consegna_Localita ")
            StrSQL.AppendLine(" , coalesce( Istat_Indirizzo_Consegna.LOCALITA + ' ' + Istat_Indirizzo_Consegna.COMUNI_PROV, '') AS Luogo_Consegna_Comune_Provincia ")
            StrSQL.AppendLine(" , coalesce(Istat_Indirizzo_Consegna.Prov, '') as Luogo_Consegna_Provincia_Istat ")
            StrSQL.AppendLine(" , coalesce(Istat_Indirizzo_Consegna.Com, '') as Luogo_Consegna_Comune_Istat ")
            StrSQL.AppendLine(" From Indirizzi As Indirizzi_Consegna ")
            StrSQL.AppendLine(" Left Join ISTAT AS Istat_Indirizzo_Consegna ")
            StrSQL.AppendLine(" On Indirizzi_Consegna.pro_cod_istat = Istat_Indirizzo_Consegna.PROV ")
            StrSQL.AppendLine(" And Indirizzi_Consegna.com_cod_istat = Istat_Indirizzo_Consegna.COM ")
            StrSQL.AppendLine(" where Indirizzi_Consegna.cod_indirizzo = " & Agro_SQL_SaveNum(codIndirizzo))
            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)


            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                indirizzo = New MVV_Indirizzo With {
                    .CAP = dt.Rows(0).Item("Luogo_Consegna_CAP"),
                    .Comune_Provincia = dt.Rows(0).Item("Luogo_Consegna_Comune_Provincia"),
                    .Indirizzo = dt.Rows(0).Item("Luogo_Consegna_Indirizzo"),
                    .Stato = dt.Rows(0).Item("Luogo_Consegna_Stato"),
                    .Istat_Provincia = dt.Rows(0).Item("Luogo_Consegna_Provincia_Istat"),
                    .Istat_Comune = dt.Rows(0).Item("Luogo_Consegna_Comune_Istat")
                }
            End If

            Return indirizzo

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try


    End Function

End Class
