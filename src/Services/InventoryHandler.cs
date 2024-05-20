﻿using RPGBot.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace RPGBot.Services;

public class InventoryHandler
{
    private readonly ILogger _logger;
    private readonly RPGBotEntities _database;

    public InventoryHandler(
        ILogger<StartupService> logger,
        RPGBotEntities database
    )
    {
        _logger = logger;
        _database = database;
    }
    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        return await _database.Items.ToListAsync();
    }
    public async Task<Item> GetItemByIdAsync(int id)
    {
        return await _database.Items.FindAsync(id);
    }
    public Dictionary<Item, int> GetPlayerInventory(ulong guildId, ulong userId)
    {
        var items = _database.Inventory
            .Where(i => i.UserId == userId &&
                        i.GuildId == guildId &&
                        i.Amount != 0)
            .Select(i => new { i.Item, i.Amount })
            .ToDictionary(i => i.Item, i => i.Amount);
        return items;
    }
    public void Create(Item item)
    {
        _database.Items.Add(item);
        _database.SaveChanges();
    }
    public void Update(Item item)
    {
        _database.Items.Update(item);
        _database.SaveChanges();
    }
    public async Task Delete(int id)
    {
        var item = await _database.Items.FindAsync(id);
        if (item != null)
        {
            _database.Items.Remove(item);
            await _database.SaveChangesAsync();
        }
    }
}