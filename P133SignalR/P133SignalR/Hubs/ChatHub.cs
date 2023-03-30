using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using P133SignalR.Data;
using P133SignalR.Models;

namespace P133SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userManager = userManager;
        }

        public async Task MesajGonder(string user, string message,string group)
        {
            await Clients.Group(group).SendAsync("MesajQebulEle", $"{user} {Context.ConnectionId}", message);
        }

        public async Task AddGroup(string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public override async Task OnConnectedAsync()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);

                appUser.ConnectionId = Context.ConnectionId;

                await _userManager.UpdateAsync(appUser);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u=>u.ConnectionId == Context.ConnectionId);
            appUser.ConnectionId = null;
            await _userManager.UpdateAsync(appUser);
        }
    }
}
