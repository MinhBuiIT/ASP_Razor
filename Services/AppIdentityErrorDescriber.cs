using Microsoft.AspNetCore.Identity;

namespace Services;
public class AppIdentityErrorDescriber: IdentityErrorDescriber {
    public override IdentityError DuplicateRoleName(string role)
    {
        return new IdentityError
        {
            Code = "DuplicateRoleName",
            Description = $"Tên Role {role} đã được tạo"
        };
    }
}