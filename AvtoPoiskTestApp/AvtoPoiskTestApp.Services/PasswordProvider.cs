using System;
using AvtoPoiskTestApp.Models;
using System.Collections.Generic;
using System.IO;
using AvtoPoiskTestApp.Services.Interfaces;
using Newtonsoft.Json;

namespace AvtoPoiskTestApp.Services
{
    public class PasswordProvider:IPasswordProvider
    {
        private const string PasswordsFileName = "credentials.json";
        private readonly string _passwordsFileFullPath;

        public PasswordProvider()
        {
            _passwordsFileFullPath = AppDomain.CurrentDomain.BaseDirectory + PasswordsFileName;
        }
    

        public Account GetNextCredentials()
        {
            var accounts = ReadAccounts();
            var result = accounts.Dequeue();
            accounts.Enqueue(result);
            SaveAccounts(accounts);
            return result;
        }

        private void SaveAccounts(Queue<Account> accounts)
        {
            using (var file = File.CreateText(_passwordsFileFullPath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, accounts);
            }
        }


        private Queue<Account> ReadAccounts()
        {
            using (var r = new StreamReader(_passwordsFileFullPath))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<Queue<Account>>(json);
                return items;
            }
        }
    }
}
