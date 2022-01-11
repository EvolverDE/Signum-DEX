
'Public Class ClsPayPalManager
'    Inherits ClassTemplate

'    ''' <summary>
'    ''' Leitet den Kunden an PayPal weiter um dort seine Accountangaben zu machen
'    ''' </summary>
'    ''' <param name="moSessionManager">SessionManager</param>
'    ''' <param name="context">HttpContext</param>
'    ''' <returns>
'    ''' <b>false</b> wenn ein Fehler im SetExpressCheckout auftritt
'    ''' </returns>
'    ''' <remarks></remarks>
'    Public Shared Function SetPayPalExpressCheckoutWithRedirect(ByVal moSessionManager As SessionManager, ByVal context As System.Web.HttpContext, ByRef errorMSG As String, ByVal applicationManager As ApplicationManager) As Boolean

'        Dim redirectPath As String = SetPayPalExpressCheckout(moSessionManager, context, errorMSG, applicationManager)
'        If Not redirectPath = "" Then
'            context.Response.Redirect(redirectPath)
'        End If

'        Return False

'    End Function

'    ''' <summary>
'    ''' Liefert den Redirect-String zu PayPal-Express
'    ''' </summary>
'    ''' <param name="moSessionManager">SessionManager</param>
'    ''' <param name="context">HttpContext</param>
'    ''' <param name="errorMSG">errorMSG</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Shared Function SetPayPalExpressCheckout(ByVal moSessionManager As SessionManager, ByVal context As System.Web.HttpContext, ByRef errorMSG As String, ByVal applicationManager As ApplicationManager) As String

'        If Not SetPaymentCondition(applicationManager, moSessionManager, context) Then
'            errorMSG = Basics.GetTextResource(applicationManager, moSessionManager, 1331)
'            Return ""
'        End If

'        Dim parameterList As New System.Collections.Generic.SortedList(Of String, String)

'        'Return- und CancelUrl ermitteln
'        Dim returnURL As String = Basics.FormatURL(applicationManager.WebPath) & "PayPal.htm"
'        Dim cancelURL As String = Basics.FormatURL(applicationManager.WebPath) & "PayPal.htm"

'        parameterList.Add("RETURNURL", returnURL)
'        parameterList.Add("CANCELURL", cancelURL)

'        If CType(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Giropay_verwenden), Boolean) Then
'            parameterList.Add("GIROPAYSUCCESSURL", returnURL)
'            parameterList.Add("GIROPAYCANCELURL", cancelURL)
'        End If

'        If CType(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_ELV_verwenden), Boolean) Then
'            parameterList.Add("BANKTXNPENDINGURL", returnURL)
'        End If

'        parameterList.Add("AMT", Replace(Format(moSessionManager.ShoppingCart.Amounts.Total, "#0.00"), ",", "."))
'        parameterList.Add("CURRENCYCODE", moSessionManager.Customer.CurrencyCode(applicationManager, moSessionManager))
'        parameterList.Add("PAYMENTACTION", "Sale")

'        If Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Description) <> "" Then
'            parameterList.Add("DESC", Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Description))
'        End If

'        parameterList.Add("REQCONFIRMSHIPPING", "0")

'        If moSessionManager.Customer.CustomerId = 0 OrElse Not CType(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_AddessOverRide), Boolean) Then
'            parameterList.Add("ADDROVERRIDE", "0")
'        Else
'            parameterList.Add("ADDROVERRIDE", "1")
'        End If

'        parameterList.Add("LOCALECODE", moSessionManager.Language)
'        parameterList.Add("HDRIMG", Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPal_Image))

'        If Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Pagestyle) <> "" Then
'            parameterList.Add("PAGESTYLE", Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Pagestyle))
'        End If

'        If Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_BorderColor) <> "" Then
'            parameterList.Add("HDRBORDERCOLOR", Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_BorderColor))
'        End If

'        If Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_BackColor) <> "" Then
'            parameterList.Add("HDRBACKCOLOR", Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_BackColor))
'        End If

'        If Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_PayflowColor) <> "" Then
'            parameterList.Add("PAYFLOWCOLOR", Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_PayflowColor))
'        End If

'        'Artikeldetails zusammen bauen
'        Dim parameterDetailList As New System.Collections.Generic.SortedList(Of String, String)

