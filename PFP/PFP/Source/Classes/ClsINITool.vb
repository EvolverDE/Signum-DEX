

Option Strict On
Option Explicit On

Public Class ClsINITool


    Private _Path As String
    Private _File As String
    Private _INIEntrys As System.Collections.Concurrent.ConcurrentBag(Of S_Section) = New Concurrent.ConcurrentBag(Of S_Section)
    Private Blocked As Boolean = False

    Structure S_Section
        Dim Section_Name As String
        Dim Keys As List(Of S_Key)
    End Structure

    Structure S_Key
        Dim Key_Name As String
        Dim Value As String
    End Structure

    Public Property Path As String
        Get
            Return _Path
        End Get
        Set(value As String)
            _Path = value
        End Set
    End Property

    Public Property File As String
        Get
            Return _File
        End Get
        Set(value As String)
            _File = value
        End Set
    End Property


    Public ReadOnly Property Entrys As List(Of S_Section)
        Get
            While Blocked
                Application.DoEvents()
            End While

            Dim T_EntryList As List(Of S_Section) = New List(Of S_Section)(_INIEntrys.OrderBy(Function(_INIEntrys) _INIEntrys.Section_Name).ToArray)

            Return T_EntryList

        End Get
    End Property

    Sub New(Optional ByVal File As String = "Settings.ini", Optional ByVal Path As String = "")

        If Path = "" Then
            Path = Application.StartupPath + "/"
        End If

        Me.Path = Path
        Me.File = File
        Dim Wait As Boolean = ReadINI()
    End Sub

    Public Function ReadINI() As Boolean

        Dim SectionKeys As S_Section = Nothing

        If System.IO.File.Exists(Path + File) Then

            Dim T_Entrys As List(Of S_Section) = New List(Of S_Section)

            For Each Line As String In System.IO.File.ReadAllLines(Path + File, Text.Encoding.Default)

                If Line.Contains("[") And Line.Contains("]") Then

                    If Not IsNothing(SectionKeys.Keys) Then
                        T_Entrys.Add(SectionKeys)
                    End If

                    SectionKeys = New S_Section
                    SectionKeys.Section_Name = Line.Replace("[", "").Replace("]", "")
                Else

                    Dim T_Key As S_Key = New S_Key
                    T_Key.Key_Name = Line.Remove(Line.IndexOf("=")).ToUpper
                    T_Key.Value = Line.Substring(Line.IndexOf("=") + 1)

                    If Not IsNothing(SectionKeys) Then

                        If IsNothing(SectionKeys.Keys) Then
                            SectionKeys.Keys = New List(Of S_Key)
                        End If

                        SectionKeys.Keys.Add(T_Key)
                    End If

                End If

            Next

            T_Entrys.Add(SectionKeys)

            Blocked = True
            _INIEntrys = ConcurrentBagAddRange(T_Entrys.ToArray)
            Blocked = False

        Else
            Dim Wait As Boolean = WriteINI()
        End If

        Return True

    End Function

    Private Function ConcurrentBagAddRange(ByVal EntryList As Array) As Concurrent.ConcurrentBag(Of S_Section)

        Dim T_CoBag As Concurrent.ConcurrentBag(Of S_Section) = New Concurrent.ConcurrentBag(Of S_Section)

        For Each Entry As S_Section In EntryList
            T_CoBag.Add(Entry)
        Next

        Return T_CoBag

    End Function

    Dim BusyWrite As Boolean = False

    Private Function WriteINI() As Boolean

        While BusyWrite
            Application.DoEvents()
        End While


        Dim Lines As List(Of String) = New List(Of String)

        Dim T_Entrys As List(Of S_Section) = New List(Of S_Section)(_INIEntrys.OrderBy(Function(_INIEntrys) _INIEntrys.Section_Name).ToArray)

        For Each T_Section As S_Section In T_Entrys

            Lines.Add("[" + T_Section.Section_Name + "]")

            If Not IsNothing(T_Section.Keys) Then
                For Each Key As S_Key In T_Section.Keys
                    Lines.Add(Key.Key_Name.ToUpper + "=" + Key.Value)
                Next
            End If

        Next


        If Lines.Count = 0 Then
            Lines.Add("[Empty]")
        End If

        BusyWrite = True

        Try
            System.IO.File.WriteAllLines(Path + File, Lines.ToArray, Text.Encoding.Default)
        Catch ex As Exception

        End Try

        BusyWrite = False

        Return True

    End Function


    Public Function GetValueFromSectionKey(ByVal Section As String, ByVal Key As String, Optional ByVal Def As String = "") As String

        Dim Wait As Boolean = ReadINI()

        For Each T_Section As S_Section In _INIEntrys

            If Not IsNothing(T_Section.Section_Name) Then

                If T_Section.Section_Name = Section Then

                    If Not IsNothing(T_Section.Keys) Then
                        For Each T_Key As S_Key In T_Section.Keys
                            If T_Key.Key_Name = Key.ToUpper Then
                                Return T_Key.Value
                            End If
                        Next
                    End If

                End If

            End If

        Next

        SetValueToSectionKey(Section, Key, Def)

        Return Def

    End Function

    Public Function SetValueToSectionKey(ByVal Section As String, ByVal Key As String, ByVal Value As String) As Boolean

        Key = Key.ToUpper

        Dim FoundSection As Boolean = False

        Dim T_Entrys As List(Of S_Section) = New List(Of S_Section)(_INIEntrys.ToArray)

        For i As Integer = 0 To T_Entrys.Count - 1

            Dim T_Section As S_Section = T_Entrys(i)

            If T_Section.Section_Name = Section Then

                FoundSection = True

                Dim FoundKey As Boolean = False

                For j As Integer = 0 To T_Section.Keys.Count - 1

                    Dim T_Key As S_Key = T_Section.Keys(j)

                    If T_Key.Key_Name = Key Then
                        T_Key.Value = Value

                        T_Section.Keys(j) = T_Key
                        FoundKey = True
                    End If
                Next

                If FoundKey Then
                    Exit For
                Else

                    Dim T_Key As S_Key = New S_Key
                    T_Key.Key_Name = Key
                    T_Key.Value = Value

                    If IsNothing(T_Section.Keys) Then
                        T_Section.Keys = New List(Of S_Key)
                    End If

                    T_Section.Keys.Add(T_Key)

                End If

            End If
        Next

        If Not FoundSection Then

            Dim T_Section As S_Section = New S_Section
            T_Section.Section_Name = Section

            Dim T_Key As S_Key = New S_Key
            T_Key.Key_Name = Key
            T_Key.Value = Value

            If IsNothing(T_Section.Keys) Then
                T_Section.Keys = New List(Of S_Key)
            End If

            T_Section.Keys.Add(T_Key)

            _INIEntrys.Add(T_Section)

        End If

        Return WriteINI()

    End Function

