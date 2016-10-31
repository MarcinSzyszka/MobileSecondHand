using Microsoft.EntityFrameworkCore;

namespace MobileSecondHand.DB.Models
{
	public class MobileSecondHandContextOptions : IMobileSecondHandContextOptions {
		public DbContextOptions<MobileSecondHandContext> DbContextOptions { get; }

		public MobileSecondHandContextOptions(ConnectionStringConfig connectionStringConfig) {
			var builder = new DbContextOptionsBuilder<MobileSecondHandContext>().UseSqlServer(connectionStringConfig.ConnectionString);
			DbContextOptions = builder.Options;
		}
	}
}
