' Filename:    ProgressWindow.xaml.cs
' Description: ProgressWindow implements a basic progress window which
'              supports a cancel button.
' 2007-01-22 nschan Initial revision.

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media

Namespace TestShapeFile
    ''' <summary>
    ''' ProgressWindow implements a basic progress window that
    ''' supports a Cancel button.
    ''' </summary>
    Partial Public NotInheritable Class ProgressWindow
        Inherits System.Windows.Window
#Region "Events"
        ''' <summary>
        ''' Event to notify that the Cancel button has been pressed.
        ''' </summary>
        Public Event Cancel As CancelEventHandler
#End Region

#Region "Private fields"
        Private m_progressText As String
        Private m_progressValue As Double
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor for ProgressWindow class.
        ''' </summary>
        Public Sub New()
            InitializeComponent()
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Specify the text to display in the progress window.
        ''' </summary>
        Public Property ProgressText() As String
            Get
                Return Me.m_progressText
            End Get
            Set(value As String)
                Me.m_progressText = value
                Me.label1.Content = Me.m_progressText
            End Set
        End Property

        ''' <summary>
        ''' Specify the value of the progress bar.
        ''' </summary>
        Public Property ProgressValue() As Double
            Get
                Return Me.m_progressValue
            End Get
            Set(value As Double)
                Me.m_progressValue = value
                Me.progressBar1.Value = Me.m_progressValue
            End Set
        End Property
#End Region

#Region "Event handling"
        ''' <summary>
        ''' Handle the Click event for the Cancel button.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub cancelButton_Click(sender As Object, e As EventArgs) Handles cancelButton.Click
            Me.Close()

        End Sub
#End Region
    End Class
End Namespace

' END