'        Dim i As Integer = 0
'        Dim itemAmt As Decimal = CType(0.0, Decimal)
'        Dim myItems As Generic.List(Of Item) = ItemHandler.GetItemsByType(ItemsType.ShoppingCartWithAccessories)
'        Dim unitPrice As Decimal = CType(0.0, Decimal)

'        For Each item As Item In myItems

'            'Normale Position
'            unitPrice = item.GetCalculatedUnitPrice
'            parameterDetailList.Add("L_NAME" & i.ToString, Left(item.Title, 50))
'            parameterDetailList.Add("L_DESC" & i.ToString, Left(item.Descriptions.ShortDescription, 50))
'            parameterDetailList.Add("L_AMT" & i.ToString, Replace(Format(unitPrice, "#0.00"), ",", "."))
'            parameterDetailList.Add("L_NUMBER" & i.ToString, item.KHKItemId)
'            parameterDetailList.Add("L_QTY" & i.ToString, CType(CType(item.ShoppingCartInfo.Quantity, Integer), String))
'            itemAmt += Integer.Parse(item.ShoppingCartInfo.Quantity) * unitPrice

'            i += 1
'        Next

'        'Gutscheine als Artikel hinzufügen
'        Dim dVouchers As Decimal = moSessionManager.Customer.GetVouchersSum(applicationManager, moSessionManager, context)
'        If dVouchers > 0 Then
'            parameterDetailList.Add("L_NAME" & i.ToString, Left(Basics.GetTextResource(applicationManager, moSessionManager, 1037), 127))
'            parameterDetailList.Add("L_DESC" & i.ToString, Left(Basics.GetTextResource(applicationManager, moSessionManager, 1037), 127))
'            parameterDetailList.Add("L_AMT" & i.ToString, Replace(Format(dVouchers * -1, "#0.00"), ",", "."))
'            parameterDetailList.Add("L_NUMBER" & i.ToString, "xxxx")
'            parameterDetailList.Add("L_QTY" & i.ToString, "1")

'            itemAmt -= dVouchers
'            i += 1
'        End If

'        'Rabatte auf Zahlungskondition als Artikel hinzufügen
'        If Not moSessionManager.ShoppingCart.Amounts.PaymentConditionDiscountAmount = 0.0 Then
'            parameterDetailList.Add("L_NAME" & i.ToString, Left(Basics.GetTextResource(applicationManager, moSessionManager, 1258), 127))
'            parameterDetailList.Add("L_DESC" & i.ToString, Left(Basics.GetTextResource(applicationManager, moSessionManager, 1258), 127))
'            parameterDetailList.Add("L_AMT" & i.ToString, Replace(CType(moSessionManager.ShoppingCart.Amounts.PaymentConditionDiscountAmount * -1, String), ",", "."))
'            parameterDetailList.Add("L_NUMBER" & i.ToString, "----")
'            parameterDetailList.Add("L_QTY" & i.ToString, "1")

'            itemAmt -= moSessionManager.ShoppingCart.Amounts.PaymentConditionDiscountAmount
'            i += 1
'        End If

'        'Zuschläge als Artikel hinzufügen
'        If Not moSessionManager.ShoppingCart.Amounts.Surcharge.Count = 0.0 Then
'            For Each surch As Surcharge In moSessionManager.ShoppingCart.Amounts.Surcharge
'                parameterDetailList.Add("L_NAME" & i.ToString, Left(surch.SurchargeName, 127))
'                parameterDetailList.Add("L_DESC" & i.ToString, String.Empty)
'                parameterDetailList.Add("L_AMT" & i.ToString, Replace(Format(surch.SurchargeAmountTotal, "#0.00"), ",", "."))
'                parameterDetailList.Add("L_NUMBER" & i.ToString, "----")
'                parameterDetailList.Add("L_QTY" & i.ToString, "1")

'                itemAmt += surch.SurchargeAmountTotal
'                i += 1
'            Next
'        End If

'        parameterDetailList.Add("SHIPPINGAMT", Replace(CType(Math.Round(moSessionManager.ShoppingCart.Amounts.ForwardingCharges, 2), String), ",", "."))
'        parameterDetailList.Add("ITEMAMT", Replace(CType(Math.Round(itemAmt, 2, MidpointRounding.AwayFromZero), String), ",", "."))


