using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Enumerations;

namespace TeamWorkFlow.Core.Models.Machine
{
    public class AllMachinesQueryModel
    {
        public int MachinesPerPage { get; } = 10;

        [Display(Name = "Search by machine name")]
        public string Search { get; init; } = null!;

        public MachineSorting Sorting { get; init; }
        
        public int CurrentPage { get; init; } = 1;

        public int TotalMachinesCount { get; set; }

        public IEnumerable<MachineServiceModel> Machines { get; set; } = new List<MachineServiceModel>();
    }
}
