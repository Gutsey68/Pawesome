using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Pawesome.Hubs;

/// <summary>
/// SignalR hub for real-time notification delivery
/// Manages user connections and groups for targeted notification delivery
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    /// <summary>
    /// Handles when a client connects to the notification hub
    /// Adds the user to a user-specific SignalR group for targeted notifications
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Handles when a client disconnects from the notification hub
    /// Removes the user from their user-specific SignalR group
    /// </summary>
    /// <param name="exception">The exception that caused the disconnection, if any</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