'        'Keine Details an Paypal übermitteln, wenn
'        '1: Rundungsfehler existieren
'        '2: die Anzahl von Artikeln + Gutscheinzeilen, usw... >= 22 Positionen (sonst: Fehler 10001 (Internal Error/Timeout))
'        If Math.Round(itemAmt, 2, MidpointRounding.AwayFromZero) + Math.Round(moSessionManager.ShoppingCart.Amounts.ForwardingCharges, 2) _
'            = moSessionManager.ShoppingCart.Amounts.Total AndAlso i <= 22 Then

'            For Each para As Generic.KeyValuePair(Of String, String) In parameterDetailList
'                parameterList.Add(para.Key, para.Value)
'            Next
'        Else
'            If Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Description) <> "" Then
'                parameterList.Add("L_NAME0", Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Description))
'            Else
'                parameterList.Add("L_NAME0", "")
'            End If
'            parameterList.Add("L_DESC0", "")
'            parameterList.Add("L_AMT0", Replace(CType(Math.Round(moSessionManager.ShoppingCart.Amounts.Total, 2, MidpointRounding.AwayFromZero) - Math.Round(moSessionManager.ShoppingCart.Amounts.ForwardingCharges, 2), String), ",", "."))
'            parameterList.Add("L_NUMBER0", "")
'            parameterList.Add("L_QTY0", "1")

'            parameterList.Add("SHIPPINGAMT", Replace(CType(Math.Round(moSessionManager.ShoppingCart.Amounts.ForwardingCharges, 2), String), ",", "."))
'            parameterList.Add("ITEMAMT", Replace(CType(Math.Round(moSessionManager.ShoppingCart.Amounts.Total, 2, MidpointRounding.AwayFromZero) - Math.Round(moSessionManager.ShoppingCart.Amounts.ForwardingCharges, 2), String), ",", "."))
'        End If

'        If moSessionManager.Customer.SaleProcedure.PaymentData = "" Then
'            moSessionManager.Customer.SaleProcedure.PaymentData = moSessionManager.Customer.SaleProcedure.ShopPaymentReference
'        End If
'        parameterList.Add("CUSTOM", moSessionManager.Customer.SaleProcedure.PaymentData)

'        If Not moSessionManager.Customer.CustomerId = 0 Then
'            parameterList.Add("EMAIL", moSessionManager.Customer.MailAddress)

'            Dim customerAddress As DataTable = moSessionManager.Customer.GetAddress(applicationManager, moSessionManager, context, moSessionManager.Customer.SaleProcedure.ShippingAddressId)
'            If customerAddress.Rows.Count > 0 Then
'                Dim row As DataRow = customerAddress.Rows(0)

'                parameterList.Add("SHIPTONAME", Left(CType(row("Firstname"), String) & " " & CType(row("Lastname"), String), 32))
'                parameterList.Add("SHIPTOSTREET", Left(CType(row("Street"), String) & " " & CType(row("Housenumber"), String), 100))
'                parameterList.Add("SHIPTOCITY", Left(CType(row("City"), String), 40))
'                parameterList.Add("SHIPTOSTATE", Left(CType(row("State"), String), 40))
'                parameterList.Add("SHIPTOCOUNTRYCODE", CType(row("CountryId"), String))
'                parameterList.Add("SHIPTOZIP", Left(CType(row("ZipCode"), String), 20))
'                parameterList.Add("SHIPTOSTREET2", Left(CType(row("AdditionalName"), String), 100))
'                parameterList.Add("PHONENUM", Left(CType(row("Phone"), String), 20))
'            End If
'        End If

'        parameterList.Add("CHANNELTYPE", "Merchant")

'        Dim paypalCredentials As New PayPalExpress.ExpressCredentials(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPal_Account), Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Version), CType(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Sandbox), Boolean), Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_RedirectUrl))
'        Dim paypalAgent As New PayPalExpress.CallAgent


'        Dim responseString As String = paypalAgent.CallPayPal(paypalCredentials, PayPalExpress.ExpressCheckoutTypes.SetExpressCheckout, PayPalExpress.Codec.Encode(parameterList))

'        Dim responseList As System.Collections.Generic.SortedList(Of String, String) = PayPalExpress.Codec.Decode(responseString)

