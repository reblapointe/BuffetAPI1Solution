﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BuffetAPI.Models;
using BuffetAPI.Configurations;

namespace BuffetAPI.Data
{
    public class BuffetAPIContext(DbContextOptions<BuffetAPIContext> options) : DbContext(options)
    {
        public DbSet<Plat> Plat { get; set; } = default!;
        public DbSet<TypePlat> TypePlat { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TypePlatConfiguration());
            modelBuilder.ApplyConfiguration(new PlatConfiguration());
        }
    }
}
