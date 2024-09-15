namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModelSchema1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "NomorTelepon", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "NomorTelepon", c => c.String(nullable: false));
        }
    }
}
