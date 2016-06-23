using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobileSecondHand.DB.Services.Chat;

namespace MobileSecondHand.DB.Services.Tests.Fixtures
{
	public class MobileSecondHandContextForTestsFixture : IDisposable
	{
		public IMobileSecondHandContextOptions MobileSecondHandContextOptions { get; private set; }
		public MobileSecondHandContext DbContext { get; private set; }
		public MobileSecondHandContextForTestsFixture()
		{
			MobileSecondHandContextOptions = new MobileSecondHandContextOptions(new ConnectionStringConfig() { ConnectionString = "Server=.\\SQLEXPRESS;Database=MobileSecondHandNew_TESTS;Integrated Security=SSPI;Trusted_Connection=True;MultipleActiveResultSets=true" });
			
			DbContext = new MobileSecondHandContext(MobileSecondHandContextOptions.DbContextOptions);
			DbContext.Database.EnsureCreated();
		}

		public void Dispose()
		{
			DbContext.Database.EnsureDeleted();
		}
	}
}
