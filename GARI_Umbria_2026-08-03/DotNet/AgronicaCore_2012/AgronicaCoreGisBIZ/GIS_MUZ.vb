Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis.MUZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.Gis.MUZ

Public Class GIS_MUZ_R
    Public Function LeggiDatiMUZVisibili(ByVal piva As String,
                                         ByVal lista_MUZ_Cod As List(Of Int32),
                                         ByRef objParametri_server As AgronicaCoreParametri) As List(Of DatiMUZVisibili_Out)

        Dim resp As New List(Of DatiMUZVisibili_Out)

        Dim xRead As New AgronicaCoreGisDAL.GIS_MUZ_R

        Dim DT As DataTable = xRead.LeggiDatiMUZVisibili(piva, lista_MUZ_Cod, objParametri_server)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Throw New Exception("Errore nel reperimento delle informazioni delle MUZ.")
        End If

        Dim MUZPrecedente As Int32 = 0
        Dim datiMuz = Nothing

        For Each row In DT.Rows

            If MUZPrecedente <> CInt(row("Area_Cod")) Then

                If datiMuz IsNot Nothing Then
                    resp.Add(datiMuz)
                End If

                MUZPrecedente = CInt(row("Area_Cod"))
                datiMuz = New DatiMUZVisibili_Out With {
                    .Area_Cod = CInt(row("Area_Cod")),
                    .MUZ_Appezzamenti = New List(Of MUZ_Appezzamento),
                    .Gruppo = New GruppoAreaOmogenea With {
                        .Gruppo_Area_Cod = CInt(row("Gruppo_Area_Cod")),
                        .Gruppo_Area_Des = row("Gruppo_Area_Des").ToString
                    }
                }
            End If

            datiMuz.MUZ_Appezzamenti.Add(New MUZ_Appezzamento With {
                .Piva = row("Piva").ToString,
                .Sa_Cod = CInt(row("Sa_Cod")),
                .Appezza = CInt(row("Appezza"))
            })
        Next

        resp.Add(datiMuz)

        Return resp

    End Function

    Public Function LeggiListaGruppiMUZ(ByVal piva As String,
                                        ByRef objParametri_server As AgronicaCoreParametri) As List(Of GruppoAreaOmogenea)

        Dim resp As New List(Of GruppoAreaOmogenea)

        Dim xRead As New AgronicaCoreGisDAL.GIS_MUZ_R

        Dim DT As DataTable = xRead.LeggiListaGruppiMUZ(piva, objParametri_server)

        If DT Is Nothing Then
            Throw New Exception("Errore nel reperimento delle informazioni dei gruppi delle MUZ.")
        End If

        For Each row In DT.Rows

            Dim gruppo As New GruppoAreaOmogenea With {
                .Piva = row("Piva").ToString,
                .Gruppo_Area_Cod = CInt(row("Gruppo_Area_Cod")),
                .Gruppo_Area_Des = row("Gruppo_Area_Des").ToString
            }

            resp.Add(gruppo)

        Next

        Return resp

    End Function

    Public Function LeggiMUZ(ByVal area_Cod As Int32,
                             ByVal piva As String,
                             ByRef objParametri_Server As AgronicaCoreParametri) As List(Of MUZ)

        Dim resp As New List(Of MUZ)

        Dim xRead As New AgronicaCoreGisDAL.GIS_MUZ_R

        Dim DT As DataTable = xRead.LeggiMUZ(area_Cod, piva, objParametri_Server)

        If DT Is Nothing Then
            Throw New Exception("Errore nella lettura dei dati delle MUZ")
        End If

        Dim muz As MUZ = Nothing
        Dim lastMUZ As Int32 = 0

        For Each row In DT.Rows

            If CInt(row("Area_Cod")) <> lastMUZ Then
                lastMUZ = CInt(row("Area_Cod"))

                If muz IsNot Nothing Then
                    resp.Add(muz)
                End If

                muz = New MUZ With {
                .Area_Cod = CInt(row("Area_Cod")),
                .Area_Des = row("Area_Des").ToString,
                .Altimetria = row("Altimetria").ToString,
                .Gruppo_Area_Cod = CInt(row("Gruppo_Area_Cod")),
                .Gruppo_Area_Des = row("Gruppo_Area_Des").ToString,
                .Piva = row("Piva").ToString,
                .SO = CDbl(row("SO")),
                .Tessitura_cod = CInt(row("Tessitura_Cod")),
                .TipoZona = row("TipoZona").ToString,
                .Validita_Inizio = CDate(row("Validita_Inizio")),
                .Validita_Fine = CDate(row("Validita_Fine")),
                .Analisi_Testate_Aggiungi = New List(Of Int32),
                .Analisi_Testate_Elimina = New List(Of Int32),
                .appezzamento = New List(Of Appezzamento_MUZ),
                .Particelle_Catastali_Aggiungi = New List(Of ParticelleCatastali_MUZ),
                .Particelle_Catastali_Elimina = New List(Of ParticelleCatastali_MUZ),
                .Analisi_Testate = New List(Of AnalisiTestate_MUZ),
                .Particelle_Catastali = New List(Of ParticelleCatastali_MUZ)
                }
            End If

            Dim appezzamento As New Appezzamento_MUZ With {
                .Piva = row("Piva").ToString,
                .Sa_Cod = CInt(row("Sa_Cod")),
                .Appezza = CInt(row("Appezza"))
            }

        Next

        resp.Add(muz)

        For Each muz In resp
            Dim DT_Catasto As DataTable = xRead.LeggiParticelleCatastaliDaMUZ(muz.Area_Cod, objParametri_Server)

            If DT_Catasto IsNot Nothing Then
                For Each row In DT_Catasto.Rows
                    Dim validitaInizio = CostantiPersonalizzate.AGRODATAINIZIO
                    Dim validitaFine = CostantiPersonalizzate.AGRODATAFINE

                    If Not IsDBNull(row("Validita_Inizio")) Then validitaInizio = CDate(row("Validita_Inizio"))
                    If Not IsDBNull(row("Validita_Fine")) Then validitaFine = CDate(row("Validita_Fine"))

                    Dim particella As New ParticelleCatastali_MUZ With {
                        .Comune = row("COM").ToString,
                        .Foglio = CInt(row("FOGLIO")),
                        .Numero = CInt(row("NUMERO")),
                        .Provincia = row("PROV").ToString,
                        .Sezione = IIf(row("SEZIONE").ToString.Equals("0"), "", row("SEZIONE").ToString),
                        .Subalterno = IIf(row("SUBALTERNO").ToString.Equals("0"), "", row("SUBALTERNO").ToString),
                        .Sup_Condotta = CDbl(row("Sup_Condotta")),
                        .Titolo_Possesso = CInt(row("TitoloPossesso")),
                        .Condotta_Validita_Inizio = validitaInizio,
                        .Condotta_Validita_Fine = validitaFine,
                        .Comune_Esteso = row("LOCALITA").ToString,
                        .Provincia_Esteso = row("PROVINCIA").ToString
                    }

                    muz.Particelle_Catastali.Add(particella)
                Next
            End If

            Dim DT_Testate As DataTable = xRead.LeggiAnalisiTestateDaMUZ(muz.Area_Cod, objParametri_Server)

            If DT_Testate IsNot Nothing Then
                For Each row In DT_Testate.Rows
                    Dim analisi As New AnalisiTestate_MUZ With {
                        .Analisi_Testata_Cod = CInt(row("Analisi_Testata_Cod")),
                        .Analisi_Testata_Des = row("Analisi_Testata_Des")
                    }

                    muz.Analisi_Testate.Add(analisi)
                Next
            End If
        Next

        Return resp

    End Function
