using Genealogy.Application;
using Genealogy.Application.Config;
using Genealogy.Application.Models;
using Genealogy.Application.Services;
using Genealogy.Application.UseCases;
using Genealogy.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


internal class Program
{
    private static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
             .ConfigureLogging(logging =>
             {
                 logging.ClearProviders();
                 logging.AddConsole();
                 logging.SetMinimumLevel(LogLevel.Information);
             })
             .ConfigureServices((context, services) =>
             {
                 services.Configure<PersistenceOptions>(
                 context.Configuration.GetSection("Persistence"));
                 services.AddApplication();
                 services.AddInfrastructure();
                 services.AddTransient<Worker>();
             })
             .Build();

        var worker = host.Services.GetRequiredService<Worker>();
        await worker.RunAsync();

      
    }

    public class Worker
    {
        private readonly AddFamilyMember _addFamilyMember;
        private readonly GetFamilyTree _getFamilyTree;

        public Worker(AddFamilyMember addFamilyMember, GetFamilyTree getFamilyTree)
        {
            _addFamilyMember = addFamilyMember;
            _getFamilyTree = getFamilyTree;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("=== Genealogy Tree Console ===");
            Console.WriteLine("Choose an option below:");
            Console.WriteLine("[A] Add Member");
            Console.WriteLine("[P] Print Family Tree");
            Console.WriteLine("[Q] Quit");


            while (true)
            {
                Console.Write("\nSelect option (A/P/Q): ");
                var inputKey = Console.ReadKey(intercept: true);
                var choice = char.ToUpperInvariant(inputKey.KeyChar);
                Console.WriteLine(); // move to next line

                switch (choice)
                {
                    case 'A':
                        await HandleAddMemberAsync();
                        break;

                    case 'P':
                        await HandlePrintFamilyTreeAsync();
                        break;

                    case 'Q':
                        Console.WriteLine("Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please choose A, P, or Q.");
                        break;
                }
            }
        }

        private async Task HandleAddMemberAsync()
        {
            Console.WriteLine("\nEnter the relationship in this format:");
            Console.WriteLine("Example: Dennis C Anna");
            Console.Write("Input: ");

            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty.");
                return;
            }

            try
            {
                var familyMember = AddMember(input);
                Console.WriteLine($"Added: {familyMember.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding member: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task HandlePrintFamilyTreeAsync()
        {
            Console.Write("\nEnter the root member name: ");
            var name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }

            try
            {
                var tree = _getFamilyTree.Execute(name);
                Console.WriteLine($"\nFamily Tree for {name}:\n");
                PrintTree(tree);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to print tree: {ex.Message}");
            }

            await Task.CompletedTask;
        }


        private FamilyMemberDTO AddMember(string inputString)
        {
            // Validation of input string can be added here
            var parts = inputString.Split(' ');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Input must be in the format: 'member1 Relationship(S,C,P) member2'");
            }   
            var newMember = new AddMemberDTO
            {
                Member1 = parts[0],
                Relationship = parts[1],
                Member2 = parts[2]
            };
            return _addFamilyMember.Execute(newMember);
        }

        private void PrintTree(FamilyTreeDto node, string indent = "", bool last = true)
        {
            System.Console.Write(indent);
            System.Console.Write(last ? "└─" : "├─");
            System.Console.WriteLine(node.Name);

            if (node.Parents.Any())
                System.Console.WriteLine($"{indent}   Parents: {string.Join(", ", node.Parents)}");           
            if (node.Siblings.Any())
                System.Console.WriteLine($"{indent}   Siblings: {string.Join(", ", node.Siblings)}");
            if (node.Children.Any())
                System.Console.WriteLine($"{indent}   Children: {string.Join(", ", node.Children.Select(c => c.Name))}");
            if (node.GrandChildren.Any())
                System.Console.WriteLine($"{indent}   Grandchildren: {string.Join(", ", node.GrandChildren.Select(gc => gc.Name))}");

            // Recurse to show next generation
            for (int i = 0; i < node.Children.Count; i++)
            {
                PrintTree(node.Children[i], indent + "   ", i == node.Children.Count - 1);
            }
        }
    }
}

/* 
 TODO:
3. Write comments for non implemented stuffs
4. Reasons and scalability

 ASSUMPTIONS:
1. Format/style of the input
2. Format/style of the output
 
 
 */