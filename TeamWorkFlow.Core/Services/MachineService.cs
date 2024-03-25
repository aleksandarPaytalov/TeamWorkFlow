using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Machine;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Services
{
	public class MachineService : IMachineService
	{
		private readonly TeamWorkFlowDbContext _context;

		public MachineService(TeamWorkFlowDbContext context)
		{
			_context = context;
		}

		public async Task<ICollection<MachineViewModel>> GetAllMachinesAsync()
		{
			return await _context.Machines
				.AsNoTracking()
				.Select(m => new MachineViewModel
				{
					Id = m.Id,
					Name = m.Name,
					IsCalibrated = m.IsCalibrated,
					CalibrationSchedule = m.CalibrationSchedule.ToString(Messages.DateFormat),
					Capacity = m.Capacity
				})
				.ToListAsync();
		}

		public async Task AddNewMachineAsync(MachineServiceModel model)
		{
			bool isValidDate = DateTime.TryParseExact(model.CalibrationSchedule, Messages.DateFormat,
				CultureInfo.InvariantCulture, DateTimeStyles.None, out var calibrationDate);

			if (!bool.TryParse(model.IsCalibrated, out bool isCalibrated))
			{
				throw new ArgumentException("Incorrect input. Must be true or false.");
			}

			if (!isValidDate)
			{
				throw new ArgumentException($"Date is not valid. It must be in format {Messages.DateFormat}");
			}

			var machine = new Machine()
			{				
				Name = model.Name,
				CalibrationSchedule = calibrationDate,
				Capacity = model.Capacity,
				IsCalibrated = isCalibrated
			};

			await _context.AddAsync(machine);
			await _context.SaveChangesAsync();
		}

		public async Task<MachineServiceModel?> GetMachineForEditAsync(int id)
		{
			var machineForEdit = await _context.Machines
				.Where(m => m.Id == id)
				.FirstOrDefaultAsync();

			if (machineForEdit == null)
			{
				throw new ArgumentException($"{Messages.InvalidIdInput}");
			}

			return await _context.Machines
				.Where(m => m.Id == id)
				.Select(m => new MachineServiceModel()
				{
					Id = m.Id,
					CalibrationSchedule = m.CalibrationSchedule.ToString(Messages.DateFormat, CultureInfo.InvariantCulture),
					Capacity = m.Capacity,
					Name = m.Name,
					IsCalibrated = m.IsCalibrated.ToString()
				})
				.FirstOrDefaultAsync();
		}

		public async Task EditMachineAsync(MachineServiceModel model, int id)
		{
			if (!bool.TryParse(model.IsCalibrated, out bool isCalibratedResult))
			{
				throw new ArgumentException($"{Messages.BooleanInput}");
			}

			var machineForEdit = await _context.Machines.FindAsync(id);

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
					throw new ArgumentException($"Date format must be {Messages.DateFormat}");
				}

				machineForEdit.Name = model.Name;
				machineForEdit.CalibrationSchedule = date;
				machineForEdit.Capacity = model.Capacity;
				machineForEdit.IsCalibrated = isCalibratedResult;

				await _context.SaveChangesAsync();
			}
		}
	}
}