End Class

'Public Class ClsINITool


'    Private _Path As String
'    Private _File As String
'    Private _INIEntrys As List(Of S_Section) = New List(Of S_Section)


'    Structure S_Section
'        Dim Section_Name As String
'        Dim Keys As List(Of S_Key)
'    End Structure

'    Structure S_Key
'        Dim Key_Name As String
'        Dim Value As String
'    End Structure


'    Sub New(Optional ByVal File As String = "Settings.ini", Optional ByVal Path As String = "")

'        If Path = "" Then
'            Path = Application.StartupPath + "/"
'        End If

'        Me.Path = Path
'        Me.File = File
'        ReadINI()
'    End Sub

'    Public Property Path As String
'        Get
'            Return _Path
'        End Get
'        Set(value As String)
'            _Path = value
'        End Set
'    End Property

'    Public Property File As String
'        Get
'            Return _File
'        End Get
'        Set(value As String)
'            _File = value
'        End Set
'    End Property


'    Public ReadOnly Property Entrys As List(Of S_Section)
'        Get
'            Return _INIEntrys
'        End Get
'    End Property


'    Public Function ReadINI() As Boolean

'        Entrys.Clear()

'        Dim SectionKeys As S_Section = Nothing

'        If System.IO.File.Exists(Path + File) Then

'            Dim x As String() = System.IO.File.ReadAllLines(Path + File, Text.Encoding.Default)

'            For Each Line As String In x

'                If Line.Trim = "" Then


'                ElseIf Line.Contains("[") And Line.Contains("]") Then

'                    If Not IsNothing(SectionKeys.Keys) Then
'                        Entrys.Add(SectionKeys)
'                    End If

