Imports System.IO
Imports System.Web.Services

Public Class export
    Inherits System.Web.UI.Page


    <WebMethod(EnableSession:=True)> _
    Public Function GetFile(filename As String) As Byte()
        Dim binReader As New BinaryReader(File.Open(Server.MapPath(filename), FileMode.Open, FileAccess.Read))
        binReader.BaseStream.Position = 0
        Dim binFile As Byte() = binReader.ReadBytes(Convert.ToInt32(binReader.BaseStream.Length))
        binReader.Close()
        Return binFile
    End Function



End Class