using SQLite;
using MAUI_Data_Access.Models;

namespace MAUI_Data_Access.DataAccess
{
    public class PersonData
    {
        private SQLiteAsyncConnection database = null!;
        private readonly SemaphoreSlim initLock = new(1, 1);
        private bool initialized;

        private async Task Init()
        {
            await initLock.WaitAsync();

            try
            {
                if (initialized)
                {
                    return;
                }

                database = new SQLiteAsyncConnection(
                    DatabaseConstants.DatabasePath,
                    DatabaseConstants.Flags);

                await database.CreateTableAsync<Person>();

                initialized = true;
            }
            finally
            {
                initLock.Release();
            }
        }

        public async Task<List<Person>> GetPeopleAsync()
        {
            await Init();

            return await database.Table<Person>().ToListAsync();
        }

        public async Task<Person?> GetPersonAsync(int id)
        {
            await Init();

            return await database.Table<Person>()
                .Where(person => person.ID == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SavePersonAsync(Person person)
        {
            await Init();

            if (person.ID != 0)
            {
                return await database.UpdateAsync(person);
            }

            return await database.InsertAsync(person);
        }

        public async Task<int> DeletePersonAsync(Person person)
        {
            await Init();

            return await database.DeleteAsync(person);
        }
    }
}