'                    SectionKeys = New S_Section
'                    SectionKeys.Section_Name = Line.Replace("[", "").Replace("]", "")
'                Else

'                    Dim T_Key As S_Key = New S_Key

'                    T_Key.Key_Name = Line.Remove(Line.IndexOf("="))
'                    T_Key.Value = Line.Substring(Line.IndexOf("=") + 1)

'                    If Not IsNothing(SectionKeys) Then

'                        If IsNothing(SectionKeys.Keys) Then
'                            SectionKeys.Keys = New List(Of S_Key)
'                        End If

'                        SectionKeys.Keys.Add(T_Key)
'                    End If

'                End If

'            Next

'            Entrys.Add(SectionKeys)

'        Else
'            WriteINI()
'        End If

'        Return True

'    End Function

'    Private Function WriteINI() As Boolean

'        Try

'            Dim Lines As List(Of String) = New List(Of String)

'            For Each T_Section As S_Section In Entrys

'                Lines.Add("[" + T_Section.Section_Name + "]")

'                If Not IsNothing(T_Section.Keys) Then
'                    For Each Key As S_Key In T_Section.Keys
'                        Lines.Add(Key.Key_Name + "=" + Key.Value)
'                    Next
'                End If

'            Next

'            If Lines.Count = 0 Then
'                Lines.Add("[Empty]")
'            End If

'            System.IO.File.WriteAllLines(Path + File, Lines.ToArray, Text.Encoding.Default)

'        Catch ex As Exception


'        End Try

'        Return True

'    End Function


'    Public Function GetValueFromSectionKey(ByVal Section As String, ByVal Key As String, Optional ByVal Def As String = "") As String

'        'ReadINI()

'        For Each T_Section As S_Section In Entrys

'            If Not IsNothing(T_Section.Section_Name) Then

'                If T_Section.Section_Name.ToUpper.Trim = Section.ToUpper.Trim Then

'                    If Not IsNothing(T_Section.Keys) Then
'                        For Each T_Key As S_Key In T_Section.Keys
'                            If T_Key.Key_Name.ToUpper.Trim = Key.ToUpper.Trim Then
'                                Return T_Key.Value
'                            End If
'                        Next
'                    End If

'                End If

'            End If

'        Next

'        SetValueToSectionKey(Section, Key, Def)

'        Return Def

'    End Function

'    Public Function SetValueToSectionKey(ByVal Section As String, ByVal Key As String, ByVal Value As String) As Boolean

'        'ReadINI()

'        Dim FoundSection As Boolean = False

'        For i As Integer = 0 To Entrys.Count - 1

'            Dim T_Section As S_Section = Entrys(i)

'            If T_Section.Section_Name.ToUpper.Trim = Section.ToUpper.Trim Then

'                FoundSection = True

'                Dim FoundKey As Boolean = False

'                For j As Integer = 0 To T_Section.Keys.Count - 1

'                    Dim T_Key As S_Key = T_Section.Keys(j)

'                    If T_Key.Key_Name.ToUpper.Trim = Key.ToUpper.Trim Then
'                        T_Key.Value = Value

'                        T_Section.Keys(j) = T_Key
'                        Entrys(i) = T_Section

'                        FoundKey = True
'                    End If
'                Next

'                If FoundKey Then
'                    Exit For
'                Else

'                    Dim T_Key As S_Key = New S_Key
'                    T_Key.Key_Name = Key.ToUpper.Trim
'                    T_Key.Value = Value

'                    If IsNothing(T_Section.Keys) Then
'                        T_Section.Keys = New List(Of S_Key)
'                    End If

'                    T_Section.Keys.Add(T_Key)
'                    Entrys(i) = T_Section
'                End If

'            End If
'        Next

'        If Not FoundSection Then

'            Dim T_Section As S_Section = New S_Section
'            T_Section.Section_Name = Section

'            Dim T_Key As S_Key = New S_Key
'            T_Key.Key_Name = Key.ToUpper.Trim
'            T_Key.Value = Value

'            If IsNothing(T_Section.Keys) Then
'                T_Section.Keys = New List(Of S_Key)
'            End If

'            T_Section.Keys.Add(T_Key)

'            Entrys.Add(T_Section)

'        End If

'        WriteINI()

'        Return True

'    End Function


'End Class