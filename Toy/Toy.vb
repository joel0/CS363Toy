﻿Module Toy

    Sub Main()
        Dim args As ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs

        If args.Count < 2 Then
            showUsage()
        End If

        Try
            If args(0).Equals("create", StringComparison.CurrentCultureIgnoreCase) Then
                createTableInteractive(args(1))
            ElseIf args(0).Equals("header") Then
                showHeader(args(1))
            ElseIf args(0).Equals("insert", StringComparison.CurrentCultureIgnoreCase) Then
                insertInteractive(args(1))
            Else
                If args.Count >= 3 Then
                    If args(0).Equals("display", StringComparison.CurrentCultureIgnoreCase) Then
                        Dim rid As Integer
                        If Not Integer.TryParse(args(1), rid) Then
                            Throw New Exception("rid must be an integer")
                        End If
                        displayRecord(args(2), rid)
                    ElseIf args(0).Equals("delete", StringComparison.CurrentCultureIgnoreCase) Then
                        Dim rid As Integer
                        If Not Integer.TryParse(args(1), rid) Then
                            Throw New Exception("rid must be an integer")
                        End If
                        deleteRecord(args(2), rid)
                    ElseIf args(0).Equals("search", StringComparison.CurrentCultureIgnoreCase) Then
                        search(args(2), args(1))
                    Else
                        showUsage()
                    End If
                Else
                    showUsage()
                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub showUsage()
        Console.WriteLine("Usage")
        Console.WriteLine("-----")
        Console.WriteLine()
        Console.WriteLine("toy create file_name")
        Console.WriteLine("toy header file_name")
        Console.WriteLine("toy insert file_name")
        Console.WriteLine("toy display record_id file_name")
        Console.WriteLine("toy delete record_id file_name")
        Console.WriteLine("toy search ""condition"" file_name")
    End Sub

    Private Sub createTableInteractive(fileName As String)
        Dim tb As New Table
        Dim attName As String
        Dim typeStr As String
        Dim type As Column.dataType
        ' Check permissions and syntax
        tb.saveToFile(fileName)

        Do
            Console.Write("Attribute name: ")
            attName = Console.ReadLine()
            Console.Write("Select a type (1. integer; 2. double; 3. boolean; 4. string): ")
            typeStr = Console.ReadLine
            If Not Integer.TryParse(typeStr, type) Then
                Throw New Exception("Type must be an integer")
            End If
            If type < 1 OrElse type > 4 Then
                Throw New Exception("Invalid type")
            End If
            tb.columns.Add(New Column(attName, type))

            Console.Write("More attributes? [y/N] ")
        Loop While Console.ReadLine.Equals("y", StringComparison.CurrentCultureIgnoreCase)
        tb.saveToFile(fileName)
        Console.WriteLine("Created new table")
    End Sub

    Private Sub showHeader(fileName As String)
        Console.WriteLine(Table.getHeaderFromFile(fileName))
    End Sub

    Private Sub insertInteractive(fileName As String)
        Dim tb As Table = Table.readFromFile(fileName)
        Dim newTuple As New Tuple(tb)
        Dim val As String
        For i As Integer = 0 To tb.columns.Count - 1
            Console.Write("{0}: ", tb.columns(i).name)
            val = Console.ReadLine
            newTuple.values.Add(tb.columns(i).valToObj(val))
        Next
        tb.records.Add(newTuple)
        Console.Write("New record added")
        tb.saveToFile(fileName)
    End Sub

    Private Sub displayRecord(fileName As String, rid As Integer)
        Dim tb As Table = Table.readFromFile(fileName)
        If rid >= tb.numberOfRecords Or rid < 0 Then
            Throw New Exception("rid out of bounds")
        End If
        Console.WriteLine(tb.records(rid))
    End Sub

    Private Sub deleteRecord(fileName As String, rid As Integer)
        Dim tb As Table = Table.readFromFile(fileName)
        If rid >= tb.numberOfRecords Or rid < 0 Then
            Throw New Exception("rid out of bounds")
        End If
        tb.records.RemoveAt(rid)
        Console.WriteLine("Record {0} deleted", rid)
        tb.saveToFile(fileName)
    End Sub

    Private Sub search(fileName As String, condition As String)
        Dim tb As Table = Table.readFromFile(fileName)
        Console.Write(tb.search(condition))
    End Sub

End Module
