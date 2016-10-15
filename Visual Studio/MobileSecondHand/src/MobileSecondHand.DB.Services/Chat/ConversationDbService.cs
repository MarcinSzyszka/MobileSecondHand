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
		DbContextOptions<MobileSecondHandContext> dbContextOptions;
		public ConversationDbService(IMobileSecondHandContextOptions mobileSecondHandContextOptions)
		{
			dbContextOptions = mobileSecondHandContextOptions.DbContextOptions;
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
			return this.dbContext.Conversation.Include(c => c.Users).ThenInclude(s => s.User).FirstOrDefault(c => c.Users.Select(u => u.UserId).Contains(userId) && c.Users.Select(u => u.UserId).Contains(addresseeId));
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

		public IDictionary<int, ChatMessage> GetNotReceivedMessagesDictionary(string userId)
		{
			var resultDictionary = new Dictionary<int, ChatMessage>();

			var groupedByConversationId = this.dbContext.ChatMessage
											.Include(m => m.Author)
											.Include(m => m.Conversation).ThenInclude(c => c.Users)
											.Where(m => !m.Received && m.AuthorId != userId && m.Conversation.Users.Any(u => u.UserId == userId))
											.GroupBy(m => m.ConversationId)
											.Select(m => new { ConversationId = m.Key, Message = m.OrderByDescending(ms => ms.Date).FirstOrDefault() })
											.ToList();

			foreach (var item in groupedByConversationId)
			{
				if (item.Message != null)
				{
					resultDictionary.Add(item.ConversationId, item.Message);
				}
			}

			return resultDictionary;
		}

		public void UpdateReceivedPropertyInNotReceivedMessages(string userId, IEnumerable<int> conversationsIds)
		{
			var messages = this.dbContext.ChatMessage
											.Where(m => conversationsIds.Contains(m.ConversationId) && !m.Received && m.AuthorId != userId)
											.ToList();

			for (int i = 0; i < messages.Count; i++)
			{
				messages[i].Received = true;
				this.dbContext.Entry(messages[i]).State = EntityState.Modified;
				if (i % 100 == 0)
				{
					SaveChangesAndRecreateContext();
				}

				this.dbContext.SaveChanges();
			}
		}

		private void SaveChangesAndRecreateContext()
		{
			this.dbContext.SaveChanges();
			this.dbContext = new MobileSecondHandContext(dbContextOptions);
		}

		public ChatMessage SaveMessage(ChatMessage messageDbModel)
		{
			if (messageDbModel.ChatMessageId > 0)
			{
				this.dbContext.Entry(messageDbModel).State = EntityState.Modified;
			}
			else
			{
				this.dbContext.Add(messageDbModel);
			}

			this.dbContext.SaveChanges();

			return this.dbContext.ChatMessage.Include(m => m.Author).FirstOrDefault(m => m.ChatMessageId == messageDbModel.ChatMessageId);
		}

		public void MarkMessageAsReceived(int messageId)
		{
			var message = this.dbContext.ChatMessage.First(m => m.ChatMessageId == messageId);
			message.Received = true;

			this.dbContext.Entry(message).State = EntityState.Modified;
			this.dbContext.SaveChanges();
		}

		public List<ConversationReadModel> GetConversationsWithLastMessage(string userId, int pageNumber)
		{
			var conversations = this.dbContext.Conversation
										.Include(c => c.Users).ThenInclude(u => u.User)
										.Include(c => c.Messages)
										.Where(c => c.Users.Any(u => u.UserId == userId) && c.Messages.Count > 0)
										.Select(c => new ConversationReadModel { ConversationId = c.ConversationId, Users = c.Users.Select(u => u.User).ToList(), Messages = c.Messages.OrderByDescending(m => m.Date).Take(1).ToList() }).ToList();
								


			return conversations.OrderByDescending(c => c.Messages.FirstOrDefault().Date).Skip(pageNumber * MESSAGES_COUNT_PER_REQUEST).Take(MESSAGES_COUNT_PER_REQUEST).ToList();
		}
	}
}
