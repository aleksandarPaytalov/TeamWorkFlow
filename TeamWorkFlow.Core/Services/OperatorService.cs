using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Operator;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;
using static TeamWorkFlow.Core.Constants.Messages;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Services
{
	public class OperatorService : IOperatorService
	{
		private readonly IRepository _repository;

		public OperatorService(IRepository repository)
		{
			_repository = repository;
		}


		public async Task<ICollection<OperatorServiceModel>> GetAllOperatorsAsync()
		{
			 return await _repository.AllReadOnly<Operator>()
				.Select(o => new OperatorServiceModel()
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

		public async Task<ICollection<AvailabilityStatusServiceModel>> GetAllOperatorStatusesAsync()
		{
			return await _repository.AllReadOnly<OperatorAvailabilityStatus>()
				.Select(a => new AvailabilityStatusServiceModel()
				{
					Id = a.Id,
					Name = a.Name
				})
				.ToListAsync();
		}

		public async Task AddNewOperatorAsync(OperatorFormModel model)
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

				await _repository.AddAsync(operatorModel);
				await _repository.SaveChangesAsync();
			}
			else
			{
				throw new ArgumentException(string.Format(BooleanInput));
			}
		}

		public async Task<OperatorFormModel?> GetOperatorForEditAsync(int id)
		{
			return await _repository.AllReadOnly<Operator>()
				.Where(o => o.Id == id)
				.Select(o => new OperatorFormModel()
				{
					FullName = o.FullName,
					AvailabilityStatusId = o.AvailabilityStatusId,
					Capacity = o.Capacity,
					Email = o.Email,
					IsActive = o.IsActive.ToString(),
					PhoneNumber = o.PhoneNumber
				})
				.FirstOrDefaultAsync();
		}

		public async Task EditOperatorAsync(OperatorFormModel model, int id)
        {
            var operatorForEdit = await _repository.GetByIdAsync<Operator>(id);

            var isValid = bool.TryParse(model.IsActive, out var result);

            if (!isValid)
            {
                throw new ArgumentException($"{BooleanInput}");
            }

            if (operatorForEdit != null)
            {
                operatorForEdit.FullName = model.FullName;
                operatorForEdit.Email = model.Email;
                operatorForEdit.Capacity = model.Capacity;
                operatorForEdit.IsActive = result;
                operatorForEdit.AvailabilityStatusId = model.AvailabilityStatusId;
                operatorForEdit.PhoneNumber = model.PhoneNumber;

                await _repository.SaveChangesAsync();
            }
        }

		public async Task<bool> OperatorStatusExistAsync(int statusId)
		{
			 bool isValid = await _repository.AllReadOnly<OperatorAvailabilityStatus>()
				.AnyAsync(os => os.Id == statusId);

			 return isValid;
		}

		public async Task<bool> OperatorExistByIdAsync(int operatorId)
		{
			return await _repository.AllReadOnly<Operator>()
				.AnyAsync(o => o.Id == operatorId);
		}

		public Task<OperatorDetailsServiceModel> GetOperatorDetailsByIdAsync(int id)
		{
			throw new NotImplementedException();
		}
	}
}
