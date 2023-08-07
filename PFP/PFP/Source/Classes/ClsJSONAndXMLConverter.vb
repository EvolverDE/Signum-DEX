
Option Strict On
Option Explicit On

Imports System.Globalization
Imports System.Runtime.InteropServices.WindowsRuntime
Imports System.Text.RegularExpressions

Public Class ClsJSONAndXMLConverter

#Region "Attributes"
    Public ReadOnly Property ListOfKeyValues As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))
    Public ReadOnly Property XMLString As String = ""
    Public ReadOnly Property JSONString As String = ""
    Private Property Results As List(Of Object) = New List(Of Object)

#End Region

#Region "Enums"
    Enum E_ParseType
        JSON = 0
        XML = 1
    End Enum
    Enum E_KeyVal
        NONE = 0
        Key = 1
        Value = 2
        StartCollection = 3
        EndCollection = 4
        Collection = 5
    End Enum
#End Region

#Region "Structures"
    Structure S_KeyVal
        Dim Value As Object
        Dim Type As E_KeyVal

        Sub New(ByVal Val As Object, Typ As E_KeyVal)
            Value = Val
            Type = Typ
        End Sub
    End Structure
#End Region

#Region "Constructors"

    Sub New(ByVal JSONInput As String, ByVal ParseType As E_ParseType)

        JSONInput = JSONInput.Replace(vbCrLf, "").Replace(vbTab, "")

        Select Case ParseType
            Case E_ParseType.JSON
                Dim Result As Object = ProcessJSONCollections(JSONInput)

                If Result.GetType = GetType(List(Of Object)) Then
                    Dim ObjList As List(Of Object) = DirectCast(Result, List(Of Object))
                    For Each ResultItem As Object In ObjList
                        If ResultItem.GetType = GetType(KeyValuePair(Of String, Object)) Then
                            Dim Entity As KeyValuePair(Of String, Object) = DirectCast(ResultItem, KeyValuePair(Of String, Object))
                            ListOfKeyValues.Add(Entity)
                        End If
                    Next
                End If

                XMLString = ListToXML(ListOfKeyValues)
            Case E_ParseType.XML

                Dim Result As Object = ProcessXMLCollection(JSONInput)


                If Result.GetType = GetType(List(Of Object)) Then
                    Dim ObjList As List(Of Object) = DirectCast(Result, List(Of Object))
                    For Each ResultItem As Object In ObjList
                        If ResultItem.GetType = GetType(KeyValuePair(Of String, Object)) Then
                            Dim Entity As KeyValuePair(Of String, Object) = DirectCast(ResultItem, KeyValuePair(Of String, Object))
                            ListOfKeyValues.Add(Entity)
                        End If
                    Next
                End If

                JSONString = ListToJSON(ListOfKeyValues)
        End Select

    End Sub

    Sub New(ByVal KeyValue As KeyValuePair(Of String, Object))
        ListOfKeyValues.Add(KeyValue)
        JSONString = ListToJSON(ListOfKeyValues)
        XMLString = ListToXML(ListOfKeyValues)
    End Sub

#End Region

#Region "XML"
    Private Function ProcessXMLCollection(ByRef input As String) As Object

        Dim T_Key As String = GetStringBetween(input, "<", ">")
        Dim T_Value As String = GetStringBetween(input, "<" + T_Key + ">", "</" + T_Key + ">")

        Dim Replacer As String = "<" + T_Key + ">" + T_Value + "</" + T_Key + ">"

        If Not T_Value.Contains("<") Then
            input = input.Replace(Replacer, "")
        Else
            input = input.Replace("<" + T_Key + ">", "").Replace("</" + T_Key + ">", "")
        End If

        Dim x As KeyValuePair(Of String, Object)

        If T_Value.Contains("<") Then
            Dim T_Val = T_Value

            Dim T = ProcessXMLCollection(T_Value)

            If T.GetType.Name = GetType(KeyValuePair(Of String, Object)).Name Then
                x = New KeyValuePair(Of String, Object)(T_Key, {T}.ToList())
            Else
                x = New KeyValuePair(Of String, Object)(T_Key, T)
            End If

            input = input.Replace(T_Val, "")

        Else
            x = New KeyValuePair(Of String, Object)(T_Key, GetConvertedType(T_Value))
        End If

        If input.Contains("<") Then
            Dim y = ProcessXMLCollection(input)

            If y.GetType.Name = GetType(String).Name Then
                'error
            ElseIf y.GetType.Name = GetType(KeyValuePair(Of String, Object)).Name Then
                Return {x, y}.ToList()
            Else

                If x.GetType.Name = GetType(String).Name Then
                    'error
                ElseIf x.GetType.Name = GetType(KeyValuePair(Of String, Object)).Name Then
                    Dim z As List(Of Object) = DirectCast(y, List(Of Object))
                    z.Insert(0, x)
                    Return z
                Else
                    Dim Entity As KeyValuePair(Of String, Object) = DirectCast(y, KeyValuePair(Of String, Object))
                    Dim l As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))({x, Entity})
                    Return l
                End If

            End If

        Else
            Return x
        End If

    End Function

