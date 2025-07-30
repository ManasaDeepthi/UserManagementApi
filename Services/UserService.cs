using System.Text.Json;
using UserManagementApi.Models;

namespace UserManagementApi.Services
{
    public class UserService
    {
        private readonly string _filePath = Path.Combine("DataFile", "users.json");
        private List<User> _users = new();

        public UserService()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                   // _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    _users = JsonSerializer.Deserialize<List<User>>(json, options) ?? new List<User>();
                }
                else
                {
                    using var client = new HttpClient();
                    var json = client.GetStringAsync("https://jsonplaceholder.typicode.com/users")
                                     .GetAwaiter().GetResult();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    _users = JsonSerializer.Deserialize<List<User>>(json, options) ?? new List<User>();
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the error as needed
                Console.WriteLine($"Error loading user data: {ex.Message}");
                _users = new List<User>();
            }
        }

        private void SaveData()
        {
            var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public List<User> GetAll() => _users;

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public User Add(User user)
        {
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
            SaveData();
            return user;
        }

        public bool Update(int id, User updatedUser)
        {
            var index = _users.FindIndex(u => u.Id == id);
            if (index == -1) return false;
            updatedUser.Id = id;
            _users[index] = updatedUser;
            SaveData();
            return true;
        }

        public bool Delete(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;
            _users.Remove(user);
            SaveData();
            return true;
        }
    }
}