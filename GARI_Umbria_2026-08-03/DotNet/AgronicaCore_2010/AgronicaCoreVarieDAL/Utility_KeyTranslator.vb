Imports System.Data.Common
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Utility_KeyTranslator_W
    Inherits AgronicaCoreDataProvider.DataProvider




    '========================================================================================
    'Public Function Modifica_CodContatto(
    '                        ByVal NomeTabella As String,
    '                        ByVal Piva As String,
    '                        ByVal CodContatto_OLD As String,
    '                        ByVal CodContatto_NEW As String,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Modifica_CodContatto()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    '------------------------------

    '    Try

    '        If NomeTabella = "" Then
    '            Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
    '        End If

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        If CodContatto_OLD = "" Then
    '            Throw New Exception("Parametro non corretto nella query (CodContatto_OLD obbligatorio)")
    '        End If

    '        If CodContatto_NEW = "" Then
    '            Throw New Exception("Parametro non corretto nella query (CodContatto_NEW obbligatorio)")
    '        End If


    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" UPDATE " & NomeTabella & " ")
    '        StrSQL.Append(" SET     Cod_Contatto = '" & Agro_SQL_SaveText(CodContatto_NEW) & "' ")
    '        StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "'")
    '        StrSQL.Append(" AND     Cod_Contatto = '" & Agro_SQL_SaveText(CodContatto_OLD) & "' ")

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '    Catch ex As Exception

    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

    '    End Try

    '    Return xRisp


    'End Function



    Public Function Modifica_CodContatto(
                            ByVal NomeTabella As String,
                            ByVal Piva As String,
                            ByVal CodContatto_OLD As String,
                            ByVal CodContatto_NEW As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Modifica_CodContatto()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If CodContatto_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (CodContatto_OLD obbligatorio)")
            End If

            If CodContatto_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (CodContatto_NEW obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE " & NomeTabella & " ")
            StrSQL.Append(" SET     Cod_Contatto = '" & Agro_SQL_SaveText(CodContatto_NEW) & "' ")
            'StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            'StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND     Cod_Contatto = '" & Agro_SQL_SaveText(CodContatto_OLD) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    Public Function Modifica_CodContatto(
                            ByVal NomeTabella As String,
                            ByVal Piva As String,
                            ByVal CodContatto_OLD As String,
                            ByVal CodContatto_NEW As String,
                                ByVal Flag_DataModifica As Boolean,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Modifica_CodContatto()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If CodContatto_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (CodContatto_OLD obbligatorio)")
            End If

            If CodContatto_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (CodContatto_NEW obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE " & NomeTabella & " ")
            StrSQL.Append(" SET     Cod_Contatto = '" & Agro_SQL_SaveText(CodContatto_NEW) & "' ")

            If Flag_DataModifica Then
                StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
                StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            End If

            StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND     Cod_Contatto = '" & Agro_SQL_SaveText(CodContatto_OLD) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function


    '##############################################################################
    Public Function Modifica_Piva_TabellaGenerica(ByVal NomeTabella As String,
                                                  ByVal NomeColonnaPiva As String,
                                                    ByVal Piva_OLD As String,
                                                    ByVal Piva_NEW As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Utility_KeyTranslator_W.Modifica_Piva_TabellaGenerica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If

            If NomeColonnaPiva = "user" Then
                NomeColonnaPiva = String.Format("[{0}]", NomeColonnaPiva)
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE " & NomeTabella & " ")
            StrSQL.Append(" SET " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(Piva_NEW) & "' ")

            'Aggiungo questa clausola solo se la tabella ha i campi data_modifica e username_modifica e se
            'il nome del campo da modificare è diverso da username_modifica.
            'If Flag_DataModifica AndAlso NomeColonnaPiva <> "username_modifica" Then
            '    StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            '    StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            'End If

            StrSQL.Append(" WHERE " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(Piva_OLD) & "'")

            'If NomeTabella <> "Zoo_FarmacixMalattie" Then
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            'End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] [" & NomeTabella & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function


    '##############################################################################
    Public Function Modifica_Piva_GerarchiaImprese(ByVal Piva_OLD As String,
                                                    ByVal Piva_NEW As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Modifica_Piva_GerarchiaImprese()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE  GerarchiaImprese ")
            StrSQL.Append(" SET     Padre = '" & Agro_SQL_SaveText(Piva_NEW) & "' ")
            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE   Padre = '" & Agro_SQL_SaveText(Piva_OLD) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp = True Then

                '---------------------------------------------
                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  GerarchiaImprese ")
                StrSQL.Append(" SET     Figlio = '" & Agro_SQL_SaveText(Piva_NEW) & "' ")
                StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
                StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
                StrSQL.Append(" WHERE   Figlio = '" & Agro_SQL_SaveText(Piva_OLD) & "'")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function


    '##############################################################################
    Public Function Modifica_Piva_Mov_Dettagli_Riferimenti(ByVal Piva_OLD As String,
                                                            ByVal Piva_NEW As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Modifica_Piva_Mov_Dettagli_Riferimenti()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE  Mov_Dettagli_Riferimenti ")
            StrSQL.Append(" SET     Piva = '" & Agro_SQL_SaveText(Piva_NEW) & "' ")
            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva_OLD) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp = True Then

                '---------------------------------------------
                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Mov_Dettagli_Riferimenti ")
                StrSQL.Append(" SET     Piva_Rif = '" & Agro_SQL_SaveText(Piva_NEW) & "' ")
                StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
                StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
                StrSQL.Append(" WHERE   Piva_Rif = '" & Agro_SQL_SaveText(Piva_OLD) & "'")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function


    '##############################################################################
    Public Function Modifica_Piva_Utenti_Dettagli(ByVal Piva_NEW As String,
                                                        ByVal Flag_Cambia_CF As Boolean,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Modifica_Piva_UtentiDettagli()"
        Dim messaggioErrore As String = ""
        Dim Flag_Risp As Boolean
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If


            Dim objUtentiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W

            If Flag_Cambia_CF Then

                Flag_Risp = objUtentiDettagli.Modifica_Parametrizzata(objParametri_Utenti.SuperUserUsername,
                                                        "CodFisc",
                                                        Piva_NEW,
                                                        "",
                                                        objParametri_Utenti,
                                                        True)

            End If


            Flag_Risp = objUtentiDettagli.Modifica_Parametrizzata(objParametri_Utenti.SuperUserUsername,
                                                          "Piva",
                                                          Piva_NEW,
                                                          "",
                                                          objParametri_Utenti,
                                                           True)


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function



    '##############################################################################
    Public Function Modifica_Piva_TabellaGenerica_SoloDataModifica(ByVal NomeTabella As String,
                                                  ByVal NomeColonnaPiva As String,
                                                    ByVal Piva_OLD As String,
                                                    ByVal Piva_NEW As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Modifica_Piva_TabellaGenerica_SoloDataModifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            If NomeColonnaPiva = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeColonnaPiva obbligatorio)")
            End If

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If

            If NomeColonnaPiva = "user" Then
                NomeColonnaPiva = String.Format("[{0}]", NomeColonnaPiva)
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE " & NomeTabella & " ")
            StrSQL.Append(" SET " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(Piva_NEW) & "' ")
            StrSQL.Append("   , Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" WHERE " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(Piva_OLD) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] [" & NomeTabella & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function


    '##############################################################################
    Public Function Modifica_Piva_TabellaGenerica_SoloUsernameModifica(ByVal NomeTabella As String,
                                                  ByVal NomeColonnaPiva As String,
                                                    ByVal Piva_OLD As String,
                                                    ByVal Piva_NEW As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Modifica_Piva_TabellaGenerica_SoloUsernameModifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            If NomeColonnaPiva = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeColonnaPiva obbligatorio)")
            End If

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If

            If NomeColonnaPiva = "user" Then
                NomeColonnaPiva = String.Format("[{0}]", NomeColonnaPiva)
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE " & NomeTabella & " ")
            StrSQL.Append(" SET " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(Piva_NEW) & "' ")
            StrSQL.Append("   , Username_Modifica        =  '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(Piva_OLD) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] [" & NomeTabella & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function


    '##############################################################################
    Public Function Replace_Piva_KendoCache(ByVal Piva_OLD As String,
                                            ByVal Piva_NEW As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Replace_Piva_KendoCache()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0


            'set strkendo = replace( strkendo, '"piva": "00000000001"','"piva": "00000000004"')

            StrSQL.Append(" UPDATE KendoCache ")
            StrSQL.Append(" SET strkendo =  REPLACE( strkendo, '""piva"": """ & Piva_OLD & """' , '""piva"": """ & Piva_NEW & """') ")
            StrSQL.Append(" , Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" , UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] [KendoCache] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    '##############################################################################
    Public Function Replace_Piva_AuditRiferimentoCod(ByVal Piva_OLD As String,
                                            ByVal Piva_NEW As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Replace_Piva_AuditRiferimentoCod()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        'nella tabella Audit viene salvato nel campo Riferimento_Cod la chiave relativa al campo (podere) 
        'a cui fa riferimento la checklist.
        'Esempio: 3_00081910390_142213121_142213121
        'Nell 'informazione memorizzata è presente anche la piva, 
        'non vorrei che avendola cambiata dava errore perché non trovava più il riferimento al podere.

        Try

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Audit ")
            StrSQL.Append(" SET Riferimento_Cod =  REPLACE( Riferimento_Cod, '" & Agro_SQL_SaveText(Piva_OLD) & "', '" & Agro_SQL_SaveText(Piva_NEW) & "' ) ")
            StrSQL.Append(" , Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" , UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] [KendoCache] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    '##############################################################################
    Public Function Replace_Piva_AlertEntitaxIndici(ByVal Piva_OLD As String,
                                            ByVal Piva_NEW As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Replace_Piva_AlertEntitaxIndici()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        'nella tabella Audit viene salvato nel campo Riferimento_Cod la chiave relativa al campo (podere) 
        'a cui fa riferimento la checklist.
        'Esempio: 3_00081910390_142213121_142213121
        'Nell 'informazione memorizzata è presente anche la piva, 
        'non vorrei che avendola cambiata dava errore perché non trovava più il riferimento al podere.

        Try

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_EntitaxIndici ")
            StrSQL.Append(" SET Elenco_Val =  REPLACE( Elenco_Val, '" & Agro_SQL_SaveText(Piva_OLD) & "', '" & Agro_SQL_SaveText(Piva_NEW) & "' ) ")
            StrSQL.Append(" , Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" , UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] [KendoCache] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    '##############################################################################
    'NomeColonnaCheContienePiva: deve essere descrizione_1 o descrizione_2
    'sulle quali va fatto il convert da text a varchar
    Public Function Replace_Piva_UtentiProfili(ByVal NomeColonnaCheContienePiva As String,
                                                    ByVal Piva_OLD As String,
                                                    ByVal Piva_NEW As String,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Replace_Piva_UtentiProfili()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If NomeColonnaCheContienePiva = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeColonnaCheContienePiva obbligatorio)")
            End If

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If

            If NomeColonnaCheContienePiva = "user" Then
                NomeColonnaCheContienePiva = String.Format("[{0}]", NomeColonnaCheContienePiva)
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Utenti_Profili ")
            StrSQL.Append(" SET " & NomeColonnaCheContienePiva & " =  REPLACE( CONVERT(varchar(MAX)," & NomeColonnaCheContienePiva & "), '" & Agro_SQL_SaveText(Piva_OLD) & "' , '" & Agro_SQL_SaveText(Piva_NEW) & "') ")
            StrSQL.Append(" , Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" , UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri_Utenti.UsernameOperazione) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Utenti, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] [Utenti_Profili] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    '#############################################################################
    'usa il Flag_DataModifica
    Public Function Modifica_CodContatto_TabellaGenerica(
                            ByVal NomeTabella As String,
                            ByVal NomeColonna_CodContatto As String,
                            ByVal Piva As String,
                            ByVal CodContatto_OLD As String,
                            ByVal CodContatto_NEW As String,
                                ByVal Flag_DataModifica As Boolean,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Modifica_CodContatto_TabellaGenerica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            If NomeColonna_CodContatto = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeColonna_CodContatto obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If CodContatto_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (CodContatto_OLD obbligatorio)")
            End If

            If CodContatto_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (CodContatto_NEW obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE " & NomeTabella & " ")
            StrSQL.Append(" SET " & NomeColonna_CodContatto & " = '" & CodContatto_NEW & "' ")

            If Flag_DataModifica Then
                StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
                StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            End If

            StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND " & NomeColonna_CodContatto & " = '" & Agro_SQL_SaveText(CodContatto_OLD) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    '##############################################################################
    Public Function Modifica_Riferimento_Liquidita(ByVal Piva As String,
                                                    ByVal Cod_contatto_OLD As String,
                                                    ByVal Cod_contatto_NEW As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Modifica_Riferimento_Liquidita()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Cod_contatto_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_contatto_OLD obbligatorio)")
            End If

            If Cod_contatto_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_contatto_NEW obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE  Liquidita ")
            StrSQL.Append(" SET     Riferimento = '" & Agro_SQL_SaveText(Cod_contatto_NEW) & "' ")
            'questa tabella non ha username_modifica e data_modifica
            StrSQL.Append(" WHERE   Riferimento = '" & Agro_SQL_SaveText(Cod_contatto_OLD) & "'")
            StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    '##############################################################################
    Public Function Modifica_CodContattoxLABCQ(ByVal Cod_contatto_OLD As String,
                                                ByVal Cod_contatto_NEW As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Modifica_CodContattoxLABCQ()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Cod_contatto_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_contatto_OLD obbligatorio)")
            End If

            If Cod_contatto_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_contatto_NEW obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE  LCQ_ParametriValori ")
            StrSQL.Append(" SET     [Utente] = '" & Agro_SQL_SaveText(Cod_contatto_NEW) & "' ")
            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE   [Utente] = '" & Agro_SQL_SaveText(Cod_contatto_OLD) & "'")
            StrSQL.Append(" AND     PivaSuperUser = '" & objParametri.PivaSuperUser & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    '##############################################################################
    Public Function Modifica_CodContatto_OrganismoReferente(ByVal Cod_contatto_OLD As String,
                                                            ByVal Cod_contatto_NEW As String,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Modifica_CodContatto_OrganismoReferente()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Cod_contatto_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_contatto_OLD obbligatorio)")
            End If

            If Cod_contatto_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_contatto_NEW obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE  Reg_Impianti_Codici ")
            StrSQL.Append(" SET     val_cod = '" & Agro_SQL_SaveText(Cod_contatto_NEW) & "' ")
            StrSQL.Append("          ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   val_cod = '" & Agro_SQL_SaveText(Cod_contatto_OLD) & "'")
            StrSQL.Append(" AND     id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Organismo_Referente) & "")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    '##############################################################################
    Public Function Modifica_Piva_TabellaGenerica(ByVal NomeTabella As String,
                                                  ByVal NomeColonnaPiva As String,
                                                    ByVal Piva_OLD As String,
                                                    ByVal Piva_NEW As String,
                                                    ByVal Flag_DataModifica As Boolean,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal xFiltroAggiuntivo As String = ""
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Utility_KeyTranslator_W.Modifica_Piva_TabellaGenerica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            If NomeColonnaPiva = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeColonnaPiva obbligatorio)")
            End If

            If Piva_OLD = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_OLD obbligatorio)")
            End If

            If Piva_NEW = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_NEW obbligatorio)")
            End If

            If NomeColonnaPiva = "user" Then
                NomeColonnaPiva = String.Format("[{0}]", NomeColonnaPiva)
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE " & NomeTabella & " ")
            StrSQL.Append(" SET " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(Piva_NEW) & "' ")

            'Aggiungo questa clausola solo se la tabella ha i campi data_modifica e username_modifica
            If Flag_DataModifica = True Then
                StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
                StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            End If

            StrSQL.Append(" WHERE " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(Piva_OLD) & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] [" & NomeTabella & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

End Class
