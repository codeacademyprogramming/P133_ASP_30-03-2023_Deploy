using Microsoft.AspNetCore.Identity;
using System.Resources;

namespace P133Allup.Helpers
{
    public class IdentityErrorDescriberAz : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Reqem Mutleqdir"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "Boyuk Herif Mutleqdir"
            };
        }
    }
}
