using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


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
        TcpListener tcpserver = new TcpListener(server.port);
        tcpserver.Start();
        Console.WriteLine("Starting server ({0}), waiting for a connection...", server.id);
        TcpClient client = tcpserver.AcceptTcpClient();
        Console.WriteLine("Connection found");
        NetworkStream strm = client.GetStream();

        //TODO: we need to loop here for messages. 
        Console.WriteLine("message found");
        IFormatter formatter = new BinaryFormatter();

        //Person p = (Person)formatter.Deserialize(strm); // you have to cast the deserialized object 

        //Console.WriteLine("Hi, I'm " + p.FirstName + " " + p.LastName + " and I'm " + p.age + " years old!");
        Console.WriteLine("Message received");



        strm.Close();
        client.Close();
        tcpserver.Stop();
        return true;
    }





}