#End Region

#Region "JSON"

    Private Function ProcessJSONCollections(ByRef input As String) As Object

        input = input.Trim()

        Select Case input(0)
            Case "{"c
                Dim T_BetweenBraces As List(Of String) = Between2List(input, "{", "}")

                Dim KeyVal As List(Of S_KeyVal) = CollectListOfKeyValStructures(T_BetweenBraces(0))
                input = T_BetweenBraces(0)

                If input = "" And Not T_BetweenBraces(1).Trim() = "" Then
                    input = T_BetweenBraces(1)
                End If

                Dim BraceCollection As List(Of KeyValuePair(Of String, Object)) = ProcessKeyValues(KeyVal)

                If BraceCollection.Count = 1 Then
                    Return BraceCollection(0).Value
                Else
                    Return BraceCollection
                End If

            Case "["c
                Dim T_BetweenBraces As List(Of String) = Between2List(input, "[", "]")

                Dim KeyVal As List(Of S_KeyVal) = CollectListOfKeyValStructures(T_BetweenBraces(0))
                input = T_BetweenBraces(0)

                If input = "" And Not T_BetweenBraces(1).Trim() = "" Then
                    input = T_BetweenBraces(1)
                End If

                Dim BraceCollection As List(Of KeyValuePair(Of String, Object)) = ProcessKeyValues(KeyVal)

                If BraceCollection.Count = 1 Then
                    Return BraceCollection(0).Value
                Else
                    Return BraceCollection
                End If

        End Select

        Return New KeyValuePair(Of String, Object)

    End Function
    Private Function ProcessKeyValues(ByVal Collection As List(Of S_KeyVal)) As List(Of KeyValuePair(Of String, Object))

        Dim BraceCollection As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))

        Dim TempKey As Object = Nothing
        Dim TempCollector As List(Of Object) = New List(Of Object)
        Dim TempCollector2 As List(Of Object) = New List(Of Object)
        Dim i As Integer = 0

        For Each KeyVal As S_KeyVal In Collection

            Select Case KeyVal.Type
                Case E_KeyVal.Key
                    TempKey = KeyVal.Value
                    TempCollector2.Add(KeyVal.Value)
                Case E_KeyVal.StartCollection
                    TempKey = KeyVal.Value
                    'Case E_KeyVal.InCollection
                    '    TempKey = KeyVal.Value
                Case E_KeyVal.EndCollection

                    BraceCollection.Add(New KeyValuePair(Of String, Object)(i.ToString(), TempCollector.ToList()))
                    i += 1
                    TempCollector = New List(Of Object)

                Case E_KeyVal.Value
                    If IsNothing(TempKey) Then
                        TempCollector.Add(New KeyValuePair(Of String, Object)("", KeyVal.Value))
                    Else
                        TempCollector.Add(New KeyValuePair(Of String, Object)(TempKey.ToString(), KeyVal.Value))
                    End If

                Case E_KeyVal.Collection
                    If IsNothing(TempKey) Then
                        TempCollector.Add(New KeyValuePair(Of String, Object)("", KeyVal.Value))
                    Else
                        TempCollector.Add(New KeyValuePair(Of String, Object)(TempKey.ToString(), KeyVal.Value))
                    End If
            End Select

        Next

        If BraceCollection.Count = 0 Then
            If TempCollector.Count = 0 And TempCollector2.Count <> 0 Then
                BraceCollection.Add(New KeyValuePair(Of String, Object)("collection", TempCollector2.ToList()))
            Else
                BraceCollection.Add(New KeyValuePair(Of String, Object)("collection", TempCollector.ToList()))
            End If

        End If

        Return BraceCollection

    End Function
    Private Function CollectListOfKeyValStructures(ByRef input As String) As List(Of S_KeyVal)

        Dim KeyVal As S_KeyVal = GetKeyOrValue(input)
        Dim Collection As List(Of S_KeyVal) = New List(Of S_KeyVal)
        Collection.Add(KeyVal)

        While Not input = ""
            KeyVal = GetKeyOrValue(input)
            If Not KeyVal.Type = E_KeyVal.NONE Then
                Collection.Add(KeyVal)
            End If
        End While

        Return Collection

    End Function
    Private Function GetKeyOrValue(ByRef input As String) As S_KeyVal

        input = input.Trim()

        If input = "" Then
            Return New S_KeyVal("", E_KeyVal.NONE)
        End If

        Select Case input(0)
            Case """"c
                Dim x = GetStringBetween(input, """", """")
                input = input.Substring(input.IndexOf("""" + x + """") + ("""" + x + """").Count)
                Return New S_KeyVal(x, E_KeyVal.Key)
            Case ":"c
                input = input.Substring(1).Trim()

                Select Case input(0)
                    Case """"c

                        If input.Contains("\""") Then
                            input = input.Replace("\""", "\•")
                        End If

                        Dim T_Str As String = GetStringBetween(input, """", """")

                        If T_Str.Contains("\•") Then
                            Dim SearchString As New Regex("\•")
                            Dim Strings As Integer = SearchString.Matches(T_Str).Count + 1
                            T_Str = T_Str.Replace("\•", """")
                            input = input.Substring(input.IndexOf("""" + T_Str + """") + ("""" + T_Str + """").Count + Strings)
                        Else
                            input = input.Substring(input.IndexOf("""" + T_Str + """") + ("""" + T_Str + """").Count)
                        End If

                        Return New S_KeyVal(T_Str, E_KeyVal.Value)

                    Case "{"c
                        Dim x = ProcessJSONCollections(input)
                        Return New S_KeyVal(x, E_KeyVal.Collection)
                    Case "["c
                        Dim x = ProcessJSONCollections(input)
                        Return New S_KeyVal(x, E_KeyVal.Collection)
                    Case Else

                        If input.Trim() = "" Then
                            Return New S_KeyVal("", E_KeyVal.NONE)
                        End If

                        Dim PlaceList As List(Of Integer) = New List(Of Integer)({input.IndexOf("}"), input.IndexOf("]"), input.IndexOf(",")})
                        PlaceList = PlaceList.Where(Function(PL) PL <> -1).ToList()
                        PlaceList.Sort()
                        Dim SplitterPlace As Integer = If(PlaceList.Count = 0, -1, PlaceList(0))

                        If Not SplitterPlace = -1 Then
                            Dim KeyVal As S_KeyVal = New S_KeyVal(input.Remove(SplitterPlace).Trim(), E_KeyVal.Value)
                            input = input.Substring(input.IndexOf(KeyVal.Value.ToString()) + (KeyVal.Value.ToString()).Count)
                            KeyVal.Value = GetConvertedType(KeyVal.Value.ToString())
                            Return KeyVal
                        Else
                            Dim x As String = input
                            input = ""
                            Return New S_KeyVal(x, E_KeyVal.Value)
                        End If

                End Select

            Case ","c
                input = input.Substring(1)
                Return New S_KeyVal(GetKeyOrValue(input).Value, E_KeyVal.Key)
            Case "{"c
                input = input.Substring(1)
                Return New S_KeyVal(GetKeyOrValue(input).Value, E_KeyVal.StartCollection)
            Case "["c
                input = input.Substring(1)
                Return New S_KeyVal(GetKeyOrValue(input).Value, E_KeyVal.StartCollection)
            Case "}"c
                input = input.Substring(1)
                Return New S_KeyVal("", E_KeyVal.EndCollection)
            Case "]"c
                input = input.Substring(1)
                Return New S_KeyVal("", E_KeyVal.EndCollection)
            Case Else ' boolean or numeric balue
                Dim x As String = input
                input = ""
                Return New S_KeyVal(x, E_KeyVal.Value)

        End Select

        Return New S_KeyVal("", E_KeyVal.NONE)

    End Function

#End Region

#Region "Search"

    Public Function AllValues(ByVal SearchPattern As String) As List(Of String)

        Dim KeyVals As List(Of KeyValuePair(Of String, Object)) = Search(Of List(Of KeyValuePair(Of String, Object)))(SearchPattern)

        If KeyVals.Count > 0 Then

            Dim StringList As List(Of String) = New List(Of String)

            For Each x In KeyVals
                StringList.Add(x.Value.ToString())
            Next

            Return StringList

        Else

            While AnyOtherThenT(Of String)(Results)

                If Results.Count > 0 Then

                    Dim SubResult As List(Of Object) = DirectCast(Results(0), List(Of Object))
                    Results = SubResult

                    If Not AnyOtherThenT(Of String)(SubResult) Then
                        Exit While
                    End If

                Else
                    Exit While
                End If

            End While


            Dim StringList As List(Of String) = New List(Of String)

            For Each StringResult In Results
                StringList.Add(StringResult.ToString())
            Next

            Return StringList

        End If

        Return New List(Of String)

    End Function

    Private Function AnyOtherThenT(Of T)(ByVal Input As List(Of Object)) As Boolean

        For Each x As Object In Input

            If x.GetType <> GetType(T) Then
                Return True
            End If

        Next

        Return False

    End Function

    Public Function GetFirstBoolean(ByVal SearchPAttern As String) As Boolean

        Dim Obj As Object = FirstValue(SearchPAttern)
        If Obj.GetType = GetType(Boolean) Then
            Dim Value As Boolean = Convert.ToBoolean(Obj)
            Return Value
        End If

        Return Nothing

    End Function

    Public Function GetFirstDouble(ByVal SearchPAttern As String) As Double

        Dim Obj As Object = FirstValue(SearchPAttern)
        If Obj.GetType = GetType(Double) Then
            Dim DoubleValue As Double = Convert.ToDouble(Obj)
            Return DoubleValue
        End If

        Return 0.0

    End Function

    Public Function GetFirstInteger(ByVal SearchPAttern As String) As Integer

        Dim Obj As Object = FirstValue(SearchPAttern)
        If Obj.GetType = GetType(Integer) Then
            Dim Value As Integer = Convert.ToInt32(Obj)
            Return Value
        End If

        Return -1

    End Function

    Public Function GetFirstULong(ByVal SearchPAttern As String) As ULong

        Dim Obj As Object = FirstValue(SearchPAttern)
        If Obj.GetType = GetType(ULong) Then
            Dim Value As ULong = Convert.ToUInt64(Obj)
            Return Value
        End If

        Return 0UL

    End Function

    Public Function GetFirstLong(ByVal SearchPAttern As String) As Long

        Dim Obj As Object = FirstValue(SearchPAttern)
        If Obj.GetType = GetType(Long) Then
            Dim Value As Long = Convert.ToInt64(Obj)
            Return Value
        End If

        Return 0L

    End Function

    Public Function FirstValue(ByVal SearchPattern As String) As Object

        Dim KeyVals = Search(Of List(Of KeyValuePair(Of String, Object)))(SearchPattern)

        If KeyVals.Count > 0 Then
            Return KeyVals(0).Value
        End If

        Return False

    End Function

    Public Function Search(ByVal SearchPattern As String, ByVal ParseTo As E_ParseType, Optional ByVal GetPath As Boolean = False) As String

        Dim IntermediateResult As List(Of KeyValuePair(Of String, Object)) = Search(Of List(Of KeyValuePair(Of String, Object)))(SearchPattern, GetPath)

        Select Case ParseTo
            Case E_ParseType.JSON
                Return ListToJSON(IntermediateResult)
            Case E_ParseType.XML
                Return ListToXML(IntermediateResult)
        End Select

        Return ""

    End Function

    Public Function GetFromPath(ByVal Path As String) As KeyValuePair(Of String, Object)

        Dim ResultList As List(Of Object) = Search(Path, True)

        For Each Entry As Object In ResultList

            If Entry.GetType = GetType(List(Of Object)) Then
                Dim EntryList As List(Of Object) = DirectCast(Entry, List(Of Object))

                If EntryList.Count > 1 Then
                    If EntryList(1).ToString() = Path Then

                        Dim LastPath As String = Path.Substring(Path.LastIndexOf("/") + 1)

                        If EntryList(0).GetType() = GetType(KeyValuePair(Of String, Object)) Then
                            Dim EndResult As KeyValuePair(Of String, Object) = DirectCast(EntryList(0), KeyValuePair(Of String, Object))
                            Return EndResult
                        Else
                            Dim EntryList1 As List(Of Object) = New List(Of Object)({New KeyValuePair(Of String, Object)("0", EntryList(0))})
                            Dim EndResult As KeyValuePair(Of String, Object) = New KeyValuePair(Of String, Object)(LastPath, EntryList1)
                            Return EndResult
                        End If

                    End If
                End If

            End If

        Next

        Return New KeyValuePair(Of String, Object)

    End Function

    Public Function Search(Of T As List(Of KeyValuePair(Of String, Object)))(ByVal SearchPattern As String, Optional ByVal GetPath As Boolean = False) As T

        Dim TempResult As List(Of Object) = Search(SearchPattern, GetPath)

        Dim EndResult As T = DirectCast(New List(Of KeyValuePair(Of String, Object)), T)

        For i As Integer = 0 To TempResult.Count - 1

            Dim TempEntry As Object = TempResult(i)

            If TempEntry.GetType = GetType(List(Of Object)) Then
                Dim SubEntry As List(Of Object) = DirectCast(TempEntry, List(Of Object))
                If SubEntry.Count > 0 Then
                    TempEntry = SubEntry(0)
                End If
            End If

            Select Case TempEntry.GetType
                Case GetType(KeyValuePair(Of String, Object))

                    Dim EResult As KeyValuePair(Of String, Object) = DirectCast(TempEntry, KeyValuePair(Of String, Object))
                    EndResult.Add(EResult)

                    'Case GetType(List(Of Object))

                    '    TempEntry = TempEntry

                Case GetType(List(Of KeyValuePair(Of String, Object)))
                    Dim EResult As List(Of KeyValuePair(Of String, Object)) = DirectCast(TempEntry, List(Of KeyValuePair(Of String, Object)))
                    EndResult.AddRange(EResult)
                    'Case Else

                    '    TempEntry = TempEntry

            End Select

        Next

        Return EndResult

    End Function
    Public Function Search(ByVal SearchPattern As String, Optional ByVal GetPath As Boolean = False) As List(Of Object)

        Results.Clear()

        If SearchPattern.Contains("/") Then
            Dim Searchlist1 As List(Of String) = SearchPattern.Split("/"c).ToList()

            Dim StartList As List(Of KeyValuePair(Of String, Object)) = ListOfKeyValues.ToList()

            For i As Integer = 0 To Searchlist1.Count - 1
                Dim Path As String = Searchlist1(i)

                Dim SearchObject As Object = SearchList(StartList, "result", Path, GetPath)

                If IsNothing(SearchObject) Then
                    If Results.Count > 0 Then
                        StartList.Clear()

                        Dim EntityList As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))

                        For Each a As Object In Results

                            If a.GetType = GetType(List(Of Object)) Then
                                Dim b As List(Of Object) = DirectCast(a, List(Of Object))

                                Select Case b(0).GetType
                                    Case GetType(KeyValuePair(Of String, Object)), GetType(List(Of Object))
                                        Dim d As KeyValuePair(Of String, Object) = DirectCast(b(0), KeyValuePair(Of String, Object))
                                        EntityList.Add(d)

                                    Case GetType(List(Of KeyValuePair(Of String, Object)))

                                        Dim KeyValueList As List(Of KeyValuePair(Of String, Object)) = DirectCast(b(0), List(Of KeyValuePair(Of String, Object)))
                                        EntityList.AddRange(KeyValueList)

                                End Select

                            End If

                        Next

                        If EntityList.Count > 0 Then
                            StartList.AddRange(EntityList)
                            If i <> Searchlist1.Count - 1 Then
                                Results.Clear()
                            End If

                        End If

                    End If
                    Continue For
                End If

                While SearchObject.GetType() <> GetType(KeyValuePair(Of String, Object))
                    Dim SearchEntityList As List(Of KeyValuePair(Of String, Object)) = DirectCast(SearchObject, List(Of KeyValuePair(Of String, Object)))
                    SearchObject = SearchEntityList.FirstOrDefault().Value
                End While

                Dim SearchEntity As KeyValuePair(Of String, Object) = DirectCast(SearchObject, KeyValuePair(Of String, Object))

                StartList.Clear()
                StartList.Add(SearchEntity)

            Next

            Return Results.ToList()

        Else
            Dim StartList As List(Of KeyValuePair(Of String, Object)) = ListOfKeyValues
            Dim u As Object = SearchList(StartList, "result", SearchPattern, GetPath)

            Return Results.ToList()
        End If

    End Function
    Private Function SearchList(ByVal Input As List(Of KeyValuePair(Of String, Object)), ByVal Path As String, ByVal Search As String, ByVal GetPath As Boolean) As Object

        For Each KeyVal As KeyValuePair(Of String, Object) In Input

            If IsNothing(KeyVal.Key) Then
                Return Nothing
            End If

            Select Case KeyVal.Value.GetType

                Case GetType(String), GetType(Boolean), GetType(Integer), GetType(ULong), GetType(Long)
                    If KeyVal.Key = Search Then

                        Dim Result As List(Of Object) = New List(Of Object)
                        Result.Add(KeyVal)
                        If GetPath Then
                            Result.Add(Path + "/" + KeyVal.Key)
                        End If

                        Results.Add(Result)

                    End If
                Case GetType(Double)
                    If KeyVal.Key = Search Then
                        Dim Result As List(Of Object) = New List(Of Object)
                        Result.Add(KeyVal)
                        If GetPath Then
                            Result.Add(Path + "/" + KeyVal.Key)
                        End If

                        Results.Add(Result)
                    End If
                Case Else

                    Select Case KeyVal.Value.GetType
                        Case GetType(List(Of Object))

                            Dim ObjectList As List(Of Object) = DirectCast(KeyVal.Value, List(Of Object))
                            Dim KeyValueList As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))

                            Dim Collection As List(Of Object) = New List(Of Object)

                            For Each KeyValue As Object In ObjectList

                                Select Case KeyValue.GetType
                                    Case GetType(String)
                                        Collection.Add(KeyValue)
                                    Case Else
                                        Dim Entity As KeyValuePair(Of String, Object) = DirectCast(KeyValue, KeyValuePair(Of String, Object))
                                        KeyValueList.Add(Entity)
                                End Select
                            Next

                            If Collection.Count > 0 Then

                                If KeyVal.Key = Search Then
                                    Dim Result As List(Of Object) = New List(Of Object)
                                    Result.Add(Collection)
                                    If GetPath Then
                                        Result.Add(Path + "/" + KeyVal.Key)
                                    End If

                                    Results.Add(Result)

                                End If

                            Else
                                If KeyVal.Key = Search Then
                                    Dim Result As List(Of Object) = New List(Of Object)

                                    If Collection.Count > 0 Then
                                        Result.Add(Collection)
                                    Else
                                        If KeyValueList.Count > 0 Then
                                            Result.Add(KeyValueList)
                                        End If
                                    End If

                                    If GetPath Then
                                        Result.Add(Path + "/" + KeyVal.Key)
                                    End If

                                    Results.Add(Result)

                                Else
                                    Dim Subsearch As Object = SearchList(KeyValueList, Path + "/" + KeyVal.Key, Search, GetPath)

                                    If Not IsNothing(Subsearch) Then

                                        Dim Result As List(Of Object) = New List(Of Object)
                                        Result.Add(Subsearch)
                                        If GetPath Then
                                            Result.Add(Path + "/" + KeyVal.Key)
                                        End If

                                        Results.Add(Result)
                                    End If

                                End If

                            End If

                        Case GetType(List(Of KeyValuePair(Of String, Object)))
                            Dim KeyValueList As List(Of KeyValuePair(Of String, Object)) = DirectCast(KeyVal.Value, List(Of KeyValuePair(Of String, Object)))

                            If KeyVal.Key = Search Then
                                Dim Result As List(Of Object) = New List(Of Object)
                                Result.Add(KeyVal)
                                If GetPath Then
                                    Result.Add(Path + "/" + KeyVal.Key)
                                End If

                                Results.Add(Result)

                            Else
                                Dim Subsearch As Object = SearchList(KeyValueList, Path + "/" + KeyVal.Key, Search, GetPath)

                                If Not IsNothing(Subsearch) Then

                                    Dim Result As List(Of Object) = New List(Of Object)
                                    Result.Add(Subsearch)
                                    If GetPath Then
                                        Result.Add(Path + "/" + KeyVal.Key)
                                    End If

                                    Results.Add(Result)
                                End If

                            End If

                    End Select

            End Select

        Next

    End Function

#End Region

#Region "Converters"
    Private Function ListToXML(ByVal Input As List(Of KeyValuePair(Of String, Object))) As String

        Dim XMLString As String = ""

        For Each KeyVal As KeyValuePair(Of String, Object) In Input

            XMLString += "<" + KeyVal.Key + ">"

            Select Case KeyVal.Value.GetType

                Case GetType(String), GetType(Boolean)
                    XMLString += KeyVal.Value.ToString()
                Case GetType(Integer), GetType(ULong), GetType(Long)
                    XMLString += String.Format("{0:#0}", KeyVal.Value)
                Case GetType(Double)
                    XMLString += String.Format("{0:#0.00000000##}", KeyVal.Value)
                Case Else

                    Select Case KeyVal.Value.GetType
                        Case GetType(List(Of Object))

                            Dim ObjectList As List(Of Object) = DirectCast(KeyVal.Value, List(Of Object))
                            Dim KeyValueList As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))

                            Dim Collection As List(Of Object) = New List(Of Object)

                            For Each KeyValue As Object In ObjectList

                                Select Case KeyValue.GetType
                                    Case GetType(String)
                                        Collection.Add(KeyValue)
                                    Case Else
                                        Dim Entity As KeyValuePair(Of String, Object) = DirectCast(KeyValue, KeyValuePair(Of String, Object))
                                        KeyValueList.Add(Entity)
                                End Select
                            Next

                            If Collection.Count > 0 Then

                                Dim CollectionString As String = ""
                                For i As Integer = 0 To Collection.Count - 1
                                    Dim coll As String = Collection(i).ToString()
                                    CollectionString += "<" + i.ToString() + ">" + coll + "</" + i.ToString() + ">"
                                Next

                                XMLString += CollectionString
                            Else
                                Dim XMLSubString As String = ListToXML(KeyValueList)
                                XMLString += XMLSubString
                            End If

                        Case GetType(List(Of KeyValuePair(Of String, Object)))
                            Dim KeyValueList As List(Of KeyValuePair(Of String, Object)) = DirectCast(KeyVal.Value, List(Of KeyValuePair(Of String, Object)))
                            Dim XMLSubString As String = ListToXML(KeyValueList)
                            XMLString += XMLSubString
                    End Select

            End Select

            XMLString += "</" + KeyVal.Key + ">"

        Next

        Return XMLString

    End Function
    Private Function ListToJSON(ByVal Input As List(Of KeyValuePair(Of String, Object))) As String

        Dim JSONString As String = "{"

        For Each KeyVal As KeyValuePair(Of String, Object) In Input

            JSONString += """" + KeyVal.Key + """:"

            Select Case KeyVal.Value.GetType

                Case GetType(String)

                    Dim NuKeyVal As String = KeyVal.Value.ToString()

                    If NuKeyVal.Contains("""") Then
                        NuKeyVal = NuKeyVal.Replace("""", "\""")
                    End If

                    If NuKeyVal = "null" Then
                        JSONString += NuKeyVal + ", "
                    Else
                        JSONString += """" + NuKeyVal + """, "
                    End If

                Case GetType(Boolean)
                    JSONString += "" + KeyVal.Value.ToString().ToLower() + ", "
                Case GetType(Integer), GetType(ULong), GetType(Long)
                    JSONString += "" + String.Format("{0:#0}", KeyVal.Value) + ", "
                Case GetType(Double)
                    JSONString += "" + String.Format("{0:#0.00000000##}", KeyVal.Value).Replace(",", ".") + ", "
                Case Else

                    Select Case KeyVal.Value.GetType
                        Case GetType(List(Of Object))

                            Dim ObjectList As List(Of Object) = DirectCast(KeyVal.Value, List(Of Object))
                            Dim KeyValueList As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))

                            Dim Collection As List(Of Object) = New List(Of Object)

                            For Each KeyValue As Object In ObjectList

                                Select Case KeyValue.GetType
                                    Case GetType(String)
                                        Collection.Add(KeyValue)
                                    Case Else
                                        Dim Entity As KeyValuePair(Of String, Object) = DirectCast(KeyValue, KeyValuePair(Of String, Object))
                                        KeyValueList.Add(Entity)
                                End Select
                            Next

                            If Collection.Count > 0 Then

                                Dim CollectionString As String = ""
                                For i As Integer = 0 To Collection.Count - 1
                                    Dim coll As String = Collection(i).ToString()
                                    CollectionString += """" + coll + ""","
                                Next

                                CollectionString = CollectionString.Remove(CollectionString.LastIndexOf(","))

                                JSONString += CollectionString
                            Else
                                Dim JSONSubString As String = ListToJSON(KeyValueList)
                                Dim lastchar As String = JSONString.Trim().Substring(JSONString.Length - 1)
                                If Not lastchar = ":" Then
                                    JSONString += ", " + JSONSubString + ", "
                                Else
                                    JSONString += JSONSubString + ", "
                                End If
                            End If

                        Case GetType(List(Of KeyValuePair(Of String, Object)))
                            Dim KeyValueList As List(Of KeyValuePair(Of String, Object)) = DirectCast(KeyVal.Value, List(Of KeyValuePair(Of String, Object)))
                            Dim JSONSubString As String = ListToJSON(KeyValueList)
                            Dim lastchar As String = JSONString.Trim().Substring(JSONString.Length - 1)

                            If Not lastchar = ":" Then
                                JSONString += ", " + JSONSubString + ", "
                            Else
                                JSONString += JSONSubString + ", "
                            End If

                    End Select

            End Select

        Next

        If JSONString.Trim().Substring(JSONString.Trim().Length - 1) = "," Then
            JSONString = JSONString.Trim()
            JSONString = JSONString.Remove(JSONString.Length - 1)
        End If

        Return JSONString + "}"

    End Function

    Private Function GetConvertedType(ByVal Input As String) As Object

        Dim BoolValue As Boolean = Nothing
        If Boolean.TryParse(Input, BoolValue) Then
            Return BoolValue
        End If

        Dim DoubleValue As Double = Nothing
        If Input.ToString().Contains(".") Or Input.ToString().Contains(",") Then
            Input = Input.Replace(",", ".")
            If Double.TryParse(Input, DoubleValue) Then
                Return Double.Parse(Input, CultureInfo.InvariantCulture)
            End If
        End If

        Dim IntegerValue As Integer = Nothing
        If Integer.TryParse(Input, IntegerValue) Then
            Return IntegerValue
        End If

        Dim ULongValue As ULong = Nothing
        If ULong.TryParse(Input, ULongValue) Then
            Return ULongValue
        End If

        Dim LongValue As Long = Nothing
        If Long.TryParse(Input, LongValue) Then
            Return LongValue
        End If

        Return Input

    End Function

#End Region

End Class
