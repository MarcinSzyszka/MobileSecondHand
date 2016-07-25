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
		public void GetConversations_MethodShouldReturnTwoConversationWithLastMessages_InDescByDateOrder()
		{
			//Arrange
			var pageNumber = 0;
			var firstUser = new ApplicationUser { UserName = "First User" };
			var secondUser = new ApplicationUser { UserName = "SecondUser" };
			var conversation = new Conversation();
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = firstUser });
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = secondUser });
			var chatMessageToSave = new ChatMessage { Author = secondUser, Conversation = conversation, Received = false, Date = DateTime.Now };
			var secondMessageToSave = new ChatMessage { Author = secondUser, Conversation = conversation, Received = false, Date = chatMessageToSave.Date.AddMinutes(5) };
			conversation.Messages.Add(chatMessageToSave);
			conversation.Messages.Add(secondMessageToSave);
			this.fixture.DbContext.Conversation.Add(conversation);


			var firstUserOther = new ApplicationUser { UserName = "First User" };
			var secondUserOthe = new ApplicationUser { UserName = "SecondUser" };
			var conversationSecond = new Conversation();
			conversationSecond.Users.Add(new UserToConversation { Conversation = conversationSecond, User = firstUserOther });
			conversationSecond.Users.Add(new UserToConversation { Conversation = conversationSecond, User = secondUserOthe });
			var chatMessageToSaveSecond = new ChatMessage { Author = secondUserOthe, Conversation = conversationSecond, Received = false, Date = DateTime.Now };
			var secondMessageToSaveSecond = new ChatMessage { Author = secondUserOthe, Conversation = conversationSecond, Received = false, Date = chatMessageToSaveSecond.Date.AddMinutes(1) };
			conversationSecond.Messages.Add(chatMessageToSaveSecond);
			conversationSecond.Messages.Add(secondMessageToSaveSecond);
			this.fixture.DbContext.Conversation.Add(conversationSecond);
			this.fixture.DbContext.SaveChanges();


			//Act
			var resultDictionary = serviceUnderTest.GetConversationsWithLastMessage(firstUser.Id, pageNumber);

			//Assert
			Assert.NotNull(resultDictionary);
			Assert.Equal(2, resultDictionary.Count);
			Assert.Equal(conversation.ConversationId, resultDictionary[0].ConversationId);
			Assert.Equal(conversationSecond.ConversationId, resultDictionary[1].ConversationId);
			Assert.Equal(1, resultDictionary[0].Messages.Count);
			Assert.Equal(1, resultDictionary[1].Messages.Count);
			Assert.Equal(secondMessageToSave.ChatMessageId, resultDictionary[0].Messages[0].ChatMessageId);
			Assert.Equal(secondMessageToSaveSecond.ChatMessageId, resultDictionary[1].Messages[0].ChatMessageId);
		}

		[Fact]
		public void GetConversations_MethodShouldReturnConversationWithLastMessage()
		{
			//Arrange
			var pageNumber = 0;
			var firstUser = new ApplicationUser { UserName = "First User" };
			var secondUser = new ApplicationUser { UserName = "SecondUser" };
			var conversation = new Conversation();
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = firstUser });
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = secondUser });
			var chatMessageToSave = new ChatMessage { Author = secondUser, Conversation = conversation, Received = false, Date = DateTime.Now };
			var secondMessageToSave = new ChatMessage { Author = secondUser, Conversation = conversation, Received = false, Date = chatMessageToSave.Date.AddMinutes(1) };
			conversation.Messages.Add(chatMessageToSave);
			conversation.Messages.Add(secondMessageToSave);
			this.fixture.DbContext.Conversation.Add(conversation);
			this.fixture.DbContext.SaveChanges();


			//Act
			var resultDictionary = serviceUnderTest.GetConversationsWithLastMessage(firstUser.Id, pageNumber);

			//Assert
			Assert.NotNull(resultDictionary);
			Assert.Equal(1, resultDictionary.Count);
			Assert.Equal(1, resultDictionary[0].Messages.Count);
			Assert.Equal(secondMessageToSave.ChatMessageId, resultDictionary[0].Messages[0].ChatMessageId);
		}

		[Fact]
		public void GetNotReceivedMessagesDictionary_UserHasTwoNotReceivedMessagesInOneConversation_MethodReturnsLastMessageByDate()
		{
			//Arrange
			var firstUser = new ApplicationUser { UserName = "First User" };
			var secondUser = new ApplicationUser { UserName = "SecondUser" };
			var conversation = new Conversation();
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = firstUser});
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = secondUser });
			var chatMessageToSave = new ChatMessage { Author = secondUser, Conversation = conversation, Received = false, Date = DateTime.Now  };
			var secondMessageToSave = new ChatMessage { Author = secondUser, Conversation = conversation, Received = false, Date = chatMessageToSave.Date.AddMinutes(1) };
			conversation.Messages.Add(chatMessageToSave);
			conversation.Messages.Add(secondMessageToSave);
			this.fixture.DbContext.Conversation.Add(conversation);
			this.fixture.DbContext.SaveChanges();


			//Act
			var resultDictionary = serviceUnderTest.GetNotReceivedMessagesDictionary(firstUser.Id);

			//Assert
			Assert.NotNull(resultDictionary);
			Assert.Equal(1, resultDictionary.Count);
			Assert.Equal(secondMessageToSave.ChatMessageId, resultDictionary[conversation.ConversationId].ChatMessageId);
		}

		[Fact]
		public void GetNotReceivedMessagesDictionary_ReturnDictionaryWithOneElement()
		{
			//Arrange
			var firstUser = new ApplicationUser { UserName = "First User" };
			var secondUser = new ApplicationUser { UserName = "SecondUser" };
			var conversation = new Conversation();
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = firstUser });
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = secondUser });
			var chatMessageToSave = new ChatMessage { Author = secondUser, Conversation = conversation, Received = false };
			conversation.Messages.Add(chatMessageToSave);
			this.fixture.DbContext.Conversation.Add(conversation);
			this.fixture.DbContext.SaveChanges();


			//Act
			var resultDictionary = serviceUnderTest.GetNotReceivedMessagesDictionary(firstUser.Id);

			//Assert
			Assert.NotNull(resultDictionary);
			Assert.Equal(1, resultDictionary.Count);
		}

		[Fact]
		public void SaveMessage_CorrectSaveEntityAndReturnIt_WithNavigationProperty_Author()
		{
			//Arrange
			var firstUser = new ApplicationUser { UserName = "First User" };
			var conversation = new Conversation();
			conversation.Users.Add(new UserToConversation { Conversation = conversation, User = firstUser });
			this.fixture.DbContext.Conversation.Add(conversation);
			this.fixture.DbContext.SaveChanges();
			var chatMessageToSave = new ChatMessage { AuthorId = firstUser.Id, ConversationId = conversation.ConversationId, Received = true };

			//Act
			var savedChatMessage = serviceUnderTest.SaveMessage(chatMessageToSave);

			//Assert
			Assert.NotNull(savedChatMessage);
			Assert.NotNull(savedChatMessage.Author);
			Assert.Equal(firstUser.UserName, savedChatMessage.Author.UserName);
			Assert.True(savedChatMessage.AuthorId == firstUser.Id);
			Assert.True(savedChatMessage.Received);
		}

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
