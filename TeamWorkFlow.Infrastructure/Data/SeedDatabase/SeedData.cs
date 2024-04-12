using TeamWorkFlow.Infrastructure.Data.Models;
using Task = TeamWorkFlow.Infrastructure.Data.Models.Task;
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
		/// Operator seeding fields
		/// </summary>
		public Operator WorkerOne { get; set; }
		public Operator WorkerTwo { get; set; }
		public Operator WorkerThree { get; set; }

		/// <summary>
		/// PartStatus seeding fields
		/// </summary>
		public PartStatus Released { get; set; }
        public PartStatus NotReleased { get; set; }
        public PartStatus ConditionalReleased { get; set; }

		/// <summary>
		/// Part seeding fields
		/// </summary>
		public Part HousingOne { get; set; }
		public Part HousingTwo { get; set; }
		public Part HousingThree { get; set; }
		public Part HousingFour { get; set; }

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

        /// <summary>
        /// Task seeding fields
        /// </summary>
        public Task TaskOne { get; set; }
        public Task TaskTwo { get; set; }
        public Task TaskThree { get; set; }
        public Task TaskFour { get; set; }


		public SeedData()
        {
            SeedOperatorAvailabilityStatus();
            SeedOperator();
            SeedPart();
            SeedPartStatus();
            SeedPriority();
            SeedProjectStatus();
            SeedTaskStatus();
            SeedTask();
            SeedMachine();
            SeedProject();
        }

        private void SeedTask()
        {
	        TaskOne = new Task()
	        {
		        Id = 4,
		        Name = "Housing Front Panel - LOP.",
		        Description =
			        "LOP dimensional report for phase 1 (T0) - samples from the tool maker should arrive in Calendar week 48.",
		        StartDate = new DateTime(2023, 11, 03),
		        TaskStatusId = 1,
		        PriorityId = 2,
		        CreatorId = "e733b261-0a9c-45eb-ad97-cc611c83e2dd",
		        DeadLine = new DateTime(2023, 12, 12),
		        EstimatedTime = 2,
		        MachineId = 1,
		        ProjectId = 2
	        };

	        TaskTwo = new Task()
	        {
		        Id = 15,
		        Name = "Housing Klima - PPAP",
		        Description = "PPAP level 3",
		        StartDate = new DateTime(2024, 06, 06),
		        TaskStatusId = 2,
		        PriorityId = 2,
		        CreatorId = "e733b261-0a9c-45eb-ad97-cc611c83e2dd",
		        DeadLine = new DateTime(2024, 07, 07),
		        EstimatedTime = 0,
		        ProjectId = 3
	        };

	        TaskThree = new Task()
	        {
		        Id = 17,
		        Name = "Housing D8 - PPAP",
		        Description = "Full PPAP documents need to be created and prepared for sending to customer no late than 07.07.2024.",
		        StartDate = new DateTime(2024, 06, 06),
		        TaskStatusId = 2,
		        PriorityId = 2,
		        CreatorId = "e733b261-0a9c-45eb-ad97-cc611c83e2dd",
		        DeadLine = new DateTime(2024, 07, 07),
		        EstimatedTime = 0,
		        ProjectId = 2
	        };

	        TaskFour = new Task()
	        {
		        Id = 18,
		        Name = "BMW Back Panel - Sample order no. 987",
		        Description = "Validation of the part on another production machine. Full dimensional report of 5 shots from the new machine. Results must be compared with measurements of the part from the serial (validated) production machine",
		        StartDate = new DateTime(2024, 07, 18),
		        TaskStatusId = 1,
		        PriorityId = 2,
		        CreatorId = "e733b261-0a9c-45eb-ad97-cc611c83e2dd",
		        EstimatedTime = 0,
		        ProjectId = 1
	        };
		}
        private void SeedPart()
        {
	        HousingOne = new Part()
	        {
		        Id = 1,
		        Name = "VW Housing Front D9",
		        PartArticleNumber = "2.4.100.501",
		        PartClientNumber = "252.166-15",
		        ToolNumber = 9055,
		        PartStatusId = 2,
		        ProjectId = 2,
		        ImageUrl =
			        "https://www.preh.com/fileadmin/templates/website/media/images/Produkte/Car_HMI/Climate_Control/Preh_Produkte_Climate_Control_AudiA1.jpg",
		        PartModel = "252.166-15_0B_VW Housing Front D9"
	        };

	        HousingTwo = new Part()
	        {
		        Id = 2,
		        Name = "VW Housing D8",
		        PartArticleNumber = "2.4.100.502",
		        PartClientNumber = "252.167-00",
		        ToolNumber = 3418,
		        PartStatusId = 2,
		        ProjectId = 2,
		        ImageUrl =
					"https://wodofogdr.com/cdn/shop/products/GDR-MBT-823287-2_grande.jpg?v=1626163358",
		        PartModel = "252.167-00_0D_VW Housing D8"
			};

	        HousingThree = new Part()
	        {
		        Id = 3,
		        Name = "Audi Housing A5 X-line",
		        PartArticleNumber = "2.4.100.605",
		        PartClientNumber = "312.205-11",
		        ToolNumber = 3459,
		        PartStatusId = 1,
		        ProjectId = 2,
		        ImageUrl =
					"https://wodofogdr.com/cdn/shop/products/GDR-MBT-823287-2_grande.jpg?v=1626163358",
		        PartModel = "334.255-10_0E_Audi Housing A5 X-line"
			};

	        HousingFour = new Part()
	        {
		        Id = 4,
		        Name = "Toyota Housing F5",
		        PartArticleNumber = "2.4.202.333",
		        PartClientNumber = "212.200-00",
		        ToolNumber = 5533,
		        PartStatusId = 3,
		        ProjectId = 3,
		        ImageUrl =
					"https://www.bhtc.com/media/pages/produkte/fahrzeugklimatisierung/bmw-klimabediengerat/3086657772-1542633776/bmw_klimabediengeraet_gkl.png",
		        PartModel = "212.200-00_0B_Toyota Housing F5"
			};
		}
        private void SeedOperator()
        {
	        WorkerOne = new Operator()
	        {
		        Id = 1,
		        FullName = "Aleksandar Paytalov",
		        AvailabilityStatusId = 4,
		        Email = "ap.softuni@gmail.bg",
		        PhoneNumber = "+359881234567",
		        IsActive = true,
		        Capacity = 8
	        };

	        WorkerTwo = new Operator()
	        {
		        Id = 2,
		        FullName = "Jon Doe",
		        AvailabilityStatusId = 1,
		        Email = "JonDoe@softuni.bg",
		        PhoneNumber = "+359887654321",
		        IsActive = true,
		        Capacity = 4
	        };

	        WorkerThree = new Operator()
	        {
		        Id = 3,
		        FullName = "Jane Doe",
		        AvailabilityStatusId = 2,
		        Email = "janeDoe@softuni.bg",
		        PhoneNumber = "+359894567890",
		        IsActive = false,
		        Capacity = 8
	        };
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
                TotalMachineLoad = 0,
                ImageUrl = "https://www.researchgate.net/profile/Nermina_Zaimovic-Uzunovic2/publication/343880067/figure/fig2/AS:928740968255491@1598440510374/Measurement-of-the-top-surface-Fig4-CMM-Zeiss-Contura-G2_Q320.jpg"
			};

	        ZeissInspect = new Machine()
	        {
		        Id = 2,
		        Name = "Zeiss O-inspect",
		        CalibrationSchedule = new DateTime(2024, 04, 04),
		        Capacity = 20,
                TotalMachineLoad = 0,
                ImageUrl = "https://www.qpluslabs.com/wp-content/uploads/2019/11/Zeiss-O-Inspect-863-475px.jpg"
			};

	        ZeissMetrotom = new Machine()
	        {
		        Id = 3,
		        Name = "Zeiss Metrotom",
		        CalibrationSchedule = new DateTime(2024, 04, 04),
		        Capacity = 20,
                TotalMachineLoad = 0,
                ImageUrl = "https://i0.wp.com/metrology.news/wp-content/uploads/2023/02/ZEISS-METROTOM-1.jpg?resize=450%2C404"
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
