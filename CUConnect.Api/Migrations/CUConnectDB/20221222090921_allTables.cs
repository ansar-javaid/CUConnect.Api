using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CUConnect.Api.Migrations.CUConnectDB
{
    public partial class allTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoleAspNetUser",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleAspNetUser", x => new { x.RoleId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    UserJoined = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ClassID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ClassID);
                    table.ForeignKey(
                        name: "FK_Classes.DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Department",
                        principalColumn: "DepartmentID");
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    EnrollmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ClassID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.EnrollmentID);
                    table.ForeignKey(
                        name: "FK_Enrollments.ClassID",
                        column: x => x.ClassID,
                        principalTable: "Classes",
                        principalColumn: "ClassID");
                    table.ForeignKey(
                        name: "FK_Enrollments.UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ProfileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: true),
                    ClassID = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ProfileID);
                    table.ForeignKey(
                        name: "FK_Profile.ClassID",
                        column: x => x.ClassID,
                        principalTable: "Classes",
                        principalColumn: "ClassID");
                    table.ForeignKey(
                        name: "FK_Profile.DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Department",
                        principalColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK_Profile.UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostID);
                    table.ForeignKey(
                        name: "FK_Posts.ProfileID",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscritionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subscrip__94D8EB0A214B8F81", x => x.SubscritionID);
                    table.ForeignKey(
                        name: "FK_Subscriptions.ProfileID",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                    table.ForeignKey(
                        name: "FK_Subscriptions.UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostsID = table.Column<int>(type: "int", nullable: true),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FamilyType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Documents.PostsID",
                        column: x => x.PostsID,
                        principalTable: "Posts",
                        principalColumn: "PostID");
                    table.ForeignKey(
                        name: "FK_Documents.ProfileID",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    ReactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostsID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ReactinType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.ReactionID);
                    table.ForeignKey(
                        name: "FK_Reactions.PostsID",
                        column: x => x.PostsID,
                        principalTable: "Posts",
                        principalColumn: "PostID");
                    table.ForeignKey(
                        name: "FK_Reactions.UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "([NormalizedName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "([NormalizedUserName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_DepartmentID",
                table: "Classes",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PostsID",
                table: "Documents",
                column: "PostsID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ProfileID",
                table: "Documents",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ClassID",
                table: "Enrollments",
                column: "ClassID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserID",
                table: "Enrollments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ProfileID",
                table: "Posts",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_ClassID",
                table: "Profile",
                column: "ClassID");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_DepartmentID",
                table: "Profile",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_UserID",
                table: "Profile",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_PostsID",
                table: "Reactions",
                column: "PostsID");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserID",
                table: "Reactions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ProfileID",
                table: "Subscriptions",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserID",
                table: "Subscriptions",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleAspNetUser");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
