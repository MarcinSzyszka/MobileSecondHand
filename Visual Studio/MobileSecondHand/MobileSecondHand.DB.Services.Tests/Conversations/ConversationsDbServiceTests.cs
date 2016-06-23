using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Chat;
using MobileSecondHand.DB.Models.Authentication;
using MobileSecondHand.DB.Services.Chat;
using MobileSecondHand.DB.Services.Tests.Fixtures;
using Xunit;

namespace MobileSecondHand.DB.Services.Tests.Conversations
{
	[Collection("MobileSecondHandContextForTestsCollection")]
	public class ConversationsDbServiceTests
	{
		[Fact]
		public void CreateConversation_ConversationIsCreatingAndReturningWithRelatedUsersProperty()
		{
			//Arrange
			var firstUser = new ApplicationUser { UserName = "First User" };
			var secondUser = new ApplicationUser { UserName = "Second User" };
			this.fixture.DbContext.ApplicationUser.Add(firstUser);
			this.fixture.DbContext.Add(secondUser);
			this.fixture.DbContext.SaveChanges();

			//Act
			var conversation = serviceUnderTest.CreateConversation(firstUser.Id, secondUser.Id);

			//Assert
			Assert.NotNull(conversation);
			Assert.True(conversation.ConversationId > 0);
			Assert.NotEmpty(conversation.Users);
			Assert.Equal(2, conversation.Users.Count);
			Assert.Equal(firstUser.Id, conversation.Users[0].UserId);
			Assert.Equal(secondUser.Id, conversation.Users[1].UserId);
			Assert.Equal(2, this.fixture.DbContext.ApplicationUser.Where(a => a.Id == firstUser.Id || a.Id == secondUser.Id).Count());

		}

		[Fact]
		public void GetConversationByUsers_ReturnConversationBetweenUsers_ByTheirIds()
		{
			//Arrange
			var firstUser = new ApplicationUser { UserName = "First User" };
			var secondUser = new ApplicationUser { UserName = "Second User" };
			this.fixture.DbContext.ApplicationUser.Add(firstUser);
			this.fixture.DbContext.Add(secondUser);

			var conversation = new Conversation();
			conversation.Users.Add(new UserToConversation { Conversation = conversation, UserId = firstUser.Id });
			conversation.Users.Add(new UserToConversation { Conversation = conversation, UserId = secondUser.Id });
			this.fixture.DbContext.Conversation.Add(conversation);
			this.fixture.DbContext.SaveChanges();

			//Act
			var conversationResult = this.serviceUnderTest.GetConversationByUsers(firstUser.Id, secondUser.Id);

			//Assert
			Assert.NotNull(conversationResult);
			Assert.Equal(conversation.ConversationId, conversationResult.ConversationId);
			Assert.Equal(2, conversationResult.Users.Count);
		}

		[Fact]
		public void GetMessagesInConversation_ReturnedMessagesHasIncludedAuthor()
		{
			//Arrange
			var pageNumber = 0;
			var user = new ApplicationUser { UserName = "First User" };
			this.fixture.DbContext.ApplicationUser.Add(user);
			var messageDate = new DateTime(2000, 01, 01);
			var conversation = new Conversation();
			conversation.Messages.Add(new ChatMessage { Author = user, Date = messageDate });
			this.fixture.DbContext.Conversation.Add(conversation);
			this.fixture.DbContext.SaveChanges();

			//Act
			var messages = this.serviceUnderTest.GetMessagesInConversation(conversation.ConversationId, pageNumber);

			//Assert
			Assert.NotNull(messages[0].Author);
			Assert.Equal(user.Id, messages[0].Author.Id);
		}

		[Fact]
		public void GetMessagesInConversation_PageNumberIs0_Return10NewestMessages()
		{
			//Arrange
			var pageNumber = 0;
			var user = new ApplicationUser { UserName = "First User" };
			this.fixture.DbContext.ApplicationUser.Add(user);
			var messageDate = new DateTime(2000, 01, 01);
			var conversation = new Conversation();
			for (int i = 0; i < 10; i++)
			{
				conversation.Messages.Add(new ChatMessage { Author = user, Date = messageDate.AddDays(i) });
			}
			this.fixture.DbContext.Conversation.Add(conversation);
			this.fixture.DbContext.SaveChanges();

			//Act
			var messages = this.serviceUnderTest.GetMessagesInConversation(conversation.ConversationId, pageNumber);

			//Assert
			Assert.Equal(10, messages.Count);
			Assert.Equal(new DateTime(2000, 01, 10), messages[0].Date);
			Assert.Equal(new DateTime(2000, 01, 01), messages[9].Date);
		}

		[Fact]
		public void GetMessagesInConversation_PageNumberIs1_Skip20MessagesAndReturn20NextOnesNewestMessages()
		{
			//Arrange
			var pageNumber = 1;
			var user = new ApplicationUser { UserName = "First User" };
			this.fixture.DbContext.ApplicationUser.Add(user);
			var messageDate = new DateTime(2000, 01, 01);
			var conversation = new Conversation();
			for (int i = 0; i < 40; i++)
			{
				conversation.Messages.Add(new ChatMessage { Author = user, Date = messageDate.AddDays(i) });
			}
			this.fixture.DbContext.Conversation.Add(conversation);
			this.fixture.DbContext.SaveChanges();

			//Act
			var messages = this.serviceUnderTest.GetMessagesInConversation(conversation.ConversationId, pageNumber);

			//Assert
			Assert.Equal(20, messages.Count);
			Assert.Equal(new DateTime(2000, 01, 20), messages[0].Date);
			Assert.Equal(new DateTime(2000, 01, 01), messages[19].Date);
		}
		#region CONFIGURATION
		MobileSecondHandContextForTestsFixture fixture;
		IConversationDbService serviceUnderTest;
		public ConversationsDbServiceTests(MobileSecondHandContextForTestsFixture fixture)
		{
			this.fixture = fixture;
			serviceUnderTest = new ConversationDbService(this.fixture.MobileSecondHandContextOptions);
		}
		#endregion
	}
}
