using System;
using AvtoPoiskTestApp.Models;
using System.Collections.Generic;
using AvtoPoiskTestApp.Services.Interfaces;

namespace AvtoPoiskTestApp.Services
{
    public class PasswordProvider : IPasswordProvider
    {
        private readonly IFileService _fileService;
        private const string PasswordsFileName = "credentials.json";
       
        public PasswordProvider(IFileService fileService)
        {
            _fileService = fileService;
        }

        public string GetPasswordFileName()
        {
            var path  = AppDomain.CurrentDomain.BaseDirectory + PasswordsFileName;
            return path;
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
            _fileService.SaveToFile(accounts, GetPasswordFileName());
        }

        private Queue<Account> ReadAccounts()
        {
            var accounts = _fileService.ReadFromFile<Queue<Account>>(GetPasswordFileName());
            return accounts;
        }
    }
}
