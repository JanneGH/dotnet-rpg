namespace dotnet_rpg.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        /// Normally this would also require adding a service, controller etc.
        /// But just to show that it is possible to seed data to the db as well an override is used.
        /// As skills have no relationships this is a nice way to see this example in action.
        /// Just to show that EF7 + .NET7 do bulkupserts/inserts/deletes so EF Core does not make a separate "add data" call for each of the objects
        /// (visible in the Migration file)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData(
                new Skill { Id = 1, Name = "Fireball", Damage = 30 },
                new Skill { Id = 2, Name = "Frenzy", Damage = 20 },
                new Skill { Id = 3, Name = "Blizzard", Damage = 50 }
            );
        }

        /// With a DbSet you are able to query and save.
        /// The name is the name of the table.
        /// Whenever you want to see a representation of your model in the database you have to add a DbSet of that model.
        public DbSet<Character> Characters => Set<Character>();

        public DbSet<User> Users => Set<User>();

        public DbSet<Weapon> Weapons => Set<Weapon>();

        public DbSet<Skill> Skills => Set<Skill>();
    }
}