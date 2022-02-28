
Option Strict On
Option Explicit On

Public Class ClsOrderSettings

    Public Property SmartContractID As ULong
    Public Property TransactionID As ULong
    Public Property Type As String
    Public Property PaytypeString As String
    Public Property PayType As E_PayType
    Public Property Infotext As String
    Public Property AutoSendInfotext As Boolean
    Public Property AutoCompleteSmartContract As Boolean
    Public Property Status As String

    Public Enum E_PayType
        Bankaccount = 0
        PayPal_E_Mail = 1
        PayPal_Order = 2
        Self_Pickup = 3
        Other = 4
    End Enum

    Public Shared Function GetPayTypes() As List(Of String)
        Dim PayTypes As List(Of String) = System.Enum.GetNames(GetType(E_PayType)).ToList

        Dim NuList As List(Of String) = New List(Of String)

        For Each PayType As String In PayTypes

            Select Case PayType
                Case E_PayType.Self_Pickup.ToString
                    NuList.Add(PayType.Replace("_", " "))
                Case Else
                    NuList.Add(PayType.Replace("_", "-"))
            End Select

        Next

        Return NuList

    End Function

    Public Sub SetPayType(Optional ByVal PayTypeStr As String = "")

        If PayTypeStr.Trim = "" Then
            PayTypeStr = PaytypeString
        End If

        Select Case PayTypeStr
            Case E_PayType.Bankaccount.ToString
                PayType = E_PayType.Bankaccount
            Case E_PayType.PayPal_E_Mail.ToString.Replace("_", "-")
                PayType = E_PayType.PayPal_E_Mail
            Case E_PayType.PayPal_Order.ToString.Replace("_", "-")
                PayType = E_PayType.PayPal_Order
            Case E_PayType.Self_Pickup.ToString.Replace("_", " ")
                PayType = E_PayType.Self_Pickup
            Case Else
                PayType = E_PayType.Other
        End Select

    End Sub

    Sub New(ByVal T_SmartContract As ULong, ByVal T_Transaction As ULong, T_isSellOrder As Boolean, T_Status As ClsDEXContract.E_Status)

        SmartContractID = T_SmartContract
        TransactionID = T_Transaction

        If T_isSellOrder Then
            Type = "SellOrder"
        Else
            Type = "BuyOrder"
        End If

        PaytypeString = GetINISetting(E_Setting.PayPalChoice, "")
        SetPayType(PaytypeString)
        Infotext = GetINISetting(E_Setting.PaymentInfoText, "")

        Try
            AutoSendInfotext = CBool(GetINISetting(E_Setting.AutoSendPaymentInfo, False))
            AutoCompleteSmartContract = CBool(GetINISetting(E_Setting.AutoCheckAndFinishSmartContract, False))
        Catch ex As Exception
            AutoSendInfotext = False
            AutoCompleteSmartContract = False
        End Try

        Status = T_Status.ToString

    End Sub

    Sub New(ByVal T_SmartContract As String, ByVal T_Transaction As String, ByVal T_Type As String, T_Status As String)

        SmartContractID = ULong.Parse(T_SmartContract)
        TransactionID = ULong.Parse(T_Transaction)
        Type = T_Type

        PaytypeString = GetINISetting(E_Setting.PayPalChoice, "")
        SetPayType(PaytypeString)
        Infotext = GetINISetting(E_Setting.PaymentInfoText, "")

        Try
            AutoSendInfotext = CBool(GetINISetting(E_Setting.AutoSendPaymentInfo, False))
            AutoCompleteSmartContract = CBool(GetINISetting(E_Setting.AutoCheckAndFinishSmartContract, False))
        Catch ex As Exception
            AutoSendInfotext = False
            AutoCompleteSmartContract = False
        End Try

        Status = ""

        Dim T_Stadi As List(Of ClsDEXContract.E_Status) = New List(Of ClsDEXContract.E_Status) 'With System.Enum.GetValues(GetType(ClsDEXContract.E_Status))
        Dim E_EnumStadi As Array = System.Enum.GetValues(GetType(ClsDEXContract.E_Status))
        T_Stadi.AddRange(DirectCast(E_EnumStadi, IEnumerable(Of ClsDEXContract.E_Status)))


        For Each TT_Status As ClsDEXContract.E_Status In T_Stadi
            If T_Status = TT_Status.ToString Then
                Status = TT_Status.ToString
                Exit For
            End If
        Next

    End Sub

End Class
