public class ServerStatus
{
    private String id;
    private ServerStatusEnum status;
    private string image;
    private readonly static string DEFAULT_IMAGE = "default.png";

    public ServerStatus(String id)
    {
        this.id = id;
        status = ServerStatusEnum.OFFLINE;
        this.image = DEFAULT_IMAGE;
    }

    public void SetStatus(ServerStatusEnum newStatus)
    {
        this.status = newStatus;
    }

    public String GetID()
    {
        return this.id;
    }

    public string GetImage()
    {
        return this.image;
    }

    public void SetImage(string newImage)
    {
        this.image = newImage;
    }
}