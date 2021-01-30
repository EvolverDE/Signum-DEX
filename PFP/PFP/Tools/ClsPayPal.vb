Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Security.Cryptography

Public Class ClsPayPal

    Const SandboxURL As String = "https://sandbox.paypal.com"
    Const LiveURL As String = "https://api.paypal.com"

    Property URL() As String

    Private ReadOnly Property AuthCredentials() As String
        Get
            Return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Client_ID + ":" + Secret))
        End Get
    End Property

    Property Client_ID() As String
    Property Secret() As String

    Private Enum E_SubURL
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

        _v1_payments_payment_PARAMS = 11
        _v1_reporting_transactions_PARAMS = 12


        _v1_notifications_webhooks = 13

        _v1_reporting_balances = 14

    End Enum


    Sub New(Optional Mode As String = "sandbox")

        If Mode.Trim = "sandbox" Then
            URL = SandboxURL
        Else
            URL = LiveURL
        End If



    End Sub

    Private Sub SetSubURL(ByVal SubURL As E_SubURL, Optional ByVal ExtraURL As String = "", Optional ByVal URL_PARAMS As String = "")

        Select Case SubURL
            Case E_SubURL._v1_oauth2_token
                URL = URL + "/v1/oauth2/token"
            Case E_SubURL._v2_checkout_orders
                URL = URL + "/v2/checkout/orders"
            Case E_SubURL._v1_payments_payouts
                URL = URL + "/v1/payments/payouts"
            Case E_SubURL._v2_checkout_orders_ID
                URL = URL + "/v2/checkout/orders/" + ExtraURL
            Case E_SubURL._v2_checkout_orders_ID_authorize
                URL = URL + "/v2/checkout/orders/" + ExtraURL + "/authorize"
            Case E_SubURL._v2_checkout_orders_ID_capture
                URL = URL + "/v2/checkout/orders/" + ExtraURL + "/capture"
            Case E_SubURL._v2_payments_authorizations_AUTHID
                URL = URL + "/v2/payments/authorizations/" + ExtraURL
            Case E_SubURL._v2_payments_authorizations_AUTHID_capture
                URL = URL + "/v2/payments/authorizations/" + ExtraURL + "/capture"
            Case E_SubURL._v2_payments_captures_CAPID
                URL = URL + "/v2/payments/captures/" + ExtraURL
            Case E_SubURL._v1_payments_payment_PARAMS
                URL = URL + "/v1/payments/payment" + URL_PARAMS
            Case E_SubURL._v1_reporting_transactions_PARAMS
                URL = URL + "/v1/reporting/transactions" + URL_PARAMS
            Case E_SubURL._v1_notifications_webhooks
                URL = URL + "/v1/notifications/webhooks"
            Case E_SubURL._v1_reporting_balances
                URL = URL + "/v1/reporting/balances"
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

        SetSubURL(ClsPayPal.E_SubURL._v1_oauth2_token)

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


        SetSubURL(ClsPayPal.E_SubURL._v2_checkout_orders)
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

        '{
        '	"id":"3KY70887AK977221B",
        '	"status":"CREATED",
        '	"links":[{
        '			"href":"https://api.sandbox.paypal.com/v2/checkout/orders/3KY70887AK977221B",
        '			"rel":"self",
        '			"method":"GET"
        '		},{
        '			"href":"https://www.sandbox.paypal.com/checkoutnow?token=3KY70887AK977221B",
        '			"rel":"approve",
        '			"method":"GET"
        '		},{
        '			"href":"https://api.sandbox.paypal.com/v2/checkout/orders/3KY70887AK977221B",
        '			"rel":"update",
        '			"method":"PATCH"
        '		},{
        '			"href":"https://api.sandbox.paypal.com/v2/checkout/orders/3KY70887AK977221B/capture",
        '			"rel":"capture",
        '			"method":"POST"
        '		}]
        '}

    End Function
    Public Function CaptureOrder(ByVal OrderID As String) As List(Of String)

        SetSubURL(ClsPayPal.E_SubURL._v2_checkout_orders_ID_capture, OrderID)

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

        '{
        '	"id":"2NS83667T8950703G",
        '	"status":"COMPLETED",
        '	"purchase_units":
        '	[
        '		{
        '			"reference_id":"default",
        '			"shipping":
        '			{
        '				"name":
        '				{
        '					"full_name":"John Doe"
        '				},
        '				"address":
        '				{
        '					"address_line_1":"ESpachstr. 1",
        '					"admin_area_2":"Freiburg",
        '					"admin_area_1":"Baden-Württemberg",
        '					"postal_code":"79111",
        '					"country_code":"DE"
        '				}
        '			},
        '			"payments":
        '			{
        '				"captures":
        '				[
        '					{
        '						"id":"7EJ68552ST726563J",
        '						"status":"PENDING",
        '						"status_details":
        '						{
        '							"reason":"RECEIVING_PREFERENCE_MANDATES_MANUAL_ACTION"
        '						},
        '						"amount":
        '						{
        '							"currency_code":"USD",
        '							"value":"1.23"
        '						},
        '						"final_capture":true,
        '						"seller_protection":
        '						{
        '							"status":"ELIGIBLE",
        '							"dispute_categories":
        '							[
        '								"ITEM_NOT_RECEIVED",
        '								"UNAUTHORIZED_TRANSACTION"
        '							]
        '						},
        '						"links":
        '						[
        '							{
        '								"href":"https://api.sandbox.paypal.com/v2/payments/captures/7EJ68552ST726563J",
        '								"rel":"self",
        '								"method":"GET"
        '							},{
        '								"href":"https://api.sandbox.paypal.com/v2/payments/captures/7EJ68552ST726563J/refund",
        '								"rel":"refund",
        '								"method":"POST"
        '							},{
        '								"href":"https://api.sandbox.paypal.com/v2/checkout/orders/2NS83667T8950703G",
        '								"rel":"up",
        '								"method":"GET"
        '							}
        '						],
        '						"create_time":"2020-10-06T12:22:41Z",
        '						"update_time":"2020-10-06T12:22:41Z"
        '					}
        '				]
        '			}
        '		}
        '	],
        '	"payer":
        '	{
        '		"name":
        '		{
        '			"given_name":"John",
        '			"surname":"Doe"
        '		},
        '		"email_address":"sb-ba6ka3358898@personal.example.com",
        '		"payer_id":"35ZAG25DABZ62",
        '		"address":
        '		{
        '			"country_code":"DE"
        '		}
        '	},
        '	"links":
        '	[
        '		{
        '			"href":"https://api.sandbox.paypal.com/v2/checkout/orders/2NS83667T8950703G",
        '			"rel":"self",
        '			"method":"GET"
        '		}
        '	]
        '}

    End Function
    Public Function GetOrderDetails(ByVal OrderID As String) As List(Of String)

        SetSubURL(ClsPayPal.E_SubURL._v2_checkout_orders_ID, OrderID)

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


        '{
        '	"id":"2NS83667T8950703G",
        '	"intent":"CAPTURE",
        '	"status":"APPROVED",
        '	"purchase_units":
        '	[
        '		{
        '			"reference_id":"default",
        '			"amount":
        '			{
        '				"currency_code":"USD",
        '				"value":"1.23",
        '				"breakdown":
        '				{
        '					"item_total":
        '					{
        '						"currency_code":"USD",
        '						"value":"1.23"
        '					}
        '				}
        '			},
        '			"payee":
        '			{
        '				"email_address":"testaccount1@burstcoin.zone",
        '				"merchant_id":"226HFCQJADVLE"
        '			},
        '			"items":
        '			[
        '				{
        '					"name":"x 123 Burst",
        '					"unit_amount":
        '					{
        '						"currency_code":"USD",
        '						"value":"1.23"
        '					},
        '					"quantity":"1"
        '				}
        '			],
        '			"shipping":
        '			{
        '				"name":
        '				{
        '					"full_name":"John Doe"
        '				},
        '				"address":
        '				{
        '					"address_line_1":"ESpachstr. 1",
        '					"admin_area_2":"Freiburg",
        '					"admin_area_1":"Baden-Württemberg",
        '					"postal_code":"79111",
        '					"country_code":"DE"
        '				}
        '			}
        '		}
        '	],
        '	"payer":
        '	{
        '		"name":
        '		{
        '			"given_name":"John",
        '			"surname":"Doe"
        '		},
        '		"email_address":"sb-ba6ka3358898@personal.example.com",
        '		"payer_id":"35ZAG25DABZ62",
        '		"address":
        '		{
        '			"country_code":"DE"
        '		}
        '	},
        '	"create_time":"2020-10-06T09:47:26Z",
        '	"links":
        '	[
        '		{
        '			"href":"https://api.sandbox.paypal.com/v2/checkout/orders/2NS83667T8950703G",
        '			"rel":"self",
        '			"method":"GET"
        '		},{
        '			"href":"https://api.sandbox.paypal.com/v2/checkout/orders/2NS83667T8950703G",
        '			"rel":"update",
        '			"method":"PATCH"
        '		},{
        '			"href":"https://api.sandbox.paypal.com/v2/checkout/orders/2NS83667T8950703G/capture",
        '			"rel":"capture",
        '			"method":"POST"
        '		}
        '	]
        '}

    End Function



    Public Function GetTransactionList(Optional ByVal SearchNote As String = "") As List(Of List(Of String))
        ' payments/payment =  ?count=10&start_index=0&sort_by=create_time&sort_order=desc
        ' reporting/transactions = ?start_date=2021-01-01T01:01:01-0100&end_date=2021-01-31T23:59:59-0100&fields=all&page_size=100&page=1

        Dim StartDatUS As String = Now.AddDays(-30).ToString("yyyy-MM-dTHH:mm:ss-0700")
        Dim EndDatUS As String = Now.ToString("yyyy-MM-dTHH:mm:ss-0700")


        SetSubURL(E_SubURL._v1_reporting_transactions_PARAMS,, "?start_date=" + StartDatUS + "&end_date=" + EndDatUS + "&fields=all&page_size=100&page=1")

        Dim ResponseStr As String = PayPalRequest("", "GET", New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of List(Of String))
        End If

        Dim JSONList As List(Of Object) = JSONRecursive(ResponseStr)
        Dim ReturnList As List(Of List(Of String)) = New List(Of List(Of String))
        Dim TXList = RecursiveSearch(JSONList, "transaction_details")

        If TXList.GetType.Name = GetType(Boolean).Name Then

        Else

            For Each TX In TXList

                Dim TXInfoList = RecursiveSearch(TX, "transaction_info")

                If TXInfoList.GetType.Name = GetType(Boolean).Name Then

                Else

                    Dim TXNote = RecursiveSearch(TXInfoList, "transaction_note")
                    Dim TXAmountList = RecursiveSearch(TXInfoList, "transaction_amount")
                    Dim TXStatus As String = RecursiveSearch(TXInfoList, "transaction_status")


                    Dim TXAmount As String = "0.0"
                    If TXAmountList.GetType.Name = GetType(Boolean).Name Then
                    Else
                        TXAmount = RecursiveSearch(TXAmountList, "value")
                    End If


                    If TXNote.GetType.Name = GetType(Boolean).Name Then
                    Else

                        If Not SearchNote.Trim = "" Then

                            If TXNote.ToString.Trim.Contains(SearchNote.Trim) Then

                                Dim NuList As List(Of String) = New List(Of String)

                                NuList.Add("<transaction_note>" + SearchNote.ToString + "</transaction_note>")
                                NuList.Add("<transaction_amount>" + TXAmount.Replace(".", ",") + "</transaction_amount>")
                                NuList.Add("<transaction_status>" + TXStatus + "</transaction_status>")

                                ReturnList.Add(NuList)

                                Return ReturnList

                            End If

                        Else

                            Dim NuList As List(Of String) = New List(Of String)

                            NuList.Add("<transaction_note>" + TXNote.ToString.Trim + "</transaction_note>")
                            NuList.Add("<transaction_amount>" + TXAmount.Replace(".", ",") + "</transaction_amount>")
                            NuList.Add("<transaction_status>" + TXStatus + "</transaction_status>")

                            ReturnList.Add(NuList)

                        End If

                    End If

                End If

            Next

        End If




        Return ReturnList



