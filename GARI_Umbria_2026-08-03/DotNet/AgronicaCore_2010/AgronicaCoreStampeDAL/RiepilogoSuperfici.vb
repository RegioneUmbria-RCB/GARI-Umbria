Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class RiepilogoSuperfici
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Recupera_Superfici_Utilizzo_Colture(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal DataRecupero As Date,
                                                        ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RiepilogoSuperfici.Recupera_Superfici_Utilizzo_Colture"
        Dim DT As DataTable
        Dim StbSQL As New Text.StringBuilder

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Appezzamento.PIVA, Appezzamento.SA_COD,  ")
            StbSQL.AppendLine(" Reg_Impianti.cul_cod, SpecieVegetali.Veg_Des, SpecieVegetali.Veg_Cod, ")
            StbSQL.AppendLine(" Appezzamento.APPEZZA, Appezzamento.APP_NOME, Reg_Impianti.sup_imp AS SUP_APP, ")
            StbSQL.AppendLine(" Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine, ")
            StbSQL.AppendLine(" Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine,  ")

            'Questa parte di query mi restituisce il numero di specie distinte utilizzate nell'impresa

            StbSQL.AppendLine(" (SELECT  count (distinct SpecieVegetali.Veg_Cod) ")
            StbSQL.AppendLine("  FROM Appezzamento, Reg_Impianti, SpecieVegetali, Cultivar ")
            StbSQL.AppendLine(" WHERE   Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND   Appezzamento.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            StbSQL.AppendLine(" AND     Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND     Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND Reg_Impianti.PIVA=Appezzamento.PIVA")
            StbSQL.AppendLine(" AND Reg_Impianti.SA_COD=Appezzamento.SA_COD")
            StbSQL.AppendLine(" AND Reg_Impianti.appezza=Appezzamento.appezza")
            StbSQL.AppendLine(" AND Reg_Impianti.cul_cod=Cultivar.cul_cod")
            StbSQL.AppendLine(" AND SpecieVegetali.veg_cod=Cultivar.veg_cod")
            StbSQL.AppendLine(" AND Reg_Impianti.cul_cod <> 0")
            StbSQL.AppendLine(" )  as Numero_Specie ")

            StbSQL.AppendLine(" FROM Appezzamento, Reg_Impianti, SpecieVegetali, Cultivar ")

            StbSQL.AppendLine(" WHERE   Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND   Appezzamento.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            StbSQL.AppendLine(" AND   Reg_Impianti.cul_cod <> 0 ")
            StbSQL.AppendLine(" AND   Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND   Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(DataRecupero) & " ")

            StbSQL.AppendLine(" AND Reg_Impianti.PIVA=Appezzamento.PIVA ")
            StbSQL.AppendLine(" AND Reg_Impianti.SA_COD=Appezzamento.SA_COD ")
            StbSQL.AppendLine(" AND Reg_Impianti.appezza=Appezzamento.appezza ")
            StbSQL.AppendLine(" AND Reg_Impianti.cul_cod=Cultivar.cul_cod ")
            StbSQL.AppendLine(" AND SpecieVegetali.veg_cod=Cultivar.veg_cod ")

            StbSQL.AppendLine(" order by SpecieVegetali.Veg_Des ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT


    End Function



    Public Function Recupera_Superfici_Utilizzo_TerreniNudi(ByVal Piva As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal DataRecupero As Date,
                                                            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RiepilogoSuperfici.Recupera_Superfici_Utilizzo_TerreniNudi"
        Dim DT As DataTable
        Dim StbSQL As New Text.StringBuilder

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  DISTINCT Appezzamento.PIVA, Appezzamento.SA_COD,  ")
            StbSQL.AppendLine(" Reg_Impianti.cul_cod,  ")
            StbSQL.AppendLine(" Appezzamento.APPEZZA, Appezzamento.APP_NOME, Reg_Impianti.sup_imp AS SUP_APP, ")
            StbSQL.AppendLine(" Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine, ")
            StbSQL.AppendLine(" Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine,  ")
            StbSQL.AppendLine(" Reg_Impianti_Codici.id_cod ")

            StbSQL.AppendLine(" FROM Reg_Impianti_Codici INNER JOIN")
            StbSQL.AppendLine(" Appezzamento INNER JOIN")
            StbSQL.AppendLine(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
            StbSQL.AppendLine(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND ")
            StbSQL.AppendLine(" Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD AND Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND ")
            StbSQL.AppendLine(" Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG")

            StbSQL.AppendLine(" WHERE   Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND   Appezzamento.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            StbSQL.AppendLine(" AND   Reg_Impianti.cul_cod = 0 ")
            StbSQL.AppendLine(" AND   Reg_Impianti_Codici.id_cod >= 3000 ")
            StbSQL.AppendLine(" AND   Reg_Impianti_Codici.id_cod < 4000 ")
            'Escludo le tare improduttive '3015'
            StbSQL.AppendLine(" AND   Reg_Impianti_Codici.id_cod <> 3015 ")

            StbSQL.AppendLine(" AND   Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND   Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRecupero) & " ")
            StbSQL.AppendLine(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(DataRecupero) & " ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT


    End Function

End Class
