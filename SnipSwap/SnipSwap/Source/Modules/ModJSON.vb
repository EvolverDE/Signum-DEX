﻿Option Strict On
Option Explicit On
Imports System.Globalization
Imports System.Text.RegularExpressions

Module ModJSON

    Function GetBooleanBetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As Boolean

        Try

            For i As Integer = 0 To inputList.Count - 1
                Dim Entry As String = inputList(i)
                If Entry.Contains(startchar) Then
                    Return GetBooleanBetween(Entry, startchar, endchar)
                End If
            Next

            Return False

        Catch ex As Exception
            Return False
        End Try

    End Function
    Function GetIntegerBetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As Integer

        Try

            For i As Integer = 0 To inputList.Count - 1
                Dim Entry As String = inputList(i)
                If Entry.Contains(startchar) Then
                    Return GetIntegerBetween(Entry, startchar, endchar)
                End If
            Next

            Return 0

        Catch ex As Exception
            Return 0
        End Try

    End Function
    Function GetULongBetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As ULong

        Try

            For i As Integer = 0 To inputList.Count - 1
                Dim Entry As String = inputList(i)
                If Entry.Contains(startchar) Then
                    Return GetULongBetween(Entry, startchar, endchar)
                End If
            Next

            Return 0UL

        Catch ex As Exception
            Return 0UL
        End Try

    End Function
    Function GetDoubleBetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As Double

        Try

            For i As Integer = 0 To inputList.Count - 1
                Dim Entry As String = inputList(i)
                If Entry.Contains(startchar) Then
                    Return GetDoubleBetween(Entry, startchar, endchar)
                End If
            Next

            Return 0.0

        Catch ex As Exception
            Return 0.0
        End Try

    End Function
    Function GetDateBetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As Date

        Try

            For i As Integer = 0 To inputList.Count - 1
                Dim Entry As String = inputList(i)
                If Entry.Contains(startchar) Then
                    Return GetDateBetween(Entry, startchar, endchar)
                End If
            Next

            Return #01/01/0001#

        Catch ex As Exception
            Return #01/01/0001#
        End Try

    End Function
    Function GetStringBetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As String

        Try

            For i As Integer = 0 To inputList.Count - 1
                Dim Entry As String = inputList(i)
                If Entry.Contains(startchar) Then
                    Return GetStringBetween(Entry, startchar, endchar)
                End If
            Next

            Return ""

        Catch ex As Exception
            Return ""
        End Try

    End Function

    Function GetBooleanBetween(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional LastIdxOf As Boolean = False) As Boolean

        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)

                If LastIdxOf Then
                    input = input.Remove(input.LastIndexOf(endchar))
                Else
                    input = input.Remove(input.IndexOf(endchar))
                End If

                Return Convert.ToBoolean(input)

            End If
        End If

        Return False

    End Function
    Function GetIntegerBetween(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional LastIdxOf As Boolean = False) As Integer

        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)

                If LastIdxOf Then
                    input = input.Remove(input.LastIndexOf(endchar))
                Else
                    input = input.Remove(input.IndexOf(endchar))
                End If

                If IsNumeric(input) Then
                    Return Convert.ToInt32(input)
                Else
                    Return 0
                End If

            End If
        End If

        Return 0

    End Function
    Function GetULongBetween(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional LastIdxOf As Boolean = False) As ULong

        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)

                If LastIdxOf Then
                    input = input.Remove(input.LastIndexOf(endchar))
                Else
                    input = input.Remove(input.IndexOf(endchar))
                End If

                Return Convert.ToUInt64(input)

            End If
        End If

        Return 0UL

    End Function
    Function GetDoubleBetween(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional LastIdxOf As Boolean = False) As Double

        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)

                If LastIdxOf Then
                    input = input.Remove(input.LastIndexOf(endchar))
                Else
                    input = input.Remove(input.IndexOf(endchar))
                End If


                'Dim test = Double.Parse(Regex.Replace(input, "[.,]", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator))

                Return Convert.ToDouble(Regex.Replace(input, "[.,]", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator))

            End If
        End If

        Return 0.0

    End Function
    Function GetDateBetween(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional LastIdxOf As Boolean = False) As Date

        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)

                If LastIdxOf Then
                    input = input.Remove(input.LastIndexOf(endchar))
                Else
                    input = input.Remove(input.IndexOf(endchar))
                End If

                Return Convert.ToDateTime(input)

            End If
        End If

        Return #01/01/0001#

    End Function
    ''' <summary>
    ''' Einen String aus einem Bereich auslesen
    ''' </summary>
    ''' <param name="input">Der String, aus dem ein Bereich ausgelesen werden soll</param>
    ''' <param name="startchar">die Startmarkierung ab der gelesen werden soll</param>
    ''' <param name="endchar">die Endmarkierung bis der gelesen werden soll</param>
    ''' <param name="LastIdxOf">Legt fest, ob bis zum letzten endchar gelesen werden soll</param>
    ''' <returns>Vorzugsweise ein Double , andernfalls z.b. ein Integer</returns>
    ''' <remarks></remarks>
    Function GetStringBetween(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional LastIdxOf As Boolean = False, Optional ByVal CaseSensitive As Boolean = True) As String

        If IsNothing(input) Then
            Return ""
        End If

        If Not CaseSensitive Then
            startchar = startchar.ToLower()
            endchar = endchar.ToLower()

            Dim LowerInput As String = input.ToLower()
            Dim startidx = LowerInput.IndexOf(startchar)

            If startidx = -1 Then
                Return ""
            End If

            Dim OldTag As String = GetStringBetween(input.Substring(startidx), "<", ">")
            input = input.Replace(OldTag, startchar.Replace("<", "").Replace(">", ""))
        End If

        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)

                If LastIdxOf Then
                    input = input.Remove(input.LastIndexOf(endchar))
                Else
                    input = input.Remove(input.IndexOf(endchar))
                End If

                Return input

            End If
        End If

        Return ""

    End Function

    Function Between2List(ByVal Input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As List(Of String)
        Dim Temp1 As String = ""
        Dim Temp2 As String = ""

        Try

            If Input.Trim() <> "" Then
                If Input.Contains(startchar) And Input.Contains(endchar) Then

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

                    'abc[x],[],def
                    'abc[x[x]x],def

                    If SplitIdx <> -1 Then
                        Temp1 = Input.Remove(SplitIdx)
                    Else
                        Temp1 = Input.Substring(1)
                        Temp1 = Temp1.Remove(Temp1.Length)
                    End If

                    'abc[x
                    'abc[x[x]x

                    Temp1 = Temp1.Substring(FinalList(0) + 1)

                    'x
                    'x[x]x

                    Try

                        Dim FirstIDX As Integer = Input.IndexOf(Temp1)
                        Dim LastIDX As Integer = Input.LastIndexOf(Temp1)

                        If FirstIDX <> LastIDX Then
                            Temp2 = Input.Remove(FirstIDX, Temp1.Length)
                        Else
                            Temp2 = Input.Replace(Temp1, "")
                        End If

                    Catch ex As Exception

                    End Try

                    Dim Output As String = Temp1
                    Dim Rest As String = Temp2

                    Try
                        Dim FirstIDX As Integer = Input.IndexOf(Output)
                        Dim LastIDX As Integer = Input.LastIndexOf(Output)

                        If FirstIDX <> LastIDX Then
                            Rest = Input.Remove(FirstIDX, Output.Length)
                        Else
                            Rest = Input.Replace(Output, "")
                        End If

                    Catch ex As Exception

                    End Try

                    Return New List(Of String)({Output, Rest})

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