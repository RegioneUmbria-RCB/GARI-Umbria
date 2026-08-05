Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Ricette
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_Trattamenti( _
                        ByVal Ricetta_Cod As Integer, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Ricette.Leggi_Trattamenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT ")
            StrSQL.Append("         Ricette_Operazioni.Ricetta_Operazione_Cod, Ricette_Operazioni.Ricetta_Operazione_Des, ")
            StrSQL.Append("         Ricette_Operazioni.Lav_Cod, Operazioni.LAV_DES, Ricette_Operazioni.Note,  ")
            StrSQL.Append("         Ricette_Operazioni.Num_Protocollo, Ricette_Operazioni.Id_Rcdpi, Ricette_Operazioni.Extra_Int, Ricette_Operazioni.Mezzo,  ")
            StrSQL.Append("         Ricette_Operazioni.Validita_Inizio, Ricette_Operazioni.Validita_Fine, Ricette_Operazioni.Gru_Op, Ricette_Operazioni.Costo,  ")
            StrSQL.Append("         Ricette_Operazioni.Noleggio_Passivo, Ricette_Dettagli.Ricetta_Dettaglio_Cod,  ")
            StrSQL.Append("         Ricette_Dettagli.Miscela_Cod, Ricette_Dettagli.Elem_Cod,  ")
            StrSQL.Append("         Ricette_Dettagli.Pro_Cod, Ricette_Dettagli.Mat_Cod, ")
            StrSQL.Append("         Formulati.Fr_Des, Trappole.TRAP_DES, ")
            'StrSQL.Append("         ISNULL(Parco_Macchine.Mac_Des,'') AS Mac_Des, ")
            'StrSQL.Append("         ISNULL(Contatti.Rag_Soc,'') AS Rag_Soc, ")
            StrSQL.Append("         Ricette_Dettagli.Udm_Cod, UnitaMisura.UDM_DES, UnitaMisura.UDM_SIM, Ricette_Dettagli.Qta, Ricette_Dettagli.Extra_Int AS Extra_Int_Dett,  ")
            StrSQL.Append("         Ricette_Dettagli.Prezzo_Unitario, Ricette_Dettagli.Cau_Mov, Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod AS Ricetta_Dettaglio_Cod_Tecnico,  ")
            StrSQL.Append("         Ricette_Dettaglio_Tecnico.Miscela_Cod AS Miscela_Cod_Tecnico, Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod, Ricette_Dettaglio_Tecnico.Qta_Ril,  ")
            StrSQL.Append("         Ricette_Dettaglio_Tecnico.Av_Cod, Ricette_Dettaglio_Tecnico.Av_Gru, ")
            StrSQL.Append("         Avversita.Av_Des_Vol, GruppoAvversita.Av_Gru_Des, ")
            StrSQL.Append("         Ricette_Dettaglio_Tecnico.Dett_Cod, Ricette_Dettaglio_Tecnico.Dose,  ")
            StrSQL.Append("         Ricette_Dettaglio_Tecnico.Parziale, Ricette_Dettaglio_Tecnico.Nitrati, Ricette_Dettaglio_Tecnico.Freatimetro, Ricette_Dettaglio_Tecnico.Inn1_Data,  ")
            StrSQL.Append("         Ricette_Dettaglio_Tecnico.Inn2_Data, Ricette_Dettaglio_Tecnico.Inn3_Data, Ricette_Dettaglio_Tecnico.Inn4_Data, Ricette_Dettaglio_Tecnico.Ditta_Cod, ")
            StrSQL.Append("         Ricette_Dettaglio_Tecnico.Sigla_AV, Ricette_Dettaglio_Tecnico.Trap_Num, Ricette_Dettaglio_Tecnico.Id_Insetto, Ricette_Dettaglio_Tecnico.FF_Classe, ")
            StrSQL.Append("         Ricette_Destinazioni.Ricetta_Destinazione_Cod, Ricette_Destinazioni.Programmazione_Entita_Cod, Ricette_Destinazioni.Piva,  ")
            StrSQL.Append("         Ricette_Destinazioni.Sa_Cod, Ricette_Destinazioni.Appezza, Ricette_Destinazioni.Id_Reg, Ricette_Destinazioni.Qta AS Qta_Dest,  ")
            StrSQL.Append("         ISNULL(Reg_Impianti.CUL_COD,0) AS Cul_Cod, ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, Cultivar.Veg_Cod, ")
            StrSQL.Append("         Reg_Impianti.GRFI_COD, Reg_Impianti.GRVA_Cod_VEG,   ")
            StrSQL.Append("         Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto, Reg_Impianti.Sup_Imp,  ")
            StrSQL.Append("         Appezzamento.APP_NOME,  ")
            StrSQL.Append("         ISNULL ((SELECT     TOP 1 Appezzamento_Codici.val_cod ")
            StrSQL.Append("                 FROM Appezzamento_Codici ")
            StrSQL.Append("                 WHERE     (Appezzamento_Codici.PIVA = Appezzamento.PIVA) ")
            StrSQL.Append("                 AND (Appezzamento_Codici.sa_cod = Appezzamento.sa_cod)  ")
            StrSQL.Append("                 AND (Appezzamento_Codici.appezza = Appezzamento.appezza) ")
            StrSQL.Append("                 AND (Appezzamento_Codici.id_cod = 1104)), '') AS App_Nome_Breve ")
            StrSQL.Append("            ")
            StrSQL.Append("          ")
            StrSQL.Append("         ,ISNULL(pa.pa_cod,0) AS Pa_Cod, ISNULL(PA.pa_des,'') AS Pa_Des   ")


            StrSQL.Append(" FROM    Cultivar INNER JOIN ")
            StrSQL.Append("         Appezzamento INNER JOIN ")
            StrSQL.Append("         Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND  ")
            StrSQL.Append("         Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD RIGHT OUTER JOIN ")
            StrSQL.Append("         GruppoAvversita RIGHT OUTER JOIN ")
            StrSQL.Append("         Ricette_Operazioni INNER JOIN ")
            StrSQL.Append("         Ricette_Dettagli ON Ricette_Operazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND  ")
            StrSQL.Append("         Ricette_Operazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND  ")
            StrSQL.Append("         Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod INNER JOIN ")
            StrSQL.Append("         Ricette_Dettaglio_Tecnico ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Dettaglio_Tecnico.Ricetta_SuperUser AND  ")
            StrSQL.Append("         Ricette_Dettagli.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Cod AND  ")
            StrSQL.Append("         Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod INNER JOIN ")
            StrSQL.Append("         Operazioni ON Ricette_Operazioni.Lav_Cod = Operazioni.LAV_COD ON GruppoAvversita.Av_Gru = Ricette_Dettaglio_Tecnico.Av_Gru LEFT OUTER JOIN ")
            StrSQL.Append("         Avversita ON Ricette_Dettaglio_Tecnico.Av_Cod = Avversita.Av_Cod LEFT OUTER JOIN ")
            StrSQL.Append("         Trappole ON Ricette_Dettagli.Pro_Cod = Trappole.TRAP_COD LEFT OUTER JOIN ")
            StrSQL.Append("         UnitaMisura ON Ricette_Dettagli.Udm_Cod = UnitaMisura.UDM_COD LEFT OUTER JOIN ")
            'StrSQL.Append("         Parco_Macchine ON Ricette_Dettagli.Mat_Cod = Parco_Macchine.Mac_Cod LEFT OUTER JOIN ")
            'StrSQL.Append("         Risorse_Umane ON Ricette_Dettagli.Mat_Cod = Risorse_Umane.Cod_RisUm LEFT OUTER JOIN ")
            'StrSQL.Append("         Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto LEFT OUTER JOIN ")
            StrSQL.Append("         Formulati ON Ricette_Dettagli.Pro_Cod = Formulati.Fr_Cod LEFT OUTER JOIN ")
            StrSQL.Append("         FormulatixPrincipiAttivi FP ON formulati.Fr_Cod = FP.Fr_Cod  LEFT OUTER JOIN ")
            StrSQL.Append("         PrincipiAttivi PA ON FP.Pa_Cod = PA.Pa_Cod  LEFT OUTER JOIN ")
            StrSQL.Append("         Ricette_Destinazioni ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Destinazioni.Ricetta_SuperUser AND  ")
            StrSQL.Append("         Ricette_Dettagli.Ricetta_Cod = Ricette_Destinazioni.Ricetta_Cod AND  ")
            StrSQL.Append("         Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Destinazioni.Ricetta_Operazione_Cod AND  ")
            StrSQL.Append("         Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Destinazioni.Ricetta_Dettaglio_Cod ON Reg_Impianti.PIVA = Ricette_Destinazioni.Piva AND  ")
            StrSQL.Append("         Reg_Impianti.SA_COD = Ricette_Destinazioni.Sa_Cod AND Reg_Impianti.APPEZZA = Ricette_Destinazioni.Appezza AND  ")
            StrSQL.Append("         Reg_Impianti.ID_REG = Ricette_Destinazioni.Id_Reg ")

            StrSQL.Append(" WHERE   Ricette_Operazioni.Ricetta_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.Append(" AND Ricette_Operazioni.Lav_Cod IN (74, 18, 103, 155, 13, 158, 116, 118, 121) ")
            StrSQL.Append(" AND Ricette_Dettagli.Cau_Mov = '2050' ")
            'StrSQL.Append(" AND Ricette_Dettagli.Cau_Mov IN ('2050','8100','6800','6850') ")

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ricette_Operazioni.Validita_Inizio, Ricette_Operazioni.Ricetta_Operazione_Cod ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
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
