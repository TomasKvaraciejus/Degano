using System;
using System.Text.RegularExpressions;

namespace Degano.Helpers
{
    public class EmailValidator
    {
        public bool isValid { get; set; } = false;

        public string errorMessage = "Should be valid e-mail!";
        
        private string expression = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public bool IsEmailValid(string email)
        {
            Match match = Regex.Match(email,expression);
            isValid = match.Success;
            return match.Success;
        }
    }
}
