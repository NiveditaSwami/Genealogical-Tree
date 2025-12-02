using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Application.Config
{
    /// <summary>
    /// Used to configure persistence options like repository type
    /// </summary>
    public class PersistenceOptions
    {
        public string RepositoryType { get; set; } = "InMemory";
    }
}
