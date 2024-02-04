namespace McUltimateTools;

public class ServiceController
{
    public Db db;
    public string something = "something";
    
    public ServiceController(Db db)
    {
        this.db = db;
    }
}
