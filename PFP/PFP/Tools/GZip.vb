

Imports System.IO
Imports System.IO.Compression

Public Class GZip

    ''' <summary>
    ''' Receives bytes, returns compressed bytes.
    ''' </summary>
    Shared Function Compress(ByVal raw() As Byte) As Byte()

        ' Clean up memory with Using-statements.
        Using memory As MemoryStream = New MemoryStream()
            ' Create compression stream.
            Using gzip As GZipStream = New GZipStream(memory, CompressionMode.Compress, True)
                ' Write.
                gzip.Write(raw, 0, raw.Length)
            End Using
            ' Return array.

            Return memory.ToArray()
        End Using
    End Function

    ''' <summary>
    ''' Receives bytes, returns decompressed bytes.
    ''' </summary>
    Shared Function Inflate(ByVal raw() As Byte) As String

        Dim OutputStr As String = ""

        ' Clean up memory with Using-statements.
        Using memory As MemoryStream = New MemoryStream(raw)
            ' Create compression stream.
            Using gzip As GZipStream = New GZipStream(memory, CompressionMode.Decompress, True)
                'Read
                Using Reader As StreamReader = New StreamReader(gzip)
                    OutputStr = Reader.ReadToEnd
                End Using

            End Using
            ' Return String.

            Return OutputStr
        End Using

    End Function


End Class
