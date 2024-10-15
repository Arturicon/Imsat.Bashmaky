using System.ComponentModel;

namespace Imsat.Bashmaky.Model.Database.Types
{
    public enum Connection
    {
        [Description("На связи")]
        Connect,
        [Description("Связь разорвана")]
        Disconnect
    }
}
