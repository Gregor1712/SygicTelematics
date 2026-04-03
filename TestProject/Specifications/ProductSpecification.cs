using TestProject.Dbo;
using TestProject.DTO;

namespace TestProject.Specifications;

public class ProductSpecification : BaseSpecification<CpuDBO>
{
    public ProductSpecification(ProductSpecParams productParams)
        : base(x =>
            (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) &&
            //&& productParams.Name != null &&
            (string.IsNullOrEmpty(productParams.Name) || x.Name.ToLower().Contains(productParams.Name)) &&
            (string.IsNullOrEmpty(productParams.Socket) || x.Socket.ToLower().Contains(productParams.Socket)) &&
            (!productParams.Cores.HasValue || x.Cores == productParams.Cores))

            //(!productParams.Description.Any() || productParams.Description.Contains(x.Description))) // Type ??
    {
        ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

        switch (productParams.Sort)
        {
            case "nameAsc":
                AddOrderBy(x => x.Name);
                break;
            case "nameDesc":
                AddOrderByDescending(x => x.Name);
                break;
            case "socketAsc":
                AddOrderBy(x => x.Socket);
                break;
            case "socketDesc":
                AddOrderByDescending(x => x.Socket);
                break;
            case "coresAsc":
                AddOrderBy(x => x.Cores);
                break;
            case "coresDesc":
                AddOrderByDescending(x => x.Cores);
                break;
            default:
                AddOrderBy(x => x.Name);
                break;
        }
    }
}