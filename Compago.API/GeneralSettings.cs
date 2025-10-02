using System.Security;

namespace Compago.API
{
    public class GeneralSettings
    {
        private bool? AuthorizationActiveValue = null;
        public bool AuthorizationActive
        {
            set { AuthorizationActiveValue = value; }
            get
            {
                return AuthorizationActiveValue ?? throw new SecurityException(
                $"{nameof(GeneralSettings)}: {nameof(AuthorizationActive)} is missing/invalid");
            }
        }
    }
}
