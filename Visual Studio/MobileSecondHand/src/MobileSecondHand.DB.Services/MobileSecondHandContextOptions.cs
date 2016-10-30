using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobileSecondHand.COMMON;
using MobileSecondHand.DB.Models;

namespace MobileSecondHand.DB.Services {
	public class MobileSecondHandContextOptions : IMobileSecondHandContextOptions {
		public DbContextOptions<MobileSecondHandContext> DbContextOptions { get; }

		public MobileSecondHandContextOptions(ConnectionStringConfig connectionStringConfig) {
			var builder = new DbContextOptionsBuilder<MobileSecondHandContext>().UseSqlServer(connectionStringConfig.ConnectionString);
			DbContextOptions = builder.Options;
		}
	}
}
