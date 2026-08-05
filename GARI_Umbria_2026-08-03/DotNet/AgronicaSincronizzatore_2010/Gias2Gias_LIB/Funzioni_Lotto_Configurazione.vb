
Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider

Imports System.Text
Imports AgronicaCoreAnagrafeDAL



Partial Public Class Funzioni
    Public Sub Elabora_Lotto_Configurazione_Salva( _
                                   ByVal objOpzioni As clsOpzioni, _
                                   ByRef Log_Import As StringBuilder, _
                                   ByRef Log_Errori As StringBuilder, _
                                   ByRef Log_Riepilogo As StringBuilder, _
                                   ByVal Piva_Origine As String, _
                                   ByVal Piva_Destinazione As String _
                           )
        Const nomeFunzione As String = "Elabora_Liquidita_Salva"

        Try

            Dim dtLotto_configurazione As DataTable
            Dim dtLotto_configurazione_alias As DataTable

            Dim leggiLotto_configurazione As New Lotto_configurazione_R
            Dim leggiLotto_configurazione_alias As New Lotto_configurazione_alias_R

            Dim scriviLotto_configurazione As New Lotto_configurazione_W
            Dim scriviLotto_configurazione_alias As New Lotto_Configurazione_Alias_W

            Dim seqTabelle As New Agro_Sequenze

            dtLotto_configurazione = leggiLotto_configurazione.Leggi(Piva_Origine, 0, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)

            For Each iCurrdtLotto_configurazione In dtLotto_configurazione.Rows

                Dim lcurLottoCod As Integer = iCurrdtLotto_configurazione("Lotto_cod")

                'Dim iNuovoID As Integer =
                '    seqTabelle.Agronica_SequenzaTabelle_NuovoID(
                '             "lotto_configurazione",
                '             objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                '         )

                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                Dim iNuovoID As Integer = seqTabelle.NuovoId_Tabella("lotto_configurazione", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                scriviLotto_configurazione.Scrivi( _
                        Piva_Destinazione _
                    , iCurrdtLotto_configurazione("Elem_Cod") _
                    , iNuovoID _
                    , iCurrdtLotto_configurazione("Lotto_Des") _
                    , iCurrdtLotto_configurazione("Cifra_Start") _
                    , iCurrdtLotto_configurazione("Cifra_End") _
                    , iCurrdtLotto_configurazione("Lotto_Des_Estesa") _
                    , iCurrdtLotto_configurazione("Tipo") _
                    , iCurrdtLotto_configurazione("validita_inizio") _
                    , iCurrdtLotto_configurazione("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , iCurrdtLotto_configurazione("data_creazione") _
                    , iCurrdtLotto_configurazione("data_modifica") _
                    , iCurrdtLotto_configurazione("username_creazione") _
                    , iCurrdtLotto_configurazione("username_modifica") _
                )


                dtLotto_configurazione_alias = leggiLotto_configurazione_alias.Leggi(Piva_Origine, 0, lcurLottoCod, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
                For Each iCurrdtLotto_configurazione_alias In dtLotto_configurazione_alias.Rows


                    scriviLotto_configurazione_alias.Scrivi( _
                          Piva_Destinazione _
                        , iCurrdtLotto_configurazione_alias("Elem_Cod") _
                        , iNuovoID _
                        , iCurrdtLotto_configurazione_alias("Lotto_Val") _
                        , iCurrdtLotto_configurazione_alias("Lotto_Alias") _
                        , iCurrdtLotto_configurazione_alias("validita_inizio") _
                        , iCurrdtLotto_configurazione_alias("validita_fine") _
                        , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                        , iCurrdtLotto_configurazione_alias("data_creazione") _
                        , iCurrdtLotto_configurazione_alias("data_modifica") _
                        , iCurrdtLotto_configurazione_alias("username_creazione") _
                        , iCurrdtLotto_configurazione_alias("username_modifica") _
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
