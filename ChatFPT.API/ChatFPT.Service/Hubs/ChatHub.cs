using Microsoft.AspNetCore.SignalR;

namespace ChatFPT.Service.Hubs
{
    public class ChatHub : Hub
    {
        public void SendMessage(string questionId, string answer)
        {
            Clients.User(Context.ConnectionId).SendAsync("ReceiveAnswer", answer);
        }
    }
}
