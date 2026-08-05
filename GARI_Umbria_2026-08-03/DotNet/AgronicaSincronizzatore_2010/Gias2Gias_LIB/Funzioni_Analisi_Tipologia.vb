Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreAnagrafeDAL


Partial Public Class Funzioni

    Public Sub Elabora_Analisi_tipologia( _
                                        ByVal objOpzioni As clsOpzioni, _
                                        ByRef Log_Import As StringBuilder, _
                                        ByRef Log_Errori As StringBuilder, _
                                        ByRef Log_Riepilogo As StringBuilder _
                                )

        Const nomeFunzione As String = "Elabora_Analisi_tipologia"



        Try


            Dim dtAnalisi_tipologia As DataTable
            Dim dtAnalisi_tipologia_Dettagli As DataTable
            Dim dtAnalisi_tipologia_Laboratori As DataTable


            Dim leggiAnalisi_tipologia As New Analisi_Tipologia_R
            Dim leggiAnalisi_tipologia_Dettagli As New Analisi_Tipologia_Dettagli_R
            Dim leggiAnalisi_tipologia_Laboratori As New Analisi_Tipologia_Laboratori_R

            Dim scriviAnalisi_tipologia As New Analisi_Tipologia_W
            Dim scriviAnalisi_tipologia_Dettagli As New Analisi_Tipologia_Dettagli_W
            Dim scriviAnalisi_tipologia_Laboratori As New Analisi_Tipologia_Laboratori_W

            Dim SeqTab As New AgronicaCoreDataProvider.Agro_Sequenze

            dtAnalisi_tipologia = leggiAnalisi_tipologia.Leggi(0, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)

            For Each iCurrdtAnalisi_tipologia In dtAnalisi_tipologia.Rows

                Dim lICurrdtAnalisi_tipologia_cod As Integer = iCurrdtAnalisi_tipologia("Analisi_tipologia_cod")

                Dim NuovoAnalisiTipologia As Integer
                'NuovoAnalisiTipologia = SeqTab.Agronica_SequenzaTabelle_NuovoID(
                '    "analisi_tipologia",
                '    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                '    )

                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                NuovoAnalisiTipologia = SeqTab.NuovoId_Tabella("analisi_tipologia", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                FunzioniGLOBAL.RecodeAnalisiTipologia.Add(New recode_Analisi_Tipologia With {.FROM_AnalisiTipologia_cod = lICurrdtAnalisi_tipologia_cod, .TO_AnalisiTipologia_cod = NuovoAnalisiTipologia})

                Dim lICurrdtAnalisi_tipologia As String = ""
                If Not IsDBNull(iCurrdtAnalisi_tipologia("Numero_Determinazioni")) Then
                    lICurrdtAnalisi_tipologia = iCurrdtAnalisi_tipologia("Numero_Determinazioni")
                End If



                scriviAnalisi_tipologia.Scrivi( _
                    NuovoAnalisiTipologia _
                    , iCurrdtAnalisi_tipologia("Analisi_Tipologia_Des") _
                    , iCurrdtAnalisi_tipologia("Analisi_Tipologia_Des_long") _
                    , iCurrdtAnalisi_tipologia("Analisi_Tipologia_Tipo") _
                    , lICurrdtAnalisi_tipologia _
                    , iCurrdtAnalisi_tipologia("Validita_inizio") _
                    , iCurrdtAnalisi_tipologia("Validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , iCurrdtAnalisi_tipologia("data_creazione") _
                    , iCurrdtAnalisi_tipologia("data_modifica") _
                    , iCurrdtAnalisi_tipologia("username_creazione") _
                    , iCurrdtAnalisi_tipologia("username_modifica") _
                )




                dtAnalisi_tipologia_Dettagli = leggiAnalisi_tipologia_Dettagli.Leggi(lICurrdtAnalisi_tipologia_cod, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)

                For Each iCurrdtAnalisi_tipologia_Dettagli In dtAnalisi_tipologia_Dettagli.Rows
                    scriviAnalisi_tipologia_Dettagli.Scrivi( _
                        NuovoAnalisiTipologia _
                        , iCurrdtAnalisi_tipologia_Dettagli("Analisi_Parametro_cod") _
                        , iCurrdtAnalisi_tipologia_Dettagli("udm_cod") _
                        , iCurrdtAnalisi_tipologia_Dettagli("ldm") _
                        , iCurrdtAnalisi_tipologia_Dettagli("ordinamento") _
                        , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                        , iCurrdtAnalisi_tipologia_Dettagli("data_creazione") _
                        , iCurrdtAnalisi_tipologia_Dettagli("data_modifica") _
                        , iCurrdtAnalisi_tipologia_Dettagli("username_creazione") _
                        , iCurrdtAnalisi_tipologia_Dettagli("username_modifica") _
                    )


                Next


                dtAnalisi_tipologia_Laboratori = leggiAnalisi_tipologia_Laboratori.Leggi(lICurrdtAnalisi_tipologia_cod, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)

                For Each iCurrdtAnalisi_tipologia_Laboratori In dtAnalisi_tipologia_Laboratori.Rows
                    Dim oldCodRisum As Integer = iCurrdtAnalisi_tipologia_Laboratori("Cod_Risum")

                    Dim newCodRisum As Integer = _
                        ( _
                            From r In FunzioniGLOBAL.Contatti _
                            Where r.From_Cod_RisUm = oldCodRisum _
                            Select r.To_Cod_RisUm _
                        ).FirstOrDefault

                    scriviAnalisi_tipologia_Laboratori.Scrivi( _
                        newCodRisum _
                        , NuovoAnalisiTipologia _
                        , iCurrdtAnalisi_tipologia_Laboratori("validita_inizio") _
                        , iCurrdtAnalisi_tipologia_Laboratori("validita_fine") _
                        , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                        , iCurrdtAnalisi_tipologia_Laboratori("data_creazione") _
                        , iCurrdtAnalisi_tipologia_Laboratori("data_modifica") _
                        , iCurrdtAnalisi_tipologia_Laboratori("username_creazione") _
                        , iCurrdtAnalisi_tipologia_Laboratori("username_modifica") _
                    )


                Next

            Next




        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try

    End Sub


End Class
