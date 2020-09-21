namespace HydrogenBot.Models
{
    public interface IDiService
    {
    }
    
    public interface IScopedDiService : IDiService
    {
    }

    public interface ISingletonDiService : IDiService
    {
    }
}