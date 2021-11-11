namespace GastroTransfer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OperationType : DbMigration
    {
        public override void Up()
        {
            AddColumn("PSP.Production", "OperationType", c => c.Int());
            CreateIndex("PSP.Production", "ProducedItemId", name: "idx_product");
            CreateIndex("PSP.Production", "IsSentToExternalSystem", name: "idx_isSent");
            CreateIndex("PSP.Production", "Registered", name: "idx_registerDate");
            CreateIndex("PSP.Production", "SentToExternalSystem", name: "idx_sentDate");
        }

        public override void Down()
        {
            DropIndex("PSP.Production", "idx_sentDate");
            DropIndex("PSP.Production", "idx_registerDate");
            DropIndex("PSP.Production", "idx_isSent");
            DropIndex("PSP.Production", "idx_product");
            DropColumn("PSP.Production", "OperationType");
        }
    }
}
