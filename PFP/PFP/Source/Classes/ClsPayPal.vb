Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Security.Cryptography

Public Class ClsPayPal

    Const SandboxURL As String = "https://www.sandbox.paypal.com"
    Const LiveURL As String = "https://www.api.paypal.com"

    Property Currencys As List(Of String) = New List(Of String)({"AUD", "BRL", "CAD", "CNY", "CZK", "DKK", "EUR", "HKD", "HUF", "INR", "ILS", "JPY", "MYR", "MXN", "TWD", "NZD", "NOK", "PHP", "PLN", "GBP", "RUB", "SGD", "SEK", "CHF", "THB", "USD"})

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

        Dim JSON As ClsJSON = New ClsJSON

        Dim XML As String = JSON.JSONToXML(ResponseStr)
        Dim NuList As List(Of String) = New List(Of String)

        Dim Erro As String = JSON.RecursiveXMLSearch(XML, "error")
        Dim Scope As String = JSON.RecursiveXMLSearch(XML, "scope")
        Dim Access_Token As String = JSON.RecursiveXMLSearch(XML, "access_token")
        Dim Token_Type As String = JSON.RecursiveXMLSearch(XML, "token_type")
        Dim App_ID As String = JSON.RecursiveXMLSearch(XML, "app_id")
        Dim Expires_In As String = JSON.RecursiveXMLSearch(XML, "expires_in")
        Dim Nonce As String = JSON.RecursiveXMLSearch(XML, "nonce")

        If Erro = "False" Or Erro = "" Then
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

        Dim JSON As ClsJSON = New ClsJSON
        Dim ReturnList As List(Of Object) = JSON.JSONRecursive(ResponseStr)

        Dim XML As String = JSON.JSONListToXMLRecursive(ReturnList)



        Dim NuList As List(Of String) = New List(Of String)

        Dim ID As String = JSON.RecursiveListSearch(ReturnList, "id")
        Dim Status As String = JSON.RecursiveListSearch(ReturnList, "status")

        NuList.Add("<id>" + ID + "</id>")
        NuList.Add("<status>" + Status + "</status>")

        Dim URLList = JSON.RecursiveListSearch(ReturnList, "links")
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
                            T_Str += "<href>" + KeyVal(1) + ":" + KeyVal(2) + "</href>"
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

#Region "CreateOrder"

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

