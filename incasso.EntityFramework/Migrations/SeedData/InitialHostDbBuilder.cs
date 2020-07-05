using EntityFramework.DynamicFilters;
using Incasso.EntityFramework;

namespace Incasso.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly IncassoDbContext _context;

        public InitialHostDbBuilder(IncassoDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
