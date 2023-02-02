using Newtonsoft.Json;

class ServerCreation
{
    private readonly string serverFileLocation = @".\serverlist.json";

    public void StartUp()
    {
        ServerList serverList = ReadServerList();

        var tasks = new List<Task>();

        List<CupServer> cupServers = new List<CupServer>();
        foreach (Server server in serverList.servers)
        {
            CupServer cupServer = new CupServer(server);
            cupServers.Add(cupServer);

            var task = Task.Run(() =>
                {
                    cupServer.StartServer();
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

}