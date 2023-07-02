Imports System.Resources.ResXFileRef

Public Class ClsSignumInteractions

    Private ReadOnly Property T_PrimaryNode As String

    Private ReadOnly Property T_DEXContract As ClsDEXContract

    Sub New(ByVal DEXContract As ClsDEXContract, PrimaryNode As String)
        T_PrimaryNode = PrimaryNode
        T_DEXContract = DEXContract
    End Sub

    Function SendBillingInfos(ByVal RecipientAddress As String, ByVal Message As String, ByVal ShowPINForm As Boolean, Optional ByVal Encrypt As Boolean = True, Optional ByVal RecipientPublicKey As String = "") As String
        Dim RecipientID As ULong = ClsReedSolomon.Decode(RecipientAddress)
        Return SendBillingInfos(RecipientID, Message, ShowPINForm, Encrypt, RecipientPublicKey)
    End Function

    Function SendBillingInfos(ByVal RecipientID As ULong, ByVal Message As String, ByVal ShowPINForm As Boolean, Optional ByVal Encrypt As Boolean = True, Optional ByVal RecipientPublicKey As String = "") As String
        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(T_PrimaryNode)

        Dim Masterkeys As List(Of String) = GetPassPhrase()

        If Masterkeys.Count > 0 Then
            Dim Response As String = SignumAPI.SendMessage(Masterkeys(0), Masterkeys(2), RecipientID, Message,, Encrypt,, RecipientPublicKey)

            Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)
            'Dim JSON As ClsJSON = New ClsJSON
            'Dim RespList As Object = JSON.JSONRecursive(Response)

            Dim Error0 As Object = Converter.FirstValue("errorCode") '  JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
            If Error0.GetType.Name = GetType(Boolean).Name Then
                'TX OK
            ElseIf Error0.GetType.Name = GetType(String).Name Then
                Return Application.ProductName + "-error in SendBillingInfos(1): -> " + vbCrLf + Response
            End If


            If Response.Contains(Application.ProductName + "-error") Then
                Return Application.ProductName + "-error in SendBillingInfos(2): -> " + vbCrLf + Response
            Else

                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, Masterkeys(1))
                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                If TX.Contains(Application.ProductName + "-error") Then
                    Return Application.ProductName + "-error in SendBillingInfos(3): -> " + vbCrLf + TX
                Else
                    Return TX
                End If

            End If

        Else

            Dim Returner As String = Application.ProductName + "-warning in SendBillingInfos(4): -> no Keys"

            If ShowPINForm Then

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then

                    Dim Response As String = SignumAPI.SendMessage(PinForm.PublicKey, PinForm.AgreeKey, RecipientID, Message,, Encrypt,, RecipientPublicKey)

                    If Response.Contains(Application.ProductName + "-error") Then
                        Return Application.ProductName + "-error in SendBillingInfos(5): -> " + vbCrLf + Response
                    Else

                        Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                        Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                        Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                        If TX.Contains(Application.ProductName + "-error") Then
                            Return Application.ProductName + "-error in SendBillingInfos(6): -> " + vbCrLf + TX
                        Else
                            Return TX
                        End If

                    End If

                Else
                    Return Returner
                End If

            Else
                Return Returner
            End If

        End If

    End Function

    Function InjectChainSwapHash(ByVal ChainSwapHash As String, Optional ByVal ShowPINForm As Boolean = True) As String
        Dim Masterkeys As List(Of String) = GetPassPhrase()

        If Masterkeys.Count > 0 Then
            Return T_DEXContract.InjectChainSwapHash(GlobalPublicKey, ChainSwapHash,, Masterkeys(1))
        Else

            Dim Returner As String = Application.ProductName + "-warning in InjectChainSwapHash(Form): -> no Keys"

            If ShowPINForm Then

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then
                    Return T_DEXContract.InjectChainSwapHash(GlobalPublicKey, ChainSwapHash,, PinForm.SignKey)
                Else
                    Return Returner
                End If

            Else
                Return Returner
            End If

        End If

    End Function

    Function RejectResponder(Optional ByVal ShowPINForm As Boolean = True) As String

        Dim Masterkeys As List(Of String) = GetPassPhrase()

        If Masterkeys.Count > 0 Then
            Return T_DEXContract.RejectResponder(GlobalPublicKey,, Masterkeys(1))
        Else

            Dim Returner As String = Application.ProductName + "-warning in RejectResponder(Form): -> no Keys"

            If ShowPINForm Then

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then
                    Return T_DEXContract.RejectResponder(GlobalPublicKey, , PinForm.SignKey)
                Else
                    Return Returner
                End If

            Else
                Return Returner
            End If

        End If


    End Function

    Function FinishWithChainSwapKey(ByVal ChainSwapKey As String, Optional ByVal ShowPINForm As Boolean = True) As String

        Dim Masterkeys As List(Of String) = GetPassPhrase()

        If Masterkeys.Count > 0 Then
            Return T_DEXContract.FinishOrderWithChainSwapKey(GlobalPublicKey, ChainSwapKey,, Masterkeys(1))
        Else

            Dim Returner As String = Application.ProductName + "-warning in FinishWithChainSwapKey(Form): -> no Keys"

            If ShowPINForm Then

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then
                    Return T_DEXContract.FinishOrderWithChainSwapKey(GlobalPublicKey, ChainSwapKey,, PinForm.SignKey)
                Else
                    Return Returner
                End If

            Else
                Return Returner
            End If

        End If

    End Function

End Class