#Region "normal Payments"

        '{
        '	"transaction_details":
        '	[
        '		{
        '			"transaction_info":
        '			{
        '				"paypal_account_id":"X4Q272J3TRDRJ",
        '				"transaction_id":"8T494979HY558923F",
        '				"transaction_event_code":"T0000",
        '				"transaction_initiation_date":"2021-01-22T08:02:08+0000",
        '				"transaction_updated_date":"2021-01-22T08:02:08+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"8.63"
        '				},
        '				"transaction_status":"S",
        '				"transaction_note":"pommes", <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-305.08"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-305.08"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"account_id":"X4Q272J3TRDRJ",
        '				"email_address":"Jack.Jack@burstcoin.com",
        '				"address_status":"N",
        '				"payer_status":"Y",
        '				"payer_name":
        '				{
        '					"given_name":"Jack",
        '					"surname":"Jackman",
        '					"alternate_full_name":"Jack Jackman"
        '				},
        '				"country_code":"US"
        '			},
        '			"shipping_info":
        '			{
        '				"name":"Jack, Jackman"
        '			},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"paypal_account_id":"X4Q272J3TRDRJ",
        '				"transaction_id":"3TN98377CT965432N",
        '				"transaction_event_code":"T0000",
        '				"transaction_initiation_date":"2021-01-22T08:34:19+0000",
        '				"transaction_updated_date":"2021-01-22T08:34:19+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"USD",
        '					"value":"-10.99"
        '				},
        '				"fee_amount":
        '				{
        '					"currency_code":"USD",
        '					"value":"-0.11"
        '				},
        '				"transaction_status":"S",
        '				"transaction_note":"erdbeeren",<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        '				"ending_balance":
        '				{
        '					"currency_code":"USD",
        '					"value":"-11.10"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"USD",
        '					"value":"-11.10"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"account_id":"X4Q272J3TRDRJ",
        '				"email_address":"Jack.Jack@burstcoin.com",
        '				"address_status":"N",
        '				"payer_status":"Y",
        '				"payer_name":
        '				{
        '					"given_name":"Jack",
        '					"surname":"Jackman",
        '					"alternate_full_name":"Jack Jackman"
        '				},
        '				"country_code":"US"
        '			},
        '			"shipping_info":
        '			{
        '				"name":"evo, lver"
        '			},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"transaction_id":"0Y902817BE6359618",
        '				"paypal_reference_id":"3TN98377CT965432N",
        '				"paypal_reference_id_type":"TXN",
        '				"transaction_event_code":"T0200",
        '				"transaction_initiation_date":"2021-01-22T08:34:19+0000",
        '				"transaction_updated_date":"2021-01-22T08:34:19+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-10.10"
        '				},
        '				"transaction_status":"S",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-315.18"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-315.18"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"address_status":"N",
        '				"payer_name":{}
        '			},
        '			"shipping_info":{},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"transaction_id":"0PR94740RF748560F",
        '				"paypal_reference_id":"3TN98377CT965432N",
        '				"paypal_reference_id_type":"TXN",
        '				"transaction_event_code":"T0200",
        '				"transaction_initiation_date":"2021-01-22T08:34:19+0000",
        '				"transaction_updated_date":"2021-01-22T08:34:19+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"USD",
        '					"value":"11.10"
        '				},
        '				"transaction_status":"S",
        '				"ending_balance":
        '				{
        '					"currency_code":"USD",
        '					"value":"0.00"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"USD",
        '					"value":"0.00"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"address_status":"N",
        '               "payer_name":{}
        '			},
        '			"shipping_info":{},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		}
        '	],
        '	"account_number":"226HFCQJADVLE",
        '	"start_date":"2020-12-23T19:56:05+0000",
        '	"end_date":"2021-01-22T09:59:59+0000",
        '	"last_refreshed_datetime":"2021-01-22T09:59:59+0000",
        '	"page":1,
        '	"total_items":4,
        '	"total_pages":1,
        '	"links":
        '	[
        '		{
        '			"href":"https://api.sandbox.paypal.com/v1/reporting/transactions?end_date=2021-01-22T12%3A56%3A05-0700&fields=all&start_date=2020-12-23T12%3A56%3A05-0700&page_size=100&page=1",
        '			"rel":"self",
        '			"method":"GET"
        '		}
        '	]
        '}

