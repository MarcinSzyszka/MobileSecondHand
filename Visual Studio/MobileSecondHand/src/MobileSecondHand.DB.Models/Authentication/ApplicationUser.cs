using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MobileSecondHand.DB.Models.Advertisement;
using MobileSecondHand.DB.Models.Chat;

namespace MobileSecondHand.DB.Models.Authentication
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
		[Key]
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
		public List<AdvertisementItem> AdvertisementItems { get; set; } = new List<AdvertisementItem>();
		public List<UserToConversation> Conversations { get; set; } = new List<UserToConversation>();
		public List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
	}
}
