
Public Class ClsOrderSettings

    Public Property AT As String
    Public Property TX As String
    Public Property Type As String
    Public Property PaytypeString As String
    Public Property PayType As E_PayType
    Public Property Infotext As String
    Public Property AutoSendInfotext As Boolean
    Public Property AutoCompleteAT As Boolean
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

    Sub New(ByVal T_AT As String, ByVal T_TX As String, T_Type As String, T_Status As String)

        AT = T_AT
        TX = T_TX
        Type = T_Type
        PaytypeString = GetINISetting(E_Setting.PayPalChoice, "")
        SetPayType(PaytypeString)
        Infotext = GetINISetting(E_Setting.PaymentInfoText, "")
        Try
            AutoSendInfotext = CBool(GetINISetting(E_Setting.AutoSendPaymentInfo, False))
            AutoCompleteAT = CBool(GetINISetting(E_Setting.AutoCheckAndFinishAT, False))
        Catch ex As Exception
            AutoSendInfotext = False
            AutoCompleteAT = False
        End Try

        Status = T_Status

    End Sub

End Class
