Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.Identity

Public Class Interferenze

    ''' <summary>
    ''' Gestione delle interferenze per il poligono
    ''' </summary>
    ''' <param name="TipoOperazioneDB"></param>
    ''' <param name="Entita_Cod_DaVerificare"></param>
    ''' <param name="HiddenPuntiModifica"></param>
    ''' <param name="DatiPassaggio"></param>
    ''' <param name="pivaSementieroReferente"></param>
    ''' <param name="specie"></param>
    ''' <param name="tipologia"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <param name="objParametri_Server"></param>
    Public Sub InterferenzeGestione(
        ByVal TipoOperazioneDB As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
        ByVal Entita_Cod_DaVerificare As Integer,
        ByVal HiddenPunti As String,
        ByVal DatiPassaggio As String,
        ByVal pivaSementieroReferente As String,
        ByVal specie As Integer,
        ByVal tipologia As Integer,
        ByVal Data_Inizio As Date,
        ByVal Data_Fine As Date,
        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)


        'metto tutte le interferenze a stato 

        If DatiPassaggio.Split("|")(5) = 1 Then

            Dim objInter_W As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_W
            Dim objIntR As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_R

            'Gabriele: suppongo che il poligono non sia più in interferenza con nessuno...
            GeneraDescrizione_Casella_Conflitto(Entita_Cod_DaVerificare, objParametri_Server, objParametri_Utenti)
            'al posto di:
            'objInter_W.Modifica_Tutti_Interferenti(entita_cod, objParametri_Server)

            'VerificaInterferenze = True
            'Dim risposta As String
            Dim dtInterferenze As DataTable
            Dim numeroRecordDaRestituire As Integer = -1
            Dim moltiplicatoreDistanze As Integer = 1

            dtInterferenze = dtInterferenze_from_poligono(pivaSementieroReferente, specie, tipologia,
                                                              Data_Inizio, Data_Fine,
                                                              moltiplicatoreDistanze, False,
                                                              numeroRecordDaRestituire,
                                                              HiddenPunti, objParametri_Server, objParametri_Utenti)

            If dtInterferenze.Rows.Count > 0 Then


                For Each drowRisp As DataRow In dtInterferenze.Rows

                    If TipoOperazioneDB = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then


                        Dim entita_cod_proprietario As Integer
                        entita_cod_proprietario = drowRisp("Entita_Cod")
                        Dim Interferenze_Cod As Integer

                        Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                        Interferenze_Cod = AgroSequenze.NuovoId_Tabella("Sementieri_Sportello_InterferenzePerConferma", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                        'Interferenze_Cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
                        '                "Sementieri_Sportello_InterferenzePerConferma",
                        '                objParametri_Server
                        '                )

                        objInter_W.Scrivi(Interferenze_Cod, entita_cod_proprietario, Entita_Cod_DaVerificare, True, drowRisp("DistanzaEffettiva"), objParametri_Server)


                    Else


                        Dim entita_cod_proprietario As Integer
                        entita_cod_proprietario = drowRisp("Entita_Cod")

                        'se è già stata confermata 
                        Dim dt_gia_Inseriti As DataTable
                        dt_gia_Inseriti = objIntR.Leggi_da_Propietario_Interferente(entita_cod_proprietario, Entita_Cod_DaVerificare, objParametri_Server)

                        If dt_gia_Inseriti.Rows.Count = 0 Then
                            Dim Interferenze_Cod As Integer

                            Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                            'Interferenze_Cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("Sementieri_Sportello_InterferenzePerConferma", objParametri_Server)
                            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                            Interferenze_Cod = AgroSequenze.NuovoId_Tabella("Sementieri_Sportello_InterferenzePerConferma", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                            objInter_W.Scrivi(Interferenze_Cod, entita_cod_proprietario, Entita_Cod_DaVerificare, True, drowRisp("DistanzaEffettiva"), objParametri_Server)
                        Else
                            'modifico e imposto a true 
                            objInter_W.Modifica_Flag(dt_gia_Inseriti.Rows(0).Item("Interferenze_Cod"), True, True, objParametri_Server)
                        End If


                    End If 'modifica o nuovo inserimento

                Next 'interferenza


            End If 'if interferenze presenti

        End If 'Sportello nella fase giusta



    End Sub



    Public Function dtInterferenze_from_poligono(ByVal pivaSementieroReferente As String,
                                                 ByVal veg_cod As Integer,
                                                 ByVal grva_cod As Integer,
                                                 ByVal data_inizio As Date,
                                                 ByVal data_fine As Date,
                                                 ByVal moltiplicatoreDistanze As Integer,
                                                 ByVal Flag_InterferenzeStessoReferente As Boolean,
                                                 ByVal numeroRecordDaRestituire As Integer,
                                                 ByVal HiddenPunti As String,
                                                 ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim gmlPol As String
        If Not HiddenPunti.Contains("gml") Then
            gmlPol = GetGmlPol(HiddenPunti)
        Else
            gmlPol = HiddenPunti
        End If

        Dim verificatore As New AgronicaCoreGisDAL.GIS_OperazioniCartograficheDB
        Dim dtInterferenze As DataTable = verificatore.ElencoImpiantiInterferenzaSementi(
            pivaSementieroReferente,
            gmlPol,
            veg_cod,
            grva_cod,
            Math.Abs(CInt(grva_cod < 0)),
            data_inizio,
            data_fine,
            Flag_InterferenzeStessoReferente,
            moltiplicatoreDistanze,
            numeroRecordDaRestituire,
            True,
            objParametri_Server,
            objParametri_Utenti)

        Return dtInterferenze
    End Function



    Private Shared Function GetGmlPol(ByVal hiddenPunti_Nuovo As String) As String
        Dim gmlPol As String
        Dim coord As String
        coord = GetCoord(hiddenPunti_Nuovo)

        Dim wktToGeoML As New AgronicaConversioneCartografiaGias.FormatsConverter.wkt_gml

        gmlPol = wktToGeoML.Trasforma(
                                coord,
                                False,
                                False,
                                False,
                                0
                            )
        gmlPol = gmlPol.Replace("<geodata ElementoGrafico_Cod=""0"" xmlns=""http://www.agronica.it/grafica/"">", "").Replace("</geodata>", "")
        gmlPol = gmlPol.Substring(gmlPol.IndexOf("<"), gmlPol.Length - (gmlPol.IndexOf("<") + 2))
        Return gmlPol
    End Function


    Private Shared Function GetCoord(ByVal hiddenPunti_Nuovo As String) As String
        Dim coord As String
        Dim app_coor = Replace(hiddenPunti_Nuovo, "(", "")
        app_coor = Replace(app_coor, ",", " ")
        app_coor = app_coor.Trim
        app_coor = Replace(app_coor, "   ", " ")
        app_coor = Replace(app_coor, "  ", " ")

        app_coor = Replace(app_coor, ") ", ",")
        app_coor = Replace(app_coor, ")", "")

        'app_coor = app_coor + " " + app_coor.Split(" ")(0) + " " + app_coor.Split(" ")(1)

        Dim TipoOggetto As String = "POLYGON"
        If hiddenPunti_Nuovo.Split(",").Count = 2 Then
            TipoOggetto = "POINT"

        End If
        If hiddenPunti_Nuovo.Split(",").Count = 4 Then
            TipoOggetto = "LINESTRING"

        End If
        coord = TipoOggetto & " ((" & app_coor & "))"
        Return coord

    End Function


    Public Shared Sub GeneraDescrizione_Casella_Conflitto(ByVal Entita_Cod As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Dim objConflitti As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_W
        'objConflitti.Modifica_Flag_Entita_Cod_Interferente(Entita_Cod, False, objParametri_Server)
        'objConflitti.Modifica_Flag_Entita_Cod_Propietario(Entita_Cod, False, objParametri_Server)

        'leggo tutto 
        Dim objR As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_R
        Dim dt = objR.Leggi(Entita_Cod, False, "", True, 0, False, objParametri_Server, objParametri_Utenti)
        ''Dim dr() As DataRow

        ''Dim dr() As DataRow

        'dt = objR.Leggi(0, False, "", 0,
        '                "AND (Flag_Attivo = 0 AND ( Entita_Cod_Propietario = " & Entita_Cod & " OR Entita_Cod_Interferente = " & Entita_Cod & "))",
        '                "", objParametri_Server)
        ''ho tutto leggo solo ciò che è ancora stato = 0 
        ''dr = dt.Select("Flag_Attivo = 0 AND ( Entita_Cod_Propietario = " & Entita_Cod & " OR Entita_Cod_Interferente = " & Entita_Cod & ")")

        Dim aggiorna As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_W

        For Each riga As DataRow In dt.Rows 'dr
            aggiorna.ScriviInCache(CInt(riga("interferenze_cod")), 0, riga, objParametri_Server)
        Next

    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="VegCod"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <returns></returns>
    Public Function LeggiSportelloAttivoDataSpecieIntervalloData(ByVal VegCod As Integer, ByVal Data_Inizio As Date, ByVal Data_Fine As Date, ByVal objParametri_Server As AgronicaCoreParametri) As List(Of String)

        Dim rval As New List(Of String)

        Dim leggiSementi As New AgronicaCoreSementieriDAL.Sportello_R

        Dim dt As DataTable = leggiSementi.Leggi(
            "",
            VegCod,
            0,
            0,
            Data_Inizio,
            Data_Fine,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            objParametri_Server
        )

        Dim primoElemento As Boolean = True
        For Each drow As DataRow In dt.Rows

            Dim lDes As String
            If Not Data_Fine = CostantiPersonalizzate.AGRODATAFINE Then
                lDes = drow("Sementieri_Sportello_Configurazione_des")
            Else
                lDes = drow("Sementieri_Sportello_Configurazione_des") & " - " & drow("Sementieri_Sportello_Passaggi_des")
            End If

            rval.Add(
                drow("Sementieri_Sportello_Passaggi_cod") & "|" &
                drow("data_inizio") & "|" &
                drow("data_fine") & "|" &
                drow("visibilita_impianti") & "|" &
                drow("DestinazioneSalvataggio") & "|" &
                drow("RichiediConfermaSuInterferenze") & "|" &
                drow("LoggaOperazioni") & "|" &
                drow("VisualizzaOperazioniLoggate")
            )

        Next

        Return rval

    End Function

End Class
