Imports Newtonsoft.Json
Imports System.IO
Imports System.Net
Imports System.Text

Module modGlobal

    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer

    Public startuppath As String = System.AppDomain.CurrentDomain.BaseDirectory.ToString

    Public Function INISetValue(ByVal INI, ByVal SEC, ByVal KEY, ByVal VAL)

        If INI = "" Then
            INI = startuppath + "config.ini"
        End If

        Dim Result As String = ""
        Result = WritePrivateProfileString(SEC, KEY, VAL, INI)
        Return Result

    End Function

    Public Function INIGetValue(ByVal INI, ByVal SEC, ByVal KEY) As Object

        If INI = "" Then
            INI = startuppath + "config.ini"
        End If

        Dim T As String = ""

        Dim Result As String = ""
        Dim Buffer As String = ""
        Buffer = Space(16384) '16384
        Result = GetPrivateProfileString(SEC, KEY, vbNullString, Buffer, Len(Buffer), INI)
        T = Microsoft.VisualBasic.Strings.Left(Buffer, Result)

        If KEY = "NodeListURL" Then
            If T.Contains(";") Then
                Return Split(T, ";").ToList
            End If
        End If

        If KEY = "PoolListURL" Then
            If T.Contains(";") Then
                Return Split(T, ";").ToList
            End If
        End If

        Return T

    End Function


    ''' <summary>
    ''' Beschreibung
    ''' </summary>
    ''' <param name="postData">gibt die PHP-Parameter für die POST-Methode an</param>
    ''' <param name="Parameter">wenn angegeben, wird das JSON-Element ausgelesen und das Value zurückgegeben</param>
    ''' <returns>Gibt den HTML-Response zurück</returns>
    ''' <remarks></remarks>
    Function BurstRequest(ByVal WalletURL As String, ByVal postData As String, Optional ByVal Parameter As String = "", Optional ByVal brq As String = "/burst")
        Try

            Dim request As WebRequest = WebRequest.Create(WalletURL + brq)
            request.Method = "POST"
            'Dim postData As String = 
            'Console.WriteLine(postData)
            'Console.ReadKey()

            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = byteArray.Length

            Dim TOut As Integer = Val(INIGetValue("", "Connection", "Timeout"))
            If TOut <= 0 Then
                TOut = 1
            End If

            request.Timeout = TOut * 1000

            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)

            dataStream.Close()

            Dim response As WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()

            reader.Close()
            dataStream.Close()
            response.Close()

            If Parameter <> "" Then

                Try
                    Dim res As String = dejson(responseFromServer, Parameter)
                    Return res
                Catch ex As Exception
                    Return "error"
                End Try
            Else
                Return responseFromServer
            End If
        Catch ex As Exception
            Return "error"
        End Try

    End Function

    Function LVColName2SubItem(ByVal LV As ListView, ByVal ColName As String, Optional ByVal LVItem As ListViewItem = Nothing, Optional ByVal index As Integer = -1) As Object

        If index > -1 Then
            LVItem = LV.Items.Item(index)
        End If

        If LVItem Is Nothing Then

            For i As Integer = 0 To LV.Columns.Count - 1
                Dim col As String = LV.Columns.Item(i).Text
                If col = ColName Then
                    Return i
                End If
            Next

        Else

            For i As Integer = 0 To LV.Columns.Count - 1
                Dim col As String = LV.Columns.Item(i).Text
                If col = ColName Then
                    Return LVItem.SubItems.Item(i).Text
                End If
            Next

        End If

        Return -1

    End Function


    Function testAddress(ByVal address As String) As Boolean
        'BURST-XXXX-XXXX-XXXX-XXXXX
        Try

            Dim First As String = address.Remove(address.IndexOf("-"))
            Dim Last As String = address.Substring(address.LastIndexOf("-") + 1)

            Dim dashes As Integer = 0
            For i As Integer = 0 To address.Length - 1
                Dim onechar As String = address.Substring(i, 1)
                If onechar = "-" Then
                    dashes += 1
                End If
            Next

            If First.Length = 5 And First = "BURST" And Last.Length = 5 And dashes = 4 And address.Substring(5, 1) = "-" And address.Substring(10, 1) = "-" And address.Substring(15, 1) = "-" And address.Substring(20, 1) = "-" And address.Length = 26 Then
                Return True
            Else
                Return False
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function merge(ByVal obj As Object) As String

        Dim response As String = ""

        If obj.GetType = GetType(String) Then
            response = obj
        Else
            For Each ari In obj
                If response = "" Then
                    response = ari
                Else
                    response += ";" + ari
                End If
            Next
        End If

        Return response

    End Function


    Public Function SetPropertyValueByName(obj As Object, name As String, value As Object) As Boolean

        Dim prop As Object = obj.GetType().GetProperty(name, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

        If IsNothing(prop) Then
            Return False
        End If

        If obj.GetType = GetType(ListView) And name = "Items" Then

            If value.Item(0) = "Clear" Then
                obj.Items.Clear()
            ElseIf value.Item(0) = "Add" Then
                Dim x = value.Item(1)
                obj.Items.Add(x)
            End If

            Return True
        Else
            If prop.CanWrite Then
                prop.SetValue(obj, value, Nothing)
                Return True
            End If
        End If

        Return False

    End Function


    Function msgsplitter(ByVal input As String) As String
        If input.Contains(";;") Then

            Dim msglist As List(Of String) = New List(Of String)(Split(input, ";;").ToList())

            Dim msgb As String = ""

            For Each msgl In msglist
                msgb += msgl + vbCrLf ' ;; = Carrier Return + Line Feed
            Next
            Return msgb
        Else
            Return input
        End If

    End Function


    Function getLang(ByVal control As Object, Optional ByVal section As iniSections = Nothing, Optional ByVal state As Integer = 0) As String

        If state <> 0 Then
            If control = iniPropStatusStrip.TSSLabOn Then
                control = iniPropStatusStrip.TSSLabOff
            ElseIf control = iniPropStatusStrip.TSSLabSec Then
                control = iniPropStatusStrip.TSSLabUnSec
            ElseIf control = iniPropStatusStrip.WLock Then
                control = iniPropStatusStrip.WUnLock
            End If
        End If

        Dim reture As String = ""

        If section = Nothing Then
            reture = INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", getSection(control), control.ToString)
            reture = msgsplitter(reture)
        Else
            reture = INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", section.ToString, control)
            reture = msgsplitter(reture)
        End If

        Return reture
    End Function

    Function getSection(ByVal obj As Object) As Object

        Select Case obj.GetType
            Case iniPropHomeStrip.iniPropHomeStrip.GetType
                Return iniSections.HomeStrip.ToString
            Case iniPropTabControl.iniPropTabControl.GetType
                Return iniSections.TabControl.ToString
            Case iniPropGrpBxAccInfo.iniPropGrpBxAccInfo.GetType
                Return iniSections.GrpBxAccInfo.ToString
            Case iniPropGrpBxMiningInfo.iniPropGrpBxMiningInfo.GetType
                Return iniSections.GrpBxMiningInfo.ToString
            Case iniPropGrpBxBalancesInfo.iniPropGrpBxBalancesInfo.GetType
                Return iniSections.GrpBxBalancesInfo.ToString
            Case iniPropStatusStrip.iniPropStatusStrip.GetType
                Return iniSections.StatusStrip.ToString
            Case iniPropTransColumns.iniPropTransColumns.GetType
                Return iniSections.TransColumns.ToString

            Case iniMsgBox.iniMsgBox.GetType
                Return iniSections.MsgBox.ToString

            Case iniSettingsMsgBox.iniSettingsMsgBox.GetType
                Return iniSections.SettingsMsgBox.ToString

            Case iniMainMsgBox.iniMainMsgBox.GetType
                Return iniSections.MainMsgBox.ToString
            Case iniSettings.iniSettings.GetType
                Return iniSections.Settings.ToString

            Case iniPropGrpBxSendTo.iniPropGrpBxSendTo.GetType
                Return iniSections.GrpBxSendTo.ToString
            Case iniPropGrpBxUnConfTrans.iniPropGrpBxUnConfTrans.GetType
                Return iniSections.GrpBxUnConfTrans.ToString

            Case iniLVContext.iniPropGrpBxUnConfTrans.GetType
                Return iniSections.LVContext.ToString


            Case Payment.Ordinary_payment.GetType
                Return Types.Payment.ToString
            Case Messaging.Account_info.GetType
                Return Types.Messaging.ToString
            Case Colored_coins.Ask_order_cancellation.GetType
                Return Types.Colored_coins.ToString
            Case Digital_goods.Delisting.GetType
                Return Types.Digital_goods.ToString
            Case Account_Control.Effective_balance_leasing.GetType
                Return Types.Account_Control.ToString

            Case Pool_assignment.Pool_assignment.GetType
                Return Types.Pool_assignment.ToString


            Case Else
                Return "error"

        End Select

    End Function


    Function dejson(ByVal input As String, ByVal suchkritik As String) As String
        Dim dji As Linq.JObject = JsonConvert.DeserializeObject(input)

        Dim find As String = dji.GetValue(suchkritik)

        If IsNothing(find) Then
            find = ""
        End If

        Return find
    End Function

    Function roundup(ByVal val As Single) As Single
        Dim tmp As Single = CSng(Math.Ceiling(val * 10))
        Return tmp / 10
    End Function


    Enum iniSections
        iniSections
        HomeStrip
        TabControl
        GrpBxAccInfo
        GrpBxMiningInfo
        GrpBxBalancesInfo
        StatusStrip
        TransColumns
        LVContext
        MsgBox
        MainMsgBox
        SettingsMsgBox
        Settings
        GrpBxSendTo
        GrpBxUnConfTrans
        Types
    End Enum

    Enum iniPropHomeStrip
        iniPropHomeStrip
        File
        refreshpeers
        unlock
        lock
        exitprog
        SettingsTSMI
        PeerTSMI
        refreshpeer
        autoconnect
        addressTSMI
        refreshaddressinfo
    End Enum

    Enum iniPropTabControl
        iniPropTabControl
        TabOverview
        TabSend
        TabTransacts
    End Enum

    Enum iniPropGrpBxAccInfo
        iniPropGrpBxAccInfo
        Title
        LabNewAcc
        BtAccVerify
        LabName
        BtSetName
        LabNumAcc
        LabPubKey
    End Enum

    Enum iniPropGrpBxMiningInfo
        iniPropGrpBxMiningInfo
        Title
        LabGenBlocks
        LabRewAss
        BtSetRewAss
    End Enum

    Enum iniPropGrpBxBalancesInfo
        iniPropGrpBxBalancesInfo
        Title
        LConBal
        LUnBal
        LGenBal
        LSumBal
    End Enum

    Enum iniPropStatusStrip
        iniPropStatusStrip
        WalletVersion
        WalletPeers
        TSSLabOn
        TSSLabConn
        TSSLabOff
        TSSLabSec
        TSSLabUnSec
        WLock
        WUnLock
    End Enum

    Enum iniPropTransColumns
        iniPropTransColumns
        BlockID
        TransID
        Confirms
        Sender
        Recipient
        Type
        Amount
        Fee
    End Enum

    Enum iniMsgBox
        iniMsgBox
        GenericError
        PeerResponse
    End Enum

    Enum iniMainMsgBox
        iniMainMsgBox
        TimeoutNodeList
        TimeOutPoolList
        GetTransError
        ConnLost
        VerifyInfo
		AccNotExist
    End Enum

    Enum iniSettingsMsgBox
        iniSettingsMsgBox
        URLInfo
        PoolURLInfo
        TypeInfo
        TimeoutInfo
        SaveQuestion
    End Enum

    Enum iniSettings
        iniSettings
        Title
        GrpBxConn
        LabURLPeers
        LabURLPools
        LabType
        LabTimeout
        ChBxOKOnly
    End Enum

    Enum iniPropGrpBxSendTo
        iniPropGrpBxSendTo
        Title
        LabRecipient
        LabAmount
        BtSend
    End Enum

    Enum iniPropGrpBxUnConfTrans
        iniPropGrpBxUnConfTrans
        Title
        LabFee
        LUnConfTrans
        LUnConTransBytes
        LAvgFee
        LBigFee
        LSumFee
        BtGetFeeInfo
    End Enum

    Enum iniLVContext
        iniPropGrpBxUnConfTrans
        Sender
        Recipient
    End Enum

    Function gettyp(ByVal input As String)

        Dim typ As String = input.Remove(input.IndexOf("/"))
        Dim subtyp As String = input.Substring(input.IndexOf("/") + 1)

        Dim typesAr As Array = System.Enum.GetValues(GetType(Types))
        typ = typesAr(CInt(typ)).ToString

        Dim subtypAr As Array

        Select Case typ
            Case "Payment"
                subtypAr = System.Enum.GetValues(GetType(Payment))
            Case "Messaging"
                subtypAr = System.Enum.GetValues(GetType(Messaging))
            Case "Colored_coins"
                subtypAr = System.Enum.GetValues(GetType(Colored_coins))
            Case "Digital_goods"
                subtypAr = System.Enum.GetValues(GetType(Digital_goods))
            Case "Account_Control"
                subtypAr = System.Enum.GetValues(GetType(Account_Control))
            Case "Pool_assignment"
                subtypAr = System.Enum.GetValues(GetType(Pool_assignment))
            Case Else
                Return "error"

        End Select

        subtyp = getLang(subtypAr(CInt(subtyp)))

        Return subtyp
    End Function


    'constantes
    Enum Types
        Payment
        Messaging
        Colored_coins
        Digital_goods
        Account_Control
        none5
        none6
        none7
        none8
        none9
        none10
        none11
        none12
        none13
        none14
        none15
        none16
        none17
        none18
        none19
        Pool_assignment
    End Enum

    Enum Payment
        Ordinary_payment
    End Enum

    Enum Messaging
        Arbitrary_message
        Alias_assignment
        Poll_creation
        Vote_casting
        Hub_terminal_announcement
        Account_info
        Alias_sell
        Alias_buy
    End Enum

    Enum Colored_coins
        Asset_issuance
        Asset_transfer
        Ask_order_placement
        Bid_order_placement
        Ask_order_cancellation
        Bid_order_cancellation
    End Enum

    Enum Digital_goods
        Listing
        Delisting
        Price_change
        Quantity_change
        Purchase
        Delivery
        Feedback
        Refund
    End Enum

    Enum Account_Control
        Effective_balance_leasing
    End Enum

    Enum Pool_assignment
        Pool_assignment
    End Enum

End Module
