namespace TeamWorkFlow.Core.Models.Part
{
    public class PartQueryServiceModel
    {
        public int TotalPartsCount { get; set; }

        public IEnumerable<PartServiceModel> Parts { get; set; } = new List<PartServiceModel>();
    }
}
