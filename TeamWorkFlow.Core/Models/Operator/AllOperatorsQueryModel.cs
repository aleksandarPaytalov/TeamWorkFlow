using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Enumerations;

namespace TeamWorkFlow.Core.Models.Operator
{
    public class AllOperatorsQueryModel
    {
        public int OperatorsPerPage { get; } = 10;

        [Display(Name = "Search by operator name or email")]
        public string Search { get; init; } = null!;

        public OperatorSorting Sorting { get; init; }
        
        public int CurrentPage { get; init; } = 1;

        public int TotalOperatorsCount { get; set; }

        public IEnumerable<OperatorServiceModel> Operators { get; set; } = new List<OperatorServiceModel>();
    }
}
