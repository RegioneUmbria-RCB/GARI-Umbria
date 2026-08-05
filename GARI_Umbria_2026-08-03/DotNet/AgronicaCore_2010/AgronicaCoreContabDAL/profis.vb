Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Profis_R
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function LeggiPerGruppoAliquote(ByVal id_Agenda As Integer, ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append(" select a.Lav_cod, coalesce( decodeIVA.Profis_TipoIVA, -1) as iva_aliquota_profis_cod " & vbCrLf)
            stb.Append("  , d.* " & vbCrLf)
            stb.Append(" from Agenda a " & vbCrLf)
            stb.Append("    inner join Movimenti m " & vbCrLf)
            stb.Append("        on a.Id_Agenda = m.Id_Agenda  " & vbCrLf)
            stb.Append("        and a.Sa_Cod = m.Sa_Cod  " & vbCrLf)
            stb.Append("        and a.PIVA = m.PIVA  " & vbCrLf)
            stb.Append("    inner join Movimenti_dettagli d " & vbCrLf)
            stb.Append("        on d.Id_Agenda = m.Id_Agenda  " & vbCrLf)
            stb.Append("        and d.Id_Mov = d.Id_Mov " & vbCrLf)
            stb.Append("    left join PROFIS_Decodifica_TipiIVA_LavCod_CodIVA decodeIva " & vbCrLf)
            stb.Append("        on decodeIva.Gias_cod_iva = d.cod_iva " & vbCrLf)
            stb.Append(" where a.Id_Agenda = " & Agro_SQL_SaveNum(id_Agenda) & " " & vbCrLf)
            stb.Append(" and m.cau_mov = '4000' " & vbCrLf)
            stb.Append("  " & vbCrLf)



            DT = EseguiQuery_Lettura(objparametri, stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT



    End Function


    Public Function LeggiDettaglio(ByVal id_Agenda As Integer, ByVal cau_mov As String, ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append("select  " & vbCrLf)
            stb.Append("      d.Elem_Cod as Categoria " & vbCrLf)
            stb.Append("    , d.Pro_Cod as Cod_Prodotto " & vbCrLf)
            stb.Append("    , d.Mov_Det_Des as Descrizione1 " & vbCrLf)
            stb.Append("    , '' as Descrizione2 " & vbCrLf)
            stb.Append("    , d.Qta  " & vbCrLf)
            stb.Append("    , udm.UDM_SIM as UnitaMisura " & vbCrLf)
            stb.Append("    , abs(d.imponibile) as Prezzo " & vbCrLf)
            stb.Append("    , d.cod_iva " & vbCrLf)
            stb.Append("    , 0 as Sconto1 " & vbCrLf)
            stb.Append("    , 0 as Sconto2 " & vbCrLf)
            stb.Append("    , 'A' as Tipo_Riga " & vbCrLf)
            stb.Append(" from movimenti m  " & vbCrLf)
            stb.Append("    inner join Movimenti_dettagli d " & vbCrLf)
            stb.Append("        on m.id_agenda = d.id_agenda " & vbCrLf)
            stb.Append("        and m.id_mov = d.Id_Mov " & vbCrLf)
            stb.Append("    inner join UnitaMisura udm " & vbCrLf)
            stb.Append("        on udm.UDM_COD = d.Udm_Cod " & vbCrLf)
            stb.Append(" where m.id_agenda = " & id_Agenda & vbCrLf)
            stb.Append(" and Cau_Mov = '" & Agro_SQL_SaveText(cau_mov) & "' " & vbCrLf)

            DT = EseguiQuery_Lettura(objparametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT



    End Function


    Public Function LeggiDataMovimento(ByVal id_Agenda As Integer, ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append(" select top 1  Movimenti.Doc_Numero_Sin , CONVERT(varchar(50), CONVERT(int, Movimenti.Doc_Numero)) as Doc_Numero , Movimenti.Doc_Numero_Des, data_movimento from movimenti where cau_mov=4000 and id_agenda = " & Agro_SQL_SaveNum(id_Agenda))

            DT = EseguiQuery_Lettura(objparametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT



    End Function

    '##############################################################################################
    Public Function LeggiTestata(ByVal piva As String, _
                                  ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append(" select top 1 ii.PIVA, coalesce(legaleRapp.rag_soc, '') as NomeCognome, c.codice_fiscale, coalesce(case when legaleRapp.Codice_Fiscale = '' then legaleRapp.Cod_Contatto else legaleRapp.Codice_Fiscale end, '') as Codice_Fiscale_legale, ii.rag_soc, icATECO.val_cod as Ateco2007, coalesce(icCF.val_cod, '') as intermediarioCF,coalesce(icCAF.val_Cod, '') as intermediarioCAF , psw.valore  as PSW  " & vbCrLf)
            stb.Append("  from imprese ii  " & vbCrLf)
            stb.Append("     inner join  imprese_codici icATECO  " & vbCrLf)
            stb.Append("         on ii.piva = icATECO.piva   " & vbCrLf)
            stb.Append("         and id_cod = 1266  " & vbCrLf)
            stb.Append("     left join  imprese_codici icCF  " & vbCrLf)
            stb.Append("         on ii.piva = icCF.piva   " & vbCrLf)
            stb.Append("         and icCF.id_cod = 1268  " & vbCrLf)
            stb.Append("     left join  imprese_codici icCAF  " & vbCrLf)
            stb.Append("         on ii.piva = icCAF.piva   " & vbCrLf)
            stb.Append("         and icCAF.id_cod = 1269         " & vbCrLf)
            stb.Append("     left join ( " & vbCrLf)
            stb.Append("        select c.piva, c.cod_contatto, c.Rag_Soc, c.Codice_Fiscale  " & vbCrLf)
            stb.Append("        from contatti c          " & vbCrLf)
            stb.Append("        inner join Risorse_Umane risum " & vbCrLf)
            stb.Append("            on c.Piva = risum.Piva  " & vbCrLf)
            stb.Append("            and c.Cod_Contatto = risum.Cod_Contatto  " & vbCrLf)
            stb.Append("            and risum.Cod_Rapporto = -1 " & vbCrLf)
            stb.Append("    ) legaleRapp " & vbCrLf)
            stb.Append("    on legaleRapp.piva = ii.PIVA           " & vbCrLf)
            stb.Append("         and legaleRapp.piva = '" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)
            stb.Append("      inner join contatti c  " & vbCrLf)
            stb.Append("         on c.piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'" & vbCrLf)
            stb.Append("         and c.Cod_Contatto = '" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)
            stb.Append("     inner join configurazione_siti psw  " & vbCrLf)
            stb.Append("     on  psw.chiave = 'SpesometroProduttoreSoftware' " & vbCrLf)

            stb.Append(" where ii.piva = '" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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


'#################################################################
'#################################################################
'#################################################################

Public Class Profis_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function ScriviXVerificaParteVariabile( _
                      ByVal Record_tipo As String _
                    , ByVal Record_Numero As String _
                    , ByVal Chiave As String _
                    , ByVal valore As String _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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
            StrSQL.Length = 0
            StrSQL.Append(" INSERT spesometro_dati " + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("Record_tipo, record_numero, Chiave, Valore ")
            'StrSQL.Append("              Inviato,            datainvio, ")
            'StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            'StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            'StrSQL.Append("              Validita_Inizio,    Validita_Fine, ")
            'StrSQL.Append("              Validazione, Data_Validazione, UserName_Validazione " + vbCrLf)


            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 '" & Agro_SQL_SaveText(Record_tipo) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveNum(Record_Numero) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Chiave) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(valore) & "' ")

            'StrSQL.Append("         , 0  " + vbCrLf)
            'StrSQL.Append("         , Null  " + vbCrLf)

            'StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            'StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            'StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            'StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append(") ")

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







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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


