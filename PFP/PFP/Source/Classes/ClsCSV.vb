
Option Strict On
Option Explicit On

Public Class ClsCSV
    Private C_Path As String
    Private C_Splitter As String

    Public Property ColumnList As List(Of String) = New List(Of String)({"ATID", "PFPAT", "HistoryOrders", "MyCurrentOrderTX", "Type", "Paytype", "Infotext", "AutoSendInfotext", "AutoCompleteAT", "Status"})
    Private C_RowList As List(Of List(Of String)) = New List(Of List(Of String))
    Private C_AppendRowList As List(Of List(Of String)) = New List(Of List(Of String))
    Private C_Close As Boolean = False

    Private C_Encrypt As Boolean = False
    Private C_Decrypt As Boolean = False
    Private C_Password As String = ""
    Private C_WriteMode As E_WriteMode
    Private C_BusyFlag As Boolean = False

    Private Property C_Timer As Timer
    Private C_RefreshTimerTicks As Integer = 0

#Region "properties"
    Public Property Path As String
        Get
            Return C_Path
        End Get
        Set(value As String)
            C_Path = value
        End Set
    End Property
    Public Property Splitter As String
        Get
            Return C_Splitter
        End Get
        Set(value As String)
            C_Splitter = value
        End Set
    End Property
    Public Property RowList As List(Of List(Of String))
        Get
            Return C_RowList
        End Get
        Set(value As List(Of List(Of String)))
            C_RowList = value
        End Set
    End Property

    Public ReadOnly Property BusyFlag As Boolean
        Get
            Return C_BusyFlag
        End Get
    End Property

#End Region

    Public Enum E_WriteMode
        Append = 0
        Create = 1
    End Enum

    Sub New(Path As String, Optional ByVal Splitter As String = ";", Optional ByVal Decrypt As Boolean = False, Optional ByVal Password As String = "")

        C_Timer = New Timer With {.Enabled = True, .Interval = 50}
        C_Timer.Start()

        AddHandler C_Timer.Tick, AddressOf SerialWriter

        C_Path = Path
        C_Splitter = Splitter
        ReadCSV(Decrypt, Password)
    End Sub
    Public Sub Closer()
        C_Close = True
    End Sub

    Private Sub ReadCSV(Optional ByVal Decrypt As Boolean = False, Optional ByVal Password As String = "")

        C_Decrypt = Decrypt
        If Not Password = "" Then
            C_Password = Password
        End If

        If System.IO.File.Exists(Path) Then

            Dim T_CSV As List(Of String) = New List(Of String)

            If Decrypt Then
                Dim Bytes As List(Of Byte) = System.IO.File.ReadAllBytes(Path).ToList

                Dim PlainText As String = AESDecrypt(Bytes.ToArray, Password)
                T_CSV.AddRange(PlainText.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            Else
                T_CSV = System.IO.File.ReadAllLines(Path, Text.Encoding.Default).ToList
            End If

            If T_CSV.Count > 0 Then
                C_RowList.Clear()
            End If

            For i As Integer = 0 To T_CSV.Count - 1
                Dim T_Row As String = T_CSV(i)
                C_RowList.Add(New List(Of String)(T_Row.Split(Convert.ToChar(Splitter))))
            Next

            If C_RowList.Count > 0 Then
                C_RowList.RemoveAt(0) 'Remove ColumnLine
            End If

        Else

        End If

    End Sub

    Public Sub SetIntoCSV(ByVal RowList As List(Of String), Optional ByVal WriteMode As E_WriteMode = E_WriteMode.Append, Optional ByVal Encrypt As Boolean = False, Optional ByVal Password As String = "")

        If WriteMode = E_WriteMode.Create Then
            C_RowList.Clear()
            C_RowList.Add(RowList)
        Else
            C_RowList.Add(RowList)
            C_AppendRowList.Add(RowList)
        End If

        WriteCSV(WriteMode, Encrypt, Password)

    End Sub
    Public Sub WriteCSV(ByVal WriteMode As E_WriteMode, Optional ByVal Encrypt As Boolean = False, Optional ByVal Password As String = "")
        C_WriteMode = WriteMode
        C_Encrypt = Encrypt

        If Not Password = "" Then
            C_Password = Password
        End If

        C_BusyFlag = True

    End Sub
    Private Sub SerialWriter(ByVal Sender As Object, ByVal E As EventArgs)

        If C_Close Then
            C_Timer.Stop()
            C_Timer.Enabled = False
            Exit Sub
        End If

        C_RefreshTimerTicks += 1

        If C_RefreshTimerTicks >= 50 Then
            C_RefreshTimerTicks = 0

            If C_BusyFlag Then
                C_BusyFlag = False
                C_Timer.Enabled = False

                Dim MaxLen As Integer = 0

                Dim T_RowList As List(Of List(Of String)) = New List(Of List(Of String))(RowList.ToArray)


                If C_WriteMode = E_WriteMode.Append Then
                    T_RowList = C_AppendRowList.ToList
                    C_AppendRowList.Clear()
                End If

                For i As Integer = 0 To T_RowList.Count - 1

                    Dim LineAry As List(Of String) = T_RowList(i)

                    If LineAry.Count > MaxLen Then
                        MaxLen = LineAry.Count - 1
                    End If
                Next

                Dim FileLines As List(Of String) = New List(Of String)
                Dim FileString As String = ""

                Dim ColumnLine As String = ""

                For Each Column As String In ColumnList
                    ColumnLine += Column + Splitter
                Next

                If ColumnList.Count < MaxLen Then
                    For i As Integer = ColumnList.Count To MaxLen - 1
                        ColumnLine += Splitter
                    Next
                End If

                If C_Encrypt Then
                    FileString += ColumnLine + vbCrLf
                Else
                    FileLines.Add(ColumnLine)
                End If


                For ii As Integer = 0 To T_RowList.Count - 1
                    Dim Row As List(Of String) = T_RowList(ii)

                    Dim RowString As String = ""
                    Dim LineLen As Integer = 0
                    For i As Integer = 0 To Row.Count - 1
                        Dim RowItem As String = Row(i)

                        RowString += RowItem + Splitter
                        LineLen = i
                    Next

                    If LineLen < MaxLen Then
                        For i As Integer = LineLen To MaxLen - 1
                            RowString += Splitter
                        Next
                    End If

                    If C_Encrypt Then
                        FileString += RowString + vbCrLf
                    Else
                        FileLines.Add(RowString)
                    End If

                Next


                If C_WriteMode = E_WriteMode.Append Then

                    If C_Encrypt Then
                        'TODO: Append AES Bytes to File
                    Else

                        Dim x As IO.StreamWriter = System.IO.File.AppendText(Path)

                        For Each Line As String In FileLines
                            x.WriteLine(Line)
                        Next

                        x.Close()
                    End If

                Else

                    If C_Encrypt Then
                        Dim Bytes As List(Of Byte) = New List(Of Byte)(AESEncrypt2ByteArray(FileString, C_Password).ToArray)
                        System.IO.File.WriteAllBytes(Path, Bytes.ToArray)
                    Else
                        System.IO.File.WriteAllLines(Path, FileLines.ToArray, Text.Encoding.Default)
                    End If

                End If

                C_Timer.Enabled = True

            End If

        End If

    End Sub

End Class