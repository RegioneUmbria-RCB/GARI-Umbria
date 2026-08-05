
Imports <xmlns="http://www.agronica.it/grafica/">

Imports AgronicaGIS2012.Commons
Imports AgronicaCoreDataProvider
Imports System.Text



Public Class DatiEntita_ProfilazioneLayers

    Public Function InserisciLayers(ByVal xmlDatiEntita As String, objParametri As AgronicaCoreParametri) As String

        Dim xmlFinaleElaborato As XDocument = XDocument.Parse(xmlDatiEntita)
        Dim sXmlRval As String = ""

        'For Each nodoEntita In ( _
        '    From e In xmlFinaleElaborato.<DatiEntita>.<Entita> _
        '    Select e).ToList


        '    Dim layerPivaPadre = <layer tipologia_layer="1">3</layer>
        '    nodoEntita.<layers>.FirstOrDefault.Add(layerPivaPadre)


        'Next


        Dim listaNS As New List(Of String)
        listaNS.Add("http://www.opengis.net/gml")


        sXmlRval = xmlHelper.RemoveNamespace(xmlFinaleElaborato, listaNS).ToString.Replace("xmlns=""""", "")

        Return sXmlRval


    End Function


End Class
