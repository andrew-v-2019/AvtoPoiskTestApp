using AvtoPoiskTestApp.Services.Interfaces;
using AvtoPoiskTestApp.Services.Services;
using Unity;

namespace AvtoPoiskTestApp.Services.Unity
{
    public static class ServiceInjector
    {
        private static readonly UnityContainer UnityContainer = new UnityContainer();

        public static void Register<I, T>() where T : I
        {
            UnityContainer.RegisterType<I, T>(new ContainerControlledLifetimeManager());
        }

        public static void InjectStub<I>(I instance)
        {
            UnityContainer.RegisterInstance(instance, new ContainerControlledLifetimeManager());
        }

        public static T Retrieve<T>()
        {
            return UnityContainer.Resolve<T>();
        }

        public static void ConfigureServices()
        {
            Register<IFileService, FileService>();
            Register<IPasswordProvider, PasswordProvider>();
            Register<IEncryptionService, EncryptionService>();
        }
    }
}