#End Region

#Region "Order Transactions"

        '{
        '"transaction_details":
        '	[
        '		{
        '			"transaction_info":
        '			{
        '				"transaction_id":"3CG39330U9149701F",
        '				"transaction_event_code":"T1503",
        '				"transaction_initiation_date":"2020-11-04T03:17:22+0000",
        '				"transaction_updated_date":"2020-11-04T03:17:22+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-125.46"
        '				},
        '				"transaction_status":"S",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-62.79"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-62.79"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"address_status":"N",
        '				"payer_name":
        '				{}
        '			},
        '			"shipping_info":{},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"paypal_account_id":"X4Q272J3TRDRJ",
        '				"transaction_id":"66K47261S71446539",
        '				"transaction_event_code":"T0001",
        '				"transaction_initiation_date":"2020-11-04T03:17:23+0000",
        '				"transaction_updated_date":"2020-11-04T03:17:23+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-123.00"
        '				},
        '				"fee_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-2.46"
        '				},
        '				"transaction_status":"S",
        '				"transaction_subject":"You got a Payment from BLS!",
        '				"transaction_note":"You have received a automatic Payment from BLS!",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-188.25"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-188.25"
        '				},
        '				"custom_field":"A0FB97F7",
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"account_id":"X4Q272J3TRDRJ",
        '				"email_address":"Jack.Jack@burstcoin.com",
        '				"address_status":"N",
        '				"payer_status":"Y",
        '				"payer_name":
        '				{
        '					"given_name":"Jack",
        '					"surname":"Jackman",
        '					"alternate_full_name":"Jack Jackman"
        '				},
        '				"country_code":"US"
        '			},
        '			"shipping_info":
        '			{
        '				"name":"evo, lver"
        '			},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"transaction_id":"56P0311095152650R",
        '				"paypal_reference_id":"66K47261S71446539",
        '				"paypal_reference_id_type":"TXN",
        '				"transaction_event_code":"T1105",
        '				"transaction_initiation_date":"2020-11-04T03:17:23+0000",
        '				"transaction_updated_date":"2020-11-04T03:17:23+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"125.46"
        '				},
        '				"transaction_status":"S",
        '				"transaction_subject":"You got a Payment from BLS!",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-62.79"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-62.79"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"address_status":"N",
        '				"payer_name":{}
        '			},
        '			"shipping_info":{},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"transaction_id":"7E108794XK360315M",
        '				"transaction_event_code":"T1503",
        '				"transaction_initiation_date":"2020-11-04T03:18:34+0000",
        '				"transaction_updated_date":"2020-11-04T03:18:34+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-125.46"
        '				},
        '				"transaction_status":"S",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-188.25"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-188.25"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"address_status":"N",
        '				"payer_name":{}
        '			},
        '			"shipping_info":{},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"paypal_account_id":"X4Q272J3TRDRJ",
        '				"transaction_id":"19W4763450106520H",
        '				"transaction_event_code":"T0001",
        '				"transaction_initiation_date":"2020-11-04T03:18:35+0000",
        '				"transaction_updated_date":"2020-11-04T03:18:35+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-123.00"
        '				},
        '				"fee_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-2.46"
        '				},
        '				"transaction_status":"S",
        '				"transaction_subject":"You got a Payment from BLS!",
        '				"transaction_note":"You have received a automatic Payment from BLS!",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-313.71"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-313.71"
        '				},
        '				"custom_field":"DE20CDE7",
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"account_id":"X4Q272J3TRDRJ",
        '				"email_address":"Jack.Jack@burstcoin.com",
        '				"address_status":"N",
        '				"payer_status":"Y",
        '				"payer_name":
        '				{
        '					"given_name":"Jack",
        '					"surname":"Jackman",
        '					"alternate_full_name":"Jack Jackman"
        '				},
        '				"country_code":"US"
        '			},
        '			"shipping_info":
        '			{
        '				"name":"evo, lver"
        '			},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"transaction_id":"18E91754AW612013M",
        '				"paypal_reference_id":"19W4763450106520H",
        '				"paypal_reference_id_type":"TXN",
        '				"transaction_event_code":"T1105",
        '				"transaction_initiation_date":"2020-11-04T03:18:35+0000",
        '				"transaction_updated_date":"2020-11-04T03:18:35+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"125.46"
        '				},
        '				"transaction_status":"S",
        '				"transaction_subject":"You got a Payment from BLS!",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-188.25"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-188.25"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"address_status":"N",
        '				"payer_name":{}
        '			},
        '			"shipping_info":{},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"transaction_id":"012549011F338644V",
        '				"transaction_event_code":"T1503",
        '				"transaction_initiation_date":"2020-11-04T04:12:47+0000",
        '				"transaction_updated_date":"2020-11-04T04:12:47+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-125.46"
        '				},
        '				"transaction_status":"S",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-313.71"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-313.71"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"address_status":"N",
        '				"payer_name":{}
        '			},
        '			"shipping_info":{},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"paypal_account_id":"X4Q272J3TRDRJ",
        '				"transaction_id":"3W203167R09582206",
        '				"transaction_event_code":"T0001",
        '				"transaction_initiation_date":"2020-11-04T04:12:48+0000",
        '				"transaction_updated_date":"2020-11-04T04:12:48+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-123.00"
        '				},
        '				"fee_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-2.46"
        '				},
        '				"transaction_status":"S",
        '				"transaction_subject":"You got a Payment from BLS!",
        '				"transaction_note":"You have received a automatic Payment from BLS!",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-439.17"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-439.17"
        '				},
        '				"custom_field":"606F2547",
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"account_id":"X4Q272J3TRDRJ",
        '				"email_address":"Jack.Jack@burstcoin.com",
        '				"address_status":"N",
        '				"payer_status":"Y",
        '				"payer_name":
        '				{
        '					"given_name":"Jack",
        '					"surname":"Jackman",
        '					"alternate_full_name":"Jack Jackman"
        '				},
        '				"country_code":"US"
        '			},
        '			"shipping_info":
        '			{
        '				"name":"evo, lver"
        '			},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		},{
        '			"transaction_info":
        '			{
        '				"transaction_id":"9AA61785GB401852S",
        '				"paypal_reference_id":"3W203167R09582206",
        '				"paypal_reference_id_type":"TXN",
        '				"transaction_event_code":"T1105",
        '				"transaction_initiation_date":"2020-11-04T04:12:48+0000",
        '				"transaction_updated_date":"2020-11-04T04:12:48+0000",
        '				"transaction_amount":
        '				{
        '					"currency_code":"EUR",
        '					"value":"125.46"
        '				},
        '				"transaction_status":"S",
        '				"transaction_subject":"You got a Payment from BLS!",
        '				"ending_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-313.71"
        '				},
        '				"available_balance":
        '				{
        '					"currency_code":"EUR",
        '					"value":"-313.71"
        '				},
        '				"protection_eligibility":"02"
        '			},
        '			"payer_info":
        '			{
        '				"address_status":"N",
        '				"payer_name":{}
        '			},
        '			"shipping_info":{},
        '			"cart_info":{},
        '			"store_info":{},
        '			"auction_info":{},
        '			"incentive_info":{}
        '		}
        '	],
        '	"account_number":"226HFCQJADVLE",
        '	"start_date":"2020-11-01T08:01:01+0000",
        '	"end_date":"2020-11-05T06:59:59+0000",
        '	"last_refreshed_datetime":"2021-01-22T01:59:59+0000",
        '	"page":1,
        '	"total_items":9,
        '	"total_pages":1,
        '	"links":
        '	[
        '		{
        '			"href":"https://api.sandbox.paypal.com/v1/reporting/transactions?end_date=2020-11-04T23%3A59%3A59-0700&fields=all&start_date=2020-11-01T01%3A01%3A01-0700&page_size=100&page=1",
        '			"rel":"self",
        '			"method":"GET"
        '		}
        '	]
        '}

