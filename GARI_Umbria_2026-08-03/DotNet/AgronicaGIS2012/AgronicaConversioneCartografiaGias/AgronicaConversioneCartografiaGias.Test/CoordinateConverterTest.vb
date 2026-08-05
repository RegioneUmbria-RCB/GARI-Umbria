Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports AgronicaConversioneCartografiaGias.Agronica
Imports System.Configuration.ConfigurationManager



'''<summary>
'''Classe di test per CoordinateConverterTest.
'''Creata per contenere tutti gli unit test CoordinateConverterTest
'''</summary>
<TestClass()> _
Public Class CoordinateConverterTest


    Private testContextInstance As TestContext

    '''<summary>
    '''Ottiene o imposta il contesto dei test, che fornisce
    '''funzionalità e informazioni sull'esecuzione dei test corrente.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(value As TestContext)
            testContextInstance = value
        End Set
    End Property

#Region "Attributi di test aggiuntivi"
    '
    'Durante la scrittura dei test è possibile utilizzare i seguenti attributi aggiuntivi:
    '
    'Utilizzare ClassInitialize per eseguire il codice prima di eseguire il primo test della classe
    '<ClassInitialize()>  _
    'Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
    'End Sub
    '
    'Utilizzare ClassCleanup per eseguire il codice dopo l'esecuzione di tutti i test di una classe
    '<ClassCleanup()>  _
    'Public Shared Sub MyClassCleanup()
    'End Sub
    '
    'Utilizzare TestInitialize per eseguire il codice prima di eseguire ciascun test
    '
    '<TestInitialize()>  _
    'Public Sub MyTestInitialize()
    'End Sub
    'Utilizzare TestCleanup per eseguire il codice dopo l'esecuzione di ciascun test
    '<TestCleanup()>  _
    'Public Sub MyTestCleanup()
    'End Sub
    '
#End Region


    '''<summary>
    '''Test per WKTPolygonWGS84_from_WKTPolygonED50
    '''</summary>
    <TestMethod()> _
    Public Sub WKTPolygonWGS84_from_WKTPolygonED50Test()

        Dim WKTPolygonED50 As String = _
            "POLYGON ((757457.33308 4939868.91518, 757477.40166 4939885.48041, 757481.86093 4939877.67669, 757521.44479 4939800.17565, 757526.51931 4939788.42379, 757483.6121 4939780.7225, 757508.53951 4939696.66395, 757407.9304 4939671.0755, 757381.99876 4939754.0212, 757369.78766 4939796.03105, 757458.6216 4939862.98798, 757457.33308 4939868.91518))"

        Dim objParametri As New ParametriCoordinateConverter With { _
            .CSFromText = AppSettings("CSFrom"), _
            .CStoText = AppSettings("CSTo"), _
            .CStoGeoText = AppSettings("CStoGeo"), _
            .AgronicaLatOffset = AppSettings("AgronicaLatOffest"), _
            .AgronicaLonOffset = AppSettings("AgronicaLonOffest") _
        }


        Dim expected As String = String.Empty ' TODO: Eseguire l'inizializzazione a un valore appropriato

        Dim actual As String
        'actual = CoordinateConverter.WKTPolygonWGS84_from_WKTPolygonED50(WKTPolygonED50, True, objParametri)

        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verificare la correttezza del metodo di test.")
    End Sub
End Class
