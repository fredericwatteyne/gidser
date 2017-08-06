using System;
using Microsoft.EntityFrameworkCore;

namespace Gidser.Model
{
    public class ApiContext: DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
		}

        public DbSet<Gidser.Model.Run> Run { get; set; }

	}
}
