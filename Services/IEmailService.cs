

public interface IEmailService
{

    void SendConfirmationEmailAsync(string email, string content);

}