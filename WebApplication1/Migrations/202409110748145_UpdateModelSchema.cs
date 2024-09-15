namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModelSchema : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "NomorTelepon", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "NomorTelepon", c => c.String());
        }
    }
}
