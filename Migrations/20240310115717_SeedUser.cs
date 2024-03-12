using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_RazorWeb.Migrations
{
    /// <inheritdoc />
    public partial class SeedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for(int i=1;i<=150;i++) {
                migrationBuilder.InsertData(table:"Users",
                columns: new string[]{"Id","UserName","Email","EmailConfirmed","PhoneNumberConfirmed","TwoFactorEnabled","LockoutEnabled","AccessFailedCount","HomeAddress"},
                values: new object[]{Guid.NewGuid().ToString(),"User"+i.ToString("D3"),$"user{i.ToString("D3")}@gmail.com",true,false,false,true,0,"__abc__"});
            }
        }
        /*
            [UserName]
      ,[NormalizedUserName]
      ,[Email]
      ,[NormalizedEmail]
      ,[EmailConfirmed]
      ,[PasswordHash]
      ,[SecurityStamp]
      ,[ConcurrencyStamp]
      ,[PhoneNumber]
      ,[PhoneNumberConfirmed]
      ,[TwoFactorEnabled]
      ,[LockoutEnd]
      ,[LockoutEnabled]
      ,[AccessFailedCount]
      ,[HomeAddress]
      ,[DateOfBirth]
        */
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
