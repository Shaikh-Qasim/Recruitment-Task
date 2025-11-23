using System.Text.Json;
using RecruitmentTasks.Models;

namespace RecruitmentTasks.Persistence.Configuration;

public static class CategorySeedConfiguration
{
    private sealed class CategoryNode
    {
        public string Name { get; set; } = string.Empty;
        public List<CategoryNode>? Children { get; set; }
    }

    public static Category[] GetSeedData(string? filePath = null)
    {
        var path = filePath ?? Path.Combine(AppContext.BaseDirectory, "SeedData", "categories.json");
        
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Seed file not found", path);
        }

        var json = File.ReadAllText(path);
        var nodes = JsonSerializer.Deserialize<List<CategoryNode>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<CategoryNode>();

        var categories = new List<Category>();
        int currentId = 1;

        foreach (var root in nodes)
        {
            BuildCategory(root, null, 0, categories, ref currentId);
        }

        return categories.ToArray();
    }

    private static void BuildCategory(CategoryNode node, int? parentId, int level, List<Category> categories, ref int currentId)
    {
        var category = new Category
        {
            Id = currentId,
            Name = node.Name,
            ParentId = parentId,
            Level = level
        };

        categories.Add(category);
        var thisId = currentId;
        currentId++;

        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                BuildCategory(child, thisId, level + 1, categories, ref currentId);
            }
        }
    }
}
