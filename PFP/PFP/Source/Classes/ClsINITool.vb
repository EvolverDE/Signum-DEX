
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

    Private Property C_Timer As Timer

    Sub New(Optional ByVal File As String = "Settings.ini", Optional ByVal Path As String = "")

        C_Timer = New Timer With {.Enabled = True, .Interval = 50}
        C_Timer.Start()

        AddHandler C_Timer.Tick, AddressOf SerialWriter

        If Path = "" Then
            Path = Application.StartupPath + "/"
        End If

        Me.Path = Path
        Me.File = File
        Dim Wait As Boolean = ReadINI()
    End Sub

    Public Function ReadINI() As Boolean

        Dim SectionKeys As S_Section = New S_Section With {.Section_Name = Nothing}

        If System.IO.File.Exists(Path + File) Then

            Dim T_Entrys As List(Of S_Section) = New List(Of S_Section)

            For Each Line As String In System.IO.File.ReadAllLines(Path + File, Text.Encoding.Default)

                If Line.Contains("[") And Line.Contains("]") Then

                    If Not SectionKeys.Keys Is Nothing Then
                        T_Entrys.Add(SectionKeys)
                    End If

                    SectionKeys = New S_Section
                    SectionKeys.Section_Name = Line.Replace("[", "").Replace("]", "")
                Else

                    Dim T_Key As S_Key = New S_Key
                    T_Key.Key_Name = Line.Remove(Line.IndexOf("=")).ToUpper
                    T_Key.Value = Line.Substring(Line.IndexOf("=") + 1)

                    If Not SectionKeys.Section_Name Is Nothing Then

                        If SectionKeys.Keys Is Nothing Then
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


    Private Function WriteINI() As Boolean
        BusyFlag = True
        Return True
    End Function


    Public Sub Closer()
        Close = True
    End Sub

    Private Property BusyFlag As Boolean = False
    Private Property Close As Boolean = False
    Dim RefreshTimerTicks As Integer = 0


    Sub SerialWriter(ByVal Sender As Object, ByVal E As EventArgs)

        Application.DoEvents()

        RefreshTimerTicks += 1

        If Close Then
            C_Timer.Stop()
            C_Timer.Enabled = False
            Exit Sub
        End If

        If RefreshTimerTicks >= 50 Then

            RefreshTimerTicks = 0

            If BusyFlag Then

                BusyFlag = False

                Dim Lines As List(Of String) = New List(Of String)

                Dim T_Entrys As List(Of S_Section) = New List(Of S_Section)(_INIEntrys.OrderBy(Function(_INIEntrys) _INIEntrys.Section_Name).ToArray)

                For i As Integer = 0 To T_Entrys.Count - 1

                    Dim T_Section As S_Section = T_Entrys(i)

                    Lines.Add("[" + T_Section.Section_Name + "]")

                    If Not T_Section.Keys Is Nothing Then

                        Dim T_KeyList As List(Of S_Key) = New List(Of S_Key)(T_Section.Keys.ToArray)

                        For ii As Integer = 0 To T_KeyList.Count - 1
                            Dim Key As S_Key = T_KeyList(ii)

                            Lines.Add(Key.Key_Name.ToUpper + "=" + Key.Value)
                        Next
                    End If

                Next

                If Lines.Count = 0 Then
                    Lines.Add("[Empty]")
                End If

                C_Timer.Enabled = False
                System.IO.File.WriteAllLines(Path + File, Lines.ToArray, Text.Encoding.Default)
                C_Timer.Enabled = True

            End If


        End If

    End Sub

    Public Function GetValueFromSectionKey(ByVal Section As String, ByVal Key As String, Optional ByVal Def As String = "") As String

        Dim T_Entrys As List(Of S_Section) = New List(Of S_Section)

        T_Entrys.AddRange(_INIEntrys.ToArray)

        For i As Integer = 0 To T_Entrys.Count - 1
            Dim T_Section As S_Section = T_Entrys(i)

            If Not T_Section.Section_Name Is Nothing Then

                If T_Section.Section_Name = Section Then

                    If Not T_Section.Keys Is Nothing Then

                        Dim T_Keys As List(Of S_Key) = New List(Of S_Key)(T_Section.Keys.ToArray)

                        For ii As Integer = 0 To T_Keys.Count - 1

                            Dim T_Key As S_Key = T_Keys(ii)

                            If T_Key.Key_Name = Key.ToUpper Then
                                Return T_Key.Value
                            End If
                        Next

                    End If

                End If

            End If

        Next

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

                    If T_Section.Keys Is Nothing Then
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

            If T_Section.Keys Is Nothing Then
                T_Section.Keys = New List(Of S_Key)
            End If

            T_Section.Keys.Add(T_Key)

            _INIEntrys.Add(T_Section)

        End If

        Return WriteINI()

    End Function

End Class