'        'Fehlerbehandlung
'        If responseList.ContainsKey("L_ERRORCODE0") Then
'            errorMSG = PayPalExpress.CallAgent.GetErrorMessageId(moSessionManager, responseList.Item("L_ERRORCODE0"), applicationManager)
'        Else
'            errorMSG = ""
'        End If

'        'RedirectPfad
'        If responseList.ContainsKey("TOKEN") Then
'            If paypalCredentials.RedirectUrl <> "" Then
'                Return (paypalCredentials.RedirectUrl & "?cmd=_express-checkout&token=" & responseList.Item("TOKEN"))
'            End If
'        End If

'        Return ""

'    End Function

'    ''' <summary>
'    ''' GetPayPalExpressCeckoutDetails
'    ''' </summary>
'    ''' <param name="moSessionManager"></param>
'    ''' <param name="token"></param>
'    ''' <param name="errorMSG"></param>
'    ''' <param name="PayPalExpressRedirect"></param>
'    ''' <param name="PayPalPaymentConditionAvailable"></param>
'    ''' <param name="applicationManager"></param>
'    ''' <param name="context"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Shared Function GetPayPalExpressCeckoutDetails(ByVal moSessionManager As SessionManager, ByVal token As String, ByRef errorMSG As String, ByRef PayPalExpressRedirect As Boolean, ByRef PayPalPaymentConditionAvailable As Boolean, ByVal applicationManager As ApplicationManager, ByVal context As HttpContext) As Boolean

'        Dim paypalCredentials As New PayPalExpress.ExpressCredentials(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPal_Account), Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Version), CType(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Sandbox), Boolean), Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_RedirectUrl))
'        Dim paypalAgent As New PayPalExpress.CallAgent
'        Dim parameterList As New System.Collections.Generic.SortedList(Of String, String)
'        parameterList.Add("TOKEN", token)

'        Dim PayPalCustomerData As PayPalAddress
'        Dim responseString As String = paypalAgent.CallPayPal(paypalCredentials, PayPalExpress.ExpressCheckoutTypes.GetExpressCheckoutDetails, PayPalExpress.Codec.Encode(parameterList))
'        Dim responseList As System.Collections.Generic.SortedList(Of String, String) = PayPalExpress.Codec.Decode(responseString)

'        PayPalExpressRedirect = False
'        If responseList.ContainsKey("ACK") AndAlso
'            (responseList.Item("ACK").ToUpperInvariant = "SUCCESS" Or responseList.Item("ACK").ToUpperInvariant = "SUCCESSWITHWARNING") AndAlso
'            Not responseList.Item("EMAIL") = "" Then

'            PayPalCustomerData = CreateShopAddress(responseList)

'            If moSessionManager.IsExpressCheckout Then
'                'Kundendaten merken, weil Kunde noch nicht angelegt werden darf
'                moSessionManager.Customer.SaleProcedure.PaymentConditionTempData.Add("CustomerData", PayPalCustomerData)
'            Else
'                CheckCustomer(applicationManager, moSessionManager, PayPalCustomerData, context)
'            End If

'            PayPalPaymentConditionAvailable = SetPaymentCondition(applicationManager, moSessionManager, context)

'            If responseList.ContainsKey("REDIRECTREQUIRED") AndAlso responseList.Item("REDIRECTREQUIRED").ToUpperInvariant = "TRUE" Then
'                PayPalExpressRedirect = True
'                errorMSG = Basics.GetTextResource(applicationManager, moSessionManager, 1038)
'            End If

'            Return True

'        Else

'            If responseList.ContainsKey("L_ERRORCODE0") Then
'                errorMSG = PayPalExpress.CallAgent.GetErrorMessageId(moSessionManager, responseList.Item("L_ERRORCODE0"), applicationManager)
'            Else
'                errorMSG = PayPalExpress.CallAgent.GetErrorMessageId(moSessionManager, "glob_ppeError1", applicationManager)
'            End If

'            Return False

'        End If

'    End Function

