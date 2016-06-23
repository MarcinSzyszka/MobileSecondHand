using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MobileSecondHand.DB.Services {
	public interface IMobileSecondHandContextOptions {
		DbContextOptions<MobileSecondHandContext> DbContextOptions { get; }
	}
}
