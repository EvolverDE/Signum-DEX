
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

                                Dim T_List As List(Of String) = T_Vals.Split(",").ToList
                                Dim Key As String = ""

                                For i As Integer = 0 To T_List.Count - 1
                                    Dim TL As String = T_List(i)
                                    If i = T_List.Count - 1 Then
                                        Key = TL.Split(":")(0).Replace("""", "")
                                    Else
                                        U_List.Add(New List(Of Object)(TL.Replace("""", "").Split(":")))
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

                                Dim T_List As List(Of String) = T_Vals.Split(",").ToList
                                Dim Key As String = ""

                                For i As Integer = 0 To T_List.Count - 1
                                    Dim TL As String = T_List(i)
                                    If i = T_List.Count - 1 Then
                                        Key = TL.Split(":")(0).Replace("""", "")
                                    Else
                                        U_List.Add(New List(Of Object)(TL.Replace("""", "").Split(":")))
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
                                Dim T_List As List(Of String) = Input.Split(",").ToList

                                For Each TL As String In T_List
                                    U_List.Add(New List(Of Object)(TL.Replace("""", "").Split(":")))
                                Next

                                Return U_List

                            End If

                        End If
                    End If

                Else
                    If Input.Contains(",") Then
                        U_List.Add(Input.Replace("""", "").Split(",").ToList)
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
                Returner = T_Key_Vals
            ElseIf T_Key_Vals.GetType.Name = GetType(List(Of )).Name Then

                Dim T_List As List(Of Object) = New List(Of Object)

                T_List.AddRange(T_Key_Vals.ToArray)

                If T_List.Count > 2 Then

                    If T_List(0).GetType.Name = GetType(String).Name Then

                        Dim T_Key As String = T_List(0)

                        For i As Integer = 0 To T_List.Count - 1
                            Dim T_Obj As Object = T_List(i)

                            If T_Obj.GetType.Name = GetType(String).Name Then
                                Returner += "<" + i.ToString + ">" + T_Obj + "</" + i.ToString + ">"
                            Else
                                Returner += JSONListToXMLRecursive(T_Obj)
                            End If

                        Next

                    Else
                        Returner += JSONListToXMLRecursive(T_List(0))
                    End If

                Else

                    If T_List(0).GetType.Name = GetType(String).Name Then

                        Dim T_Key As String = T_List(0)
                        Returner += "<" + T_Key.Trim + ">"

                        If T_List.Count = 2 Then
                            If T_List(1).GetType.Name = GetType(String).Name Then
                                Returner += T_List(1)
                            Else
                                Returner += JSONListToXMLRecursive(T_List(1))
                            End If
                        End If

                        Returner += "</" + T_Key.Trim + ">"

                    Else
                        Returner += JSONListToXMLRecursive(T_List(0))
                    End If
                End If

            End If

        Next

        Return Returner

    End Function

    Function RecursiveXMLSearch(Input As String, ByVal Key As String) As String

        If Key.Contains("/") Then

            Dim KeyPathList As List(Of String) = New List(Of String)(Key.Split("/"))

            Dim T_Key As String = KeyPathList(0)
            Dim T_Path As String = Key.Substring(Key.IndexOf("/") + 1)

            If Input.Contains("<" + T_Key + ">") Then
                Return RecursiveXMLSearch(Between(Input, "<" + T_Key + ">", "</" + T_Key + ">", GetType(String)), T_Path)
            Else
                Return ""
            End If

        Else
            If Input.Contains("<" + Key + ">") Then
                Return Between(Input, "<" + Key + ">", "</" + Key + ">", GetType(String))
            Else
                Return ""
            End If
        End If

    End Function

End Class