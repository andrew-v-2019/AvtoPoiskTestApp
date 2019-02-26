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
        private readonly string _initialJson;

        public PasswordProvider(string initialJson = "")
        {
            _initialJson = initialJson;
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


        private void InitialCreate()
        {
            if (string.IsNullOrWhiteSpace(_initialJson))
            {
                throw new Exception("Settings not filled");
            }
            var items = GetQueueFromJson(_initialJson);
            SaveAccounts(items);
        }

        private Queue<Account> ReadAccounts()
        {
            if (!File.Exists(_passwordsFileFullPath))
            {
                InitialCreate();
            }

            using (var r = new StreamReader(_passwordsFileFullPath))
            {
                var json = r.ReadToEnd();
                var items = GetQueueFromJson(json);
                return items;
            }
        }

        private static Queue<Account> GetQueueFromJson(string json)
        {
            var items = JsonConvert.DeserializeObject<Queue<Account>>(json);
            return items;
        }
    }
}
