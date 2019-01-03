namespace BackupVideos
{
  public class Login
  {
    public string userId { get; set; }

    public string email { get; set; }

    public string token { get; set; }

    public string paymentId { get; set; }

    public int authenticated { get; set; }

    public string accountStatus { get; set; }

    public string serialNumber { get; set; }

    public string countryCode { get; set; }

    public bool tocUpdate { get; set; }

    public bool policyUpdate { get; set; }

    public bool validEmail { get; set; }

    public bool arlo { get; set; }

    public long dateCreated { get; set; }
  }
}
