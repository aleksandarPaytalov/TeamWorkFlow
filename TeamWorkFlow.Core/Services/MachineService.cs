using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Machine;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Services
{
	public class MachineService : IMachineService
	{
		private readonly IRepository _repository;

		public MachineService(IRepository repository)
		{
			_repository = repository;
		}


		public async Task<ICollection<MachineServiceModel>> GetAllMachinesAsync()
		{
			return await _repository.AllReadOnly<Machine>()
				.AsNoTracking()
				.Select(m => new MachineServiceModel
				{
					Id = m.Id,
					Name = m.Name,
					IsCalibrated = m.IsCalibrated,
					CalibrationSchedule = m.CalibrationSchedule.ToString(DateFormat),
					Capacity = m.Capacity
				})
				.ToListAsync();
		}

		public async Task<(ICollection<MachineServiceModel> Machines, int TotalCount)> GetAllMachinesAsync(int page, int pageSize)
		{
			var query = _repository.AllReadOnly<Machine>().AsNoTracking();
			var totalCount = await query.CountAsync();
			var machines = await query
				.OrderBy(m => m.Id)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(m => new MachineServiceModel
				{
					Id = m.Id,
					Name = m.Name,
					IsCalibrated = m.IsCalibrated,
					CalibrationSchedule = m.CalibrationSchedule.ToString(DateFormat),
					Capacity = m.Capacity
				})
				.ToListAsync();

			return (machines, totalCount);
		}

	public async Task<MachineQueryServiceModel> AllAsync(
		MachineSorting sorting = MachineSorting.LastAdded,
		string? search = null,
		int machinesPerPage = 10,
		int currentPage = 1)
	{
		IQueryable<Machine> machinesToBeDisplayed = _repository.AllReadOnly<Machine>().AsNoTracking();

		if (!string.IsNullOrWhiteSpace(search))
		{
			string normalizedSearch = search.ToLower();
			machinesToBeDisplayed = machinesToBeDisplayed
				.Where(m => m.Name.ToLower().Contains(normalizedSearch));
		}

		machinesToBeDisplayed = sorting switch
		{
			MachineSorting.NameAscending => machinesToBeDisplayed.OrderBy(m => m.Name),
			MachineSorting.NameDescending => machinesToBeDisplayed.OrderByDescending(m => m.Name),
			MachineSorting.CalibrationDateAscending => machinesToBeDisplayed.OrderBy(m => m.CalibrationSchedule),
			MachineSorting.CalibrationDateDescending => machinesToBeDisplayed.OrderByDescending(m => m.CalibrationSchedule),
			MachineSorting.CapacityAscending => machinesToBeDisplayed.OrderBy(m => m.Capacity),
			MachineSorting.CapacityDescending => machinesToBeDisplayed.OrderByDescending(m => m.Capacity),
			_ => machinesToBeDisplayed.OrderByDescending(m => m.Id)
		};

		var machines = await machinesToBeDisplayed
			.Skip((currentPage - 1) * machinesPerPage)
			.Take(machinesPerPage)
			.Select(m => new MachineServiceModel
			{
				Id = m.Id,
				Name = m.Name,
				IsCalibrated = m.IsCalibrated,
				CalibrationSchedule = m.CalibrationSchedule.ToString(DateFormat),
				Capacity = m.Capacity
			})
			.ToListAsync();

		int totalMachines = await machinesToBeDisplayed.CountAsync();

		return new MachineQueryServiceModel()
		{
			Machines = machines,
			TotalMachinesCount = totalMachines
		};
	}

		public async Task AddNewMachineAsync(MachineFormModel model)
		{
			bool isValidDate = DateTime.TryParseExact(model.CalibrationSchedule, DateFormat,
				CultureInfo.InvariantCulture, DateTimeStyles.None, out var calibrationDate);

			if (!isValidDate)
			{
				throw new ArgumentException(string.Format(InvalidDate, DateFormat));
			}
			
			if (!bool.TryParse(model.IsCalibrated, out bool isCalibrated))
			{
				throw new ArgumentException(BooleanInput);
			}

			if (model.Capacity is < MachineCapacityMinValue or > MachineCapacityMaxValue)
			{
				throw new ArgumentException(string.Format(CapacityRange, MachineCapacityMinValue, MachineCapacityMaxValue));
			}

			var machine = new Machine()
			{				
				Name = model.Name,
				CalibrationSchedule = calibrationDate,
				Capacity = model.Capacity,
				IsCalibrated = isCalibrated,
				ImageUrl = model.ImageUrl
			};

			await _repository.AddAsync(machine);
			await _repository.SaveChangesAsync();
		}

		public async Task<MachineFormModel?> GetMachineForEditAsync(int id)
		{
			var machineForEdit = await _repository.AllReadOnly<Machine>()
				.Where(m => m.Id == id)
				.FirstOrDefaultAsync();

			return machineForEdit == null
				? throw new ArgumentException($"{InvalidIdInput}")
				: await _repository.AllReadOnly<Machine>()
					.Where(m => m.Id == id)
					.Select(m => new MachineFormModel()
					{
						Id = m.Id,
						CalibrationSchedule =
							m.CalibrationSchedule.ToString(Messages.DateFormat, CultureInfo.InvariantCulture),
						Capacity = m.Capacity,
						Name = m.Name,
						IsCalibrated = m.IsCalibrated.ToString(),
						ImageUrl = m.ImageUrl
					})
					.FirstOrDefaultAsync();
		}

		public async Task EditMachineAsync(MachineFormModel model, int id)
		{
			if (!bool.TryParse(model.IsCalibrated, out bool isCalibratedResult))
			{
				throw new ArgumentException($"{BooleanInput}");
			}

			var machineForEdit = await _repository.GetByIdAsync<Machine>(id);

			if (machineForEdit != null)
			{
				var dateIsValid = DateTime.TryParseExact(
					model.CalibrationSchedule, 
					Messages.DateFormat,
					CultureInfo.InvariantCulture, 
					DateTimeStyles.None, 
					out DateTime date);

				if (!dateIsValid)
				{
					throw new ArgumentException(string.Format(InvalidDate, DateFormat));
				}

				if (model.Capacity is < MachineCapacityMinValue or > MachineCapacityMaxValue)
				{
					throw new ArgumentException(string.Format(CapacityRange, MachineCapacityMinValue, MachineCapacityMaxValue));
				}

				machineForEdit.Name = model.Name;
				machineForEdit.CalibrationSchedule = date;
				machineForEdit.Capacity = model.Capacity;
				machineForEdit.IsCalibrated = isCalibratedResult;
				machineForEdit.ImageUrl = model.ImageUrl;

				await _repository.SaveChangesAsync();
			}
		}

		public async Task<bool> MachineExistByIdAsync(int machineId)
		{
			return await _repository.AllReadOnly<Machine>()
				.AnyAsync(m => m.Id == machineId);
		}

		public async Task<MachineDetailsServiceModel?> MachineDetailsAsync(int machineId)
		{
			return await _repository.AllReadOnly<Machine>()
				.Where(m => m.Id == machineId)
				.Select(m => new MachineDetailsServiceModel()
				{
					Id = m.Id,
					ImageUrl = m.ImageUrl,
					Name = m.Name,
					Capacity = m.Capacity,
					CalibrationSchedule = m.CalibrationSchedule.ToString(DateFormat, CultureInfo.InvariantCulture),
					IsCalibrated = m.IsCalibrated,
					TotalMachineLoad = m.TotalMachineLoad
				})
				.FirstOrDefaultAsync();

		}

		public async Task DeleteMachineAsync(int machineId)
		{
			var machine = await _repository.GetByIdAsync<Machine>(machineId);

			if (machine != null)
			{
				var listOfTasks = await GetAllTaskByAssignedMachineId(machineId);

				if (listOfTasks.Count > 0)
				{
					foreach (var task in listOfTasks)
					{
						task.MachineId = null;
					}

					// Save changes to update the MachineId values in the Task table
					await _repository.SaveChangesAsync();
				}

				// After setting MachineId to null in all related tasks, delete the machine
				await _repository.DeleteAsync<Machine>(machineId);
				await _repository.SaveChangesAsync(); // Save changes after deleting the machine
			}
		}
		
		public async Task<MachineDeleteServiceModel?> GetMachineForDeleteByIdAsync(int machineId)
		{
			return await _repository.AllReadOnly<Machine>()
				.Where(m => m.Id == machineId)
				.Select(m => new MachineDeleteServiceModel()
				{
					Id = m.Id,
					ImageUrl = m.ImageUrl,
					Name = m.Name
				})
				.FirstOrDefaultAsync();
		}

		public async Task<ICollection<Infrastructure.Data.Models.Task>> GetAllTaskByAssignedMachineId(int machineId)
		{
			var tasksList = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
				.Where(t => t.MachineId == machineId)
				.ToListAsync();

			return tasksList;
		}
	}
}
