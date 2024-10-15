using System.ComponentModel;

namespace Imsat.Bashmaky.Model.Database.Types
{
    public enum ST //номер события от устройства
    {
        [Description("Открытие ящика")]
        BoxOpened = 0,
        [Description("Закрытие ящика ")]
        BoxClosed = 1,
        [Description("Несанкционированный доступ")]
        IlligalAccess = 2,
        [Description("Поднят")]
        Raised = 10,
        [Description("Положен на рельс")]
        PutOnRail = 11,
        [Description("Положен в ящик")]
        PutInBox = 12,
        Non
    }
}
