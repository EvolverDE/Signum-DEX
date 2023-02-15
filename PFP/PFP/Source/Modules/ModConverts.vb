
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

        If MessageIsHEXString(HEXStr) Then
            If HEXStr.Length Mod 2 > 0 Then
                HEXStr += "0"
            End If

            For i As Integer = 0 To HEXStr.Length - 1 Step 2

                Dim TStr As String = HEXStr.Substring(i, 2)
                TempBytlist.Add(Convert.ToByte(TStr, 16))

            Next

            Return TempBytlist.ToArray
        Else
            Return TempBytlist.ToArray
        End If

    End Function
    Function StringToHEXString(ByVal Input As String) As String
        Dim Output As String = ""
        For i As Integer = 0 To Input.Length - 1
            Dim Temp As String = Input.Substring(i, 1)
            Output += Hex(Asc(Temp))
        Next
        Return Output
    End Function


    Public Function HEXStringToULongList(ByVal HEXStr As String) As List(Of ULong)

        Dim InputBytes As List(Of Byte) = New List(Of Byte)

        If MessageIsHEXString(HEXStr) Then
            InputBytes = HEXStringToByteArray(HEXStr).ToList
        Else
            InputBytes = System.Text.Encoding.ASCII.GetBytes(HEXStr).ToList
        End If

        While InputBytes.Count Mod 8 <> 0
            InputBytes.Insert(0, &H0)
        End While

        InputBytes.Reverse()

        Dim T_ULongList As List(Of ULong) = New List(Of ULong)

        For i As Integer = 0 To InputBytes.Count - 1 Step 8
            T_ULongList.Add(BitConverter.ToUInt64(InputBytes.ToArray, i))
        Next

        Return T_ULongList

    End Function

    Public Function ULongListToHEXString(ByVal ULongList As List(Of ULong)) As String

        Dim HEXBytes As List(Of Byte) = New List(Of Byte)

        Dim AllZero As Boolean = True
        For Each UL As ULong In ULongList

            If UL > 0 Then
                AllZero = False
            End If

            HEXBytes.AddRange(BitConverter.GetBytes(UL))
        Next

        If AllZero Then
            Return ""
        End If

        HEXBytes.Reverse()

        While HEXBytes(0) = &H0 And HEXBytes.Count > 0
            HEXBytes.RemoveAt(0)
        End While

        Return ByteArrayToHEXString(HEXBytes.ToArray)

    End Function


End Module
