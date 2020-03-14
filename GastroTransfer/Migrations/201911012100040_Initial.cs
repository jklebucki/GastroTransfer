namespace GastroTransfer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "PSP.ProducedItems",
                c => new
                    {
                        ProducedItemId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        UnitOfMesure = c.String(),
                        ConversionRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExternalId = c.Int(),
                        ExternalIndex = c.String(),
                        ExternalName = c.String(),
                        ExternalUnitOfMesure = c.String(),
                        ProductGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProducedItemId);
            
            CreateTable(
                "PSP.ProductGroups",
                c => new
                    {
                        ProductGroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                    })
                .PrimaryKey(t => t.ProductGroupId);
            
            CreateTable(
                "PSP.Production",
                c => new
                    {
                        ProductionItemId = c.Int(nullable: false, identity: true),
                        ProducedItemId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TransferType = c.Int(nullable: false),
                        IsSentToExternalSystem = c.Boolean(nullable: false),
                        Registered = c.DateTime(nullable: false),
                        SentToExternalSystem = c.DateTime(nullable: false),
                        PackageNumber = c.Int(),
                        DocumentType = c.String(),
                    })
                .PrimaryKey(t => t.ProductionItemId);
            
        }
        
        public override void Down()
        {
            DropTable("PSP.Production");
            DropTable("PSP.ProductGroups");
            DropTable("PSP.ProducedItems");
        }
    }
}
