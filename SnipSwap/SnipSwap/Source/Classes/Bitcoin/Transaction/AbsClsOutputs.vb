Public MustInherit Class AbsClsOutputs

    Property Addresses As List(Of String) = New List(Of String)

    ReadOnly Property GetAddressesString As String
        Get
            Dim T_Addresses As String = ""
            For Each T_Address In Addresses
                T_Addresses += T_Address + ";"
            Next
            T_Addresses = T_Addresses.Remove(T_Addresses.Length - 1)

            Return T_Addresses

        End Get
    End Property

    Property AmountNQT As ULong = 0UL
    Property Spendable As Boolean = False
    Property LengthOfScript As Integer = 0
    Property ScriptHex As String = ""
    Property Script As List(Of ClsScriptEntry) = New List(Of ClsScriptEntry)
    ReadOnly Property GetScriptString As String
        Get
            Dim T_LS As String = ""
            For Each s As ClsScriptEntry In Script
                T_LS += s.Key.ToString
                If s.ValueHex.Length > 2 Or Not s.Key.ToString().Contains("OP") Then
                    T_LS += "=" + s.ValueHex + " "
                Else
                    T_LS += " "
                End If
            Next

            Return T_LS.Trim()

        End Get
    End Property
    Property ScriptHash As String = ""
    Property OutputType As E_Type

    Public Enum E_Type
        Standard = 0
        ChainSwapHash = 1
        LockTime = 2
        ChainSwapHashWithLockTime = 3
        Pay2ScriptHash = 4
        Unknown = 5
    End Enum

End Class
