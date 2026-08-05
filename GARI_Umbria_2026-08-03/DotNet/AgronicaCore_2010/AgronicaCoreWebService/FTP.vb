Imports System.IO
Imports System.Net

Public Class FTP

    Public Shared Function UploadFTPFiles(ftpAddress As String,
                                          ftpUser As String,
                                          ftpPassword As String,
                                          fileToUpload As String,
                                          targetFileName As String,
                                          deleteAfterUpload As Boolean,
                                          ByRef MsgError As String) As Boolean
        MsgError = ""

        Try
            Dim credential = New NetworkCredential(ftpUser, ftpPassword)

            If ftpAddress.EndsWith("/") = False Then ftpAddress = ftpAddress & "/"

            Dim sFtpFile As String = ftpAddress & targetFileName

            Dim request As FtpWebRequest = DirectCast(WebRequest.Create(sFtpFile), FtpWebRequest)

            request.KeepAlive = False
            request.Method = WebRequestMethods.Ftp.UploadFile
            request.Credentials = credential
            request.UsePassive = False
            request.Timeout = (60 * 1000) * 3 '3 mins

            Using reader As New FileStream(fileToUpload, FileMode.Open)

                Dim buffer(Convert.ToInt32(reader.Length - 1)) As Byte
                reader.Read(buffer, 0, buffer.Length)
                reader.Close()

                request.ContentLength = buffer.Length
                Dim stream As Stream = request.GetRequestStream
                stream.Write(buffer, 0, buffer.Length)
                stream.Close()

                Using response As FtpWebResponse = DirectCast(request.GetResponse, FtpWebResponse)

                    If deleteAfterUpload Then
                        My.Computer.FileSystem.DeleteFile(fileToUpload)
                    End If

                    response.Close()
                End Using

            End Using

            Return True

        Catch ex As Exception
            MsgError = ex.Message
            Return False
        End Try

    End Function

    Public Shared Sub DownloadFTPFile(ByVal downloadpath As String,
                                      ByVal ftpuri As String,
                                      ByVal pathImport As String,
                                      ByVal ftpusername As String,
                                      ByVal ftppassword As String,
                                      ByRef MsgError As String)
        MsgError = ""

        'Create a WebClient.
        ' Confirm the Network credentials based on the user name and password passed in.
        Dim request As New WebClient With {
            .Credentials = New NetworkCredential(ftpusername, ftppassword)
        }
        Try

            'Read the file data into a Byte array
            'Dim bytes() As Byte = request.DownloadData(ftpuri + pathImport)
            request.DownloadFile(ftpuri + pathImport, downloadpath)



            ''  Create a FileStream to read the file into
            'Dim DownloadStream As FileStream = IO.File.Create(downloadpath)
            ''  Stream this data into the file
            'DownloadStream.Write(bytes, 0, bytes.Length)
            ''  Close the FileStream
            'DownloadStream.Close()

        Catch ex As Exception
            MsgError = ex.Message
            Exit Sub
        End Try

    End Sub


End Class
