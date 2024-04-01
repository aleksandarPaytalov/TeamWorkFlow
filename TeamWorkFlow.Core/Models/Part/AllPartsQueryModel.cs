﻿using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Enumerations;

namespace TeamWorkFlow.Core.Models.Part
{
    public class AllPartsQueryModel
    {
        public int PartsPerPage { get; } = 6;

        public string Status { get; init; } = string.Empty;

        [Display(Name = "Search by part name")]
        public string Search { get; init; } = string.Empty;

        public PartSorting Sorting { get; init; }
        
        public int CurrentPage { get; init; } = 1;

        public int TotalPartsCount { get; set; }

        public IEnumerable<string> Statuses { get; set; } = null!;

        public IEnumerable<PartServiceModel> Parts { get; set; } = new List<PartServiceModel>();
    }
}
