Public MustInherit Class AbsClsOutputs

    Property Addresses As List(Of String) = New List(Of String)
    Property AmountNQT As ULong = 0UL
    Property Spendable As Boolean = False
    Property LengthOfScript As Integer = 0
    Property ScriptHex As String = ""
    Property Script As List(Of ClsScriptEntry) = New List(Of ClsScriptEntry)
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
