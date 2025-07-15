using Microsoft.AspNetCore.Identity;
using TeamWorkFlow.Infrastructure.Data.Models;
using TaskStatus = TeamWorkFlow.Infrastructure.Data.Models.TaskStatus;
using static TeamWorkFlow.Infrastructure.Constants.CustomClaimsConstants;
using Task = TeamWorkFlow.Infrastructure.Data.Models.Task;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class SeedData
    {
        /// <summary>
        /// OperatorAvailabilityStatus seeding fields
        /// </summary>
        public OperatorAvailabilityStatus AtWorkStatus { get; set; } = null!;
        public OperatorAvailabilityStatus InSickLeaveStatus { get; set; } = null!;
        public OperatorAvailabilityStatus OnVacation { get; set; } = null!;
        public OperatorAvailabilityStatus OnTraining { get; set; } = null!;

        /// <summary>
        /// Operator seeding fields
        /// </summary>
        public Operator OperatorOne { get; set; } = null!;
        public Operator OperatorTwo { get; set; } = null!;
        public Operator OperatorThree { get; set; } = null!;

		/// <summary>
		/// PartStatus seeding fields
		/// </summary>
		public PartStatus Released { get; set; } = null!;
        public PartStatus NotReleased { get; set; } = null!;
        public PartStatus ConditionalReleased { get; set; } = null!;

        /// <summary>
        /// Part seeding fields
        /// </summary>
        public Part PartOne { get; set; } = null!;
        public Part PartTwo { get; set; } = null!;
        public Part PartThree { get; set; } = null!;
        public Part PartFour { get; set; } = null!;
        public Part PartFive { get; set; } = null!;
        public Part PartSix { get; set; } = null!;
        public Part PartSeven { get; set; } = null!;
        public Part PartEight { get; set; } = null!;


		/// <summary>
		/// Priority seeding fields
		/// </summary>
		public Priority Low { get; set; } = null!;
        public Priority Normal { get; set; } = null!;
        public Priority High { get; set; } = null!;

        /// <summary>
        /// ProjectStatus seeding fields
        /// </summary>
        public ProjectStatus InProduction { get; set; } = null!;
        public ProjectStatus InDevelopment { get; set; } = null!;
        public ProjectStatus InAcl { get; set; } = null!;

        /// <summary>
        /// TaskStatus seeding fields
        /// </summary>
        public TaskStatus Open { get; set; } = null!;
        public TaskStatus InProgress { get; set; } = null!;
        public TaskStatus Finished { get; set; } = null!;
        public TaskStatus Canceled { get; set; } = null!;

        /// <summary>
        /// Task seeding fields
        /// </summary>
		public Task TaskOne { get; set; } = null!;
		public Task TaskTwo { get; set; } = null!;
		public Task TaskThree { get; set; } = null!;
		public Task TaskFour { get; set; } = null!;
		public Task TaskFive { get; set; } = null!;
		public Task TaskSix { get; set; } = null!;

		/// <summary>
		/// Machine seeding fields
		/// </summary>
		public Machine ZeissConturaOne { get; set; } = null!;
        public Machine ZeissInspect { get; set; } = null!;
        public Machine ZeissMetrotom { get; set; } = null!;
        public Machine MachineFour { get; set; } = null!;
		public Machine MachineFive { get; set; } = null!;
		public Machine MachineSix { get; set; } = null!;

		/// <summary>
		/// Project seeding fields
		/// </summary>
		public Project BmwHousingGx9 { get; set; } = null!;
        public Project VwFrontPanel { get; set; } = null!;
        public Project ToyotaClimaticModule { get; set; } = null!;

        /// <summary>
        /// Project seeding fields
        /// </summary>
        public IdentityUser GuestUser { get; set; } = null!;
        public IdentityUser OperatorUser { get; set; } = null!;
        public IdentityUser AdminUser { get; set; } = null!;
        public IdentityUserClaim<string> GuestUserClaim { get; set; } = null!;
        public IdentityUserClaim<string> OperatorUserClaim { get; set; } = null!;
        public IdentityUserClaim<string> AdminUserClaim { get; set; } = null!;

		/// <summary>
		/// TaskOperator seeding fields
		/// </summary>
		public TaskOperator TaskOperatorOne { get; set; } = null!;
		public TaskOperator TaskOperatorTwo { get; set; } = null!;
		public TaskOperator TaskOperatorThree { get; set; } = null!;

		public SeedData()
        {
            SeedOperatorAvailabilityStatus();
            SeedPartStatus();
            SeedPriority();
            SeedProjectStatus();
            SeedTaskStatus();
            SeedMachine();
            SeedPart();
            SeedProject();
            SeedUsers();
            SeedOperator();
			SeedTask();
			SeedTaskOperator();
        }


        private void SeedTaskOperator()
        {
	        TaskOperatorOne = new TaskOperator()
	        {
		        OperatorId = 1,
		        TaskId = 1
	        };

	        TaskOperatorTwo = new TaskOperator()
	        {
		        OperatorId = 1,
		        TaskId = 6
	        };

	        TaskOperatorThree = new TaskOperator()
	        {
		        OperatorId = 2,
		        TaskId = 2
	        };
        }
		private void SeedPart()
        {
	        PartOne = new Part()
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

	        PartTwo = new Part()
	        {
		        Id = 2,
		        Name = "VW Housing D8",
		        PartArticleNumber = "2.4.100.502",
		        PartClientNumber = "252.167-00",
		        ToolNumber = 3418,
		        PartStatusId = 2,
		        ProjectId = 2,
		        ImageUrl = "https://wodofogdr.com/cdn/shop/products/GDR-MBT-823287-2_grande.jpg?v=1626163358",
		        PartModel = "252.167-00_0D_VW Housing D8"
	        };

	        PartThree = new Part()
	        {
		        Id = 3,
		        Name = "Audi Housing A5 X-line",
		        PartArticleNumber = "2.4.100.605",
		        PartClientNumber = "312.205-11",
		        ToolNumber = 3459,
		        PartStatusId = 1,
		        ProjectId = 2,
		        ImageUrl = "https://wodofogdr.com/cdn/shop/products/GDR-MBT-823287-2_grande.jpg?v=1626163358",
		        PartModel = "334.255-10_0E_Audi Housing A5 X-line"
	        };

	        PartFour = new Part()
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

	        PartFive = new Part()
	        {
		        Id = 5,
		        Name = "BMW Front-Back Panels X5",
		        PartArticleNumber = "2.3.105.603",
		        PartClientNumber = "212.200-11",
		        ToolNumber = 3360,
		        PartStatusId = 3,
		        ProjectId = 1,
		        ImageUrl = "https://conti-engineering.com/wp-content/uploads/2020/09/climatecontrol_beitrag.jpg",
		        PartModel = "212.200-11_0E_BMW Front-Back Panels X5"
	        };

	        PartSix = new Part()
	        {
		        Id = 6,
		        Name = "VW Tuareg Housing panel G5",
		        PartArticleNumber = "2.4.305.777",
		        PartClientNumber = "431.222-07",
		        ToolNumber = 2515,
		        PartStatusId = 1,
		        ProjectId = 2,
		        ImageUrl =
			        "https://www.preh.com/fileadmin/templates/website/media/images/Produkte/Car_HMI/Climate_Control/Preh_Produkte_Climate_Control_FordFocus.jpg",
		        PartModel = "431.222-07_0A_VW Tuareg Housing panel G5"
	        };

	        PartSeven = new Part()
	        {
		        Id = 7,
		        Name = "Toyota Aventis Housing Klima module V6",
		        PartArticleNumber = "2.4.105.589",
		        PartClientNumber = "305.201-11",
		        ToolNumber = 9999,
		        PartStatusId = 1,
		        ProjectId = 3,
		        ImageUrl =
			        "https://www.preh.com/fileadmin/templates/website/media/images/Produkte/Car_HMI/Climate_Control/Preh_Produkte_Climate_Control_AudiR8.jpg",
		        PartModel = "305.201-11_0B_Toyota Aventis Housing Klima module V6"
	        };

	        PartEight = new Part()
	        {
		        Id = 8,
		        Name = "VW Light Conductor Front Panel",
		        PartArticleNumber = "2.4.222.777",
		        PartClientNumber = "213.891-22",
		        ToolNumber = 9995,
		        PartStatusId = 1,
		        ProjectId = 2,
		        ImageUrl =
			        "https://autoprotoway.com/wp-content/uploads/2022/09/precision-automotive-lighting-parts.jpg",
		        PartModel = "213.891-22_0T_VW Light Conductor Front Panel"
	        };
        }
        private void SeedUsers()
        {
	        var hasher = new PasswordHasher<IdentityUser>();

            //Admin
	        AdminUser = new IdentityUser()
	        {
		        Id = "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
		        UserName = "admin@test.local",
		        NormalizedUserName = "ADMIN@TEST.LOCAL",
		        Email = "admin@test.local",
		        NormalizedEmail = "ADMIN@TEST.LOCAL"
	        };

	        AdminUserClaim = new IdentityUserClaim<string>()
	        {
		        Id = 1,
		        ClaimType = UserName,
		        ClaimValue = "admin@test.local",
		        UserId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77"
	        };

	        AdminUser.PasswordHash = hasher.HashPassword(AdminUser, "TestPass123!");

            //Operator
            OperatorUser = new IdentityUser()
            {
	            Id = "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
	            UserName = "operator@test.local",
	            NormalizedUserName = "OPERATOR@TEST.LOCAL",
	            Email = "operator@test.local",
                NormalizedEmail = "OPERATOR@TEST.LOCAL"
			};

            OperatorUserClaim = new IdentityUserClaim<string>()
            {
	            Id = 2,
	            ClaimType = UserName,
	            ClaimValue = "operator@test.local",
	            UserId = "7bf9623c-54d9-45ba-84c6-52806dcee7bd"
            };

            OperatorUser.PasswordHash = hasher.HashPassword(OperatorUser, "TestPass456!");

            //Guest
            GuestUser = new IdentityUser()
            {
	            Id = "b806eee6-2ceb-4956-9643-e2e2e82289d2",
	            UserName = "guest@test.local",
	            NormalizedUserName = "GUEST@TEST.LOCAL",
	            Email = "guest@test.local",
	            NormalizedEmail = "GUEST@TEST.LOCAL"
            };

            GuestUserClaim = new IdentityUserClaim<string>()
            {
	            Id = 3,
	            ClaimType = UserName,
	            ClaimValue = "guest@test.local",
	            UserId = "b806eee6-2ceb-4956-9643-e2e2e82289d2"
            };

            GuestUser.PasswordHash = hasher.HashPassword(GuestUser, "TestPass789!");
        }
        private void SeedOperator()
        {
            OperatorOne = new Operator()
            {
                Id = 1,
                FullName = "Test Admin",
                AvailabilityStatusId = 4,
                Email = "admin@test.local",
                PhoneNumber = "+1234567890",
                IsActive = true,
                Capacity = 8,
				UserId = AdminUser.Id
            };

            OperatorTwo = new Operator()
            {
                Id = 2,
                FullName = "Test Operator",
                AvailabilityStatusId = 1,
                Email = "operator@test.local",
                PhoneNumber = "+1234567891",
                IsActive = true,
                Capacity = 4,
				UserId = OperatorUser.Id
            };

            OperatorThree = new Operator()
            {
                Id = 3,
                FullName = "Test Guest",
                AvailabilityStatusId = 2,
                Email = "guest@test.local",
                PhoneNumber = "+1234567892",
                IsActive = true,
                Capacity = 8,
				UserId = GuestUser.Id
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
		        Capacity = 24,
                TotalMachineLoad = 0,
                ImageUrl = "https://www.researchgate.net/profile/Nermina_Zaimovic-Uzunovic2/publication/343880067/figure/fig2/AS:928740968255491@1598440510374/Measurement-of-the-top-surface-Fig4-CMM-Zeiss-Contura-G2_Q320.jpg"
			};

	        ZeissInspect = new Machine()
	        {
		        Id = 2,
		        Name = "Zeiss O-inspect",
		        CalibrationSchedule = new DateTime(2024, 04, 04),
		        Capacity = 24,
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

	        MachineFour = new Machine()
	        {
		        Id = 4,
		        Name = "Zeiss X-ray",
		        Capacity = 16,
		        CalibrationSchedule = new DateTime(2024, 06, 06),
		        TotalMachineLoad = 0,
		        IsCalibrated = true,
		        ImageUrl =
			        "https://www.zeiss.com/content/dam/metrology/products/systems/ct/bosello-new/bosello-sre-max.jpg"
	        };

	        MachineFive = new Machine()
	        {
		        Id = 5,
		        Name = "Mitutoyo Scan",
		        Capacity = 20,
		        CalibrationSchedule = new DateTime(2024, 06, 06),
		        TotalMachineLoad = 0,
		        IsCalibrated = true,
		        ImageUrl = "https://measuremetrology.com/wp-content/uploads/2023/03/mitutoyobrightapex504.png"
	        };

	        MachineSix = new Machine()
	        {
				Id = 6,
				Name = "Zeiss Microscope E9000",
				Capacity = 11,
				CalibrationSchedule = new DateTime(2024,10,10),
				TotalMachineLoad = 0,
				IsCalibrated = true,
				ImageUrl = "https://www.micro-shop.zeiss.com/data/image/shop-catalog-system/group_6038.jpg"
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
        private void SeedTask()
        {
	        TaskOne = new Task()
	        {
		        Id = 1,
		        Name = "Housing Front Panel - LOP.",
		        Description =
			        "LOP dimensional report for phase 1 (T0) - samples from the tool maker should arrive in Calendar week 48.",
		        StartDate = new DateTime(2023, 11, 03),
		        TaskStatusId = 1,
		        PriorityId = 2,
		        CreatorId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
		        DeadLine = new DateTime(2023, 12, 12),
		        EstimatedTime = 25,
		        MachineId = 1,
		        ProjectId = 2
	        };

	        TaskTwo = new Task()
	        {
		        Id = 2,
		        Name = "Housing Klima - PPAP",
		        Description = "PPAP level 3",
		        StartDate = new DateTime(2024, 06, 06),
		        TaskStatusId = 2,
		        PriorityId = 2,
		        CreatorId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
		        DeadLine = new DateTime(2024, 07, 07),
		        EstimatedTime = 32,
		        MachineId = 2,
		        ProjectId = 3
	        };

	        TaskThree = new Task()
	        {
		        Id = 3,
		        Name = "Housing D8 - PPAP",
		        Description =
			        "Full PPAP documents need to be created and prepared for sending to customer no late than 07.07.2024.",
		        StartDate = new DateTime(2024, 06, 06),
		        TaskStatusId = 2,
		        PriorityId = 2,
		        CreatorId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
		        DeadLine = new DateTime(2024, 07, 07),
		        EstimatedTime = 32,
		        ProjectId = 2
	        };

	        TaskFour = new Task()
	        {
		        Id = 4,
		        Name = "BMW Back Panel - Sample order no. 987",
		        Description =
			        "Validation of the part on another production machine. Full dimensional report of 5 shots from the new machine. Results must be compared with measurements of the part from the serial (validated) production machine",
		        StartDate = new DateTime(2024, 07, 18),
		        TaskStatusId = 1,
		        PriorityId = 2,
		        CreatorId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
		        EstimatedTime = 8,
		        ProjectId = 1
	        };

	        TaskFive = new Task()
	        {
		        Id = 5,
		        Name = "BMW Front panel - Sample order No. 954",
		        Description =
			        "Validation of the part on another production machine. Full dimensional report of 5 shots from the new machine. Results must be compared with measurements of the part from the serial (validated) production machine",
		        StartDate = new DateTime(2024, 06, 06),
		        EndDate = new DateTime(2024, 07, 12),
		        TaskStatusId = 3,
		        PriorityId = 1,
		        CreatorId = "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
		        EstimatedTime = 10,
		        MachineId = 2,
		        ProjectId = 1
	        };

	        TaskSix = new Task()
	        {
		        Id = 6,
		        Name = "Housing Klima module V6 - PPAP",
		        Description =
			        "PPAP documents level 3 must be performed. Note: Deviations on dimensions 10 and 150 have been accepted from the customer. Drawing will be adjusted with next PPAP revision",
		        StartDate = new DateTime(2024, 06, 06),
		        EndDate = new DateTime(2024, 06, 12),
		        TaskStatusId = 3,
		        PriorityId = 3,
		        CreatorId = "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
		        DeadLine = new DateTime(2024, 06, 12),
		        EstimatedTime = 16,
		        MachineId = 3,
		        ProjectId = 3
	        };
        }
	}
}
