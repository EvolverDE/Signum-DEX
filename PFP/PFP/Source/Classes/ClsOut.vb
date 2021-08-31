Public Class ClsOut

    Public Property Path() As String

    Sub New(Optional ByVal _Path As String = "")

        If _Path.Trim = "" Then
            _Path = Application.StartupPath
        End If

        Path = _Path
    End Sub

    Public Sub ToFile(ByVal str As String, Optional ByVal File As String = "Log.log", Optional ByVal Mode As IO.FileMode = IO.FileMode.Append)

        Try

            Dim yf As IO.FileStream = IO.File.Open(Path + "\" + File, Mode)
            Dim verinfo As Byte() = New System.Text.UTF8Encoding(True).GetBytes(str)

            yf.Write(verinfo, 0, verinfo.Length)
            yf.Close()

        Catch ex As Exception
            'MsgBox(ex.Message,, "an error occurred in out/ToFile")

            Try

                Dim NowStr As String = Now.ToString
                NowStr = NowStr.Replace(" ", "").Replace(":", "")

                Dim yf As IO.FileStream = IO.File.Open(Path + "\Errors" + NowStr + ".log", IO.FileMode.Append)
                Dim verinfo As Byte() = New System.Text.UTF8Encoding(True).GetBytes(str)

                yf.Write(verinfo, 0, verinfo.Length)
                yf.Close()

            Catch exxx As Exception

            End Try

        End Try

    End Sub



    Public Sub Info2File(ByVal str As String)

        str = "#################### MESSAGE START ####################" + vbCrLf + Now.ToShortDateString + " " + Now.ToShortTimeString + vbCrLf + str + vbCrLf
        str += vbCrLf + "#################### MESSAGE END ####################" + vbCrLf

        Try
            Dim yf As IO.FileStream = IO.File.Open(Path + "\Info.log", IO.FileMode.Append)
            Dim verinfo As Byte() = New System.Text.UTF8Encoding(True).GetBytes(str)

            yf.Write(verinfo, 0, verinfo.Length)
            yf.Close()

        Catch ex As Exception
            'MsgBox(ex.Message,, "an error occurred in out/Info2File")
            Try

                Dim NowStr As String = Now.ToString
                NowStr = NowStr.Replace(" ", "").Replace(":", "")

                Dim yf As IO.FileStream = IO.File.Open(Path + "\Errors" + NowStr + ".log", IO.FileMode.Append)
                Dim verinfo As Byte() = New System.Text.UTF8Encoding(True).GetBytes(str)

                yf.Write(verinfo, 0, verinfo.Length)
                yf.Close()

            Catch exxx As Exception

            End Try
        End Try

    End Sub


    Public Sub ErrorLog2File(ByVal str As String)

        str = "#################### MESSAGE START ####################" + vbCrLf + Now.ToShortDateString + " " + Now.ToShortTimeString + vbCrLf + vbCrLf + str
        str += vbCrLf + "##################### MESSAGE END #####################" + vbCrLf

        Try
            Dim yf As IO.FileStream = IO.File.Open(Path + "\Errors.log", IO.FileMode.Append)
            Dim verinfo As Byte() = New System.Text.UTF8Encoding(True).GetBytes(str)

            yf.Write(verinfo, 0, verinfo.Length)
            yf.Close()
        Catch ex As Exception
            'MsgBox(ex.Message,, "an error occurred in out/ErrorLog2File")
            Try

                Dim NowStr As String = Now.ToString
                NowStr = NowStr.Replace(" ", "").Replace(":", "")

                Dim yf As IO.FileStream = IO.File.Open(Path + "\Errors" + NowStr + ".log", IO.FileMode.Append)
                Dim verinfo As Byte() = New System.Text.UTF8Encoding(True).GetBytes(str)

                yf.Write(verinfo, 0, verinfo.Length)
                yf.Close()

            Catch exxx As Exception

            End Try

        End Try

    End Sub

    Public Sub WarningLog2File(ByVal str As String)

        str = "#################### MESSAGE START ####################" + vbCrLf + Now.ToShortDateString + " " + Now.ToShortTimeString + vbCrLf + vbCrLf + str
        str += vbCrLf + "##################### MESSAGE END #####################" + vbCrLf
        Try
            Dim yf As IO.FileStream = IO.File.Open(Path + "\Warnings.log", IO.FileMode.Append)
            Dim verinfo As Byte() = New System.Text.UTF8Encoding(True).GetBytes(str)

            yf.Write(verinfo, 0, verinfo.Length)
            yf.Close()

        Catch ex As Exception
            'MsgBox(ex.Message,, "an error occurred in out/WarningLog2File")
            Try

                Dim NowStr As String = Now.ToString
                NowStr = NowStr.Replace(" ", "").Replace(":", "")

                Dim yf As IO.FileStream = IO.File.Open(Path + "\Errors" + NowStr + ".log", IO.FileMode.Append)
                Dim verinfo As Byte() = New System.Text.UTF8Encoding(True).GetBytes(str)

                yf.Write(verinfo, 0, verinfo.Length)
                yf.Close()

            Catch exxx As Exception

            End Try
        End Try

    End Sub

End Class
