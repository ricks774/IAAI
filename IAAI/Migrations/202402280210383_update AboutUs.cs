namespace IAAI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAboutUs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Abouts", "Certified", c => c.String());
            AddColumn("dbo.Abouts", "Expert", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Abouts", "Expert");
            DropColumn("dbo.Abouts", "Certified");
        }
    }
}
