using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using shellApi.Models;

namespace shellApi.Models
{
    public partial class ShellContext : DbContext
    {
        public ShellContext(DbContextOptions<ShellContext> options) 
            : base(options) { }
        public DbSet<Values> Values { get; set; }

    }
}