'    ''' <summary>
'    ''' DoPayPalExpressCheckoutPayment
'    ''' </summary>
'    ''' <param name="moSessionManager">SessionManager</param>
'    ''' <param name="token">Token</param>
'    ''' <param name="PayerId"></param>
'    ''' <param name="errorMSG">Referenzparameter errorMSG</param>
'    ''' <param name="PayPalExpressRedirect"></param>
'    ''' <param name="applicationManager"></param>
'    ''' <param name="context"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Shared Function DoPayPalExpressCheckoutPayment(ByVal moSessionManager As SessionManager, ByVal token As String, ByVal PayerId As String, ByRef errorMSG As String, ByRef payPalExpressRedirect As Boolean, ByVal applicationManager As ApplicationManager, ByVal context As HttpContext) As Boolean

'        Dim parameterList As New System.Collections.Generic.SortedList(Of String, String)
'        parameterList.Add("AMT", Replace(Format(moSessionManager.ShoppingCart.Amounts.Total, "#0.00"), ",", "."))
'        parameterList.Add("TOKEN", token)
'        parameterList.Add("CURRENCYCODE", moSessionManager.Customer.CurrencyCode(applicationManager, moSessionManager))
'        parameterList.Add("PAYMENTACTION", "Sale")
'        parameterList.Add("PAYERID", PayerId)
'        If Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Description) <> "" Then
'            parameterList.Add("DESC", Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Description))
'        End If

'        parameterList.Add("CUSTOM", moSessionManager.Customer.MailAddress)

'        Dim customerAddress As DataTable = moSessionManager.Customer.GetAddress(applicationManager, moSessionManager, context, moSessionManager.Customer.SaleProcedure.ShippingAddressId)
'        If customerAddress.Rows.Count > 0 Then
'            Dim row As DataRow = customerAddress.Rows(0)

'            parameterList.Add("SHIPTONAME", Left(CType(row("Firstname"), String) & " " & CType(row("Lastname"), String), 32))
'            parameterList.Add("SHIPTOSTREET", Left(CType(row("Street"), String) & " " & CType(row("Housenumber"), String), 100))
'            parameterList.Add("SHIPTOCITY", Left(CType(row("City"), String), 40))
'            parameterList.Add("SHIPTOSTATE", Left(CType(row("State"), String), 40))
'            parameterList.Add("SHIPTOCOUNTRYCODE", CType(row("CountryId"), String))
'            parameterList.Add("SHIPTOZIP", Left(CType(row("ZipCode"), String), 20))
'            parameterList.Add("SHIPTOSTREET2", Left(CType(row("AdditionalName"), String), 100))
'            parameterList.Add("PHONENUM", Left(CType(row("Phone"), String), 20))
'        End If

'        parameterList.Add("CHANNELTYPE", "Merchant")

'        Dim paypalCredentials As New PayPalExpress.ExpressCredentials(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPal_Account), Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Version), CType(Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_Sandbox), Boolean), Basics.GetShopProperty(applicationManager, moSessionManager, ShopProperty.PayPalExpress_RedirectUrl))
'        Dim paypalAgent As New PayPalExpress.CallAgent
'        Dim responseString As String = paypalAgent.CallPayPal(paypalCredentials, PayPalExpress.ExpressCheckoutTypes.DoExpressCheckoutPayment, PayPalExpress.Codec.Encode(parameterList))

'        Dim responseList As System.Collections.Generic.SortedList(Of String, String) = PayPalExpress.Codec.Decode(responseString)

'        If responseList.ContainsKey("ACK") AndAlso (responseList.Item("ACK").ToUpperInvariant = "SUCCESS" Or responseList.Item("ACK").ToUpperInvariant = "SUCCESSWITHWARNING") Then
'            moSessionManager.Customer.SaleProcedure.PaymentData = responseList.Item("TRANSACTIONID")

'            'Erweiterung: Redirect nur, wenn auch im DoExpress-Response Redirect auf True steht
'            If Not responseList.ContainsKey("REDIRECTREQUIRED") OrElse Not responseList.Item("REDIRECTREQUIRED").ToUpperInvariant = "TRUE" Then
'                payPalExpressRedirect = False
'            End If

'            Return True
'        Else
'            If responseList.ContainsKey("L_ERRORCODE0") Then
'                errorMSG = PayPalExpress.CallAgent.GetErrorMessageId(moSessionManager, responseList.Item("L_ERRORCODE0"), applicationManager)
'            Else
'                errorMSG = PayPalExpress.CallAgent.GetErrorMessageId(moSessionManager, "glob_ppeError1", applicationManager)
'            End If
'            Return False
'        End If

