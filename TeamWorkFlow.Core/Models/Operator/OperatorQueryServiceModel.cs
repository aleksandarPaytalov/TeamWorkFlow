namespace TeamWorkFlow.Core.Models.Operator
{
    public class OperatorQueryServiceModel
    {
        public int TotalOperatorsCount { get; set; }

        public IEnumerable<OperatorServiceModel> Operators { get; set; } = new List<OperatorServiceModel>();
    }
}
