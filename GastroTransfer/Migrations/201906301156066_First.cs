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
                        ConversionRate = c.Decimal(nullable: false, precision: 18, scale: 4, defaultValue: 1),
                        ExternalId = c.Int(),
                        ExternalIndex = c.String(),
                        ExternalName = c.String(),
                        ExternalUnitOfMesure = c.String(),
                        ProductGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProducedItemId)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupID, cascadeDelete: true)
                .Index(t => t.ProductGroupID);
            
            CreateTable(
                "dbo.ProductGroups",
                c => new
                    {
                        ProductGroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                    })
                .PrimaryKey(t => t.ProductGroupId);
            
            CreateTable(
                "PSP.TransferredItems",
                c => new
                    {
                        TransferredItemId = c.Int(nullable: false, identity: true),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
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
            DropForeignKey("PSP.ProducedItems", "ProductGroupID", "dbo.ProductGroups");
            DropIndex("PSP.ProducedItems", new[] { "ProductGroupID" });
            DropTable("PSP.TransferredItems");
            DropTable("dbo.ProductGroups");
            DropTable("PSP.ProducedItems");
        }
    }
}
