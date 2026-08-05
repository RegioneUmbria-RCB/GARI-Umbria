Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Alert_Entita

    Public Sub New()
        Sa_Cod = 0
        Appezza = 0
        Campo_Cod = 0
        Id_Imp = 0
        PROV = String.Empty
        COM = String.Empty
        SEZIONE = String.Empty
        FOGLIO = 0
        NUMERO = 0
        SUBALTERNO = String.Empty
        Programmazione_Entita_Cod = 0
        ID_Agenda = 0
        Ricetta_Operazione_cod = 0
        Id_ImpresexParticelle = 0
        analisi_campione_cod = 0
        Cod_Contatto = String.Empty
        Analisi_Testata_Cod = 0
        PC_Testata_Cod = 0
        PUA_Cod = 0
        Mac_Cod = 0
        ChkDocumento = 0
        Richiesta_Cod = 0
        Id_Schema_Template = 0
        ChkStorico = 0
    End Sub

    Public Allegati_Documenti_Cod As Integer

    Public PivaSuperUser As String
    Public ID_Alert_Entita As Integer
    Public TipoEntita_Cod As Integer
    Public Piva As String
    Public Sa_Cod As Integer
    Public Appezza As Integer
    Public Campo_Cod As Integer
    Public Id_Imp As Integer
    Public PROV As String
    Public COM As String
    Public SEZIONE As String
    Public FOGLIO As Integer
    Public NUMERO As Integer
    Public SUBALTERNO As String
    Public Programmazione_Entita_Cod As Integer
    Public ID_Agenda As Integer
    Public Ricetta_Operazione_cod As Integer
    Public Id_ImpresexParticelle As Integer
    Public analisi_campione_cod As Integer
    Public Cod_Contatto As String
    Public Analisi_Testata_Cod As Integer
    Public PC_Testata_Cod As Integer    ' Piano concimazione
    Public PUA_Cod As Integer
    Public Mac_Cod As Integer
    Public ChkDocumento As Integer
    Public Richiesta_Cod As Integer
    Public Id_Schema_Template As Integer
    Public ChkStorico As Integer

End Class


