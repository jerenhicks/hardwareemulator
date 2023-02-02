using System.Net.Sockets;
using Newtonsoft.Json;

class CupServer
{

    private Server server;
    private TcpListener tcpserver;

    public CupServer(Server server)
    {
        this.server = server;
        //FIXME: obsolete here, do something about that. 
        tcpserver = new TcpListener(server.port);
    }

    public void StartServer()
    {
        tcpserver.Start();
        ProcessMessage("Starting server, waiting for a connection...");
        EstablishConnection();
    }

    public void StopServer()
    {
        tcpserver.Stop();
    }

    private void EstablishConnection()
    {
        TcpClient client = tcpserver.AcceptTcpClient();

        try
        {
            ProcessMessage("Connection Found");
            ProcessMessages(client);
        }
        catch (Exception e)
        {
            //FIXME: find the correct exception here, stupid vscode.
            Console.WriteLine("Error while establishing connection");
            client.Close();
        }
    }

    private void ProcessMessages(TcpClient client)
    {
        NetworkStream strm = null;
        try
        {
            strm = client.GetStream();

            while (true)
            {
                var reader = new BinaryReader(strm);
                string p = reader.ReadString(); // you have to cast the deserialized object 

                Message message = JsonConvert.DeserializeObject<Message>(p);

                ProcessMessage(String.Format("Message received: {0} ", p));
            }

        }
        catch (Exception e)
        {
            //FIXME: find the correct exception here, stupid vscode.
            strm.Close();
            client.Close();
            ProcessMessage("Client Disconnect, Waiting For New Client");
            EstablishConnection();
        }
    }

    private void ProcessMessage(string message)
    {
        //FIXME: get a logger
        Console.WriteLine("(Server ID {0}): {1}", server.id, message);
    }

}