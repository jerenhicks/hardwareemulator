using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


class Program
{
    readonly static string serverFileLocation = @".\serverlist.json";

    public static int Main(String[] args)
    {
        ServerCreation serverCreation = new ServerCreation();
        serverCreation.StartUp();
        return 0;
    }
}
