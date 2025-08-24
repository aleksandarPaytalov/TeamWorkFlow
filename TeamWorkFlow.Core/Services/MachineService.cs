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
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TasksOperators)
					.ThenInclude(to => to.Operator)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.Project)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TaskStatus)
				.Select(m => new MachineServiceModel
				{
					Id = m.Id,
					Name = m.Name,
					IsCalibrated = m.IsCalibrated,
					CalibrationSchedule = m.CalibrationSchedule.ToString(DateFormat),
					Capacity = m.Capacity,
					IsOccupied = m.Tasks.Any(t => t.TaskStatus.Name.ToLower() != "finished"),
					AssignedTaskId = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => (int?)t.Id).FirstOrDefault(),
					AssignedTaskName = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Name).FirstOrDefault(),
					AssignedTaskProjectNumber = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Project.ProjectNumber).FirstOrDefault(),
					TaskStatus = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.TaskStatus.Name).FirstOrDefault(),
					AssignedOperatorNames = string.Join(", ", m.Tasks
						.Where(t => t.TaskStatus.Name.ToLower() != "finished")
						.SelectMany(t => t.TasksOperators)
						.Select(to => to.Operator.FullName))
				})
				.ToListAsync();
		}

		public async Task<(ICollection<MachineServiceModel> Machines, int TotalCount)> GetAllMachinesAsync(int page, int pageSize)
		{
			var query = _repository.AllReadOnly<Machine>()
				.AsNoTracking()
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TasksOperators)
					.ThenInclude(to => to.Operator)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.Project)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TaskStatus);

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
					Capacity = m.Capacity,
					IsOccupied = m.Tasks.Any(t => t.TaskStatus.Name.ToLower() != "finished"),
					AssignedTaskId = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => (int?)t.Id).FirstOrDefault(),
					AssignedTaskName = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Name).FirstOrDefault(),
					AssignedTaskProjectNumber = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Project.ProjectNumber).FirstOrDefault(),
					TaskStatus = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.TaskStatus.Name).FirstOrDefault(),
					AssignedOperatorNames = string.Join(", ", m.Tasks
						.Where(t => t.TaskStatus.Name.ToLower() != "finished")
						.SelectMany(t => t.TasksOperators)
						.Select(to => to.Operator.FullName))
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
			.Include(m => m.Tasks)
				.ThenInclude(t => t.TasksOperators)
				.ThenInclude(to => to.Operator)
			.Include(m => m.Tasks)
				.ThenInclude(t => t.Project)
			.Include(m => m.Tasks)
				.ThenInclude(t => t.TaskStatus)
			.Select(m => new MachineServiceModel
			{
				Id = m.Id,
				Name = m.Name,
				IsCalibrated = m.IsCalibrated,
				CalibrationSchedule = m.CalibrationSchedule.ToString(DateFormat),
				Capacity = m.Capacity,
				IsOccupied = m.Tasks.Any(t => t.TaskStatus.Name.ToLower() != "finished"),
				AssignedTaskId = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => (int?)t.Id).FirstOrDefault(),
				AssignedTaskName = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Name).FirstOrDefault(),
				AssignedTaskProjectNumber = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Project.ProjectNumber).FirstOrDefault(),
				TaskStatus = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.TaskStatus.Name).FirstOrDefault(),
				AssignedOperatorNames = string.Join(", ", m.Tasks
					.Where(t => t.TaskStatus.Name.ToLower() != "finished")
					.SelectMany(t => t.TasksOperators)
					.Select(to => to.Operator.FullName))
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
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TasksOperators)
					.ThenInclude(to => to.Operator)
					.ThenInclude(o => o.User)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.Project)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TaskStatus)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.Priority)
				.Where(m => m.Id == machineId)
				.Select(m => new MachineDetailsServiceModel()
				{
					Id = m.Id,
					ImageUrl = m.ImageUrl,
					Name = m.Name,
					Capacity = m.Capacity,
					CalibrationSchedule = m.CalibrationSchedule.ToString(DateFormat, CultureInfo.InvariantCulture),
					IsCalibrated = m.IsCalibrated,
					TotalMachineLoad = m.TotalMachineLoad,
					IsOccupied = m.Tasks.Any(t => t.TaskStatus.Name.ToLower() != "finished"),
					AssignedTaskId = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => (int?)t.Id).FirstOrDefault(),
					AssignedTaskName = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Name).FirstOrDefault(),
					AssignedTaskProjectNumber = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Project.ProjectNumber).FirstOrDefault(),
					AssignedTaskDescription = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Description).FirstOrDefault(),
					AssignedTaskDeadline = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.DeadLine != null ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : null).FirstOrDefault(),
					AssignedTaskPriority = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Priority.Name).FirstOrDefault(),
					TaskStatus = m.Tasks.Where(t => t.TaskStatus.Name.ToLower() != "finished").Select(t => t.TaskStatus.Name).FirstOrDefault(),
					AssignedOperatorNames = string.Join(", ", m.Tasks
						.Where(t => t.TaskStatus.Name.ToLower() != "finished")
						.SelectMany(t => t.TasksOperators)
						.Select(to => to.Operator.FullName)),
					AssignedOperators = m.Tasks
						.Where(t => t.TaskStatus.Name.ToLower() != "finished")
						.SelectMany(t => t.TasksOperators)
						.Select(to => new AssignedOperatorInfo
						{
							OperatorId = to.OperatorId,
							OperatorName = to.Operator.FullName,
							OperatorEmail = to.Operator.User.Email ?? ""
						}).ToList()
				})
				.FirstOrDefaultAsync();

		}

		public async Task DeleteMachineAsync(int machineId)
		{
			// Validate that machine can be deleted (not occupied by active tasks)
			var validation = await ValidateMachineForDeletionAsync(machineId);
			if (!validation.CanDelete)
			{
				throw new InvalidOperationException(validation.Reason);
			}

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
			var machine = await _repository.AllReadOnly<Machine>()
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TaskStatus)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.Project)
				.Where(m => m.Id == machineId)
				.FirstOrDefaultAsync();

			if (machine == null)
			{
				return null;
			}

			// Check if machine can be deleted
			var validation = await ValidateMachineForDeletionAsync(machineId);

			return new MachineDeleteServiceModel()
			{
				Id = machine.Id,
				ImageUrl = machine.ImageUrl,
				Name = machine.Name,
				CanDelete = validation.CanDelete,
				DeletionBlockReason = validation.CanDelete ? null : validation.Reason
			};
		}

		public async Task<ICollection<Infrastructure.Data.Models.Task>> GetAllTaskByAssignedMachineId(int machineId)
		{
			var tasksList = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
				.Where(t => t.MachineId == machineId)
				.ToListAsync();

			return tasksList;
		}

		public async Task<bool> IsMachineAvailableForAssignmentAsync(int machineId)
		{
			var validation = await ValidateMachineAvailabilityAsync(machineId);
			return validation.CanAssign;
		}

		public async Task<(bool CanAssign, string Reason)> ValidateMachineAvailabilityAsync(int machineId, int? excludeTaskId = null)
		{
			var machine = await _repository.AllReadOnly<Machine>()
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TaskStatus)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.Project)
				.FirstOrDefaultAsync(m => m.Id == machineId);

			if (machine == null)
			{
				return (false, "Machine not found");
			}

			if (!machine.IsCalibrated)
			{
				return (false, "Machine is not calibrated and cannot be assigned to tasks");
			}

			// Check if machine is already assigned to another active task
			var activeTask = machine.Tasks
				.Where(t => t.TaskStatus.Name.ToLower() != "finished")
				.Where(t => excludeTaskId == null || t.Id != excludeTaskId)
				.FirstOrDefault();

			if (activeTask != null)
			{
				return (false, $"Machine is already assigned to task '{activeTask.Name}' (Project #{activeTask.Project?.ProjectNumber})");
			}

			return (true, "Machine is available for assignment");
		}

		public async Task<(bool CanDelete, string Reason)> ValidateMachineForDeletionAsync(int machineId)
		{
			var machine = await _repository.AllReadOnly<Machine>()
				.Include(m => m.Tasks)
					.ThenInclude(t => t.TaskStatus)
				.Include(m => m.Tasks)
					.ThenInclude(t => t.Project)
				.FirstOrDefaultAsync(m => m.Id == machineId);

			if (machine == null)
			{
				return (false, "Machine not found");
			}

			// Check if machine is currently occupied by any active (non-finished) tasks
			var activeTask = machine.Tasks
				.Where(t => t.TaskStatus.Name.ToLower() != "finished")
				.FirstOrDefault();

			if (activeTask != null)
			{
				return (false, $"This machine is currently used for task '{activeTask.Name}' (Project: {activeTask.Project.ProjectNumber}) and cannot be deleted.");
			}

			return (true, "Machine can be deleted");
		}
	}
}
