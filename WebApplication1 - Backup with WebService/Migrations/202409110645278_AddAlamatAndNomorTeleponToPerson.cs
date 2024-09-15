namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddAlamatAndNomorTeleponToPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "Alamat", c => c.String());
            AddColumn("dbo.People", "NomorTelepon", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.People", "NomorTelepon");
            DropColumn("dbo.People", "Alamat");
        }
    }
}
