using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Engine.Logging.Database.Model;

namespace Engine.Logging.Database
{
	public class EventLogContext : DbContext
	{
		public DbSet<GameInstance> GameInstances { get; set; }

		public DbSet<Player> InstancePlayers { get; set; }

		public DbSet<Event> InstanceEvents { get; set; }

		public EventLogContext(string nameOrConnectionString) : base(nameOrConnectionString)
		{
		}

		public EventLogContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection,
			contextOwnsConnection)
		{
		}

		protected override void OnModelCreating(DbModelBuilder builder)
		{
			base.OnModelCreating(builder);

			#region table mapping

			builder.Entity<GameInstance>()
				.ToTable("Instances");

			builder.Entity<Player>()
				.ToTable("InstancePlayers");

			builder.Entity<Event>()
				.ToTable("InstanceEvents");

			#endregion

			#region keys

			builder.Entity<GameInstance>()
				.HasKey(gi => gi.Id)
				.Property(g => g.Id)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			builder.Entity<Player>()
				.HasKey(p => new
				{
					p.GameId,
					p.PlayerId
				})
				.Property(p => p.PlayerId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			builder.Entity<Event>()
				.HasKey(ev => new
				{
					ev.GameId,
					ev.EventId,
				});

			//builder.Entity<EventData>()
			//	.HasKey(ed => new
			//	{
			//		ed.Event,
			//		ed.Key
			//	});

			#endregion

			#region foreign keys

			builder.Entity<Event>()
				.HasRequired(ev => ev.Game)
				.WithMany(g => g.Events);

			builder.Entity<Player>()
				.HasRequired(p => p.Game)
				.WithMany(g => g.Players);

			builder.Entity<Event>()
				.HasOptional(ev => ev.Player)
				.WithMany(p => p.Events);

			//builder.Entity<EventData>()
			//	.HasRequired(ed => ed.Event)
			//	.WithMany(ev => ev.Data)
			//	.HasForeignKey(ev => new { ev.GameId, ev.EventId });

			#endregion

		}
	}
}
