using System.ComponentModel;

namespace Imsat.Bashmaky.Model.Database.Types
{
    public enum DeviceType
    {
        [Description("Терминал")]
        Terminal,
        [Description("Башмак")]
        Bashmak,
        [Description("Ящик")]
        Box
    }
}
