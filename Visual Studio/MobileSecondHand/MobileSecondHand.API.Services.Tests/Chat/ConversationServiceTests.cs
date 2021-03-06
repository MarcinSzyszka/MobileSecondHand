﻿using System;
using System.Collections.Generic;
using MobileSecondHand.API.Models.Chat;
using MobileSecondHand.API.Services.Conversation;
using MobileSecondHand.API.Services.Photos;
using MobileSecondHand.DB.Models.Authentication;
using MobileSecondHand.DB.Models.Chat;
using MobileSecondHand.DB.Services.Chat;
using Moq;
using Xunit;

namespace MobileSecondHand.API.Services.Tests.Chat
{
	public class ConversationServiceTests
	{

		[Fact]
		public void AddMessageToConversation_ReturnCorrectMappedReadModel_WhereUserWhoAreGoingToReceiveThisMessage_WasNotSender()
		{
			//Arrange
			var saveModel = new ChatMessageSaveModel
			{
				AddresseeId = "addresseeTestId",
				SenderId = "senderTestId",
				Content = "test content",
			};

			var date = new DateTime(2016, 07, 06);
			var savedMessageDbModel = new ChatMessage
			{
				Author = new ApplicationUser { Id = saveModel.SenderId, UserName = saveModel.SenderId },
				Date = date,
				Content = saveModel.Content
			};

			this.conversationDbService.Setup(s => s.SaveMessage(It.IsAny<ChatMessage>())).Returns(savedMessageDbModel);

			//Act
			var messageReadModel = this.serviceUnderTest.AddMessageToConversation(saveModel);

			//Assert
			Assert.NotNull(messageReadModel);
			Assert.False(messageReadModel.UserWasSender);
			Assert.NotEmpty(messageReadModel.MessageContent);
			Assert.NotEmpty(messageReadModel.MessageHeader);
		}


		[Fact]
		public void GetMessages_ShouldReturnListWithOneMessage_WithAllProperties_UserWasSender()
		{
			//Arrange
			var pageNumber = 0;
			var messageId = 1;
			var conversationId = 2;
			var userId = "1234";
			var userName = "marcin.szyszka@gmail.com";
			var dbMessage = new ChatMessage();
			var content = "bla bla bla";
			dbMessage.Content = content;
			dbMessage.Author = new ApplicationUser { Id = userId, UserName = userName };
			dbMessage.AuthorId = userId;
			dbMessage.Date = new DateTime(2016, 06, 21, 15, 22, 05);
			dbMessage.ChatMessageId = messageId;
			dbMessage.ConversationId = conversationId;
			this.conversationDbService.Setup(s => s.GetMessagesInConversation(conversationId, pageNumber)).Returns(new List<ChatMessage> { dbMessage });

			//Act
			var result = serviceUnderTest.GetMessages(userId, conversationId, pageNumber);

			//Assert
			Assert.Equal(1, result.Count);
			Assert.Equal(messageId, result[0].Id);
			Assert.Equal(conversationId, result[0].ConversationId);
			Assert.Equal(content, result[0].MessageContent);
			Assert.Equal("ja, 21.06.2016 15:22:05", result[0].MessageHeader);
			Assert.True(result[0].UserWasSender);
		}

		[Fact]
		public void GetMessages_ShouldReturnListWithOneMessage_WithAllProperties_UserWasNotSender()
		{
			//Arrange
			var pageNumber = 0;
			var messageId = 1;
			var conversationId = 2;
			var userId = "1234";
			var senderId = "5678";
			var userName = "marcin.szyszka@gmail.com";
			var dbMessage = new ChatMessage();
			var content = "bla bla bla";
			dbMessage.Content = content;
			dbMessage.Author = new ApplicationUser { Id = senderId, UserName = userName };
			dbMessage.AuthorId = senderId;
			dbMessage.Date = new DateTime(2016, 06, 21, 15, 22, 05);
			dbMessage.ChatMessageId = messageId;
			dbMessage.ConversationId = conversationId;
			this.conversationDbService.Setup(s => s.GetMessagesInConversation(conversationId, pageNumber)).Returns(new List<ChatMessage> { dbMessage });

			//Act
			var result = serviceUnderTest.GetMessages(userId, conversationId, pageNumber);

			//Assert
			Assert.Equal(1, result.Count);
			Assert.Equal(messageId, result[0].Id);
			Assert.Equal(conversationId, result[0].ConversationId);
			Assert.Equal(content, result[0].MessageContent);
			Assert.Equal("marcin.szyszka, 21.06.2016 15:22:05", result[0].MessageHeader);
			Assert.False(result[0].UserWasSender);
		}
		#region CONFIGURATION
		Mock<IConversationDbService> conversationDbService;
		Mock<IPhotosService> photosService;
		IConversationService serviceUnderTest;
		public ConversationServiceTests()
		{
			this.conversationDbService = new Mock<IConversationDbService>();
			this.photosService = new Mock<IPhotosService>();
			this.serviceUnderTest = new ConversationService(conversationDbService.Object, this.photosService.Object);

		}
		#endregion
	}
}
