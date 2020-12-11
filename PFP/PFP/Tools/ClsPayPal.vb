Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Security.Cryptography

Public Class ClsPayPal

    Const SandboxURL As String = "https://api.sandbox.paypal.com"
    Const LiveURL As String = "https://api.paypal.com"

    Property URL() As String

    Private ReadOnly Property AuthCredentials() As String
        Get
            Return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Client_ID + ":" + Secret))
        End Get
    End Property

    Property Client_ID() As String
    Property Secret() As String

    Private Enum URLx
        _v1_oauth2_token = 0
        _v1_checkout_orders = 1
        _v1_checkout_orders_ID_authorize = 2
        _v1_payments_payouts = 3

        _v2_checkout_orders = 4
        _v2_checkout_orders_ID = 5
        _v2_checkout_orders_ID_authorize = 6
        _v2_checkout_orders_ID_capture = 7

        _v2_payments_captures_CAPID = 8
        _v2_payments_authorizations_AUTHID = 9
        _v2_payments_authorizations_AUTHID_capture = 10
    End Enum


    Sub New(Optional Mode As String = "sandbox")

        If Mode.Trim = "sandbox" Then
            URL = SandboxURL
        Else
            URL = LiveURL
        End If



    End Sub

    Private Sub SetSubURL(ByVal SURL As URLx, Optional ByVal XURL As String = "")

        Select Case SURL
            Case URLx._v1_oauth2_token
                URL = URL + "/v1/oauth2/token"
            Case URLx._v2_checkout_orders
                URL = URL + "/v2/checkout/orders"
            Case URLx._v1_payments_payouts
                URL = URL + "/v1/payments/payouts"
            Case URLx._v2_checkout_orders_ID
                URL = URL + "/v2/checkout/orders/" + XURL
            Case URLx._v2_checkout_orders_ID_authorize
                URL = URL + "/v2/checkout/orders/" + XURL + "/authorize"
            Case URLx._v2_checkout_orders_ID_capture
                URL = URL + "/v2/checkout/orders/" + XURL + "/capture"
            Case URLx._v2_payments_authorizations_AUTHID
                URL = URL + "/v2/payments/authorizations/" + XURL
            Case URLx._v2_payments_authorizations_AUTHID_capture
                URL = URL + "/v2/payments/authorizations/" + XURL + "/capture"
            Case URLx._v2_payments_captures_CAPID
                URL = URL + "/v2/payments/captures/" + XURL
            Case Else

        End Select

    End Sub


    Public Function PayPalRequest(ByVal postData As String, Optional ByVal Method As String = "POST", Optional ByVal HeaderList As List(Of String) = Nothing) As String

        Dim ResponseStr As String = ""
        Try

            Dim request As WebRequest = WebRequest.Create(URL)
            request.Method = Method
            request.ContentType = "application/json" 'x-www-form-urlencoded"

            If Not IsNothing(HeaderList) Then
                For Each Header As String In HeaderList
                    request.Headers.Add(Header)
                Next
            End If


            Dim dataStream As Stream
            If Not postData.Trim = "" Then
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
                request.ContentLength = byteArray.Length

                dataStream = request.GetRequestStream()

                dataStream.Write(byteArray, 0, byteArray.Length)
                dataStream.Close()
            End If


            Dim response As WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            ResponseStr = reader.ReadToEnd()

            reader.Close()
            dataStream.Close()
            response.Close()

            'Return ResponseStr

        Catch ex As Exception
            Try


                Dim obj As Object = ex

                Dim hwr As HttpWebResponse = DirectCast(obj.Response, HttpWebResponse)

                Dim ResponseStream As Stream = hwr.GetResponseStream()
                Dim ResponseReader As New StreamReader(ResponseStream)
                ResponseStr = ResponseReader.ReadToEnd()

            Catch exep As Exception

            End Try
        End Try

        Return ResponseStr

    End Function

    Public Function GetAuthToken() As List(Of String)

        SetSubURL(ClsPayPal.URLx._v1_oauth2_token)

        Dim ResponseStr As String = PayPalRequest("grant_type=client_credentials",, New List(Of String)({"Accept-Language: en_US", "Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        Dim ReturnList As List(Of Object) = JSONRecursive(ResponseStr)

        Dim NuList As List(Of String) = New List(Of String)

        Dim Erro As String = RecursiveSearch(ReturnList, "error").ToString
        Dim Scope As String = RecursiveSearch(ReturnList, "scope").ToString
        Dim Access_Token As String = RecursiveSearch(ReturnList, "access_token").ToString
        Dim Token_Type As String = RecursiveSearch(ReturnList, "token_type").ToString
        Dim App_ID As String = RecursiveSearch(ReturnList, "app_id").ToString
        Dim Expires_In As String = RecursiveSearch(ReturnList, "expires_in").ToString
        Dim Nonce As String = RecursiveSearch(ReturnList, "nonce")

        If Erro = "False" Then
            NuList.Add("<scope>" + Scope + "</scope>")
            NuList.Add("<access_token>" + Access_Token + "</access_token>")
            NuList.Add("<token_type>" + Token_Type + "</token_type>")
            NuList.Add("<app_id>" + App_ID + "</app_id>")
            NuList.Add("<expires_in>" + Expires_In + "</expires_in>")
            NuList.Add("<nonce>" + Nonce + "</nonce>")
        Else
            NuList.Add("<error>" + Erro + "</error>")
        End If


        Return NuList

    End Function

    Public Function CreateOrder(ByVal XItem As String, ByVal XAmount As Double, ByVal Price As Double, Optional ByVal Currency As String = "USD") As List(Of String)

        Dim Request = "{"
        Request += """intent"": ""CAPTURE"", "
        Request += """purchase_units"": [{"
        Request += """amount"": {"
        Request += """currency_code"": """ + Currency.Trim.ToUpper + """, ""value"": """ + Price.ToString.Replace(",", ".") + """, "
        Request += """breakdown"": {""item_total"": {""currency_code"":""" + Currency.Trim.ToUpper + """, ""value"": """ + Price.ToString.Replace(",", ".") + """}}"
        Request += "}, "
        Request += """items"": [{"
        Request += """name"": ""bunch of " + XAmount.ToString.Replace(",", ".") + " " + XItem.Trim + """, ""unit_amount"": {""currency_code"":""" + Currency.Trim.ToUpper + """, ""value"": """ + Price.ToString.Replace(",", ".") + """},"
        Request += """quantity"": ""1"""
        Request += "}]"
        Request += "}], "
        Request += """application_context"": {"
        Request += """shipping_preference"": ""NO_SHIPPING""}"
        Request += "}"


        SetSubURL(ClsPayPal.URLx._v2_checkout_orders)
        Dim ResponseStr As String = PayPalRequest(Request, , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        Dim ReturnList As List(Of Object) = JSONRecursive(ResponseStr)

        Dim NuList As List(Of String) = New List(Of String)

        Dim ID As String = RecursiveSearch(ReturnList, "id")
        Dim Status As String = RecursiveSearch(ReturnList, "status")

        NuList.Add("<id>" + ID + "</id>")
        NuList.Add("<status>" + Status + "</status>")

        Dim URLList = RecursiveSearch(ReturnList, "links")
        If URLList.GetType.Name = GetType(Boolean).Name Then

        Else

            Dim EntryList As List(Of Object) = New List(Of Object)
            Dim TempOBJList As List(Of Object) = New List(Of Object)

            For Each Entry In URLList
                If Entry(0) = "href" Then
                    If TempOBJList.Count > 0 Then
                        EntryList.Add(TempOBJList)
                    End If

                    TempOBJList = New List(Of Object)
                    TempOBJList.Add(Entry)
                Else
                    TempOBJList.Add(Entry)
                End If
            Next

            EntryList.Add(TempOBJList)

            For Each Entry In EntryList
                Dim T_Str As String = ""
                For Each KeyVal In Entry
                    Select Case KeyVal(0)
                        Case "href"
                            T_Str += "<href>" + KeyVal(1) + "</href>"
                        Case "rel"
                            T_Str += "<rel>" + KeyVal(1) + "</rel>"
                        Case "method"
                            T_Str += "<method>" + KeyVal(1) + "</method>"
                    End Select
                Next
                NuList.Add(T_Str)
            Next

        End If

        Return NuList

    End Function
    Public Function CaptureOrder(ByVal OrderID As String) As List(Of String)

        SetSubURL(ClsPayPal.URLx._v2_checkout_orders_ID_capture, OrderID)

        Dim ResponseStr As String = PayPalRequest("", , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        Dim ResponseList As List(Of Object) = JSONRecursive(ResponseStr)

        Dim ReturnList As List(Of String) = New List(Of String)

        Dim ID As String = RecursiveSearch(ResponseList, "id")
        Dim Status As String = RecursiveSearch(ResponseList, "status")

        ReturnList.Add("<id>" + ID + "</id>")
        ReturnList.Add("<status>" + Status + "</status>")

        Dim URLList = RecursiveSearch(ResponseList, "links")
        If URLList.GetType.Name = GetType(Boolean).Name Then

        Else

            Dim EntryList As List(Of Object) = New List(Of Object)
            Dim TempOBJList As List(Of Object) = New List(Of Object)

            For Each Entry In URLList
                If Entry(0) = "href" Then
                    If TempOBJList.Count > 0 Then
                        EntryList.Add(TempOBJList)
                    End If

                    TempOBJList = New List(Of Object)
                    TempOBJList.Add(Entry)
                Else
                    TempOBJList.Add(Entry)
                End If
            Next

            EntryList.Add(TempOBJList)

            For Each Entry In EntryList
                Dim T_Str As String = ""
                For Each KeyVal In Entry
                    Select Case KeyVal(0)
                        Case "href"
                            T_Str += "<href>" + KeyVal(1) + "</href>"
                        Case "rel"
                            T_Str += "<rel>" + KeyVal(1) + "</rel>"
                        Case "method"
                            T_Str += "<method>" + KeyVal(1) + "</method>"
                    End Select
                Next
                ReturnList.Add(T_Str)
            Next

        End If

        Return ReturnList

    End Function
    Public Function GetOrderDetails(ByVal OrderID As String)

        SetSubURL(ClsPayPal.URLx._v2_checkout_orders_ID, OrderID)

        Dim ResponseStr As String = PayPalRequest("", "GET", New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        Dim ReturnList As List(Of Object) = JSONRecursive(ResponseStr)

        Dim NuList As List(Of String) = New List(Of String)

        Dim ID As String = RecursiveSearch(ReturnList, "id")
        Dim Intent As String = RecursiveSearch(ReturnList, "intent")
        Dim Status As String = RecursiveSearch(ReturnList, "status")

        NuList.Add("<id>" + ID + "</id>")
        NuList.Add("<intent>" + Intent + "</intent>")
        NuList.Add("<status>" + Status + "</status>")

        Dim URLList = RecursiveSearch(ReturnList, "links")
        If URLList.GetType.Name = GetType(Boolean).Name Then

        Else

            Dim EntryList As List(Of Object) = New List(Of Object)
            Dim TempOBJList As List(Of Object) = New List(Of Object)

            For Each Entry In URLList
                If Entry(0) = "href" Then
                    If TempOBJList.Count > 0 Then
                        EntryList.Add(TempOBJList)
                    End If

                    TempOBJList = New List(Of Object)
                    TempOBJList.Add(Entry)
                Else
                    TempOBJList.Add(Entry)
                End If
            Next

            EntryList.Add(TempOBJList)

            For Each Entry In EntryList
                Dim T_Str As String = ""
                For Each KeyVal In Entry
                    Select Case KeyVal(0)
                        Case "href"
                            T_Str += "<href>" + KeyVal(1) + "</href>"
                        Case "rel"
                            T_Str += "<rel>" + KeyVal(1) + "</rel>"
                        Case "method"
                            T_Str += "<method>" + KeyVal(1) + "</method>"
                    End Select
                Next
                NuList.Add(T_Str)
            Next

        End If

        Return NuList

    End Function



    Public Function AuthOrder()

        SetSubURL(ClsPayPal.URLx._v2_checkout_orders_ID_authorize, "2NS83667T8950703G")

        Dim ResponseStr As String = PayPalRequest("", , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        ResponseStr = ResponseStr

    End Function



    Public Function GetPaymentDetails()

        SetSubURL(ClsPayPal.URLx._v2_payments_authorizations_AUTHID, "7EJ68552ST726563J")

        Dim ResponseStr As String = PayPalRequest("", "GET", New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        ResponseStr = ResponseStr

    End Function
    Public Function CaptureAuthPayment()

        SetSubURL(ClsPayPal.URLx._v2_payments_authorizations_AUTHID_capture, "7EJ68552ST726563J")

        Dim Request = "{"
        Request += """amount"" {"
        Request += """value"": ""1.23"", "
        Request += """currency_code"": ""USD"""
        Request += "}, "
        Request += """invoice_id"": ""INVOICE-123"", "
        Request += """final_capture"": true"
        Request += "}"

        Dim ResponseStr As String = PayPalRequest("", , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        ResponseStr = ResponseStr

    End Function
    Public Function CapturePaymentDetails()

        SetSubURL(ClsPayPal.URLx._v2_payments_captures_CAPID, "7EJ68552ST726563J")

        Dim ResponseStr As String = PayPalRequest("", "GET", New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        ResponseStr = ResponseStr

    End Function


    Public Function CreateBatchPayOut(ByVal Recipient As String, ByVal Amount As Double, Optional ByVal SendType As String = "EMAIL", Optional ByVal Currency As String = "USD")

        Dim Hash As String = CreateHash(Now.ToLongDateString + " " + Now.ToLongTimeString)

        Hash = Hash.Substring(0, 4) + Hash.Substring(Hash.Length - 4)

        Dim Request = "{"
        Request += """sender_batch_header"": { "
        Request += """sender_batch_id"": """ + Hash + ""","
        Request += """email_subject"":""You got a Payment from BLS!"","
        Request += """email_message"": ""You have received a automatic Payment from BLS!"""
        Request += "},"
        Request += """items"": ["
        Request += "{"

        If SendType.Trim = "EMAIL" Then

            Request += """recipient_type"":""EMAIL"", "
            Request += """amount"": { "
            Request += """value"": """ + Amount.ToString.Replace(",", ".") + """, "
            Request += """currency"":""" + Currency + """"
            Request += "}, "
            Request += """note"": ""thank you"", "
            Request += """sender_item_id"": """ + Hash + """, "
            Request += """receiver"": """ + Recipient + """" 'sb-ba6ka3358898@personal.example.com

        ElseIf SendType.Trim = "PAYPAL_ID" Then

            Request += """recipient_type"":""PAYPAL_ID"", "
            Request += """amount"": { "
            Request += """value"": """ + Amount.ToString.Replace(",", ".") + """, "
            Request += """currency"": """ + Currency + """"
            Request += "}, "
            Request += """note"": ""Thank you"", "
            Request += """sender_item_id"": """ + Hash + """, "
            Request += """receiver"": """ + Recipient + """" '35ZAG25DABZ62

        End If

        Request += "}"
        Request += "]"
        Request += "}"

        SetSubURL(ClsPayPal.URLx._v1_payments_payouts)
        Dim ResponseStr As String = PayPalRequest(Request, , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        Dim ReturnList As List(Of Object) = JSONRecursive(ResponseStr)
        Dim NuList As List(Of String) = New List(Of String)
        Dim x = RecursiveSearch(ReturnList, "payout_batch_id")

        Return x

    End Function



#Region "Toolfunctions"

    Dim RekursivRest As List(Of Object) = New List(Of Object)
    Private Structure S_Sorter
        Dim Timestamp As ULong
        Dim TXID As ULong
    End Structure

    Private Function SortTimeStamp(ByVal input As List(Of List(Of String))) As List(Of List(Of String))

        Dim TSSort As List(Of S_Sorter) = New List(Of S_Sorter)

        For i As Integer = 0 To input.Count - 1

            Dim Entry As List(Of String) = input(i)

            Dim T_Timestamp As ULong = CULng(BetweenFromList(Entry, "<timestamp>", "</timestamp>"))
            Dim T_Transaction As ULong = CULng(BetweenFromList(Entry, "<transaction>", "</transaction>"))

            Dim NuSort As S_Sorter = New S_Sorter
            NuSort.Timestamp = T_Timestamp
            NuSort.TXID = T_Transaction

            TSSort.Add(NuSort)
        Next

        TSSort = TSSort.OrderBy(Function(s) s.Timestamp).ToList

        Dim SReturnList As List(Of List(Of String)) = New List(Of List(Of String))

        For Each sort In TSSort

            For i As Integer = 0 To input.Count - 1
                Dim retent = input(i)

                Dim T_Timestamp As ULong = CULng(BetweenFromList(retent, "<timestamp>", "</timestamp>"))
                Dim T_Transaction As ULong = CULng(BetweenFromList(retent, "<transaction>", "</transaction>"))

                If T_Timestamp = sort.Timestamp And T_Transaction = sort.TXID Then
                    SReturnList.Add(retent)
                    Exit For
                End If

            Next

        Next

        Return SReturnList

    End Function


    Function BetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional ByVal GetListIndex As Boolean = False) As String

        For i As Integer = 0 To inputList.Count - 1
            Dim Entry As String = inputList(i)
            If Entry.Contains(startchar) Then

                If GetListIndex Then
                    Return i.ToString
                Else
                    Return Between(Entry, startchar, endchar, GetType(String))
                End If

            End If

        Next

        If GetListIndex Then
            Return "-1"
        Else
            Return ""
        End If

    End Function
    Function Between(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional ByVal GetTyp As Object = Nothing) As Object

        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)
                input = input.Remove(input.IndexOf(endchar))

                If IsNothing(GetTyp) Then
                    Return input
                Else
                    Select Case GetTyp.Name
                        Case GetType(Integer).Name
                            Return CInt(input)
                        Case GetType(Double).Name
                            Return CDbl(input.Replace(".", ","))
                        Case GetType(Date).Name
                            Return CDate(input)
                        Case GetType(String).Name
                            Return input
                    End Select
                End If

            End If
        End If

        If IsNothing(GetTyp) Then
            Return 0.0
        Else
            Select Case GetTyp.Name
                Case GetType(Double).Name
                    Return 0.0
                Case GetType(String).Name
                    Return ""
                Case Else
                    Return 0
            End Select
        End If

    End Function
    Function Between2List(ByVal Input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As List(Of String) ', Optional ByVal LastIdx As Boolean = False) As List(Of String)

        Dim Output As String = ""
        Dim Rest As String = ""

        Try

            If Input.Trim <> "" Then
                If Input.Contains(startchar) And Input.Contains(endchar) Then

                    Dim Temp As String = ""
                    Dim Ruecksetz As Boolean = False
                    Dim Aufnahme As Boolean = False
                    Dim StartMax As Integer = 0

                    For Each ch As String In Input

                        If startchar.Contains(ch) Then
                            Temp += ch
                            Ruecksetz = False
                        Else
                            If Not Ruecksetz Then
                                Ruecksetz = True
                                Temp = ""
                            End If
                        End If

                        If Temp = startchar Then
                            StartMax += 1
                            Temp = ""
                            If StartMax = 1 Then
                                Continue For
                            End If

                        End If



                        If endchar.Contains(ch) Then
                            Temp += ch
                            Ruecksetz = False
                        Else
                            If Not Ruecksetz Then
                                Ruecksetz = True
                                Temp = ""
                            End If
                        End If

                        If Temp = endchar Then
                            StartMax -= 1
                            Temp = ""
                            If StartMax = 0 Then
                                Exit For
                            End If

                        End If



                        If StartMax > 0 Then
                            Output += ch
                        End If

                    Next





                    'Output = Input.Substring(Input.IndexOf(startchar) + startchar.Length)
                    Try
                        Rest = Input.Replace(Output, "")
                    Catch ex As Exception

                    End Try


                    'If LastIdx Then
                    '    Output = Output.Remove(Output.LastIndexOf(endchar))
                    'Else
                    '    Output = Output.Remove(Output.IndexOf(endchar))
                    'End If

                    If Output.Trim = "" Then
                        Return New List(Of String)({Output, Rest})
                        'Return New List(Of String)({Input.Replace(startchar, "").Replace(endchar, ""), ""})
                    Else

                        'Rest = Input.Replace(Output, "")


                        Return New List(Of String)({Output, Rest})
                        'Return New List(Of String)({Output, Input.Replace(Output, "")})
                    End If


                End If
            End If

        Catch ex As Exception
            Return New List(Of String)
        End Try

        Return New List(Of String)

    End Function


    Function JSONRecursive(ByVal Input As String) As List(Of Object)

        If Input.Length > 0 Then

            Dim U_List As List(Of Object) = New List(Of Object)

            If Input(0) = """" Then

                If Input.Contains(":") Then

                    Dim Prop As String = Input.Remove(Input.IndexOf(":"))
                    Dim Val As String = Input.Substring(Input.IndexOf(":") + 1)

                    If Input.Length > 0 Then
                        If Val(0) = "{" Then

                            U_List.Add(New List(Of Object)({Prop.Replace("""", ""), JSONRecursive(Val)}))

                            If Not RekursivRest.Count = 0 Then

                                For Each RekuRest In RekursivRest
                                    U_List.Add(RekuRest)
                                Next
                                RekursivRest = New List(Of Object)
                            End If

                        ElseIf Val(0) = "[" Then

                            U_List.Add(New List(Of Object)({Prop.Replace("""", ""), JSONRecursive(Val)}))

                            If Not RekursivRest.Count = 0 Then

                                For Each RekuRest In RekursivRest
                                    U_List.Add(RekuRest)
                                Next
                                RekursivRest = New List(Of Object)
                            End If

                        Else

                            Dim Brackedfound As String = ""

                            For Each Cha In Input
                                Brackedfound = Cha
                                If Brackedfound = "{" Or Brackedfound = "[" Then
                                    Exit For
                                End If

                            Next


                            If Brackedfound = "{" Then

                                Dim T_Vals As String = Input.Remove(Input.IndexOf("{"))
                                Dim T_Rest As String = Input.Substring(Input.IndexOf("{"))

                                Dim T_List As List(Of String) = T_Vals.Split(",").ToList

                                'If T_Vals.Contains(", """) Then
                                '    Dim TTList As List(Of String) = JSONSplitter(T_Vals, ", """)
                                '    T_List = TTList
                                'End If

                                Dim Key As String = ""

                                For i As Integer = 0 To T_List.Count - 1
                                    Dim TL As String = T_List(i)
                                    If i = T_List.Count - 1 Then
                                        Key = TL.Split(":")(0).Replace("""", "")
                                    Else

                                        Dim TTStrList As List(Of String) = JSONSplitter(TL, """:") ' New List(Of String)(TL.Split(""":"""))

                                        U_List.Add(TTStrList.ToList) '.Replace("""", "")
                                    End If
                                Next


                                Dim EmptyEntry As Boolean = False
                                If T_Rest.Contains("{},") Then
                                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("{},") + 3)
                                    U_List.Add(New List(Of Object)({Key, New List(Of Object)}))
                                    EmptyEntry = True
                                End If

                                If EmptyEntry Then
                                    Dim T_Entrys As List(Of Object) = JSONRecursive(T_Rest)

                                    For Each T_Entry In T_Entrys
                                        U_List.Add(T_Entry)
                                    Next

                                Else
                                    U_List.Add(New List(Of Object)({Key, JSONRecursive(T_Rest)}))
                                End If


                                If Not RekursivRest.Count = 0 Then

                                    For Each RekuRest In RekursivRest
                                        U_List.Add(RekuRest)
                                    Next
                                    RekursivRest = New List(Of Object)
                                End If

                            ElseIf Brackedfound = "[" Then

                                Dim T_Vals As String = Input.Remove(Input.IndexOf("["))
                                Dim T_Rest As String = Input.Substring(Input.IndexOf("["))

                                Dim T_List As List(Of String) = T_Vals.Split(",").ToList


                                'If T_Vals.Contains(", """) Then
                                '    Dim TTList As List(Of String) = JSONSplitter(T_Vals, ", """)
                                '    T_List = TTList
                                'End If


                                Dim Key As String = ""

                                For i As Integer = 0 To T_List.Count - 1
                                    Dim TL As String = T_List(i)
                                    If i = T_List.Count - 1 Then
                                        Key = TL.Split(":")(0).Replace("""", "")
                                    Else

                                        Dim TTStrList As List(Of String) = JSONSplitter(TL, """:") ' New List(Of String)(TL.Split(""":"""))

                                        U_List.Add(TTStrList.ToList) '.Replace("""", "")
                                    End If
                                Next



                                Dim EmptyEntry As Boolean = False
                                If T_Rest.Contains("[],") Then
                                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("[],") + 3)
                                    U_List.Add(New List(Of Object)({Key, New List(Of Object)}))
                                    EmptyEntry = True
                                End If

                                If EmptyEntry Then
                                    Dim T_Entrys As List(Of Object) = JSONRecursive(T_Rest)

                                    For Each T_Entry In T_Entrys
                                        U_List.Add(T_Entry)
                                    Next

                                Else
                                    U_List.Add(New List(Of Object)({Key, JSONRecursive(T_Rest)}))
                                End If



                                If Not RekursivRest.Count = 0 Then

                                    For Each RekuRest In RekursivRest
                                        U_List.Add(RekuRest)
                                    Next
                                    RekursivRest = New List(Of Object)
                                End If

                            Else
                                'Dim TTList As List(Of String) = JSONSplitter(Input, ",")

                                Dim T_List As List(Of String) = Input.Split(",").ToList

                                For Each TL As String In T_List

                                    Dim TTStrList As List(Of String) = JSONSplitter(TL, """:") ' New List(Of String)(TL.Split(""":"""))

                                    U_List.Add(TTStrList.ToList) '.Replace("""", "")
                                Next

                                Return U_List

                            End If

                        End If
                    End If

                Else
                    If Input.Contains(",") Then

                        'Dim TTStrList As List(Of String) = JSONSplitter(Input, ",") ' New List(Of String)(TL.Split(""":"""))


                        U_List.Add(Input.Replace("""", "").Split(",").ToList)
                    Else
                        U_List.Add(Input.Replace("""", ""))
                    End If

                End If


            ElseIf Input(0) = "{" Then

                Dim T_Input As List(Of String) = Between2List(Input, "{", "}")

                If T_Input.Count > 0 Then
                    Input = T_Input(0)
                End If


                If Input(0) = "," Then
                    Input = Input.Substring(1)
                End If

                Dim SubList As List(Of Object) = JSONRecursive(Input)

                For Each SubListItem In SubList
                    U_List.Add(SubListItem)
                Next

                Dim T_Rest As String = T_Input(1)

                If T_Rest.Contains("{},") Then
                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("{},") + 3)

                    Dim RestRek As List(Of Object) = JSONRecursive(T_Rest)

                    For Each Rest In RestRek
                        RekursivRest.Add(Rest)
                    Next

                End If


            ElseIf Input(0) = "[" Then

                Dim T_Input As List(Of String) = Between2List(Input, "[", "]")

                If T_Input.Count > 0 Then
                    Input = T_Input(0)
                End If

                If Not Input.Trim = "" Then
                    If Input(0) = "," Then
                        Input = Input.Substring(1)
                    End If
                End If


                Dim SubList As List(Of Object) = JSONRecursive(Input)

                For Each SubListItem In SubList
                    U_List.Add(SubListItem)
                Next


                If Not RekursivRest.Count = 0 Then

                    For Each RekuRest In RekursivRest
                        U_List.Add(RekuRest)
                    Next
                    RekursivRest = New List(Of Object)
                End If

                Dim T_Rest As String = T_Input(1)

                If T_Rest.Contains("[],") Then
                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("[],") + 3)
                    Dim RestRek As List(Of Object) = JSONRecursive(T_Rest)

                    For Each Rest In RestRek
                        RekursivRest.Add(Rest)
                    Next

                End If


            End If


            Return U_List

        Else
            Return New List(Of Object)
        End If

    End Function
    Function JSONSplitter(ByVal input As String, ByVal Splitter As String) As List(Of String)

        Dim ReturnStr As List(Of String) = New List(Of String)({input}.ToList)
        If input.Contains(Splitter) Then

            Dim T_List As List(Of String) = New List(Of String)(Strings.Split(input, Splitter))

            For i As Integer = 0 To T_List.Count - 1
                Dim entry As String = T_List(i)
                entry = entry.Replace("""", "")
                T_List(i) = entry
            Next

            ReturnStr = T_List
        Else

        End If

        Return ReturnStr

    End Function


    Function RecursiveSearch(ByVal List As Object, ByVal Key As String) As Object

        Dim Returner As Object = False

        For Each Entry In List

            If Entry.GetType.Name = GetType(String).Name Then

                If Entry.ToLower.trim = Key.ToLower.Trim Then

                    Dim FindList As List(Of Object) = New List(Of Object)

                    For i As Integer = 1 To List.count - 1
                        FindList.Add(List(i))
                    Next

                    If FindList.Count > 1 Then
                        Return FindList
                    Else
                        Return FindList(0)
                    End If

                End If

            Else
                Returner = RecursiveSearch(Entry, Key)

                If Not Returner.GetType.Name = GetType(Boolean).Name Then
                    Return Returner
                End If

            End If

        Next

        Return Returner

    End Function

    Function CreateSignature(ByVal Secret As String, ByVal Data As String)
        'Return Bytes_To_String2(SignHMACSHA512(Secret, StringToByteArray(Data)))
        Return ByteArrayToString(SignHMACSHA512(Secret, StringToByteArray(Data))).ToUpper
    End Function

    Function CreateHash(ByVal Data As String)
        Return ByteArrayToString(SignSHA512(StringToByteArray(Data))).ToUpper
    End Function

    Function SignHMACSHA512(ByVal Secret As String, ByVal Data As Byte())
        Dim SHA512Hasher As HMACSHA512 = New HMACSHA512(Encoding.ASCII.GetBytes(Secret))
        Return SHA512Hasher.ComputeHash(Data)
    End Function

    Function SignSHA512(ByVal Data As Byte())
        Dim SHA512Hasher As SHA512 = New SHA512Managed()
        Return SHA512Hasher.ComputeHash(Data)
    End Function

    Function StringToByteArray(ByVal DataStr As String) As Byte()
        Return System.Text.Encoding.ASCII.GetBytes(DataStr)
    End Function

    Function ByteArrayToString(ByVal hash As Byte()) As String
        Return BitConverter.ToString(hash).Replace("-", "").ToLower
    End Function

    Function Bytes_To_String2(ByVal bytes_Input As Byte()) As String
        Dim strTemp As New StringBuilder(bytes_Input.Length * 2)
        For Each b As Byte In bytes_Input
            strTemp.Append(Conversion.Hex(b))
        Next
        Return strTemp.ToString()
    End Function

#End Region


End Class
