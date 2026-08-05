Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreModello.Nogmo
Imports System.Text

Public Class SincroNogmo
    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri

    Dim logDirectory As String
    Dim logFileName As String
    Dim objLog As New AgronicaCoreDataProvider.LogProvider

    Dim uploadNogmo As uploadNogmo
    Dim updateNogmo As updateNogmo
    Dim datiStalle As List(Of IDictionary(Of String, Object))
    Dim datiAnimali As IDictionary(Of String, IDictionary(Of String, Object))
    Dim razzeAnimali As List(Of IDictionary(Of String, Object))

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti

        logDirectory = objParametriServer.LogDirectory & "\Nogmo\"

        uploadNogmo = New uploadNogmo(objParametriServer, objParametriUtenti)
        updateNogmo = New updateNogmo(objParametriServer, objParametriUtenti)
    End Sub

    Public Function aggiungiANogmo(piva As String, infoAnimali As HashSet(Of JToken)) As String
        Dim operatore = ""
        Dim esportatore = ""
        Dim filieraDiPertinenza = "NON OGM"
        Dim centroDiRaccolta = ""
        Dim exporterId = 15 'arbitrario


        Dim StallaDAL = New AgronicaCoreAnagrafeDAL.Stalla_R
        Dim Zoo_AnimaliDAL = New AgronicaCoreAnagrafeDAL.Zoo_Animali
        razzeAnimali = Zoo_AnimaliDAL.ottieniRazzeNogmo(objParametriServer).ToExpandoObject
        datiAnimali = Zoo_AnimaliDAL.OttieniDatiNogmo(infoAnimali.Select(Of String)(Function(infoAnimale)
                                                                                        Return infoAnimale("Cod_Progetto")
                                                                                    End Function).ToList, objParametriServer).ToExpandoObject _
                                                                                    .ToDictionary(Of String, IDictionary(Of String, Object)) _
                                                                                    (Function(elem) elem("Cod_Progetto"), Function(elem) elem)

        'Tiro fuori direttamente i dati sia per le stalle attuali, sia per quelle di nascita
        Dim listaStalle = infoAnimali.Select(Of String)(Function(infoAnimale)
                                                            Return infoAnimale("Stalla")
                                                        End Function).Concat(datiAnimali.ToHashSet.Select(Of String)(Function(elem) elem.Value("AUSL_AZI_NASCITA")).Where(Function(s) Not String.IsNullOrEmpty(s))).Distinct.ToList

        datiStalle = StallaDAL.IndirizzoStalla(piva, listaStalle, objParametriServer).ToExpandoObject

        Dim nazioniPossibili As New List(Of String)({"IT", "FR", "IE"})
        Dim returnMessagge = New StringBuilder("")
        returnMessagge.Length = 0

        Dim matricoleFallite = New List(Of String)
        nazioniPossibili.ForEach(Sub(nazione)
                                     Dim capiPerNazione = infoAnimali.Where(Function(animale) animale("Matricola").ToString.StartsWith(nazione))
                                     If Not capiPerNazione.Any Then
                                         Exit Sub
                                     End If
                                     Dim datiNogmo = creaDatiNogmo(capiPerNazione, nazione, uploadNogmo.exporterID)
                                     Dim matricoleCapiNazioneFalliti = datiNogmo.SelectMany(Function(datiNogmoSingoli) uploadNogmo.add(datiNogmoSingoli)).ToList
                                     matricoleFallite.AddRange(matricoleCapiNazioneFalliti)
                                 End Sub)

        If matricoleFallite.Any Then
            returnMessagge.AppendLine("L'invio dei seguenti capi e' fallito, per ulteriori informazioni guardare la colonna note:")
            matricoleFallite.ForEach(Sub(matricola) returnMessagge.AppendLine(matricola))
        End If

        Return returnMessagge.ToString
    End Function


    Private Function creaDatiNogmo(infoAnimali As IEnumerable(Of JToken), nazione As String, exporterId As Integer) As List(Of DatiNogmo)

        Const LIMITE_ANIMALI_PER_CHIAMATA = 200
        Return infoAnimali.ChunkBy(LIMITE_ANIMALI_PER_CHIAMATA).Select(Of DatiNogmo)(
           Function(chunkedInfoAnimali)
               Return New DatiNogmo With {
                .Nation = nazione,
                .ExporterID = exporterId,
                .BovineList = chunkedInfoAnimali.Select(Of Animale)(Function(infoAnimale)
                                                                        Dim objLog As New AgronicaCoreDataProvider.LogProvider

                                                                        Dim animale = New Animale(infoAnimale)
                                                                        Dim animaleStr = ""
                                                                        If animale IsNot Nothing Then
                                                                            animaleStr = JsonConvert.SerializeObject(animale)
                                                                        End If
                                                                        Dim datiAnimale = datiAnimali(animale.Cod_Progetto)
                                                                        animale.aggiungiDatiAnimaleDaExpando(datiAnimale, razzeAnimali)
                                                                        animale.aggiungiDatiStallaDaExpando(datiStalle.Where(Function(stalla) stalla("BDN_Codice_Azienda") = animale.BreedingNumber).FirstOrDefault,
                                                                                                     datiStalle.Where(Function(stalla) stalla("BDN_Codice_Azienda") = datiAnimale("AUSL_AZI_NASCITA")).FirstOrDefault, datiAnimale("AUSL_AZI_NASCITA"))
                                                                        animale.aggiungiDatiGenerici("", "", "", "", "")
                                                                        Return animale
                                                                    End Function).ToList()
               }

           End Function).ToList()

    End Function







End Class
