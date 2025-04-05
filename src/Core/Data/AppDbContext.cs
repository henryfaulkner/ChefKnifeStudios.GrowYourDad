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
	public DbSet<XpLevel> XpLevels { get; set; }
	#endregion

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PcLevel>(entity =>
		{
			entity.HasNoKey(); 
			entity.ToView("PcLevel"); 
		});
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = Constants.Config.ConnectionString;
		optionsBuilder.UseSqlite(connectionString);
	}

	public void DropAndRecreateXpLevelTable()
	{
		this.Database.ExecuteSqlRaw(
			XpLevel.SqlScript
		);
	}
}
