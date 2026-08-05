Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Richieste_Lavorazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal gruppo_colturale As String,
                          ByVal Programmazione_Cod As Integer,
                          ByVal Richiesta_Cod As Integer,
                          ByVal Richiesta_Dettaglio_Cod As Integer,
                          ByVal Selezione_Variabile As enumSelezioneVariabile,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT UMA_Richieste_Lavorazioni.Piva_SuperUser, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Piva, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Gruppo_Colturale_UMA, ")
            stb.AppendLine(" 	UMA_Macrousi.Macrouso_UMA_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Richiesta_Cod, ")
            stb.AppendLine("    UMA_Richieste_Lavorazioni.Programmazione_Cod, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Richiesta_Dettaglio_Cod, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Lavorazione_UMA, ")
            stb.AppendLine(" 	UMA_Lavorazioni.Lav_UMA_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Lavorazione_GIAS, ")
            stb.AppendLine(" 	Operazioni.Lav_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Tipo_Carburante, ")
            stb.AppendLine(" 	Carburanti.Car_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Superficie_Maggiorazione_Trasferimenti, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Nr_Lavorazioni_Previste, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Nr_Lavorazioni_Richieste, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Piu_Lavorazioni_Previste, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Piu_Raccolti_Previsti, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Fabbisogno_Calcolato, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Fabbisogno_Richiesto, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Validita_Inizio, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Validita_Fine, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Inviato, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.DataInvio, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Data_Creazione, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Data_Modifica, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Username_Creazione, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Username_Modifica, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Totale_Superficie_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Pendenza_A_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Pendenza_B_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Tessitura_Normale_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Tessitura_Media_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Tessitura_Tenace_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Programmazione_Cod, ")
            stb.AppendLine(" 	ISNULL(UMA_Richieste_Lavorazioni.Id_Attivita, 0) as Attivita_Cod, ")
            stb.AppendLine(" 	ISNULL(Attivita.[Desc], '') as Attivita_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Note_Compilatore, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Note_Approvatore, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Qta_Manuale, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Mesi, ")
            stb.AppendLine(" 	ISNULL(c.Udm_Alternativa, '') as Udm_Alternativa ")
            stb.AppendLine(" FROM UMA_Richieste_Lavorazioni ")
            stb.AppendLine(" JOIN UMA_Lavorazioni ON UMA_Richieste_Lavorazioni.Lavorazione_UMA = UMA_Lavorazioni.Lav_UMA_Cod ")
            stb.AppendLine(" JOIN Operazioni ON UMA_Richieste_Lavorazioni.Lavorazione_GIAS = Operazioni.Lav_Cod ")
            stb.AppendLine(" JOIN Carburanti ON UMA_Richieste_Lavorazioni.Tipo_Carburante = Carburanti.Car_Cod ")
            stb.AppendLine(" LEFT JOIN UMA_Macrousi ON UMA_Richieste_Lavorazioni.Gruppo_Colturale_UMA = UMA_Macrousi.Macrouso_UMA_Cod ")
            stb.AppendLine(" LEFT JOIN Attivita ON UMA_Richieste_Lavorazioni.Id_Attivita = Attivita.ID_Attivita ")
            stb.AppendLine(" LEFT JOIN UMA_Configurazione_MacrousixLavorazioni c ON c.Macrouso_UMA_Cod = UMA_Richieste_Lavorazioni.Gruppo_Colturale_UMA ")
            stb.AppendLine(" 											    AND c.Lav_UMA_Cod = UMA_Richieste_Lavorazioni.Lavorazione_UMA ")
            stb.AppendLine(" 												AND c.Lav_Cod = UMA_Richieste_Lavorazioni.Lavorazione_GIAS ")
            stb.AppendLine(" 												AND c.Id_Attivita = UMA_Richieste_Lavorazioni.id_attivita ")
            stb.AppendLine(" WHERE 1= 1 ")
            stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Piva_SuperUser = " + Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + " ")

            If piva <> "" Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Piva = " + Agro_SQL_SaveText_NULL(piva) + " ")
            End If

            If gruppo_colturale <> "" Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Gruppo_Colturale_UMA = " + Agro_SQL_SaveText_NULL(gruppo_colturale) + " ")
            End If

            If Richiesta_Cod <> 0 Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Richiesta_Cod = " + Agro_SQL_SaveNum(Richiesta_Cod) + " ")
            End If

            If Programmazione_Cod <> 0 Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Programmazione_Cod = " + Agro_SQL_SaveNum(Programmazione_Cod) + " ")
            End If

            If Richiesta_Dettaglio_Cod <> 0 Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Richiesta_Dettaglio_Cod = " + Agro_SQL_SaveNum(Richiesta_Dettaglio_Cod) + " ")
            End If

            stb.AppendLine(" ORDER BY UMA_Richieste_Lavorazioni.Data_Creazione DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Lavorazioni_Alternative(ByVal gruppo_colturale As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  Optional ByVal xFiltroAggiuntivo As String = "",
                                                  Optional ByVal xOrderBy As String = "") As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi_Lavorazioni_Alternative()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT l.Lavorazione_UMA , umal.Lav_UMA_Des, l.Lavorazione_UMA_Alt ")
            stb.AppendLine(" FROM UMA_Lavorazioni_Alternative l ")
            stb.AppendLine(" JOIN UMA_Lavorazioni umal ON l.Lavorazione_UMA = umal.Lav_UMA_Cod ")
            stb.AppendLine(" WHERE Gruppo_Colturale_UMA = " + Agro_SQL_SaveNum(gruppo_colturale) + " ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy) + " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Per_Controllo_Incrociato(ByVal piva As String, ByVal programmazione_cod As Integer, ByVal pivaChiamante As String, ByVal gruppo_colturale As Integer, ByVal anno As Integer,
                                                   ByVal xOrderBy As String, ByVal xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional isTerzista As Boolean = False,
                                                   Optional ByVal richiesta_Cod As Integer = 0, Optional ByVal lavorazioneUMA As Integer = 0) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi_Per_Controllo_Incrociato()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT ic.val_cod, p.numero, i.rag_soc, ul.Lav_UMA_Des, l.Gruppo_Colturale_UMA, l.Lavorazione_UMA, l.Totale_Superficie_UMA, l.Zona_Pendenza_A_UMA, l.Zona_Pendenza_B_UMA,  ")
            stb.AppendLine(" l.Zona_Tessitura_Normale_UMA, l.Zona_Tessitura_Media_UMA, l.Zona_Tessitura_Tenace_UMA, l.Validita_Inizio ")
            stb.AppendLine(" from UMA_Richieste_Lavorazioni l")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.richiesta_cod = l.Richiesta_Cod ")
            stb.AppendLine(" Join Pratiche p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" Join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" Join Imprese i on i.piva = t.piva ")
            stb.AppendLine(" Join UMA_LAvorazioni ul on l.Lavorazione_UMA = ul.Lav_UMA_Cod ")
            stb.AppendLine(" join imprese_codici ic on ic.piva = t.piva and ic.id_cod = 1010 ")
            stb.AppendLine(" where  psa.Stato_Cod <> 2009 " + IIf(lavorazioneUMA <> 0, "AND Lavorazione_UMA = " + lavorazioneUMA.ToString + " ", "") + " AND l.Programmazione_Cod = " + programmazione_cod.ToString + " AND l.Richiesta_Cod IN (Select r.Richiesta_Cod from UMA_Richieste r ")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.Richiesta_Cod = r.Richiesta_Cod ")
            stb.AppendLine(" join pratiche p on t.pratica_cod = p.pratica_cod ")
            stb.AppendLine(" where t.Avanzamento_Richiesta = 1 AND p.Anno = " + IIf(anno <> 0, anno.ToString, "(Select Anno From Pratiche ppp JOIN UMA_Richieste_Testata ttt ON ttt.Pratica_Cod = ppp.Pratica_cod Where ttt.Richiesta_Cod = " + richiesta_Cod.ToString + ")") + " And ")
            If isTerzista Then

                stb.AppendLine(" t.Piva != " + IIf(pivaChiamante <> "", "'" + pivaChiamante + "'", "(Select piva From UMA_Richieste_Testata Where Richiesta_Cod = " + richiesta_Cod.ToString + ")") + " AND ")

            Else

                stb.AppendLine(" t.Tipo_Richiesta = -1 AND ")

            End If

            If gruppo_colturale <> 0 Then

                stb.AppendLine(" l.Gruppo_Colturale_UMA = " + gruppo_colturale.ToString + " AND  ")

            End If


            stb.AppendLine(" r.Piva = '" + piva + "' AND ")
            stb.AppendLine(" (Select COUNT(*) from UMA_Richieste_Lavorazioni ll where ll.Richiesta_Cod = l.Richiesta_Cod) > 0 ) ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy) + " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function
End Class

Public Class UMA_Richieste_Lavorazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_Lavorazioni(ByVal InsertArray As ArrayList,
                                         ByVal UpdateArray As ArrayList,
                                         ByVal RichiesteUpdateArray As ArrayList,
                                         ByVal DeleteArray As ArrayList,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Aggiorna_Lavorazioni()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each del As UMA_Richieste_Lavorazioni In DeleteArray
                    GiasContext.UMA_Richieste_Lavorazioni.Attach(del)
                    GiasContext.UMA_Richieste_Lavorazioni.Remove(del)
                Next

                For Each up As UMA_Richieste_Lavorazioni In UpdateArray
                    GiasContext.UMA_Richieste_Lavorazioni.Attach(up)
                    GiasContext.Entry(up).State = EntityState.Modified
                Next

                For Each upRichieste As UMA_Richieste In RichiesteUpdateArray
                    GiasContext.UMA_Richieste.Attach(upRichieste)
                    GiasContext.Entry(upRichieste).State = EntityState.Modified
                Next

                For Each ins As UMA_Richieste_Lavorazioni In InsertArray
                    GiasContext.UMA_Richieste_Lavorazioni.Add(ins)
                Next

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato

    End Function

End Class
