
using Genealogy.Application.Config;
using Genealogy.Application.Models;
using Genealogy.Domain.Repositories;
using Genealogy.Application.Services;
using Genealogy.Application.UseCases;
using Genealogy.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Genealogy.Tests.Application
{
    public class AddFamilyMemberTests
    {

        private readonly Mock<IFamilyMemberRepository> _mockRepository;
        private readonly Mock<ILogger<FamilyMemberService>> _loggerMock;
        private readonly IOptions<PersistenceOptions> _options;

        private readonly FamilyMemberService _service;
        public AddFamilyMemberTests() 
        {
            _mockRepository = new Mock<IFamilyMemberRepository>();
            _loggerMock = new Mock<ILogger<FamilyMemberService>>();
            _options = Options.Create(new PersistenceOptions());

            _service = new FamilyMemberService(_mockRepository.Object, _loggerMock.Object, _options);

        }

        [Fact]
        public void AddMember_Should_Add_When_Not_Existing()
        {
            // Arrange
            var existingMembers = new List<FamilyMember>(); // repo returns empty
            _mockRepository.Setup(r => r.GetAll()).Returns(existingMembers);
            _mockRepository.Setup(r => r.Add(It.IsAny<FamilyMember>()))
                     .Callback<FamilyMember>(m => existingMembers.Add(m));
            
            var addMember = new AddMemberDTO
            {
                Member1 = "Paul",
                Member2 = "Anna",
                Relationship = "P" // Anna is parent of Paul
            };

            // Act
            var result = _service.AddFamilyMember(addMember);

            // Assert
            Assert.NotNull(result);
            Assert.True(existingMembers.Count >= 2, "At least two members should exist");
            Assert.Contains(existingMembers, m => m.Name == "Paul");
            Assert.Contains(existingMembers, m => m.Name == "Anna");

            _mockRepository.Verify(r => r.Add(It.IsAny<FamilyMember>()), Times.AtLeast(2));
        }

        [Fact]
        public void AddMember_Should_Add_MoreDepth_Relationship_Existing()
        {
            // Arrange
            var existingMembers = new List<FamilyMember>(); // repo returns empty
            _mockRepository.Setup(r => r.GetAll()).Returns(existingMembers);
            _mockRepository.Setup(r => r.Add(It.IsAny<FamilyMember>()))
                     .Callback<FamilyMember>(m => existingMembers.Add(m));

            var addMember = new AddMemberDTO
            {
                Member1 = "Mary",
                Member2 = "Anna",
                Relationship = "PS" // Anna is parent of Paul
            };

            // Act
            var result = _service.AddFamilyMember(addMember);

            // Assert
            Assert.NotNull(result);
            Assert.True(existingMembers.Count >= 3, "At least three members should exist");
            Assert.Contains(existingMembers, m => m.Name == "Mary");
            Assert.Contains(existingMembers, m => m.Name == "Anna");
            Assert.Contains(existingMembers, m => m.Name == "Mary's Parent");

            _mockRepository.Verify(r => r.Add(It.IsAny<FamilyMember>()), Times.AtLeast(3));
        }

        [Fact]
        public void GetFamily_Tree_Present()
        {
            // Arrange
            var existingMembers = new List<FamilyMember>(); // repo returns empty
            _mockRepository.Setup(r => r.GetAll()).Returns(existingMembers);
            _mockRepository.Setup(r => r.GetByName(It.IsAny<string>())).Returns((string name) => existingMembers.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)));
            _mockRepository.Setup(r => r.Add(It.IsAny<FamilyMember>()))
                     .Callback<FamilyMember>(m => existingMembers.Add(m));
            _mockRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Guid id) => existingMembers.FirstOrDefault(m=> m.Id == id));

            var addMemberList = GetAddMembers();

            FamilyMemberDTO result1;
            FamilyTreeDto result2;

            
            // Act 1 Adding the members
            foreach (var dto in addMemberList)
            {
                result1 = _service.AddFamilyMember(dto);
                Assert.NotNull(result1);
            }

            // Act 2 Get the family tree
            result2 = _service.GetFamilyTree("Anna");

            // Assert
            Assert.NotNull(result2);
            Assert.True(existingMembers.Count >= 6, "At least three members should exist");
            Assert.Contains(existingMembers, m => m.Name == "Mary");
            Assert.Contains(existingMembers, m => m.Name == "Anna");
            Assert.Contains(existingMembers, m => m.Name == "Mary's Parent");
            _mockRepository.Verify(r => r.Add(It.IsAny<FamilyMember>()), Times.Exactly(8));
        }

        private List<FamilyMemberDTO> GetFamilyMemberDTOs() 
        {
            var familyMemberList = new List<FamilyMemberDTO>
            {
                new FamilyMemberDTO
                {
                    Name = "Paul"
                },
                new FamilyMemberDTO
                {
                    Name =  "Mary"
                },
                new FamilyMemberDTO
                {
                    Name =  "Dorothy"
                },
                new FamilyMemberDTO
                {
                    Name =  "Anna"
                },
                new FamilyMemberDTO
                {
                    Name = "Anna"
                }
            };

            return familyMemberList;
        }

        private List<AddMemberDTO> GetAddMembers()
        {
            var addMemberList = new List<AddMemberDTO>
            {
                new AddMemberDTO
                {
                    Member1 = "Paul",
                    Member2 = "Anna",
                    Relationship = "P" // Anna is parent of Paul
                },
                new AddMemberDTO
                {
                    Member1 = "Mary",
                    Member2 = "Dennis",
                    Relationship = "C" // Mary has a child Dennis
                },
                new AddMemberDTO
                {
                    Member1 = "Dorothy",
                    Member2 = "Dennis",
                    Relationship = "S" // Dorothy and Dennis are siblings
                },
                new AddMemberDTO
                {
                    Member1 = "Mary",
                    Member2 = "Anna",
                    Relationship = "PS" // Anna has another child Linda
                },
                new AddMemberDTO
                {
                    Member1 = "Anna",
                    Member2 = "Roni",
                    Relationship = "CCS" // Robert has a child Emma
                }
            };
            return addMemberList;
        }

        private List<FamilyMember> GetFamilyMember()
        {
            var familyMemberList = new List<FamilyMember>
            {
                new FamilyMember
                {
                    Name = "Paul",
                    ParentIds = new List<Guid>(1)
                },
                new FamilyMember
                {
                    Name =  "Mary",
                    ChildrenIds = new List<Guid>(1)
                },
                new FamilyMember
                {
                    Name =  "Dorothy",
                    SiblingIds = new List<Guid>(1)
                },
                new FamilyMember
                {
                    Name =  "Anna",
                    ChildrenIds = new List <Guid>(1)
                },
                new FamilyMember
                {
                    Name = "Mary's Parent",
                    SiblingIds = new List<Guid>(1)
                }
            };

            return familyMemberList;
        }
    }
}
