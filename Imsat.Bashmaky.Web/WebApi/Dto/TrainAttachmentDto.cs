using Imsat.Bashmaky.Model.Database.Entities;

namespace Imsat.Bashmaky.Web.WebApi.Dto
{
    public class TrainAttachmentDto
    {
        public int Id { get; set; }
        public DateTime AttachingTimeUtc { get; set; }
        public DateTime DetachingTimeUtc { get; set; }
        public string AttachingFitter { get; set; } = "";
        public string DetachingFitter { get; set; } = "";
        public IEnumerable<BashmakDto> Bashmakies { get; set; } = new List<BashmakDto>();

        public static TrainAttachmentDto Create(TrainAttachment trainAttachment)
        {
            return new TrainAttachmentDto
            {
                Id = trainAttachment.Id,
                AttachingFitter = trainAttachment.AttachingFitter,
                DetachingFitter = trainAttachment.DetachingFitter,
                AttachingTimeUtc = trainAttachment.AttachingTimeUtc,
                DetachingTimeUtc = trainAttachment.DetachingTimeUtc,
                Bashmakies = trainAttachment.Bashmakies.Select(BashmakDto.Create)
            };
        }


    }
}
