namespace GastroTransfer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "PSP.ProducedItems",
                c => new
                    {
                        ProducedItemId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Group = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        UnitOfMesure = c.String(),
                        ConversionRate = c.Decimal(nullable: false, precision: 18, scale: 6),
                        ExternalId = c.Int(),
                        ExternalIndex = c.String(),
                        ExternalName = c.String(),
                        ExternalUnitOfMesure = c.String(),
                    })
                .PrimaryKey(t => t.ProducedItemId);
            
            CreateTable(
                "PSP.TransferredItems",
                c => new
                    {
                        TransferredItemId = c.Int(nullable: false, identity: true),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransferType = c.Int(nullable: false),
                        IsSentToExternalSystem = c.Boolean(nullable: false),
                        Registered = c.DateTime(nullable: false),
                        SentToExternalSystem = c.DateTime(nullable: false),
                        PackageNumber = c.Int(),
                    })
                .PrimaryKey(t => t.TransferredItemId);
            
        }
        
        public override void Down()
        {
            DropTable("PSP.TransferredItems");
            DropTable("PSP.ProducedItems");
        }
    }
}