#End Region

    End Function
    Public Function CaptureOrder(ByVal OrderID As String) As List(Of String)

        SetSubURL(ClsPayPal.E_SubURL._v2_checkout_orders_ID_capture, OrderID)

        Dim ResponseStr As String = PayPalRequest("", , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If
        Dim JSON As ClsJSON = New ClsJSON
        Dim ResponseList As List(Of Object) = JSON.JSONRecursive(ResponseStr)

        Dim ReturnList As List(Of String) = New List(Of String)

        Dim ID As String = JSON.RecursiveListSearch(ResponseList, "id")
        Dim Status As String = JSON.RecursiveListSearch(ResponseList, "status")

        ReturnList.Add("<id>" + ID + "</id>")
        ReturnList.Add("<status>" + Status + "</status>")

        Dim URLList = JSON.RecursiveListSearch(ResponseList, "links")
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

#Region "CaptureOrder"

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

#End Region

    End Function
    Public Function GetOrderDetails(ByVal OrderID As String) As List(Of String)

        SetSubURL(ClsPayPal.E_SubURL._v2_checkout_orders_ID, OrderID)

        Dim ResponseStr As String = PayPalRequest("", "GET", New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of String)
        End If

        Dim JSON As ClsJSON = New ClsJSON
        Dim ReturnList As List(Of Object) = JSON.JSONRecursive(ResponseStr)

        Dim NuList As List(Of String) = New List(Of String)

        Dim ID As String = JSON.RecursiveListSearch(ReturnList, "id")
        Dim Intent As String = JSON.RecursiveListSearch(ReturnList, "intent")
        Dim Status As String = JSON.RecursiveListSearch(ReturnList, "status")

        NuList.Add("<id>" + ID + "</id>")
        NuList.Add("<intent>" + Intent + "</intent>")
        NuList.Add("<status>" + Status + "</status>")

        Dim URLList = JSON.RecursiveListSearch(ReturnList, "links")
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

#Region "GetOrderDetails"

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
        '					"name":"x 123 Signa",
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

#End Region

    End Function

    Public Function GetTransactionList(Optional ByVal SearchNote As String = "") As List(Of List(Of String))
        ' payments/payment =  ?count=10&start_index=0&sort_by=create_time&sort_order=desc
        ' reporting/transactions = ?start_date=2021-01-01T01:01:01-0100&end_date=2021-01-31T23:59:59-0100&fields=all&page_size=100&page=1

        Dim StartDatUS As String = Now.AddDays(-29).ToString("yyyy-MM-dTHH:mm:ss-0500")
        Dim EndDatUS As String = Now.AddDays(1).ToString("yyyy-MM-dTHH:mm:ss-0500")

        SetSubURL(E_SubURL._v1_reporting_transactions_PARAMS,, "?start_date=" + StartDatUS + "&end_date=" + EndDatUS + "&fields=all&page_size=100&page=1")

        Dim ResponseStr As String = PayPalRequest("", "GET", New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        If ResponseStr.Trim = "" Then
            Return New List(Of List(Of String))
        End If

        Dim JSON As ClsJSON = New ClsJSON
        Dim JSONList As List(Of Object) = JSON.JSONRecursive(ResponseStr)
        Dim XML As String = JSON.JSONListToXMLRecursive(JSONList)

        Dim ReturnList As List(Of List(Of String)) = New List(Of List(Of String))
        Dim TXList = JSON.RecursiveListSearch(JSONList, "transaction_details")

        If TXList.GetType.Name = GetType(Boolean).Name Then

        Else

            For Each TX In TXList

                Dim TXInfoList = JSON.RecursiveListSearch(TX, "transaction_info")

                If TXInfoList.GetType.Name = GetType(Boolean).Name Then

                Else

                    Dim TXNote = JSON.RecursiveListSearch(TXInfoList, "transaction_note")
                    Dim TXAmountList = JSON.RecursiveListSearch(TXInfoList, "transaction_amount")
                    Dim TXStatus As String = JSON.RecursiveListSearch(TXInfoList, "transaction_status")


                    Dim TXAmount As String = "0.0"
                    If TXAmountList.GetType.Name = GetType(Boolean).Name Then
                    Else
                        TXAmount = JSON.RecursiveListSearch(TXAmountList, "value")
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

    Public Function CreateBatchPayOut(ByVal Recipient As String, ByVal Amount As Double, Optional ByVal Currency As String = "USD", Optional ByVal Note As String = "thank you", Optional ByVal SendType As E_RecipientType = E_RecipientType.EMAIL)

        Dim Hash As String = CreateHash(Now.ToLongDateString + " " + Now.ToLongTimeString)

        Hash = Hash.Substring(0, 4) + Hash.Substring(Hash.Length - 4)

        Dim Request = "{"
        Request += """sender_batch_header"": { "
        Request += """sender_batch_id"": """ + Hash + ""","
        Request += """email_subject"":""You got a Payment from PFP!"","
        Request += """email_message"": ""You have received a automatic Payment from PFP!"""
        Request += "},"
        Request += """items"": ["
        Request += "{"

        If SendType = E_RecipientType.EMAIL Then

            Request += """recipient_type"":""EMAIL"", "
            Request += """amount"": { "
            Request += """value"": """ + Amount.ToString.Replace(",", ".") + """, "
            Request += """currency"":""" + Currency + """"
            Request += "}, "
            Request += """note"": """ + Note + """, "
            Request += """sender_item_id"": """ + Hash + """, "
            Request += """receiver"": """ + Recipient + """" 'sb-ba6ka3358898@personal.example.com

        ElseIf SendType = E_RecipientType.PAYPAL_ID Then

            Request += """recipient_type"":""PAYPAL_ID"", "
            Request += """amount"": { "
            Request += """value"": """ + Amount.ToString.Replace(",", ".") + """, "
            Request += """currency"": """ + Currency + """"
            Request += "}, "
            Request += """note"": """ + Note + """, "
            Request += """sender_item_id"": """ + Hash + """, "
            Request += """receiver"": """ + Recipient + """" '35ZAG25DABZ62

        End If

        Request += "}"
        Request += "]"
        Request += "}"

        SetSubURL(ClsPayPal.E_SubURL._v1_payments_payouts)
        Dim ResponseStr As String = PayPalRequest(Request, , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        Dim JSON As ClsJSON = New ClsJSON

        Dim ReturnList As List(Of Object) = JSON.JSONRecursive(ResponseStr)

        Dim XML As String = JSON.JSONListToXMLRecursive(ReturnList)



        Dim x = JSON.RecursiveListSearch(ReturnList, "payout_batch_id")

        Return x

    End Function

    Public Function CheckPayOuts()

        SetSubURL(ClsPayPal.E_SubURL._v1_payments_payouts)
        Dim ResponseStr As String = PayPalRequest("", , New List(Of String)({"Authorization: Basic " + AuthCredentials}))

        ResponseStr = ResponseStr

    End Function

#Region "Toolfunctions"

    Private Structure S_Sorter
        Dim Timestamp As ULong
        Dim TXID As ULong
    End Structure

    Private Function SortTimeStamp(ByVal input As List(Of List(Of String))) As List(Of List(Of String))

        Dim TSSort As List(Of S_Sorter) = New List(Of S_Sorter)

        For i As Integer = 0 To input.Count - 1

            Dim Entry As List(Of String) = input(i)

            Dim T_Timestamp As ULong = BetweenFromList(Entry, "<timestamp>", "</timestamp>",, GetType(ULong))
            Dim T_Transaction As ULong = BetweenFromList(Entry, "<transaction>", "</transaction>",, GetType(ULong))

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

                Dim T_Timestamp As ULong = BetweenFromList(retent, "<timestamp>", "</timestamp>",, GetType(ULong))
                Dim T_Transaction As ULong = BetweenFromList(retent, "<transaction>", "</transaction>",, GetType(ULong))

                If T_Timestamp = sort.Timestamp And T_Transaction = sort.TXID Then
                    SReturnList.Add(retent)
                    Exit For
                End If

            Next

        Next

        Return SReturnList

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