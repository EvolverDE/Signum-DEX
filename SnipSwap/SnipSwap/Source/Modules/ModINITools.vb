
Option Strict On
Option Explicit On

Module ModINITools

    Dim INISettings As ClsINITool = New ClsINITool()

    'Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
    'Private Declare Ansi Function DeletePrivateProfileSection Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal Section As String, ByVal NoKey As Integer, ByVal NoSetting As Integer, ByVal FileName As String) As Integer
    'Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer

    Public Enum E_Setting

        'Basic
        PassPhrase = 0
        PINFingerPrint = 1
        Address = 2
        LastMarketViewed = 3
        RefreshMinutes = 4
        Nodes = 5
        DefaultNode = 6
        TCPAPIEnable = 7
        TCPAPIServerPort = 8
        DEXNETServerPort = 9
        DEXNETNodes = 10
        DEXNETMyHost = 11
        'Default
        AutoSendPaymentInfo = 100
        AutoCheckAndFinishSmartContract = 101
        AllowKnownAccountsAcceptOrdersOverDEXNET = 102
        PaymentType = 103
        PaymentInfoText = 104
        'Filter
        ShowMaxSellOrders = 200
        ShowMaxBuyOrders = 201
        SellFilterAutoinfo = 202
        BuyFilterAutoinfo = 203
        SellFilterAutofinish = 204
        BuyFilterAutofinish = 205
        SellFilterMethods = 206
        BuyFilterMethods = 207
        SellFilterPayable = 208
        BuyFilterPayable = 209
        'SellFilterOffChainOrders = 210
        BuyFilterOffchainOrders = 211
        'PayPal
        PayPalChoice = 300
        PayPalEMail = 301
        PayPalAPIUser = 302
        PayPalAPISecret = 303
        'Temp
        AutoSignalTransactions = 400
        AutoInfoTransactions = 401
        'BlockingTransactions = 402
        BitcoinTransactions = 403
        'Develope
        InfoOut = 500
        TCPAPIShowStatus = 501
        DEXNETEnable = 502
        DEXNETShowStatus = 503
        'Bitcoin
        'BitcoinDPath = 600
        'BitcoinDArguments = 601
        BitcoinAPINode = 602
        BitcoinAPIUser = 603
        BitcoinAPIPassword = 604
        BitcoinWallet = 605
        BitcoinAccounts = 606

    End Enum

    Public Enum E_SettingSection
        Basics = 0
        Defaults = 1
        Filter = 2
        PayPal = 3
        Temp = 4
        Develope = 5
        Bitcoin = 6
    End Enum

    Function ReloadINI() As Boolean
        INISettings.ReadINI()
        Return True

    End Function

    Sub StopINIClass()
        INISettings.Closer()
    End Sub

    Function CreateINI() As Boolean

        If Not IO.File.Exists(Application.StartupPath + "/Settings.ini") Then

#Region "Basics"

            Dim Temp As Boolean = SetINISetting(E_Setting.PassPhrase, "")
            Temp = SetINISetting(E_Setting.PINFingerPrint, "")
            Temp = SetINISetting(E_Setting.Address, "")
            Temp = SetINISetting(E_Setting.LastMarketViewed, "USD")
            Temp = SetINISetting(E_Setting.RefreshMinutes, 1)
            Temp = SetINISetting(E_Setting.Nodes, ClsSignumAPI._Nodes)
            Temp = SetINISetting(E_Setting.DefaultNode, ClsSignumAPI._DefaultNode)
            Temp = SetINISetting(E_Setting.TCPAPIEnable, True)
            Temp = SetINISetting(E_Setting.TCPAPIServerPort, 8130)
            Temp = SetINISetting(E_Setting.DEXNETServerPort, 8131)
            Temp = SetINISetting(E_Setting.DEXNETNodes, "signum.zone:8131")
            Temp = SetINISetting(E_Setting.DEXNETMyHost, "")

#End Region

#Region "Default"

            Temp = SetINISetting(E_Setting.AutoSendPaymentInfo, False)
            Temp = SetINISetting(E_Setting.AutoCheckAndFinishSmartContract, False)
            Temp = SetINISetting(E_Setting.AllowKnownAccountsAcceptOrdersOverDEXNET, False)
            Temp = SetINISetting(E_Setting.PaymentType, "Other")
            Temp = SetINISetting(E_Setting.PaymentInfoText, "self pickup")

