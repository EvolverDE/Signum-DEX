
Module ModJSON

    Function BetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional ByVal GetListIndex As Boolean = False, Optional ByVal GetTyp As Object = Nothing) As Object

        Try

            For i As Integer = 0 To inputList.Count - 1
                Dim Entry As String = inputList(i)
                If Entry.Contains(startchar) Then

                    If GetListIndex Then
                        Return i.ToString
                    Else
                        Return Between(Entry, startchar, endchar, GetTyp)
                    End If

                End If

            Next

            If GetListIndex Then
                Return "-1"
            Else
                Return ""
            End If

        Catch ex As Exception

            If GetListIndex Then
                Return "-1"
            Else
                Return ""
            End If

        End Try

    End Function


    ''' <summary>
    ''' Einen String aus einem Bereich auslesen
    ''' </summary>
    ''' <param name="input">Der String, aus dem ein Bereich ausgelesen werden soll</param>
    ''' <param name="startchar">die Startmarkierung ab der gelesen werden soll</param>
    ''' <param name="endchar">die Endmarkierung bis der gelesen werden soll</param>
    ''' <param name="GetTyp">Der Typ der Rückgabewert zurückgegeben werden soll (z.b. GetType(Double))</param>
    ''' <returns>Vorzugsweise ein Double , andernfalls z.b. ein Integer</returns>
    ''' <remarks></remarks>
    Function Between(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional ByVal GetTyp As Object = Nothing, Optional LastIdxOf As Boolean = False) As Object

        'TODO: OUT from Between
        'If GetINISetting(E_Setting.InfoOut, False) Then
        '    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
        '    Out.ErrorLog2File(Application.ProductName + "-error in SendMessages(): -> " + ex.Message)
        'End If



        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)

                If LastIdxOf Then
                    input = input.Remove(input.LastIndexOf(endchar))
                Else
                    input = input.Remove(input.IndexOf(endchar))
                End If


                If IsNothing(GetTyp) Then
                    Return input
                Else
                    Select Case GetTyp.Name
                        Case GetType(Boolean).Name
                            Try
                                Return CBool(input)
                            Catch ex As Exception
                                Return False
                            End Try

                        Case GetType(Integer).Name
                            Try
                                Return CInt(input)
                            Catch ex As Exception
                                Return 0
                            End Try

                        Case GetType(ULong).Name
                            Try
                                Return CULng(input)
                            Catch ex As Exception
                                Return 0UL
                            End Try

                        Case GetType(Double).Name
                            Try
                                Return Val(input.Replace(",", "."))
                            Catch ex As Exception
                                Return 0.0
                            End Try

                        Case GetType(Date).Name
                            Try
                                Return CDate(input)
                            Catch ex As Exception
                                Return Now
                            End Try

                        Case GetType(String).Name
                            Return input
                    End Select
                End If

            End If
        End If

        If IsNothing(GetTyp) Then
            Return 0.0
        Else
            Select Case GetTyp.Name
                Case GetType(Boolean).Name
                    Return False
                Case GetType(Integer).Name
                    Return 0
                Case GetType(ULong).Name
                    Return 0UL
                Case GetType(Double).Name
                    Return 0.0
                Case GetType(Date).Name
                    Return Now
                Case GetType(String).Name
                    Return ""
                Case Else
                    Return 0
            End Select
        End If

    End Function


    Function Between2List(ByVal Input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As List(Of String) ', Optional ByVal LastIdx As Boolean = False) As List(Of String)

        Dim Output As String = ""
        Dim Rest As String = ""
        Dim Temp1 As String = ""
        Dim Temp2 As String = ""

        Try

            If Input.Trim <> "" Then
                If Input.Contains(startchar) And Input.Contains(endchar) Then

                    'Dim StartIdx As Integer = -1
                    'Dim EndIdx As Integer = -1
                    Dim CntIdx As Integer = 0


                    Dim StartList As List(Of Integer) = CharCounterList(Input, startchar)
                    Dim EndList As List(Of Integer) = CharCounterList(Input, endchar)

                    Dim FinalList As List(Of Integer) = New List(Of Integer)

                    FinalList.AddRange(StartList.ToArray)
                    FinalList.AddRange(EndList.ToArray)

                    FinalList.Sort()

                    Dim SplitIdx As Integer = -1

                    For Each Idx As Integer In FinalList

                        Dim Ch As String = Input.Substring(Idx, 1)

                        If Ch = startchar Then
                            CntIdx += 1
                        ElseIf Ch = endchar Then
                            CntIdx -= 1
                        End If

                        If CntIdx = 0 Then
                            SplitIdx = Idx
                            Exit For
                        End If

                    Next

                    If SplitIdx <> -1 Then
                        Temp1 = Input.Remove(SplitIdx)
                    End If

                    Temp1 = Temp1.Remove(FinalList(0), 1)


                    Try
                        Temp2 = Input.Replace(Temp1, "")
                    Catch ex As Exception

                    End Try

                    Output = Temp1
                    Rest = Temp2

                    Try
                        Rest = Input.Replace(Output, "")
                    Catch ex As Exception

                    End Try


                    If Output.Trim = "" Then
                        Return New List(Of String)({Output, Rest})
                    Else
                        Return New List(Of String)({Output, Rest})
                    End If


                End If
            End If

        Catch ex As Exception
            Return New List(Of String)
        End Try

        Return New List(Of String)

    End Function

    Function CharCounterList(ByVal Input As String, StartChar As String) As List(Of Integer)

        Dim StartIdx As Integer = -1
        Dim StartList As List(Of Integer) = New List(Of Integer)

        While Not StartIdx = 0

            If Not StartIdx = 0 Then

                If StartIdx = -1 Then StartIdx = 1

                StartIdx = InStr(StartIdx, Input, StartChar)

                If StartIdx > 0 Then
                    If Not StartIdx = 0 Then
                        StartList.Add(StartIdx - 1)
                        StartIdx += 1
                    End If
                End If
            End If

        End While

        Return StartList

    End Function


End Module