'    End Function

'    Public Shared Sub CheckCustomer(ByVal applicationManager As ApplicationManager, ByVal sessionManager As SessionManager, ByVal PayPalCustomerData As PayPalAddress, ByVal context As HttpContext)

'        If sessionManager.Customer.CustomerId = 0 Then

'            Dim customerId As Integer = sessionManager.Customer.GetCustomerToMail(PayPalCustomerData.Email)

'            If customerId = 0 Then
'                'neuen Gastaccount anlegen
'                Dim guid As String = System.Guid.NewGuid().ToString
'                If Not 0 = sessionManager.Customer.CreateNewAccount(context, applicationManager, PayPalCustomerData.Email, guid, "", "", Trim(PayPalCustomerData.Firstname & " " & PayPalCustomerData.Lastname), "", PayPalCustomerData.Street, "", PayPalCustomerData.ZipCode, PayPalCustomerData.City, PayPalCustomerData.State, PayPalCustomerData.Country, PayPalCustomerData.Phone, "", True, True, True, sessionManager, "", CustomerType.GuestAccount) Then
'                    'Dim ws As New logicbase.Shop.Frontend.LBShopWebService
'                    'ws.AddMessage("CashingProcedureOverview", New logicbase.Web.MessageObject(logicbase.Web.Enums.Message.MessageTypes.Page, Basics.GetTextResource(applicationManager, sessionManager, 1076), "", logicbase.Web.Enums.Message.SeverityCodes.Error))

'                End If
'            Else
'                'Kunde einloggen
'                sessionManager.Customer.SetCustomerBasics(applicationManager, CType(customerId, Integer), CType(Basics.GetShopProperty(applicationManager, sessionManager, ShopProperty.General_DefaultPriceList), Integer), False)
'            End If
'        End If

'        If sessionManager.Customer.CustomerId > 0 Then
'            'prüfen ob Lieferadresse für Kunden vorhanden und ggf. anlegen
'            Dim shippingAddressId As Integer?
'            Dim addresses As DataTable = sessionManager.Customer.GetAddresses(applicationManager, sessionManager, context, False, True, False)
'            Dim address() As DataRow = addresses.Select("CountryId='" & PayPalCustomerData.Country & "'" &
'                                                        " AND ZipCode='" & PayPalCustomerData.ZipCode & "'" &
'                                                        " AND TRIM(Firstname+' '+LastName)='" & Trim(PayPalCustomerData.Firstname & " " & PayPalCustomerData.Lastname) & "'" &
'                                                        " AND TRIM(Street+' '+HouseNumber)='" & Trim(PayPalCustomerData.Street) & "'")
'            If address.Length > 0 Then
'                shippingAddressId = CType(address(0).Item("AddressId"), Integer)
'            Else
'                shippingAddressId = Nothing
'            End If

'            If shippingAddressId Is Nothing Then
'                shippingAddressId = sessionManager.Customer.AddAddress("", "", Trim(PayPalCustomerData.FirstName & " " & PayPalCustomerData.LastName), "", PayPalCustomerData.Street, "", PayPalCustomerData.ZipCode, PayPalCustomerData.City, "", PayPalCustomerData.Country, PayPalCustomerData.Phone, "", False, True, False, "", False)
'            End If
'            sessionManager.Customer.SaleProcedure.ShippingAddressId = CType(shippingAddressId, Integer)

'            'prüfen ob Rechnungsadresse des Kunden durch Login gesetzt
'            If sessionManager.Customer.SaleProcedure.BillingAddressId = 0 Then
'                Dim billingAddressId As Integer? = sessionManager.Customer.GetBillingAddressId(context)
'                If billingAddressId Is Nothing Then
'                    'eventuell hier eine Meldung anzeigen das eine gültige Rechnungsadresse angelegt werden muss. Dieser Fall sollte bei Shopansprechpartnern nie eintreten, da Rechnungsadresse bei Login erstellt/aktualisiert wird.
'                    billingAddressId = shippingAddressId
'                End If
'                sessionManager.Customer.SaleProcedure.BillingAddressId = CType(billingAddressId, Integer)
'            End If
'        End If
'    End Sub

