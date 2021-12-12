using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataImporter.Web.Data.Migrations
{
    public partial class CoreEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exports_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExportGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExportId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportGroup_Exports_ExportId",
                        column: x => x.ExportId,
                        principalTable: "Exports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExportGroup_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Headers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Headers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Headers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Imports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    DisplayFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imports_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rows_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowId = table.Column<int>(type: "int", nullable: false),
                    HeaderId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cells_Headers_HeaderId",
                        column: x => x.HeaderId,
                        principalTable: "Headers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cells_Rows_RowId",
                        column: x => x.RowId,
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Exports",
                columns: new[] { "Id", "ApplicationUserId", "CreatedAt", "DisplayFileName", "Email", "EmailStatus", "Status", "StorageFileName" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(5024), "File-1.xlsx", "email1@mail.com", "Done", "Done", "fcbea8cd-1d91-4dc5-ab84-468b55495595.xlsx" },
                    { 14, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7918), "File-14.xlsx", "email14@mail.com", "Done", "Done", "c62386db-f677-470f-b201-75b0d221c986.xlsx" },
                    { 13, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7911), "File-13.xlsx", "email13@mail.com", "Done", "Done", "3e3068d6-5768-415a-ba5a-1622ac6fed1f.xlsx" },
                    { 12, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7824), "File-12.xlsx", "email12@mail.com", "Done", "Done", "e556cdb1-25f7-41cd-9f30-b0ce1551706b.xlsx" },
                    { 11, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7816), "File-11.xlsx", "email11@mail.com", "Done", "Done", "57499074-e8dc-4ddb-9a3f-d8720076a236.xlsx" },
                    { 10, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7808), "File-10.xlsx", "email10@mail.com", "Done", "Done", "156de451-47a0-4997-8d33-2292c2351218.xlsx" },
                    { 9, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7801), "File-9.xlsx", "email9@mail.com", "Done", "Done", "3624a2e2-f98d-4209-8b46-737720f03e43.xlsx" },
                    { 15, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7937), "File-15.xlsx", "email15@mail.com", "Done", "Done", "656926d6-0264-439f-86f8-e9641d32f28c.xlsx" },
                    { 7, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7786), "File-7.xlsx", "email7@mail.com", "Done", "Done", "568943b4-79b4-4459-946a-39fbe851ef49.xlsx" },
                    { 6, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7778), "File-6.xlsx", "email6@mail.com", "Done", "Done", "256c5f50-cb82-41d4-a38b-bcef93eb13ef.xlsx" },
                    { 5, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7771), "File-5.xlsx", "email5@mail.com", "Done", "Done", "10b2e232-0056-4475-8a09-3436e20e3e6e.xlsx" },
                    { 4, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7755), "File-4.xlsx", "email4@mail.com", "Done", "Done", "0524825f-1366-403d-8fb7-67c802a9069e.xlsx" },
                    { 3, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7745), "File-3.xlsx", "email3@mail.com", "Done", "Done", "901e959f-f224-4229-aee7-c92a2a14b1c1.xlsx" },
                    { 2, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7704), "File-2.xlsx", "email2@mail.com", "Done", "Done", "8b12feb7-407d-477d-ae14-8a38ccbc8ee1.xlsx" },
                    { 8, new Guid("00000000-0000-0000-0000-0000000000a1"), new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(7794), "File-8.xlsx", "email8@mail.com", "Done", "Done", "74633d3a-8fb3-4d72-afed-c81f2d141fe9.xlsx" }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "ApplicationUserId", "Name" },
                values: new object[,]
                {
                    { 13, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 13" },
                    { 12, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 12" },
                    { 11, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 11" },
                    { 10, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 10" },
                    { 9, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 9" },
                    { 8, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 8" },
                    { 4, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 4" },
                    { 6, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 6" },
                    { 5, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 5" },
                    { 3, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 3" },
                    { 2, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 2" },
                    { 1, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 1" },
                    { 14, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 14" },
                    { 7, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 7" },
                    { 15, new Guid("00000000-0000-0000-0000-0000000000a1"), "Group 15" }
                });

            migrationBuilder.InsertData(
                table: "ExportGroup",
                columns: new[] { "Id", "ExportId", "GroupId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 13, 13, 13 },
                    { 12, 12, 12 },
                    { 11, 11, 11 },
                    { 10, 10, 10 },
                    { 9, 9, 9 },
                    { 8, 8, 8 },
                    { 7, 7, 7 },
                    { 6, 6, 6 },
                    { 5, 5, 5 },
                    { 4, 4, 4 },
                    { 3, 3, 3 },
                    { 2, 2, 2 },
                    { 14, 14, 14 },
                    { 15, 15, 15 }
                });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "GroupId", "Name", "Position" },
                values: new object[,]
                {
                    { 1, 1, "Name", 1 },
                    { 2, 1, "Age", 2 },
                    { 3, 1, "Email", 3 },
                    { 4, 1, "Salary", 4 }
                });

            migrationBuilder.InsertData(
                table: "Imports",
                columns: new[] { "Id", "CreatedAt", "DisplayFileName", "GroupId", "Status", "StorageFileName" },
                values: new object[,]
                {
                    { 11, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(823), "File-11.xlsx", 1, "Done", "b4a14f7e-cd8f-481a-9dcb-897a46f3397b.xlsx" },
                    { 1, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(290), "File-1.xlsx", 1, "Done", "d003dcd8-0ea0-4ace-af41-035ca1076625.xlsx" },
                    { 2, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(654), "File-2.xlsx", 1, "Done", "cbcad55d-cfb3-46e9-a9d7-b1022adf8a7b.xlsx" },
                    { 3, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(677), "File-3.xlsx", 1, "Done", "238c64a3-60ee-4df5-8cfb-902fb5c97558.xlsx" },
                    { 4, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(684), "File-4.xlsx", 1, "Done", "1527bd53-b7de-4e12-8879-5e75938048d8.xlsx" },
                    { 10, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(815), "File-10.xlsx", 1, "Done", "02a7ed3c-a32b-4b92-adef-b09845b15742.xlsx" },
                    { 5, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(787), "File-5.xlsx", 1, "Done", "17d113f0-168e-4e0c-8f84-4a419b0c7ea8.xlsx" },
                    { 7, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(799), "File-7.xlsx", 1, "Done", "effcb46b-77bb-4e10-8831-f9ea13de3566.xlsx" },
                    { 8, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(804), "File-8.xlsx", 1, "Done", "3d18eb8e-009a-4e02-9eb1-1c9237603b68.xlsx" },
                    { 15, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(879), "File-15.xlsx", 1, "Done", "a1a105de-ea71-445f-b76b-abef07b58007.xlsx" },
                    { 14, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(873), "File-14.xlsx", 1, "Done", "31111680-ed1d-491c-87d5-18778c3215e8.xlsx" },
                    { 9, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(810), "File-9.xlsx", 1, "Done", "e69a259c-4305-49cf-83d3-59d9fc3ea553.xlsx" },
                    { 12, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(860), "File-12.xlsx", 1, "Done", "e6997c47-c582-4d90-a1e1-d23ec800d3b9.xlsx" },
                    { 6, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(794), "File-6.xlsx", 1, "Done", "52d8a50b-b9c8-48ba-9c53-654f47d33946.xlsx" },
                    { 13, new DateTime(2021, 10, 7, 22, 10, 38, 428, DateTimeKind.Local).AddTicks(866), "File-13.xlsx", 1, "Done", "f26bb57e-57e6-4ce7-882c-0a79eed56f8f.xlsx" }
                });

            migrationBuilder.InsertData(
                table: "Rows",
                columns: new[] { "Id", "CreatedAt", "GroupId" },
                values: new object[,]
                {
                    { 2, new DateTime(2021, 10, 7, 22, 10, 38, 426, DateTimeKind.Local).AddTicks(2826), 1 },
                    { 1, new DateTime(2021, 10, 7, 22, 10, 38, 424, DateTimeKind.Local).AddTicks(9435), 1 }
                });

            migrationBuilder.InsertData(
                table: "Cells",
                columns: new[] { "Id", "HeaderId", "Position", "RowId", "Value" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, "One" },
                    { 2, 2, 2, 1, "10" },
                    { 3, 3, 3, 1, "one@mail.com" },
                    { 4, 4, 4, 1, "200.11" },
                    { 5, 1, 1, 2, "Two" },
                    { 6, 2, 2, 2, "20" },
                    { 7, 3, 3, 2, "two@mail.com" },
                    { 8, 4, 4, 2, "200.20" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cells_HeaderId",
                table: "Cells",
                column: "HeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Cells_RowId",
                table: "Cells",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportGroup_ExportId",
                table: "ExportGroup",
                column: "ExportId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportGroup_GroupId",
                table: "ExportGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Exports_ApplicationUserId",
                table: "Exports",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ApplicationUserId",
                table: "Groups",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Headers_GroupId",
                table: "Headers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Imports_GroupId",
                table: "Imports",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_GroupId",
                table: "Rows",
                column: "GroupId");

            var sp = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GET_PAGINATED_CONTACTS]
    (@GroupId           INT,
    @PageIndex         INT,
    @PageSize          INT,
    @ApplicationUserId [UNIQUEIDENTIFIER] = NULL,
    @SearchText        NVARCHAR(500) = NULL,
    @SortField         NVARCHAR(500) = NULL,
    @SortOrder         NVARCHAR(500) = NULL,
    @StartDate		   DATETIME2 = NULL,
    @EndDate		   DATETIME2 = NULL)
AS
  BEGIN
    SET nocount ON
    SET @PageSize = @PageSize * (SELECT DISTINCT Count(headers.[name])
    FROM headers
    WHERE  groupid = @GroupId);
    DECLARE @sql        NVARCHAR(max),
              @paramlist  NVARCHAR(max),
              @PageOffset INT = ( ( @PageIndex - 1 ) * @PageSize );
    SET @sql='
				DECLARE @SortPivot1 TABLE (RowNum bigint);
				INSERT INTO @SortPivot1 SELECT DISTINCT TOP 100 PERCENT 
				[RowId] FROM [Cells]
				INNER JOIN [Rows] ON [Rows].[Id] = [Cells].[RowId] 
				INNER JOIN [Groups] ON [Groups].[Id] = [Rows].[GroupId] 
				WHERE [Rows].[GroupId] = @xGroupId'
    IF @ApplicationUserId IS NOT NULL
        BEGIN
        SET @sql = @sql
                       + ' AND [Groups].[ApplicationUserId] = @xApplicationUserId'
    END
    IF @SearchText IS NOT NULL
        BEGIN
        SET @sql = @sql
                       + ' AND [Cells].[Value] like ''%'' + @xSearchText + ''%'''
    END
    SET @sql=@sql + '; DECLARE @SortPivot2 TABLE (RowNum bigint, SortOrder INT NOT NULL IDENTITY(1,1));
				INSERT INTO @SortPivot2 SELECT TOP 100 PERCENT 
				[RowNum] FROM @SortPivot1
                INNER JOIN [Cells] ON [Cells].[RowId] = [RowNum]
				INNER JOIN [Rows] ON [Rows].[Id] = [Cells].[RowId]
				INNER JOIN [Headers] ON [Headers].[Id] = [Cells].[HeaderId]
				WHERE [Cells].[RowId] = [RowNum]'
    IF @StartDate IS NOT NULL
        BEGIN
        SET @sql = @sql
                       + ' AND DATEDIFF(day, @xStartDate, [Rows].[CreatedAt]) >= 0'
    END
    IF @EndDate IS NOT NULL
        BEGIN
        SET @sql = @sql
                       + ' AND DATEDIFF(day, [Rows].[CreatedAt], @xEndDate) >= 0'
    END
    IF @SortField IS NOT NULL AND @SortOrder IS NOT NULL
        BEGIN
        print(111)
        SET @sql = @sql + ' AND [Headers].[Name] = @xSortField ORDER BY [Cells].[Value] '
                       + CONVERT(NVARCHAR(500), @SortOrder)
    END
    IF @SortOrder IS NULL OR @SortField IS NULL
        BEGIN
        SET @sql = @sql
                         + ' AND [Headers].[Name] = (SELECT Top 1 [Name] from [Headers] WHERE [GroupId] = @xGroupId ORDER BY [Id])'
    END
    SET @sql = @sql + '; DECLARE @SortPivot3 TABLE (RowNum bigint, SortOrder INT NOT NULL, TotalFiltered INT);
							INSERT INTO @SortPivot3 SELECT DISTINCT RowNum, SortOrder, 0 FROM @SortPivot2 GROUP BY RowNum, SortOrder ORDER BY SortOrder;
							UPDATE @SortPivot3 SET [TotalFiltered] = (SELECT COUNT(RowNum) FROM @SortPivot3);
							SELECT [Rows].[GroupId], [CreatedAt], [RowId], [HeaderId], [Cells].[Id] AS CellId, [Headers].[Name], [Value], [Cells].[Position], [TotalFiltered]
							FROM [Cells]
							INNER JOIN [Headers] ON [Headers].[Id] = [Cells].[HeaderId]
							INNER JOIN @SortPivot3 AS [Sort] ON [Cells].[RowId] = [Sort].[RowNum]
							INNER JOIN [Rows] ON [Cells].[RowId] = [Rows].[Id]
							ORDER BY [Sort].[SortOrder], [HeaderId]
							OFFSET '
               + CONVERT(NVARCHAR(10), @PageOffset)
               + ' ROWS
							FETCH NEXT '
               + CONVERT(NVARCHAR(10), @PageSize) + ' ROWS ONLY
							';
    SELECT @paramlist = '@xGroupId [int],
						@xStartDate [datetime2],
						@xEndDate [datetime2],
						@xApplicationUserId [uniqueidentifier],
						@xSortField [nvarchar](500),
						@xSearchText [nvarchar](500),
						@xSortOrder [nvarchar](500)'
    EXEC Sp_executesql
      @sql,
      @paramlist,
      @GroupId,
	  @StartDate,
	  @EndDate,
      @ApplicationUserId,
      @SortField,
      @SearchText,
      @SortOrder
END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sp = @"DROP PROCEDURE IF EXISTS [GET_PAGINATED_CONTACTS]";
            migrationBuilder.Sql(sp);

            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "ExportGroup");

            migrationBuilder.DropTable(
                name: "Imports");

            migrationBuilder.DropTable(
                name: "Headers");

            migrationBuilder.DropTable(
                name: "Rows");

            migrationBuilder.DropTable(
                name: "Exports");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
