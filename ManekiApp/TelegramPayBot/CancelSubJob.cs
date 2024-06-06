namespace ManekiApp.TelegramPayBot;

public class CancelSubJob
{
    private readonly ILogger<CancelSubJob> _logger;

    public CancelSubJob(ILogger<CancelSubJob> logger)
    {
        _logger = logger;
        
    }

    public void DoSmth()
    {
        _logger.LogInformation("I did smth");
    }
}