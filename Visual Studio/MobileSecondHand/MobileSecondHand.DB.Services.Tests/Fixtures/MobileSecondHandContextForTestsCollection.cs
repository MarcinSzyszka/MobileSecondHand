using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MobileSecondHand.DB.Services.Tests.Fixtures
{
	[CollectionDefinition("MobileSecondHandContextForTestsCollection")]
	public class MobileSecondHandContextForTestsCollection : ICollectionFixture<MobileSecondHandContextForTestsFixture>
	{
	}
}
