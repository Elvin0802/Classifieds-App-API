using ClassifiedsApp.Core.Entities.Common;

namespace ClassifiedsApp.Core.Entities;

public class AdImage : BaseEntity
{
	public string Url { get; set; } = ""; // url of image in azure blob storage.
	public string BlobName { get; set; } = "";
	public int SortOrder { get; set; } = 0; // if SortOrder == 0 { this image is the main image }
	public Guid AdId { get; set; }
	public Ad Ad { get; set; }
}
