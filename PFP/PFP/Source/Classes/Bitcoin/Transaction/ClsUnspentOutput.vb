
Public Class ClsUnspentOutput
    Inherits AbsClsOutputs

    Property TransactionID As String = ""
    'Property Addresses As List(Of String) = New List(Of String)
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

    ReadOnly Property GetScriptString As String
        Get
            Dim T_LS As String = ""
            For Each s As ClsScriptEntry In Script
                T_LS += s.Key.ToString
                If s.ValueHex.Length > 2 Then
                    T_LS += "=" + s.ValueHex + " "
                Else
                    T_LS += " "
                End If
            Next

            Return T_LS

        End Get
    End Property


    Property InputIndex As Integer = 0
    Property Confirmations As Integer = 0
    Property Sequence As ULong = 4294967295UL

    Public Structure S_UTXEntry
        Dim Key As E_UTXEntry
        Dim Value As String
    End Structure

    Public Enum E_UTXEntry
        version = 0
        numberOfInputs = 1
        transactionID = 2
        unspentOutputIndex = 3
        inputScriptLength = 4
        inputScript = 5
        sequence = 6
        numberOfOutputs = 7
        amountNQT = 8
        outputScriptLength = 9
        outputScript = 10
        locktime = 11
        hashCodeType = 12
        signature = -1
        recipientPublicKey = -2
    End Enum

    Private Property C_UnsignedTransaction As List(Of S_UTXEntry) = New List(Of S_UTXEntry)
    Public Property UnsignedTransaction As List(Of S_UTXEntry)
        Set(value As List(Of S_UTXEntry))
            C_UnsignedTransaction = value
        End Set
        Get
            Return C_UnsignedTransaction
        End Get
    End Property

    Property UnsignedTransactionHex As String = ""
    Property SignerAddress As String = ""
    Property SignatureScript As List(Of ClsScriptEntry) = New List(Of ClsScriptEntry)

    Sub New()

    End Sub

End Class