#End Region

#Region "Filter"

            Temp = SetINISetting(E_Setting.ShowMaxSellOrders, 10)
            Temp = SetINISetting(E_Setting.ShowMaxBuyOrders, 10)
            Temp = SetINISetting(E_Setting.SellFilterAutoinfo, False)
            Temp = SetINISetting(E_Setting.BuyFilterAutoinfo, False)
            Temp = SetINISetting(E_Setting.SellFilterAutofinish, False)
            Temp = SetINISetting(E_Setting.BuyFilterAutofinish, False)


            Dim DefaultMethodList As List(Of String) = New List(Of String)(ClsOrderSettings.GetPayTypes.ToArray)

            Dim DefaultMethods As String = "Unknown;"
            For Each DefMet As String In DefaultMethodList
                DefaultMethods += DefMet + ";"
            Next

            DefaultMethods = DefaultMethods.Remove(DefaultMethods.Length - 1)

            Temp = SetINISetting(E_Setting.SellFilterMethods, DefaultMethods)
            Temp = SetINISetting(E_Setting.BuyFilterMethods, DefaultMethods)
            Temp = SetINISetting(E_Setting.SellFilterPayable, False)
            Temp = SetINISetting(E_Setting.BuyFilterPayable, False)

#End Region

#Region "PayPal"

            Temp = SetINISetting(E_Setting.PayPalChoice, "EMail")
            Temp = SetINISetting(E_Setting.PayPalEMail, "test@test.com")
            Temp = SetINISetting(E_Setting.PayPalAPIUser, "")
            Temp = SetINISetting(E_Setting.PayPalAPISecret, "")

#End Region

#Region "Temp"

            Temp = SetINISetting(E_Setting.AutoSignalTransactions, "")
            Temp = SetINISetting(E_Setting.AutoInfoTransactions, "")
            'Temp = SetINISetting(E_Setting.BlockingTransactions, "")

#End Region

            Temp = SetINISetting(E_Setting.InfoOut, True)
            Temp = SetINISetting(E_Setting.TCPAPIShowStatus, False)

#Region "Develope"

            Temp = SetINISetting(E_Setting.DEXNETEnable, True)
            Temp = SetINISetting(E_Setting.DEXNETShowStatus, False)

#End Region

#Region "Bitcoin"

            'Temp = SetINISetting(E_Setting.BitcoinDPath, "")
            'Temp = SetINISetting(E_Setting.BitcoinDArguments, "-testnet -rpcuser=bitcoin -rpcpassword=bitcoin -txindex")

            Temp = SetINISetting(E_Setting.BitcoinAPINode, If(GlobalInDevelope, "http://127.0.0.1:18332", "http://127.0.0.1:8332"))
            Temp = SetINISetting(E_Setting.BitcoinAPIUser, "bitcoin")
            Temp = SetINISetting(E_Setting.BitcoinAPIPassword, "bitcoin")
            Temp = SetINISetting(E_Setting.BitcoinWallet, If(GlobalInDevelope, "TESTWALLET", "DEXWALLET"))

            Temp = SetINISetting(E_Setting.BitcoinAccounts, "")

#End Region

            Return True

        Else
            Return False
        End If

    End Function

    Function InitiateINI() As Boolean

#Region "Basics"

        Dim Temp As String = GetINISetting(E_Setting.PassPhrase, "")
        Temp = GetINISetting(E_Setting.PINFingerPrint, "")
        Temp = GetINISetting(E_Setting.Address, "")
        Temp = GetINISetting(E_Setting.LastMarketViewed, "USD")
        Temp = GetINISetting(E_Setting.RefreshMinutes, 1).ToString
        Temp = GetINISetting(E_Setting.Nodes, ClsSignumAPI._Nodes)
        Temp = GetINISetting(E_Setting.DefaultNode, ClsSignumAPI._DefaultNode)
        Temp = GetINISetting(E_Setting.TCPAPIEnable, True).ToString
        Temp = GetINISetting(E_Setting.TCPAPIServerPort, 8130).ToString
        Temp = GetINISetting(E_Setting.DEXNETServerPort, 8131).ToString
        Temp = GetINISetting(E_Setting.DEXNETNodes, "signum.zone:8131")
        Temp = GetINISetting(E_Setting.DEXNETMyHost, "")

#End Region