#End Region


    End Function



    Public Function AuthOrder()

        SetSubURL(ClsPayPal.E_SubURL._v2_checkout_orders_ID_authorize, "2NS83667T8950703G")

        Dim ResponseStr As String = PayPalRequest("", , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        ResponseStr = ResponseStr

    End Function



    Public Function GetPaymentDetails()

        SetSubURL(ClsPayPal.E_SubURL._v2_payments_authorizations_AUTHID, "7EJ68552ST726563J")

        Dim ResponseStr As String = PayPalRequest("", "GET", New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        ResponseStr = ResponseStr


    End Function
    Public Function CaptureAuthPayment()

        SetSubURL(ClsPayPal.E_SubURL._v2_payments_authorizations_AUTHID_capture, "7EJ68552ST726563J")

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

        SetSubURL(ClsPayPal.E_SubURL._v2_payments_captures_CAPID, "7EJ68552ST726563J")

        Dim ResponseStr As String = PayPalRequest("", "GET", New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        ResponseStr = ResponseStr


        '{
        '	"id":"7EJ68552ST726563J",
        '	"amount":
        '	{
        '		"currency_code":"USD",
        '		"value":"1.23"
        '	},
        '	"final_capture":true,
        '	"seller_protection":
        '	{
        '		"status":"ELIGIBLE",
        '		"dispute_categories":
        '		[
        '			"ITEM_NOT_RECEIVED",
        '			"UNAUTHORIZED_TRANSACTION"
        '		]
        '	},
        '	"status":"PENDING",
        '	"status_details":
        '	{
        '		"reason":"RECEIVING_PREFERENCE_MANDATES_MANUAL_ACTION"
        '	},
        '	"create_time":"2020-10-06T12:22:41Z",
        '	"update_time":"2020-10-06T12:22:41Z",
        '	"links":
        '	[
        '		{
        '			"href":"https://api.sandbox.paypal.com/v2/payments/captures/7EJ68552ST726563J",
        '			"rel":"self",
        '			"method":"GET"
        '		},{
        '			"href":"https://api.sandbox.paypal.com/v2/payments/captures/7EJ68552ST726563J/refund",
        '			"rel":"refund",
        '			"method":"POST"
        '		},{
        '			"href":"https://api.sandbox.paypal.com/v2/checkout/orders/2NS83667T8950703G",
        '			"rel":"up",
        '			"method":"GET"
        '		}
        '	]
        '}



    End Function


    Enum E_RecipientType
        EMAIL = 0
        PHONE = 1
        PAYPAL_ID = 2
    End Enum

    Public Function CreateBatchPayOut(ByVal Recipient As String, ByVal Amount As Double, Optional ByVal SendType As E_RecipientType = E_RecipientType.EMAIL, Optional ByVal Currency As String = "USD")

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

        If SendType = E_RecipientType.EMAIL Then

            Request += """recipient_type"":""EMAIL"", "
            Request += """amount"": { "
            Request += """value"": """ + Amount.ToString.Replace(",", ".") + """, "
            Request += """currency"":""" + Currency + """"
            Request += "}, "
            Request += """note"": ""thank you"", "
            Request += """sender_item_id"": """ + Hash + """, "
            Request += """receiver"": """ + Recipient + """" 'sb-ba6ka3358898@personal.example.com

        ElseIf SendType = E_RecipientType.PAYPAL_ID Then

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

        SetSubURL(ClsPayPal.E_SubURL._v1_payments_payouts)
        Dim ResponseStr As String = PayPalRequest(Request, , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        Dim ReturnList As List(Of Object) = JSONRecursive(ResponseStr)
        Dim NuList As List(Of String) = New List(Of String)
        Dim x = RecursiveSearch(ReturnList, "payout_batch_id")

        Return x

    End Function

    Public Function CheckPayOuts()

        SetSubURL(ClsPayPal.E_SubURL._v1_payments_payouts)
        Dim ResponseStr As String = PayPalRequest("", , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        ResponseStr = ResponseStr

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

                If Not Input.Trim = "" Then
                    If Input(0) = "," Then
                        Input = Input.Substring(1)
                    End If
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


        '{
        '	"result":
        '	{
        '		"txid":"5f17e18c0e0363fc3a14dd6c2e74141d6f0e636d0592b48d94d0a6272dcb4ef3",
        '		"hash":"188bf760059888f9304321cfb771586b71ef50e1b4f988fdba8db3b405864c5d",
        '		"version":2,
        '		"size":222,
        '		"vsize":141,
        '		"weight":561,
        '		"locktime":1746616,
        '		"vin":
        '		[
        '			{
        '				"txid":"9f30228774543ac5c5ba07a56e26e6079ae1c9221fa3eccb7798596cbb401711",
        '				"vout":0,
        '				"scriptSig":
        '				{
        '					"asm":"",
        '					"hex":""
        '				},
        '				"txinwitness":
        '				[
        '					"30440220225eef02bf7d4f9b655b61a96e8095f9b5f7e482748dbb8d26d642eee64977f70220566064db332acd0577a8918826c03842ce1e6eb19ee8ba3c2fef9a1b6d89cefe01",
        '					"031abc3d54d4d388dd81f154395566397911ff0e6a6e6317d2579d195416332a3d"
        '				],
        '				"sequence":4294967294
        '			}
        '		],
        '		"vout":
        '		[
        '			{
        '				"value":0.01198846,
        '				"n":0,
        '				"scriptPubKey":
        '				{
        '					"asm":"0 b0f27c0a32d29689acf19940c62fade458e5edb1",
        '					"hex":"0014b0f27c0a32d29689acf19940c62fade458e5edb1",
        '					"reqSigs":1,
        '					"type":"witness_v0_keyhash",
        '					"addresses":
        '					[
        '						"tb1qkre8cz3j62tgnt83n9qvvtadu3vwtmd3fzx5vd"
        '					]
        '				}
        '			},
        '			{
        '				"value":0.00500000,
        '				"n":1,
        '				"scriptPubKey":
        '				{
        '					"asm":"0 0f0f5f15ce92234399a10469a2ca7f890eccc2cc",
        '					"hex":"00140f0f5f15ce92234399a10469a2ca7f890eccc2cc",
        '					"reqSigs":1,
        '					"type":"witness_v0_keyhash",
        '					"addresses":
        '					[
        '						"tb1qpu8479wwjg358xdpq3569jnl3y8veskvwqugjt"
        '					]
        '				}
        '			}
        '		]
        '	},
        '	"error":null,
        '    "id": 1
        '}



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
