Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider

Imports System.Text
Imports AgronicaCoreContabDAL
Imports AgronicaCoreAnagrafeDAL



Partial Public Class Funzioni

    Public Sub Elabora_Liquidita_Salva( _
                                    ByVal objOpzioni As clsOpzioni, _
                                    ByRef Log_Import As StringBuilder, _
                                    ByRef Log_Errori As StringBuilder, _
                                    ByRef Log_Riepilogo As StringBuilder, _
                                    ByVal Piva_Origine As String, _
                                    ByVal Piva_Destinazione As String _
                            )


        Const nomeFunzione As String = "Elabora_Liquidita_Salva"

        Try

            Dim dtLquidita As DataTable
            Dim dtPagamentiCausali As DataTable
            Dim dtistituti_credito As DataTable


            Dim LeggiLiquidita As New Liquidita_R
            Dim LeggiPagamentiCausali As New Pagamenti_Causali_R
            Dim leggiistituti_credito As New Ist_Credito_R



            Dim scriviLiquidita As New Liquidita_W
            Dim scriviPagamenti_Causali As New Pagamenti_Causali_W
            Dim scriviistituti_credito As New ist_credito_W



            If Piva_Origine = "" Then
                dtPagamentiCausali = LeggiPagamentiCausali.Leggi( _
                    "", _
                    0, _
                    "", _
                    "", _
                    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )
            Else
                dtPagamentiCausali = New DataTable
            End If


            If Piva_Origine = "" Then
                dtistituti_credito = leggiistituti_credito.Leggi(0, 0, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                dtistituti_credito = New DataTable
            End If

            dtLquidita = LeggiLiquidita.Leggi( _
                Piva_Origine, _
                "", _
                "", _
                objOpzioni.objParametri_Server_GIAS_ORIGINE _
            )

            Dim seqTabelle As New Agro_Sequenze

            For Each iCurrdtistituti_credito In dtistituti_credito.Rows

                ' Dim iNuovoID As Integer =
                'seqTabelle.Agronica_SequenzaTabelle_NuovoID(
                '                "ist_credito",
                '                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                '            )

                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                Dim iNuovoID As Integer = seqTabelle.NuovoId_Tabella("ist_credito", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                scriviistituti_credito.Scrivi( _
                    0 _
                    , iNuovoID _
                    , iCurrdtistituti_credito("Istituto_Des") _
                    , iCurrdtistituti_credito("Filiale") _
                    , iCurrdtistituti_credito("Per_Risorsa") _
                    , iCurrdtistituti_credito("validita_inizio") _
                    , iCurrdtistituti_credito("validita_fine") _
                    , objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE _
                    , iCurrdtistituti_credito("data_creazione") _
                    , iCurrdtistituti_credito("data_modifica") _
                    , iCurrdtistituti_credito("username_creazione") _
                    , iCurrdtistituti_credito("username_modifica") _
                )

            Next


            For Each iPagamenti_Causali In dtPagamentiCausali.Rows

                'Dim iNuovoID As Integer =
                'seqTabelle.Agronica_SequenzaTabelle_NuovoID(
                '                "pagamenti_causali",
                '                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                '            )

                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                Dim iNuovoID As Integer = seqTabelle.NuovoId_Tabella("pagamenti_causali", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                scriviPagamenti_Causali.Scrivi( _
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                        , -iNuovoID _
                    , iPagamenti_Causali("Cau_Pagamento_Sigla") _
                    , iPagamenti_Causali("Cau_Pagamento_Des") _
                    , iPagamenti_Causali("Giorni_Scadenza") _
                    , iPagamenti_Causali("Opzione") _
                    , iPagamenti_Causali("Cau_Risorsa") _
                    , iPagamenti_Causali("Tipo") _
                    , iPagamenti_Causali("validita_inizio") _
                    , iPagamenti_Causali("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , iPagamenti_Causali("data_creazione") _
                    , iPagamenti_Causali("data_modifica") _
                    , iPagamenti_Causali("username_creazione") _
                    , iPagamenti_Causali("username_modifica") _
                )

            Next



            For Each iLiquidita In dtLquidita.Rows
                scriviLiquidita.Scrivi( _
                    Piva_Destinazione, _
                    0, _
                    iLiquidita("Cod_Liquidita"), _
                    iLiquidita("Cod_Contatto"), _
                    iLiquidita("Riferimento"), _
                    iLiquidita("Cau_Risorsa"), _
                    iLiquidita("Cod_Istituto"), _
                    iLiquidita("Numero"), _
                    iLiquidita("Abi"), _
                    iLiquidita("Cab"), _
                    iLiquidita("Cin"), _
                    iLiquidita("Cifre_Controllo"), _
                    iLiquidita("Nazione"), _
                    iLiquidita("Bic"), _
                    iLiquidita("Interbancario"), _
                    iLiquidita("Saldo_Attuale"), _
                    iLiquidita("Saldo_Iniziale"), _
                    iLiquidita("Avviso"), _
                    iLiquidita("Importo_Avviso"), _
                    iLiquidita("Note"), _
                    iLiquidita("Rilevamento"), _
                    iLiquidita("Data_Rilevamento"), _
                    iLiquidita("Offset"), _
                    iLiquidita("ChkDefault"), _
                    iLiquidita("ChkAbilitazione"), _
                    iLiquidita("Validita_Inizio"), _
                    iLiquidita("Validita_Fine"), _
                    "", _
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
            )

            Next



            'aggiustamento di sequenza_tabelle
            Dim stb As New StringBuilder
            Dim exer As New AgronicaCoreDataProvider.DataProvider

            stb.Length = 0
            stb.Append(" update Sequenza_tabelle  " & vbCrLf)
            stb.Append(" set Ultimo_Valore = coalesce((select max(Cod_Liquidita)+1 from liquidita), 1) " & vbCrLf)
            stb.Append(" where nome_Tabella = 'liquidita' " & vbCrLf)
            exer.EseguiQuery_Scrittura(objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, stb.ToString, "")


            stb.Length = 0
            stb.Append(" update Sequenza_tabelle  " & vbCrLf)
            stb.Append(" set Ultimo_Valore = coalesce((select max(cod_istituto)+1 from ist_credito), 1) " & vbCrLf)
            stb.Append(" where nome_Tabella = 'ist_credito' " & vbCrLf)
            exer.EseguiQuery_Scrittura(objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, stb.ToString, "")


        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try


    End Sub


End Class