#Region "Defaults"

        Temp = GetINISetting(E_Setting.AutoSendPaymentInfo, False).ToString
        Temp = GetINISetting(E_Setting.AutoCheckAndFinishSmartContract, False).ToString
        Temp = GetINISetting(E_Setting.AllowKnownAccountsAcceptOrdersOverDEXNET, False).ToString
        Temp = GetINISetting(E_Setting.PaymentType, "Other").ToString
        Temp = GetINISetting(E_Setting.PaymentInfoText, "self pickup")

#End Region

#Region "Filter"

        Temp = GetINISetting(E_Setting.ShowMaxSellOrders, 10).ToString
        Temp = GetINISetting(E_Setting.ShowMaxBuyOrders, 10).ToString
        Temp = GetINISetting(E_Setting.SellFilterAutoinfo, False).ToString
        Temp = GetINISetting(E_Setting.BuyFilterAutoinfo, False).ToString
        Temp = GetINISetting(E_Setting.SellFilterAutofinish, False).ToString
        Temp = GetINISetting(E_Setting.BuyFilterAutofinish, False).ToString

        Dim DefaultMethodList As List(Of String) = New List(Of String)(ClsOrderSettings.GetPayTypes.ToArray)

        Dim DefaultMethods As String = "Unknown;"
        For Each DefMet As String In DefaultMethodList
            DefaultMethods += DefMet + ";"
        Next

        DefaultMethods = DefaultMethods.Remove(DefaultMethods.Length - 1)

        Temp = GetINISetting(E_Setting.SellFilterMethods, DefaultMethods)
        Temp = GetINISetting(E_Setting.BuyFilterMethods, DefaultMethods)
        Temp = GetINISetting(E_Setting.SellFilterPayable, False).ToString
        Temp = GetINISetting(E_Setting.BuyFilterPayable, False).ToString
#End Region

#Region "PayPal"

        Temp = GetINISetting(E_Setting.PayPalChoice, "EMail")
        Temp = GetINISetting(E_Setting.PayPalEMail, "test@test.com")
        Temp = GetINISetting(E_Setting.PayPalAPIUser, "")
        Temp = GetINISetting(E_Setting.PayPalAPISecret, "")

#End Region

#Region "Temp"
        Temp = GetINISetting(E_Setting.AutoSignalTransactions, "")
        Temp = GetINISetting(E_Setting.AutoInfoTransactions, "")
        'Temp = GetINISetting(E_Setting.BlockingTransactions, "")
#End Region

        Temp = GetINISetting(E_Setting.InfoOut, True).ToString
        Temp = GetINISetting(E_Setting.TCPAPIShowStatus, False).ToString

#Region "Develope"

        Temp = GetINISetting(E_Setting.DEXNETEnable, True).ToString
        Temp = GetINISetting(E_Setting.DEXNETShowStatus, False).ToString

#End Region

#Region "Bitcoin"

        'Temp = GetINISetting(E_Setting.BitcoinDPath, "").ToString
        'Temp = GetINISetting(E_Setting.BitcoinDArguments, "-testnet -rpcuser=bitcoin -rpcpassword=bitcoin -txindex").ToString

        Temp = GetINISetting(E_Setting.BitcoinAPINode, "http://127.0.0.1:18332").ToString
        Temp = GetINISetting(E_Setting.BitcoinAPIUser, "bitcoin").ToString
        Temp = GetINISetting(E_Setting.BitcoinAPIPassword, "bitcoin").ToString
        Temp = GetINISetting(E_Setting.BitcoinWallet, "DEXWALLET").ToString

        Temp = GetINISetting(E_Setting.BitcoinAccounts, "").ToString

