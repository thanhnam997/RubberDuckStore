using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace RubberDuckStore.Pages
{
    public class RubberDucksModel : PageModel
    {
        [BindProperty]
        public int SelectedDuckId { get; set; }

        public List<SelectListItem> DuckList { get; set; }
        public Duck SelectedDuck { get; set; }

        public void OnGet()
        {
            LoadDuckList();
        }

        public IActionResult OnPost()
        {
            LoadDuckList();
            if (SelectedDuckId != 0)
            {
                SelectedDuck = GetDuckById(SelectedDuckId);
            }
            return Page();
        }

        private void LoadDuckList()
        {
            DuckList = new List<SelectListItem>();
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Ducks";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DuckList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
        }

        private Duck GetDuckById(int id)
        {
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Ducks WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Duck
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageFileName = reader.GetString(4)
                        };
                    }
                }
            }
            return null;
        }
    }

    public class Duck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageFileName { get; set; }
    }
}

    
