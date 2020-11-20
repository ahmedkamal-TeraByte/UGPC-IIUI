namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addchangestabletodb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Changes",
                c => new
                    {
                        ChangeId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Changes = c.String(),
                    })
                .PrimaryKey(t => t.ChangeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Changes");
        }
    }
}
