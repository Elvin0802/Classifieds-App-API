using ClassifiedsApp.Core.Entities.Common;

namespace ClassifiedsApp.Core.Entities;

public class Category : BaseCategory
{
	public IList<MainCategory> MainCategories { get; set; }
}
