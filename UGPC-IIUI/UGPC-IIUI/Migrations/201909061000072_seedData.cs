namespace UGPC_IIUI.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class seedData : DbMigration
    {
        public override void Up()
        {
            Sql(@"
SET IDENTITY_INSERT [dbo].[Departments] ON
INSERT INTO [dbo].[Departments] ([DepartmentId], [Name]) VALUES (1, N'Software Engineering')
INSERT INTO [dbo].[Departments] ([DepartmentId], [Name]) VALUES (2, N'Computer Science')
INSERT INTO [dbo].[Departments] ([DepartmentId], [Name]) VALUES (3, N'Information Technology')
SET IDENTITY_INSERT [dbo].[Departments] OFF
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'0b121177-65d6-41a8-8f24-78994ba2cc98', N'Admin')
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'3f691159-9ff5-47e8-8856-58be5ae3687c', N'Committee Incharge')
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'544d4a76-c73e-47b6-a958-3f4a89428f03', N'Committee Member')
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'3a84d7fb-9854-4e9e-88e0-bf474e8d9330', N'Student')
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'08c62938-52be-4e4a-abab-b7937a1daecc', N'Supervisor')
INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [DepartmentId], [Name], [StudentId], [ProfessorId], [Joined]) VALUES (N'caf5d3b0-e857-421f-827d-1d73e7900c4c', N'admin123@ugpc.com', 0, N'APyvSOTuwfbfl9qp5fB+ybYdfQOHujTa894gHcHpulbNxhvwlHahaM8peY57WXEKHQ==', N'a55774c1-7d28-4d3d-b81b-e57acbe99536', NULL, 0, 0, NULL, 1, 0, N'admin123', 1, N'Admin', NULL, NULL, 0)
INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'caf5d3b0-e857-421f-827d-1d73e7900c4c', N'0b121177-65d6-41a8-8f24-78994ba2cc98')

");
        }

        public override void Down()
        {
        }
    }
}
