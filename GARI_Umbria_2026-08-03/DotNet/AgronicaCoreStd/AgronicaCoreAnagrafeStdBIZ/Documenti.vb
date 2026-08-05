Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreModelloSTD
Imports Newtonsoft.Json
Imports System.IO

Public Class Documenti
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    ''' <summary>
    ''' Lettura lista documenti con filtro ricerca
    ''' </summary>
    ''' <param name="Ricerca"></param>
    Public Function EstraiListaDocumenti(Ricerca As String, Optional Visita_Cod As Integer = 0) As List(Of Documento)

        Dim xLettura As New Documenti_R
        Dim letturaImprese As New Imprese_R
        Dim letturaTipologie As New Tipologie_R

        Dim listaDocumenti As New List(Of AgronicaCoreModelloSTD.Documento)
        Dim listaImprese = letturaImprese.Leggi(dbContext, "")
        Dim listaTipologie = letturaTipologie.Leggi(dbContext)
        Dim listaDocumentiApp = xLettura.Leggi(dbContext, 0, Visita_Cod)

        For Each documentoAPP In listaDocumentiApp

            Dim azienda = listaImprese.Where(Function(i) i.piva = documentoAPP.Piva).FirstOrDefault
            Dim tipologia = listaTipologie.Where(Function(i) i.ID_Tipologia = documentoAPP.ID_Tipologia).FirstOrDefault
            Dim allegati = JsonConvert.DeserializeObject(Of List(Of Allegato))(documentoAPP.Allegati)

            Dim documento As New AgronicaCoreModelloSTD.Documento With {
                .TipoOperazioneDB = enum_TipoOperazioneDB.Lettura,
                .Documento_Cod = documentoAPP.Documento_Cod,
                .Piva_Superuser = documentoAPP.Piva_Superuser,
                .Piva = documentoAPP.Piva,
                .Azienda = azienda,
                .ID_Tipologia = documentoAPP.ID_Tipologia,
                .Tipologia = tipologia,
                .Data_Upload = documentoAPP.Data_Creazione,
                .Data_Scadenza = documentoAPP.Data_Scadenza,
                .Descrizione = documentoAPP.Descrizione,
                .Allegati = allegati,
                .Note = documentoAPP.Note
            }

            If allegati IsNot Nothing AndAlso allegati.Count > 0 Then
                documento.DescrizioneAllegati = String.Join(", ", (From a In allegati Select a.FileName).ToArray)
            End If

            listaDocumenti.Add(documento)

        Next

        ' filtro ricerca libero
        If Not String.IsNullOrEmpty(Ricerca) Then
            listaDocumenti = listaDocumenti.Where(Function(i) String.IsNullOrEmpty(Ricerca) OrElse i.TestoRicerca.ToLower.Contains(Ricerca.ToLower)).ToList()
        End If

        Return listaDocumenti

    End Function

    Public Function LeggiDocumentiDaInviare(piva As String, Optional visita As Boolean = False) As List(Of DocumentoPerScarico)

        Dim documentiR As New Documenti_R
        Dim documenti = documentiR.LeggiPerRicaricoDati(dbContext, piva, visita)

        Dim documentiDaInviare As New List(Of DocumentoPerScarico)

        For Each documento In documenti

            Dim allegati = JsonConvert.DeserializeObject(Of List(Of Allegato))(documento.Allegati)
            Dim documentoAllegati As New List(Of DocumentoAllegato)

            For Each allegato In allegati
                Dim filebyte = File.ReadAllBytes(allegato.FullPath)
                documentoAllegati.Add(New DocumentoAllegato() With {
                    .FileName = allegato.FileName,
                    .FileByte = Convert.ToBase64String(filebyte)
                })
            Next

            Dim documentoDaInviare = New DocumentoPerScarico With {
                .Piva_Superuser = documento.Piva_Superuser,
                .Piva = documento.Piva,
                .Documento_Cod = documento.Documento_Cod,
                .ID_Tipologia = documento.ID_Tipologia,
                .Data_Scadenza = documento.Data_Scadenza,
                .Descrizione = documento.Descrizione,
                .Allegati = documentoAllegati,
                .Note = documento.Note,
                .Visita_Cod = documento.Visita_Cod
            }

            documentiDaInviare.Add(documentoDaInviare)

        Next

        Return documentiDaInviare

    End Function

    Public Sub ScriviDocumento(documento As APP_Documenti, username As String)

        Dim xScrittura = New Documenti_W()

        If documento.Documento_Cod = 0 Then
            Dim agroSequenze As New AgronicaCoreDataProviderSTD.Agro_Sequenze
            documento.Documento_Cod = agroSequenze.NuovoId_Tabella_EF(dbContext, "documenti", AgroSequenzeBase0, AgroSequenzeEndUpperBound)
            dbContext.SaveChanges()
        Else
            xScrittura.Cancella(dbContext, documento.Documento_Cod)
        End If

        If Not String.IsNullOrEmpty(documento.Allegati) Then
            ScritturaDatiComuni(documento, username)
            xScrittura.Scrivi(dbContext, documento, True)
        End If

    End Sub

    Public Sub CancellaDocumento(Documento_Cod As Integer)

        Dim xScrittura = New Documenti_W()
        xScrittura.Cancella(dbContext, Documento_Cod)

    End Sub

End Class
