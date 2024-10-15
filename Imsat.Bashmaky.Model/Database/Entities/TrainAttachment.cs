using Imsat.Bashmaky.Model.Database.Entities.Devices;


namespace Imsat.Bashmaky.Model.Database.Entities
{
    public class TrainAttachment
    {
        public int Id { get; set; }
        public DateTime AttachingTimeUtc { get; set; }
        public DateTime DetachingTimeUtc { get; set; }
        public int RailwayNumber { get; set; }
        public int WagonNumber { get; set; }
        public List<Bashmak> Bashmakies { get; set; } = new List<Bashmak>();
        public string AttachingFitter { get; set; } = "";
        public string DetachingFitter { get; set; } = "";
    }

    
}
