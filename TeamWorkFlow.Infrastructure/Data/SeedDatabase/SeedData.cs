﻿using TeamWorkFlow.Infrastructure.Data.Models;
using TaskStatus = TeamWorkFlow.Infrastructure.Data.Models.TaskStatus;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class SeedData
    {
        /// <summary>
        /// OperatorAvailabilityStatus seeding fields
        /// </summary>
        public OperatorAvailabilityStatus AtWorkStatus { get; set; }
        public OperatorAvailabilityStatus InSickLeaveStatus { get; set; }
        public OperatorAvailabilityStatus OnVacation { get; set; }
        public OperatorAvailabilityStatus OnTraining { get; set; }

        /// <summary>
        /// PartStatus seeding fields
        /// </summary>
        public PartStatus Released { get; set; }
        public PartStatus NotReleased { get; set; }
        public PartStatus ConditionalReleased { get; set; }

        /// <summary>
        /// Priority seeding fields
        /// </summary>
        public Priority Low { get; set; }
        public Priority Normal { get; set; }
        public Priority High { get; set; }

        /// <summary>
        /// ProjectStatus seeding fields
        /// </summary>
        public ProjectStatus InProduction { get; set; }
        public ProjectStatus InDevelopment { get; set; }
        public ProjectStatus InAcl { get; set; }

        /// <summary>
        /// TaskStatus seeding fields
        /// </summary>
        public TaskStatus Open { get; set; }
        public TaskStatus InProgress { get; set; }
        public TaskStatus Finished { get; set; }
        public TaskStatus Canceled { get; set; }

        /// <summary>
        /// Machine seeding fields 
        /// </summary>
        public Machine ZeissConturaOne { get; set; }
        public Machine ZeissInspect { get; set; }
        public Machine ZeissMetrotom { get; set; }

		/// <summary>
		/// Project seeding fields
		/// </summary>
        public Project BmwHousingGx9 { get; set; }
        public Project VwFrontPanel { get; set; }
        public Project ToyotaClimaticModule { get; set; }

	
		public SeedData()
        {
            SeedOperatorAvailabilityStatus();
            SeedPartStatus();
            SeedPriority();
            SeedProjectStatus();
            SeedTaskStatus();
            SeedMachine();
            SeedProject();
        }

        private void SeedProject()
        {
	        BmwHousingGx9 = new Project()
	        {
                Id = 1,
                Appliance = "Automotive industry",
                ClientName = "Bmw",
                ProjectName = "BMW Housing Gx9",
                ProjectNumber = "249100",
                ProjectStatusId = 1,
                TotalHoursSpent = 50
	        };

	        VwFrontPanel = new Project()
	        {
		        Id = 2,
		        Appliance = "Automotive industry",
		        ClientName = "Vw",
		        ProjectName = "Vw Tuareg Front panel ",
		        ProjectNumber = "249200",
		        ProjectStatusId = 2,
		        TotalHoursSpent = 20
	        };

	        ToyotaClimaticModule = new Project()
	        {
		        Id = 3,
		        Appliance = "Automotive industry",
		        ClientName = "Toyota",
		        ProjectName = "Toyota Climatic module X5",
		        ProjectNumber = "249300",
		        ProjectStatusId = 3,
		        TotalHoursSpent = 41
	        };
		}
        private void SeedMachine()
        {
			ZeissConturaOne = new Machine()
	        {
		        Id = 1,
		        Name = "Zeiss Contura",
		        CalibrationSchedule = new DateTime(2024,04,04),
		        Capacity = 20,
                TotalMachineLoad = 0
	        };

	        ZeissInspect = new Machine()
	        {
		        Id = 2,
		        Name = "Zeiss O-inspect",
		        CalibrationSchedule = new DateTime(2024, 04, 04),
		        Capacity = 20,
                TotalMachineLoad = 0
	        };

	        ZeissMetrotom = new Machine()
	        {
		        Id = 3,
		        Name = "Zeiss Metrotom",
		        CalibrationSchedule = new DateTime(2024, 04, 04),
		        Capacity = 20,
                TotalMachineLoad = 0
	        };

		}
        private void SeedTaskStatus()
        {
            Open = new TaskStatus()
            {
                Id = 1,
                Name = "open"
            };

            InProgress = new TaskStatus()
            {
                Id = 2,
                Name = "in progress"
            };

            Finished = new TaskStatus()
            {
                Id = 3,
                Name = "finished"
            };

            Canceled = new TaskStatus()
            {
                Id = 4,
                Name = "canceled"
            };
        }
        private void SeedProjectStatus()
        {
            InProduction = new ProjectStatus()
            {
                Id = 1,
                Name = "In production"
            };

            InDevelopment = new ProjectStatus()
            {
                Id= 2, 
                Name = "In development"
            };
            InAcl = new ProjectStatus()
            {
                Id = 3,
                Name = "in ACL"
            };
        }
        private void SeedPriority()
        {
            Low = new Priority()
            {
                Id = 1, 
                Name = "low"
            };

            Normal = new Priority()
            {
                Id = 2,
                Name = "normal"
            };

            High = new Priority()
            {
                Id = 3,
                Name = "high"
            };
        }
        private void SeedPartStatus()
        {
            Released = new PartStatus()
            {
                Id = 1, 
                Name = "released"
            };

            NotReleased = new PartStatus()
            {
                Id = 2, 
                Name = "not released"
            };

            ConditionalReleased = new PartStatus()
            {
                Id = 3,
                Name = "Conditional released"
            };
        }
        private void SeedOperatorAvailabilityStatus()
        {
            AtWorkStatus = new OperatorAvailabilityStatus()
            {
                Id = 1,
                Name = "at work"
            };

            InSickLeaveStatus = new OperatorAvailabilityStatus()
            {
                Id = 2,
                Name = "in sick leave"
            };

            OnVacation = new OperatorAvailabilityStatus()
            {
                Id = 3,
                Name = "on vacation"
            };

            OnTraining = new OperatorAvailabilityStatus()
            {
                Id = 4,
                Name = "on training"
            };
        }


    }
}
