namespace Gamepangin
{
    public static class Database
    {
        public static T Get<T>() where T : class, IRepository, new()
        {
            return TRepository<T>.Get;
        }
    }
}
