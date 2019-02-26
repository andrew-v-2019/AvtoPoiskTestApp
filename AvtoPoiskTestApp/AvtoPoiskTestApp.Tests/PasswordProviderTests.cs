using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AvtoPoiskTestApp.Models;
using AvtoPoiskTestApp.Services;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AvtoPoiskTestApp.Tests
{

    //Checks the Round-robin algoritm
    public class PasswordProviderTests
    {
        private const string PasswordsFileName = "credentials.json";
        private string _passwordsFileFullPath;

        private PasswordProvider _passwordProvider;

        [SetUp]
        public void SetUp()
        {
            _passwordProvider = new PasswordProvider();
            _passwordsFileFullPath = AppDomain.CurrentDomain.BaseDirectory + PasswordsFileName;
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GetNextCredentials_WithAccounts_PasswordReturnedAddedToTheEndOfFile(int count)
        {
            var json = GetJsonForTests(count);
            CreateTestFileWithAccount(json);
            var testAccounts = ReadTestAccounts();

            if (count == 0)
            {
                throw new Exception("Invalid count");
            }

            // Get second account in the list to check its shift to first place
            var secondAccount = testAccounts.First();
            if (count > 1)
            {
                secondAccount = testAccounts.GetRange(1, 1).First();
            }

            var account = _passwordProvider.GetNextCredentials();

            Assert.AreEqual(testAccounts.Count, count);
            Assert.IsNotNull(account);

            //Check the function returned correct account from the begining of the file 
            Assert.IsTrue(CheckObjectsAreEqual(account, testAccounts.First()));


            var testAccountsAfterEnqueue = ReadTestAccounts();

            //Check the items count in the list has not been changed
            Assert.AreEqual(testAccountsAfterEnqueue.Count, count);

            //Check if the returned credentials in the end of the list 
            Assert.IsTrue(CheckObjectsAreEqual(testAccountsAfterEnqueue.Last(), account));


            //Check if the second item in the first position
            Assert.IsTrue(CheckObjectsAreEqual(testAccountsAfterEnqueue.First(), secondAccount));
        }

        private static bool CheckObjectsAreEqual(object obj1, object obj2)
        {
            var obj1Json = JsonConvert.SerializeObject(obj1);
            var obj2Json = JsonConvert.SerializeObject(obj2);
            return obj1Json.Equals(obj2Json);
        }

        private void CreateTestFileWithAccount(string json)
        {
            File.WriteAllText(_passwordsFileFullPath, json);
        }

        private List<Account> ReadTestAccounts()
        {
            using (var r = new StreamReader(_passwordsFileFullPath))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<List<Account>>(json);
                return items;
            }
        }


        private static string GetJsonForTests(int count)
        {
            var accounts = new List<Account>();

            for (var i = 0; i < count; i++)
            {
                var acc = new Account()
                {
                    Name = Guid.NewGuid().ToString(),
                    Password = Guid.NewGuid().ToString()
                };
                accounts.Add(acc);
            }

            var json = JsonConvert.SerializeObject(accounts);

            return json;
        }
    }
}
