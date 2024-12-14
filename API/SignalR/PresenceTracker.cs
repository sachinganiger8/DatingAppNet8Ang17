using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> onlineUsers = [];

    public Task<bool> UserConnected(string username, string connectionId)
    {
        var isOnline = false;
        lock (onlineUsers)
        {
            if (onlineUsers.ContainsKey(username))
                onlineUsers[username].Add(connectionId);
            else
            {
                onlineUsers.Add(username, [connectionId]);
                isOnline = true;
            }
        }
        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string username, string connectionId)
    {
        var isOffline = false;
        lock (onlineUsers)
        {
            if (!onlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

            onlineUsers[username].Remove(connectionId);

            if (onlineUsers[username].Count == 0)
            {
                onlineUsers.Remove(username);
                isOffline = true;
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsrs;
        lock (onlineUsers)
        {
            onlineUsrs = onlineUsers.OrderBy(k => k.Key).Select(s => s.Key).ToArray();
        }

        return Task.FromResult(onlineUsrs);
    }

    public static Task<List<string>> GetConnectoinsForUser(string username)
    {
        List<string> connectionIds;

        if (onlineUsers.TryGetValue(username, out var connections))
        {
            lock (connections)
            {
                connectionIds = [.. connections];
            }
        }
        else
        {
            connectionIds = [];
        }

        return Task.FromResult(connectionIds);
    }
}
