using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DapperSimpleSample
{
    class Program
    {
        public static List<string> CategoriesId { get; set; }
        
        static void Main(string[] args)
        {
            string connection =
                @"Data Source=.\SQLEXPRESS;Initial Catalog=InventoryDb;Integrated Security=True";

            using (var db = new SqlConnection(connection))
            {
                CategoriesId = new List<string>();

                InsertCategory(db);
                var categoryId = CategoriesId[0];
                GetFirstCategory(db, categoryId);
                
                UpdCategory(db, categoryId, "New name");
                GetFirstCategory(db, categoryId);

                DelCategories(db, CategoriesId);
                GetAllCategories(db);
            }
        }

        private static void InsertCategory(SqlConnection db)
        {
            var sql = "insert into Categories (CategoryId, CategoryName) values (@categoryId, @categoryName)";
            var categoryId = GenerateRandomAlphanumericString(3);
            var categoryName = GenerateRandomAlphanumericString(10);
            var result = db.Execute(sql, new { categoryId = categoryId, categoryName = categoryName });

            CategoriesId.Add(categoryId);
        }

        private static void UpdCategory(SqlConnection db, string categoryId, string categoryName)
        {
            var sql = "update Categories set CategoryName=@categoryName where CategoryId=@categoryId";
            var result = db.Execute(sql, new { categoryName = categoryName, categoryId = categoryId });
        }

        private static void GetAllCategories(SqlConnection db)
        {
            var sql = "select CategoryId, CategoryName from Categories";
            var categoryList = db.Query<Category>(sql);
            foreach (var category in categoryList)
            {
                Console.WriteLine(category.CategoryId + " - " + category.CategoryName);
            }
        }
        private static void GetFirstCategory(SqlConnection db, string categoryId)
        {
            var sql = "select CategoryId, CategoryName from Categories where CategoryId=@categoryId";
            var category = db.QueryFirst<Category>(sql, new { categoryId = categoryId });
            Console.WriteLine(category.CategoryId + " - " + category.CategoryName);
        }

        private static void DelCategories(SqlConnection db, List<string> deleteCategories)
        {
            var sql = "delete Categories where CategoryId = @categoryId";
            var result = db.Execute(sql, deleteCategories.Select(x => new { categoryId = x }).ToArray());
        }

        /// <summary>
        /// Generates a random alphanumeric string.
        /// </summary>
        /// <param name="length">The desired length of the string</param>
        /// <returns>The string which has been generated</returns>
        public static string GenerateRandomAlphanumericString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length)
                                                    .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}
