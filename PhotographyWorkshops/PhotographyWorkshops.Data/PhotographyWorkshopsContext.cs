namespace PhotographyWorkshops.Data
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class PhotographyWorkshopsContext : DbContext
    {
        // Your context has been configured to use a 'PhotographyWorkshopsContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'PhotographyWorkshops.Data.PhotographyWorkshopsContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'PhotographyWorkshopsContext' 
        // connection string in the application configuration file.
        public PhotographyWorkshopsContext()
            : base("name=PhotographyWorkshopsContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Camera> Cameras { get; set; }

        public virtual DbSet<Lens> Lenses { get; set; }

        public virtual DbSet<Photographer> Photographers { get; set; }

        public virtual DbSet<Workshop> Workshops { get; set; }

        public virtual DbSet<Accessory> Accessories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}