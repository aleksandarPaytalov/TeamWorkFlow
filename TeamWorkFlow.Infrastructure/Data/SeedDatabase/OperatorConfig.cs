using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class OperatorConfig : IEntityTypeConfiguration<Operator>
	{
		public void Configure(EntityTypeBuilder<Operator> builder)
		{
			var data = new SeedData();
			builder.HasData(new Operator[]
			{
				data.OperatorOne,
				data.OperatorTwo,
				data.OperatorThree
			});
		}
	}
}
