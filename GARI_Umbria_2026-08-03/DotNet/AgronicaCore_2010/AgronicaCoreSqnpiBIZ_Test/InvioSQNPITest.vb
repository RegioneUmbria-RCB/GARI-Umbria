Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports AgronicaCoreSqnpiBIZ



'''<summary>
'''Classe di test per InvioSQNPITest.
'''Creata per contenere tutti gli unit test InvioSQNPITest
'''</summary>
<TestClass()> _
Public Class InvioSQNPITest


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
            testContextInstance = Value
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
    '''Test per InvioDati
    '''</summary>
    <TestMethod()> _
    Public Sub InvioDatiTest()
        Dim target As InvioSQNPI = New InvioSQNPI() ' TODO: Eseguire l'inizializzazione a un valore appropriato
        Dim expected As Object = Nothing ' TODO: Eseguire l'inizializzazione a un valore appropriato
        Dim actual As Object
        'actual = target.InvioDati("", "", "", Nothing)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verificare la correttezza del metodo di test.")
    End Sub
End Class
