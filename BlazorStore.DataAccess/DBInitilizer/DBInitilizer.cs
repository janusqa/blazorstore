
using BlazorStore.Common;
using BlazorStore.DataAccess.Data;
using BlazorStore.DataAccess.UnitOfWork.IUnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BlazorStore.DataAccess.DBInitilizer
{
    public class DBInitilizer : IDBInitilizer
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _uow;
        // private readonly RoleManager<IdentityRole> _rm;

        public DBInitilizer(
            ApplicationDbContext db,
            IUnitOfWork uow
        // RoleManager<IdentityRole> rm
        )
        {
            _db = db;
            _uow = uow;
            // _rm = rm;
        }

        public async Task Initilize()
        {
            // 1. Run any unapplied migrations
            // ****
            try
            {
                if (_db.Database.GetPendingMigrations().Any()) await _db.Database.MigrateAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration Error: {ex.Message}");
            }

            // 2. Create Triggers
            // ****
            var triggers = new List<string>();

            foreach (var table in SD.dbTables)
            {
                triggers.Add($@"
                    DROP TRIGGER IF EXISTS {table}_update_updated_date;
                    CREATE TRIGGER {table}_update_updated_date 
                    AFTER UPDATE ON {table}
                    BEGIN
                        UPDATE {table} 
                        SET 
                            UpdatedDate = datetime('now') 
                        WHERE id = NEW.id;
                    END;      
                ");
            }

            var transaction = _uow.Transaction();
            // await _uow.Categories.ExecuteSqlAsync(@$"USE {SD.dbName};", []); 
            foreach (var trigger in triggers)
            {
                await _uow.Categories.ExecuteSqlAsync(trigger, []);
            }
            transaction.Commit();

            // 3. Create Roles if the do not already exist
            // ****
            // var roles = new List<string> {
            //     SD.Role_Customer,
            //     SD.Role_Admin,
            //     SD.Role_Employee,
            // };

            // var rolesToCreate = new List<string>();
            // foreach (var role in roles)
            // {
            //     if (!await _rm.RoleExistsAsync(role))
            //     {
            //         rolesToCreate.Add(role);
            //     }
            // }

            // foreach (var task in rolesToCreate)
            // {
            //     await _rm.CreateAsync(new IdentityRole(task));
            // }

            return;
        }
    }
}