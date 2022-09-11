using fromInputToDataase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data.SqlClient;



namespace fromInputToDataase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InsertIntoDB(AccountDto Model)
        {
            var sql = "INSERT INTO Account([FirstName], [LastName], [Email], [Password]) VALUES(@FirstName, @LastName, @Email, @Password)";
            SqlConnection connection = new SqlConnection("Server=tcp:afs-server.database.windows.net,1433;Initial Catalog=test;Persist Security Info=False;User ID=afs-admin;Password=autofastest1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            using (SqlCommand command = new SqlCommand(sql, connection))
            {

                {
                    command.Parameters.AddWithValue("@FirstName", Model.FirstName);
                    command.Parameters.AddWithValue("@LastName", Model.LastName);
                    command.Parameters.AddWithValue("@Email", Model.Email);
                    command.Parameters.AddWithValue("@Password", Model.Password);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Close();
                    }

                    connection.Close();
                }
            }

            return View(Model);
        }

        public IActionResult ShowDB()
        {

            List<ShowModel> list = new List<ShowModel>();
            var sql = "SELECT * FROM Account";
            SqlConnection connection = new SqlConnection("Server=tcp:afs-server.database.windows.net,1433;Initial Catalog=test;Persist Security Info=False;User ID=afs-admin;Password=autofastest1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            using(SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ShowModel show = new ShowModel()
                        {
                            Id = (int)reader["Id"],
                            FirstName = reader["FirstName"]?.ToString(),
                            LastName = reader["LastName"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Password = reader["Password"]?.ToString(),
                        };
                        list.Add(show);
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return View(list);
        }

        public IActionResult Delete(int Id)
        {
            var sql = "DELETE FROM Account WHERE Id=@Id";
            SqlConnection connection = new SqlConnection("Server=tcp:afs-server.database.windows.net,1433;Initial Catalog=test;Persist Security Info=False;User ID=afs-admin;Password=autofastest1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            using( SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@Id", Id);
                command.ExecuteNonQuery();
               
                connection.Close();
            }
            return RedirectToAction("ShowDB");
        }

        public IActionResult EditById(int Id)
        {
            AccountDto account = null;

            var sql = "SELECT * FROM Account WHERE Id=@Id";
            SqlConnection connection = new SqlConnection("Server=tcp:afs-server.database.windows.net,1433;Initial Catalog=test;Persist Security Info=False;User ID=afs-admin;Password=autofastest1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");


            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@Id", Id);

                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        account = new AccountDto
                        {
                            Id = Id,
                            FirstName = reader["FirstName"]?.ToString(),
                            LastName = reader["LastName"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Password = reader["Password"]?.ToString(),
                        };
                    }
                    reader.Close();
                }
                

                connection.Close();
            }
            return View(account);
        }

        [HttpPost]
        public IActionResult UpdateDB(AccountDto model,int Id)
        {
            var sql = "UPDATE Account SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Password=@Password WHERE Id=@Id";
            SqlConnection connection = new SqlConnection("Server=tcp:afs-server.database.windows.net,1433;Initial Catalog=test;Persist Security Info=False;User ID=afs-admin;Password=autofastest1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@FirstName", model.FirstName);
                command.Parameters.AddWithValue("@LastName", model.LastName);
                command.Parameters.AddWithValue("@Email", model.Email);
                command.Parameters.AddWithValue("@Password", model.Password);

                connection.Open();


                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Close();
                }
                connection.Close();
            }
            return RedirectToAction("ShowDB");
        }

      [HttpPost]
      public IActionResult SearchInDB(fromInputToDatabase.Models.SearchModel model)
        {
            List<AccountDto> list = new List<AccountDto>();

            var sql = "SELECT * FROM Account WHERE FirstName LIKE @FirstName OR LastName LIKE @LastName OR Email LIKE @Email OR Password LIKE @Password ";
            SqlConnection connection = new SqlConnection("Server=tcp:afs-server.database.windows.net,1433;Initial Catalog=test;Persist Security Info=False;User ID=afs-admin;Password=autofastest1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");


            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

              
                command.Parameters.AddWithValue("@FirstName", "%" + model.searchString + "%");
                command.Parameters.AddWithValue("@LastName", "%" + model.searchString + "%");
                command.Parameters.AddWithValue("@Email", "%" + model.searchString + "%");
                command.Parameters.AddWithValue("@Password", "%" + model.searchString + "%");

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AccountDto account = new AccountDto
                        {
                           Id = (int)reader["Id"],
                            FirstName = reader["FirstName"]?.ToString(),
                            LastName = reader["LastName"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Password = reader["Password"]?.ToString(),
                        };
                        list.Add(account);
                    }
                    reader.Close();
                }


                connection.Close();
            }
            return View(list);
        }
    

       
        [HttpPost]
        public IActionResult SortByFirstNameCresc()
        {
            List<ShowModel> sortedList = new List<ShowModel>();
            var sql = "SELECT * FROM Account ORDER BY FirstName";
            SqlConnection connection = new SqlConnection("Server=tcp:afs-server.database.windows.net,1433;Initial Catalog=test;Persist Security Info=False;User ID=afs-admin;Password=autofastest1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ShowModel show = new ShowModel()
                        {
                            Id = (int)reader["Id"],
                            FirstName = reader["FirstName"]?.ToString(),
                            LastName = reader["LastName"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Password = reader["Password"]?.ToString(),
                        };
                        sortedList.Add(show);
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return View(sortedList);
        }
    

        [HttpPost]
        public IActionResult SortByFirstNameDesc()
        {
            List<ShowModel> sortedList = new List<ShowModel>();
            var sql = "SELECT * FROM Account ORDER BY FirstName DESC";
            SqlConnection connection = new SqlConnection("Server=tcp:afs-server.database.windows.net,1433;Initial Catalog=test;Persist Security Info=False;User ID=afs-admin;Password=autofastest1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ShowModel show = new ShowModel()
                        {
                            Id = (int)reader["Id"],
                            FirstName = reader["FirstName"]?.ToString(),
                            LastName = reader["LastName"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Password = reader["Password"]?.ToString(),
                        };
                        sortedList.Add(show);
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return View(sortedList);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}