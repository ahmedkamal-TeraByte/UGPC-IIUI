namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addGroupAndGroupMember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        GroupMembershipId = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        User1Id = c.String(maxLength: 128),
                        User2Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GroupMembershipId)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User1Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User2Id)
                .Index(t => t.GroupId)
                .Index(t => t.User1Id)
                .Index(t => t.User2Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupMembers", "User2Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupMembers", "User1Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupMembers", "GroupId", "dbo.Groups");
            DropIndex("dbo.GroupMembers", new[] { "User2Id" });
            DropIndex("dbo.GroupMembers", new[] { "User1Id" });
            DropIndex("dbo.GroupMembers", new[] { "GroupId" });
            DropTable("dbo.Groups");
            DropTable("dbo.GroupMembers");
        }
    }
}
