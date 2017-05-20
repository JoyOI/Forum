using Microsoft.AspNetCore.SignalR;

namespace JoyOI.Forum.Hubs
{
    public class ForumHub : Hub
    {
        public void JoinGroup(string name)
        {
            Groups.Add(Context.ConnectionId, name);
        }
    }
}
