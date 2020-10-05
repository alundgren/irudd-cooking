using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace irudd_cooking.Code
{
    public class RecipeBook
    {
        public List<Recipe> Recipes { get; set; }
        public List<string> Tags { get; set; }

        public List<Recipe> GetRecipesWithTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return Recipes;
            return Recipes.Where(x => x.Tags.Contains(tag)).ToList();
        }

        public class Recipe
        {
            public string Name { get; set; }
            public string MainImageUrl { get; set; }
            public List<string> Steps { get; set; }
            public List<string> Comments { get; set; }
            public List<Ingredient> Ingredients { get; set; }
            public List<string> Tags { get; set; }
            public int? NrOfPortions { get; set; }
        }

        public class Ingredient
        {
            public string Name { get; set; }
            public string Quantity { get; set; }
            public int? Kcal { get; set; }
            public List<string> Links { get; set; }
        }

        public static RecipeBook CreateFromRecipeFolder(IDirectoryContents folder)
        {
            var b = new RecipeBook
            {
                Recipes = new List<Recipe>()
            };
            foreach (var file in folder)
            {
                if (file.Name.EndsWith(".txt"))
                {
                    using (var s = file.CreateReadStream())
                    using (var r = new StreamReader(s, Encoding.UTF8))
                    {
                        var lines = new List<string>();
                        string line;
                        while ((line = r.ReadLine()) != null)
                            lines.Add(line);

                        b.Recipes.Add(CreateFromFileLines(lines, Path.GetFileNameWithoutExtension(file.Name)));
                    }
                }
            }
            b.Tags = b.Recipes.SelectMany(x => x.Tags).Distinct().OrderBy(x => x).ToList();
            return b;
        }

        public static Recipe CreateFromFileLines(List<string> lines, string name)
        {
            var r = new Recipe
            {
                Name = name,
                Steps = new List<string>(),
                Ingredients = new List<Ingredient>(),
                Comments = new List<string>(),
                Tags = new List<string>()
            };

            string currentSection = null;

            foreach (var lineRaw in lines)
            {
                var line = lineRaw?.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }
                else if (line == "b:" || line == "t:" || line == "n:")
                    throw new Exception(name);
                else if (line == "i:" || line == "s:" || line == "k:" || line == "m:")
                {
                    currentSection = line.Substring(0, 1);
                }
                else if (currentSection == "n")
                {

                }
                else if (currentSection == "i")
                {
                    var parts = line.Split(";").Select(x => x?.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    var i = new Ingredient
                    {
                        Name = parts[0],
                        Links = new List<string>()
                    };
                    foreach (var p in parts.Skip(1))
                    {
                        if (p.StartsWith("http://", true, CultureInfo.InvariantCulture) || p.StartsWith("https://", true, CultureInfo.InvariantCulture))
                        {
                            i.Links.Add(p);
                        }
                        else if (p.ToLowerInvariant().EndsWith("kcal"))

                            i.Kcal = int.Parse(new string(p.Where(char.IsDigit).ToArray()));
                        else
                            i.Quantity = p;
                    }
                    r.Ingredients.Add(i);
                }
                else if (currentSection == "s")
                {
                    r.Steps.Add(line);
                }
                else if (currentSection == "k")
                {
                    r.Comments.Add(line);
                }
                else if (currentSection == "m")
                {
                    //Meta
                    var i = line.IndexOf('=');
                    if (i < 0)
                        continue;

                    var propertyName = line.Substring(0, i).Trim().ToLowerInvariant();
                    var propertyValue = line.Substring(i + 1).Trim();
                    if (propertyName == "bild")
                    {
                        r.MainImageUrl = propertyValue;
                    }
                    else if (propertyName == "taggar" || propertyName == "tags")
                    {
                        r.Tags.AddRange(propertyValue.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0));
                    }
                    else if (propertyName == "portioner")
                    {
                        r.NrOfPortions = int.Parse(propertyValue);
                    }
                    else if (propertyName == "namn")
                    {
                        r.Name = propertyValue;
                    }
                }
            }

            return r;
        }
    }
}