'    ''' <summary>
'    ''' Setzt die Zahlungskondition PayPal.
'    ''' </summary>
'    ''' <param name="ApplicationManager"></param>
'    ''' <param name="sm">SessionManager</param>
'    ''' <param name="context"></param>
'    ''' <returns>
'    ''' <b>false</b> wenn PaypalExpress nicht möglich
'    ''' <b>true</b> wenn Zahlungskondition PayPal gefunden wurde
'    ''' </returns>
'    ''' <remarks></remarks>
'    Private Shared Function SetPaymentCondition(ByVal ApplicationManager As ApplicationManager, ByVal sm As SessionManager, ByVal context As HttpContext) As Boolean

'        sm.Customer.SaleProcedure.PaymentCondition = PaymentConditions.PAYPALEXPRESS

'        Dim shippingCosts As List(Of ShippingCostsResultItem) = ShippingCostsLibrary.GetCartShippingCosts().Raw

'        Dim selection As ShippingCostsResultItem = ShippingCostsLibrary.GetMappingShippingCosts(
'                            shippingCosts, PaymentConditions.PAYPALEXPRESS,
'                            sm.Customer.SaleProcedure.Forwarder,
'                            sm.Customer.SaleProcedure.DeliveryCondition)


'        If Not selection Is Nothing Then

'            sm.Customer.SaleProcedure.SurchargeId = selection.AdditionId
'            sm.Customer.SaleProcedure.Forwarder = selection.Shipper
'            sm.Customer.SaleProcedure.DeliveryCondition = selection.ShippingType
'            sm.Customer.SaleProcedure.PaymentConditionName = selection.PaymentConditionDescription
'            If sm.Customer.CustomerId = 0 Then sm.Customer.SetCustomerBasics(ApplicationManager, 0, CType(Basics.GetShopProperty(ApplicationManager, sm, ShopProperty.General_DefaultPriceList), Integer), False)
'            sm.ShoppingCart.RefreshAmounts(ApplicationManager, context)

'            Return True
'        Else
'            sm.Customer.SaleProcedure.PaymentCondition = PaymentConditions.EMPTY
'            Return False

'        End If

'    End Function

'    Public Shared Function GetPayPalClassicUrl(ByVal sale As ShopSale) As String

'        Dim url As New StringBuilder()

'        url.Append(Basics.GetShopProperty(ApplicationManager, SessionManager, ShopProperty.PayPal_URL) & "?cmd=_ext-enter&redirect_cmd=_xclick&business=" & HttpUtility.UrlEncode(Basics.GetShopProperty(ApplicationManager, SessionManager, ShopProperty.PayPal_Account)))

'        url.AppendFormat("&custom={0}", HttpUtility.UrlEncode(sale.EMail))
'        url.AppendFormat("&item_number={0}", HttpUtility.UrlEncode(Basics.GetShopProperty(ApplicationManager, SessionManager, ShopProperty.PayPal_CartName)))
'        url.AppendFormat("&amount={0:f2}", Replace(Math.Round((sale.Amounts.ItemsNet + sale.Amounts.ItemsVat - sale.Amounts.Vouchers - sale.Amounts.PaymentConditionDiscountAmount), 2).ToString, ",", "."))
'        url.AppendFormat("&shipping={0:f2}", Replace(Math.Round((sale.Amounts.ForwardingChargesNet + sale.Amounts.ForwardingChargesVat), 2).ToString, ",", "."))
'        url.AppendFormat("&currency_code={0}", sale.CurrencyCode)
'        url.Append("&no_shipping=0")
'        url.AppendFormat("&no_note={0}", Basics.GetShopProperty(ApplicationManager, SessionManager, ShopProperty.PayPal_Comments))
'        url.AppendFormat("&image_url={0}", HttpUtility.UrlEncode(Basics.GetShopProperty(ApplicationManager, SessionManager, ShopProperty.PayPal_Image)))

