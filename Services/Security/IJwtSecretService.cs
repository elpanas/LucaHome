namespace LucaHome.Services.Security
{
    public interface IJwtSecretService
    {
        public byte[] TakeJwtSecretFromFile();
    }
}
