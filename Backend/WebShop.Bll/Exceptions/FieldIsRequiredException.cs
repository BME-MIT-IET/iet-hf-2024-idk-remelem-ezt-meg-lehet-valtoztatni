namespace WebShop.Bll.Exceptions;

public class FieldIsRequiredException : Exception
{
    public FieldIsRequiredException()
        : base("Hiányzó mező megadása kötelező")
    {
    }

    public FieldIsRequiredException(string message)
        : base(message)
    {
    }
}
