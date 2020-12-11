Module modINITools

    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
    Private Declare Ansi Function DeletePrivateProfileSection Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal Section As String, ByVal NoKey As Integer, ByVal NoSetting As Integer, ByVal FileName As String) As Integer
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer

    Public Sub EraseINISection(ByVal INIFile As String, ByVal Section As String)
        DeletePrivateProfileSection(Section, 0, 0, INIFile)
    End Sub

    Private Structure S_SectionKey

        Dim Section As String
        Dim Key As String

    End Structure

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

    Private Function GetSectionKey(ByVal InSectionKey As Object) As S_SectionKey

        Dim T As S_SectionKey = Nothing

        Try
            Dim ASecKey() As String = InSectionKey.ToString.Trim.Replace(" ", "").Split(New Char() {"_"})
            Dim SectionKey As S_SectionKey
            Dim inSec As String = ASecKey(0)
            Dim inKey As String = ASecKey(1)
            Dim InternSec As String = UCase(inSec.ToString.Trim.Replace(" ", ""))
            Dim InternKey As String = "_" + UCase(inKey.ToString.Trim.Replace(" ", ""))
            SectionKey.Section = InternSec
            If InternKey.StartsWith("_") Then InternKey = InternKey.Replace("_", "")
            SectionKey.Key = InternKey
            T = SectionKey
        Catch EXC As Exception
            T = Nothing
        Finally

        End Try

        Return T

    End Function

    Public Enum E_INISectionKey

        Server_Address = 1
        Server_Port = 2
        Server_Database = 3
        Server_User = 4
        Server_Password = 5

    End Enum

    Public Sub INISetValue(ByVal File As String, ByVal SectionKey As E_INISectionKey, ByVal Value As String)

        Try
            INISetValueToFile(File, GetSectionKey(SectionKey).Section, GetSectionKey(SectionKey).Key, Value)
        Catch EXC As Exception
        Finally

        End Try

    End Sub

    Public Sub INISetValue(ByVal File As String, ByVal Section As String, ByVal Key As String, ByVal Value As String)

        Try
            INISetValueToFile(File, Section, Key, Value)
        Catch EXC As Exception
        Finally

        End Try

    End Sub


    Public Function INIGetValue(ByVal File As String, ByVal SectionKey As E_INISectionKey) As String

        Dim T As String = ""

        Try
            T = INIGetValueFromFile(File, GetSectionKey(SectionKey).Section, GetSectionKey(SectionKey).Key)
        Catch EXC As Exception
            T = ""
        Finally

        End Try

        Return T

    End Function


    Public Function INIGetValue(ByVal File As String, ByVal Section As String, ByVal Key As String, Optional ByVal Def As String = "") As String

        Dim T As String = ""

        Try
            T = INIGetValueFromFile(File, Section.ToUpper, Key.ToUpper, Def)
        Catch EXC As Exception
            T = ""
        Finally

        End Try

        Return T

    End Function


End Module