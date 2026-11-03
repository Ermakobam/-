using Moq;
using Shop.BLL.DTOs;
using Shop.BLL.Security;
using Shop.BLL.Services;
using Shop.Core.Entities;
using Shop.DAL.Interfaces;

namespace Shop.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> userRepository = new Mock<IUserRepository>();
        private readonly Mock<IPasswordHasher> passwordHasher = new Mock<IPasswordHasher>();

        private UserService CreateService(bool useMockHasher = true)
        {
            if (useMockHasher)
            {
                return new UserService(userRepository.Object, passwordHasher.Object);
            }

            return new UserService(userRepository.Object);
        }

        [Fact]
        public void RegisterUser_WhenRequestNull_Throws()
        {
            UserService service = CreateService();
            Exception ex = Assert.Throws<Exception>(() => service.RegisterUser(null!));
            Assert.Equal("Данные пользователя не могут быть пустыми.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void RegisterUser_WhenEmailEmpty_Throws(string? email)
        {
            UserService service = CreateService();
            UserRegisterRequest request = new UserRegisterRequest { Email = email! };
            Exception ex = Assert.Throws<Exception>(() => service.RegisterUser(request));
            Assert.Equal("Email обязателен для заполнения.", ex.Message);
        }

        [Fact]
        public void RegisterUser_WhenEmailExists_Throws()
        {
            UserService service = CreateService();
            userRepository.Setup(r => r.GetByEmail("a@mail.ru")).Returns(new User { Email = "a@mail.ru" });
            UserRegisterRequest request = new UserRegisterRequest { Email = "a@mail.ru", Password = "x" };

            Exception ex = Assert.Throws<Exception>(() => service.RegisterUser(request));
            Assert.Contains("уже зарегистрирован", ex.Message);
        }

        [Fact]
        public void RegisterUser_WhenValid_UsesHasherAndAddsUser()
        {
            UserService service = CreateService();
            passwordHasher.Setup(h => h.Hash("secret")).Returns("hashed");
            userRepository.Setup(r => r.GetByEmail("new@mail.ru")).Returns((User?)null);

            UserRegisterRequest request = new UserRegisterRequest
            {
                Email = "new@mail.ru",
                Password = "secret",
                FirstName = "Иван",
                LastName = "Иванов",
                Phone = "123"
            };

            User created = service.RegisterUser(request);

            Assert.Equal("new@mail.ru", created.Email);
            Assert.Equal("hashed", created.PasswordHash);
            Assert.Equal("Иван", created.FirstName);
            userRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void RegisterUser_DefaultConstructor_HashesWithBase64()
        {
            UserService service = CreateService(useMockHasher: false);
            userRepository.Setup(r => r.GetByEmail("b@mail.ru")).Returns((User?)null);

            User created = service.RegisterUser(new UserRegisterRequest
            {
                Email = "b@mail.ru",
                Password = "pwd"
            });

            Assert.Equal(new Base64PasswordHasher().Hash("pwd"), created.PasswordHash);
        }

        [Fact]
        public void GetUserById_DelegatesToRepository()
        {
            User user = new User { Id = 3, Email = "c@mail.ru" };
            userRepository.Setup(r => r.GetById(3)).Returns(user);
            Assert.Equal("c@mail.ru", CreateService().GetUserById(3)!.Email);
        }

        [Fact]
        public void GetUserByEmail_DelegatesToRepository()
        {
            User user = new User { Email = "d@mail.ru" };
            userRepository.Setup(r => r.GetByEmail("d@mail.ru")).Returns(user);
            Assert.Equal("d@mail.ru", CreateService().GetUserByEmail("d@mail.ru")!.Email);
        }
    }
}
