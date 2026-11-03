using Shop.Core.Entities;
using Shop.DAL.Repositories;

namespace Shop.Tests
{
    public class JsonRepositoryTests
    {
        private static string TempFile()
        {
            return Path.Combine(Path.GetTempPath(), "shop-tests-" + Guid.NewGuid().ToString("N") + ".json");
        }

        [Fact]
        public void JsonRepository_CreatesFileAndReloads()
        {
            string path = TempFile();
            try
            {
                JsonRepository<User> repo = new JsonRepository<User>(path);
                repo.Add(new User { Email = "one@mail.ru" });
                repo.Add(new User { Id = 20, Email = "two@mail.ru" });

                JsonRepository<User> reloaded = new JsonRepository<User>(path);
                Assert.Equal(2, reloaded.GetAll().Count);
                Assert.Equal("one@mail.ru", reloaded.GetById(1)!.Email);
                Assert.Equal(20, reloaded.GetById(20)!.Id);

                User first = reloaded.GetById(1)!;
                first.FirstName = "One";
                reloaded.Update(first);
                reloaded.Update(new User { Id = 777, Email = "nope" });
                reloaded.Delete(999);
                reloaded.Delete(20);
                Assert.Null(reloaded.GetById(20));
                Assert.Equal("One", new JsonRepository<User>(path).GetById(1)!.FirstName);
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [Fact]
        public void JsonRepository_WhenFileMissing_StartsEmpty()
        {
            string path = TempFile();
            JsonRepository<Product> repo = new JsonRepository<Product>(path);
            Assert.Empty(repo.GetAll());
            Assert.Null(repo.GetById(1));
        }

        [Fact]
        public void JsonRepository_WhenFileEmptyOrInvalid_StartsEmpty()
        {
            string emptyPath = TempFile();
            string badPath = TempFile();
            File.WriteAllText(emptyPath, "   ");
            File.WriteAllText(badPath, "{not-json");

            try
            {
                Assert.Empty(new JsonRepository<User>(emptyPath).GetAll());
                Assert.Empty(new JsonRepository<User>(badPath).GetAll());
            }
            finally
            {
                File.Delete(emptyPath);
                File.Delete(badPath);
            }
        }

        [Fact]
        public void JsonRepository_WhenFileContainsNullJson_StartsEmpty()
        {
            string path = TempFile();
            File.WriteAllText(path, "null");
            try
            {
                Assert.Empty(new JsonRepository<User>(path).GetAll());
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void JsonRepository_CreatesDirectoryIfNeeded()
        {
            string dir = Path.Combine(Path.GetTempPath(), "shop-json-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(dir, "nested", "users.json");
            try
            {
                JsonRepository<User> repo = new JsonRepository<User>(path);
                repo.Add(new User { Email = "dir@mail.ru" });
                Assert.True(File.Exists(path));
            }
            finally
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }
        }
    }
}
