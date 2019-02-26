using AvtoPoiskTestApp.Models;

namespace AvtoPoiskTestApp.Services.Interfaces
{
    public interface IPasswordProvider
    {
        Account GetNextCredentials();
    }
}
