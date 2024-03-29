﻿using SQLite;
using System.Diagnostics.CodeAnalysis;

namespace Degano.SqliteDb
{
    [ExcludeFromCodeCoverage]
    public class SqliteDatabase
    {
        SQLiteAsyncConnection Database;
        public SqliteDatabase()
        {
        }
        public async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await Database.CreateTableAsync<UserResult>();
            var result2 = await Database.CreateTableAsync<Cards>();
        }

        public async Task<int> SaveItemAsync(UserResult item)
        {
            await Init();
            return await Database.InsertAsync(item);
        }

        public async Task<UserResult> GetItemAsync(string email)
        {
            await Init();
            var a = await Database.Table<UserResult>().Where(i => i.Email == email).FirstOrDefaultAsync();
            return a;
        }

        public async Task<int> UpdateItemAsync(UserResult item)
        {
            await Init();
            return await Database.UpdateAsync(item);
        }
        public async Task<int> UpdateCardAsync(Cards item)
        {
            await Init();
            return await Database.UpdateAsync(item);
        }

        public async Task<int> InsertCardAsync(Cards item)
        {
            await Init();
            return await Database.InsertAsync(item);
        }

        public async Task<List<Cards>> GetCardsAsync(string email)
        {
            await Init();
            var a = await Database.Table<Cards>().Where(i => i.Email == email).ToListAsync();
            return a;
        }

        public async Task<Cards> GetCardAsync(string email, string type)
        {
            await Init();
            return await Database.Table<Cards>().Where(i => i.Email == email && i.CardName == type).FirstOrDefaultAsync();
        }

        public async Task<int> DeleteCardAsync(string email, string type)
        {
            await Init();
            return await Database.DeleteAsync(await GetCardAsync(email, type));
        }
    }
}
