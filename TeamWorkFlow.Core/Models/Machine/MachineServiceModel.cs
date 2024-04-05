namespace TeamWorkFlow.Core.Models.Machine
{
    public class MachineServiceModel
    {
	    public int Id { get; set; }
	    public string Name { get; set; } = string.Empty;
		public string CalibrationSchedule { get; set; } = string.Empty;
	    public bool IsCalibrated { get; set; }
		public int Capacity { get; set; }
    }
}
