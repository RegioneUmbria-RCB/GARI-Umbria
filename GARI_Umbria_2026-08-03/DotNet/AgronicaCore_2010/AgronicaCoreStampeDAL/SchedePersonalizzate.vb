Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL


Public Class SchedePersonalizzate
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim BaseCode As Integer

    Public Function Esporta_GiasToSap(ByVal objC As Object,
                                  ByVal objPs As Object,
                                  ByVal objSV As Object,
                                  ByVal objV As Object,
                                  ByVal objDi As String,
                                  ByVal objDf As String,
                                  ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Definizione delle variabili

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.SchedePersonalizzate.Esporta_GiasToSap()"

        Dim DT As DataTable

        Dim Dt_struttura As New DataTable
        Dim Dt_new As DataTable
        Dim Dr As DataRow
        Dim Dr_new() As DataRow
        Dim Dr_Temp() As DataRow

        Dim StrSQL As New System.Text.StringBuilder

        Dim messaggioerrore As String = ""
        Dim strPianiSemina As String = ""
        Dim strCoop As String = ""
        Dim strVarieta As String = ""

        Dim i As Integer
        Dim s As String

        Dim Num_Lotti As Integer

        '----- Definisco la struttura del DataTable

        Dt_struttura.Columns.Add(New DataColumn("Chiave", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("PSe", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("Varieta", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("DesVarieta", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("PIvaCoop", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("RSCoop", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("Coop", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("PIvaSocio", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("RSSocio", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("Socio", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("DataSemina", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("Sup", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("QtaSeme", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("UdmSeme", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("Lotto", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("sa_nome", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("Sup2", GetType(String)))
        Dt_struttura.Columns.Add(New DataColumn("QtaSeme2", GetType(String)))

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" Select DISTINCT ")
            StrSQL.AppendLine(" ISNULL((SELECT     TOP 1 val_cod")
            StrSQL.AppendLine(" FROM          Reg_impianti_codici AS R2")
            StrSQL.AppendLine(" WHERE      id_cod = 1071 AND R1.piva = R2.piva AND R1.sa_cod = R2.sa_cod AND R1.appezza = R2.appezza AND R1.id_reg = R2.id_reg),'') ")
            StrSQL.AppendLine(" AS PSe, ")
            StrSQL.AppendLine(" CAC_Codifica_Cultivar.Cultivar_Coltiva AS Varieta, Cultivar.Cul_Des AS DesVarieta, R1.Cul_Cod, ")
            StrSQL.AppendLine(" ISNULL((SELECT     TOP 1 val_cod ")
            StrSQL.AppendLine(" FROM          Reg_impianti_codici AS R2 ")
            StrSQL.AppendLine(" WHERE      id_cod = 1074 AND R1.piva = R2.piva AND R1.sa_cod = R2.sa_cod AND R1.appezza = R2.appezza AND R1.id_reg = R2.id_reg),'') ")
            StrSQL.AppendLine(" AS Coop, ")
            StrSQL.AppendLine(" CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.Piva ELSE Imprese.partitaIvaReale END AS PIvaSocio, ")
            StrSQL.AppendLine(" Imprese.rag_soc AS RSSocio, ISNULL(Imprese_Codici.val_cod,'') AS Socio, ")
            StrSQL.AppendLine(" Agenda.Validita_Inizio AS DataSemina, Appezzamento.SUP_APP, R1.Sup_Imp AS Sup, ")
            StrSQL.AppendLine(" Mov_Destinazioni.Qta AS QtaSeme, UnitaMisura.UDM_SIM AS UdmSeme, Materie_Prime.Cod_Articolo AS Lotto, R1.SA_COD, R1.APPEZZA, R1.ID_REG, Centri_Aziendali.sa_nome ")

            StrSQL.AppendLine(" FROM  GerarchiaImprese INNER JOIN ")
            StrSQL.AppendLine(" Imprese_Codici INNER JOIN ")
            StrSQL.AppendLine(" Imprese ON Imprese_Codici.PIVA = Imprese.PIVA INNER JOIN ")
            StrSQL.AppendLine(" Centri_Aziendali INNER JOIN ")
            StrSQL.AppendLine(" Appezzamento ON Centri_Aziendali.PIVA = Appezzamento.PIVA AND ")
            StrSQL.AppendLine(" Centri_Aziendali.sa_cod = Appezzamento.SA_COD INNER JOIN ")
            StrSQL.AppendLine(" Reg_Impianti R1 ON Appezzamento.PIVA = R1.PIVA AND Appezzamento.SA_COD = R1.SA_COD AND ")
            StrSQL.AppendLine(" Appezzamento.APPEZZA = R1.APPEZZA INNER JOIN ")
            StrSQL.AppendLine(" Cultivar ON R1.CUL_COD = Cultivar.Cul_Cod INNER JOIN ")
            StrSQL.AppendLine(" SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            StrSQL.AppendLine(" Movimenti_dettagli INNER JOIN ")
            StrSQL.AppendLine(" Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND ")
            StrSQL.AppendLine(" Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
            StrSQL.AppendLine(" Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det AND ")
            StrSQL.AppendLine(" Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov INNER JOIN ")
            StrSQL.AppendLine(" Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND ")
            StrSQL.AppendLine(" Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            StrSQL.AppendLine(" Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND ")
            StrSQL.AppendLine(" Movimenti.Id_Agenda = Agenda.Id_Agenda INNER JOIN ")
            StrSQL.AppendLine(" UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD ON R1.PIVA = Mov_Destinazioni.Piva AND ")
            StrSQL.AppendLine(" R1.SA_COD = Mov_Destinazioni.Sa_Cod AND R1.APPEZZA = Mov_Destinazioni.Appezza AND R1.ID_REG = Mov_Destinazioni.Id_Destinazione ON ")
            StrSQL.AppendLine(" Imprese.PIVA = Centri_Aziendali.PIVA ON GerarchiaImprese.Figlio = Imprese.PIVA LEFT OUTER JOIN ")
            StrSQL.AppendLine(" Imprese Imprese_1 ON GerarchiaImprese.Padre = Imprese_1.PIVA LEFT OUTER JOIN ")
            StrSQL.AppendLine(" Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND ")
            StrSQL.AppendLine(" Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod LEFT OUTER JOIN ")
            StrSQL.AppendLine(" CAC_Codifica_Cultivar ON R1.CUL_COD = CAC_Codifica_Cultivar.Cultivar_Gias ")

            StrSQL.AppendLine(" WHERE     (Agenda.Lav_Cod = 2 OR Agenda.Lav_Cod = 71) ")
            StrSQL.AppendLine(" AND Movimenti.Cau_Mov = '2300' ")
            StrSQL.AppendLine(" AND Imprese_Codici.id_cod = 1033 ")

            If objSV("Testo") <> "" Then
                StrSQL.AppendLine(" AND SpecieVegetali.Veg_Cod = " & objSV("Valore"))
            End If


            For Each s In objV
                If s <> "" Then
                    strVarieta &= s & ","
                Else

                End If
            Next

            If strVarieta <> "" Then
                strVarieta = Left(strVarieta, strVarieta.Length - 1)
                StrSQL.AppendLine(" AND R1.Cul_Cod IN (" & Agro_SQL_Save_Clausola_IN(strVarieta) & ")")
            End If

            If objDi <> "" Then
                If objDf <> "" Then
                    'Caso Intervallo/Giorno
                    StrSQL.AppendLine(" AND R1.Validita_Inizio >= " & Agro_SQL_SaveDate(CDate(objDi)))
                    StrSQL.AppendLine(" AND R1.Validita_Inizio <= " & Agro_SQL_SaveDate(CDate(objDf)))
                Else
                    'Caso Anno
                    StrSQL.AppendLine(" AND R1.Validita_Inizio >= " & Agro_SQL_SaveDate(CDate("01/01/" & objDi)))
                    StrSQL.AppendLine(" AND R1.Validita_Inizio <= " & Agro_SQL_SaveDate(CDate("31/12/" & objDi)))
                End If
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                'filtro su eventuali piani semina scelti..
                If objPs Is Nothing Then

                Else
                    If objPs(0) <> "" Then
                        For Each s In objPs
                            strPianiSemina &= " PSe ='" & s.ToString & "' OR"
                        Next
                    End If
                End If

                If strPianiSemina <> "" Then
                    strPianiSemina = Left(strPianiSemina, strPianiSemina.Length - 3)
                    Dim drselect = DT.Select(strPianiSemina)
                    If drselect.Count > 0 Then
                        DT = drselect.CopyToDataTable
                    Else
                        DT = New DataTable
                    End If
                End If

                For Each row In DT.Rows

                    'Creo una nuova riga
                    Dr = Dt_struttura.NewRow

                    'la riempio
                    Dr.Item("Chiave") = Right("00000000000" & row("PIvaSocio"), 11) & Right("00" & (row("SA_COD") - BaseCode), 2) & Right("000" & (row("APPEZZA") - BaseCode), 3) & Right("0000" & (row("ID_REG") - BaseCode), 4)

                    Dr.Item("PSe") = row("PSe")
                    Dr.Item("Varieta") = row("Varieta")
                    Dr.Item("DesVarieta") = row("DesVarieta")

                    'Aggiunto riferimento per la funzione RagSoc_from_Piva
                    Dim impr As New AgronicaCoreAnagrafeDAL.Imprese_Read

                    If Not IsDBNull(row("Coop")) Then
                        Dr.Item("PIvaCoop") = row("Coop").ToString
                        Dr.Item("RSCoop") = impr.RagSoc_from_Piva(row("Coop").ToString, objParametri)
                    End If

                    Dr.Item("Coop") = "000000"

                    Dr.Item("PIvaSocio") = Right("00000000000" & row("PIvaSocio"), 11)
                    Dr.Item("RSSocio") = row("RSSocio")
                    Dr.Item("Socio") = row("Socio")
                    Dr.Item("DataSemina") = row("DataSemina")

                    Dr.Item("Sup") = row("Sup")

                    Dr.Item("QtaSeme") = row("QtaSeme")

                    If row("UdmSeme") = "unità di seme" Then
                        Dr.Item("UdmSeme") = "Cfz"
                    Else
                        Dr.Item("UdmSeme") = row("UdmSeme")
                    End If

                    Dr.Item("Lotto") = row("Lotto")
                    Dr.Item("sa_nome") = row("sa_nome")

                    Dr.Item("Sup2") = row("Sup")

                    Dr.Item("QtaSeme2") = row("QtaSeme")

                    'aggiungo la riga
                    Dt_struttura.Rows.Add(Dr)

                Next

                DT = Nothing

            End If

            'ciclo per dividere eventuali sup doppie...
            For i = 0 To Dt_struttura.Rows.Count - 1

                Dr_Temp = Dt_struttura.Select("Chiave = '" & Dt_struttura.Rows(i).Item("Chiave").ToString & "'")
                Num_Lotti = Dr_Temp.Length
                Dt_struttura.Rows(i).Item("Sup") = Dt_struttura.Rows(i).Item("Sup") / Num_Lotti
                Dt_struttura.Rows(i).Item("Sup2") = Math.Round(CDbl(Dt_struttura.Rows(i).Item("Sup2") / Num_Lotti), 2)

            Next

            'filtro sulle cooperative..
            'lo faccio ora sul dt perchè ho ricavato i dati delle cooperative solo dopo aver creato il rs..

            For Each s In objC
                strCoop &= " PIvaCoop ='" & s & "' OR"
            Next

            If strCoop <> "" Then
                strCoop = Left(strCoop, strCoop.Length - 3)
                Dt_new = New DataTable
                Dt_new = Dt_struttura.Clone
                Dr_new = Dt_struttura.Select(strCoop)
            End If

            If Not Dr_new Is Nothing Then
                For i = 0 To UBound(Dr_new)
                    Dt_new.ImportRow(Dr_new(i))
                Next
                Return Dt_new
            Else
                Return Dt_struttura
            End If

        Catch ex As Exception

            messaggioerrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioerrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioerrore)

        End Try

    End Function

    '###########################################################################

End Class
