Imports System
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Text

Module Program
    Sub Main(args As String())

        Dim hostName As String = Environment.MachineName

        If Not HttpListener.IsSupported Then
            Console.WriteLine("Unable to start web server. HttpListener class not supported.")
            Return
        End If

        Select Case args.Length
            Case 1

                If args(0).ToLower = "listnics" Then
                    ListNics()
                    Return
                End If

            Case 2
                If args(0).ToLower = "listen" Then
                    Dim netInterface As String = args(1)

                    Dim listener As New HttpListener

                    'This is where we tell the HttpListener what interface
                    'and port to listen on
                    listener.Prefixes.Add($"http://{netInterface}/")

                    Try
                        'Start listenting for HTTP requests
                        listener.Start()
                    Catch ex As Exception

                        Console.WriteLine($"Error trying to start server : {ex.Message}")

                        Return
                    End Try

                    Console.WriteLine($"Listening for HTTP requests at {netInterface}")
                    Console.WriteLine("Press any key to exit.")

                    'Start the web server on another thread
                    Threading.ThreadPool.QueueUserWorkItem(Sub()



                                                               Do
                                                                   Dim context = listener.GetContext
                                                                   Dim response As HttpListenerResponse = context.Response
                                                                   Dim request = context.Request
                                                                   Dim savegamehandled As Boolean = False

                                                                   If request.RawUrl.ToLower.StartsWith("/savegame/") Then
                                                                       Dim SavegameCheckerObj As New SavegameChecker
                                                                       savegamehandled = True
                                                                       Dim farmhandled As Boolean = False
                                                                       ' Redirect to savegame specific page
                                                                       Dim savegamenum As String = request.RawUrl.ToLower.Split("/savegame/")(1).Split("/")(0)
                                                                       Dim savegamerequest As String = request.RawUrl.ToLower.Split("/savegame/")(1).Substring(1)
                                                                       savegamerequest = savegamerequest.Substring(savegamerequest.IndexOf("/"))

                                                                       If savegamerequest.StartsWith("/farm/") Then
                                                                           farmhandled = True

                                                                           Dim farmnum As String = request.RawUrl.ToLower.Split("/farm/")(1).Split("/")(0)
                                                                           Dim farmrequest As String = request.RawUrl.ToLower.Split("/farm/")(1).Substring(1)
                                                                           farmrequest = farmrequest.Substring(farmrequest.IndexOf("/"))

                                                                           Select Case farmrequest
                                                                               Case "/dashboard"
                                                                                   ' Dashboard farm page
                                                                                   ' ******************

                                                                                   WriteWebPageResponse(FarmDashboard(farmnum), response)

                                                                               Case Else
                                                                                   '404 Error. This happens when a request is made for a page
                                                                                   'or resource from a path that doesn't exist.
                                                                                   '******************************************


                                                                                   response.StatusCode = HttpStatusCode.NotFound
                                                                                   WriteWebPageResponse(Get404HTML(), response)


                                                                           End Select
                                                                       End If
                                                                       If SavegameCheckerObj.CheckSaveGame(savegamenum) = False Then
                                                                           savegamerequest = "" ' blanks request to force 404
                                                                       End If

                                                                       Select Case savegamerequest
                                                                           Case "/dashboard"
                                                                               ' Dashboard savegame page
                                                                               '*************************

                                                                               WriteWebPageResponse(SavegameDashboard(savegamenum), response)

                                                                           Case Else
                                                                               '404 Error. This happens when a request is made for a page
                                                                               'or resource from a path that doesn't exist.
                                                                               '******************************************

                                                                               If farmhandled = False Then
                                                                                   response.StatusCode = HttpStatusCode.NotFound
                                                                                   WriteWebPageResponse(Get404HTML(), response)
                                                                               End If


                                                                       End Select
                                                                       End If

                                                                       Select Case request.RawUrl.ToLower ' select case used for premade sites


                                                                       'Redirect to the home page
                                                                       Case "/"
                                                                           response.Redirect("/home")


                                                                       Case "/home"
                                                                           'Home page
                                                                           '******************************************

                                                                           WriteWebPageResponse(GetHomePage(hostName), response)

                                                                       Case "/img"
                                                                           'This path is used for the favicon.
                                                                           'This is the icon that shows up in the left side of
                                                                           'a web page's browser tab in the web browser.
                                                                           '******************************************

                                                                           WriteImageResponse(My.Resources.LocalImages.favicon, response)


                                                                       Case Else
                                                                           '404 Error. This happens when a request is made for a page
                                                                           'or resource from a path that doesn't exist.
                                                                           '******************************************

                                                                           If savegamehandled = False Then ' prevent 404 on savegame addresses
                                                                               response.StatusCode = HttpStatusCode.NotFound
                                                                               WriteWebPageResponse(Get404HTML(), response)
                                                                           End If


                                                                   End Select

                                                                   'Send reponse to web browser by closing the output stream
                                                                   response.OutputStream.Close()
                                                               Loop
                                                           End Sub)

                    Console.ReadKey()

                    Return
                End If

        End Select

        'Generates help for using the web server's command line interface
        '***********************************************************************
        Console.WriteLine("Usage:")
        Console.WriteLine("Eg 1: SimpleWebServer listen 192.168.100.27:5050")
        Console.WriteLine("Listens on interface 192.168.100.27, port 5050 for HTTP requests.")
        Console.WriteLine()

        Console.WriteLine("Eg 2: SimpleWebServer listen localhost:5050")
        Console.WriteLine("Listens on whatever interface localhost resolves to which is usually 127.0.0.1, and port 5050 for HTTP requests.")
        Console.WriteLine()

        Console.WriteLine("Eg 3: SimpleWebServer listnics")
        Console.WriteLine("Lists all network interfaces and their IPv4 addresses.")
    End Sub


    Private Sub WriteWebPageResponse(ByVal text As String, ByVal r As HttpListenerResponse)

        Dim data As Byte() = System.Text.Encoding.UTF8.GetBytes(text)

        r.ContentLength64 = data.Length
        r.OutputStream.Write(data, 0, data.Length)

    End Sub

    Private Sub WriteImageResponse(ByVal img As Byte(), ByVal r As HttpListenerResponse)

        'This is how web servers send images to web browsers
        '*********************************************************
        r.ContentType = "image/png"
        r.ContentLength64 = img.Length
        r.OutputStream.Write(img, 0, img.Length)

    End Sub

    Private Function GetFaviconHTML() As String
        'This is how you give a web page a favicon
        '*********************************************************
        Dim sb As New StringBuilder
        sb.AppendLine("<head>")
        sb.AppendLine("<link rel=""icon"" href=""/img""")
        sb.AppendLine("<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"" integrity=""sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3"" crossorigin=""anonymous"">")
        sb.AppendLine("<script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.min.js"" integrity=""sha384-QJHtvGhmr9XOIpI6YVutG+2QOK9T+ZnN4kzFN1RtK3zEFEIsxhlmWl5/YESvpZ13"" crossorigin=""anonymous""></script>")
        sb.AppendLine("<link rel=""stylesheet"" href=""https://getbootstrap.com/docs/5.1/examples/dashboard/dashboard.css"">")
        sb.AppendLine("</head>")

        Return sb.ToString
    End Function

    Private Function GetHomePage(ByVal hostName As String) As String
        'This function generates the HTML for the home page.
        '*********************************************************

        Dim sb As New StringBuilder

        sb.AppendLine("<HTML>")

        sb.AppendLine(GetFaviconHTML())

        sb.AppendLine(GetLinksHTML())

        sb.AppendLine($"<p>Hello from {hostName}!</p>")

        Dim SavegameCheckerObj As New SavegameChecker
        Dim FS22FolderExists As Boolean = SavegameCheckerObj.CheckFolder()

        If FS22FolderExists = True Then
            Dim FS22SavegameFolders As List(Of Boolean) = SavegameCheckerObj.ReturnSavegames()

            Dim count As Integer = 1
            For Each savegame As Boolean In FS22SavegameFolders
                If savegame = True Then
                    Dim saveGameInfo As List(Of String) = SavegameCheckerObj.FetchSavegameInfo(count)
                    sb.AppendLine($"<p><a href=""/savegame/{count}/dashboard"">(Savegame{count}) {saveGameInfo(0)} - Last played {saveGameInfo(1)} on map {saveGameInfo(2)} located.</a></p>")
                End If
                count += 1
            Next
        Else
            sb.AppendLine("<p>The FS22 Directory has not been located.</p>")
        End If



        sb.AppendLine("</HTML>")

        Return sb.ToString
    End Function

    Private Function SavegameDashboard(ByVal savegamenum As Integer) As String
        Dim sb As New StringBuilder

        sb.AppendLine("<HTML>")

        sb.AppendLine(GetFaviconHTML())

        sb.AppendLine(GetLinksHTML())

        Dim FarmReaderObj As New FarmReader
        Dim FarmNames As List(Of List(Of String)) = FarmReaderObj.GetFarms(savegamenum)

        For Each farm In FarmNames
            Dim roundedbal As Integer = CType(farm(2), Integer)
            Dim roundedloan As Integer = CType(farm(3), Integer)
            sb.AppendLine($"<p><a href=""/savegame/{savegamenum}/farm/{farm(0)}/dashboard"">Farm {farm(0)}: {farm(1)} (${roundedbal} with debt of ${roundedloan})</a></p>")
        Next

        sb.AppendLine("</HTML>")

        Return sb.ToString
    End Function

    Private Function FarmDashboard(ByVal farmnum As Integer) As String
        Dim sb As New StringBuilder

        sb.AppendLine("<HTML>")

        sb.AppendLine(GetFaviconHTML())

        sb.AppendLine(GetLinksHTML())

        sb.AppendLine("<p>Test page</p>")

        sb.AppendLine("</HTML>")

        Return sb.ToString
    End Function

    Private Function Get404HTML() As String
        'This function generates the HTML for the 404 page.
        '*********************************************************

        Dim sb As New StringBuilder
        sb.AppendLine("<HTML>")

        sb.AppendLine(GetFaviconHTML())

        sb.AppendLine(GetLinksHTML())

        sb.AppendLine($"<b style=""font-size:70px;color:red"">ERROR 404</b>")
        sb.AppendLine("</br>")
        sb.AppendLine($"<b style=""font-size:30px;color:blue"">There is nothing here. Get out!!</b>")

        sb.AppendLine("</HTML>")

        Return sb.ToString
    End Function

    Private Function GetLinksHTML() As String
        'This function generates the HTML for the links at the bottom
        'of every HTML page
        '*********************************************************

        Dim sb As New StringBuilder

        sb.AppendLine("<header class=""navbar navbar-dark sticky-top bg-dark flex-md-nowrap p-0 shadow"">")
        sb.AppendLine("<a class=""navbar-brand col-md-3 col-lg-2 me-0 px-3"" href=""/home"">FS22-Online</a>")
        sb.AppendLine("</header>")

        Return sb.ToString
    End Function

    Private Sub ListNics()
        'This function is called by the command line interface when the
        '/listnics argument is used. It displays a list of all the network interfaces
        'on OS where the web server is being executed.
        For Each ni In NetworkInterface.GetAllNetworkInterfaces()

            Console.WriteLine(ni.Name)

            For Each ip In ni.GetIPProperties.UnicastAddresses.Where(Function(ipi) ipi.Address.AddressFamily = Net.Sockets.AddressFamily.InterNetwork)
                Console.Write("    ")
                Console.WriteLine(ip.Address.ToString)
            Next
            Console.WriteLine()
        Next

    End Sub




End Module
