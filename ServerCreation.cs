using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;


class ServerCreation
{
    private readonly string serverFileLocation = @".\serverlist.json";

    public void StartUp()
    {
        ServerList serverList = ReadServerList();

        var tasks = new List<Task>();

        foreach (Server server in serverList.servers)
        {
            var task = Task.Run(() =>
            {
                StartServer(server);
            });
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
    }

    public ServerList ReadServerList()
    {
        string jsonFile = File.ReadAllText(serverFileLocation);
        ServerList serverList = null;
        if (jsonFile != null)
        {
            serverList = JsonConvert.DeserializeObject<ServerList>(jsonFile);
        }
        else
        {
            //let's be brutal, just kill things
            throw new Exception("Could not find the json file");
        }
        return serverList;
    }


    private bool StartServer(Server server)
    {

        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, server.port);

        try
        {

            // Create a Socket that will use Tcp protocol      
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // A Socket must be associated with an endpoint using the Bind method  
            listener.Bind(localEndPoint);
            // Specify how many requests a Socket can listen before it gives Server busy response.  
            // We will listen 10 requests at a time  
            listener.Listen(1);

            string logMessage = String.Format("Starting server ({0}), waiting for a connection...", server.id);
            Console.WriteLine(logMessage);
            Socket handler = listener.Accept();
            Console.WriteLine("Are we waiting?");

            // Incoming data from the client.    
            string data = null;
            byte[] bytes = null;

            while (true)
            {
                bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }

            Console.WriteLine("Text received : {0}", data);

            byte[] msg = Encoding.ASCII.GetBytes(data);
            handler.Send(msg);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return true;
    }





}

