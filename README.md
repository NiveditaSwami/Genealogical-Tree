# FamilyTree
Genealogy or Family tree application using the clean architecture and justifying the use of the same.


               +-----------------------------------+
               |         Presentation Layer         |
               |------------------------------------|
               | Genealogy.Console (Program.cs)     |
               | - Handles user input/output        |
               |- Calls Application Layer use cases |
               |- (TODO)                            |
               |  Composition Root: configures DI   |
               +------------------------------------+
                          
                             ↓ depends on                             
               +-----------------------------------+
               |       Application Layer            |
               |------------------------------------|
               | UseCases: AddFamilyMember, GetTree |
               | Services: FamilyMemberService,     |
               |            IFamilyMemberService    |
               | - Orchestrates domain operations   |
               | - Calls domain services/repositories|
               | Mapper: Mapper.cs                  |
               | Models: FamilyMemberDto, TreeDto   |
               | DI: AddApplicationServices          |
               +------------------------------------+
                            
                             ↓ depends on                             
               +-----------------------------------+
               |            Domain Layer            |
               |------------------------------------|
               | Entities: FamilyMember,            |
               |            FamilyRelationship      |
               | Interfaces: IFamilyMemberRepo      |
               | Services: FamilyTreeValidator      |
               | - Contains all business rules      |
               +------------------------------------+
                             
                             ↑ depends on                             
               +-----------------------------------+
               |          Infrastructure Layer      |
               |------------------------------------|
               | Persistence: InMemoryFamilyRepo    |
               | DI: DependencyInjection.cs         |
               +------------------------------------+

                             ↑
               +-----------------------------------+
               |           Test Layer              |
               |------------------------------------|
               | Genealogy.Tests                   |
               |  └── Application                  |
               |       └── AddFamilyMemberTests.cs |
               +-----------------------------------+
