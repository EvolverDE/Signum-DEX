
'Imports SnipSwap.ClsDEXContract

Public Class ClsSignumTransaction

    Private Property C_SignumAPI As ClsSignumAPI = Nothing
    Private Property C_SignumNET As ClsSignumNET = New ClsSignumNET()

    Private Property C_Timestamp As ULong = 0UL
    Public ReadOnly Property Timestamp As ULong
        Get
            Return C_Timestamp
        End Get
    End Property

    Public ReadOnly Property DateTimeStamp As Date
        Get
            Return ClsSignumAPI.UnixToTime(C_Timestamp)
        End Get
    End Property

    Private Property C_Transaction As ULong = 0UL
    Public ReadOnly Property Transaction As ULong
        Get
            Return C_Transaction
        End Get
    End Property

    Private Property C_Block As String = ""
    Public ReadOnly Property Block As String
        Get
            Return C_Block
        End Get
    End Property

    Private Property C_Height As ULong = 0UL
    Public ReadOnly Property Height As ULong
        Get
            Return C_Height
        End Get
    End Property

    Private Property C_SenderPublicKey As String = ""
    Public ReadOnly Property SenderPublicKey As String
        Get
            Return C_SenderPublicKey
        End Get
    End Property

    Private Property C_SenderID As ULong = 0UL
    Public ReadOnly Property SenderID As ULong
        Get
            Return C_SenderID
        End Get
    End Property

    Private Property C_SenderAddress As String = ""
    Public ReadOnly Property SenderAddress As String
        Get
            Return C_SenderAddress
        End Get
    End Property


    Private Property C_RecipientPublicKey As String = ""
    Public ReadOnly Property RecipientPublicKey As String
        Get
            Return C_RecipientPublicKey
        End Get
    End Property

    Private Property C_RecipientID As ULong = 0UL
    Public ReadOnly Property RecipientID As ULong
        Get
            Return C_RecipientID
        End Get
    End Property

    Private Property C_RecipientAddress As String = ""
    Public ReadOnly Property RecipientAddress As String
        Get
            Return C_RecipientAddress
        End Get
    End Property

    Private Property C_AmountNQT As ULong = 0UL
    Public ReadOnly Property AmountNQT As ULong
        Get
            Return C_AmountNQT
        End Get
    End Property
    Public ReadOnly Property Amount As Double
        Get
            Return ClsSignumAPI.Planck2Dbl(AmountNQT)
        End Get
    End Property
    Public ReadOnly Property AmountString As String
        Get
            Return String.Format("{0:#0.00000000}", Amount)
        End Get
    End Property

    Private Property C_FeeNQT As ULong = 0UL
    Public ReadOnly Property FeeNQT As ULong
        Get
            Return C_FeeNQT
        End Get
    End Property
    Public ReadOnly Property Fee As Double
        Get
            Return ClsSignumAPI.Planck2Dbl(FeeNQT)
        End Get
    End Property
    Public ReadOnly Property FeeString As String
        Get
            Return String.Format("{0:#0.00000000}", Fee)
        End Get
    End Property

    Private Property C_BalanceNQT As ULong = 0UL
    Public ReadOnly Property BalanceNQT As ULong
        Get
            Return C_BalanceNQT
        End Get
    End Property
    Public ReadOnly Property Balance As Double
        Get
            Return ClsSignumAPI.Planck2Dbl(BalanceNQT)
        End Get
    End Property
    Public ReadOnly Property BalanceString As String
        Get
            Return String.Format("{0:#0.00000000}", Balance)
        End Get
    End Property


    Private Property C_Confirmations As Integer = -1
    Public ReadOnly Property Confirmations As Integer
        Get
            Return C_Confirmations
        End Get
    End Property

    Private Property C_Attachment As String = ""
    Public ReadOnly Property Attachment As String
        Get
            Return C_Attachment
        End Get
    End Property

    Private Property C_Message As String = ""
    Public ReadOnly Property Message As String
        Get
            Return C_Message
        End Get
    End Property

    Public Enum E_Type
        NONE = -1
        PAYMENT_TO_ADDRESS = 0
        PAYMENT_TO_CONTRACT = 1
        ARBITRARY_MESSAGE = 2
        ENCRYPTED_MESSAGE = 3
        DATA_MESSAGE = 4
        ENCRYPTED_DATA_MESSAGE = 5
        AUTOMATED_TRANSACTIONS_CREATION = 6
    End Enum

    Private Property C_Type As E_Type = E_Type.NONE
    Public ReadOnly Property Type As E_Type
        Get
            Return C_Type
        End Get
    End Property

    Private Property C_ReferenceCommand As ClsDEXContract.E_ReferenceCommand = ClsDEXContract.E_ReferenceCommand.NONE
    Public ReadOnly Property ReferenceCommand As ClsDEXContract.E_ReferenceCommand
        Get
            Return C_ReferenceCommand
        End Get
    End Property
    Sub New(ByVal TransactionID As ULong, Optional ByVal Node As String = "")
        C_SignumAPI = New ClsSignumAPI(Node)
        LoadTransaction(TransactionID)
    End Sub

    Sub New(ByVal TransactionID As ULong, ByVal PassPhrase As String, Optional ByVal Node As String = "")
        C_SignumAPI = New ClsSignumAPI(Node)
        LoadTransaction(TransactionID)
        DecryptAttachment(PassPhrase)
    End Sub

    Private Sub LoadTransaction(ByVal TransactionID As ULong)

        Dim TXL As List(Of String) = New List(Of String)(C_SignumAPI.GetTransaction(TransactionID).ToArray())

        C_Timestamp = GetULongBetweenFromList(TXL, "<timestamp>", "</timestamp>")

        C_Transaction = GetULongBetweenFromList(TXL, "<transaction>", "</transaction>")
        C_Block = GetStringBetweenFromList(TXL, "<block>", "</block>")
        C_Height = GetULongBetweenFromList(TXL, "<height>", "</height>")

        C_SenderPublicKey = GetStringBetweenFromList(TXL, "<senderPublicKey>", "</senderPublicKey>")
        C_SenderID = GetULongBetweenFromList(TXL, "<sender>", "</sender>")
        C_SenderAddress = GetStringBetweenFromList(TXL, "<senderRS>", "</senderRS>")

        C_RecipientPublicKey = GetStringBetweenFromList(TXL, "<recipientPublicKey>", "</recipientPublicKey>")
        C_RecipientID = GetULongBetweenFromList(TXL, "<recipient>", "</recipient>")
        C_RecipientAddress = GetStringBetweenFromList(TXL, "<recipientRS>", "</recipientRS>")

        C_AmountNQT = GetULongBetweenFromList(TXL, "<amountNQT>", "</amountNQT>")
        C_FeeNQT = GetULongBetweenFromList(TXL, "<feeNQT>", "</feeNQT>")
        C_BalanceNQT = GetULongBetweenFromList(TXL, "<balanceNQT>", "</balanceNQT>")

        C_Confirmations = GetIntegerBetweenFromList(TXL, "<confirmations>", "</confirmations>")
        C_Attachment = GetStringBetweenFromList(TXL, "<attachment>", "</attachment>")

        If C_Attachment.Trim() = "" Then
            C_Attachment = TXL.FirstOrDefault(Function(c) c.ToLower().Contains("message"))
        End If

        If C_Attachment.Trim() = "" Then
            If C_RecipientPublicKey = "0000000000000000000000000000000000000000000000000000000000000000" Then
                C_Type = E_Type.PAYMENT_TO_CONTRACT
            Else
                C_Type = E_Type.PAYMENT_TO_ADDRESS
            End If
        ElseIf C_Attachment.Contains("<encryptedMessage>") Then
            If GetBooleanBetween(C_Attachment, "<isText>", "</isText>") Then
                C_Type = E_Type.ENCRYPTED_MESSAGE
            Else
                C_Type = E_Type.ENCRYPTED_DATA_MESSAGE
            End If
        ElseIf C_Attachment.Contains("<message>") Then
            If GetBooleanBetween(C_Attachment, "<messageIsText>", "</messageIsText>") Then
                C_Type = E_Type.ARBITRARY_MESSAGE
                C_Message = GetStringBetween(C_Attachment, "<message>", "</message>")
                If MessageIsHEXString(C_Message) Then
                    C_Message = HEXStringToString(C_Message)
                End If
            Else
                C_Type = E_Type.DATA_MESSAGE
                C_Message = GetStringBetween(C_Attachment, "<message>", "</message>")

                Dim ReferenceMessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(C_Message)
                C_ReferenceCommand = ClsDEXContract.GetReferenceCommand(ReferenceMessageList(0))

            End If
        ElseIf C_Attachment.Contains("<version.AutomatedTransactionsCreation>") Then
            C_Type = E_Type.AUTOMATED_TRANSACTIONS_CREATION
            C_Message = GetStringBetween(C_Attachment, "<creationBytes>", "</creationBytes>")
        End If

    End Sub

    Public Function DecryptAttachment(ByVal PassPhrase As String) As Boolean

        Dim Returner As Boolean = False

        If (C_Type = E_Type.ENCRYPTED_DATA_MESSAGE Or C_Type = E_Type.ENCRYPTED_MESSAGE) Then
            '0=PublicKey; 1=SignKey; 2=AgreementKey
            Dim Masterkeys As List(Of String) = GetMasterKeys(PassPhrase)

            Dim Data As String = GetStringBetween(C_Attachment, "<data>", "</data>")
            Dim Nonce As String = GetStringBetween(C_Attachment, "<nonce>", "</nonce>")

            If C_SenderPublicKey = Masterkeys(0) Then
                C_Message = C_SignumNET.DecryptMessage(Data, Nonce, C_RecipientPublicKey, Masterkeys(2))
            ElseIf C_RecipientPublicKey = Masterkeys(0) Then
                C_Message = C_SignumNET.DecryptMessage(Data, Nonce, C_SenderPublicKey, Masterkeys(2))
            End If

            If C_Type = E_Type.ENCRYPTED_MESSAGE Then
                If MessageIsHEXString(C_Message) Then
                    C_Message = HEXStringToString(C_Message)
                End If
            Else
                Dim ReferenceMessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(C_Message)
                C_ReferenceCommand = ClsDEXContract.GetReferenceCommand(ReferenceMessageList(0))
            End If

            Returner = True

        End If

        Return Returner

    End Function






End Class