namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeGroupMemberClass : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GroupMembers", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupMembers", "Student1Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupMembers", "Student2Id", "dbo.AspNetUsers");
            DropIndex("dbo.GroupMembers", new[] { "GroupId" });
            DropIndex("dbo.GroupMembers", new[] { "Student1Id" });
            DropIndex("dbo.GroupMembers", new[] { "Student2Id" });
            AddColumn("dbo.Groups", "Student1Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Groups", "Student2Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Groups", "Student1Id");
            CreateIndex("dbo.Groups", "Student2Id");
            AddForeignKey("dbo.Groups", "Student1Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Groups", "Student2Id", "dbo.AspNetUsers", "Id");
            DropTable("dbo.GroupMembers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        GroupMembershipId = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        Student1Id = c.String(maxLength: 128),
                        Student2Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GroupMembershipId);
            
            DropForeignKey("dbo.Groups", "Student2Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Groups", "Student1Id", "dbo.AspNetUsers");
            DropIndex("dbo.Groups", new[] { "Student2Id" });
            DropIndex("dbo.Groups", new[] { "Student1Id" });
            DropColumn("dbo.Groups", "Student2Id");
            DropColumn("dbo.Groups", "Student1Id");
            CreateIndex("dbo.GroupMembers", "Student2Id");
            CreateIndex("dbo.GroupMembers", "Student1Id");
            CreateIndex("dbo.GroupMembers", "GroupId");
            AddForeignKey("dbo.GroupMembers", "Student2Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.GroupMembers", "Student1Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.GroupMembers", "GroupId", "dbo.Groups", "Id", cascadeDelete: true);
        }
    }
}
