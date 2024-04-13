using Microsoft.AspNetCore.Identity;
using TeamWorkFlow.Infrastructure.Data.Models;
using TaskStatus = TeamWorkFlow.Infrastructure.Data.Models.TaskStatus;
using static TeamWorkFlow.Infrastructure.Constants.CustomClaimsConstants;

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
        /// Part seeding fields
        /// </summary>
        public Part PartOne { get; set; }
        public Part PartTwo { get; set; }
        public Part PartThree { get; set; }
        public Part PartFour { get; set; }
        public Part PartFive { get; set; }
        public Part PartSix { get; set; }
        public Part PartSeven { get; set; }
        public Part PartEight { get; set; }


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
        /// Project seeding fields
        /// </summary>
        public IdentityUser GuestUser { get; set; }
        public IdentityUser OperatorUser { get; set; }
        public IdentityUser AdminUser { get; set; }
        public IdentityUserClaim<string> GuestUserClaim { get; set; }
        public IdentityUserClaim<string> OperatorUserClaim { get; set; }
        public IdentityUserClaim<string> AdminUserClaim { get; set; }


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
		        UserName = "ap.softuni@gmail.com",
		        NormalizedUserName = "AP.SOFTUNI@GMAIL.COM",
		        Email = "ap.softuni@gmail.com",
		        NormalizedEmail = "AP.SOFTUNI@GMAIL.COM"
	        };

	        AdminUserClaim = new IdentityUserClaim<string>()
	        {
		        Id = 1,
		        ClaimType = UserName,
		        ClaimValue = "ap.softuni@gmail.com",
		        UserId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77"
	        };

	        AdminUser.PasswordHash = hasher.HashPassword(AdminUser, "1234aA!");

            //Operator
            OperatorUser = new IdentityUser()
            {
	            Id = "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
	            UserName = "jon.doe@softuni.bg",
	            NormalizedUserName = "JON.DOE@SOFTUNI.BG",
	            Email = "jon.doe@softuni.bg",
                NormalizedEmail = "JON.DOE@SOFTUNI.BG"
			};

            OperatorUserClaim = new IdentityUserClaim<string>()
            {
	            Id = 2,
	            ClaimType = UserName,
	            ClaimValue = "jon.doe@softuni.bg",
	            UserId = "7bf9623c-54d9-45ba-84c6-52806dcee7bd"
            };

            OperatorUser.PasswordHash = hasher.HashPassword(OperatorUser, "1234bB!");

            //Guest
            GuestUser = new IdentityUser()
            {
	            Id = "b806eee6-2ceb-4956-9643-e2e2e82289d2",
	            UserName = "jane.doe@softuni.bg",
	            NormalizedUserName = "JANE.DOE@SOFTUNI.BG",
	            Email = "jane.doe@softuni.bg",
	            NormalizedEmail = "JANE.DOE@SOFTUNI.BG"
            };

            GuestUserClaim = new IdentityUserClaim<string>()
            {
	            Id = 3,
	            ClaimType = UserName,
	            ClaimValue = "jane.doe@softuni.bg",
	            UserId = "b806eee6-2ceb-4956-9643-e2e2e82289d2"
            };

            GuestUser.PasswordHash = hasher.HashPassword(GuestUser, "1234cC!");
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
