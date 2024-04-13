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
            SeedProject();
            SeedUsers();
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
