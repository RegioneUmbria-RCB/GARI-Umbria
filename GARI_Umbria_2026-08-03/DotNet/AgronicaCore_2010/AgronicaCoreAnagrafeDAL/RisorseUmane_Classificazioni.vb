Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class Risorse_Umane_Classificazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_EF_JSON(ByVal Piva As String,
                                  ByVal Id_RisUm_CL As Integer,
                                  ByVal Codice As String,
                                  ByVal Descrizione As String,
                                  ByVal Descr_Breve As String,
                                  ByVal Visibilita As Integer,
                                  ByVal ChkDefault As Integer,
                                  ByVal Flag_Aggiungi_Visibilita_Tutti As Boolean,
                                  ByVal Flag_Aggiungi_Col_x_VB6 As Boolean,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Risorse_Umane_Classificazioni_R.Leggi_EF_JSON()"

        Dim messaggioErrore As String = ""
        Dim risposta As String = ""

        Try
            Dim listItems = Leggi_EF(Piva, Id_RisUm_CL, Codice, Descrizione, Descr_Breve, Visibilita, ChkDefault,
                                     Flag_Aggiungi_Visibilita_Tutti, Flag_Aggiungi_Col_x_VB6,
                                     objParametri, Nothing)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            risposta = JsonConvert.SerializeObject(listItems, Formatting.None, serializerSettings)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_EF(ByVal Piva As String,
                             ByVal Id_RisUm_CL As Integer,
                             ByVal Codice As String,
                             ByVal Descrizione As String,
                             ByVal Descr_Breve As String,
                             ByVal Visibilita As Integer,
                             ByVal ChkDefault As Integer,
                             ByVal Flag_Aggiungi_Visibilita_Tutti As Boolean,
                             ByVal Flag_Aggiungi_Col_x_VB6 As Boolean,
                             ByRef objParametri As AgronicaCoreParametri,
                             ByVal GiasContext As Gias_DeveloperServer_Entities
                             ) As List(Of Risorse_Umane_Classificazioni)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Risorse_Umane_Classificazioni_R.Leggi_EF()"

        Dim messaggioErrore As String = ""
        Dim listItems As List(Of Risorse_Umane_Classificazioni)

        Dim gefutils As New Gias_EF_Utility
        Dim GiasContextInternal As Gias_DeveloperServer_Entities

        Try
            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            If GiasContext Is Nothing Then
                GiasContextInternal = New Gias_DeveloperServer_Entities(efConnString)
            Else
                GiasContextInternal = GiasContext
            End If

            Dim RuClALL = From ruc In GiasContextInternal.Risorse_Umane_Classificazioni _
                          Select ruc

            If Piva <> "" Then
                RuClALL = RuClALL.Where(Function(x) x.Piva.Equals(Piva))
            End If

            If Id_RisUm_CL <> 0 Then
                RuClALL = RuClALL.Where(Function(x) x.Id_RisUm_CL = CInt(Id_RisUm_CL))
            End If

            If Codice <> "" Then
                RuClALL = RuClALL.Where(Function(x) x.Codice.Equals(Codice))
            End If

            If Descrizione <> "" Then
                RuClALL = RuClALL.Where(Function(x) x.Descrizione.Equals(Descrizione))
            End If

            If Descr_Breve <> "" Then
                RuClALL = RuClALL.Where(Function(x) x.Descr_Breve.Equals(Descr_Breve))
            End If

            If Visibilita <> -999 AndAlso Flag_Aggiungi_Visibilita_Tutti Then
                RuClALL = RuClALL.Where(Function(x) (x.Visibilita = CInt(Visibilita) OrElse x.Visibilita = 0))

            ElseIf Visibilita <> -999 AndAlso Not Flag_Aggiungi_Visibilita_Tutti Then
                RuClALL = RuClALL.Where(Function(x) x.Visibilita = CInt(Visibilita))
            End If

            If ChkDefault <> -1 Then
                RuClALL = RuClALL.Where(Function(x) x.ChkDefault = CInt(ChkDefault))
            End If

            listItems = RuClALL.ToList()

            GiasContextInternal = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            GiasContextInternal = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return listItems

    End Function


    '##############################################################################################
    Public Function Leggi_SQL(ByVal Piva As String,
                              ByVal Id_RisUm_CL As Integer,
                              ByVal Codice As String,
                              ByVal Descrizione As String,
                              ByVal Descr_Breve As String,
                              ByVal Visibilita As Integer,
                              ByVal ChkDefault As Integer,
                              ByVal Flag_Aggiungi_Visibilita_Tutti As Boolean,
                              ByVal Flag_Aggiungi_Col_x_VB6 As Boolean,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable

        '--------------------------------------------------------
        ' Facoltativi
        '   Piva = ""
        '   Id_RisUm_CL = 0
        '   Codice = ""
        '   Descrizione = ""
        '   Descr_Breve = ""
        '   Visibilita = -999 (0 è significativo = vale per tutti i rapporti contabili)
        '   ChkDefault = -1
        '--------------------------------------------------------

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Risorse_Umane_Classificazioni_R.Leggi_SQL()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT     ruc.* " & vbCrLf)

            If Flag_Aggiungi_Col_x_VB6 Then
                StrSQL.Append(" , IIF(ruc.Visibilita = 0, 'Tutti i Rapporti Contabili', rc.Rapporto_Des) as Visibilita_Des " & vbCrLf)
                StrSQL.Append(" , '' as OperazioneDB, '' as Select_Col " & vbCrLf)
            End If

            StrSQL.Append(" FROM       Risorse_Umane_Classificazioni ruc " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Rapporti_Contabili rc on rc.Cod_rapporto = ruc.Visibilita " & vbCrLf)
            StrSQL.Append(" WHERE 1 = 1 " & vbCrLf)

            If Piva <> "" Then
                StrSQL.Append(" AND ruc.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If

            If Id_RisUm_CL <> 0 Then
                StrSQL.Append(" AND ruc.Id_RisUm_CL = " & Agro_SQL_SaveNum(Id_RisUm_CL) & "   " & vbCrLf)
            End If

            If Codice <> "" Then
                StrSQL.Append(" AND ruc.Codice = '" & Agro_SQL_SaveText(Codice) & "'   " & vbCrLf)
            End If

            If Descrizione <> "" Then
                StrSQL.Append(" AND ruc.Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "'   " & vbCrLf)
            End If

            If Descr_Breve <> "" Then
                StrSQL.Append(" AND ruc.Descr_Breve = '" & Agro_SQL_SaveText(Descr_Breve) & "'   " & vbCrLf)
            End If


            If Visibilita <> -999 Then
                StrSQL.Append(" AND (ruc.Visibilita = " & Agro_SQL_SaveNum(Visibilita) & "  " & vbCrLf)

                If Flag_Aggiungi_Visibilita_Tutti Then
                    StrSQL.Append(" OR ruc.Visibilita = 0 ) " & vbCrLf)
                Else
                    StrSQL.Append(" ) " & vbCrLf)
                End If
            End If

            If ChkDefault <> -1 Then
                StrSQL.Append(" AND ruc.ChkDefault = " & Agro_SQL_SaveNum(ChkDefault) & "   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ruc.Inviato >=0 " & vbCrLf)
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ruc.Inviato =-1 " & vbCrLf)
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Risorse_Umane_Classificazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '############################################################################ 
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Id_RisUm_CL"></param>
    ''' <param name="Codice"></param>
    ''' <param name="Descrizione"></param>
    ''' <param name="Descr_Breve"></param>
    ''' <param name="Visibilita"></param>
    ''' <param name="ChkDefault"></param>
    ''' <param name="OperazioneDB">1 = Insert, 2 = Update, 3 = Delete</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Aggiorna_EF_IUD(ByVal Piva As String,
                                    ByVal Id_RisUm_CL As Integer,
                                    ByVal Codice As String,
                                    ByVal Descrizione As String,
                                    ByVal Descr_Breve As String,
                                    ByVal Visibilita As Integer,
                                    ByVal ChkDefault As Integer,
                                    ByVal OperazioneDB As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByVal GiasContext As Gias_DeveloperServer_Entities
                                    ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Risorse_Umane_Classificazioni_W.Aggiorna_EF_IUD()"
        Dim messaggioErrore As String = ""
        Dim result As String = ""

        Dim ObjRUClassR As New Risorse_Umane_Classificazioni_R

        Dim objSequenze = New Agro_Sequenze
        Dim idSeq As Integer = 0

        'Dim retries As Integer = 3
        'Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim GiasContextInternal As Gias_DeveloperServer_Entities

        Try

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            If GiasContext Is Nothing Then
                GiasContextInternal = New Gias_DeveloperServer_Entities(efConnString)
            Else
                GiasContextInternal = GiasContext
            End If

            'verifico se l'elemento è già presente ==> update (else insert)
            Dim listItems = ObjRUClassR.Leggi_EF(Piva, Id_RisUm_CL, "", "", "",
                                                 -999, -1, True, False,
                                                 objParametri, GiasContextInternal)

            If listItems.Count = 1 AndAlso (OperazioneDB = enum_TipoOperazioneDB.Modifica OrElse OperazioneDB = 0) Then
                'l'elemento esiste ==> update
                result = "UPDATE: "

                Dim tempElement As Risorse_Umane_Classificazioni = listItems.FirstOrDefault

                tempElement.Codice = Codice
                tempElement.Descrizione = CStr(Descrizione)
                tempElement.Descr_Breve = Descr_Breve
                tempElement.Visibilita = Visibilita
                tempElement.ChkDefault = CInt(ChkDefault)
                tempElement.Data_Modifica = Date.Now
                tempElement.Username_Modifica = objParametri.UsernameOperazione

                GiasContextInternal.Risorse_Umane_Classificazioni.Attach(tempElement)
                GiasContextInternal.Entry(tempElement).State = EntityState.Modified
                GiasContextInternal.SaveChanges()

            ElseIf listItems.Count = 1 AndAlso (OperazioneDB = enum_TipoOperazioneDB.Cancellazione) Then
                'si è scelto di eliminare l'elemento
                result = "DELETE: "

                Dim tempElement As Risorse_Umane_Classificazioni = listItems.FirstOrDefault

                GiasContextInternal.Risorse_Umane_Classificazioni.Remove(tempElement)
                GiasContextInternal.SaveChanges()

            ElseIf listItems.Count = 0 AndAlso (OperazioneDB = enum_TipoOperazioneDB.Scrittura OrElse OperazioneDB = 0) Then
                'l'elemento non esiste ==> insert
                result = "INSERT: "

                'Richiedo un nuovo id sequenza
                idSeq = objSequenze.NuovoId_Tabella_EF(GiasContextInternal, "Risorse_Umane_Classificazioni",
                                                       0, 2000000000, objParametri)

                Dim tempElement As New Risorse_Umane_Classificazioni With {
                    .Piva = CStr(Piva),
                    .Id_RisUm_CL = CInt(idSeq),
                    .Codice = Codice,
                    .Descrizione = CStr(Descrizione),
                    .Descr_Breve = Descr_Breve,
                    .Visibilita = Visibilita,
                    .ChkDefault = CInt(ChkDefault),
                    .inviato = 0,
                    .Validita_Inizio = AGRODATAINIZIO,
                    .Validita_Fine = AGRODATAFINE,
                    .Data_Creazione = Date.Now,
                    .Data_Modifica = Date.Now,
                    .Username_Creazione = objParametri.UsernameOperazione,
                    .Username_Modifica = objParametri.UsernameOperazione
                }

                GiasContextInternal.Risorse_Umane_Classificazioni.Add(tempElement)
                GiasContextInternal.SaveChanges()
            Else
                Throw New Exception("IMPOSSIBILE ESEGUIRE OPERAZIONE DI UPDATE O INSERT [Piva = " & Piva & ", Id_RisUm_CL = " & Id_RisUm_CL & "]")
            End If

            GiasContextInternal = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : [Piva = " & Piva & ", Id_RisUm_CL = " & Id_RisUm_CL & "] " & messaggioErrore)
            GiasContextInternal = Nothing
            Return result & " [Piva = " & Piva & ", Id_RisUm_CL = " & Id_RisUm_CL & "] " & messaggioErrore
        End Try

        Return result & "OK"

    End Function

End Class
