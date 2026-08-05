Imports System.Text
Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.ListExtensions
Imports AgronicaCoreAnagrafeDAL



Public Class MaterialeVivaistico
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function OrdiniVivaio(ByVal Programmazione_Cod_Lista As List(Of Integer),
                                                ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.MaterialeVivaistico.OrdiniVivaio()"

        Dim messaggioErrore As String = ""
        Dim SQL_Generale As New StringBuilder
        Dim DT As DataTable
        Dim StrQuery_Output As String = ""

        Try



            SQL_Generale.Append(" select isnull(GR.GruppoRaccolta_Des, ' ') as 'GruppoRaccolta_Des'
                ,isnull( (select CONCAT(Nome, ' ', Cognome) From Contatti where Contatti.Cod_Contatto = ImpC.val_cod), ' ') as 'NCtecnico'
                ,isnull( (select Mat_Des from Materie_Prime WHERE Materie_Prime.Mat_Cod = Bimp.Mat_Cod), ' ') as 'Coltura'
                ,PT.note AS 'rag_soc_vivaio'
                ,Bri.Sup_Imp
                ,PE.Num_Piante
                ,PE.Macrouso_Cod as 'Num_Seme'
                ,Pe.Programmazione_Cod
                ,datepart(WEEK, PE.Data_Fioritura_Prevista) AS 'Settimana_Trapianto'
                ,datepart(WEEK, Bimp.Data_Fine_Prevista) AS 'Settimana_Raccolta'
                ,'TipoImpianto' = CASE
                    WHEN Bri.SETUP_COD = '0' Then ' '
                    ELSE Bri.SETUP_COD
                END
                , TS.Descrizione AS 'Tipo_Seme'
                , PE.Data_Fioritura_Prevista
                ,Bimp.Data_Fine_Prevista
                , Imp.rag_soc
                , C.Cul_Des
                , CONCAT(Budget_piva, '_', Bimp.Mat_Cod) AS 'Gruppo'
                , Imp.PIVA
                , Plateau.descrizione as 'Plateau_Des'
            from programmazione_entita PE
            INNER JOIN Programmazione_Testata PT ON (PE.Programmazione_Cod = PT.Programmazione_Cod)
            INNER JOIN Imprese Imp ON (Imp.PIVA = PE.Budget_Piva)
            LEFT JOIN Gruppi_Raccolta GR ON (Imp.GruppoRaccolta_Cod = GR.GruppoRaccolta_Cod)
            LEFT JOIN Imprese_Codici ImpC ON (ImpC.PIVA = PE.Budget_Piva AND Impc.id_cod = 1088)
            LEFT JOIN Budget_Imprese_Progetti Bimp ON (Bimp.Piva = PE.Budget_Piva AND Bimp.Id_Reg = PE.Budget_Id_Reg AND Bimp.Sa_Cod = PE.Budget_Sa_Cod AND Bimp.Appezza = PE.Budget_Appezza AND Bimp.Id_Budget = PE.Id_Budget )
            LEFT JOIN Budget_Reg_Impianti Bri ON (Bri.Piva = PE.Budget_Piva AND Bri.Id_Reg = PE.Budget_Id_Reg AND Bri.Sa_Cod = PE.Budget_Sa_Cod AND Bri.Appezza = PE.Budget_Appezza AND Bri.Id_Budget = PE.Id_Budget )
            LEFT JOIN Materie_Prime MP ON (MP.Mat_Cod = PE.Superficie_Futura AND MP.Elem_Cod = 10 AND MP.Sem_Cod = 1)
            LEFT JOIN Tecnologie_Sementi Ts ON (Ts.Cod_TecnologiaSementi = MP.Cod_TecnologiaSementi)
            LEFT JOIN cultivar C on (C.Veg_Cod = Mp.Veg_Cod And C.Cul_Cod = Mp.Cul_Cod)
            LEFT JOIN Plateau on (Plateau.id_plateau = PE.id_plateau)
            WHERE PT.Tipo_Pianificazione = 12
            AND PE.Programmazione_Cod IN ").AppendLine(Programmazione_Cod_Lista.ToQueryInExpression)
            SQL_Generale.AppendLine("Order by Gruppo, Settimana_Trapianto,  C.Cul_Des")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, SQL_Generale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function

    Public Function leggiDatiConferimentoDettaglio(piva As String,
                                                  validita_inizio As Date,
                                                  validita_fine As Date,
                                                   isBudget As Boolean,
                                                  ByRef objParametriServer As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.MaterialeVivaistico.leggiDatiConferimentoDettaglio()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New StringBuilder
        Dim DT As DataTable
        Dim StrQuery_Output As String = ""
        Dim prefixTable = If(isBudget, "Budget_", "")

        Try


            strSQL.AppendLine("select BIP.Mat_Cod, MAX(MP.Mat_Des) as 'NomeProdotto', SUM(BA.SUP_APP) AS 'HaFascicolo', SUM(BIP.produzione_Prevista * BA.SUP_APP)/1000 AS 'Previsione_Produttiva'")
            strSQL.AppendFormat("from {0}Imprese_Progetti BIP", prefixTable).AppendLine()

            If isBudget Then
                strSQL.AppendLine("INNER JOIN Budget_Testata BT ON BT.id_budget = BIP.id_budget")
            End If

            strSQL.AppendLine("INNER JOIN Materie_Prime MP on MP.Mat_Cod = BIP.Mat_Cod")
            strSQL.AppendFormat("INNER JOIN {0}Appezzamento BA ON BA.PIVA = BIP.Piva AND BA.SA_COD = BIP.Sa_Cod AND BA.APPEZZA = BIP.Appezza", prefixTable)

            If isBudget Then
                strSQL.Append(" AND BA.Id_Budget = BT.id_budget")
            End If
            strSQL.AppendLine()

            strSQL.AppendLine("WHERE 1 = 1")

            If isBudget Then
                strSQL.AppendFormat(" AND BT.In_Uso = 1 ")
            End If
            strSQL.AppendFormat("AND BIP.Piva = '{0}'", piva).AppendLine()
            strSQL.AppendLine("AND MP.Mat_Cod <> 0 ")
            strSQL.AppendFormat("AND BIP.Validita_Inizio >= {0} AND BIP.Validita_Fine <= {1}", Agro_SQL_SaveDate(validita_inizio), Agro_SQL_SaveDate(validita_fine))
            strSQL.AppendLine("GROUP BY BIP.Mat_Cod")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function

    Public Function leggiDatiConferimentoGenerale(piva As String,
                                                  ByRef objParametriServer As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.MaterialeVivaistico.leggiDatiConferimentoGenerale()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New StringBuilder
        Dim DT As DataTable
        Dim StrQuery_Output As String = ""

        Try


            strSQL.AppendLine("With datiRubrica AS (")
            strSQL.AppendLine("SELECT")
            strSQL.AppendLine("CA.piva,")
            strSQL.AppendLine("CA.sa_cod,")
            strSQL.AppendLine("MAX(CASE WHEN R.descr = 'Telefono' THEN R.numero ELSE NULL END) AS Telefono,")
            strSQL.AppendLine("MAX(CASE WHEN R.descr = 'Cellulare' THEN R.numero ELSE NULL END) AS Cellulare,")
            strSQL.AppendLine("MAX(CASE WHEN R.descr = 'Fax' THEN R.numero ELSE NULL END) AS Fax,")
            strSQL.AppendLine("MAX(CASE WHEN R.descr = 'Email' THEN R.numero ELSE NULL END) AS Email,")
            strSQL.AppendLine("MAX(CASE WHEN R.descr = 'Pec' THEN R.numero ELSE NULL END) AS Pec")
            strSQL.AppendLine("FROM")
            strSQL.AppendLine("Centri_Aziendali CA")
            strSQL.AppendLine("INNER JOIN")
            strSQL.AppendLine("Centri_Aziendali_Codici CAC ON CAC.PIVA = CA.PIVA AND CAC.sa_cod = CA.sa_cod")
            strSQL.AppendLine("LEFT JOIN")
            strSQL.AppendLine("CentriXRubrica CXR ON CXR.piva = CA.PIVA AND CXR.sa_cod = CA.sa_cod")
            strSQL.AppendLine("LEFT JOIN")
            strSQL.AppendLine("Rubrica R ON R.cod_rubrica = CXR.cod_rubrica AND R.descr IN ('Telefono', 'Cellulare', 'Fax', 'Email', 'Pec')")
            strSQL.AppendLine("WHERE")
            strSQL.AppendLine("CAC.id_cod = 101")
            strSQL.AppendLine("GROUP BY")
            strSQL.AppendLine("CA.PIVA, CA.sa_cod")
            strSQL.AppendLine("), datiCodici AS (")
            strSQL.AppendLine("SELECT")
            strSQL.AppendLine("I.piva,")
            strSQL.AppendLine("I.rag_soc,")
            strSQL.AppendLine("MAX(CASE WHEN ISNULL(I.partitaIvaReale, '') = '' THEN I.piva ELSE I.partitaIvaReale	END) PivaReale,")
            strSQL.AppendLine("MAX(CASE WHEN IC.id_cod = '1010' THEN IC.val_cod ELSE NULL END) AS CUAA,")
            strSQL.AppendLine("MAX(CASE WHEN IC.id_cod = '1304' THEN IC.val_cod ELSE NULL END) AS UfficioREA,")
            strSQL.AppendLine("MAX(CASE WHEN IC.id_cod = '1305' THEN IC.val_cod ELSE NULL END) AS NumeroREA")
            strSQL.AppendLine("FROM")
            strSQL.AppendLine("Imprese I")
            strSQL.AppendLine("INNER JOIN")
            strSQL.AppendLine("Imprese_Codici IC ON IC.PIVA = I.PIVA")
            strSQL.AppendLine("GROUP BY I.PIVA, I.rag_soc")
            strSQL.AppendLine(")")
            strSQL.AppendLine("")
            strSQL.AppendLine("SELECT da.PivaReale, CA.sa_cod, Ind.ind_des, Ind.cap, Ind.com_des, Ind.pro_cod, dr.*, da.*")
            strSQL.AppendLine("FROM Centri_Aziendali CA")
            strSQL.AppendLine("INNER JOIN Centri_Aziendali_Codici CAC ON Cac.PIVA = CA.PIVA AND Cac.sa_cod = CA.sa_cod")
            strSQL.AppendLine("LEFT JOIN CentrixIndirizzi CI ON CI.PIVA = CA.PIVA AND CI.sa_cod = CA.sa_cod")
            strSQL.AppendLine("LEFT JOIN Indirizzi Ind ON CI.cod_indirizzo = Ind.cod_indirizzo")
            strSQL.AppendLine("LEFT JOIN datiRubrica dr ON dr.PIVA = CA.PIVA AND dr.sa_cod = CA.sa_cod")
            strSQL.AppendLine("LEFT JOIN datiCodici da on da.PIVA = CA.PIVA")
            strSQL.AppendFormat("WHERE CA.PIVA = '{0}'", piva).AppendLine()
            strSQL.AppendLine("AND CAC.id_cod = 101")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function
    Public Function leggiContratti(piva As String,
                                   dataIniziale As Date,
                                   dataFinale As Date,
                               ByRef objParametriServer As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.MaterialeVivaistico.leggiDatiConferimentoGenerale()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New StringBuilder
        Dim DT As DataTable
        Dim StrQuery_Output As String = ""

        Try


            strSQL.AppendLine("select IC.Contratto_Nome, IC.Data_Stipulazione, ICF.QtaPrevista /1000 as QtaPrevista, ICF.Superficie, ICF.Tipo_Trasporto, IC.sa_cod, I.ind_des, I.frz_des, I.CAP, I.com_des, CONCAT(MP.Cod_Articolo, ' ', MP.Mat_Des) as coltura, Imprese.rag_soc from Imprese_Contratti IC")
            strSQL.AppendLine("INNER JOIN")
            strSQL.AppendLine("Risorse_Umane on Risorse_Umane.Cod_RisUm = IC.Cod_Risum")
            strSQL.AppendLine("LEFT JOIN")
            strSQL.AppendLine("Imprese_Contratto_Fasi ICF On ICF.Contratto_cod = IC.Contratto_cod")
            strSQL.AppendLine("LEFT JOIN")
            strSQL.AppendLine("CentrixIndirizzi ON CentrixIndirizzi.PIVA = IC.Piva AND CentrixIndirizzi.sa_cod = IC.sa_cod")
            strSQL.AppendLine("LEFT JOIN")
            strSQL.AppendLine("Indirizzi I ON CentrixIndirizzi.cod_indirizzo = I.cod_indirizzo")
            strSQL.AppendLine("LEFT JOIN")
            strSQL.AppendLine("Imprese ON Imprese.PIVA = IC.Piva")
            strSQL.AppendLine("LEFT JOIN Materie_Prime MP")
            strSQL.AppendLine("ON MP.Elem_Cod = ICF.Elem_Cod AND MP.Mat_Cod = ICF.Mat_Cod")
            strSQL.AppendLine("Where")
            strSQL.AppendFormat("IC.Piva = '{0}'", piva).AppendLine()
            strSQL.AppendLine(" AND IC.Cau_Contratto = 9301")
            strSQL.Append("AND IC.Data_Stipulazione >= ").AppendLine(Agro_SQL_SaveDate(dataIniziale))
            strSQL.Append("AND IC.Data_Stipulazione <= ").AppendLine(Agro_SQL_SaveDate(dataFinale))



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function

    Public Function leggiFasiCod(listaFaseCod As List(Of Integer),
                               ByRef objParametriServer As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.MaterialeVivaistico.leggiDatiConferimentoGenerale()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New StringBuilder
        Dim DT As DataTable
        Dim StrQuery_Output As String = ""

        Try

            If listaFaseCod.Count = 0 Then
                Return DT
            End If

            strSQL.AppendLine("Select fase_cod, tipo_trasporto from Imprese_Contratto_Fasi")
            strSQL.Append("WHERE Fase_Cod IN ").Append(listaFaseCod.ToQueryInExpression)



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function
End Class
