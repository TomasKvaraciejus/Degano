using System;
using System.Text.RegularExpressions;

namespace Degano.Helpers
{
    internal class EmailValidator
    {
        internal bool isValid { get; set; } = false;
        private Regex regex { get; } = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        internal bool IsEmailValid(string email)
        {
            Match match = regex.Match(email);
            isValid = match.Success;
            return match.Success;
        }
    }
}
