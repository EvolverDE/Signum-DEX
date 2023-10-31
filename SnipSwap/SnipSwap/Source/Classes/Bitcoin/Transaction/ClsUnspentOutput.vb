
Public Class ClsUnspentOutput
    Inherits AbsClsOutputs

    Property TransactionID As String = ""
    'Property Addresses As List(Of String) = New List(Of String)
    Property InputIndex As Integer = 0
    Property Confirmations As Integer = 0

    Property Sequence As ULong = 4294967295UL 'FFFFFFFF

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
