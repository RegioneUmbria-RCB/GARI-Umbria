Imports System.IO
Imports System.Net
Imports System.Text
Imports AgronicaCoreUtility
Imports Renci.SshNet

Public Class FTPHelper

    Public Function UploadFile(ByVal sourcePath As String, ByVal urlFTP As String, ByVal username As String, ByVal password As String, ByRef FtpCmdStatusDescription As String) As Boolean
        Dim ret As Boolean = False
        Try
            Dim ftpClient = New SftpClient(urlFTP, 22, username, password)

            ftpClient.Connect()

            If ftpClient.IsConnected Then
                Using fileStream As FileStream = New FileStream(sourcePath, FileMode.Open)
                    ftpClient.UploadFile(fileStream, "/" + Path.GetFileName(sourcePath))
                    ftpClient.Disconnect()
                    ftpClient.Dispose()
                End Using
                ret = True
            Else
                ret = False
                FtpCmdStatusDescription = "Errore di connessione al server FTP"
            End If
        Catch ex As Exception
            ret = False
            FtpCmdStatusDescription = ex.Message
        End Try
        Return ret
    End Function

    Public Function UploadFile2(ByVal sourcePath As String, ByVal urlFTP As String, ByVal username As String, ByVal password As String, ByRef FtpCmdStatusDescription As String) As Boolean
        Dim ret As Boolean = False
        Try
            Dim ftpRequest As FtpWebRequest = WebRequest.Create(urlFTP)
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile

            ftpRequest.Credentials = New NetworkCredential(username, password)

            Dim fileContent As Byte()
            Using sourceStream As StreamReader = New StreamReader(sourcePath)
                fileContent = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd)
            End Using

            ftpRequest.ContentLength = fileContent.Length

            Using requestStream As Stream = ftpRequest.GetRequestStream()
                requestStream.Write(fileContent, 0, fileContent.Length)
            End Using

            Using response As FtpWebResponse = ftpRequest.GetResponse()
                If response.StatusCode = FtpStatusCode.CommandOK Then
                    ret = True
                Else
                    ret = False
                    FtpCmdStatusDescription = response.StatusDescription
                End If
            End Using

        Catch ex As Exception
            ret = False
            FtpCmdStatusDescription = ex.Message
        End Try
        Return ret
    End Function
End Class
