using System.Net.Sockets;
using Newtonsoft.Json;
using System.Net;


class CupServer
{


    private ServerStatus status;
    private TcpListener tcpserver;
    private Queue<Message> outboundMessages;

    public CupServer(Server server)
    {
        this.status = new ServerStatus(server.id);
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        tcpserver = new TcpListener(localAddr, server.port);
        outboundMessages = new Queue<Message>();
    }

    public void StartServer()
    {
        tcpserver.Start();
        this.status.SetStatus(ServerStatusEnum.ONLINE);
        ProcessMessage("Starting server, waiting for a connection...");
        EstablishConnection();
    }

    public void StopServer()
    {
        tcpserver.Stop();
        this.status.SetStatus(ServerStatusEnum.OFFLINE);
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
            ProcessMessage("Error while establishing connection with client");
            client.Close();
        }
    }

    private void ProcessMessages(TcpClient client)
    {
        NetworkStream strm = null;
        try
        {
            strm = client.GetStream();

            var tasks = new List<Task>();
            var inboundTask = Task.Run(() =>
            {
                ProcessInboundMessages(strm);
            });
            tasks.Add(inboundTask);

            var outboundTask = Task.Run(() =>
            {
                ProcessOutboundMessages(strm);
            });
            tasks.Add(outboundTask);
            Task.WaitAll(tasks.ToArray());
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

    private void ProcessInboundMessages(NetworkStream strm)
    {
        var reader = new BinaryReader(strm);

        while (true)
        {
            string p = reader.ReadString(); // you have to cast the deserialized object 

            Message message = JsonConvert.DeserializeObject<Message>(p);

            ProcessMessage(String.Format("Message received: {0} ", p));

            outboundMessages.Enqueue(Message.CreateMessage(MessageType.ACK));

        }


    }

    private void ProcessOutboundMessages(NetworkStream strm)
    {
        var writer = new BinaryWriter(strm);

        //FIXME: this still keeps going, even if the inbound fails. Need a way to close these down together. 
        while (true)
        {
            while (outboundMessages.Count > 0)
                writer.Write(JsonConvert.SerializeObject(outboundMessages.Dequeue()));

            Thread.Sleep(1000);
        }

    }

    private void ProcessMessage(string message)
    {
        //FIXME: get a logger
        Console.WriteLine("(Server ID {0}): {1}", status.GetID(), message);
    }

}