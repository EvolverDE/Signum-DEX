
Option Strict On
Option Explicit On

Public Class ClsJSON
    Private Property RecursiveRest() As List(Of Object) = New List(Of Object)
    Function JSONRecursive(ByVal Input As String) As List(Of Object)

        Input = Input.Trim

        If Input.Length > 0 Then

            Dim U_List As List(Of Object) = New List(Of Object)

            If Input(0) = """" Then

                If Input.Contains(":") Then

                    Dim Prop As String = Input.Remove(Input.IndexOf(":"))
                    Dim Val As String = Input.Substring(Input.IndexOf(":") + 1)

                    If Input.Length > 0 Then

                        Dim CheckBrackets As Boolean = False
                        Dim CheckCurlys As Boolean = False

                        If Val.Length > 2 Then
                            Dim T_Val As String = Val.Substring(0, 3)

                            If T_Val = "[]," Then
                                CheckBrackets = True
                            ElseIf T_Val = "{}," Then
                                CheckCurlys = True
                            End If

                        End If

                        If CheckBrackets Then
                            Dim T_Val As String = Val.Substring(3)

                            U_List.Add(New List(Of Object)({Prop.Replace("""", ""), ""}))
                            U_List.AddRange(JSONRecursive(T_Val).ToArray)

                        ElseIf CheckCurlys Then
                            Dim T_Val As String = Val.Substring(3)

                            U_List.Add(New List(Of Object)({Prop.Replace("""", ""), ""}))
                            U_List.AddRange(JSONRecursive(T_Val).ToArray)

                        ElseIf Val(0) = "{" Then

                            U_List.Add(New List(Of Object)({Prop.Replace("""", ""), JSONRecursive(Val)}))

                            If Not RecursiveRest.Count = 0 Then

                                For Each RekuRest In RecursiveRest
                                    U_List.Add(RekuRest)
                                Next
                                RecursiveRest = New List(Of Object)
                            End If

                        ElseIf Val(0) = "[" Then

                            U_List.Add(New List(Of Object)({Prop.Replace("""", ""), JSONRecursive(Val)}))

                            If Not RecursiveRest.Count = 0 Then

                                For Each RekuRest In RecursiveRest
                                    U_List.Add(RekuRest)
                                Next
                                RecursiveRest = New List(Of Object)
                            End If

                        Else

                            Dim Brackedfound As String = ""

                            For Each Cha In Input
                                Brackedfound = Cha
                                If Brackedfound = "{" Or Brackedfound = "[" Then
                                    Exit For
                                End If

                            Next


                            If Brackedfound = "{" Then

                                Dim T_Vals As String = Input.Remove(Input.IndexOf("{"))
                                Dim T_Rest As String = Input.Substring(Input.IndexOf("{"))

                                Dim T_List As List(Of String) = T_Vals.Split(","c).ToList
                                Dim Key As String = ""

                                For i As Integer = 0 To T_List.Count - 1
                                    Dim TL As String = T_List(i)
                                    If i = T_List.Count - 1 Then
                                        Key = TL.Split(":"c)(0).Replace("""", "")
                                    Else
                                        U_List.Add(New List(Of Object)(TL.Replace("""", "").Split(":"c)))
                                    End If
                                Next


                                Dim EmptyEntry As Boolean = False
                                If T_Rest.Contains("{},") Then
                                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("{},") + 3)
                                    U_List.Add(New List(Of Object)({Key, New List(Of Object)}))
                                    EmptyEntry = True
                                End If

                                If EmptyEntry Then
                                    Dim T_Entrys As List(Of Object) = JSONRecursive(T_Rest)

                                    For Each T_Entry In T_Entrys
                                        U_List.Add(T_Entry)
                                    Next

                                Else
                                    U_List.Add(New List(Of Object)({Key, JSONRecursive(T_Rest)}))
                                End If


                                If Not RecursiveRest.Count = 0 Then

                                    For Each RekuRest In RecursiveRest
                                        U_List.Add(RekuRest)
                                    Next
                                    RecursiveRest = New List(Of Object)
                                End If

                            ElseIf Brackedfound = "[" Then

                                Dim T_Vals As String = Input.Remove(Input.IndexOf("["))
                                Dim T_Rest As String = Input.Substring(Input.IndexOf("["))

                                Dim T_List As List(Of String) = T_Vals.Split(","c).ToList
                                Dim Key As String = ""

                                For i As Integer = 0 To T_List.Count - 1
                                    Dim TL As String = T_List(i)
                                    If i = T_List.Count - 1 Then
                                        Key = TL.Split(":"c)(0).Replace("""", "")
                                    Else
                                        U_List.Add(New List(Of Object)(TL.Replace("""", "").Split(":"c)))
                                    End If
                                Next



                                Dim EmptyEntry As Boolean = False
                                If T_Rest.Contains("[],") Then
                                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("[],") + 3)
                                    U_List.Add(New List(Of Object)({Key, New List(Of Object)}))
                                    EmptyEntry = True
                                End If

                                If EmptyEntry Then
                                    Dim T_Entrys As List(Of Object) = JSONRecursive(T_Rest)

                                    For Each T_Entry In T_Entrys
                                        U_List.Add(T_Entry)
                                    Next

                                Else
                                    U_List.Add(New List(Of Object)({Key, JSONRecursive(T_Rest)}))
                                End If



                                If Not RecursiveRest.Count = 0 Then

                                    For Each RekuRest In RecursiveRest
                                        U_List.Add(RekuRest)
                                    Next
                                    RecursiveRest = New List(Of Object)
                                End If

                            Else
                                Dim T_List As List(Of String) = Input.Split(","c).ToList

                                For Each TL As String In T_List
                                    U_List.Add(New List(Of Object)(TL.Replace("""", "").Split(":"c)))
                                Next

                                Return U_List

                            End If

                        End If
                    End If

                Else
                    If Input.Contains(",") Then
                        U_List.Add(Input.Replace("""", "").Split(","c).ToList)
                    Else
                        U_List.Add(Input.Replace("""", ""))
                    End If

                End If


            ElseIf Input(0) = "{" Then

                Dim Opencurly As Integer = CharCounterList(Input, "{").Count
                Dim Closecurly As Integer = CharCounterList(Input, "}").Count

                If Opencurly < Closecurly Then
                    Input = "{" + Input
                ElseIf Opencurly > Closecurly Then
                    Input += "}"
                End If

                Dim T_Input As List(Of String) = Between2List(Input, "{", "}")

                If T_Input.Count > 0 Then
                    Input = T_Input(0)
                End If

                If Not Input.Trim = "" Then
                    If Input(0) = "," Then
                        Input = Input.Substring(1)
                    End If
                End If

                Dim SubList As List(Of Object) = JSONRecursive(Input)

                For Each SubListItem In SubList
                    U_List.Add(SubListItem)
                Next

                If Not RecursiveRest.Count = 0 Then

                    For Each RekuRest In RecursiveRest
                        U_List.Add(RekuRest)
                    Next
                    RecursiveRest = New List(Of Object)
                End If


                Dim T_Rest As String = T_Input(1)

                If T_Rest.Contains("{},") Then
                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("{},") + 3)

                    Dim RestRek As List(Of Object) = JSONRecursive(T_Rest)

                    For Each Rest In RestRek
                        RecursiveRest.Add(Rest)
                    Next

                End If

            ElseIf Input(0) = "[" Then

                Dim Opensquare As Integer = CharCounterList(Input, "[").Count
                Dim Closesquare As Integer = CharCounterList(Input, "]").Count

                If Opensquare < Closesquare Then
                    Input = "[" + Input
                ElseIf Opensquare > Closesquare Then
                    Input += "]"
                End If

                Dim T_Input As List(Of String) = Between2List(Input, "[", "]")

                If T_Input.Count > 0 Then
                    Input = T_Input(0)
                End If

                If Not Input.Trim = "" Then
                    If Input(0) = "," Then
                        Input = Input.Substring(1)
                    End If
                End If


                Dim SubList As List(Of Object) = JSONRecursive(Input)

                For Each SubListItem In SubList
                    U_List.Add(SubListItem)
                Next


                If Not RecursiveRest.Count = 0 Then

                    For Each RekuRest In RecursiveRest
                        U_List.Add(RekuRest)
                    Next
                    RecursiveRest = New List(Of Object)
                End If

                Dim T_Rest As String = T_Input(1)

                If T_Rest.Contains("[],") Then
                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("[],") + 3)
                    Dim RestRek As List(Of Object) = JSONRecursive(T_Rest)

                    For Each Rest In RestRek
                        RecursiveRest.Add(Rest)
                    Next

                End If

            End If

            Return U_List

        Else
            Return New List(Of Object)
        End If


    End Function
    Function RecursiveListSearch(ByVal List As List(Of Object), ByVal Key As String) As Object

        Dim Returner As Object = False

        Try

            For Each Entry As Object In List

                If Entry.GetType.Name = GetType(String).Name Then

                    Dim Entry1 As String = DirectCast(Entry, String)

                    If Entry1.ToLower.Trim = Key.ToLower.Trim Then

                        Dim FindList As List(Of Object) = New List(Of Object)

                        For i As Integer = 1 To List.Count - 1
                            FindList.Add(List(i))
                        Next

                        If FindList.Count > 1 Then
                            Return FindList
                        Else
                            Return FindList(0)
                        End If

                    End If

                Else

                    Dim Entry1 As List(Of Object) = DirectCast(Entry, List(Of Object))

                    Returner = RecursiveListSearch(Entry1, Key)

                    If Not Returner.GetType.Name = GetType(Boolean).Name Then
                        Return Returner
                    End If

                End If

            Next

        Catch ex As Exception

        End Try

        Return Returner

    End Function

    Function JSONToXML(ByVal Input As String) As String
        Dim JSONList As List(Of Object) = JSONRecursive(Input)
        Dim XMLStr As String = JSONListToXMLRecursive(JSONList)
        Return XMLStr
    End Function
    Function JSONListToXMLRecursive(ByVal JSONList As List(Of Object)) As String

        Dim Returner As String = ""

        For Each T_Key_Vals As Object In JSONList

            If T_Key_Vals.GetType.Name = GetType(String).Name Then
                Returner = DirectCast(T_Key_Vals, String)
            ElseIf T_Key_Vals.GetType.Name = GetType(List(Of )).Name Then

                Dim T_List As List(Of Object) = New List(Of Object)

                T_List.AddRange(DirectCast(T_Key_Vals, List(Of Object)).ToArray)

                If T_List.Count > 2 Then

                    If T_List(0).GetType.Name = GetType(String).Name Then

                        Dim T_Key As String = T_List(0).ToString

                        For i As Integer = 0 To T_List.Count - 1
                            Dim T_Obj As Object = T_List(i)

                            If T_Obj.GetType.Name = GetType(String).Name Then
                                Returner += "<" + i.ToString + ">" + T_Obj.ToString + "</" + i.ToString + ">"
                            Else
                                Returner += JSONListToXMLRecursive(DirectCast(T_Obj, List(Of Object)))
                            End If

                        Next

                    Else
                        Returner += JSONListToXMLRecursive(DirectCast(T_List(0), List(Of Object)))
                    End If

                Else

                    If T_List(0).GetType.Name = GetType(String).Name Then

                        Dim T_Key As String = T_List(0).ToString
                        Returner += "<" + T_Key.Trim + ">"

                        If T_List.Count = 2 Then
                            If T_List(1).GetType.Name = GetType(String).Name Then
                                Returner += T_List(1).ToString
                            Else
                                Returner += JSONListToXMLRecursive(DirectCast(T_List(1), List(Of Object)))
                            End If
                        End If

                        Returner += "</" + T_Key.Trim + ">"

                    Else
                        Returner += JSONListToXMLRecursive(DirectCast(T_List(0), List(Of Object)))
                    End If
                End If

            End If

        Next

        Return Returner

    End Function

    Function RecursiveXMLSearch(Input As String, ByVal Key As String) As String

        If Key.Contains("/") Then

            Dim KeyPathList As List(Of String) = New List(Of String)(Key.Split("/"c))

            Dim T_Key As String = KeyPathList(0)
            Dim T_Path As String = Key.Substring(Key.IndexOf("/") + 1)

            If Input.Contains("<" + T_Key + ">") Then
                Return RecursiveXMLSearch(GetStringBetween(Input, "<" + T_Key + ">", "</" + T_Key + ">"), T_Path)
            Else
                Return ""
            End If

        Else
            If Input.Contains("<" + Key + ">") Then
                Return GetStringBetween(Input, "<" + Key + ">", "</" + Key + ">")
            Else
                Return ""
            End If
        End If

    End Function


    Function GetFromJSON(ByVal JSON As String, ByVal Path As String, Optional ByVal SearchPattern As String = "") As List(Of String)

        If Path.Contains("/") Then

            Dim IDXList As List(Of String) = GetIDXList(JSON)

            Dim JSONList As List(Of Object) = JSONRecursive(JSON)
            Dim XML As String = JSONListToXMLRecursive(JSONList)

            Dim Search As String = ""
            Dim Search_List As List(Of String) = New List(Of String)
            For i As Integer = 0 To IDXList.Count - 1
                Dim T_Path As String = IDXList(i)
                Dim StartIDX As Integer = Convert.ToInt32(T_Path.Substring(T_Path.LastIndexOf("/") + 1))

                If T_Path.Contains("|") Then
                    T_Path = T_Path.Substring(T_Path.IndexOf("|") + 1)
                End If

                T_Path = T_Path.Remove(T_Path.LastIndexOf("/"))

                Dim T_LastPath As String = T_Path.Substring(T_Path.LastIndexOf("/") + 1)

                If T_Path.Contains(Path) Then

                    If SearchPattern.Trim = "" Then
                        Search_List.Add(IDXList(i))
                    Else
                        Dim T_Val As String = GetStringBetween(XML.Substring(StartIDX), "<" + T_LastPath + ">", "</" + T_LastPath + ">")

                        If T_Val.Contains(SearchPattern) Then
                            Search = IDXList(i)
                        End If
                    End If

                End If

            Next

            Dim SearchList As List(Of String) = New List(Of String)
            If SearchPattern.Trim = "" Then
                SearchList = Search_List
            Else
                While Search.Contains("/")

                    For Each idx As String In IDXList

                        If idx.Contains(Search) Then

                            Dim AlreadyIn As Boolean = False

                            For Each T_search As String In SearchList

                                If T_search = idx Then
                                    AlreadyIn = True
                                    Exit For
                                End If

                            Next

                            If Not AlreadyIn Then
                                SearchList.Add(idx)
                            End If

                        End If

                    Next

                    Search = Search.Remove(Search.LastIndexOf("/"))

                End While

            End If

            Dim EndList As List(Of String) = New List(Of String)

            For i As Integer = 0 To SearchList.Count - 1

                Dim T_Search As String = SearchList(i)

                Dim T_IDX As Integer = -1

                If T_Search.Contains("|"c) Then
                    T_IDX = Convert.ToInt32(T_Search.Remove(T_Search.LastIndexOf("|")))
                End If


                Dim T_GetFrom As Integer = Convert.ToInt32(T_Search.Substring(T_Search.LastIndexOf("/") + 1))

                T_Search = T_Search.Remove(T_Search.LastIndexOf("/"))

                Dim T_LastPath As String = T_Search.Substring(T_Search.LastIndexOf("/") + 1)

                Dim T_Val As String = GetStringBetween(XML.Substring(T_GetFrom), "<" + T_LastPath + ">", "</" + T_LastPath + ">")

                If Not T_Val.Contains("<") Then
                    If T_IDX = -1 Then
                        EndList.Add("<0>" + "<" + T_LastPath + ">" + T_Val + "</" + T_LastPath + ">" + "</0>")
                    Else
                        EndList.Add("<" + T_IDX.ToString + ">" + "<" + T_LastPath + ">" + T_Val + "</" + T_LastPath + ">" + "</" + T_IDX.ToString + ">")
                    End If

                End If

            Next

            Return EndList

        Else
            Dim XML As String = JSONToXML(JSON)
            Return New List(Of String)({GetStringBetween(XML, "<" + Path + ">", "</" + Path + ">")})
        End If

    End Function

    Private Function GetIDXList(ByVal JSON As String) As List(Of String)

        Dim JSONList As List(Of Object) = JSONRecursive(JSON)
        Dim XML As String = JSONListToXMLRecursive(JSONList)
        Dim IdxList As List(Of String) = GetPaths(JSONList)
        IdxList = IdxPaths(IdxList)
        IdxList = GetPathIndices(XML, IdxList)


        Dim Before As String = ""

        For i As Integer = 0 To IdxList.Count - 1

            Dim T_IDX As String = IdxList(i)

            If T_IDX.Contains("|") Then
                Before = T_IDX.Remove(T_IDX.IndexOf("|"))
            Else
                If IdxList.Count - 1 > i Then
                    If Before <> "" Then
                        Dim After_IDX As String = IdxList(i + 1)

                        If After_IDX.Contains("|") Then
                            Dim After As String = After_IDX.Remove(After_IDX.IndexOf("|"))

                            If Before = After Then
                                T_IDX = Before + "|" + T_IDX
                                IdxList(i) = T_IDX
                                Before = ""
                            End If

                        End If

                    End If
                End If

            End If

        Next


        Return IdxList

    End Function

    Private Function GetPathIndices(ByVal XML As String, ByVal PathList As List(Of String)) As List(Of String)

        Dim T_List As List(Of String) = New List(Of String)

        Dim T_StartIdx As Integer = 0

        For Each Path As String In PathList

            Dim T_PathList As List(Of String) = New List(Of String)

            If Path.Contains("/") Then
                T_PathList = Path.Split("/"c).ToList
            Else
                T_PathList.Add(Path)
            End If

            If T_PathList.Count > 0 Then
                Dim LastPath As String = T_PathList(T_PathList.Count - 1)

                Dim T_TagStart As String = "<" + LastPath + ">"
                Dim T_TagEnd As String = "</" + LastPath + ">"

                T_StartIdx = XML.IndexOf(T_TagStart, T_StartIdx)

                Dim T_ListStr As String = Path + "/" + T_StartIdx.ToString
                T_List.Add(T_ListStr)

            End If

        Next

        Return T_List

    End Function
    Private Function GetPaths(ByVal JSONList As List(Of Object), Optional ByVal Path As String = "") As List(Of String)

        Dim RetList As List(Of String) = New List(Of String)

        If JSONList.Count > 1 Then

            Dim T_ObjName As Type = JSONList(0).GetType

            If T_ObjName = GetType(String) Then
                Dim Tag As String = DirectCast(JSONList(0), String)

                If Path.Trim = "" Then
                    Path += Tag
                Else
                    Path += "/" + Tag
                End If

                RetList.Add(Path)

                Dim Obj_List As List(Of Object) = TryCast(JSONList(1), List(Of Object))
                Dim Str_List As List(Of String) = TryCast(JSONList(1), List(Of String))

                If Not Obj_List Is Nothing Then

                    For Each T_Obj As Object In Obj_List
                        Dim SubObj_List As List(Of Object) = TryCast(T_Obj, List(Of Object))
                        If Not SubObj_List Is Nothing Then
                            RetList.AddRange(GetPaths(SubObj_List, Path))
                            'Else
                            '    Dim T_ValStr As String = T_Obj.ToString

                            'nix list of obj
                        End If

                    Next

                ElseIf Not Str_List Is Nothing Then
                    Dim T_ValList As List(Of String) = DirectCast(JSONList(1), List(Of String))
                    RetList.Add(T_ValList(0))
                    'Else
                    '    Dim T_ValStr As String = JSONList(1).ToString
                    'RetList.Add(T_ValStr)
                End If

            Else
                Dim T_List As List(Of Object) = TryCast(JSONList(0), List(Of Object))

                If Not T_List Is Nothing Then
                    RetList.AddRange(GetPaths(T_List, Path))
                End If

            End If

        Else

            If JSONList(0).GetType.FullName = GetType(String).FullName Then
                Dim Tag As String = DirectCast(JSONList(0), String)

                If Path.Trim = "" Then
                    Path += Tag
                Else
                    Path += "/" + Tag
                End If

                RetList.Add(Path)

                'Else
                '    Dim T_Str As String = JSONList(0).ToString
                '    RetList.Add(T_Str)
            End If

        End If

        Return RetList

    End Function

    Function IdxPaths(ByVal PathList As List(Of String)) As List(Of String)

        For i As Integer = 0 To PathList.Count - 1

            Dim T_Path As String = PathList(i)

            Dim Dupletten As Integer = 0

            For j As Integer = i + 1 To PathList.Count - 1

                Dim TT_Path As String = PathList(j)

                If T_Path = TT_Path Then
                    Dupletten += 1

                    If Not PathList(i).Contains("0|") Then
                        PathList(i) = "0|" + PathList(i)
                    End If

                    PathList(j) = Dupletten.ToString + "|" + PathList(j)
                End If

            Next

        Next

        Return PathList

    End Function


End Class