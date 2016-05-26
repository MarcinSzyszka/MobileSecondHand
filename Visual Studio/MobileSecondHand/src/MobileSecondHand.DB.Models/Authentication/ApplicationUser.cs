using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Models.Authentication
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
		public override string Id
		{
			get
			{
				return base.Id;
			}

			set
			{
				base.Id = value;
			}
		}
		public override string UserName
		{
			get
			{
				return base.UserName;
			}

			set
			{
				base.UserName = value;
			}
		}

		public override string Email
		{
			get
			{
				return base.Email;
			}

			set
			{
				base.Email = value;
			}
		}
		public virtual ICollection<AdvertisementItem> AdvertisementItems { get; set; }
	}
}
