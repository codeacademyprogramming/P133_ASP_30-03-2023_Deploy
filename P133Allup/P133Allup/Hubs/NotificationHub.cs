using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using P133Allup.Models;

namespace P133Allup.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public NotificationHub(UserManager<AppUser> userManager, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _httpContext = httpContext;

        }

        public async Task SendNotify(string group, string message)
        {
            await Clients.Group(group).SendAsync("ReciveNotify",message);
        }

        public override async Task OnConnectedAsync()
        {
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.FindByNameAsync(_httpContext.HttpContext.User.Identity.Name);

                appUser.ConnectionId = Context.ConnectionId;

                await _userManager.UpdateAsync(appUser);


                if (_httpContext.HttpContext.User.IsInRole("Admin") || _httpContext.HttpContext.User.IsInRole("SuperAdmin"))
                {
                    await Groups.AddToGroupAsync(appUser.ConnectionId, "admins");
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);

            if (appUser != null)
            {
                appUser.ConnectionId = null;

                await _userManager.UpdateAsync(appUser);
            }
        }
    }
}