Public Class Alert_Entita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Alert_Entita_OBJ As Alert_Entita,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Alert_Entita ")
            StrSQL.Append("                   ( PivaSuperUser, ID_Alert_Entita, TipoEntita_Cod , Allegati_Documenti_Cod, ")
            StrSQL.Append("                   Piva, Sa_Cod, Appezza, Campo_Cod, Id_Imp, ")
            StrSQL.Append("                   PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, Programmazione_Entita_Cod, ID_Agenda, Ricetta_Operazione_cod, analisi_campione_cod, Cod_Contatto, ")
            StrSQL.Append("                   Analisi_Testata_Cod, PC_Testata_Cod, PUA_Cod, Mac_Cod, ChkDocumento, Richiesta_Cod, Id_Schema_Template, ChkStorico, ")
            StrSQL.Append("                   Id_ImpresexParticelle ")

            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(Alert_Entita_OBJ.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.ID_Alert_Entita) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.TipoEntita_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Allegati_Documenti_Cod) & "  ")


            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Alert_Entita_OBJ.Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Campo_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Id_Imp) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Alert_Entita_OBJ.PROV) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Alert_Entita_OBJ.COM) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Alert_Entita_OBJ.SEZIONE) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.FOGLIO) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.NUMERO) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Alert_Entita_OBJ.SUBALTERNO) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Programmazione_Entita_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.ID_Agenda) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Ricetta_Operazione_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.analisi_campione_cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Alert_Entita_OBJ.Cod_Contatto) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Analisi_Testata_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.PC_Testata_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.PUA_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Mac_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.ChkDocumento) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Richiesta_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Id_Schema_Template) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Alert_Entita_OBJ.ChkStorico) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Alert_Entita_OBJ.Id_ImpresexParticelle) & " ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" )")
            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    Public Function Modifica(ByVal Id_Alert_Entita As Integer,
                             ByVal Allegati_Documenti_Cod As Integer,
                             ByVal Alert_Entita_OBJ As Alert_Entita,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Latitudine = 0
        '   Longitudine = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            StrSQL.Append(" UPDATE Alert_Entita SET ")



            StrSQL.Append("        TipoEntita_Cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.TipoEntita_Cod) & "  ")
            StrSQL.Append("         , Piva = '" & Agro_SQL_SaveText(Alert_Entita_OBJ.Piva) & "' ")
            StrSQL.Append("         , Sa_Cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Sa_Cod) & "  ")
            StrSQL.Append("         , Appezza = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Appezza) & "  ")
            StrSQL.Append("         , Campo_Cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Campo_Cod) & "  ")
            StrSQL.Append("         , Id_Imp = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Id_Imp) & "  ")
            StrSQL.Append("         , PROV = '" & Agro_SQL_SaveText(Alert_Entita_OBJ.PROV) & "' ")
            StrSQL.Append("         , COM = '" & Agro_SQL_SaveText(Alert_Entita_OBJ.COM) & "' ")
            StrSQL.Append("         , SEZIONE= '" & Agro_SQL_SaveText(Alert_Entita_OBJ.SEZIONE) & "' ")
            StrSQL.Append("         , FOGLIO = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.FOGLIO) & "  ")
            StrSQL.Append("         , NUMERO = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.NUMERO) & "  ")
            StrSQL.Append("         , SUBALTERNO= '" & Agro_SQL_SaveText(Alert_Entita_OBJ.SUBALTERNO) & "' ")
            StrSQL.Append("         , Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Programmazione_Entita_Cod) & "  ")
            StrSQL.Append("         , ID_Agenda = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.ID_Agenda) & "  ")
            StrSQL.Append("         , Ricetta_Operazione_cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Ricetta_Operazione_cod) & "  ")
            StrSQL.Append("         , analisi_campione_cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.analisi_campione_cod) & "  ")
            StrSQL.Append("         , analisi_testata_cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Analisi_Testata_Cod) & "  ")
            StrSQL.Append("         , pc_testata_cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.PC_Testata_Cod) & "  ")
            StrSQL.Append("         , pua_cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.PUA_Cod) & "  ")
            StrSQL.Append("         , mac_cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Mac_Cod) & "  ")
            StrSQL.Append("         , cod_contatto = '" & Agro_SQL_SaveText(Alert_Entita_OBJ.Cod_Contatto) & "'  ")

            StrSQL.Append("         , Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & "  ")
            StrSQL.Append("         , ChkDocumento = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.ChkDocumento) & "  ")
            StrSQL.Append("         , Richiesta_Cod = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Richiesta_Cod) & "  ")
            StrSQL.Append("         , Id_Schema_Template = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.Id_Schema_Template) & "  ")
            StrSQL.Append("         , ChkStorico = " & Agro_SQL_SaveNum(Alert_Entita_OBJ.ChkStorico) & "  ")

            StrSQL.Append(" WHERE   ID_Alert_Entita        =" & Agro_SQL_SaveNum(Id_Alert_Entita) & " ")
            StrSQL.Append(" AND     PivaSuperUser      = '" & Agro_SQL_SaveText(Alert_Entita_OBJ.PivaSuperUser) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp


    End Function

    Public Function Modifica_ChkDocumento(ByVal ChkDocumento As Integer,
                                          ByVal Id_Alert_Entita As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_W.Modifica_ChkDocumento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Append(" UPDATE Alert_Entita SET ")
            StrSQL.Append("        ChkDocumento = " & Agro_SQL_SaveNum(ChkDocumento) & " ")
            StrSQL.Append(" WHERE  ID_Alert_Entita        =" & Agro_SQL_SaveNum(Id_Alert_Entita) & " ")
            StrSQL.Append(" AND    PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Storicizza(ByVal Elenco As String,
                               ByVal ChkStorico As Integer,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Storicizza.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Latitudine = 0
        '   Longitudine = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            StrSQL.Append(" UPDATE Alert_Entita SET ")

            StrSQL.Append("  ChkStorico = " & Agro_SQL_SaveNum(ChkStorico) & "  ")

            StrSQL.Append(" WHERE   ID_Alert_Entita  In (" & Agro_SQL_Save_Clausola_IN(Elenco) & ") ")
            StrSQL.Append(" AND     PivaSuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp


    End Function



    Public Function Reset_Agenda(ByVal Ricetta_Operazione_Cod As Integer,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Reset_Agenda()"

        '====================================================================================
        'Parametri opzionali :
        '   Latitudine = 0
        '   Longitudine = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Append(" UPDATE Alert_Entita SET ")

            StrSQL.Append("  Id_Agenda = 0 ")

            StrSQL.Append(" WHERE   Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "  ")
            StrSQL.Append(" AND     PivaSuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp


    End Function


    Public Function Cancella(ByVal ID_Alert_Entita As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Alert_Entita ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Entita ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND ID_Alert_Entita = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            StrSQL.Append(" AND pivasuperuser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella_xDocumento(
                                ByVal Allegati_Documenti_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_W.Cancella_xDocumento()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Alert_Entita ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Entita ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            StrSQL.Append(" AND pivasuperuser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Alert_Entita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiOggetto(ByVal ID_Alert_Entita As Integer,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As Alert_Entita


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.LeggiOggetto()"
        Dim MessaggioErrore As String = ""

        Dim objE As New Alert_Entita

        Try

            Dim DT As DataTable = Leggi(ID_Alert_Entita, objParametri)

            objE.Allegati_Documenti_Cod = DT.Rows(0).Item("Allegati_Documenti_Cod")
            objE.PivaSuperUser = DT.Rows(0).Item("PivaSuperUser")
            objE.ID_Alert_Entita = DT.Rows(0).Item("ID_Alert_Entita")
            objE.TipoEntita_Cod = DT.Rows(0).Item("TipoEntita_Cod")
            objE.Piva = DT.Rows(0).Item("Piva")
            objE.Sa_Cod = DT.Rows(0).Item("Sa_Cod")
            objE.Appezza = DT.Rows(0).Item("Appezza")
            objE.Campo_Cod = DT.Rows(0).Item("Campo_Cod")
            objE.Id_Imp = DT.Rows(0).Item("Id_Imp")
            objE.PROV = DT.Rows(0).Item("PROV")
            objE.COM = DT.Rows(0).Item("COM")
            objE.SEZIONE = DT.Rows(0).Item("SEZIONE")
            objE.FOGLIO = DT.Rows(0).Item("FOGLIO")
            objE.NUMERO = DT.Rows(0).Item("NUMERO")
            objE.SUBALTERNO = DT.Rows(0).Item("SUBALTERNO")
            objE.Programmazione_Entita_Cod = DT.Rows(0).Item("Programmazione_Entita_Cod")
            objE.ID_Agenda = DT.Rows(0).Item("ID_Agenda")
            objE.Ricetta_Operazione_cod = DT.Rows(0).Item("Ricetta_Operazione_cod")
            objE.analisi_campione_cod = DT.Rows(0).Item("analisi_campione_cod")
            objE.Cod_Contatto = DT.Rows(0).Item("Cod_Contatto")
            objE.Analisi_Testata_Cod = DT.Rows(0).Item("Analisi_Testata_Cod")
            objE.PC_Testata_Cod = DT.Rows(0).Item("PC_Testata_Cod")
            objE.PUA_Cod = DT.Rows(0).Item("PUA_Cod")
            objE.Mac_Cod = DT.Rows(0).Item("Mac_Cod")
            objE.Id_ImpresexParticelle = DT.Rows(0).Item("Id_ImpreseXParticelle")

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return objE

    End Function


    '##############################################################################################
    Public Function Leggi(ByVal ID_Alert_Entita As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = ""
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM  Alert_Entita ")
            StrSQL.Append(" WHERE 1= 1 ")


            If ID_Alert_Entita <> 0 Then
                StrSQL.Append(" AND ID_Alert_Entita = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            End If


            StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################
    Public Function LeggiAlertEntita_AlertElenco(ByVal ID_Alert_Entita As Integer,
                                                 ByVal Piva As String,
                                                 ByVal Id_Agenda As Integer,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 Optional ByVal xFiltroAggiuntivo As String = "",
                                                 Optional ByVal xOrderBy As String = ""
                                                 ) As DataTable


        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.LeggiAlertEntita_AlertElenco()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT *  ")
            StrSQL.AppendLine(" FROM  Alert_Entita ")
            StrSQL.AppendLine(" INNER JOIN Alert_Elenco ON Alert_Entita.PivaSuperUser = Alert_Elenco.PivaSuperUser AND Alert_Entita.ID_Alert_Entita = Alert_Elenco.ID_Alert_Entita ")
            StrSQL.AppendLine(" WHERE Alert_Entita.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_Alert_Entita <> 0 Then
                StrSQL.AppendLine(" AND Alert_Entita.ID_Alert_Entita = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND Alert_Entita.ID_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Alert_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Alert_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_con_documenti(ByVal ID_Elenco As Integer,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.Leggi_con_documenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Alert_Entita.* ," + vbCrLf)
            StrSQL.Append(" ISNULL(Allegati_Documenti.Sottocartella, '') as Sottocartella, " + vbCrLf)
            StrSQL.Append(" ISNULL(Allegati_Documenti.Allegati_Documenti_Estensione, '') as Allegati_Documenti_Estensione," + vbCrLf)
            StrSQL.Append(" ISNULL(Allegati_Documenti.Allegati_Documenti_NomeFile, '') as Allegati_Documenti_NomeFile,  " + vbCrLf)
            StrSQL.Append(" ISNULL(Allegati_Documenti.Allegati_Documenti_Numero, '') as Allegati_Documenti_Numero, " + vbCrLf)
            StrSQL.Append(" ISNULL(Alert_Entita.Richiesta_Cod, 0) as Richiesta_Cod, ")
            StrSQL.Append(" ISNULL(Alert_Entita.Id_Schema_Template, 0) as Id_Schema_Template, ")
            StrSQL.Append(" Alert_Tipologia.ID_Area, Alert_Tipologia.ID_Tipologia, " + vbCrLf)
            StrSQL.Append(" Alert_Area.Nome As nome_area, Alert_Tipologia.Nome As nome_tipologia, " + vbCrLf)
            StrSQL.Append(" Alert_Elenco.Data_Scadenza, Alert_Elenco.Descrizione_Scadenza, Alert_Entita.Allegati_Documenti_Cod, Alert_Elenco.Note, " + vbCrLf)
            'StrSQL.Append(" ISNULL(Username_Upload, '') AS Username_Upload, ISNULL(Data_Upload, '') AS Data_Upload,  " + vbCrLf)
            StrSQL.Append(" LTRIM(RTRIM((Dettagli.Cognome + ' ' + Dettagli.Nome + ' ' + Dettagli.Rag_soc))) AS Username_Upload,  " + vbCrLf)
            StrSQL.Append(" ISNULL(Data_Upload, '') AS Data_Upload,  " + vbCrLf)
            StrSQL.Append(" ISNULL(Allegati_Documenti_Data, '') AS Allegati_Documenti_Data,  " + vbCrLf)
            StrSQL.Append(" ISNULL(Validazione_Flag, 0) as  Validazione_Flag, " + vbCrLf)
            StrSQL.Append(" Alert_Entita.ChkStorico as xChkStorico, Isnull(Alert_Entita.ChkStorico, 0) as ChkStorico, " + vbCrLf)
            StrSQL.Append(" File_Allegato_DB AS File_Allegato_DB, CompressoDaGIAS, " + vbCrLf)
            StrSQL.Append(" Alert_Entita.Ricetta_Operazione_cod," + vbCrLf)
            StrSQL.Append(" Agenda.Lav_Cod, " + vbCrLf)
            StrSQL.Append(" Alert_Entita.Id_ImpresexParticelle, " + vbCrLf)
            StrSQL.Append(" Allegati_Documenti.Pratica_Cod " + vbCrLf)

            StrSQL.Append(" , Allegati_Documenti.Validazione_Data " + vbCrLf)
            StrSQL.Append(" , Allegati_Documenti.Allegati_Documenti_Ente_Des " + vbCrLf)

            StrSQL.Append(" FROM  Alert_Tipologia ")

            StrSQL.Append(" INNER JOIN Alert_Elenco ON Alert_Tipologia.ID_Tipologia = Alert_Elenco.ID_Tipologia " + vbCrLf)
            StrSQL.Append(" INNER JOIN Alert_Entita ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser And Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita " + vbCrLf)
            StrSQL.Append(" LEFT JOIN Allegati_Documenti ON Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod " + vbCrLf)
            StrSQL.Append(" INNER JOIN Alert_Area ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area " + vbCrLf)
            StrSQL.Append(" LEFT JOIN Agenda on Alert_Entita.ID_Agenda = Agenda.Id_Agenda AND Alert_Entita.Piva = Agenda.PIVA " + vbCrLf)

            Dim NomeDB_Utenti As String = String.Format("{0}.dbo.", NomeDataBase_FromStringaConnessione(objParametri_Utenti.StringaConnessione))
            StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Utenti & "Utenti_Dettagli Dettagli ON Dettagli.UserName = Allegati_Documenti.Username_Upload")

            StrSQL.Append(" WHERE 1=1 " + vbCrLf)


            If ID_Elenco <> 0 Then
                StrSQL.Append(" And Alert_Elenco.ID_Elenco = " & Agro_SQL_SaveNum(ID_Elenco) & " " + vbCrLf)
            End If


            StrSQL.Append(" And Alert_Entita.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "' " + vbCrLf)


            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Alert_Entita.Inviato >=0 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Alert_Entita.Inviato =-1 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" + vbCrLf)
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    '##############################################################################################
    Public Function Leggi_con_documenti(ByVal ID_Alert_Entita As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.Leggi_con_documenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Alert_Entita.* ,Allegati_Documenti.Sottocartella, Allegati_Documenti.Allegati_Documenti_NomeFile ")
            StrSQL.Append(" FROM Alert_Entita  ")
            StrSQL.Append(" INNER JOIN Allegati_Documenti ON Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod  ")

            StrSQL.Append(" WHERE 1 = 1 ")

            If ID_Alert_Entita <> 0 Then
                StrSQL.Append(" AND Alert_Entita.ID_Alert_Entita = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            End If


            StrSQL.Append(" AND Alert_Entita.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Alert_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Alert_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    ' Nico --> ne ho dovuta fare un'altra perchè avevo bisogno di tutti i join, ma anche del filtro.
    ' e' stata fatta male l'originale, ma non so quante volte sia richiamata in giro
    '##############################################################################################
    Public Function Leggi_con_documenti_2(ByVal ID_Alert_Entita As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.Leggi_con_documenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Alert_Entita.* ,Allegati_Documenti.Sottocartella, Allegati_Documenti.Allegati_Documenti_NomeFile, Alert_Tipologia.ID_Area, Alert_Tipologia.ID_Tipologia, Alert_Area.Nome as nome_area, Alert_Tipologia.Nome AS nome_tipologia , Alert_Elenco.Data_Scadenza , Alert_Elenco.Descrizione_Scadenza, Alert_Entita.Allegati_documenti_cod, Alert_Elenco.ID_Elenco ")
            StrSQL.AppendLine(" , Alert_Entita.Id_Schema_Template Template_Da_Alert_Entita, ISNULL(Schema_Documenti_Template.Descrizione, '') Descrizione_Template")
            StrSQL.AppendLine(" , CASE")
            StrSQL.AppendLine("       WHEN Schema_Documenti_Template.Flag_obbligatorio = 0 Then 'No'")
            StrSQL.AppendLine("       WHEN Schema_Documenti_Template.Flag_obbligatorio = 1 Then 'Si'")
            StrSQL.AppendLine("       ELSE 'No'")
            StrSQL.AppendLine("   END AS Obbligatorio_Des")
            StrSQL.AppendLine(" , CASE")
            StrSQL.AppendLine("       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_prima_richiesta & " Then 'UMA Prima Richiesta'")
            StrSQL.AppendLine("       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_approvazione_prima_richiesta & " Then 'UMA Approvazione Prima Richiesta'")
            StrSQL.AppendLine("       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_richiesta_integrativa & " Then 'UMA Richiesta Integrativa'")
            StrSQL.AppendLine("       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_approvazione_richiesta_integrativa & " Then 'UMA Approvazione Richiesta Integrativa'")
            StrSQL.AppendLine("       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_rendicontazione & " Then 'UMA Rendicontazione'")
            StrSQL.AppendLine("       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_approvazione_rendicontazione & " Then 'UMA Approvaizone rendicontazione'")
            StrSQL.AppendLine("       ELSE 'Altro'")
            StrSQL.AppendLine("   END AS Fase_Des")
            StrSQL.AppendLine(" FROM  Alert_Tipologia ")
            StrSQL.AppendLine(" INNER JOIN Alert_Elenco ON Alert_Tipologia.ID_Tipologia = Alert_Elenco.ID_Tipologia ")
            StrSQL.AppendLine(" INNER JOIN Alert_Entita  ")
            StrSQL.AppendLine(" INNER JOIN Allegati_Documenti ON Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita ")
            StrSQL.AppendLine(" INNER JOIN Alert_Area ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area ")
            StrSQL.AppendLine(" LEFT JOIN Schema_Documenti_Template ON Alert_Entita.Id_Schema_Template = Schema_Documenti_Template.Id_Schema_Template ")

            StrSQL.Append(" WHERE 1= 1 ")


            If ID_Alert_Entita <> 0 Then
                StrSQL.Append(" AND Alert_Entita.ID_Alert_Entita = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            End If

            StrSQL.Append(" AND Alert_Entita.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Alert_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Alert_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

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


    Public Function Leggi_con_documenti(
            ByVal LeggiFileAllegatoDB As Boolean,
            ByVal Allegati_Documenti_Cod As Integer,
            ByVal ID_Alert_Entita As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.Leggi_con_documenti()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.AppendLine("  Select ")
            stb.AppendLine("    Alert_Entita.* ")
            stb.AppendLine("  , Allegati_Documenti.Sottocartella ")
            stb.AppendLine("  , Allegati_Documenti.Allegati_Documenti_NomeFile ")
            stb.AppendLine("  , Alert_Tipologia.ID_Area ")
            stb.AppendLine("  , Alert_Tipologia.ID_Tipologia ")
            stb.AppendLine("  , Alert_Area.Nome as nome_area ")
            stb.AppendLine("  , Alert_Tipologia.Nome AS nome_tipologia ")
            stb.AppendLine("  , Alert_Elenco.Data_Scadenza ")
            stb.AppendLine("  , Alert_Elenco.Descrizione_Scadenza ")
            stb.AppendLine("  , Alert_Entita.Allegati_documenti_cod  ")
            stb.AppendLine("  , Alert_Elenco.ID_Elenco")

            If LeggiFileAllegatoDB Then
                stb.AppendLine("  , Allegati_Documenti.File_Allegato_DB")
            End If

            stb.Append(" FROM  Alert_Tipologia ")
            stb.Append(" INNER JOIN Alert_Elenco ON Alert_Tipologia.ID_Tipologia = Alert_Elenco.ID_Tipologia ")
            stb.Append(" INNER JOIN Alert_Entita  ")
            stb.Append(" INNER JOIN Allegati_Documenti ON Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita ")
            stb.Append(" INNER JOIN Alert_Area ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area ")

            stb.Append(" WHERE 1= 1 ")

            If Allegati_Documenti_Cod <> 0 Then
                stb.Append(" AND Alert_Entita.Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            End If

            If ID_Alert_Entita <> 0 Then
                stb.Append(" AND Alert_Entita.ID_Alert_Entita = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            End If

            stb.Append(" AND Alert_Entita.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   Alert_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   Alert_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

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


    Public Function LeggiListFattureDaPagare(
            ByVal piva As String,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.LeggiListFattureDaPagare()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select Alert_Entita.PivaSuperUser, a.Piva, a.Id_Agenda, a.des_lib ")
            stb.AppendLine(" , m.Data_Movimento ")
            stb.AppendLine(" , m.Cod_RisUm, ISNULL(ru1.Settore_Des, '') AS Cod_Esterno_Contatto ")
            stb.AppendLine(" , m.Cod_Destinazione AS Conferente, ISNULL(ru2.Settore_Des, '') AS Cod_Esterno_Conferente ")
            stb.AppendLine(" , Alert_Entita.ID_Alert_Entita ")
            stb.AppendLine(" , ISNULL(indDataFatt.Valore_Des, '') AS Data_Fattura ")
            stb.AppendLine(" , ISNULL(indNrFatt.Valore_Des, '') AS Numero_Fattura ")
            stb.AppendLine(" , ISNULL(indContatto.Elenco_Val, '') AS Contatto ")
            stb.AppendLine(" , ISNULL(indStatoPag.ID_Indice_Det, 7) AS Stato_Pagamento_Cod ")
            stb.AppendLine(" , indStatoPagDetta.ID_Indice AS ID_Indice_Alert_Indice_Dettagli")
            stb.AppendLine(" , ISNULL(indStatoPagDetta.Valore, 'Nessuno') AS Stato_Pagamento_Des ")
            stb.AppendLine(" , Alert_Elenco.Data_Scadenza ")
            stb.AppendLine(" , Alert_Elenco.ID_Tipologia ")
            stb.AppendLine(" From Alert_Entita ")
            stb.AppendLine(" INNER Join Alert_Elenco ON Alert_Entita.ID_Alert_Entita = Alert_Elenco.ID_Alert_Entita ")
            stb.AppendLine(" INNER Join Agenda a ON Alert_Entita.Piva = a.Piva And Alert_Entita.ID_Agenda = a.Id_Agenda ")
            stb.AppendLine(" INNER Join Movimenti m ON a.Piva = m.Piva And a.Id_Agenda = m.Id_Agenda And m.Cau_Mov = '4000' ")
            stb.AppendLine(" Left Join Risorse_Umane ru1 ON m.Cod_RisUm = ru1.Cod_RisUm ")
            stb.AppendLine(" Left Join Risorse_Umane ru2 ON m.Cod_Destinazione = ru2.Cod_RisUm ")
            stb.AppendLine(" Left Join Alert_EntitaxIndici indDataFatt ON Alert_Entita.ID_Alert_Entita = indDataFatt.ID_Alert_Entita And indDataFatt.ID_Indice = -4 -- Indice Data Fattura ")
            stb.AppendLine(" Left Join Alert_EntitaxIndici indNrFatt ON Alert_Entita.ID_Alert_Entita = indNrFatt.ID_Alert_Entita And indNrFatt.ID_Indice = -5 -- Indice Numero Fattura ")
            stb.AppendLine(" Left Join Alert_EntitaxIndici indContatto ON Alert_Entita.ID_Alert_Entita = indContatto.ID_Alert_Entita And indContatto.ID_Indice = -8 -- Indice Contatto ")
            stb.AppendLine(" Left Join Alert_EntitaxIndici indStatoPag ON Alert_Entita.ID_Alert_Entita = indStatoPag.ID_Alert_Entita And indStatoPag.ID_Indice = -9 -- Indice Stato Pagamento ")
            stb.AppendLine(" Left Join Alert_Indice_Dettagli indStatoPagDetta ON indStatoPag.ID_Indice = indStatoPagDetta.ID_Indice And indStatoPag.ID_Indice_Det = indStatoPagDetta.ID_Indice_Det -- Valori dettaglio stato pagamento ")
            stb.AppendLine(" WHERE a.Lav_Cod In (1031, 1025, 1054, 1076, 1078) -- DDT Emesso + DDT Ricevuto + Accettazione ")
            stb.AppendLine(" And Alert_Elenco.ID_Tipologia IN (-16,-17) -- Fatture attive e fatture passive ")
            stb.AppendLine(" And indStatoPag.ID_Indice_Det <> 5 -- Fattura non interamente pagata ")
            ''stb.AppendLine(" And Alert_Elenco.Data_Scadenza <= CAST(GETDATE() AS DATE) RaramDPassare -- Data scadenza superata ")
            stb.AppendLine(" And indStatoPag.ID_Indice_Det <> 4 ")



            If piva <> "" Then
                stb.Append(" And a.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   Alert_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   Alert_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

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
