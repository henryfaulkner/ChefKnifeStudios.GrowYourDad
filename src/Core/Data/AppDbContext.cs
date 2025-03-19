using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AppDbContext : DbContext
{
	public AppDbContext()
	{
		Database.EnsureCreated();
	}

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
	{ 
		Database.EnsureCreated();
	}
	
	public override int SaveChanges()
	{
		return base.SaveChanges();
	}

	#region Tables
	public DbSet<GameSave> GameSaves { get; set; }
	public DbSet<CrawlStats> CrawlStats { get; set; }
	#endregion

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = Constants.Config.ConnectionString;
		optionsBuilder.UseSqlite(connectionString);
	}
}