#End Region

        Return True

    End Function

    Function GetINISection(ByVal Setting As E_Setting) As E_SettingSection

        Dim Section As E_SettingSection

        Dim Int_Setting As Integer = Convert.ToInt32(Setting)

        Select Case Int_Setting
            Case 0 To 99
                Section = E_SettingSection.Basics
            Case 100 To 199
                Section = E_SettingSection.Defaults
            Case 200 To 299
                Section = E_SettingSection.Filter
            Case 300 To 399
                Section = E_SettingSection.PayPal
            Case 400 To 499
                Section = E_SettingSection.Temp
            Case 500 To 599
                Section = E_SettingSection.Develope
            Case 600 To 699
                Section = E_SettingSection.Bitcoin
            Case Else
                Section = E_SettingSection.Basics
        End Select

        Return Section

    End Function

    'Public Sub EraseINISection(ByVal INIFile As String, ByVal Section As String)
    '    DeletePrivateProfileSection(Section, 0, 0, INIFile)
    'End Sub

    Private Function INISetValueToFile(ByVal INI As String, ByVal Section As String, ByVal Key As String, ByVal Value As String) As Boolean
        Return INISettings.SetValueToSectionKey(Section, Key, Value)
    End Function

    Private Function INIGetValueFromFile(ByVal File As String, ByVal Section As String, ByVal Key As String, Optional ByVal Def As String = "") As String
        Return INISettings.GetValueFromSectionKey(Section, Key, Def)
    End Function

    Public Function INISetValue(ByVal File As String, ByVal Section As String, ByVal Key As String, ByVal Value As String) As Boolean

        Try
            Return INISetValueToFile(File, Section, Key, Value)
        Catch EXC As Exception
            Return False
        End Try

    End Function
    Public Function INIGetValue(ByVal File As String, ByVal Section As String, ByVal Key As String, Optional ByVal Def As String = "") As String

        Dim T As String = ""

        Try
            T = INIGetValueFromFile(File, Section, Key, Def)
        Catch EXC As Exception

            'If GetINISetting(E_Setting.InfoOut, False) Then
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in INIGetValue(File=" + File + " Section=" + Section + " Key=" + Key + " Def=" + Def + "): -> " + EXC.ToString)
            'End If

            T = ""
        End Try

        Return T

    End Function

    Function GetINISetting(ByVal Setting As E_Setting, Optional ByVal DefaultValue As String = "", Optional ByVal File As String = "/" + "Settings.ini") As String
        Dim Section As E_SettingSection = GetINISection(Setting)
        Return INIGetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, DefaultValue)
    End Function
    Function GetINISetting(ByVal Setting As E_Setting, Optional ByVal DefaultValue As Integer = -1, Optional ByVal File As String = "/" + "Settings.ini") As Integer

        Dim Section As E_SettingSection = GetINISection(Setting)
        Dim Returner As Integer = DefaultValue

        Try
            Returner = Integer.Parse(INIGetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, DefaultValue.ToString))
        Catch ex As Exception

        End Try

        Return Returner

    End Function
    Function GetINISetting(ByVal Setting As E_Setting, Optional ByVal DefaultValue As Boolean = False, Optional ByVal File As String = "/" + "Settings.ini") As Boolean

        Dim Section As E_SettingSection = GetINISection(Setting)
        Dim Returner As Boolean = DefaultValue

        If GlobalInDevelope Then
            Try
                Returner = CBool(INIGetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, DefaultValue.ToString))
            Catch ex As Exception

            End Try

        Else
            If Section = E_SettingSection.Develope Then

                Select Case Setting
                    Case E_Setting.DEXNETEnable
                        Returner = True

                    Case E_Setting.InfoOut
                        Returner = False

                    Case E_Setting.DEXNETShowStatus
                        Returner = False

                    Case E_Setting.TCPAPIShowStatus
                        Returner = False

                End Select

            Else

                Try
                    Returner = CBool(INIGetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, DefaultValue.ToString))
                Catch ex As Exception

                End Try

            End If
        End If

        Return Returner

    End Function

    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As String, Optional ByVal File As String = "/" + "Settings.ini") As Boolean

        Dim Section As E_SettingSection = GetINISection(Setting)

        If GlobalInDevelope Then
            Return INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.Trim)
        Else
            If Not Section = E_SettingSection.Develope Then
                Return INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.Trim)
            End If
        End If

        Return False

    End Function
    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As Integer, Optional ByVal File As String = "/" + "Settings.ini") As Boolean

        Dim Section As E_SettingSection = GetINISection(Setting)

        If GlobalInDevelope Then
            Return INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.ToString.Trim)
        Else
            If Not Section = E_SettingSection.Develope Then
                Return INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.ToString.Trim)
            End If
        End If

        Return False

    End Function
    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As Boolean, Optional ByVal File As String = "/" + "Settings.ini") As Boolean

        Dim Section As E_SettingSection = GetINISection(Setting)

        If GlobalInDevelope Then
            Return INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.ToString.Trim)
        Else
            If Not Section = E_SettingSection.Develope Then
                Return INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.ToString.Trim)
            End If
        End If

        Return False

    End Function

End Module
