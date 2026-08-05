
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports SincroAnagrafeBA
Imports AgroFascicoloBA_SIGPA_DAL
Imports System.Xml


Public Class Fascicolo_R    

    Public Function GetSingoloFascicoloAgea( _
            ByVal EnteValidatore_COD As Integer, _
            ByVal cuaa As String, _
            ByVal Validazione_Numero As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As Fascicolo



        Dim leggi As New AgroBA_Fascicolo_R
        Dim dtParticelle As DataTable = _
        leggi.getFascicoloParticelle( _
              cuaa _
            , Validazione_Numero _
            , "" _
            , "" _
            , objParametri _
        )

        Dim f As New Fascicolo
        Dim t1 As New List(Of ISWSTerritorio1)

        Dim utiliz As New ISWSUtilizzoTerra1

        f.fascicolo = New ISWSRespAnagFascicolo2


        'info di testata
        f.fascicolo.CUAA = cuaa

        'catasto..

        t1 = getTerr1_Particelle(dtParticelle)

        For Each p In t1

            p.ISWSUtilizzoTerra1 = getUtilizzoTerra1(p, dtParticelle).ToArray

            For Each m In p.ISWSUtilizzoTerra1

                m.ISWSUtilizzoTerra = getUso(p, m, dtParticelle).ToArray

            Next
        Next

        f.fascicolo.ISWSTerritorio1 = t1.ToArray

        Return f

    End Function



    Private Function getTerr1_Particelle(ByVal dt As DataTable) As List(Of ISWSTerritorio1)

        Return ( _
            From ii In dt.AsEnumerable _
            Group By _
                gProvincia = ii("Provincia"), _
                gComune = ii("Comune"), _
                gSezione = ii("Sezione"), _
                gFoglio = ii("Foglio"), _
                gParticella = ii("particella"), _
                gSubalterno = ii("subalterno"), _
                gSuperficieCatastale = ii("superficiecatastale"), _
                gSuperficieCondotta = ii("superficiecondotta"), _
                gDataInizioConduzione = ii("datainizioconduzione"), _
                gDataFineConduzione = ii("datafineconduzione") _
                Into gg = Group _
             Select New ISWSTerritorio1 With { _
                .Provincia = gProvincia, _
                .Comune = gComune, _
                .Sezione = gSezione, _
                .Foglio = gFoglio, _
                .Particella = gParticella, _
                .Subalterno = gSubalterno, _
                .SuperficieCatastale = gSuperficieCatastale, _
                .SuperficieCondotta = gSuperficieCondotta, _
                .DataInizioConduzione = AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(gDataInizioConduzione), _
                .DataFineConduzione = AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(gDataFineConduzione) _
            }).ToList


    End Function

    ''' <summary>
    ''' macrousi di una particelle
    ''' </summary>
    ''' <param name="t1"></param>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getUtilizzoTerra1(ByVal t1 As ISWSTerritorio1, ByVal dt As DataTable) As List(Of ISWSUtilizzoTerra1)

        Return ( _
            From ii In dt.AsEnumerable _
            Where t1.Provincia = ii("Provincia") _
            And t1.Comune = ii("Comune") _
            And t1.Sezione = ii("Sezione") _
            And t1.Foglio = ii("Foglio") _
            And t1.Particella = ii("Particella") _
            And t1.Subalterno = ii("Subalterno") _
            And ii("Destinazione_codicemacrouso") <> "" _
            Group By _
               gDestinazione_codicemacrouso = ii("Destinazione_codicemacrouso"), _
               gDestinazione_superficieutilizzata = CInt(ii("Destinazione_superficieutilizzata")) _
            Into gg = Group _
            Select New ISWSUtilizzoTerra1 With { _
                .CodiceMacrouso = gDestinazione_codicemacrouso, _
                .SuperficieUtilizzata = CInt(gDestinazione_superficieutilizzata), _
                .SuperficieUtilizzataSpecified = True _
            }).ToList


    End Function


    ''' <summary>
    ''' usi associati ai macrousi di una particella
    ''' </summary>
    ''' <param name="t1"></param>
    ''' <param name="u1"></param>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getUso(ByVal t1 As ISWSTerritorio1, ByVal u1 As ISWSUtilizzoTerra1, ByVal dt As DataTable) As List(Of ISWSUtilizzoTerra)
        Return ( _
                    From ii In dt.AsEnumerable _
                    Where t1.Provincia = ii("Provincia") _
                    And t1.Comune = ii("Comune") _
                    And t1.Sezione = ii("Sezione") _
                    And t1.Foglio = ii("Foglio") _
                    And t1.Particella = ii("Particella") _
                    And t1.Subalterno = ii("Subalterno") _
                    And u1.CodiceMacrouso = ii("Destinazione_codicemacrouso") _
                    And ii("USO_codiceprodotto") <> "" _
                    Select New ISWSUtilizzoTerra With { _
                        .CodiceProdotto = ii("USO_codiceprodotto"), _
                        .SuperficieUtilizzata = CInt(ii("USO_superficieutilizzata")), _
                        .SuperficieUtilizzataSpecified = True, _
                        .CodiceVarieta = "" _
                    }).ToList


    End Function



End Class
