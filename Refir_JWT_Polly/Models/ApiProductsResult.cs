using System.Collections.Generic;

namespace Refir_JWT_Polly.Models
{
    public class ApiProductsResult
    {
        public string Action { get; set; }
        public bool Success { get; set; }
        public List<string> Inconsistences { get; set; }
    }
}
