using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Admin.Operator;
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
        private readonly UserManager<IdentityUser> _userManager;

		public OperatorService(IRepository repository, 
            UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }


		public async Task<ICollection<OperatorServiceModel>> GetAllActiveOperatorsAsync()
		{
			 return await _repository.AllReadOnly<Operator>()
				 .Where(o => o.IsActive)
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

		public async Task<(ICollection<OperatorServiceModel> Operators, int TotalCount)> GetAllActiveOperatorsAsync(int page, int pageSize)
		{
			var query = _repository.AllReadOnly<Operator>().Where(o => o.IsActive);
			var totalCount = await query.CountAsync();
			var operators = await query
				.OrderBy(o => o.Id)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
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

			return (operators, totalCount);
		}

	public async Task<OperatorQueryServiceModel> AllAsync(
		OperatorSorting sorting = OperatorSorting.LastAdded,
		string? search = null,
		int operatorsPerPage = 10,
		int currentPage = 1)
	{
		IQueryable<Operator> operatorsToBeDisplayed = _repository.AllReadOnly<Operator>().Where(o => o.IsActive);

		if (!string.IsNullOrWhiteSpace(search))
		{
			string normalizedSearch = search.ToLower();
			operatorsToBeDisplayed = operatorsToBeDisplayed
				.Where(o => o.FullName.ToLower().Contains(normalizedSearch) ||
						   o.Email.ToLower().Contains(normalizedSearch) ||
						   o.PhoneNumber.ToLower().Contains(normalizedSearch));
		}

		operatorsToBeDisplayed = sorting switch
		{
			OperatorSorting.NameAscending => operatorsToBeDisplayed.OrderBy(o => o.FullName),
			OperatorSorting.NameDescending => operatorsToBeDisplayed.OrderByDescending(o => o.FullName),
			OperatorSorting.EmailAscending => operatorsToBeDisplayed.OrderBy(o => o.Email),
			OperatorSorting.EmailDescending => operatorsToBeDisplayed.OrderByDescending(o => o.Email),
			OperatorSorting.CapacityAscending => operatorsToBeDisplayed.OrderBy(o => o.Capacity),
			OperatorSorting.CapacityDescending => operatorsToBeDisplayed.OrderByDescending(o => o.Capacity),
			OperatorSorting.StatusAscending => operatorsToBeDisplayed.OrderBy(o => o.AvailabilityStatus.Name),
			OperatorSorting.StatusDescending => operatorsToBeDisplayed.OrderByDescending(o => o.AvailabilityStatus.Name),
			_ => operatorsToBeDisplayed.OrderByDescending(o => o.Id)
		};

		var operators = await operatorsToBeDisplayed
			.Skip((currentPage - 1) * operatorsPerPage)
			.Take(operatorsPerPage)
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

		int totalOperators = await operatorsToBeDisplayed.CountAsync();

		return new OperatorQueryServiceModel()
		{
			Operators = operators,
			TotalOperatorsCount = totalOperators
		};
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

		public async Task AddNewOperatorAsync(OperatorFormModel model, string userId)
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
					PhoneNumber = model.PhoneNumber,
					UserId = userId
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
				.Where(o => o.IsActive)
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
				.AnyAsync(o => o.Id == operatorId); // && o.IsActive == true);
		}

		public async Task<OperatorDetailsServiceModel?> GetOperatorDetailsByIdAsync(int id)
		{
			int totalCompletedTasks = await GetAllCompletedTasksAssignedToOperatorByIdAsync(id);
			int totalActiveAssignedTasks = await GetAllActiveAssignedTaskToOperatorByIdAsync(id);

			return await _repository.AllReadOnly<Operator>()
				.Where(o => o.Id == id)
				.Where(o => o.IsActive)
				.Select(o => new OperatorDetailsServiceModel()
				{
					Id = o.Id,
					FullName = o.FullName,
					Email = o.Email,
					PhoneNumber = o.PhoneNumber,
					AvailabilityStatus = o.AvailabilityStatus.Name,
					Capacity = o.Capacity,
					IsActive = o.IsActive,
					TotalCompletedTasks = totalCompletedTasks,
					CurrentTasks = totalActiveAssignedTasks
				})
				.FirstOrDefaultAsync();
		}

		public async Task<int> GetAllCompletedTasksAssignedToOperatorByIdAsync(int operatorId)
		{
			return await _repository.AllReadOnly<TaskOperator>()
				.Where(o => o.OperatorId == operatorId && o.Task.TaskStatus.Name.ToLower() == "finished")
				.CountAsync();
		}

		public async Task<int> GetAllActiveAssignedTaskToOperatorByIdAsync(int operatorId)
		{
			return await _repository.AllReadOnly<TaskOperator>()
				.Where(o => o.OperatorId == operatorId && o.Task.TaskStatus.Name.ToLower() == "in progress")
				.CountAsync();
		}

		public async Task<OperatorDeleteServiceModel?> GetOperatorModelForDeleteByIdAsync(int operatorId)
		{
			return await _repository.AllReadOnly<Operator>()
				.Where(o => o.Id == operatorId)
				.Where(o => o.IsActive)
				.Select(o => new OperatorDeleteServiceModel()
				{
					Id = o.Id,
					FullName = o.FullName,
					Email = o.Email,
					PhoneNumber = o.PhoneNumber,
					IsActive = o.IsActive
				})
				.FirstOrDefaultAsync();
		}

		public async Task DeleteOperatorByIdAsync(int operatorId)
		{
			var operatorForDelete = await _repository.GetByIdAsync<Operator>(operatorId);

			if (operatorForDelete != null)
			{
				await _repository.DeleteAsync<Operator>(operatorForDelete.Id);
				await _repository.SaveChangesAsync();
			}
		}

		public async Task<ICollection<OperatorAccessServiceModel>> GetAllOperatorsAsync()
		{
			var model = await _repository.AllReadOnly<Operator>()
				.Select(o => new OperatorAccessServiceModel()
				{
					FullName = o.FullName,
					Email = o.Email,
					PhoneNumber = o.PhoneNumber,
					IsActive = o.IsActive
				})
				.ToListAsync();

			return model;
		}

		public async Task<ICollection<OperatorServiceModel>> GetAllUnActiveOperatorsAsync()
		{
			return await _repository.AllReadOnly<Operator>()
				.Where(o => o.IsActive == false)
				.Select(o => new OperatorServiceModel()
				{
					Id = o.Id,
					FullName = o.FullName,
					Email = o.Email,
					PhoneNumber = o.PhoneNumber,
					AvailabilityStatus = o.AvailabilityStatus.Name,
					Capacity = o.Capacity
				})
				.ToListAsync();
		}

		public async Task ActivateOperatorAsync(int id)
		{
			var operatorModel = await _repository.GetByIdAsync<Operator>(id);

			if (operatorModel != null && operatorModel.IsActive == false)
			{
				operatorModel.IsActive = true;

				await _repository.SaveChangesAsync();
			}
		}

		public async Task<string?> GetUserIdByEmailAsync(string emailAddress)
        {
            var user = await _userManager.FindByEmailAsync(emailAddress);

            if (user != null)
            {
                return user.Id;
            }

            return null;
        }

        public async Task<string?> GetOperatorFullNameByUserIdAsync(string userId)
        {
            var operatorModel = await _repository.AllReadOnly<Operator>()
                .Where(o => o.UserId == userId)
                .FirstOrDefaultAsync();

            return operatorModel?.FullName;
        }
	}
}
