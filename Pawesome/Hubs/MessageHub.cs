using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Pawesome.Hubs;

/// <summary>
/// SignalR hub for real-time messaging functionality
/// </summary>
[Authorize]
public class MessageHub : Hub
{
    /// <summary>
    /// Handles when a client connects to the hub
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
    /// Handles when a client disconnects from the hub
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