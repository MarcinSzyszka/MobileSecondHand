using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobileSecondHand.COMMON;
using MobileSecondHand.DB.Models.Chat;

namespace MobileSecondHand.DB.Services.Chat
{
	public class ConversationDbService : IConversationDbService
	{
		const int MESSAGES_COUNT_PER_REQUEST = 20;
		MobileSecondHandContext dbContext;

		public ConversationDbService(IMobileSecondHandContextOptions mobileSecondHandContextOptions)
		{
			this.dbContext = new MobileSecondHandContext(mobileSecondHandContextOptions.DbContextOptions);
		}

		public Conversation CreateConversation(string userId, string addresseeId)
		{
			var conversation = new Conversation();
			conversation.Users.Add(new UserToConversation { Conversation = conversation, UserId = userId });
			conversation.Users.Add(new UserToConversation { Conversation = conversation, UserId = addresseeId });

			this.dbContext.Conversation.Add(conversation);
			this.dbContext.SaveChanges();

			return conversation;
		}

		public Models.Chat.Conversation GetConversationByUsers(string userId, string addresseeId)
		{
			return this.dbContext.Conversation.Include(c => c.Users).FirstOrDefault(c => c.Users.Select(u => u.UserId).Contains(userId) && c.Users.Select(u => u.UserId).Contains(addresseeId));
		}

		public List<ChatMessage> GetMessagesInConversation(int conversationId, int pageNumber)
		{
			var messagesQuery = this.dbContext.ChatMessage
												.Include(c => c.Author)
												.Where(c => c.ConversationId == conversationId)
												.OrderByDescending(c => c.Date)
												.Skip(MESSAGES_COUNT_PER_REQUEST * pageNumber)
												.Take(MESSAGES_COUNT_PER_REQUEST);

			return messagesQuery.ToList();
		}
	}
}
