namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeNamesinGroupMember : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.GroupMembers", name: "User1Id", newName: "Student1Id");
            RenameColumn(table: "dbo.GroupMembers", name: "User2Id", newName: "Student2Id");
            RenameIndex(table: "dbo.GroupMembers", name: "IX_User1Id", newName: "IX_Student1Id");
            RenameIndex(table: "dbo.GroupMembers", name: "IX_User2Id", newName: "IX_Student2Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.GroupMembers", name: "IX_Student2Id", newName: "IX_User2Id");
            RenameIndex(table: "dbo.GroupMembers", name: "IX_Student1Id", newName: "IX_User1Id");
            RenameColumn(table: "dbo.GroupMembers", name: "Student2Id", newName: "User2Id");
            RenameColumn(table: "dbo.GroupMembers", name: "Student1Id", newName: "User1Id");
        }
    }
}
