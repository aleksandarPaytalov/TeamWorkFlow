using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Operator;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Services
{
	public class OperatorService : IOperatorService
	{
		private readonly TeamWorkFlowDbContext _context;

		public OperatorService(TeamWorkFlowDbContext context)
		{
			_context = context;
		}

		public async Task<ICollection<OperatorViewModel>> GetAllOperatorsAsync()
		{
			 return await _context.Operators
				.AsNoTracking()
				.Select(o => new OperatorViewModel()
				{
					Id = o.Id,
					FullName = o.FullName,
					Email = o.Email,
					PhoneNumber = o.PhoneNumber,
					AvailabilityStatus = o.AvailabilityStatus.Name,
					Capacity = o.Capacity,
					IsActive = o.IsActive
				})
				.ToListAsync();
		}

		public async Task<ICollection<AvailabilityStatusViewModel>> GetAllStatusesAsync()
		{
			return await _context.OperatorAvailabilityStatusEnumerable
				.Select(a => new AvailabilityStatusViewModel()
				{
					Id = a.Id,
					Name = a.Name
				})
				.ToListAsync();
		}

		public async Task AddNewOperatorAsync(OperatorServicesModel model)
		{
			if (bool.TryParse(model.IsActive, out bool isActive))
			{
				
				var operatorModel = new Operator()
				{
					Id = model.Id,
					AvailabilityStatusId = model.AvailabilityStatusId,
					Capacity = model.Capacity,
					Email = model.Email,
					FullName = model.FullName,
					IsActive = isActive,
					PhoneNumber = model.PhoneNumber
				};

				await _context.AddAsync(operatorModel);
				await _context.SaveChangesAsync();
			}
			else
			{
				throw new ArgumentException("Invalid input. Must be true or false!");
			}
		}

		public async Task<OperatorServicesModel?> GetOperatorForEditAsync(int id)
		{
			var operatorModelForEditing = await _context.Operators
				.Where(o => o.Id == id)
				.FirstOrDefaultAsync();

			if (operatorModelForEditing == null)
			{
				throw new ArgumentException($"{Messages.InvalidIdInput}");
			}

			var statusModels = await GetAllStatusesAsync();

			return await _context.Operators
				.Where(o => o.Id == id)
				.Select(o => new OperatorServicesModel()
				{
					FullName = o.FullName,
					AvailabilityStatusId = o.AvailabilityStatusId,
					Capacity = o.Capacity,
					Email = o.Email,
					IsActive = o.IsActive.ToString(),
					PhoneNumber = o.PhoneNumber,
					AvailabilityStatusModels = statusModels

				})
				.FirstOrDefaultAsync();
		}

		public async Task EditOperatorAsync(OperatorServicesModel model, int id)
        {
            var operatorForEdit = await _context.Operators.FindAsync(id);

            var isValid = bool.TryParse(model.IsActive, out var result);

            if (!isValid)
            {
                throw new ArgumentException($"{Messages.BooleanInput}");
            }

            if (operatorForEdit != null)
            {
                operatorForEdit.FullName = model.FullName;
                operatorForEdit.Email = model.Email;
                operatorForEdit.Capacity = model.Capacity;
                operatorForEdit.IsActive = result;
                operatorForEdit.AvailabilityStatusId = model.AvailabilityStatusId;
                operatorForEdit.PhoneNumber = model.PhoneNumber;

                await _context.SaveChangesAsync();
            }
        }

	}
}
