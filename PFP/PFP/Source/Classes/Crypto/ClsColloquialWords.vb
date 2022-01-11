Option Strict On
Option Explicit On

Public Class ClsColloquialWords

    Public Property RandomWord As String = ""

    Sub New()
        RandomWord = GenerateColloquialWords()
    End Sub

    Public Function GenerateColloquialWords(Optional ByVal Seed As String = "", Optional ByVal Caps As Boolean = False, Optional ByVal Separator As String = " ", Optional ByVal Length As Integer = 0) As String

        Dim ColloquialWords As String = ""

        If Seed.Trim = "" Then
            ColloquialWords = CreateWord(, Length)

            If Caps Then
                ColloquialWords = ColloquialWords.ToUpper
            Else
                ColloquialWords = ColloquialWords.ToLower
            End If

        Else
            Dim Byt() As Byte = HEXStringToByteArray(StringToHEXString(Seed))

            Dim CrossSection As Integer = 0

            For Each Byta In Byt
                CrossSection += Byta
            Next

            CrossSection = Convert.ToInt32(CrossSection / 255)

            If CrossSection = 0 Then
                CrossSection = 1
            End If

            Dim ExtraInt As String = Math.Round(Byt.Length / CrossSection, 5).ToString

            If ExtraInt = "NaN" Then
                ExtraInt = "1"
            End If

            If ExtraInt.Contains(",") Then
                ExtraInt = ExtraInt.Remove(ExtraInt.LastIndexOf(","))
            End If

            Dim WordLen As Integer = Integer.Parse(ExtraInt)

            If Length > 0 Then

                If Seed.Length > Length Then
                    WordLen = Length
                    CrossSection = Convert.ToInt32(Byt.Length / Length)
                End If

            End If

            Dim WordList As List(Of String) = New List(Of String)

            If Byt.Length / CrossSection Mod WordLen = 0 Then

                Dim Word As String = ""
                For j As Integer = 0 To CrossSection - 1

                    If Not (j * WordLen) + WordLen > Seed.Length Then
                        Word += Seed.Substring(j * WordLen, WordLen)
                    Else
                        Word += Seed.Substring(j * WordLen)
                    End If

                    If Word.Length = WordLen Then

                        WordList.Add(Word)
                        Word = ""

                    End If

                Next

            Else

                Dim Word As String = ""

                For j As Integer = 0 To CrossSection - 1

                    If Not (j * WordLen) + WordLen > Seed.Length Then
                        Word += Seed.Substring(j * WordLen, WordLen)
                    Else
                        Word += Seed.Substring(j * WordLen)
                    End If

                    If Word.Length = WordLen Then

                        If j = CrossSection - 1 Then
                            Word = Seed.Substring(j * WordLen)
                        End If

                        WordList.Add(Word)
                        Word = ""
                    Else
                        WordList.Add(Word)
                        Word = ""
                    End If

                Next

            End If

            For Each Word As String In WordList

                Dim Temp As String = CreateWord(Word)

                If Caps Then
                    ColloquialWords += Temp.ToUpper + Separator
                Else
                    ColloquialWords += Temp.ToLower + Separator
                End If

            Next

            If Not ColloquialWords.Trim = "" Then
                ColloquialWords = ColloquialWords.Remove(ColloquialWords.Length - 1)
            End If

        End If

        Return ColloquialWords.Trim

    End Function
    Public Function CreateWord(Optional ByVal Seed As String = "", Optional ByVal Length As Integer = 0) As String

        Dim T_Pass As String = ""

        If Seed = "" Then

            Dim Len As Integer = Length
            If Len = 0 Then

                For i As Integer = 0 To Now.Millisecond

                    If i Mod 2 = 0 Then
                        Len = 4
                    ElseIf i Mod 3 = 0 Then
                        Len = 5
                    ElseIf i Mod 4 = 0 Then
                        Len = 6
                    ElseIf i Mod 5 = 0 Then
                        Len = 7
                    Else
                        Len = 8
                    End If

                    Rnd()

                Next

            End If

            For i As Integer = 0 To Now.Millisecond
                Rnd()
            Next

            Dim rand As String = CStr(2 * Rnd() + 1)
            If rand.Contains(",") Then
                rand = rand.Remove(rand.IndexOf(","))
            End If
            Dim ri As Integer = Integer.Parse(rand)

            Dim FlipFlop As Boolean = False
            If ri = 1 Then
                FlipFlop = True
            End If


            For i As Integer = 0 To Len - 1

                If FlipFlop Then
                    FlipFlop = False
                    Dim RandomByt As Byte = CByte((254 * Rnd()) + 1)
                    T_Pass += GetChar(RandomByt)
                Else
                    FlipFlop = True
                    Dim RandomByt As Byte = CByte((254 * Rnd()) + 1)
                    T_Pass += GetChar(RandomByt, True)
                End If

            Next

        Else

            Dim Byt() As Byte = HEXStringToByteArray(StringToHEXString(Seed))
            Dim Len As Integer = Byt.Length

            For i As Integer = 0 To Len - 1

                Dim TT_Byt As Byte = Byt(i)

                If i Mod 2 = 0 Then
                    T_Pass += GetChar(TT_Byt)
                Else
                    T_Pass += GetChar(TT_Byt, True)
                End If

            Next

        End If

        If T_Pass.Substring(T_Pass.Length - 1) = "j" Then
            T_Pass = T_Pass.Remove(T_Pass.Length - 1)
            T_Pass += "i"
        End If

        Return T_Pass

    End Function
    Private Function GetChar(Optional ByVal Byt As Byte = 255, Optional ByVal Vowel_Mutation As Boolean = False) As String

        Dim Byt2 As Byte = Byt
        Dim T_Byt As Byte = 0
        Dim T_BytStr As String = ""

        If Not Vowel_Mutation Then

            If Byt2 > 21 Then
                T_BytStr = Math.Round(Byt / 21, 5, MidpointRounding.ToEven).ToString

                If T_BytStr.Contains(",") Then
                    T_BytStr = T_BytStr.Remove(T_BytStr.IndexOf(","))
                End If

                T_Byt = CByte(T_BytStr)

                If T_Byt = 0 Then
                    T_Byt = 1
                End If

                Byt2 = Convert.ToByte(Byt2 - (T_Byt * 21))
            End If

            If Byt2 = 0 Then
                Byt2 = 1
            End If

            Select Case Byt2
                Case 1
                    Return "b"
                Case 2
                    Return "c"
                Case 3
                    Return "d"
                Case 4
                    Return "f"
                Case 5
                    Return "g"
                Case 6
                    Return "h"
                Case 7
                    Return "j"
                Case 8
                    Return "k"
                Case 9
                    Return "l"
                Case 10
                    Return "m"
                Case 11
                    Return "n"
                Case 12
                    Return "p"
                Case 13
                    Return "k" 'Q
                Case 14
                    Return "r"
                Case 15
                    Return "s"
                Case 16
                    Return "t"
                Case 17
                    Return "v"
                Case 18
                    Return "w"
                Case 19
                    Return "x"
                Case 20
                    Return "y"
                Case 21
                    Return "z"
                Case Else
                    Return "error"
            End Select

        Else

            If Byt2 > 5 Then

                T_BytStr = Math.Round(Byt / 5, 5, MidpointRounding.ToEven).ToString

                If T_BytStr.Contains(",") Then
                    T_BytStr = T_BytStr.Remove(T_BytStr.IndexOf(","))
                End If

                T_Byt = CByte(T_BytStr)
                If T_Byt = 0 Then
                    T_Byt = 1
                End If

                Byt2 = Convert.ToByte(Byt2 - (T_Byt * 5))
            End If

            If Byt2 = 0 Then
                Byt2 = 1
            End If

            Select Case Byt2
                Case 1
                    Return "a"
                Case 2
                    Return "e"
                Case 3
                    Return "i"
                Case 4
                    Return "o"
                Case 5
                    Return "u"
                Case Else
                    Return "error"
            End Select

        End If

    End Function

End Class