End Class
Public Class GIS_MUZ_W
    Public Function EseguiOperazioneGruppiMUZ(ByVal inData As OperazioniGruppiMUZ_In,
                                              ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Select Case inData.operazione
            Case enum_Tipo_OperazioneGruppi.AGGIUNGIAGRUPPO
                Return AggiungiMUZAGruppo(inData.Area_Cod, inData.Gruppo_Area_Cod_Esistente, objParametri_Server)
            Case enum_Tipo_OperazioneGruppi.CLONAGRUPPO
                Return ClonaGruppoMUZ(inData, objParametri_Server)
            Case enum_Tipo_OperazioneGruppi.COPIAAPPEZZAMENTO
                Return CopiaAppezzamento(inData, objParametri_Server)
            Case enum_Tipo_OperazioneGruppi.CREAGRUPPOVUOTO
                Return CreaGruppoVuoto(inData.New_Gruppo_Area_Des, inData.Piva, objParametri_Server)
            Case Else
                Throw New Exception("Operazione non consentita")
        End Select

    End Function

    Private Function CreaGruppoVuoto(ByVal new_Gruppo_Area_Des As String,
                                     ByVal piva As String,
                                     ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim res As Boolean

        Dim xWriteMUZ As New AgronicaCoreGisDAL.GIS_MUZ_W

        Dim sequenza As New Agro_Sequenze

        Dim Gruppo_Cod = sequenza.NuovoId_Tabella("Gruppo_Area_Omogenea", 0, Int32.MaxValue, objParametri_Server, True)

        res = xWriteMUZ.ScriviGruppo(new_Gruppo_Area_Des, piva, objParametri_Server, Gruppo_Cod)

        If Not res Then
            Throw New Exception("Errore in creazione del nuovo gruppo MUZ.")
        End If

        Return res
    End Function

    Private Function CopiaAppezzamento(ByVal inData As OperazioniGruppiMUZ_In,
                                       ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xRead As New AgronicaCoreGisDAL.GIS_MUZ_R
        Dim xWriteMUZ As New AgronicaCoreGisDAL.GIS_MUZ_W
        Dim xWriteElementiGrafici As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim res As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim DT As DataTable = xRead.LeggiMUZDaGruppo(inData.Gruppo_Area_Cod_Esistente, objParametri_Server)

            If DT Is Nothing Then
                Throw New Exception("Errore nella lettura del contenuto del gruppo.")
            End If

            For Each row In DT.Rows
                Dim nuovo_Area_Cod As Int32 = xWriteMUZ.ClonaMUZ(CInt(row("Area_Cod")), objParametri_Server)
                Dim nuovo_Gis_Entita As Int32 = xWriteMUZ.ClonaGisEntita(nuovo_Area_Cod, CInt(row("Area_Cod")), objParametri_Server)

                Dim ElementoGrafico_Cod As Int32 = xRead.leggiElementoGraficoDaMUZ(CInt(row("Area_Cod")), objParametri_Server)

                res = xWriteElementiGrafici.ClonaElementoGrafico(ElementoGrafico_Cod, nuovo_Gis_Entita, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nella clonazione dell'elemento grafico.")
                End If

                res = xWriteMUZ.AssociaMUZAdAppezzamento(inData.Piva, inData.Sa_Cod, inData.Appezza, nuovo_Area_Cod, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione del gruppo all'appezzamento.")
                End If

                res = ClonaAssociazioniMUZ(CInt(row("Area_Cod")), nuovo_Area_Cod, xRead, xWriteMUZ, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nella clonazione delle associazioni della MUZ.")
                End If

            Next

            res = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception

            res = False

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return res

    End Function

    Private Function ClonaAssociazioniMUZ(ByVal old_Area_Cod As Int32,
                                          ByVal nuovo_Area_Cod As Int32,
                                          ByRef xRead As AgronicaCoreGisDAL.GIS_MUZ_R,
                                          ByRef xWriteMUZ As AgronicaCoreGisDAL.GIS_MUZ_W,
                                          ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim res As Boolean

        Dim DT_Particelle = xRead.LeggiParticelleCatastaliDaMUZ(old_Area_Cod, objParametri_Server)

        If DT_Particelle IsNot Nothing Then
            For Each particella_row In DT_Particelle.Rows

                Dim particella As New ParticelleCatastali_MUZ With {
                            .Provincia = particella_row("PROV").ToString,
                            .Comune = particella_row("COM").ToString,
                            .Sezione = IIf(particella_row("SEZIONE").ToString.Equals("0"), "", particella_row("SEZIONE").ToString),
                            .Foglio = CInt(particella_row("FOGLIO")),
                            .Numero = CInt(particella_row("NUMERO")),
                            .Subalterno = IIf(particella_row("SUBALTERNO").ToString.Equals("0"), "", particella_row("SUBALTERNO").ToString)
                        }

                res = xWriteMUZ.AssociaMUZAParticellaCatastale(nuovo_Area_Cod, particella, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione della MUZ alla particella catastale.")
                End If

            Next
        End If

        Dim DT_Analisi = xRead.LeggiAnalisiTestateDaMUZ(old_Area_Cod, objParametri_Server)

        If DT_Analisi IsNot Nothing Then
            For Each analisi_row In DT_Analisi.Rows

                res = xWriteMUZ.AssociaMUZAdAnalisi(nuovo_Area_Cod, CInt(analisi_row("Analisi_Testata_Cod")), objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione della MUZ alla testata dell'analisi.")
                End If
            Next
        End If

        Return True

    End Function

    Private Function ClonaGruppoMUZ(ByVal inData As OperazioniGruppiMUZ_In,
                                    ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xWriteMUZ As New AgronicaCoreGisDAL.GIS_MUZ_W
        Dim xWriteElementiGrafici As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
        Dim xRead As New AgronicaCoreGisDAL.GIS_MUZ_R

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim res As Boolean = False

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim sequenza As New Agro_Sequenze

            Dim Gruppo_Cod = sequenza.NuovoId_Tabella("Gruppo_Area_Omogenea", 0, Int32.MaxValue, objParametri_Server, True)

            res = xWriteMUZ.ScriviGruppo(inData.New_Gruppo_Area_Des, inData.Piva, objParametri_Server, Gruppo_Cod)

            If Not res Then
                Throw New Exception("Errore in creazione del nuovo gruppo MUZ.")
            End If

            Dim DT As DataTable = xRead.LeggiMUZDaGruppo(inData.Gruppo_Area_Cod_Esistente, objParametri_Server)

            If DT Is Nothing Then
                Throw New Exception("Errore nella lettura del contenuto del gruppo.")
            End If

            For Each row In DT.Rows
                Dim nuovo_Area_Cod As Int32 = xWriteMUZ.ClonaMUZ(CInt(row("Area_Cod")), objParametri_Server)
                Dim nuovo_Gis_Entita As Int32 = xWriteMUZ.ClonaGisEntita(nuovo_Area_Cod, CInt(row("Area_Cod")), objParametri_Server)

                Dim ElementoGrafico_Cod As Int32 = xRead.leggiElementoGraficoDaMUZ(CInt(row("Area_Cod")), objParametri_Server)

                res = xWriteElementiGrafici.ClonaElementoGrafico(ElementoGrafico_Cod, nuovo_Gis_Entita, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nella clonazione dell'elemento grafico.")
                End If

                res = xWriteMUZ.AggiungiMUZAGruppo(nuovo_Area_Cod, Gruppo_Cod, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'aggiunta della MUZ al gruppo.")
                End If

                res = xWriteMUZ.AssociaMUZAdAppezzamento(inData.Piva, inData.Sa_Cod, inData.Appezza, nuovo_Area_Cod, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione del gruppo all'appezzamento.")
                End If

                res = ClonaAssociazioniMUZ(CInt(row("Area_Cod")), nuovo_Area_Cod, xRead, xWriteMUZ, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nella clonazione delle associazioni della MUZ.")
                End If
            Next

            res = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception

            res = False

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return res

    End Function

    Private Function AggiungiMUZAGruppo(ByVal Area_Cod As Integer,
                                        ByVal Gruppo_Area_Cod As Integer,
                                        ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_MUZ_W

        Return xWrite.AggiungiMUZAGruppo(Area_Cod, Gruppo_Area_Cod, objParametri_Server)

    End Function

    Public Function EseguiOperazioneMUZ(ByVal muz As MUZ,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        Optional associaEntita As Boolean = False) As Boolean

        Select Case muz.Operazione_Cod
            Case enum_Operazioni_MUZ.INSERT
                Return ScriviMUZ(muz, objParametri_Server, associaEntita)
            Case enum_Operazioni_MUZ.UPDATE
                Return UpdateMUZ(muz, objParametri_Server)
            Case enum_Operazioni_MUZ.DELETE
                Return EliminaMUZ(muz.Area_Cod, objParametri_Server)
            Case Else
                Throw New Exception("Operazione non consentita")
        End Select

    End Function

    Private Function EliminaMUZ(ByVal Area_Cod As Int32,
                                ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xWriteMUZ As New AgronicaCoreGisDAL.GIS_MUZ_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim res As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            res = xWriteMUZ.DeleteDataMUZ(Area_Cod, objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nell'eliminazione dei dati della MUZ.")
            End If

            res = xWriteMUZ.DeleteEntitaMUZ(Area_Cod, objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nell'eliminazione degli elementi grafici della MUZ.")
            End If

            res = xWriteMUZ.DeleteMUZ(Area_Cod, objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nell'eliminazione della MUZ.")
            End If

            res = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception

            res = False

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return res

    End Function

    Private Function UpdateMUZ(ByVal muz As MUZ,
                               ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xWriteMUZ As New AgronicaCoreGisDAL.GIS_MUZ_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim res As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            res = xWriteMUZ.UpdateMUZ(muz, objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nell'aggiornamento della MUZ.")
            End If

            Dim ElementoGrafico_Des = String.Format("Descrizione §{0}|Classe §{1}", muz.Area_Des, muz.TipoZona)

            Dim xWritePoligono As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_W

            xWritePoligono.VerificaPoligonoRestituisciWKTRuotato(muz.Geometry, objParametri_Server)

            res = xWriteMUZ.AggiornaElementoGrafico(muz.Area_Cod, muz.Geometry, ElementoGrafico_Des, objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nell'aggiornamento dell'Elemento grafico.")
            End If

            For Each particella In muz.Particelle_Catastali_Elimina
                res = xWriteMUZ.DisassociaMUZDaParticellaCatastale(muz.Area_Cod, particella, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nella rimozione della particella catastale dalla MUZ.")
                End If
            Next

            For Each particella In muz.Particelle_Catastali_Aggiungi
                res = xWriteMUZ.AssociaMUZAParticellaCatastale(muz.Area_Cod, particella, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione della particella catastale alla MUZ.")
                End If
            Next

            For Each Analisi_Testata_Cod In muz.Analisi_Testate_Elimina
                res = xWriteMUZ.DisassociaAnalisiDaMUZ(muz.Area_Cod, Analisi_Testata_Cod, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nella rimozione dell'analisi dalla MUZ.")
                End If
            Next

            For Each Analisi_Testata_Cod In muz.Analisi_Testate_Aggiungi
                res = xWriteMUZ.AssociaMUZAdAnalisi(muz.Area_Cod, Analisi_Testata_Cod, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione dell'analisi alla MUZ.")
                End If
            Next

            res = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception

            res = False

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return res


    End Function

    Private Function ScriviMUZ(ByVal muz As MUZ,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               Optional associaMuzAdEntita As Boolean = False) As Boolean

        Dim xWriteMUZ As New AgronicaCoreGisDAL.GIS_MUZ_W
        Dim xWriteEntita As New AgronicaCoreGisDAL.GIS_Entita_W
        Dim xWriteElementoGrafico As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim res As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim Area_cod = xWriteMUZ.ScriviMUZ(muz, objParametri_Server)

            Dim sequenza As New Agro_Sequenze
            Dim Entita_Cod = sequenza.NuovoId_Tabella("GIS_Entita", 0, Int32.MaxValue, objParametri_Server, True)

            res = xWriteEntita.Scrivi(objParametri_Server.PivaSuperUser,
                                      Entita_Cod,
                                      TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.MUZ,
                                      muz.Piva,
                                      0, 0, 0, 0, "", "", "", 0, 0, "", 0, 0, 0, 0, 0, 0, "", 0,
                                      muz.Validita_Inizio,
                                      muz.Validita_Fine,
                                      objParametri_Server,
                                      Date.Now,
                                      Date.Now,
                                      objParametri_Server.UsernameOperazione,
                                      objParametri_Server.UsernameOperazione,
                                      0,
                                      Area_cod)

            If Not res Then
                Throw New Exception("Errore nella creazione dell'entità.")
            End If

            Dim ElementoGrafico_Cod = sequenza.NuovoId_Tabella("GIS_ElementiGrafici", 0, Int32.MaxValue, objParametri_Server, True)

            Dim ElementoGrafico_Des = String.Format("Descrizione §{0}|Classe §{1}", muz.Area_Des, muz.TipoZona)

            Dim xWritePoligono As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_W

            xWritePoligono.VerificaPoligonoRestituisciWKTRuotato(muz.Geometry, objParametri_Server)

            res = xWriteElementoGrafico.ScriviElementoGraficoBaseDaWKT(ElementoGrafico_Cod,
                                                                       ElementoGrafico_Des,
                                                                       Entita_Cod,
                                                                       TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.MUZ,
                                                                       muz.Geometry,
                                                                       muz.Validita_Inizio,
                                                                       muz.Validita_Fine,
                                                                       objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nella creazione dell'elemento grafico.")
            End If
            
            If associaMuzAdEntita Then 
                res = xWriteMUZ.AssociaMUZAdEntita(
                    muz.appezzamento.FirstOrDefault.Piva, 
                    muz.appezzamento.FirstOrDefault.Sa_Cod,
                    Entita_Cod, 
                    Area_cod, 
                    objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nella creazione dell'elemento grafico.")
                End If
            End If

            If muz.Gruppo_Area_Cod <> 0 Then
                res = xWriteMUZ.AggiungiMUZAGruppo(Area_cod, muz.Gruppo_Area_Cod, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione della MUZ al gruppo.")
                End If
            End If

            res = xWriteMUZ.AssociaMUZAdAppezzamento(muz.appezzamento.FirstOrDefault.Piva,
                                                     muz.appezzamento.FirstOrDefault.Sa_Cod,
                                                     muz.appezzamento.FirstOrDefault.Appezza,
                                                     Area_cod,
                                                     objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nell'associazione della MUZ all'appezzamento.")
            End If

            For Each particella In muz.Particelle_Catastali_Aggiungi
                res = xWriteMUZ.AssociaMUZAParticellaCatastale(Area_cod, particella, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione della MUZ alla particella catastale.")
                End If
            Next

            For Each Analisi_Testata_Cod In muz.Analisi_Testate_Aggiungi
                res = xWriteMUZ.AssociaMUZAdAnalisi(Area_cod, Analisi_Testata_Cod, objParametri_Server)

                If Not res Then
                    Throw New Exception("Errore nell'associazione della MUZ all'analisi.")
                End If
            Next

            res = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception

            res = False

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return res

    End Function
End Class
