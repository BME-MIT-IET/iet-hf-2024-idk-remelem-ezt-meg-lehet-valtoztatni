namespace WebShop.Bll.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
        : base("A keresett entitás nem található")
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }
}
