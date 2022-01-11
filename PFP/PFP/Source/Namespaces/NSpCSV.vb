
Option Strict On
Option Explicit On

Namespace CSVTool

    Public Class CSVReader
        Private _Path As String
        Private _Splitter As String
        Private _ListsParameter As List(Of List(Of String)) = New List(Of List(Of String))


        Sub New(Path As String, Optional ByVal Splitter As String = ";", Optional ByVal Decrypt As Boolean = False, Optional ByVal Password As String = "")
            Me.Path = Path
            Me.Splitter = Splitter
            ReadCSV(Decrypt, Password)
        End Sub

        Public Property Path As String
            Get
                Return _Path
            End Get
            Set(value As String)
                _Path = value
            End Set
        End Property

        Public Property Splitter As String
            Get
                Return _Splitter
            End Get
            Set(value As String)
                _Splitter = value
            End Set
        End Property

        Public ReadOnly Property Lists As List(Of List(Of String))
            Get
                Return _ListsParameter
            End Get
        End Property


        Private Sub ReadCSV(Optional ByVal Decrypt As Boolean = False, Optional ByVal Password As String = "")
            For Each Line As String In System.IO.File.ReadAllLines(Path, Text.Encoding.Default)

                If Decrypt Then
                    Line = AESDecrypt(Line, Password)
                End If
                Lists.Add(New List(Of String)(Line.Split(Convert.ToChar(Splitter))))
            Next

            Lists.RemoveAt(0)

        End Sub

    End Class

    Public Class CSVWriter
        Private _Path As String
        Private _Splitter As String
        Private _ListsParameter As List(Of List(Of String)) = New List(Of List(Of String))


        Sub New(Path As String, ByVal List As List(Of List(Of String)), Optional ByVal Splitter As String = ";", Optional ByVal Mode As String = "append", Optional ByVal Encrypt As Boolean = False, Optional ByVal Password As String = "")
            Me.Path = Path
            Me.Splitter = Splitter
            Me.Lists = List
            WriteCSV(Mode, Encrypt, Password)
        End Sub


        Public Property Path As String
            Get
                Return _Path
            End Get
            Set(value As String)
                _Path = value
            End Set
        End Property

        Public Property Splitter As String
            Get
                Return _Splitter
            End Get
            Set(value As String)
                _Splitter = value
            End Set
        End Property

        Public WriteOnly Property Lists As List(Of List(Of String))
            Set(ByVal value As List(Of List(Of String)))
                _ListsParameter = value
            End Set
        End Property

        Private Sub WriteCSV(ByVal Mode As String, Optional ByVal Encrypt As Boolean = False, Optional ByVal Password As String = "")
            Try

                Dim MaxLen As Integer = 0
                For Each LineAry As List(Of String) In _ListsParameter
                    If LineAry.Count > MaxLen Then
                        MaxLen = LineAry.Count - 1
                    End If
                Next

                Dim Lines As List(Of String) = New List(Of String)
                For Each LineAry As List(Of String) In _ListsParameter

                    Dim Line As String = ""
                    Dim LineLen As Integer = 0
                    For i As Integer = 0 To LineAry.Count - 1
                        Dim LineItem As String = LineAry(i)

                        Line += LineItem + Splitter
                        LineLen = i
                    Next

                    If LineLen < MaxLen Then
                        Line += Splitter
                    End If

                    If Encrypt Then
                        Lines.Add(AESEncrypt2HEXStr(Line, Password))
                    Else
                        Lines.Add(Line)
                    End If

                Next

                If Mode = "append" Then
                    Dim x As IO.StreamWriter = System.IO.File.AppendText(Path)

                    For Each Line As String In Lines
                        x.WriteLine(Line)
                    Next

                    x.Close()
                Else
                    System.IO.File.WriteAllLines(Path, Lines.ToArray, Text.Encoding.Default)
                End If

            Catch ex As Exception
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in WriteCSV(): -> " + ex.Message)
                End If

            End Try

        End Sub

    End Class

End Namespace