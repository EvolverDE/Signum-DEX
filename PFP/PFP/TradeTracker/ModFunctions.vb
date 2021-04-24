Module ModFunctions
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DateValueList">Liste mit Datums und dessen Werte</param>
    ''' <returns></returns>
    Public Function DateIntToCandle(ByVal DateValueList As List(Of Object)) As List(Of Graph.S_PieceGraph)

        Dim CandleList As List(Of Graph.S_PieceGraph) = New List(Of Graph.S_PieceGraph)

        For i As Integer = 0 To DateValueList.Count - 1

            Dim Candle As Graph.S_PieceGraph = New Graph.S_PieceGraph

            Dim Dat As Date = DateValueList(i)(0)
            Dim Vali As Double = DateValueList(i)(1)

            Dim High As Double = Vali
            Dim Low As Double = 0.0

            If i = 0 Then
                Candle.OpenDate = Dat
                Candle.CloseDate = Dat

                Candle.OpenValue = Vali
                Candle.CloseValue = Vali

                Candle.MaxValue = Vali
                Candle.MinValue = Vali

            Else
                Candle.OpenValue = CandleList.Item(CandleList.Count - 1).CloseValue
                Candle.CloseDate = Dat

                Candle.OpenDate = CandleList.Item(CandleList.Count - 1).CloseDate
                Candle.CloseValue = Vali

                If DateValueList(i).Count = 4 Then
                    Candle.MaxValue = DateValueList(i)(2)
                    Candle.MinValue = DateValueList(i)(3)
                Else
                    If Candle.OpenValue > Candle.CloseValue Then
                        Candle.MaxValue = Candle.OpenValue
                        Candle.MinValue = Candle.CloseValue
                    Else
                        Candle.MaxValue = Candle.CloseValue
                        Candle.MinValue = Candle.OpenValue
                    End If
                End If

            End If

            CandleList.Add(Candle)

        Next

        Return CandleList

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DateValueList">Liste mit Datums und dessen Werte</param>
    ''' <param name="EMA">die EMA zahl</param>
    ''' <returns></returns>
    Public Function EMAx(ByVal DateValueList As List(Of Object), ByVal EMA As Integer) As List(Of Object)

        Dim EMAList As List(Of Object) = New List(Of Object)

        For i As Integer = 0 To DateValueList.Count - 1

            Dim Dat As Date = DateValueList(i)(0)
            Dim Valu As Double = DateValueList(i)(1)

            Dim EMA_t_m1 As Double = Valu

            If i > 0 Then
                EMA_t_m1 = EMAList(i - 1)(1)
            End If

            Dim SF As Double = 2 / (EMA + 1)

            EMAList.Add(New List(Of Object)({Dat, CDbl((Valu - EMA_t_m1) * SF + EMA_t_m1)}))

        Next

        Return EMAList

    End Function

    Public Function EMAx(ByVal Value As Double, ByVal EMA As Integer, OldEMA As Double) As Double

        Dim EMAList As List(Of Object) = New List(Of Object)

        Dim EMA_t_m1 As Double = OldEMA

        'If i > 0 Then
        '    EMA_t_m1 = EMAList(i - 1)(1)
        'End If

        Dim SF As Double = 2 / (EMA + 1)

        Return CDbl((Value - EMA_t_m1) * SF + EMA_t_m1)

    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="EMA1">Liste des ersten EMA</param>
    ''' <param name="EMA2">Liste des zweiten EMA</param>
    ''' <returns></returns>
    Public Function MACDx(ByVal EMA1 As List(Of Object), ByVal EMA2 As List(Of Object)) As List(Of Object)

        Dim MACDList As List(Of Object) = New List(Of Object)

        Dim ForCnt As Integer = EMA1.Count - 1

        If EMA1.Count < EMA2.Count Then

        Else
            ForCnt = EMA2.Count - 1
        End If


        For i As Integer = 0 To ForCnt

            Dim Dat As Date = EMA1(i)(0)
            Dim EMA1Valu As Double = EMA1(i)(1)
            Dim EMA2Valu As Double = EMA2(i)(1)

            Dim EMASig As Double = EMA1Valu - EMA2Valu

            MACDList.Add(New List(Of Object)({Dat, EMASig}))

        Next


        Return MACDList


        Dim MinVal As Double = Double.MaxValue

        For Each MACDitem As List(Of Object) In MACDList

            Dim MACDVal As Double = MACDitem(1)

            If MACDVal < MinVal Then
                MinVal = MACDVal
            End If

        Next


        If MinVal < 0.0 Then
            MinVal *= -1
        End If

        'Dim Len As Integer = 


        For i As Integer = 0 To MACDList.Count - 1
            Dim XItem As List(Of Object) = MACDList(i)

            XItem(1) = (XItem(1) + MinVal)

            MACDList(i) = XItem

        Next


        Return MACDList ' New List(Of Object)({MACDList, MinVal, MaxVal})

    End Function

    Public Function ZeroLAG_MACD(ByVal EMA1 As List(Of Object), ByVal EMA2 As List(Of Object), Optional EMA1V As Integer = 12, Optional ByVal EMA2V As Integer = 26)
        'ZeroLAG MACD(i) = (2*EMA(Close, FP, i) - EMA(EMA(Close, FP, i), FP, i)) - (2*EMA(Close, SP, i) - EMA(EMA(Close, SP, i), SP, i));

        'ZeroLAG MACD Signal(i) = 2*EMA( ZeroLAG MACD(i), SigP, i) - EMA(EMA( ZeroLAG MACD(i), SigP, i), SigP, i);

        'EMA -Exponential Moving Average
        'Close -Schlusskurs des Balkens
        'FP -Periode für den schnellen EMA
        'SP -Periode für den langsamen EMA
        'SigP -Periode für den Signal-MA.

        Dim ZeroLAGMACD As List(Of Object) = New List(Of Object)

        Dim ForCnt As Integer = EMA1.Count - 1

        If EMA1.Count < EMA2.Count Then

        Else
            ForCnt = EMA2.Count - 1
        End If

        For i As Integer = 0 To ForCnt

            Dim Dat As Date = EMA1(i)(0)

            Dim EMAf As Double = EMA1(i)(1)
            Dim EMAfOld As Double = EMAf

            If i > 0 Then
                EMAfOld = EMA1(i - 1)(1)
            End If

            Dim EMAs As Double = EMA2(i)(1)
            Dim EMAsOld As Double = EMAs

            If i > 0 Then
                EMAsOld = EMA2(i - 1)(1)
            End If



            ZeroLAGMACD.Add(New List(Of Object)({Dat, ((2 * EMAf) - EMAx(EMAf, EMA1V, EMAfOld)) - ((2 * EMAs) - EMAx(EMAs, EMA2V, EMAsOld))}))

        Next

        Return ZeroLAGMACD

    End Function


    Public Function RSIx(ByVal DateValueList As List(Of Graph.S_PieceGraph), ByVal RSIPeriode As Integer) As List(Of Object)

        Dim NewDateValueList As List(Of Object) = New List(Of Object)

        For i As Integer = RSIPeriode To DateValueList.Count - 1

            Dim Dat As Date = DateValueList(i).CloseDate

            Dim Positive As Double = 0.0
            Dim Negative As Double = 0.0

            For j As Integer = i - RSIPeriode To i
                Dim DV As Graph.S_PieceGraph = DateValueList(j)

                If DV.OpenValue > DV.CloseValue Then
                    Negative += DV.OpenValue - DV.CloseValue
                ElseIf DV.OpenValue < DV.CloseValue Then
                    Positive += DV.CloseValue - DV.OpenValue
                End If

            Next

            If Positive = 0.0 And Negative = 0.0 Then
                Positive = RSIPeriode
                Negative = RSIPeriode
            End If

            Positive /= RSIPeriode
            Negative /= RSIPeriode

            Dim RSI As Double = 100 * (Positive / (Positive + Negative))

            NewDateValueList.Add(New List(Of Object)({Dat, RSI}))

        Next

        Return NewDateValueList

    End Function

    'change = change(close)
    'gain = change >= 0 ? change : 0.0
    'loss = change < 0 ? (-1) * change : 0.0
    'avgGain = rma(gain, 14)
    'avgLoss = rma(loss, 14)
    'rs = avgGain / avgLoss
    'rsi = 100 - (100 / (1 + rs))

End Module
