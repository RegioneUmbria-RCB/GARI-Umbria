Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.Widgets
Imports AgronicaCoreUtentiDAL
Imports Newtonsoft.Json

Public Class Widgets
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Leggi(ByVal IdWidget As Integer,
                          ByRef objParametri As AgronicaCoreParametri) As Widget

        Dim objWidgets As New AgronicaCoreMetaSchemaDAL.Widgets_R
        Dim dt As DataTable = Nothing
        Dim retVal As Widget = Nothing

        If EsisteTabella("Widgets", objParametri) Then
            dt = objWidgets.Leggi("", "", "",
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "", "", objParametri, Nothing, Nothing, Nothing, IdWidget)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Dim r = dt.Rows(0)
                retVal = New Widget With
                {
                    .IdWidget = Convert.ToInt32(r.Item("IdWidget")),
                    .Abilitato = Convert.ToBoolean(r.Item("Abilitato")),
                    .Visibile = Convert.ToBoolean(r.Item("Visibile")),
                    .RichiedeAziendaSelezionata = Convert.ToBoolean(r.Item("RichiedeAziendaSelezionata")),
                    .Aspetto = IIf(r.Item("Aspetto") Is DBNull.Value, "", r.Item("Aspetto")),
                    .Parametri = IIf(r.Item("Parametri") Is DBNull.Value, "", r.Item("Parametri")),
                    .Codice = IIf(r.Item("Codice") Is DBNull.Value, "", r.Item("Codice")),
                    .Descrizione = IIf(r.Item("Descrizione") Is DBNull.Value, "", r.Item("Descrizione")),
                    .Titolo = IIf(r.Item("Titolo") Is DBNull.Value, "", r.Item("Titolo")),
                    .TipoWidget = IIf(r.Item("TipoWidget") Is DBNull.Value, "", r.Item("TipoWidget")),
                    .PresetIniziale = Convert.ToBoolean(r.Item("PresetIniziale"))
                }
                ''.Permesso = IIf(r.Item("PermessoCod") Is DBNull.Value, -1, Convert.ToInt32(r.Item("PermessoCod")))
            End If
        End If

        Return retVal

    End Function


    Public Function LeggiWidgets(ByVal Codice As String,
                                 ByVal Titolo As String,
                                 ByVal Descrizione As String,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 ByRef objParametriUtenti As AgronicaCoreParametri,
                                 Optional ByVal Visibile As Boolean? = Nothing,
                                 Optional ByVal Abilitato As Boolean? = Nothing,
                                 Optional ByVal PresetIniziale As Boolean? = Nothing,
                                 Optional ByVal idWidget As Integer? = Nothing
                                ) As List(Of Widget)

        Dim retVal As New List(Of Widget)

        Dim objWidgets As New AgronicaCoreMetaSchemaDAL.Widgets_R
        Dim dt As DataTable = Nothing

        If EsisteTabella("Widgets", objParametri) Then
            dt = objWidgets.Leggi(Codice, Titolo, Descrizione,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "", "", objParametri, Visibile, Abilitato, PresetIniziale, idWidget)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                For Each r As DataRow In dt.Rows
                    Dim w As New Widget With
                {
                    .IdWidget = Convert.ToInt32(r.Item("IdWidget")),
                    .Abilitato = Convert.ToBoolean(r.Item("Abilitato")),
                    .Visibile = Convert.ToBoolean(r.Item("Visibile")),
                    .RichiedeAziendaSelezionata = Convert.ToBoolean(r.Item("RichiedeAziendaSelezionata")),
                    .Aspetto = IIf(r.Item("Aspetto") Is DBNull.Value, "", r.Item("Aspetto")),
                    .Parametri = IIf(r.Item("Parametri") Is DBNull.Value, "", r.Item("Parametri")),
                    .Codice = IIf(r.Item("Codice") Is DBNull.Value, "", r.Item("Codice")),
                    .Descrizione = IIf(r.Item("Descrizione") Is DBNull.Value, "", r.Item("Descrizione")),
                    .Titolo = IIf(r.Item("Titolo") Is DBNull.Value, "", r.Item("Titolo")),
                    .TipoWidget = IIf(r.Item("TipoWidget") Is DBNull.Value, "", r.Item("TipoWidget")),
                    .PresetIniziale = Convert.ToBoolean(r.Item("PresetIniziale")),
                    .MultiAziendale = Convert.ToBoolean(r.Item("flagMultiAzienda"))
                }
                    retVal.Add(w)
                Next
            End If
        End If


        Dim linguaCodiceISO As String = "it"
        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri.Lingua_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)
        If Not IsNothing(dtLingua) AndAlso dtLingua.Rows.Count > 0 Then
            linguaCodiceISO = dtLingua.Rows(0)("CodiceISO")
        End If

        Dim ci As CultureInfo = New CultureInfo(linguaCodiceISO)

        For Each w As Widget In retVal
            Dim locTit As String = My.Resources.AgronicaCoreMetaSchemaBIZ.ResourceManager.GetString(w.Titolo, ci)
            Dim locDes As String = My.Resources.AgronicaCoreMetaSchemaBIZ.ResourceManager.GetString(w.Descrizione, ci)
            w.Titolo = If(IsNothing(locTit), "", locTit)
            w.Descrizione = If(IsNothing(locDes), "", locDes)
        Next

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim isSuperUser As Boolean = (objParametri.SuperUserUsername = objParametri.UtenteUsername)

        Try
            ' Controllo dei permessi
            For Each w As Widget In retVal

                If Not isSuperUser Then
                    If Not String.IsNullOrEmpty(w.Parametri) Then
                        Dim pb = JsonConvert.DeserializeObject(Of WidgetParametriBase)(w.Parametri)

                        ' prima controllo se permessi multipli
                        If Not IsNothing(pb) AndAlso Not String.IsNullOrEmpty(pb.ListaCodPermessi) AndAlso Not String.IsNullOrWhiteSpace(pb.ListaCodPermessi) Then
                            Dim codiciPermessi = pb.ListaCodPermessi.Split(",")
                            If codiciPermessi.Count > 0 Then
                                Dim almenoUnoAbilitato As Boolean = False
                                For Each cp In codiciPermessi
                                    If IsNumeric(cp.Trim) Then
                                        almenoUnoAbilitato = ObjUtenti.Controlla_Permessi_Utente(
                                                            objParametriUtenti.UtenteUsername, 5,
                                                            CInt(cp), enum_Security_Operazione.Lettura, Date.Now, "", objParametriUtenti)
                                        If almenoUnoAbilitato Then
                                            Exit For
                                        End If
                                    End If
                                Next
                                w.Abilitato = If(almenoUnoAbilitato, True, False)
                            End If
                        End If

                        ' permesso singolo
                        If Not IsNothing(pb) AndAlso pb.CodPermesso.HasValue AndAlso pb.CodPermesso <> -1 Then
                            Dim abilitatoLettura = ObjUtenti.Controlla_Permessi_Utente(
                            objParametriUtenti.UtenteUsername, 5,
                            pb.CodPermesso, enum_Security_Operazione.Lettura, Date.Now, "", objParametriUtenti)
                            w.Abilitato = abilitatoLettura

                        End If
                    End If
                Else
                    w.Abilitato = True
                End If

            Next
        Catch ex As Exception
            For Each w As Widget In retVal
                w.Abilitato = True
            Next
        End Try

        Return retVal.Where(Function(w) w.Abilitato).ToList()

    End Function

    Private Function EsisteTabella(ByVal nomeTabella As String, ByVal objParametri As AgronicaCoreParametri) As Boolean

        Dim retVal As Boolean
        Dim xExists As New AgronicaCoreMetaSchemaDAL.Widgets_R
        retVal = xExists.EsisteTabella(nomeTabella, objParametri)
        Return retVal

    End Function

End Class
