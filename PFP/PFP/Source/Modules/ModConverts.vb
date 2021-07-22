
Module ModConverts

    Public Function ByteArrayToHEXString(ByVal BytAry() As Byte) As String

        Dim RetStr As String = ""

        Dim ParaBytes As List(Of Byte) = BytAry.ToList

        For Each ParaByte As Byte In ParaBytes
            Dim T_RetStr As String = Conversion.Hex(ParaByte)

            If T_RetStr.Length < 2 Then
                T_RetStr = "0" + T_RetStr
            End If

            RetStr += T_RetStr

        Next

        Return RetStr.ToLower

    End Function

    Public Function HEXStringToByteArray(ByVal HEXStr As String) As Byte()

        Dim TempBytlist As List(Of Byte) = New List(Of Byte)

        If HEXStr.Length Mod 2 > 0 Then
            HEXStr += "0"
        End If

        For i As Integer = 0 To HEXStr.Length - 1 Step 2

            Dim TStr As String = HEXStr.Substring(i, 2)
            TempBytlist.Add(Convert.ToByte(TStr, 16))

        Next

        Return TempBytlist.ToArray

    End Function

    Function StringToHEXString(ByVal Input As String) As String
        Dim Output As String = ""
        For i As Integer = 0 To Input.Length - 1
            Dim Temp As String = Input.Substring(i, 1)
            Output += Hex(Asc(Temp))
        Next
        Return Output
    End Function

End Module
