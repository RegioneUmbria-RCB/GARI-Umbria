


Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ModelliPrevisionaliXpiva_OperazioniAutorizzate_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
        ByVal Modello_Cod As Integer,
        ByVal piva As String,
        ByVal Piva_SuperUser As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select Mod_Cod, Piva_SuperUser, piva, Tipo_Visibilita, DescrizioneAggiuntiva ")
            Stb.AppendLine(" From ModelliPrevisionaliXpiva_OperazioniAutorizzate ")
            Stb.AppendLine(" where piva_SuperUser  = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
            Stb.AppendLine(" and piva = '" & Agro_SQL_SaveText(piva) & "'")

            If Modello_Cod <> 0 Then
                Stb.AppendLine(" and Mod_Cod = " & Modello_Cod)
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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



'#################################################################
'#################################################################
'#################################################################

Public Class ModelliPrevisionaliXpiva_OperazioniAutorizzate_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
                          ByVal Mod_Cod As Integer,
                          ByVal Piva_SuperUser As String,
                          ByVal piva As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT ModelliPrevisionaliXpiva_OperazioniAutorizzate " + vbCrLf)

            Stb.Append("              (")
            Stb.Append("              Mod_Cod, Piva_SuperUser, piva, Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")

            Stb.Append("			  " & Agro_SQL_SaveNum(Mod_Cod) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(piva) & "' ")

            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            Stb.Append("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")


            Stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function RiportaModelliDatoPacchettoCommerciale(
                          ByVal ModelliPrevisionaliRaggruppamenti_COD As Integer,
                          ByVal Piva_SuperUser As String,
                          ByVal piva As String,
                          ByVal scadenza As DateTime,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0

            Stb.AppendLine(" insert [dbo].[ModelliPrevisionaliXpiva_OperazioniAutorizzate](mod_cod, Piva_SuperUser, piva, Tipo_Visibilita, DescrizioneAggiuntiva, Validita_Fine) ")
            Stb.AppendLine(" select s.mod_cod, '" & Agro_SQL_SaveText(Piva_SuperUser) & " ' as piva_SuperUser, '" & Agro_SQL_SaveText(piva) & "' as piva, 1 as tipo_Visibilita, S.DescrizioneAggiuntiva, " & Agro_SQL_SaveDate(scadenza) & " as scadenza  ")
            Stb.AppendLine(" From [dbo].[ModelliPrevisionaliRaggruppamentiXModelliPrevisionali] p ")
            Stb.AppendLine("            inner Join ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate s ")
            Stb.AppendLine("         On p.Mod_COD = s.Mod_Cod ")
            Stb.AppendLine("      And s.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            Stb.AppendLine("      and s.Tipo_Visibilita = " & enum_ModelliPrevisionali_TipoVisibilita.VerificaUlterioreNecessaria)
            Stb.AppendLine(" Where ModelliPrevisionaliRaggruppamenti_COD = " & Agro_SQL_SaveNum(ModelliPrevisionaliRaggruppamenti_COD))
            Stb.AppendLine(" And Not exists ( ")
            Stb.AppendLine("  Select 1 ")
            Stb.AppendLine("     From ModelliPrevisionaliXpiva_OperazioniAutorizzate test ")
            Stb.AppendLine("  Where p.Mod_COD = test.Mod_Cod ")
            Stb.AppendLine("  And test.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine("  And test.Piva_SuperUser = '" & Piva_SuperUser & "' ")
            Stb.AppendLine(" )")




            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function






    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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



