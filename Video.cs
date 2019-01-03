namespace BackupVideos
{
  public class Video
  {
    public string ownerId { get; set; }

    public string uniqueId { get; set; }

    public string deviceId { get; set; }

    public string createdDate { get; set; }

    public string currentState { get; set; }

    public string name { get; set; }

    public string contentType { get; set; }

    public string reason { get; set; }

    public string createdBy { get; set; }

    public object lastModified { get; set; }

    public object localCreatedDate { get; set; }

    public string presignedContentUrl { get; set; }

    public string presignedThumbnailUrl { get; set; }

    public object utcCreatedDate { get; set; }

    public string timeZone { get; set; }

    public string mediaDuration { get; set; }

    public int mediaDurationSecond { get; set; }

    public bool donated { get; set; }
  }
}
