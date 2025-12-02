using Genealogy.Application.Models;
using Genealogy.Application.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Application.UseCases
{
    /// <summary>
    /// Get family tree for a member
    /// </summary>
    public class GetFamilyTree
    {
        private readonly IFamilyMemberService _service;
        private readonly ILogger<AddFamilyMember> _logger;

        public GetFamilyTree(
           IFamilyMemberService service,
           ILogger<AddFamilyMember> logger)
        {
            _service = service;
            _logger = logger;
        }

        public FamilyTreeDto Execute(string name)
        {
            // Validate input
            if (name.Trim().Length <= 0)
            {
                _logger.LogWarning("Name cannot be empty.");
                throw new ArgumentException("Name cannot be empty.");
            }
            return _service.GetFamilyTree(name);
        }
    }
}
