using AvtoPoiskTestApp.Services.Services;
using NUnit.Framework;

namespace AvtoPoiskTestApp.Tests
{
    public class EncryptionServiceTests
    {

        private EncryptionService _encryptionService;

        [SetUp]
        public void SetUp()
        {
            _encryptionService = new EncryptionService();
        }


        [Test]
        [TestCase(nameof(Encrypt_WithString_StringDecrypted))]
        [TestCase("r7V4Suwyaj")] //AN16122357
        [TestCase("P6cKp1gWD2")] //AN97294833
        public void Encrypt_WithString_StringDecrypted(string testString)
        {
            var encryptedPass = _encryptionService.Encrypt(testString);
            var decryptedPass = _encryptionService.Decrypt(encryptedPass);

            Assert.AreEqual(testString, decryptedPass);
        }
    }
}