'        Try
'            url.Append("&address_override=1")
'            url.AppendFormat("&first_name={0}", System.Web.HttpUtility.UrlEncode(sale.ShippingAddress.FirstName))
'            url.AppendFormat("&last_name={0}", System.Web.HttpUtility.UrlEncode(sale.ShippingAddress.LastName))
'            url.AppendFormat("&address1={0}", System.Web.HttpUtility.UrlEncode(Trim(sale.ShippingAddress.Street & " " & sale.ShippingAddress.HouseNumber)))
'            url.AppendFormat("&address2={0}", System.Web.HttpUtility.UrlEncode(sale.ShippingAddress.AdditionalName))
'            url.AppendFormat("&city={0}", System.Web.HttpUtility.UrlEncode(sale.ShippingAddress.City))
'            url.AppendFormat("&shipping_state={0}", System.Web.HttpUtility.UrlEncode(sale.ShippingAddress.State))
'            url.AppendFormat("&zip={0}", System.Web.HttpUtility.UrlEncode(sale.ShippingAddress.ZipCode))
'            url.AppendFormat("&country={0}", System.Web.HttpUtility.UrlEncode(sale.ShippingAddress.Country))

'        Catch
'        End Try

'        Return url.ToString

'    End Function

'    ''' <summary>
'    ''' Liefert aus zu einer PayPal Response Liste ein Shopadressen Objekt
'    ''' </summary>
'    ''' <param name="respList"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Shared Function CreateShopAddress(ByVal respList As System.Collections.Generic.SortedList(Of String, String)) As PayPalAddress

'        Dim mail As String
'        Dim title As String
'        Dim firstname As String
'        Dim lastname As String
'        Dim additionalName As String
'        Dim street As String
'        Dim zipCode As String
'        Dim city As String
'        Dim state As String
'        Dim country As String
'        Dim phone As String

'        mail = respList.Item("EMAIL")
'        If respList.ContainsKey("SALUTATION") Then
'            title = Left(respList.Item("SALUTATION"), 50)
'        Else
'            title = String.Empty
'        End If

'        If respList.ContainsKey("BUSINESS") AndAlso respList("LASTNAME") = respList("BUSINESS") Then
'            firstname = Left(respList("FIRSTNAME"), 50)
'            lastname = Left(respList("LASTNAME"), 50)
'        Else
'            firstname = String.Empty
'            lastname = Left(respList("SHIPTONAME"), 50)
'        End If

'        If respList.ContainsKey("SHIPTOSTREET2") AndAlso Not respList("SHIPTOSTREET2") = "" Then
'            additionalName = Left(respList("SHIPTOSTREET2"), 50)
'            street = Left(respList("SHIPTOSTREET"), 50)
'        Else
'            additionalName = String.Empty
'            street = Left(respList("SHIPTOSTREET"), 50)
'        End If

'        zipCode = Left(respList("SHIPTOZIP"), 10)
'        city = Left(respList("SHIPTOCITY"), 40)

'        If respList.ContainsKey("SHIPTOSTATE") Then
'            state = Left(respList("SHIPTOSTATE"), 50)
'        Else
'            state = String.Empty
'        End If

'        country = respList("SHIPTOCOUNTRYCODE")

'        If respList.ContainsKey("PHONENUM") Then
'            phone = Left(respList("PHONENUM"), 25)
'        Else
'            phone = String.Empty
'        End If

'        Return New PayPalAddress(0, mail, title, firstname, lastname, additionalName, street, "", zipCode, city, state, country, phone)

'    End Function

'    ''' <summary>
'    ''' Liefert das Lieferland für PayPalExpress Kunden
'    ''' </summary>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Shared Function GetShippingCountry() As String

'        If SessionManager.Customer.CustomerId > 0 Then
'            Return SessionManager.Customer.SaleProcedure.ShippingCountry
'        End If

'        If SessionManager.Customer.SaleProcedure.PaymentCondition = PaymentConditions.PAYPALEXPRESS Then
'            If Not SessionManager.Customer.SaleProcedure.PaymentConditionTempData Is Nothing AndAlso SessionManager.Customer.SaleProcedure.PaymentConditionTempData.ContainsKey("CustomerData") Then
'                If Not String.IsNullOrEmpty(CType(SessionManager.Customer.SaleProcedure.PaymentConditionTempData("CustomerData"), PayPalAddress).Country) Then
'                    Return CType(SessionManager.Customer.SaleProcedure.PaymentConditionTempData("CustomerData"), PayPalAddress).Country
'                End If
'            End If
'        End If

'        Return Basics.GetShopProperty(ShopProperty.General_ShopCountry)
'    End Function

'End Class
