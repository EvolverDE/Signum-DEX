
Option Strict On
Option Explicit On

Imports PFP.ClsINITool

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
        AutoCheckAndFinishAT = 101
        PaymentType = 102
        PaymentInfoText = 103
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
        'PayPal
        PayPalChoice = 300
        PayPalEMail = 301
        PayPalAPIUser = 302
        PayPalAPISecret = 303
        'Temp
        AutoSignalTransactions = 400
        AutoInfoTransactions = 401
        'Develope
        InfoOut = 500
        TCPAPIShowStatus = 501
        DEXNETEnable = 502
        DEXNETShowStatus = 503

    End Enum

    Public Enum E_SettingSection
        Basics = 0
        Defaults = 1
        Filter = 2
        PayPal = 3
        Temp = 4
        Develope = 5
    End Enum


    Function ReloadINI() As Boolean
        INISettings.ReadINI()
        Return True

    End Function

    Sub StopINIClass()
        INISettings.Closer()
    End Sub

    Function InitiateINI() As Boolean

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

        Temp = GetINISetting(E_Setting.AutoSendPaymentInfo, False).ToString
        Temp = GetINISetting(E_Setting.AutoCheckAndFinishAT, False).ToString
        Temp = GetINISetting(E_Setting.PaymentType, "Other").ToString
        Temp = GetINISetting(E_Setting.PaymentInfoText, "self pickup")

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

        Temp = GetINISetting(E_Setting.PayPalChoice, "EMail")
        Temp = GetINISetting(E_Setting.PayPalEMail, "test@test.com")
        Temp = GetINISetting(E_Setting.PayPalAPIUser, "")
        Temp = GetINISetting(E_Setting.PayPalAPISecret, "")

        Temp = GetINISetting(E_Setting.AutoSignalTransactions, "")
        Temp = GetINISetting(E_Setting.AutoInfoTransactions, "")

        Temp = GetINISetting(E_Setting.InfoOut, True).ToString
        Temp = GetINISetting(E_Setting.TCPAPIShowStatus, False).ToString
        Temp = GetINISetting(E_Setting.DEXNETEnable, False).ToString
        Temp = GetINISetting(E_Setting.DEXNETShowStatus, False).ToString

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
            Case Else
                Section = E_SettingSection.Basics
        End Select

        Return Section

    End Function

    'Public Sub EraseINISection(ByVal INIFile As String, ByVal Section As String)
    '    DeletePrivateProfileSection(Section, 0, 0, INIFile)
    'End Sub

    Private Sub INISetValueToFile(ByVal INI As String, ByVal Section As String, ByVal Key As String, ByVal Value As String)

        Dim Result As String = ""
        Result = INISettings.SetValueToSectionKey(Section, Key, Value).ToString

    End Sub

    Private Function INIGetValueFromFile(ByVal File As String, ByVal Section As String, ByVal Key As String, Optional ByVal Def As String = "") As String
        Return INISettings.GetValueFromSectionKey(Section, Key, Def)
    End Function

    Public Sub INISetValue(ByVal File As String, ByVal Section As String, ByVal Key As String, ByVal Value As String)

        Try
            INISetValueToFile(File, Section, Key, Value)
        Catch EXC As Exception

        End Try

    End Sub
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

        Try
            Returner = CBool(INIGetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, DefaultValue.ToString))
        Catch ex As Exception

        End Try

        Return Returner

    End Function

    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As String, Optional ByVal File As String = "/" + "Settings.ini") As Boolean
        Dim Section As E_SettingSection = GetINISection(Setting)
        INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.Trim)
        Return True
    End Function
    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As Integer, Optional ByVal File As String = "/" + "Settings.ini") As Boolean
        Dim Section As E_SettingSection = GetINISection(Setting)
        INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.ToString.Trim)
        Return True
    End Function
    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As Boolean, Optional ByVal File As String = "/" + "Settings.ini") As Boolean
        Dim Section As E_SettingSection = GetINISection(Setting)
        INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.ToString.Trim)
        Return True
    End Function

End Module