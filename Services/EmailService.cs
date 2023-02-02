

public class EmailService: IEmailService
{
    public void SendConfirmationEmailAsync(string email, string content)
    {
        Console.WriteLine("email sent to" + email);
    }


}