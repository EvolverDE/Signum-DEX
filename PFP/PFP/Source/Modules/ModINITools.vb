Module ModINITools

    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
    Private Declare Ansi Function DeletePrivateProfileSection Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal Section As String, ByVal NoKey As Integer, ByVal NoSetting As Integer, ByVal FileName As String) As Integer
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer

    Public Sub EraseINISection(ByVal INIFile As String, ByVal Section As String)
        DeletePrivateProfileSection(Section, 0, 0, INIFile)
    End Sub

    Private Sub INISetValueToFile(ByVal INI, ByVal Section, ByVal Key, ByVal Value)

        Dim Result As String = ""
        Result = WritePrivateProfileString(Section, Key, Value, INI)

    End Sub

    Private Function INIGetValueFromFile(ByVal File As String, ByVal Section As String, ByVal Key As String, Optional ByVal Def As String = "") As String

        Dim T As String = ""

        Dim Result As String = ""
        Dim Buffer As String = ""
        Buffer = Space(16384)
        Result = GetPrivateProfileString(Section, Key, vbNullString, Buffer, Len(Buffer), File)
        T = Left(Buffer, Result)

        If Result = 0 Then
            INISetValue(File, Section, Key, Def)
            T = Def
        End If

        Return T

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
            T = INIGetValueFromFile(File, Section.ToUpper, Key.ToUpper, Def)
        Catch EXC As Exception
            T = ""
        End Try

        Return T

    End Function


    Public Enum E_Setting
        'Basic
        PassPhrase = 0
        LastMarketViewed = 1
        RefreshMinutes = 2
        Nodes = 3
        DefaultNode = 4
        TCPAPIEnable = 5
        TCPAPIServerPort = 6
        DEXNETServerPort = 7
        DEXNETNodes = 8
        DEXNETMyHost = 9
        'Default
        AutoSendPaymentInfo = 100
        AutoCheckAndFinishAT = 101
        PaymentType = 102
        PaymentInfoText = 103
        'PayPal
        PayPalChoice = 200
        PayPalEMail = 201
        PayPalAPIUser = 202
        PayPalAPISecret = 203
        'Temp
        AutoSignalTransactions = 300
        AutoInfoTransactions = 301
        'Develope
        TCPAPIShowStatus = 400
        DEXNETEnable = 401
        DEXNETShowStatus = 402

    End Enum

    Public Enum E_SettingSection
        Basics = 0
        Defaults = 1
        PayPal = 2
        Temp = 3
        Develope = 4
    End Enum


    Function GetINISection(ByVal Setting As E_Setting) As E_SettingSection

        Dim Section As E_SettingSection

        Select Case Setting
            Case 0 To 99
                Section = E_SettingSection.Basics
            Case 100 To 199
                Section = E_SettingSection.Defaults
            Case 200 To 299
                Section = E_SettingSection.PayPal
            Case 300 To 399
                Section = E_SettingSection.Temp
            Case 400 To 499
                Section = E_SettingSection.Develope
            Case Else
                Section = E_SettingSection.Basics
        End Select

        Return Section

    End Function

    Function GetINISetting(ByVal Setting As E_Setting, Optional ByVal DefaultValue As String = "", Optional ByVal File As String = "/Settings.ini") As String
        Dim Section As E_SettingSection = GetINISection(Setting)
        Return INIGetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, DefaultValue)
    End Function
    Function GetINISetting(ByVal Setting As E_Setting, Optional ByVal DefaultValue As Integer = -1, Optional ByVal File As String = "/Settings.ini") As String

        Dim Section As E_SettingSection = GetINISection(Setting)
        Dim Returner As Integer = DefaultValue

        Try
            Returner = CInt(INIGetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, DefaultValue))
        Catch ex As Exception

        End Try

        Return Returner

    End Function
    Function GetINISetting(ByVal Setting As E_Setting, Optional ByVal DefaultValue As Boolean = False, Optional ByVal File As String = "/Settings.ini") As String

        Dim Section As E_SettingSection = GetINISection(Setting)
        Dim Returner As Boolean = DefaultValue

        Try
            Returner = CBool(INIGetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, DefaultValue))
        Catch ex As Exception

        End Try

        Return Returner

    End Function

    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As String, Optional ByVal File As String = "/Settings.ini") As Boolean
        Dim Section As E_SettingSection = GetINISection(Setting)
        INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.Trim)
        Return True
    End Function
    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As Integer, Optional ByVal File As String = "/Settings.ini") As Boolean
        Dim Section As E_SettingSection = GetINISection(Setting)
        INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.ToString.Trim)
        Return True
    End Function
    Function SetINISetting(ByVal Setting As E_Setting, ByVal Value As Boolean, Optional ByVal File As String = "/Settings.ini") As Boolean
        Dim Section As E_SettingSection = GetINISection(Setting)
        INISetValue(Application.StartupPath + File, Section.ToString, Setting.ToString, Value.ToString.Trim)
        Return True
    End Function

End Module