Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ

Namespace OperazioneAgenda_Temp

    Public Class Movimento_Destinazione
        Implements IEquatable(Of Movimento_Destinazione)

        Sub New(ByVal pivaInput As String, ByVal dataOperazione As Date)
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Data = dataOperazione
            Piva = pivaInput
            Sa_Cod = 0
            Appezza = 0
            Id_Destinazione = 0
            Progetto_Cod = 0
            Tipo = 0
            Qta = 0
            Qta2 = 0
            Tipo_Scorta = 0
            Scorta_Min = 0

            Qta_Dest1 = 0
            Qta_Dest2 = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

            QuotaDistribuzione = 0

            parametroGenerico = ""

            GisWkt = ""
            GisWktSistemaRiferimento = ""
            GisWktGps = ""
            GisTipoEntita_cod = 0
            GisLayerCod = 0

            Sup_Riduzione_BufferZone = 0
            Perc_Riduzione_Deriva = 0

            MagazzinoEsterno_Cod = ""
            MagazzinoEsterno_Des = ""
            MagazzinoEsterno_Dettagli = ""

            Extra_Str = ""
        End Sub

        Sub New()

            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Data = AGRODATAINIZIO
            Piva = ""
            Sa_Cod = 0
            Appezza = 0
            Id_Destinazione = 0
            Progetto_Cod = 0
            Tipo = 0
            Qta = 0
            Qta2 = 0
            Tipo_Scorta = 0
            Scorta_Min = 0

            Qta_Dest1 = 0
            Qta_Dest2 = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

            QuotaDistribuzione = 0

            parametroGenerico = ""

            GisWkt = ""
            GisWktGps = ""
            GisWktSistemaRiferimento = ""
            GisTipoEntita_cod = 0
            GisLayerCod = 0

            Sup_Riduzione_BufferZone = 0
            Perc_Riduzione_Deriva = 0

            MagazzinoEsterno_Cod = ""
            MagazzinoEsterno_Des = ""
            MagazzinoEsterno_Dettagli = ""

            Extra_Str = ""
        End Sub

        Public Overloads Function Equals(other As Movimento_Destinazione) As Boolean Implements IEquatable(Of Movimento_Destinazione).Equals
            If Me.Piva = other.Piva AndAlso
                    Me.Sa_Cod = other.Sa_Cod AndAlso
                    Me.Id_Agenda = other.Id_Agenda AndAlso
                    Me.Id_Mov = other.Id_Mov AndAlso
                    Me.Id_Mov_Det = other.Id_Mov_Det AndAlso
                    Me.Appezza = other.Appezza AndAlso
                    Me.Id_Destinazione = other.Id_Destinazione Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Property GisTipoEntita_cod As Integer
        Public Property GisLayerCod As Integer
        Public Property GisWkt As String

        Public Property GisWktSistemaRiferimento As String

        Public Property GisWktGps As String

        Public Property Piva As String

        Public Property Sa_Cod As Integer

        Public Property Id_Agenda As Integer

        Public Property Id_Mov As Integer

        Public Property Id_Mov_Det As Integer

        Public Property Appezza As Integer

        Public Property Id_Destinazione As Integer

        Public Property Progetto_Cod As Integer

        Public Property Tipo As Integer

        Public Property Data As Date

        Public Property Qta As Decimal

        Public Property Qta2 As Decimal

        Public Property Tipo_Scorta As Integer

        Public Property Scorta_Min As Decimal

        Public Property mov_destinazioni_graphickey As String

        Public Property Qta_Dest1 As Decimal

        Public Property Qta_Dest2 As Decimal

        Public Property QuotaDistribuzione As Decimal

        Public Property parametroGenerico As String

        Public Property Programmazione_Entita_Cod As Integer

        Public Property Data_Creazione As DateTime

        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

        Public Property TopCode As Integer

        Public Property BaseCode As Integer

        Public Property Sup_Riduzione_BufferZone As Decimal

        Public Property Perc_Riduzione_Deriva As Decimal

        Public Property MagazzinoEsterno_Cod As String 'DT: codice agenzia in Ricette (lista di piva-sacod-fabbricatoCod-tipo|piva-sacod-fabbricatoCod-tipo) in cui tipo=1 o non valorizzato--> agenzia, tipo=2 --> uso da terzi

        Public Property MagazzinoEsterno_Des As String 'DT: descrizione agenzia in Ricette (lista di fabbricatoDes|fabbricatoDes)

        Public Property MagazzinoEsterno_Dettagli As String 'DT: dettagli agenzia in Ricette (lista di qta|qta)

        Public Property Extra_Str As String

    End Class

    Public Class Agenda_Movimenti_Destinazioni_Helper

        Public Function Scrivi(ByVal Movimento_Destinazione As Movimento_Destinazione,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Movimento_Destinazione
                '---------------------------

                Dim objMovimentiDestinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_W
                Dim scriviGIS As New GisHelper

                'osservazione fasi fenologiche usa validità_inizio per salvare data rilievo
                Dim validitaIn As Date = AGRODATAINIZIO

                If Movimento_Destinazione.Data <> AGRODATAINIZIO Then
                    validitaIn = Movimento_Destinazione.Data
                End If


                objMovimentiDestinazioni.Scrivi(Movimento_Destinazione.Piva,
                                                Movimento_Destinazione.Sa_Cod,
                                                Movimento_Destinazione.Id_Agenda,
                                                Movimento_Destinazione.Id_Mov,
                                                Movimento_Destinazione.Id_Mov_Det,
                                                Movimento_Destinazione.Appezza,
                                                Movimento_Destinazione.Id_Destinazione,
                                                Movimento_Destinazione.Tipo,
                                                Movimento_Destinazione.Qta,
                                                Movimento_Destinazione.Qta2,
                                                Movimento_Destinazione.Tipo_Scorta,
                                                Movimento_Destinazione.Scorta_Min,
                                                Movimento_Destinazione.mov_destinazioni_graphickey,
                                                Movimento_Destinazione.QuotaDistribuzione,
                                                validitaIn,
                                                AGRODATAFINE,
                                                objParametri,
                                                Qta_Dest1:=Movimento_Destinazione.Qta_Dest1,
                                                Qta_Dest2:=Movimento_Destinazione.Qta_Dest2,
                                                Data_creazione:=Movimento_Destinazione.Data_Creazione,
                                                username_creazione:=Movimento_Destinazione.Username_Creazione,
                                                Sup_Riduzione_BufferZone:=Movimento_Destinazione.Sup_Riduzione_BufferZone,
                                                Perc_Riduzione_Deriva:=Movimento_Destinazione.Perc_Riduzione_Deriva,
                                                Extra_Str:=Movimento_Destinazione.Extra_Str)

                objMovimentiDestinazioni = Nothing

                If Movimento_Destinazione.GisWkt <> "" Then

                    Dim ElmentoGrafico_Des As String = ""
                    '"^Dettaglio§ " & Movimento_Destinazione. & "|"

                    Dim EsitoGis As RispostaStandard =
                        scriviGIS.ScriviDatoCartografico(
                            objParametri,
                            Movimento_Destinazione.Programmazione_Entita_Cod,
                            0,
                            Movimento_Destinazione.Piva,
                            Movimento_Destinazione.Sa_Cod,
                            Movimento_Destinazione.Appezza,
                            Movimento_Destinazione.Id_Destinazione,
                            Movimento_Destinazione.Id_Agenda,
                            Movimento_Destinazione.Id_Mov_Det,
                            Movimento_Destinazione.GisWkt,
                            Movimento_Destinazione.GisWktSistemaRiferimento,
                            Movimento_Destinazione.GisWktGps,
                            Movimento_Destinazione.GisLayerCod,
                            Movimento_Destinazione.GisTipoEntita_cod,
                            0, 0, enum_TipoOperazioneDB.Scrittura, ElmentoGrafico_Des, swapLatLong:=True
                       )

                    If Not EsitoGis.RispostaOK Then
                        Throw New Exception("[ Agenda_Movimenti_Destinazioni_Helper.ScriviDatoCartografico() ] : " & EsitoGis.Errore)
                    End If

                End If



                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Movimenti_Destinazioni_Helper.scrivi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function Cancella(ByVal piva As String,
                                 ByVal saCod As Integer,
                                 ByVal idAgenda As Integer,
                                 ByVal idMov As Integer,
                                 ByVal idMovDet As Integer,
                                 ByVal appezza As Integer,
                                 ByVal idDestinazione As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'GIS
                '---------------------------
                Dim ge As New AgronicaCoreGisDAL.GIS_Entita_R()
                Dim DTGisEntita As DataTable = ge.LeggiDB(objParametri.PivaSuperUser, 0, 0, piva, saCod, appezza, 0, idDestinazione, "", "", "", 0, 0, 0, idAgenda, 0, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri, id_mov_det:=idMovDet)


                If Not IsNothing(DTGisEntita) AndAlso DTGisEntita.Rows.Count > 0 AndAlso IsNumeric(DTGisEntita.Rows(0).Item("Entita_Cod")) AndAlso CInt(DTGisEntita.Rows(0).Item("Entita_Cod")) <> 0 Then

                    Dim geg As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R()
                    Dim DTGisElemGrafici As DataTable = geg.Leggi(objParametri.PivaSuperUser, 0, DTGisEntita.Rows(0).Item("Entita_Cod"), 0, enumFromatoCartograficoConvertito.WKT, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

                    If Not IsNothing(DTGisElemGrafici) AndAlso DTGisElemGrafici.Rows.Count > 0 AndAlso IsNumeric(DTGisElemGrafici.Rows(0).Item("ElementoGrafico_Cod")) AndAlso CInt(DTGisElemGrafici.Rows(0).Item("ElementoGrafico_Cod")) <> 0 Then
                        Dim scriviGIS As New GisHelper
                        Dim EsitoGis As RispostaStandard = scriviGIS.ScriviDatoCartografico(
                                objParametri,
                                0, 0,
                                piva,
                                saCod,
                                appezza,
                                idDestinazione,
                                idAgenda,
                                idMovDet,
                                "", "-1", 0,
                                DTGisElemGrafici.Rows(0).Item("LayerElementiGrafici_Cod"),
                                DTGisEntita.Rows(0).Item("TipoEntita_Cod"),
                                DTGisEntita.Rows(0).Item("Entita_Cod"),
                                DTGisElemGrafici.Rows(0).Item("ElementoGrafico_Cod"),
                                enum_TipoOperazioneDB.Cancellazione
                           )

                        If Not EsitoGis.RispostaOK Then
                            Throw New Exception("[ Agenda_Operazione_Helper.Cancella() ] : " & EsitoGis.Errore)
                        End If

                        scriviGIS = Nothing
                    End If
                End If

                '---------------------------
                'Movimento_Destinazione
                '---------------------------

                Dim objMovimentiDestinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_W

                objMovimentiDestinazioni.Cancella(piva,
                                                  saCod,
                                                  idAgenda,
                                                  idMov,
                                                  idMovDet,
                                                  appezza,
                                                  idDestinazione,
                                                  "",
                                                  objParametri)

                objMovimentiDestinazioni = Nothing


                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Movimenti_Destinazioni_Helper.cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function ModificaPuntuale(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByVal idAgenda As Integer,
                                         ByVal idMov As Integer,
                                         ByVal idMovDet As Integer,
                                         ByVal appezza As Integer,
                                         ByVal idDestinazione As Integer,
                                         ByVal objParametri As AgronicaCoreParametri,
                                         Optional ByVal tipoDestinazione As Integer? = Nothing,
                                         Optional ByVal qta As Decimal? = Nothing,
                                         Optional ByVal qta2 As Decimal? = Nothing,
                                         Optional ByVal data As Date? = Nothing,
                                         Optional ByVal tipoScorta As Integer? = Nothing,
                                         Optional ByVal scortaMin As Decimal? = Nothing,
                                         Optional ByVal movDestinazioniGrapichkey As String = Nothing,
                                         Optional ByVal qtaDest1 As Decimal? = Nothing,
                                         Optional ByVal qtaDest2 As Decimal? = Nothing,
                                         Optional ByVal quotaDistribuzione As Decimal? = Nothing,
                                         Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                         Optional ByVal usernameModifica As String = "",
                                         Optional ByVal Sup_Riduzione_BufferZone As Decimal? = Nothing,
                                         Optional ByVal Perc_Riduzione_Deriva As Decimal? = Nothing,
                                         Optional ByVal Extra_Str As String = ""
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


                Dim objMovDestW As New AgronicaCoreContabDAL.Mov_Destinazioni_W

                xRisp = objMovDestW.ModificaPuntuale(piva, saCod, idAgenda, idMov, idMovDet, appezza, idDestinazione, objParametri,
                                                     Tipo_Destinazione:=tipoDestinazione,
                                                     Qta:=qta,
                                                     Qta2:=qta2,
                                                     Validita_Inizio:=data,
                                                     Tipo_Scorta:=tipoScorta,
                                                     Scorta_Min:=scortaMin,
                                                     Mov_Destinazioni_Graphickey:=movDestinazioniGrapichkey,
                                                     Qta_Dest1:=qtaDest1,
                                                     Qta_Dest2:=qtaDest2,
                                                     QuotaDistribuzione:=quotaDistribuzione,
                                                     Data_Modifica:=dataModifica,
                                                     Username_Modifica:=usernameModifica,
                                                    Sup_Riduzione_BufferZone:=Sup_Riduzione_BufferZone,
                                                    Perc_Riduzione_Deriva:=Perc_Riduzione_Deriva,
                                                    Extra_Str:=Extra_Str)

                objMovDestW = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Movimenti_Destinazioni_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        Public Function ModificaPuntuale(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByVal idAgenda As Integer,
                                         ByVal idMov As Integer,
                                         ByVal idMovDet As Integer,
                                         ByVal appezza As Integer,
                                         ByVal idDestinazione As Integer,
                                         ByVal objParametri As AgronicaCoreParametri,
                                         ByVal Movimento_Destinazione As Movimento_Destinazione
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


                Dim objMovDestW As New AgronicaCoreContabDAL.Mov_Destinazioni_W

                xRisp = objMovDestW.ModificaPuntuale(piva, saCod, idAgenda, idMov, idMovDet, appezza, idDestinazione, objParametri,
                                                     Tipo_Destinazione:=Movimento_Destinazione.Tipo,
                                                     Qta:=Movimento_Destinazione.Qta,
                                                     Qta2:=Movimento_Destinazione.Qta2,
                                                     Validita_Inizio:=Movimento_Destinazione.Data,
                                                     Tipo_Scorta:=Movimento_Destinazione.Tipo_Scorta,
                                                     Scorta_Min:=Movimento_Destinazione.Scorta_Min,
                                                     Mov_Destinazioni_Graphickey:=Movimento_Destinazione.mov_destinazioni_graphickey,
                                                     Qta_Dest1:=Movimento_Destinazione.Qta_Dest1,
                                                     Qta_Dest2:=Movimento_Destinazione.Qta_Dest2,
                                                     QuotaDistribuzione:=Movimento_Destinazione.QuotaDistribuzione,
                                                     Data_Modifica:=Movimento_Destinazione.Data_Modifica,
                                                     Username_Modifica:=Movimento_Destinazione.Username_Creazione)

                objMovDestW = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Movimenti_Destinazioni_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        Public Function Leggi(ByVal piva As String,
                              ByVal saCod As Integer,
                              ByVal idAgenda As Integer,
                              ByVal idMov As Integer,
                              ByVal idMovDet As Integer,
                              ByVal objParametri As AgronicaCoreParametri
                              ) As List(Of Movimento_Destinazione)

            Dim flagConnessione As Boolean = False

            Dim listaMovimentiDestinazioni As New List(Of Movimento_Destinazione)
            Dim Movimento_Destinazione As Movimento_Destinazione

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MOVIMENTI_DESTINAZIONI ------------------------
                '--------------------------------------------------------
                Dim objMovDestinazioni = New AgronicaCoreContabDAL.Mov_Destinazioni_R
                Dim dtMovDestinazioni As DataTable

                dtMovDestinazioni = objMovDestinazioni.Leggi(CStr(piva),
                                                             CInt(saCod),
                                                             CInt(idAgenda),
                                                             idMov,
                                                             idMovDet,
                                                             0,
                                                             0,
                                                             0,
                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                             "",
                                                             "",
                                                             objParametri)

                objMovDestinazioni = Nothing

                Dim sLista1 =
              (From s In dtMovDestinazioni.AsEnumerable
                        Where s("appezza") IsNot Nothing AndAlso Not IsDBNull(s("appezza")) AndAlso s("appezza") > 0
               Select " (e.piva = '" & s("Piva") & "' AND e.sa_cod = " & s("Sa_Cod") & " AND e.Appezza = " & s("appezza") & " AND e.id_imp = " & s("Id_Destinazione") & " AND e.id_agenda = " & s("Id_agenda") & " AND e.Id_Mov_Det = " & s("Id_Mov_Det") & ")"
               ).ToList()

                Dim filtroListaSQL As String = String.Join(" OR ", sLista1.ToArray)
                Dim letturaDatiGIS As New AgronicaCoreGisDAL.GIS_Entita_R
                Dim rvalImpiantiLetti As DataTable
                If sLista1.Count > 0 Then
                    rvalImpiantiLetti = letturaDatiGIS.AgendaLetturaWKT_DaListaImpianti(" (" & filtroListaSQL & ")", enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, enum_Gis_TipoOggetto.NonSpecificato, objParametri)
                End If



                If dtMovDestinazioni.Rows.Count > 0 Then

                    For j = 0 To dtMovDestinazioni.Rows.Count - 1

                        Movimento_Destinazione = New Movimento_Destinazione With {
                            .Piva = dtMovDestinazioni.Rows(j).Item("Piva"),
                            .Sa_Cod = dtMovDestinazioni.Rows(j).Item("Sa_Cod"),
                            .Id_Agenda = dtMovDestinazioni.Rows(j).Item("Id_Agenda"),
                            .Id_Mov = dtMovDestinazioni.Rows(j).Item("Id_Mov"),
                            .Id_Mov_Det = dtMovDestinazioni.Rows(j).Item("Id_Mov_Det"),
                            .Appezza = dtMovDestinazioni.Rows(j).Item("Appezza"),
                            .Id_Destinazione = dtMovDestinazioni.Rows(j).Item("Id_Destinazione"),
                            .Progetto_Cod = dtMovDestinazioni.Rows(j).Item("Progetto_Cod"),
                            .Tipo = dtMovDestinazioni.Rows(j).Item("Tipo_Destinazione"),
                            .Qta = dtMovDestinazioni.Rows(j).Item("Qta"),
                            .Qta2 = dtMovDestinazioni.Rows(j).Item("Qta2"),
                            .Tipo_Scorta = dtMovDestinazioni.Rows(j).Item("Tipo_Scorta"),
                            .Scorta_Min = dtMovDestinazioni.Rows(j).Item("Scorta_Min"),
                            .mov_destinazioni_graphickey = dtMovDestinazioni.Rows(j).Item("mov_destinazioni_graphickey"),
                            .Data = dtMovDestinazioni.Rows(j).Item("Validita_inizio"),
                            .Data_Creazione = CDate(dtMovDestinazioni.Rows(j).Item("Data_Creazione")),
                            .Data_Modifica = CDate(dtMovDestinazioni.Rows(j).Item("Data_Modifica")),
                            .Username_Creazione = dtMovDestinazioni.Rows(j).Item("Username_Creazione"),
                            .Username_Modifica = dtMovDestinazioni.Rows(j).Item("Username_Modifica")
                        }

                        If Not IsNothing(dtMovDestinazioni.Rows(j).Item("Qta_Dest1")) AndAlso
                           Not IsDBNull(dtMovDestinazioni.Rows(j).Item("Qta_Dest1")) Then
                            Movimento_Destinazione.Qta_Dest1 = dtMovDestinazioni.Rows(j).Item("Qta_Dest1")
                        End If

                        If Not IsNothing(dtMovDestinazioni.Rows(j).Item("Qta_Dest2")) AndAlso
                           Not IsDBNull(dtMovDestinazioni.Rows(j).Item("Qta_Dest2")) Then
                            Movimento_Destinazione.Qta_Dest2 = dtMovDestinazioni.Rows(j).Item("Qta_Dest2")
                        End If

                        If Not IsNothing(dtMovDestinazioni.Rows(j).Item("QuotaDistribuzione")) AndAlso
                           Not IsDBNull(dtMovDestinazioni.Rows(j).Item("QuotaDistribuzione")) Then
                            Movimento_Destinazione.QuotaDistribuzione = dtMovDestinazioni.Rows(j).Item("QuotaDistribuzione")
                        End If

                        If Not IsNothing(dtMovDestinazioni.Rows(j).Item("Sup_Riduzione_BufferZone")) AndAlso
                           Not IsDBNull(dtMovDestinazioni.Rows(j).Item("Sup_Riduzione_BufferZone")) Then
                            Movimento_Destinazione.Sup_Riduzione_BufferZone = dtMovDestinazioni.Rows(j).Item("Sup_Riduzione_BufferZone")
                        End If

                        If Not IsNothing(dtMovDestinazioni.Rows(j).Item("Perc_Riduzione_Deriva")) AndAlso
                           Not IsDBNull(dtMovDestinazioni.Rows(j).Item("Perc_Riduzione_Deriva")) Then
                            Movimento_Destinazione.Perc_Riduzione_Deriva = dtMovDestinazioni.Rows(j).Item("Perc_Riduzione_Deriva")
                        End If

                        If Not rvalImpiantiLetti Is Nothing Then
                            Dim Dr() As DataRow = rvalImpiantiLetti.Select("piva = '" & dtMovDestinazioni.Rows(j).Item("Piva") & "' AND sa_cod = " & dtMovDestinazioni.Rows(j).Item("Sa_Cod") & " AND Appezza = " & dtMovDestinazioni.Rows(j).Item("Appezza") & " AND id_imp = " & dtMovDestinazioni.Rows(j).Item("Id_Destinazione") & " AND id_agenda = " & dtMovDestinazioni.Rows(j).Item("Id_agenda") & " AND Id_Mov_Det = " & dtMovDestinazioni.Rows(j).Item("Id_Mov_Det"))
                            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                                Movimento_Destinazione.GisWkt = Dr(0).Item("Wkt")
                                Movimento_Destinazione.GisWktGps = Dr(0).Item("flag_Gps")
                                Movimento_Destinazione.GisWktSistemaRiferimento = Dr(0).Item("GisWktSistemaRiferimento")
                                Movimento_Destinazione.GisTipoEntita_cod = Dr(0).Item("GisTipoEntita_cod")
                                Movimento_Destinazione.GisLayerCod = Dr(0).Item("GisLayerCod")
                            End If
                        End If

                        If Not IsNothing(dtMovDestinazioni.Rows(j).Item("Extra_Str")) AndAlso
                           Not IsDBNull(dtMovDestinazioni.Rows(j).Item("Extra_Str")) Then
                            Movimento_Destinazione.Extra_Str = dtMovDestinazioni.Rows(j).Item("Extra_Str")
                        End If

                        listaMovimentiDestinazioni.Add(Movimento_Destinazione)


                    Next




                End If

            Catch ex As Exception

                Throw New Exception("[ Agenda_Movimenti_Destinazioni_Helper.leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaMovimentiDestinazioni

        End Function

    End Class

End Namespace