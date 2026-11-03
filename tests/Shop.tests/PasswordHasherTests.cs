using Shop.BLL.DTOs;
using Shop.BLL.Security;

namespace Shop.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Base64PasswordHasher_HashesEmptyAndText()
        {
            Base64PasswordHasher hasher = new Base64PasswordHasher();
            Assert.Equal(Convert.ToBase64String(Array.Empty<byte>()), hasher.Hash(null!));
            Assert.Equal(Convert.ToBase64String(Array.Empty<byte>()), hasher.Hash(""));
            Assert.Equal(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("abc")), hasher.Hash("abc"));
        }

        [Fact]
        public void PasswordHashes_Sha256_ComputesAndAcceptsReadyHash()
        {
            string hash = PasswordHashes.Sha256("admin123");
            Assert.Equal(64, hash.Length);
            Assert.True(PasswordHashes.IsHex(hash));
            Assert.Equal(hash, PasswordHashes.Sha256(hash));
            Assert.Equal(hash, PasswordHashes.Sha256(hash.ToUpperInvariant()));
        }

        [Fact]
        public void PasswordHashes_Sha256_WhenNull_HashesEmptyString()
        {
            Assert.Equal(PasswordHashes.Sha256(""), PasswordHashes.Sha256(null!));
        }

        [Fact]
        public void PasswordHashes_IsHex_CoversBranches()
        {
            Assert.False(PasswordHashes.IsHex(null!));
            Assert.False(PasswordHashes.IsHex(""));
            Assert.False(PasswordHashes.IsHex("zz"));
            Assert.False(PasswordHashes.IsHex("0g"));
            Assert.False(PasswordHashes.IsHex("G"));
            Assert.False(PasswordHashes.IsHex(":"));
            Assert.False(PasswordHashes.IsHex("/"));
            Assert.False(PasswordHashes.IsHex("@"));
            Assert.False(PasswordHashes.IsHex("`"));
            Assert.True(PasswordHashes.IsHex("09AFaf"));
            Assert.True(PasswordHashes.IsHex("0"));
            Assert.True(PasswordHashes.IsHex("f"));
            Assert.True(PasswordHashes.IsHex("A"));
        }

        [Fact]
        public void PasswordHashes_Sha256_When64CharsNotHex_HashesAsRaw()
        {
            string raw = new string('z', 64);
            string hashed = PasswordHashes.Sha256(raw);
            Assert.NotEqual(raw, hashed);
            Assert.Equal(64, hashed.Length);
        }

        [Fact]
        public void UserRestDto_DefaultValues()
        {
            UserRestDto dto = new UserRestDto();
            Assert.Equal("", dto.Login);
            Assert.Equal("", dto.PassHash);
            dto.Login = "u";
            dto.PassHash = "p";
            Assert.Equal("u", dto.Login);
        }
    }
}
