using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("TaskOperator data model")]
    public class TaskOperator
    {
        [Required]
        [ForeignKey(nameof(Operator))]
        public int OperatorId { get; set; }

        public Operator Operator { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Task))]
        public int TaskId { get; set; }
        public Task Task { get; set; } = null!;
    }
}
